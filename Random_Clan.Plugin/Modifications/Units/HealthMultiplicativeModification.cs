using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class HealthMultiplicativeModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => (ctx.Character ?? card.GetSpawnCharacterData()) != null;

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null)
                return;
            character.SetHealth(Math.Max(1, character.GetHealth()).MutateScaled(rng));
        }
    }
}
