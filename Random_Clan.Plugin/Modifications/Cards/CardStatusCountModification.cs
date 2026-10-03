namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class CardStatusCountModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => StatusModificationHelper.HasCardStatusEntries(card);

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
            => StatusModificationHelper.TryAdjustSingleCardStatus(card, rng, swap: false);
    }
}
