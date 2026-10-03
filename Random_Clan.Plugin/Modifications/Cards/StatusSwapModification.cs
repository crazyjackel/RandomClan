using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class StatusSwapModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => card.HasStatusEffects() || (ctx.Character?.HasStartingStatuses() ?? false);

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            StatusCountModification.AdjustCardStatuses(card, rng, swap: true);
            var ability = ctx.Character?.GetUnitAbilityCardData();
            if (ability != null)
                StatusCountModification.AdjustCardStatuses(ability, rng, swap: true);
            ctx.Character?.AdjustStartingStatuses(rng, swap: true);
        }
    }
}
