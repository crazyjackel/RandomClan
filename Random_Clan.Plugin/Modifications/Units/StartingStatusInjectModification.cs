using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class StartingStatusInjectModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => ctx.StatusIds.Count > 0 && (ctx.Character ?? card.GetSpawnCharacterData()) != null;

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character != null)
                StatusModificationHelper.TryInjectStartingStatus(character, rng, ctx.StatusIds);
        }
    }
}
