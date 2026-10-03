using HarmonyLib;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Champions
{
    public sealed class ChampionUpgradeTreeModification : ICardModification
    {
        public bool CanModify(CardData card, RandomizeContext ctx)
            => card.IsChampionCard() && ctx.ChampionUpgradePool.Count > 0;

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            foreach (var classData in ctx.SaveManager.GetAllGameData().GetAllClassDatas())
            {
                var champions = AccessTools.Field(typeof(ClassData), "champions")?.GetValue(classData) as List<ChampionData>;
                if (champions == null)
                    continue;

                foreach (var champion in champions)
                {
                    if (champion.championCardData != card)
                        continue;

                    var tree = AccessTools.Field(typeof(ChampionData), "upgradeTree")?.GetValue(champion) as CardUpgradeTreeData;
                    if (tree == null)
                        continue;

                    var upgradeTrees = AccessTools.Field(typeof(CardUpgradeTreeData), "upgradeTrees")?.GetValue(tree)
                        as List<CardUpgradeTreeData.UpgradeTree>;
                    if (upgradeTrees == null || upgradeTrees.Count == 0)
                    {
                        upgradeTrees = [new CardUpgradeTreeData.UpgradeTree()];
                        AccessTools.Field(typeof(CardUpgradeTreeData), "upgradeTrees")?.SetValue(tree, upgradeTrees);
                    }

                    foreach (var upgradeTree in upgradeTrees)
                    {
                        var available = ctx.ChampionUpgradePool.ToList();
                        var picked = new List<CardUpgradeData>();
                        while (picked.Count < 3 && available.Count > 0)
                        {
                            var idx = rng.Next(available.Count);
                            picked.Add(available[idx]);
                            available.RemoveAt(idx);
                        }
                        picked.Sort((a, b) => (a.GetBonusDamage() + a.GetBonusHP()).CompareTo(b.GetBonusDamage() + b.GetBonusHP()));
                        AccessTools.Field(typeof(CardUpgradeTreeData.UpgradeTree), "cardUpgrades")?.SetValue(upgradeTree, picked);
                        Plugin.Logger.LogInfo($"Champion tree rebuilt with {picked.Count} upgrades for {card.name}");
                    }
                }
            }
        }
    }
}
