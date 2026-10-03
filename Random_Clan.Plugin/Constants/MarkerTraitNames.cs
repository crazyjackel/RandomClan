using Random_Clan.Plugin.CardTraits;

namespace Random_Clan.Plugin.Constants
{
    public static class MarkerTraitNames
    {
        public const string Randomized = nameof(CardTraitRandomized);
        public const string SpellModifier = nameof(CardTraitSpellModifier);
        public const string UnitModifier = nameof(CardTraitUnitModifier);
        public const string RoomModifier = nameof(CardTraitRoomModifier);
        public const string ChampionModifier = nameof(CardTraitChampionModifier);

        public static readonly string[] ModifierStates =
        [
            SpellModifier,
            UnitModifier,
            RoomModifier,
            ChampionModifier,
        ];
    }
}
