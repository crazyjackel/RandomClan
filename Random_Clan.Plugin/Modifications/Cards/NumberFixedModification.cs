namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class NumberFixedModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => NumberModificationHelper.HasPositiveParamInt(card, ctx);

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
            => NumberModificationHelper.MutateFixed(card, ctx, rng);
    }
}
