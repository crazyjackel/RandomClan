namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class CardStatusSwapModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => ctx.StatusIds.Count > 1 && StatusModificationHelper.HasCardStatusEntries(card);

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
            => StatusModificationHelper.TryAdjustSingleCardStatus(card, rng, swap: true, ctx.StatusIds);
    }
}
