using HarmonyLib;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Cards
{
    public sealed class CardTriggerTypeModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => card.GetTriggerList().Count > 0 && ctx.SwappableCardTriggers.Count > 1;

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var triggers = card.GetTriggerList();
            if (triggers.Count == 0 || ctx.SwappableCardTriggers.Count < 2)
                return;

            var trigger = triggers[rng.Next(triggers.Count)];
            var field = AccessTools.Field(typeof(CardTriggerEffectData), "trigger");
            if (field == null)
                return;

            var current = (CardTriggerType)field.GetValue(trigger)!;
            var options = ctx.SwappableCardTriggers;
            CardTriggerType next;
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
