using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class StartingStatusSwapModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => ctx.StatusIds.Count > 1
               && StatusModificationHelper.HasStartingStatusEntries(ctx.Character ?? card.GetSpawnCharacterData());

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character != null)
                StatusModificationHelper.TryAdjustSingleStartingStatus(character, rng, swap: true, ctx.StatusIds);
        }
    }
}
