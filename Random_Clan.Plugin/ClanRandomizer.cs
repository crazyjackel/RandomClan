using HarmonyLib;
using Random_Clan.Plugin.Copying;
using Random_Clan.Plugin.Extensions;
using Random_Clan.Plugin.Modifications;
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
            GameDataClient client,
            ModificationRegistry modifications,
            CardDonorCopier copier)
        {
            _cards = cards;
            _traits = traits;
            _classes = classes;
            _upgrades = upgrades;
            _statuses = statuses;
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
            var donors = BuildDonorPools(all);
            var upgradePool = BuildChampionUpgradePool(all);

            var ctx = new RandomizeContext(saveManager, _cards, _traits, _classes, _upgrades, _statuses, _client)
            {
                ChampionUpgradePool = upgradePool,
            };

            Plugin.Logger.LogInfo(
                $"Random Clan seed={seed} slots={slots.Count} donors: spells={donors.Spells.Count} units={donors.Units.Count} champs={donors.Champions.Count} rooms={donors.Rooms.Count}");

            foreach (var slot in slots)
            {
                var markers = slot.GetMarkerTraits();
                var pool = SelectPool(slot, donors);
                if (pool.Count == 0)
                {
                    Plugin.Logger.LogWarning($"No donors for {slot.name}");
                    continue;
                }

                var donor = pool[rng.Next(pool.Count)];
                Plugin.Logger.LogInfo($"{slot.name} <- {donor.name}");
                _copier.CopyOnto(slot, donor, markers);

                ctx.Character = slot.GetSpawnCharacterData();
                foreach (var mod in _modifications.Pick(slot, ctx, rng, slot.GetModifierCount()))
                {
                    mod.Modify(slot, ctx, rng);
                    Plugin.Logger.LogInfo($"  mod {mod.GetType().Name}");
                }
            }

            _lastSeed = seed;
            Plugin.Logger.LogInfo("Random Clan randomization complete.");
        }

        private static List<CardData> SelectPool(CardData slot, DonorPools donors)
        {
            var type = slot.GetCardTypeValue();
            var rarity = slot.GetRarityValue();
            return type switch
            {
                CardType.Spell => donors.Spells,
                CardType.TrainRoomAttachment => donors.Rooms,
                CardType.Monster when rarity == CollectableRarity.Champion => donors.Champions,
                CardType.Monster => donors.Units,
                _ => [],
            };
        }

        private static DonorPools BuildDonorPools(AllGameData all)
        {
            var pools = new DonorPools();
            foreach (var card in all.GetAllCardData())
            {
                if (card == null || card.HasRandomized() || card.IsUnitAbility() || card.IsRoomAbility())
                    continue;

                switch (card.GetCardTypeValue())
                {
                    case CardType.Spell:
                        pools.Spells.Add(card);
                        break;
                    case CardType.Monster when card.GetRarityValue() == CollectableRarity.Champion:
                        pools.Champions.Add(card);
                        break;
                    case CardType.Monster:
                        pools.Units.Add(card);
                        break;
                    case CardType.TrainRoomAttachment:
                        pools.Rooms.Add(card);
                        break;
                }
            }
            return pools;
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
            foreach (var name in new[] { "GetRunSeed", "GetSeed", "GetCurrentSeed", "GetRngSeed" })
            {
                var method = AccessTools.Method(typeof(SaveManager), name);
                if (method != null && method.GetParameters().Length == 0 && method.Invoke(saveManager, null) is int i)
                    return i;
            }

            foreach (var name in new[] { "runSeed", "seed", "_runSeed", "currentSeed", "rngSeed" })
            {
                var field = AccessTools.Field(typeof(SaveManager), name);
                if (field != null && field.FieldType == typeof(int))
                    return (int)field.GetValue(saveManager)!;
            }

            Plugin.Logger.LogWarning("Could not resolve run seed; using TickCount fallback.");
            return Environment.TickCount;
        }

        private sealed class DonorPools
        {
            public List<CardData> Spells { get; } = [];
            public List<CardData> Units { get; } = [];
            public List<CardData> Champions { get; } = [];
            public List<CardData> Rooms { get; } = [];
        }
    }
}
