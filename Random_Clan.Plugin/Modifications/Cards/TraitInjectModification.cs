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

            var traits = card.GetTraitList().Where(t => t != null).ToList();
            var existingStates = new HashSet<string>(
                traits.Select(t => t.traitStateName ?? ""),
                StringComparer.Ordinal);

            var pool = ctx.InjectableTraits
                .Where(t => t != null && !existingStates.Contains(t.traitStateName ?? ""))
                .ToList();
            if (pool.Count == 0)
                return;

            var donor = pool[rng.Next(pool.Count)];
            var clone = donor.Copy();
            if (clone == null)
                return;

            traits.Add(clone);
            card.SetTraits(traits);
        }
    }
}
