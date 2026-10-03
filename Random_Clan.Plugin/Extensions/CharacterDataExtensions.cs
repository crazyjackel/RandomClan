using HarmonyLib;

namespace Random_Clan.Plugin.Extensions
{
    public static class CharacterDataExtensions
    {
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
    }
}
