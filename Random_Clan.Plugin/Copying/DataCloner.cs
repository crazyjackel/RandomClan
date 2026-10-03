using System.Reflection;
using HarmonyLib;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Copying
{
    public static class DataCloner
    {
        private static readonly MethodInfo MemberwiseCloneMethod =
            typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!;

        public static T ShallowClone<T>(T source) where T : class
            => (T)MemberwiseCloneMethod.Invoke(source, null)!;

        public static CardEffectData CloneEffect(CardEffectData source)
        {
            var clone = ShallowClone(source);
            var statuses = source.GetStatusEffects();
            if (statuses != null)
            {
                var statusCopy = new StatusEffectStackData[statuses.Length];
                for (var i = 0; i < statuses.Length; i++)
                {
                    var s = statuses[i];
                    statusCopy[i] = new StatusEffectStackData
                    {
                        statusId = s.statusId,
                        count = s.count,
                        fromPermanentUpgrade = s.fromPermanentUpgrade
                    };
                }
                clone.SetStatusEffects(statusCopy);
            }
            return clone;
        }

        public static CharacterTriggerData CloneCharacterTrigger(CharacterTriggerData source)
        {
            var clone = ShallowClone(source);
            var effects = AccessTools.Field(typeof(CharacterTriggerData), "effects")?.GetValue(source) as List<CardEffectData>;
            if (effects != null)
            {
                var clones = new List<CardEffectData>(effects.Count);
                foreach (var effect in effects)
                {
                    if (effect != null)
                        clones.Add(CloneEffect(effect));
                }
                AccessTools.Field(typeof(CharacterTriggerData), "effects")?.SetValue(clone, clones);
            }
            return clone;
        }

        public static CardTriggerEffectData CloneCardTrigger(CardTriggerEffectData source)
        {
            var clone = ShallowClone(source);
            var effects = AccessTools.Field(typeof(CardTriggerEffectData), "cardEffects")?.GetValue(source) as List<CardEffectData>;
            if (effects != null)
            {
                var clones = new List<CardEffectData>(effects.Count);
                foreach (var effect in effects)
                {
                    if (effect != null)
                        clones.Add(CloneEffect(effect));
                }
                AccessTools.Field(typeof(CardTriggerEffectData), "cardEffects")?.SetValue(clone, clones);
            }
            return clone;
        }

        public static RoomModifierData CloneRoomModifier(RoomModifierData source)
            => ShallowClone(source);
    }
}
