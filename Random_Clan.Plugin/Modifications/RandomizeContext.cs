using Random_Clan.Plugin.Extensions;
using TrainworksReloaded.Base;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.CardUpgrade;
using TrainworksReloaded.Base.Class;
using TrainworksReloaded.Base.StatusEffects;
using TrainworksReloaded.Base.Trait;
using TrainworksReloaded.Core.Enum;

namespace Random_Clan.Plugin.Modifications
{
    public sealed class RandomizeContext
    {
        private IReadOnlyList<string>? _statusIds;
        private IReadOnlyList<CardTraitData>? _injectableTraits;
        private IReadOnlyList<CardData>? _abilityDonors;
        private IReadOnlyList<string>? _mutableSubtypeKeys;

        public RandomizeContext(
            SaveManager saveManager,
            CardDataRegister cards,
            CardTraitDataRegister traits,
            ClassDataRegister classes,
            CardUpgradeRegister upgrades,
            StatusEffectDataRegister statuses,
            GameDataClient client)
        {
            SaveManager = saveManager;
            Cards = cards;
            Traits = traits;
            Classes = classes;
            Upgrades = upgrades;
            Statuses = statuses;
            Client = client;
        }

        public SaveManager SaveManager { get; }
        public CardDataRegister Cards { get; }
        public CardTraitDataRegister Traits { get; }
        public ClassDataRegister Classes { get; }
        public CardUpgradeRegister Upgrades { get; }
        public StatusEffectDataRegister Statuses { get; }
        public GameDataClient Client { get; }
        public CharacterData? Character { get; set; }
        public List<CardUpgradeData> ChampionUpgradePool { get; set; } = [];

        public IReadOnlyList<string> StatusIds
            => _statusIds ??= Statuses.GetAllIdentifiers(RegisterIdentifierType.ReadableID);

        public IReadOnlyList<CardTraitData> InjectableTraits
            => _injectableTraits ??= BuildInjectableTraits();

        public IReadOnlyList<CardData> AbilityDonors
            => _abilityDonors ??= BuildAbilityDonors();

        public IReadOnlyList<string> MutableSubtypeKeys
            => _mutableSubtypeKeys ??= BuildMutableSubtypeKeys();

        private IReadOnlyList<CardTraitData> BuildInjectableTraits()
        {
            var byState = new Dictionary<string, CardTraitData>(StringComparer.Ordinal);
            var all = SaveManager.GetAllGameData();
            if (all == null)
                return [];

            foreach (var card in all.GetAllCardData())
            {
                if (card == null)
                    continue;
                foreach (var trait in card.GetTraitList())
                {
                    if (trait == null || trait.IsMarkerTrait())
                        continue;
                    var state = trait.traitStateName ?? "";
                    if (state.Length == 0 || byState.ContainsKey(state))
                        continue;
                    byState[state] = trait;
                }
            }
            return byState.Values.ToList();
        }

        private IReadOnlyList<CardData> BuildAbilityDonors()
        {
            var all = SaveManager.GetAllGameData();
            if (all == null)
                return [];

            var byName = new Dictionary<string, CardData>(StringComparer.Ordinal);
            foreach (var card in all.GetAllCardData())
            {
                if (card == null || card.HasRandomized())
                    continue;

                if (card.IsUnitAbility() && !byName.ContainsKey(card.name))
                    byName[card.name] = card;

                var ability = card.GetSpawnCharacterData()?.GetUnitAbilityCardData();
                if (ability != null && !byName.ContainsKey(ability.name))
                    byName[ability.name] = ability;
            }
            return byName.Values.ToList();
        }

        private static IReadOnlyList<string> BuildMutableSubtypeKeys()
        {
            var keys = new List<string>();
            foreach (var subtype in SubtypeManager.AllData)
            {
                if (subtype == null || subtype.IsNone || subtype.IsChampion)
                    continue;
                var key = subtype.Key;
                if (string.IsNullOrEmpty(key))
                    continue;
                if (key.Contains("BannerUnit", StringComparison.OrdinalIgnoreCase))
                    continue;
                keys.Add(key);
            }
            return keys;
        }
    }
}
