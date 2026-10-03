using Random_Clan.Plugin.Constants;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class SizeFixedModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => (ctx.Character ?? card.GetSpawnCharacterData()) != null;

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null)
                return;

            var size = character.GetSize() + ModificationTuning.SizeSteps[rng.Next(ModificationTuning.SizeSteps.Length)];
            character.SetSize(Math.Clamp(size, ModificationTuning.MinSize, ModificationTuning.MaxSize));
        }
    }
}
