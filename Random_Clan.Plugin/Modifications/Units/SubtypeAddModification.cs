using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class SubtypeAddModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null || ctx.MutableSubtypeKeys.Count == 0)
                return false;

            var existing = character.GetSubtypeKeys();
            return ctx.MutableSubtypeKeys.Any(key => !existing.Contains(key));
        }

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null)
                return;

            var existing = character.GetSubtypeKeys();
            var candidates = ctx.MutableSubtypeKeys.Where(key => !existing.Contains(key)).ToList();
            if (candidates.Count == 0)
                return;

            var next = new List<string>(existing) { candidates[rng.Next(candidates.Count)] };
            character.SetSubtypeKeys(next);
        }
    }
}
