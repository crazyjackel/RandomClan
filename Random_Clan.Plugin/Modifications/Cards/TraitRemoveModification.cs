using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class TraitRemoveModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => card.GetTraitList().Any(t => t != null && !t.IsMarkerTrait());

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var traits = card.GetTraitList().Where(t => t != null).ToList();
            var removable = traits
                .Select((t, i) => (Trait: t, Index: i))
                .Where(x => !x.Trait.IsMarkerTrait())
                .ToList();
            if (removable.Count == 0)
                return;

            var pick = removable[rng.Next(removable.Count)];
            traits.RemoveAt(pick.Index);
            card.SetTraits(traits);
        }
    }
}
