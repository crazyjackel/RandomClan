using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications
{
    internal static class StatusModificationHelper
    {
        public static bool HasCardStatusEntries(CardData card)
            => CollectCardEntries(card).Count > 0;

        public static bool HasStartingStatusEntries(CharacterData? character)
            => character != null && character.GetStartingStatusEffectsArray().Length > 0;

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
            var statuses = character.GetStartingStatusEffectsArray();
            if (statuses.Length == 0)
                return false;

            var index = rng.Next(statuses.Length);
            if (!AdjustEntry(statuses, index, rng, swap, statusIds))
                return false;

            character.SetStartingStatusEffectsArray(statuses);
            return true;
        }

        public static bool TryInjectStartingStatus(
            CharacterData character,
            Random rng,
            IReadOnlyList<string> statusIds)
        {
            if (statusIds.Count == 0)
                return false;

            var existing = character.GetStartingStatusEffectsArray();
            var next = new StatusEffectStackData[existing.Length + 1];
            for (var i = 0; i < existing.Length; i++)
                next[i] = existing[i];

            next[existing.Length] = new StatusEffectStackData
            {
                statusId = statusIds[rng.Next(statusIds.Count)],
                count = Math.Max(1, 1.MutateBalanced(rng)),
            };
            character.SetStartingStatusEffectsArray(next);
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
