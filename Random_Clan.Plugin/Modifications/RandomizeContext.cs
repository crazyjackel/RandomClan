using Random_Clan.Plugin.Extensions;
using TrainworksReloaded.Base;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.CardUpgrade;
using TrainworksReloaded.Base.Class;
using TrainworksReloaded.Base.StatusEffects;
using TrainworksReloaded.Base.Trait;

namespace Random_Clan.Plugin.Modifications
{
    public sealed class RandomizeContext
    {
        private IReadOnlyList<string>? _statusIds;
        private IReadOnlyList<CardTraitData>? _injectableTraits;
        private IReadOnlyList<CardData>? _abilityDonors;
        private IReadOnlyList<string>? _mutableSubtypeKeys;
        private IReadOnlyList<CharacterTriggerData.Trigger>? _swappableCharacterTriggers;
        private IReadOnlyList<CardTriggerType>? _swappableCardTriggers;

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

        /// <summary>Statuses safe to inject/swap onto units and cards (visible, non-ability UI statuses).</summary>
        public IReadOnlyList<string> StatusIds
            => _statusIds ??= BuildSafeStatusIds();

        public IReadOnlyList<CardTraitData> InjectableTraits
            => _injectableTraits ??= BuildInjectableTraits();

        public IReadOnlyList<CardData> AbilityDonors
            => _abilityDonors ??= BuildAbilityDonors();

        public IReadOnlyList<string> MutableSubtypeKeys
            => _mutableSubtypeKeys ??= BuildMutableSubtypeKeys();

        /// <summary>Trigger types already used by real units, so UI/localization exist.</summary>
        public IReadOnlyList<CharacterTriggerData.Trigger> SwappableCharacterTriggers
            => _swappableCharacterTriggers ??= BuildSwappableCharacterTriggers();

        public IReadOnlyList<CardTriggerType> SwappableCardTriggers
            => _swappableCardTriggers ??= BuildSwappableCardTriggers();

        private IReadOnlyList<string> BuildSafeStatusIds()
        {
            var ids = new List<string>();
            try
            {
                var all = StatusEffectManager.Instance?.GetAllStatusEffectsData()?.GetStatusEffectData();
                if (all == null)
                    return ids;

                foreach (var data in all)
                {
                    if (data == null || data.IsHidden())
                        continue;

                    var id = data.GetStatusId();
                    if (string.IsNullOrEmpty(id))
                        continue;
                    if (id.Equals("unit_ability", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (data.GetDisplayCategory() == StatusEffectData.DisplayCategory.Ability)
                        continue;

                    ids.Add(id);
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"Failed building safe status ids: {ex.Message}");
            }
            return ids;
        }

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

        private IReadOnlyList<CharacterTriggerData.Trigger> BuildSwappableCharacterTriggers()
        {
            var set = new HashSet<CharacterTriggerData.Trigger>();
            var all = SaveManager.GetAllGameData();
            if (all == null)
                return [];

            foreach (var card in all.GetAllCardData())
            {
                if (card == null || card.HasRandomized())
                    continue;
                var character = card.GetSpawnCharacterData();
                if (character == null)
                    continue;
                foreach (var trigger in character.GetTriggerList())
                {
                    if (trigger == null)
                        continue;
                    var value = trigger.GetTrigger();
                    // Valiant is skipped by CharacterUI icon refresh; keep the pool combat-facing.
                    if (value == CharacterTriggerData.Trigger.OnValiant)
                        continue;
                    if (!CharacterTriggerData.ShouldDisplayOnCharacter(value))
                        continue;
                    set.Add(value);
                }
            }
            return set.ToList();
        }

        private IReadOnlyList<CardTriggerType> BuildSwappableCardTriggers()
        {
            var set = new HashSet<CardTriggerType>();
            var all = SaveManager.GetAllGameData();
            if (all == null)
                return [];

            foreach (var card in all.GetAllCardData())
            {
                if (card == null || card.HasRandomized())
                    continue;
                foreach (var trigger in card.GetTriggerList())
                {
                    if (trigger == null)
                        continue;
                    var field = HarmonyLib.AccessTools.Field(typeof(CardTriggerEffectData), "trigger");
                    if (field?.GetValue(trigger) is CardTriggerType value)
                        set.Add(value);
                }
            }
            return set.ToList();
        }
    }
}
