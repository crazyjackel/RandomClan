using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class AttackAdjustmentModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => (ctx.Character ?? card.GetSpawnCharacterData()) != null;

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null)
                return;
            character.SetAttackDamage(Math.Max(1, character.GetAttackDamage()).MutateBalanced(rng));
        }
    }
}
