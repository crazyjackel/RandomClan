using TrainworksReloaded.Base;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.CardUpgrade;
using TrainworksReloaded.Base.Class;
using TrainworksReloaded.Base.StatusEffects;
using TrainworksReloaded.Base.Trait;
using TrainworksReloaded.Core.Enum;

namespace Random_Clan.Plugin.Modifications
{
    public sealed class RandomizeContext
    {
        private IReadOnlyList<string>? _statusIds;

        public RandomizeContext(
            SaveManager saveManager,
            CardDataRegister cards,
            CardTraitDataRegister traits,
            ClassDataRegister classes,
            CardUpgradeRegister upgrades,
            StatusEffectDataRegister statuses,
            GameDataClient client)
        {
            SaveManager = saveManager;
            Cards = cards;
            Traits = traits;
            Classes = classes;
            Upgrades = upgrades;
            Statuses = statuses;
            Client = client;
        }

        public SaveManager SaveManager { get; }
        public CardDataRegister Cards { get; }
        public CardTraitDataRegister Traits { get; }
        public ClassDataRegister Classes { get; }
        public CardUpgradeRegister Upgrades { get; }
        public StatusEffectDataRegister Statuses { get; }
        public GameDataClient Client { get; }
        public CharacterData? Character { get; set; }
        public List<CardUpgradeData> ChampionUpgradePool { get; set; } = [];

        public IReadOnlyList<string> StatusIds
            => _statusIds ??= Statuses.GetAllIdentifiers(RegisterIdentifierType.ReadableID);
    }
}
