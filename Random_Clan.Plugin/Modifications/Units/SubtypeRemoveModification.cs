using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class SubtypeRemoveModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null)
                return false;
            return character.GetSubtypeKeys().Any(IsRemovable);
        }

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null)
                return;

            var existing = character.GetSubtypeKeys();
            var removable = existing.Where(IsRemovable).ToList();
            if (removable.Count == 0)
                return;

            var remove = removable[rng.Next(removable.Count)];
            character.SetSubtypeKeys(existing.Where(s => s != remove).ToList());
        }

        private static bool IsRemovable(string key)
            => !key.Contains("BannerUnit", StringComparison.OrdinalIgnoreCase)
               && !key.Contains("Champion", StringComparison.OrdinalIgnoreCase);
    }
}
