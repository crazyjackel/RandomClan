using HarmonyLib;
using Random_Clan.Plugin.Constants;

namespace Random_Clan.Plugin.Extensions
{
    public static class CharacterDataExtensions
    {
        private static string? _morselSubtypeKey;
        public static void SetHealth(this CharacterData character, int health)
            => AccessTools.Field(typeof(CharacterData), "health")?.SetValue(character, health);

        public static void SetAttackDamage(this CharacterData character, int attack)
            => AccessTools.Field(typeof(CharacterData), "attackDamage")?.SetValue(character, attack);

        public static void SetSize(this CharacterData character, int size)
            => AccessTools.Field(typeof(CharacterData), "size")?.SetValue(character, size);

        public static void SetSubtypeKeys(this CharacterData character, List<string> subtypes)
            => AccessTools.Field(typeof(CharacterData), "subtypeKeys")?.SetValue(character, subtypes);

        public static List<string> GetSubtypeKeys(this CharacterData character)
            => AccessTools.Field(typeof(CharacterData), "subtypeKeys")?.GetValue(character) as List<string> ?? [];

        public static void SetTriggers(this CharacterData character, List<CharacterTriggerData> triggers)
            => AccessTools.Field(typeof(CharacterData), "triggers")?.SetValue(character, triggers);

        public static List<CharacterTriggerData> GetTriggerList(this CharacterData character)
            => AccessTools.Field(typeof(CharacterData), "triggers")?.GetValue(character) as List<CharacterTriggerData> ?? [];

        public static void SetUnitAbility(this CharacterData character, CardData? ability)
            => AccessTools.Field(typeof(CharacterData), "unitAbility")?.SetValue(character, ability);

        public static void SetNameKey(this CharacterData character, string nameKey)
            => AccessTools.Field(typeof(CharacterData), "nameKey")?.SetValue(character, nameKey);

        public static StatusEffectStackData[] GetStartingStatusEffectsArray(this CharacterData character)
            => AccessTools.Field(typeof(CharacterData), "startingStatusEffects")?.GetValue(character) as StatusEffectStackData[]
               ?? [];

        public static void SetStartingStatusEffectsArray(this CharacterData character, StatusEffectStackData[] statuses)
            => AccessTools.Field(typeof(CharacterData), "startingStatusEffects")?.SetValue(character, statuses);

        public static void CopyStartingStatusesFrom(this CharacterData slot, CharacterData donor)
        {
            var field = AccessTools.Field(typeof(CharacterData), "startingStatusEffects");
            if (field == null)
                return;

            var value = field.GetValue(donor);
            if (value is Array arr)
                field.SetValue(slot, arr.Clone());
        }

        /// <summary>
        /// Buffet units are eaten like morsels; ensure the Morsel subtype is present.
        /// </summary>
        public static void EnsureMorselIfBuffet(this CharacterData? character, string? statusId)
        {
            if (character == null || string.IsNullOrEmpty(statusId))
                return;
            if (!statusId.Equals(ModificationTuning.BuffetStatusId, StringComparison.OrdinalIgnoreCase))
                return;

            var morselKey = ResolveMorselSubtypeKey();
            if (morselKey == null)
                return;

            var existing = character.GetSubtypeKeys();
            if (existing.Any(s => s.Equals(morselKey, StringComparison.OrdinalIgnoreCase)))
                return;

            var next = new List<string>(existing) { morselKey };
            character.SetSubtypeKeys(next);
        }

        private static string? ResolveMorselSubtypeKey()
        {
            if (_morselSubtypeKey != null)
                return _morselSubtypeKey;

            foreach (var subtype in SubtypeManager.AllData)
            {
                var key = subtype?.Key;
                if (string.IsNullOrEmpty(key))
                    continue;
                if (key.Equals("SubtypesData_Morsel", StringComparison.OrdinalIgnoreCase)
                    || key.EndsWith("_Morsel", StringComparison.OrdinalIgnoreCase)
                    || key.Equals("Morsel", StringComparison.OrdinalIgnoreCase))
                {
                    _morselSubtypeKey = key;
                    return key;
                }
            }

            return null;
        }
    }
}
