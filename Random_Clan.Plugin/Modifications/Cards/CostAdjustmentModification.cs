using Random_Clan.Plugin.Constants;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class CostAdjustmentModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => !card.IsChampionCard();

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var max = card.GetCardTypeValue() == CardType.Spell ? 5 : 4;
            var next = Math.Clamp(
                card.GetCostValue() + ModificationTuning.CostDeltas[rng.Next(ModificationTuning.CostDeltas.Length)],
                0,
                max);
            card.SetCost(next);
        }
    }
}
