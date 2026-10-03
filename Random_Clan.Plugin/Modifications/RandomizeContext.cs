using TrainworksReloaded.Base;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.CardUpgrade;
using TrainworksReloaded.Base.Class;
using TrainworksReloaded.Base.Trait;

namespace Random_Clan.Plugin.Modifications
{
    public sealed class RandomizeContext
    {
        public RandomizeContext(
            SaveManager saveManager,
            CardDataRegister cards,
            CardTraitDataRegister traits,
            ClassDataRegister classes,
            CardUpgradeRegister upgrades,
            GameDataClient client)
        {
            SaveManager = saveManager;
            Cards = cards;
            Traits = traits;
            Classes = classes;
            Upgrades = upgrades;
            Client = client;
        }

        public SaveManager SaveManager { get; }
        public CardDataRegister Cards { get; }
        public CardTraitDataRegister Traits { get; }
        public ClassDataRegister Classes { get; }
        public CardUpgradeRegister Upgrades { get; }
        public GameDataClient Client { get; }
        public CharacterData? Character { get; set; }
        public List<CardUpgradeData> ChampionUpgradePool { get; set; } = [];
    }
}
