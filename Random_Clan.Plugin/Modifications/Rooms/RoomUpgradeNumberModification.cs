using HarmonyLib;
using Random_Clan.Plugin.Copying;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Rooms
{
    public sealed class RoomUpgradeNumberModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
        {
            if (card.GetCardTypeValue() != CardType.TrainRoomAttachment)
                return false;
            var effects = card.GetEffects();
            return effects != null && effects.Any(e => e != null && e.GetCardUpgrade() != null);
        }

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            var effects = card.GetEffects();
            if (effects == null)
                return;

            foreach (var effect in effects)
            {
                if (effect == null)
                    continue;
                var upgrade = effect.GetCardUpgrade();
                if (upgrade == null)
                    continue;

                var clone = UnityEngine.Object.Instantiate(upgrade);
                effect.SetCardUpgrade(clone);

                var dmg = clone.GetBonusDamage();
                var hp = clone.GetBonusHP();
                if (dmg != 0)
                    AccessTools.Field(typeof(CardUpgradeData), "bonusDamage")?.SetValue(clone, Math.Abs(dmg).MutateBalanced(rng) * Math.Sign(dmg));
                if (hp != 0)
                    AccessTools.Field(typeof(CardUpgradeData), "bonusHP")?.SetValue(clone, Math.Abs(hp).MutateBalanced(rng) * Math.Sign(hp));

                var roomMods = AccessTools.Field(typeof(CardUpgradeData), "roomModifierUpgrades")?.GetValue(clone) as List<RoomModifierData>;
                if (roomMods == null)
                    continue;

                var modClones = new List<RoomModifierData>(roomMods.Count);
                foreach (var mod in roomMods)
                {
                    if (mod == null)
                        continue;
                    var modClone = DataCloner.CloneRoomModifier(mod);
                    var paramInt = (int)(AccessTools.Field(typeof(RoomModifierData), "paramInt")?.GetValue(modClone) ?? 0);
                    if (paramInt != 0)
                        AccessTools.Field(typeof(RoomModifierData), "paramInt")?.SetValue(modClone, Math.Abs(paramInt).MutateBalanced(rng) * Math.Sign(paramInt));
                    modClones.Add(modClone);
                }
                AccessTools.Field(typeof(CardUpgradeData), "roomModifierUpgrades")?.SetValue(clone, modClones);
            }
        }
    }
}
