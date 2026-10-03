using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class NumberAdjustmentModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
        {
            foreach (var target in Targets(card, ctx))
            {
                var effects = target.GetEffects();
                if (effects != null && effects.Any(e => e != null && e.GetParamIntValue() > 0))
                    return true;
            }
            return false;
        }

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
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
                    if (paramInt > 0)
                        effect.SetParamIntValue(paramInt.MutateBalanced(rng));
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
