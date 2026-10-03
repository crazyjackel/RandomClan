using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class StatusCountModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => card.HasStatusEffects() || (ctx.Character?.HasStartingStatuses() ?? false);

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            AdjustCardStatuses(card, rng, swap: false);
            var ability = ctx.Character?.GetUnitAbilityCardData();
            if (ability != null)
                AdjustCardStatuses(ability, rng, swap: false);
            ctx.Character?.AdjustStartingStatuses(rng, swap: false);
        }

        internal static void AdjustCardStatuses(
            CardData card,
            Random rng,
            bool swap,
            IReadOnlyList<string>? statusIds = null)
        {
            var effects = card.GetEffects();
            if (effects == null)
                return;

            foreach (var effect in effects)
            {
                if (effect == null)
                    continue;
                var statuses = effect.GetStatusEffects();
                if (statuses == null || statuses.Length == 0)
                    continue;

                for (var i = 0; i < statuses.Length; i++)
                {
                    if (swap)
                    {
                        if (statusIds == null)
                            continue;
                        var next = statuses[i].statusId.TrySwapStatus(rng, statusIds);
                        if (next != null)
                            statuses[i].statusId = next;
                    }
                    else
                    {
                        statuses[i].count = Math.Max(1, Math.Max(1, statuses[i].count).MutateBalanced(rng));
                    }
                }
                effect.SetStatusEffects(statuses);
            }
        }
    }
}
