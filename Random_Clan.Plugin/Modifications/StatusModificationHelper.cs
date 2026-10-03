using HarmonyLib;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications
{
    internal static class StatusModificationHelper
    {
        private static readonly string[] StartingStatusFields = ["startingStatusEffects", "statusEffectStacks"];

        public static bool HasCardStatusEntries(CardData card)
            => CollectCardEntries(card).Count > 0;

        public static bool HasStartingStatusEntries(CharacterData? character)
            => character != null && GetStartingStatuses(character) is { Length: > 0 };

        public static bool TryAdjustSingleCardStatus(
            CardData card,
            Random rng,
            bool swap,
            IReadOnlyList<string>? statusIds = null)
        {
            var entries = CollectCardEntries(card);
            if (entries.Count == 0)
                return false;

            var (effect, statuses, index) = entries[rng.Next(entries.Count)];
            if (!AdjustEntry(statuses, index, rng, swap, statusIds))
                return false;

            effect.SetStatusEffects(statuses);
            return true;
        }

        public static bool TryAdjustSingleStartingStatus(
            CharacterData character,
            Random rng,
            bool swap,
            IReadOnlyList<string>? statusIds = null)
        {
            foreach (var fieldName in StartingStatusFields)
            {
                var field = AccessTools.Field(typeof(CharacterData), fieldName);
                if (field?.GetValue(character) is not StatusEffectStackData[] statuses || statuses.Length == 0)
                    continue;

                var index = rng.Next(statuses.Length);
                if (!AdjustEntry(statuses, index, rng, swap, statusIds))
                    return false;

                field.SetValue(character, statuses);
                return true;
            }
            return false;
        }

        public static bool TryInjectStartingStatus(
            CharacterData character,
            Random rng,
            IReadOnlyList<string> statusIds)
        {
            if (statusIds.Count == 0)
                return false;

            var field = AccessTools.Field(typeof(CharacterData), "startingStatusEffects")
                ?? AccessTools.Field(typeof(CharacterData), "statusEffectStacks");
            if (field == null)
                return false;

            var existing = field.GetValue(character) as StatusEffectStackData[] ?? [];
            var next = new StatusEffectStackData[existing.Length + 1];
            for (var i = 0; i < existing.Length; i++)
                next[i] = existing[i];

            next[existing.Length] = new StatusEffectStackData
            {
                statusId = statusIds[rng.Next(statusIds.Count)],
                count = Math.Max(1, 1.MutateBalanced(rng)),
            };
            field.SetValue(character, next);
            return true;
        }

        private static List<(CardEffectData Effect, StatusEffectStackData[] Statuses, int Index)> CollectCardEntries(
            CardData card)
        {
            var entries = new List<(CardEffectData, StatusEffectStackData[], int)>();
            var effects = card.GetEffects();
            if (effects == null)
                return entries;

            foreach (var effect in effects)
            {
                if (effect == null)
                    continue;
                var statuses = effect.GetStatusEffects();
                if (statuses == null || statuses.Length == 0)
                    continue;
                for (var i = 0; i < statuses.Length; i++)
                    entries.Add((effect, statuses, i));
            }
            return entries;
        }

        private static StatusEffectStackData[]? GetStartingStatuses(CharacterData character)
        {
            foreach (var fieldName in StartingStatusFields)
            {
                var field = AccessTools.Field(typeof(CharacterData), fieldName);
                if (field?.GetValue(character) is StatusEffectStackData[] statuses && statuses.Length > 0)
                    return statuses;
            }
            return null;
        }

        private static bool AdjustEntry(
            StatusEffectStackData[] statuses,
            int index,
            Random rng,
            bool swap,
            IReadOnlyList<string>? statusIds)
        {
            if (swap)
            {
                if (statusIds == null || statusIds.Count < 2)
                    return false;
                var next = statuses[index].statusId.TrySwapStatus(rng, statusIds);
                if (next == null)
                    return false;
                statuses[index].statusId = next;
                return true;
            }

            statuses[index].count = Math.Max(1, Math.Max(1, statuses[index].count).MutateBalanced(rng));
            return true;
        }
    }
}
