using Random_Clan.Plugin.Copying;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class AbilitySwapModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null || character.GetUnitAbilityCardData() == null)
                return false;
            return ctx.AbilityDonors.Count > 0;
        }

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null || ctx.AbilityDonors.Count == 0)
                return;

            var current = character.GetUnitAbilityCardData();
            CardData donor;
            var attempts = 0;
            do
            {
                donor = ctx.AbilityDonors[rng.Next(ctx.AbilityDonors.Count)];
                attempts++;
            } while (current != null
                     && donor.name == current.name
                     && ctx.AbilityDonors.Count > 1
                     && attempts < 8);

            var clone = CardDonorCopier.CloneAbilityCard(donor, $"{card.name}_AbilitySwap");
            character.SetUnitAbility(clone);
        }
    }
}
