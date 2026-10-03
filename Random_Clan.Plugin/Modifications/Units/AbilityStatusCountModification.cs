using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class AbilityStatusCountModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
        {
            var ability = ctx.Character?.GetUnitAbilityCardData() ?? card.GetSpawnCharacterData()?.GetUnitAbilityCardData();
            return ability != null && StatusModificationHelper.HasCardStatusEntries(ability);
        }

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var ability = ctx.Character?.GetUnitAbilityCardData() ?? card.GetSpawnCharacterData()?.GetUnitAbilityCardData();
            if (ability != null)
                StatusModificationHelper.TryAdjustSingleCardStatus(ability, rng, swap: false);
        }
    }
}
