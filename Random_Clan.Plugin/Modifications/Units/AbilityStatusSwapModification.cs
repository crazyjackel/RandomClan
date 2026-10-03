using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class AbilityStatusSwapModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
        {
            if (ctx.StatusIds.Count <= 1)
                return false;
            var ability = ctx.Character?.GetUnitAbilityCardData() ?? card.GetSpawnCharacterData()?.GetUnitAbilityCardData();
            return ability != null && StatusModificationHelper.HasCardStatusEntries(ability);
        }

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var ability = ctx.Character?.GetUnitAbilityCardData() ?? card.GetSpawnCharacterData()?.GetUnitAbilityCardData();
            if (ability != null)
                StatusModificationHelper.TryAdjustSingleCardStatus(ability, rng, swap: true, ctx.StatusIds);
        }
    }
}
