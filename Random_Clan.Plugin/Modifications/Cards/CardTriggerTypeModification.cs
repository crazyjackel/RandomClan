using HarmonyLib;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class CardTriggerTypeModification : ICardModification
    {
        private static readonly CardTriggerType[] Options = Enum.GetValues(typeof(CardTriggerType))
            .Cast<CardTriggerType>()
            .ToArray();

        public bool CanModify(CardData card, RandomizeContext ctx)
            => card.GetTriggerList().Count > 0 && Options.Length > 1;

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var triggers = card.GetTriggerList();
            if (triggers.Count == 0)
                return;

            var trigger = triggers[rng.Next(triggers.Count)];
            var field = AccessTools.Field(typeof(CardTriggerEffectData), "trigger");
            if (field == null)
                return;

            var current = (CardTriggerType)field.GetValue(trigger)!;
            CardTriggerType next;
            do
            {
                next = Options[rng.Next(Options.Length)];
            } while (next.Equals(current) && Options.Length > 1);

            field.SetValue(trigger, next);
        }
    }
}
