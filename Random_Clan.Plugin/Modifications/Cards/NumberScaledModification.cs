namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class NumberScaledModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => NumberModificationHelper.HasPositiveParamInt(card, ctx);

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
            => NumberModificationHelper.MutateScaled(card, ctx, rng);
    }
}
