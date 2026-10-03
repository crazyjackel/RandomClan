using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class TraitInjectModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => ctx.InjectableTraits.Count > 0;

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            if (ctx.InjectableTraits.Count == 0)
                return;

            var donor = ctx.InjectableTraits[rng.Next(ctx.InjectableTraits.Count)];
            var clone = donor.Copy();
            if (clone == null)
                return;

            var traits = card.GetTraitList().Where(t => t != null).ToList();
            traits.Add(clone);
            card.SetTraits(traits);
        }
    }
}
