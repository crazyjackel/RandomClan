using TrainworksReloaded.Base;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.CardUpgrade;
using TrainworksReloaded.Base.Class;
using TrainworksReloaded.Base.Trait;

namespace Random_Clan.Plugin.Modifications
{
    public sealed class RandomizeContext
    {
        public required SaveManager SaveManager { get; init; }
        public required CardDataRegister Cards { get; init; }
        public required CardTraitDataRegister Traits { get; init; }
        public required ClassDataRegister Classes { get; init; }
        public required CardUpgradeRegister Upgrades { get; init; }
        public required GameDataClient Client { get; init; }
        public CharacterData? Character { get; set; }
        public List<CardUpgradeData> ChampionUpgradePool { get; set; } = [];
    }
}
