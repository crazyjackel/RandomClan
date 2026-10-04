using HarmonyLib;
using Random_Clan.Plugin.Copying;
using Random_Clan.Plugin.Extensions;
using Random_Clan.Plugin.Modifications;
using Random_Clan.Plugin.Modifications.Champions;
using Random_Clan.Plugin.Snapshots;
using TrainworksReloaded.Base;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.CardUpgrade;
using TrainworksReloaded.Base.Class;
using TrainworksReloaded.Base.StatusEffects;
using TrainworksReloaded.Base.Trait;

namespace Random_Clan.Plugin
{
    public sealed class ClanRandomizer
    {
        private readonly CardDataRegister _cards;
        private readonly CardTraitDataRegister _traits;
        private readonly ClassDataRegister _classes;
        private readonly CardUpgradeRegister _upgrades;
        private readonly StatusEffectDataRegister _statuses;
        private readonly CardPoolRegister _cardPools;
        private readonly GameDataClient _client;
        private readonly ModificationRegistry _modifications;
        private readonly CardDonorCopier _copier;

        private readonly Dictionary<string, CardSnapshot> _snapshots = new();
        private int _lastSeed = int.MinValue;

        public ClanRandomizer(
            CardDataRegister cards,
            CardTraitDataRegister traits,
            ClassDataRegister classes,
            CardUpgradeRegister upgrades,
            StatusEffectDataRegister statuses,
            CardPoolRegister cardPools,
            GameDataClient client,
            ModificationRegistry modifications,
            CardDonorCopier copier)
        {
            _cards = cards;
            _traits = traits;
            _classes = classes;
            _upgrades = upgrades;
            _statuses = statuses;
            _cardPools = cardPools;
            _client = client;
            _modifications = modifications;
            _copier = copier;
        }

        public void Randomize(SaveManager saveManager)
        {
            var all = saveManager.GetAllGameData();
            if (all == null)
                return;

            if (_snapshots.Count == 0)
            {
                foreach (var card in all.GetAllCardData())
                {
                    if (!card.HasRandomized())
                        continue;
                    _snapshots[card.name] = CardSnapshot.Capture(card);
                }
                Plugin.Logger.LogInfo($"Captured {_snapshots.Count} Randomized card snapshots.");
            }

            var slots = all.GetAllCardData().Where(c => c.HasRandomized()).ToList();
            if (slots.Count == 0)
                return;

            var seed = ReadRunSeed(saveManager);
            if (seed == _lastSeed)
                return;

            foreach (var slot in slots)
            {
                if (_snapshots.TryGetValue(slot.name, out var snapshot))
                    snapshot.Restore(slot);
            }

            var rng = new Random(seed);
            var starterCards = BuildStarterSet(all);
            var donors = BuildDonorPools(all, starterCards);
            var upgradePool = BuildChampionUpgradePool(all);

            var ctx = new RandomizeContext(saveManager, _cards, _traits, _classes, _upgrades, _statuses, _client)
            {
                ChampionUpgradePool = upgradePool,
            };

            Plugin.Logger.LogInfo(
                $"Random Clan seed={seed} slots={slots.Count} starterDonors={donors.Starters.Count} " +
                $"spellDonors={donors.ByKey.Count(kv => kv.Key.Type == CardType.Spell)} " +
                $"unitDonors={donors.ByKey.Count(kv => kv.Key.Type == CardType.Monster)} " +
                $"roomDonors={donors.ByKey.Count(kv => kv.Key.Type == CardType.TrainRoomAttachment)} " +
                $"champDonors={donors.Champions.Count}");

            foreach (var slot in slots)
            {
                var markers = slot.GetMarkerTraits();
                var pool = SelectPool(slot, donors, starterCards);
                if (pool.Count == 0)
                {
                    Plugin.Logger.LogWarning($"No donors for {slot.name} ({DescribeSlot(slot, starterCards)})");
                    continue;
                }

                var donor = pool[rng.Next(pool.Count)];
                Plugin.Logger.LogInfo($"{slot.name} <- {donor.name} [{DescribeSlot(slot, starterCards)}]");
                _copier.CopyOnto(slot, donor, markers);

                ctx.Character = slot.GetSpawnCharacterData();

                var remainingMods = slot.GetModifierCount();
                if (slot.IsChampionCard())
                {
                    // Champion Chaos always clears placeholder paths first, then rebuilds from other clans.
                    var treeMod = new ChampionUpgradeTreeModification();
                    if (treeMod.CanModify(slot, ctx))
                    {
                        treeMod.Modify(slot, ctx, rng);
                        Plugin.Logger.LogInfo($"  mod {nameof(ChampionUpgradeTreeModification)} (guaranteed first)");
                        remainingMods = Math.Max(0, remainingMods - 1);
                    }
                    else
                    {
                        ChampionUpgradeTreeModification.ClearUpgradeTrees(slot, ctx);
                        Plugin.Logger.LogInfo("  cleared champion upgrade trees (no donor pool)");
                    }
                }

                foreach (var mod in _modifications.Pick(
                             slot,
                             ctx,
                             rng,
                             remainingMods,
                             excludeType: slot.IsChampionCard() ? typeof(ChampionUpgradeTreeModification) : null))
                {
                    mod.Modify(slot, ctx, rng);
                    Plugin.Logger.LogInfo($"  mod {mod.GetType().Name}");
                }
            }

            _lastSeed = seed;
            Plugin.Logger.LogInfo("Random Clan randomization complete.");
        }

        private static string DescribeSlot(CardData slot, HashSet<CardData> starters)
        {
            var type = slot.GetCardTypeValue();
            var rarity = slot.GetRarityValue();
            var starter = starters.Contains(slot);
            var banner = slot.IsBannerUnitCard();
            return $"{type}/{rarity}" + (starter ? "/starter" : "") + (banner ? "/banner" : "");
        }

        private static List<CardData> SelectPool(CardData slot, DonorPools donors, HashSet<CardData> starters)
        {
            var type = slot.GetCardTypeValue();
            var rarity = slot.GetRarityValue();

            if (type == CardType.Monster && rarity == CollectableRarity.Champion)
                return donors.Champions;

            if (starters.Contains(slot) || rarity == CollectableRarity.Starter)
                return donors.Starters;

            var key = new DonorKey(type, rarity, slot.IsBannerUnitCard());
            if (donors.ByKey.TryGetValue(key, out var pool) && pool.Count > 0)
                return pool;

            // Soft fallback: same type+rarity ignoring banner flag.
            key = new DonorKey(type, rarity, !key.Banner);
            if (donors.ByKey.TryGetValue(key, out pool) && pool.Count > 0)
                return pool;

            return [];
        }

        private HashSet<CardData> BuildStarterSet(AllGameData all)
        {
            var starters = new HashSet<CardData>();

            foreach (var classData in all.GetAllClassDatas())
            {
                var champions = AccessTools.Field(typeof(ClassData), "champions")?.GetValue(classData) as List<ChampionData>;
                if (champions == null)
                    continue;
                foreach (var champion in champions)
                {
                    if (champion.starterCardData != null)
                        starters.Add(champion.starterCardData);
                }
            }

            if (_cardPools.TryGetValue("StarterCardsOnly", out var starterPool) && starterPool != null)
            {
                var into = new HashSet<CardData>();
                starterPool.CollectAllCards(into);
                foreach (var card in into)
                    starters.Add(card);
            }

            return starters;
        }

        private static DonorPools BuildDonorPools(AllGameData all, HashSet<CardData> starters)
        {
            var pools = new DonorPools();
            foreach (var card in all.GetAllCardData())
            {
                if (card == null || card.HasRandomized() || card.IsUnitAbility() || card.IsRoomAbility())
                    continue;

                var type = card.GetCardTypeValue();
                var rarity = card.GetRarityValue();

                if (starters.Contains(card) || rarity == CollectableRarity.Starter)
                {
                    pools.Starters.Add(card);
                    continue;
                }

                switch (type)
                {
                    case CardType.Spell:
                    case CardType.TrainRoomAttachment:
                        Add(pools, new DonorKey(type, rarity, banner: false), card);
                        break;
                    case CardType.Monster when rarity == CollectableRarity.Champion:
                        pools.Champions.Add(card);
                        break;
                    case CardType.Monster:
                        Add(pools, new DonorKey(type, rarity, card.IsBannerUnitCard()), card);
                        break;
                }
            }
            return pools;
        }

        private static void Add(DonorPools pools, DonorKey key, CardData card)
        {
            if (!pools.ByKey.TryGetValue(key, out var list))
            {
                list = [];
                pools.ByKey[key] = list;
            }
            list.Add(card);
        }

        private static List<CardUpgradeData> BuildChampionUpgradePool(AllGameData all)
        {
            var pool = new List<CardUpgradeData>();
            foreach (var classData in all.GetAllClassDatas())
            {
                var champions = AccessTools.Field(typeof(ClassData), "champions")?.GetValue(classData) as List<ChampionData>;
                if (champions == null)
                    continue;

                foreach (var champion in champions)
                {
                    if (champion.championCardData != null && champion.championCardData.HasRandomized())
                        continue;

                    var tree = AccessTools.Field(typeof(ChampionData), "upgradeTree")?.GetValue(champion) as CardUpgradeTreeData;
                    if (tree == null)
                        continue;

                    var upgradeTrees = AccessTools.Field(typeof(CardUpgradeTreeData), "upgradeTrees")?.GetValue(tree)
                        as List<CardUpgradeTreeData.UpgradeTree>;
                    if (upgradeTrees == null)
                        continue;

                    foreach (var upgradeTree in upgradeTrees)
                    {
                        var upgrades = AccessTools.Field(typeof(CardUpgradeTreeData.UpgradeTree), "cardUpgrades")?.GetValue(upgradeTree)
                            as List<CardUpgradeData>;
                        if (upgrades == null)
                            continue;
                        foreach (var upgrade in upgrades)
                        {
                            if (upgrade != null && !pool.Contains(upgrade))
                                pool.Add(upgrade);
                        }
                    }
                }
            }
            return pool;
        }

        private static int ReadRunSeed(SaveManager saveManager)
        {
            try
            {
                var conditions = saveManager.GetStartingConditions();
                if (conditions != null)
                    return conditions.Seed;
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"GetStartingConditions.Seed failed: {ex.Message}");
            }

            var forceSeed = AccessTools.Field(typeof(SaveManager), "forceSeed");
            if (forceSeed?.FieldType == typeof(int))
            {
                var value = (int)forceSeed.GetValue(saveManager)!;
                if (value != 0)
                    return value;
            }

            Plugin.Logger.LogWarning("Could not resolve run seed; using TickCount fallback.");
            return Environment.TickCount;
        }

        private readonly struct DonorKey : IEquatable<DonorKey>
        {
            public DonorKey(CardType type, CollectableRarity rarity, bool banner)
            {
                Type = type;
                Rarity = rarity;
                Banner = banner;
            }

            public CardType Type { get; }
            public CollectableRarity Rarity { get; }
            public bool Banner { get; }

            public bool Equals(DonorKey other)
                => Type == other.Type && Rarity == other.Rarity && Banner == other.Banner;

            public override bool Equals(object? obj) => obj is DonorKey other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((int)Type * 397) ^ ((int)Rarity * 397) ^ (Banner ? 1 : 0);
                }
            }
        }

        private sealed class DonorPools
        {
            public Dictionary<DonorKey, List<CardData>> ByKey { get; } = new();
            public List<CardData> Starters { get; } = [];
            public List<CardData> Champions { get; } = [];
        }
    }
}
