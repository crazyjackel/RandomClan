using Random_Clan.Plugin.Constants;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class NumberFixedModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => NumberMutation.HasPositiveParamInt(card, ctx);

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
            => NumberMutation.Mutate(card, ctx, rng, fixedSteps: true);
    }

    public sealed class NumberScaledModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => NumberMutation.HasPositiveParamInt(card, ctx);

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
            => NumberMutation.Mutate(card, ctx, rng, fixedSteps: false);
    }

    file static class NumberMutation
    {
        public static bool HasPositiveParamInt(CardData card, RandomizeContext ctx)
            => Targets(card, ctx).Any(t =>
                t.GetEffects()?.Any(e => e != null && e.GetParamIntValue() > 0) == true);

        public static void Mutate(CardData card, RandomizeContext ctx, Random rng, bool fixedSteps)
        {
            foreach (var target in Targets(card, ctx))
            {
                var effects = target.GetEffects();
                if (effects == null)
                    continue;
                foreach (var effect in effects)
                {
                    if (effect == null)
                        continue;
                    var paramInt = effect.GetParamIntValue();
                    if (paramInt <= 0)
                        continue;
                    effect.SetParamIntValue(
                        fixedSteps
                            ? paramInt.MutateFixed(rng, ModificationTuning.NumberSteps)
                            : paramInt.MutateScaled(rng, ModificationTuning.NumberScales));
                }
            }
        }

        private static IEnumerable<CardData> Targets(CardData card, RandomizeContext ctx)
        {
            yield return card;
            var ability = ctx.Character?.GetUnitAbilityCardData() ?? card.GetSpawnCharacterData()?.GetUnitAbilityCardData();
            if (ability != null)
                yield return ability;
        }
    }
}
