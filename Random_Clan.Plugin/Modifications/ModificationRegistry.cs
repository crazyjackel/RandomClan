using Random_Clan.Plugin.Modifications.Cards;
using Random_Clan.Plugin.Modifications.Champions;
using Random_Clan.Plugin.Modifications.Rooms;
using Random_Clan.Plugin.Modifications.Units;

namespace Random_Clan.Plugin.Modifications
{
    public sealed class ModificationRegistry
    {
        private readonly List<ICardModification> _modifications = [];

        public ModificationRegistry()
        {
            Register(new CostAdjustmentModification());
            Register(new NumberAdjustmentModification());
            Register(new StatusCountModification());
            Register(new StatusSwapModification());
            Register(new TraitParamModification());
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

            var eligible = _modifications.Where(m => m.CanModify(card, ctx)).ToList();
            for (var i = eligible.Count - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);
                (eligible[i], eligible[j]) = (eligible[j], eligible[i]);
            }

            return eligible.Take(count).ToList();
        }
    }
}
