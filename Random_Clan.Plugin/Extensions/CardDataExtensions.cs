using HarmonyLib;
using Random_Clan.Plugin.Constants;

namespace Random_Clan.Plugin.Extensions
{
    public static class CardDataExtensions
    {
        public static CardType GetCardTypeValue(this CardData card)
            => (CardType)(AccessTools.Field(typeof(CardData), "cardType")?.GetValue(card) ?? CardType.Invalid);

        public static CollectableRarity GetRarityValue(this CardData card)
            => (CollectableRarity)(AccessTools.Field(typeof(CardData), "rarity")?.GetValue(card) ?? default(CollectableRarity));

        public static bool IsChampionCard(this CardData card)
            => card.GetCardTypeValue() == CardType.Monster && card.GetRarityValue() == CollectableRarity.Champion;

        public static bool IsBannerUnitCard(this CardData card)
        {
            if (card.GetCardTypeValue() != CardType.Monster || card.IsChampionCard())
                return false;
            var character = card.GetSpawnCharacterData();
            if (character == null)
                return false;
            return character.GetSubtypeKeys().Any(s =>
                s.Contains("BannerUnit", StringComparison.OrdinalIgnoreCase));
        }

        public static List<CardTraitData> GetTraitList(this CardData card)
            => card.GetTraits()
               ?? AccessTools.Field(typeof(CardData), "traits")?.GetValue(card) as List<CardTraitData>
               ?? [];

        public static bool HasRandomized(this CardData card)
            => card.GetTraitList().Any(t =>
                (t.traitStateName ?? "").Contains(MarkerTraitNames.Randomized, StringComparison.Ordinal));

        public static int GetModifierCount(this CardData card)
        {
            foreach (var trait in card.GetTraitList())
            {
                if (!trait.IsModifierTrait())
                    continue;
                return trait.ReadParamInt();
            }
            return 0;
        }

        public static List<CardTraitData> GetMarkerTraits(this CardData card)
            => card.GetTraitList().Where(t => t.IsMarkerTrait()).ToList();

        public static bool HasStatusEffects(this CardData card)
        {
            var effects = card.GetEffects();
            if (effects == null)
                return false;
            return effects.Any(e => e != null && e.GetStatusEffects() is { Length: > 0 });
        }

        public static void SetCost(this CardData card, int cost)
            => AccessTools.Field(typeof(CardData), "cost")?.SetValue(card, cost);

        public static int GetCostValue(this CardData card)
            => (int)(AccessTools.Field(typeof(CardData), "cost")?.GetValue(card) ?? 0);

        public static void SetTraits(this CardData card, List<CardTraitData> traits)
            => AccessTools.Field(typeof(CardData), "traits")?.SetValue(card, traits);

        public static void SetEffects(this CardData card, List<CardEffectData> effects)
            => AccessTools.Field(typeof(CardData), "effects")?.SetValue(card, effects);

        public static void SetTriggers(this CardData card, List<CardTriggerEffectData> triggers)
            => AccessTools.Field(typeof(CardData), "triggers")?.SetValue(card, triggers);

        public static List<CardTriggerEffectData> GetTriggerList(this CardData card)
            => AccessTools.Field(typeof(CardData), "triggers")?.GetValue(card) as List<CardTriggerEffectData> ?? [];

        public static void SetNameKey(this CardData card, string nameKey)
            => AccessTools.Field(typeof(CardData), "nameKey")?.SetValue(card, nameKey);

        public static string? GetDescriptionKey(this CardData card)
            => AccessTools.Field(typeof(CardData), "overrideDescriptionKey")?.GetValue(card) as string;

        public static void SetDescriptionKey(this CardData card, string key)
            => AccessTools.Field(typeof(CardData), "overrideDescriptionKey")?.SetValue(card, key);

        public static object? GetRarityObject(this CardData card)
            => AccessTools.Field(typeof(CardData), "rarity")?.GetValue(card);

        public static void SetRarityObject(this CardData card, object rarity)
            => AccessTools.Field(typeof(CardData), "rarity")?.SetValue(card, rarity);

        public static void SetIsUnitAbility(this CardData card, bool value)
            => AccessTools.Field(typeof(CardData), "isUnitAbility")?.SetValue(card, value);

        /// <summary>Steal donor card art prefab ref (shared asset). Monster slots use GlitchedArtFactory instead.</summary>
        public static void CopyCardArtFrom(this CardData slot, CardData donor)
        {
            var field = AccessTools.Field(typeof(CardData), "cardArtPrefabVariantRef");
            var art = field?.GetValue(donor);
            if (art != null)
                field!.SetValue(slot, art);
        }
    }
}
