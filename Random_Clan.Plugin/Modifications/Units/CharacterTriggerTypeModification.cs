using HarmonyLib;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Units
{
    public sealed class CharacterTriggerTypeModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
        {
            if (ctx.SwappableCharacterTriggers.Count < 2)
                return false;
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            return character != null && character.GetTriggerList().Count > 0;
        }

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var character = ctx.Character ?? card.GetSpawnCharacterData();
            if (character == null || ctx.SwappableCharacterTriggers.Count < 2)
                return;

            var triggers = character.GetTriggerList();
            if (triggers.Count == 0)
                return;

            var trigger = triggers[rng.Next(triggers.Count)];
            var field = AccessTools.Field(typeof(CharacterTriggerData), "trigger");
            if (field == null)
                return;

            var current = (CharacterTriggerData.Trigger)field.GetValue(trigger)!;
            var options = ctx.SwappableCharacterTriggers;
            CharacterTriggerData.Trigger next;
            var attempts = 0;
            do
            {
                next = options[rng.Next(options.Count)];
                attempts++;
            } while (next.Equals(current) && attempts < 8);

            field.SetValue(trigger, next);
        }
    }
}
