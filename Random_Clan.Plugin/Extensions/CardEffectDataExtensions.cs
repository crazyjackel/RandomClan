using HarmonyLib;

namespace Random_Clan.Plugin.Extensions
{
    public static class CardEffectDataExtensions
    {
        public static int GetParamIntValue(this CardEffectData effect)
            => (int)(AccessTools.Field(typeof(CardEffectData), "paramInt")?.GetValue(effect) ?? 0);

        public static void SetParamIntValue(this CardEffectData effect, int value)
            => AccessTools.Field(typeof(CardEffectData), "paramInt")?.SetValue(effect, value);

        public static StatusEffectStackData[]? GetStatusEffects(this CardEffectData effect)
            => AccessTools.Field(typeof(CardEffectData), "paramStatusEffects")?.GetValue(effect) as StatusEffectStackData[];

        public static void SetStatusEffects(this CardEffectData effect, StatusEffectStackData[] statuses)
            => AccessTools.Field(typeof(CardEffectData), "paramStatusEffects")?.SetValue(effect, statuses);

        public static CardUpgradeData? GetCardUpgrade(this CardEffectData effect)
            => AccessTools.Field(typeof(CardEffectData), "paramCardUpgradeData")?.GetValue(effect) as CardUpgradeData;

        public static void SetCardUpgrade(this CardEffectData effect, CardUpgradeData upgrade)
            => AccessTools.Field(typeof(CardEffectData), "paramCardUpgradeData")?.SetValue(effect, upgrade);
    }
}
