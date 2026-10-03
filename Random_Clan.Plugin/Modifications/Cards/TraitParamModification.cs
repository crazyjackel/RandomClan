using HarmonyLib;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class TraitParamModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => card.GetTraitList().Any(t => !t.IsMarkerTrait() && t.HasPositiveParam());

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            foreach (var trait in card.GetTraitList())
            {
                if (trait == null || trait.IsMarkerTrait())
                    continue;
                foreach (var fieldName in new[] { "paramInt", "paramInt2" })
                {
                    var field = AccessTools.Field(typeof(CardTraitData), fieldName);
                    if (field == null || field.FieldType != typeof(int))
                        continue;
                    var value = (int)field.GetValue(trait)!;
                    if (value > 0)
                        field.SetValue(trait, value.MutateBalanced(rng));
                }
            }
        }
    }
}