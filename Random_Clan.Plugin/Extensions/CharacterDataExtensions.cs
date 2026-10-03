using HarmonyLib;

namespace Random_Clan.Plugin.Extensions
{
    public static class CharacterDataExtensions
    {
        private static readonly string[] StartingStatusFields = ["startingStatusEffects", "statusEffectStacks"];

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

        public static bool HasStartingStatuses(this CharacterData character)
        {
            foreach (var fieldName in StartingStatusFields)
            {
                var field = AccessTools.Field(typeof(CharacterData), fieldName);
                if (field?.GetValue(character) is StatusEffectStackData[] statuses && statuses.Length > 0)
                    return true;
            }
            return false;
        }

        public static void AdjustStartingStatuses(this CharacterData character, Random rng, bool swap)
        {
            foreach (var fieldName in StartingStatusFields)
            {
                var field = AccessTools.Field(typeof(CharacterData), fieldName);
                if (field?.GetValue(character) is not StatusEffectStackData[] statuses || statuses.Length == 0)
                    continue;

                for (var i = 0; i < statuses.Length; i++)
                {
                    if (swap)
                    {
                        var next = statuses[i].statusId.TrySwapStatus(rng);
                        if (next != null)
                            statuses[i].statusId = next;
                    }
                    else
                    {
                        statuses[i].count = Math.Max(1, Math.Max(1, statuses[i].count).MutateBalanced(rng));
                    }
                }
                field.SetValue(character, statuses);
                return;
            }
        }

        public static void CopyStartingStatusesFrom(this CharacterData slot, CharacterData donor)
        {
            foreach (var fieldName in StartingStatusFields)
            {
                var field = AccessTools.Field(typeof(CharacterData), fieldName);
                if (field == null)
                    continue;
                var value = field.GetValue(donor);
                if (value is Array arr)
                {
                    field.SetValue(slot, arr.Clone());
                    return;
                }
                if (value is System.Collections.IList list)
                {
                    var copy = Activator.CreateInstance(list.GetType()) as System.Collections.IList;
                    if (copy == null)
                        return;
                    foreach (var item in list)
                        copy.Add(item);
                    field.SetValue(slot, copy);
                    return;
                }
            }
        }
    }
}
