using Random_Clan.Plugin.Modifications.Cards;
using Random_Clan.Plugin.Modifications.Champions;
using Random_Clan.Plugin.Modifications.Rooms;
using Random_Clan.Plugin.Modifications.Units;

namespace Random_Clan.Plugin.Modifications
{
    public sealed class ModificationRegistry
    {
        private const float HighWeight = 2f;
        private const float DefaultWeight = 1f;
        private const float LowWeight = 0.5f;

        private readonly List<ICardModification> _modifications = [];

        public ModificationRegistry()
        {
            Register(new CostAdjustmentModification());
            Register(new NumberFixedModification());
            Register(new NumberScaledModification());
            Register(new CardStatusCountModification());
            Register(new CardStatusSwapModification());
            Register(new CardStatusInjectModification());
            Register(new CardTriggerTypeModification());
            Register(new TraitParamModification());
            Register(new TraitInjectModification());
            Register(new TraitRemoveModification());

            Register(new AbilityStatusCountModification());
            Register(new AbilityStatusSwapModification());
            Register(new StartingStatusCountModification());
            Register(new StartingStatusSwapModification());
            Register(new StartingStatusInjectModification());
            Register(new AbilitySwapModification());
            Register(new CharacterTriggerTypeModification());
            Register(new SubtypeAddModification());
            Register(new SubtypeRemoveModification());
            Register(new HealthFixedModification());
            Register(new HealthMultiplicativeModification());
            Register(new AttackFixedModification());
            Register(new AttackMultiplicativeModification());
            Register(new SizeFixedModification());

            Register(new RoomUpgradeNumberModification());
            Register(new ChampionUpgradeTreeModification());
        }

        public void Register(ICardModification modification) => _modifications.Add(modification);

        public IReadOnlyList<ICardModification> All => _modifications;

        public List<ICardModification> Pick(CardData card, RandomizeContext ctx, Random rng, int count)
        {
            if (count <= 0)
                return [];

            var remaining = _modifications.Where(m => m.CanModify(card, ctx)).ToList();
            var picked = new List<ICardModification>(Math.Min(count, remaining.Count));

            while (picked.Count < count && remaining.Count > 0)
            {
                var total = 0f;
                for (var i = 0; i < remaining.Count; i++)
                    total += GetWeight(remaining[i]);

                var roll = (float)(rng.NextDouble() * total);
                var acc = 0f;
                var index = remaining.Count - 1;
                for (var i = 0; i < remaining.Count; i++)
                {
                    acc += GetWeight(remaining[i]);
                    if (roll < acc)
                    {
                        index = i;
                        break;
                    }
                }

                picked.Add(remaining[index]);
                remaining.RemoveAt(index);
            }

            return picked;
        }

        private static float GetWeight(ICardModification modification) => modification switch
        {
            CostAdjustmentModification => HighWeight,
            NumberFixedModification => HighWeight,
            NumberScaledModification => HighWeight,
            HealthFixedModification => HighWeight,
            HealthMultiplicativeModification => HighWeight,
            AttackFixedModification => HighWeight,
            AttackMultiplicativeModification => HighWeight,
            CardStatusCountModification => HighWeight,
            AbilityStatusCountModification => HighWeight,
            StartingStatusCountModification => HighWeight,
            RoomUpgradeNumberModification => HighWeight,

            TraitRemoveModification => LowWeight,
            SubtypeRemoveModification => LowWeight,
            CardTriggerTypeModification => LowWeight,
            CharacterTriggerTypeModification => LowWeight,

            _ => DefaultWeight,
        };
    }
}
