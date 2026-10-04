namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class StartingStatusCountModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => StatusModificationHelper.HasStartingStatusEntries(ctx.Character ?? card.GetSpawnCharacterData());

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character != null)
                StatusModificationHelper.TryAdjustSingleStartingStatus(character, ctx, rng, swap: false);
        }
    }
}
