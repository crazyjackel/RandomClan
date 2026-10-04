using HarmonyLib;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Modifications.Champions
{
    public sealed class ChampionUpgradeTreeModification : ICardModification
    {
        private const int PathCount = 3;
        private const int TiersPerPath = 3;

        public bool CanModify(CardData card, RandomizeContext ctx)
            => card.IsChampionCard() && ctx.ChampionUpgradePool.Count > 0;

        /// <summary>Strip placeholder / prior-run paths so champion chaos never keeps defaults.</summary>
        public static void ClearUpgradeTrees(CardData card, RandomizeContext ctx)
        {
            foreach (var champion in FindChampionEntries(card, ctx))
            {
                var tree = AccessTools.Field(typeof(ChampionData), "upgradeTree")?.GetValue(champion) as CardUpgradeTreeData;
                if (tree == null)
                    continue;

                var empty = new List<CardUpgradeTreeData.UpgradeTree>(PathCount);
                for (var path = 0; path < PathCount; path++)
                {
                    var upgradeTree = new CardUpgradeTreeData.UpgradeTree();
                    AccessTools.Field(typeof(CardUpgradeTreeData.UpgradeTree), "cardUpgrades")
                        ?.SetValue(upgradeTree, new List<CardUpgradeData>());
                    empty.Add(upgradeTree);
                }

                AccessTools.Field(typeof(CardUpgradeTreeData), "upgradeTrees")?.SetValue(tree, empty);
            }
        }

        public void Modify(CardData card, RandomizeContext ctx, Random rng)
        {
            ClearUpgradeTrees(card, ctx);

            foreach (var champion in FindChampionEntries(card, ctx))
            {
                var tree = AccessTools.Field(typeof(ChampionData), "upgradeTree")?.GetValue(champion) as CardUpgradeTreeData;
                if (tree == null)
                    continue;

                var available = ctx.ChampionUpgradePool.ToList();
                Shuffle(available, rng);

                var upgradeTrees = new List<CardUpgradeTreeData.UpgradeTree>(PathCount);
                for (var path = 0; path < PathCount; path++)
                {
                    var picked = TakeUpgrades(available, ctx.ChampionUpgradePool, rng, TiersPerPath);
                    picked.Sort((a, b) =>
                        (a.GetBonusDamage() + a.GetBonusHP()).CompareTo(b.GetBonusDamage() + b.GetBonusHP()));

                    var upgradeTree = new CardUpgradeTreeData.UpgradeTree();
                    AccessTools.Field(typeof(CardUpgradeTreeData.UpgradeTree), "cardUpgrades")
                        ?.SetValue(upgradeTree, picked);
                    upgradeTrees.Add(upgradeTree);
                }

                AccessTools.Field(typeof(CardUpgradeTreeData), "upgradeTrees")?.SetValue(tree, upgradeTrees);
                Plugin.Logger.LogInfo(
                    $"Champion tree cleared then rebuilt with {PathCount} paths x {TiersPerPath} tiers for {card.name}");
            }
        }

        private static IEnumerable<ChampionData> FindChampionEntries(CardData card, RandomizeContext ctx)
        {
            foreach (var classData in ctx.SaveManager.GetAllGameData().GetAllClassDatas())
            {
                var champions = AccessTools.Field(typeof(ClassData), "champions")?.GetValue(classData) as List<ChampionData>;
                if (champions == null)
                    continue;

                foreach (var champion in champions)
                {
                    if (champion.championCardData == card)
                        yield return champion;
                }
            }
        }

        private static List<CardUpgradeData> TakeUpgrades(
            List<CardUpgradeData> available,
            IReadOnlyList<CardUpgradeData> fallbackPool,
            Random rng,
            int count)
        {
            var picked = new List<CardUpgradeData>(count);
            while (picked.Count < count)
            {
                if (available.Count == 0)
                {
                    if (fallbackPool.Count == 0)
                        break;
                    available.AddRange(fallbackPool);
                    Shuffle(available, rng);
                }

                var idx = rng.Next(available.Count);
                picked.Add(available[idx]);
                available.RemoveAt(idx);
            }
            return picked;
        }

        private static void Shuffle<T>(IList<T> list, Random rng)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
