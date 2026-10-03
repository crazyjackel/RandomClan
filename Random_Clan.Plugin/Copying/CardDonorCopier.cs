using HarmonyLib;
using Random_Clan.Plugin.Extensions;
using UnityEngine;

namespace Random_Clan.Plugin.Copying
{
    public sealed class CardDonorCopier
    {
        public void CopyOnto(CardData slot, CardData donor, IReadOnlyList<CardTraitData> markerTraits)
        {
            if (slot.GetCardTypeValue() == CardType.Monster)
                CopyUnitOnto(slot, donor);
            else
                CopySpellLikeOnto(slot, donor);

            var traits = slot.GetTraitList().Where(t => !t.IsMarkerTrait()).ToList();
            foreach (var marker in markerTraits)
            {
                if (marker != null && !traits.Contains(marker))
                    traits.Add(marker);
            }
            slot.SetTraits(traits);
        }

        private void CopySpellLikeOnto(CardData slot, CardData donor)
        {
            slot.SetNameKey(donor.GetNameKey());
            var desc = donor.GetDescriptionKey();
            if (!string.IsNullOrEmpty(desc))
                slot.SetDescriptionKey(desc!);
            var rarity = donor.GetRarityObject();
            if (rarity != null)
                slot.SetRarityObject(rarity);
            slot.SetCost(donor.GetCostValue());

            var donorEffects = donor.GetEffects() ?? [];
            var effectClones = new List<CardEffectData>(donorEffects.Count);
            foreach (var effect in donorEffects)
            {
                if (effect != null)
                    effectClones.Add(CloneEffect(effect));
            }
            slot.SetEffects(effectClones);

            var traitClones = new List<CardTraitData>();
            foreach (var trait in donor.GetTraitList())
            {
                if (trait != null && !trait.IsMarkerTrait())
                    traitClones.Add(trait);
            }
            slot.SetTraits(traitClones);

            var triggerClones = new List<CardTriggerEffectData>();
            foreach (var trigger in donor.GetTriggerList())
            {
                if (trigger != null)
                    triggerClones.Add(UnityEngine.Object.Instantiate(trigger));
            }
            slot.SetTriggers(triggerClones);
        }

        private void CopyUnitOnto(CardData slot, CardData donor)
        {
            slot.SetNameKey(donor.GetNameKey());
            var desc = donor.GetDescriptionKey();
            if (!string.IsNullOrEmpty(desc))
                slot.SetDescriptionKey(desc!);
            var rarity = donor.GetRarityObject();
            if (rarity != null)
                slot.SetRarityObject(rarity);
            slot.SetCost(donor.GetCostValue());

            var slotChar = slot.GetSpawnCharacterData();
            var donorChar = donor.GetSpawnCharacterData();
            if (slotChar == null || donorChar == null)
            {
                Plugin.Logger.LogWarning($"Unit copy skipped character: {slot.name} <- {donor.name}");
                return;
            }

            CopyCharacterMechanics(slotChar, donorChar, slot);
        }

        private void CopyCharacterMechanics(CharacterData slot, CharacterData donor, CardData slotCard)
        {
            slot.SetHealth(donor.GetHealth());
            slot.SetAttackDamage(donor.GetAttackDamage());
            slot.SetSize(donor.GetSize());
            slot.SetSubtypeKeys(new List<string>(donor.GetSubtypeKeys()));

            var triggerClones = new List<CharacterTriggerData>();
            foreach (var trigger in donor.GetTriggerList())
            {
                if (trigger != null)
                    triggerClones.Add(CloneCharacterTrigger(trigger));
            }
            slot.SetTriggers(triggerClones);
            slot.CopyStartingStatusesFrom(donor);

            var donorAbility = donor.GetUnitAbilityCardData();
            var slotAbility = slot.GetUnitAbilityCardData();
            if (donorAbility != null)
            {
                if (slotAbility != null)
                {
                    CopySpellLikeOnto(slotAbility, donorAbility);
                    slot.SetUnitAbility(slotAbility);
                }
                else
                {
                    var abilityClone = ScriptableObject.CreateInstance<CardData>();
                    abilityClone.name = $"{slotCard.name}_AbilityClone";
                    CopySpellLikeOnto(abilityClone, donorAbility);
                    abilityClone.SetIsUnitAbility(true);
                    slot.SetUnitAbility(abilityClone);
                }
            }

            slot.SetNameKey(donor.GetNameKey());
            var art = AccessTools.Field(typeof(CharacterData), "characterPrefabVariantRef")?.GetValue(donor);
            if (art != null)
                AccessTools.Field(typeof(CharacterData), "characterPrefabVariantRef")?.SetValue(slot, art);
            foreach (var vfx in new[] { "attackVFX", "impactVFX", "deathVFX" })
            {
                var field = AccessTools.Field(typeof(CharacterData), vfx);
                var value = field?.GetValue(donor);
                if (value != null)
                    field!.SetValue(slot, value);
            }
        }

        public static CardEffectData CloneEffect(CardEffectData source)
        {
            var clone = UnityEngine.Object.Instantiate(source);
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

        private static CharacterTriggerData CloneCharacterTrigger(CharacterTriggerData source)
        {
            var clone = UnityEngine.Object.Instantiate(source);
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
    }
}
