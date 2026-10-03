using HarmonyLib;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class CharacterTriggerTypeModification : ICardModification
    {
        private static readonly CharacterTriggerData.Trigger[] Options =
            Enum.GetValues(typeof(CharacterTriggerData.Trigger))
                .Cast<CharacterTriggerData.Trigger>()
                .ToArray();

        public bool CanModify(CardData card, RandomizeContext ctx)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            return character != null && character.GetTriggerList().Count > 0 && Options.Length > 1;
        }

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null)
                return;

            var triggers = character.GetTriggerList();
            if (triggers.Count == 0)
                return;

            var trigger = triggers[rng.Next(triggers.Count)];
            var field = AccessTools.Field(typeof(CharacterTriggerData), "trigger");
            if (field == null)
                return;

            var current = (CharacterTriggerData.Trigger)field.GetValue(trigger)!;
            CharacterTriggerData.Trigger next;
            do
            {
                next = Options[rng.Next(Options.Length)];
            } while (next.Equals(current) && Options.Length > 1);

            field.SetValue(trigger, next);
        }
    }
}
