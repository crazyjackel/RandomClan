using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class StatusSwapModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => ctx.StatusIds.Count > 1
               && (card.HasStatusEffects() || (ctx.Character?.HasStartingStatuses() ?? false));

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var statusIds = ctx.StatusIds;
            StatusCountModification.AdjustCardStatuses(card, rng, swap: true, statusIds);
            var ability = ctx.Character?.GetUnitAbilityCardData();
            if (ability != null)
                StatusCountModification.AdjustCardStatuses(ability, rng, swap: true, statusIds);
            ctx.Character?.AdjustStartingStatuses(rng, swap: true, statusIds);
        }
    }
}
