using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class StartingStatusInjectModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => ctx.StatusIds.Count > 0 && (ctx.Character ?? card.GetSpawnCharacterData()) != null;

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null || ctx.StatusIds.Count == 0)
                return;

            var existing = character.GetStartingStatusEffectsArray();
            var next = new StatusEffectStackData[existing.Length + 1];
            for (var i = 0; i < existing.Length; i++)
                next[i] = existing[i];

            next[existing.Length] = new StatusEffectStackData
            {
                statusId = ctx.StatusIds[rng.Next(ctx.StatusIds.Count)],
                count = Math.Max(1, 1.MutateBalanced(rng)),
            };
            character.SetStartingStatusEffectsArray(next);
        }
    }
}
