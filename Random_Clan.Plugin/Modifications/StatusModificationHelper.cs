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
            RandomizeContext ctx,
            Random rng,
            bool swap)
        {
            var entries = CollectCardEntries(card);
            if (entries.Count == 0)
                return false;

            var (effect, statuses, index) = entries[rng.Next(entries.Count)];
            var favorPositive = effect.FavorsPositiveStatuses();
            if (!AdjustEntry(statuses, index, ctx, rng, swap, favorPositive))
                return false;

            effect.SetStatusEffects(statuses);
            return true;
        }

        public static bool TryAdjustSingleStartingStatus(
            CharacterData character,
            RandomizeContext ctx,
            Random rng,
            bool swap)
        {
            var statuses = character.GetStartingStatusEffectsArray();
            if (statuses.Length == 0)
                return false;

            var index = rng.Next(statuses.Length);
            // Player units favor buffs / fewer debuff stacks.
            if (!AdjustEntry(statuses, index, ctx, rng, swap, favorPositive: true))
                return false;

            character.SetStartingStatusEffectsArray(statuses);
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
            RandomizeContext ctx,
            Random rng,
            bool swap,
            bool favorPositive)
        {
            if (swap)
            {
                var pool = ctx.GetStatusSwapPool(rng, favorPositive);
                if (pool.Count < 2)
                    return false;
                var next = statuses[index].statusId.TrySwapStatus(rng, pool);
                if (next == null)
                    return false;
                statuses[index].statusId = next;
                return true;
            }

            var polarity = ctx.IsPositiveStatus(statuses[index].statusId);
            var count = Math.Max(1, statuses[index].count);
            if (polarity is bool isPositive)
            {
                // Good for player: raise stacks that match favored polarity, lower the opposite.
                var favorIncrease = isPositive == favorPositive;
                statuses[index].count = count.MutateToward(rng, favorIncrease);
            }
            else
            {
                statuses[index].count = count.MutateBalanced(rng);
            }
            return true;
        }
    }
}
