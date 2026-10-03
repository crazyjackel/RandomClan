using HarmonyLib;
using Random_Clan.Plugin.Extensions;
using TrainworksReloaded.Base.Prefab;
using UnityEngine;

namespace Random_Clan.Plugin.Copying
{
    public sealed class CardDonorCopier
    {
        private readonly GlitchedArtFactory _glitchedArt = new();

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
            var desc = donor.GetDescriptionKey();
            if (!string.IsNullOrEmpty(desc))
                slot.SetDescriptionKey(desc!);
            var rarity = donor.GetRarityObject();
            if (rarity != null)
                slot.SetRarityObject(rarity);
            slot.SetCost(donor.GetCostValue());
            slot.CopyCardArtFrom(donor);

            var donorEffects = donor.GetEffects() ?? [];
            var effectClones = new List<CardEffectData>(donorEffects.Count);
            foreach (var effect in donorEffects)
            {
                if (effect != null)
                    effectClones.Add(DataCloner.CloneEffect(effect));
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
                    triggerClones.Add(DataCloner.CloneCardTrigger(trigger));
            }
            slot.SetTriggers(triggerClones);
        }

        private void CopyUnitOnto(CardData slot, CardData donor)
        {
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
                slot.CopyCardArtFrom(donor);
                return;
            }

            CopyCharacterMechanics(slotChar, donorChar, slot);
            _glitchedArt.Apply(slot, donor, slotChar, donorChar);
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
                    triggerClones.Add(DataCloner.CloneCharacterTrigger(trigger));
            }
            slot.SetTriggers(triggerClones);
            slot.CopyStartingStatusesFrom(donor);

            var donorAbility = donor.GetUnitAbilityCardData();
            if (donorAbility != null)
                slot.SetUnitAbility(CloneAbilityCard(donorAbility, $"{slotCard.name}_AbilityClone"));
            else
                slot.SetUnitAbility(null);

            foreach (var vfx in new[] { "attackVFX", "impactVFX", "deathVFX" })
            {
                var field = AccessTools.Field(typeof(CharacterData), vfx);
                var value = field?.GetValue(donor);
                if (value != null)
                    field!.SetValue(slot, value);
            }
        }

        public static CardData CloneAbilityCard(CardData donorAbility, string name)
        {
            // Instantiate preserves CardData asset fields (VFX, art, etc.) that CreateInstance leaves null.
            var clone = UnityEngine.Object.Instantiate(donorAbility);
            clone.name = name;
            clone.SetIsUnitAbility(true);
            EnsureCardVfxDefaults(clone);

            var donorEffects = donorAbility.GetEffects() ?? [];
            var effectClones = new List<CardEffectData>(donorEffects.Count);
            foreach (var effect in donorEffects)
            {
                if (effect != null)
                    effectClones.Add(DataCloner.CloneEffect(effect));
            }
            clone.SetEffects(effectClones);

            var traitClones = new List<CardTraitData>();
            foreach (var trait in donorAbility.GetTraitList())
            {
                if (trait != null && !trait.IsMarkerTrait())
                    traitClones.Add(trait);
            }
            clone.SetTraits(traitClones);

            var triggerClones = new List<CardTriggerEffectData>();
            foreach (var trigger in donorAbility.GetTriggerList())
            {
                if (trigger != null)
                    triggerClones.Add(DataCloner.CloneCardTrigger(trigger));
            }
            clone.SetTriggers(triggerClones);

            return clone;
        }

        private static void EnsureCardVfxDefaults(CardData card)
        {
            foreach (var fieldName in new[] { "specialEdgeVFX", "offCooldownVFX" })
            {
                var field = AccessTools.Field(typeof(CardData), fieldName);
                if (field == null || field.GetValue(card) != null)
                    continue;
                field.SetValue(card, VfxRegister.Default);
            }
        }
    }
}
