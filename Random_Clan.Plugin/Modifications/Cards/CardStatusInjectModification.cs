using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class CardStatusInjectModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => ctx.StatusIds.Count > 0 && StatusModificationHelper.HasCardStatusEntries(card);

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            if (ctx.StatusIds.Count == 0)
                return;

            var effects = card.GetEffects();
            if (effects == null)
                return;

            var candidates = effects
                .Where(e => e != null && e.GetStatusEffects() is { Length: > 0 })
                .ToList();
            if (candidates.Count == 0)
                return;

            var effect = candidates[rng.Next(candidates.Count)];
            var existing = effect.GetStatusEffects() ?? [];
            var statusId = ctx.PickStatusId(rng, effect.FavorsPositiveStatuses());
            if (statusId == null)
                return;

            var next = new StatusEffectStackData[existing.Length + 1];
            for (var i = 0; i < existing.Length; i++)
                next[i] = existing[i];

            next[existing.Length] = new StatusEffectStackData
            {
                statusId = statusId,
                count = Math.Max(1, 1.MutateBalanced(rng)),
            };
            effect.SetStatusEffects(next);
            ctx.Character?.EnsureMorselIfBuffet(statusId);
        }
    }
}
