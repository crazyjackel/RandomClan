using HarmonyLib;
using Random_Clan.Plugin.Constants;

namespace Random_Clan.Plugin.Extensions
{
    public static class CardTraitDataExtensions
    {
        public static bool IsMarkerTrait(this CardTraitData? trait)
        {
            if (trait == null)
                return false;
            var state = trait.traitStateName ?? "";
            return state.Contains(MarkerTraitNames.Randomized, StringComparison.Ordinal)
                || MarkerTraitNames.ModifierStates.Any(m => state.Contains(m, StringComparison.Ordinal));
        }

        public static bool IsModifierTrait(this CardTraitData? trait)
        {
            if (trait == null)
                return false;
            var state = trait.traitStateName ?? "";
            return MarkerTraitNames.ModifierStates.Any(m => state.Contains(m, StringComparison.Ordinal));
        }

        public static int ReadParamInt(this CardTraitData trait)
        {
            var method = AccessTools.Method(typeof(CardTraitData), "GetParamInt");
            if (method != null && method.GetParameters().Length == 0 && method.Invoke(trait, null) is int i)
                return i;
            return (int)(AccessTools.Field(typeof(CardTraitData), "paramInt")?.GetValue(trait) ?? 0);
        }

        public static bool HasPositiveParam(this CardTraitData trait)
        {
            foreach (var fieldName in new[] { "paramInt", "paramInt2" })
            {
                var field = AccessTools.Field(typeof(CardTraitData), fieldName);
                if (field?.FieldType == typeof(int) && (int)field.GetValue(trait)! > 0)
                    return true;
            }
            return false;
        }
    }
}
