using Random_Clan.Plugin.Copying;
using Random_Clan.Plugin.Extensions;

namespace Random_Clan.Plugin.Snapshots
{
    internal sealed class CardSnapshot
    {
        private readonly int _cost;
        private readonly string? _nameKey;
        private readonly string? _descKey;
        private readonly object? _rarity;
        private readonly List<CardEffectData> _effects;
        private readonly List<CardTraitData> _traits;
        private readonly List<CardTriggerEffectData> _triggers;
        private readonly CharacterSnapshot? _character;

        private CardSnapshot(
            int cost,
            string? nameKey,
            string? descKey,
            object? rarity,
            List<CardEffectData> effects,
            List<CardTraitData> traits,
            List<CardTriggerEffectData> triggers,
            CharacterSnapshot? character)
        {
            _cost = cost;
            _nameKey = nameKey;
            _descKey = descKey;
            _rarity = rarity;
            _effects = effects;
            _traits = traits;
            _triggers = triggers;
            _character = character;
        }

        public static CardSnapshot Capture(CardData card)
        {
            var effects = (card.GetEffects() ?? []).Select(DataCloner.CloneEffect).ToList();
            var traits = card.GetTraitList().Where(t => t != null).ToList();
            var triggers = card.GetTriggerList()
                .Where(t => t != null)
                .Select(DataCloner.CloneCardTrigger)
                .ToList();

            CharacterSnapshot? character = null;
            var spawn = card.GetSpawnCharacterData();
            if (spawn != null)
                character = CharacterSnapshot.Capture(spawn);

            return new CardSnapshot(
                card.GetCostValue(),
                card.GetNameKey(),
                card.GetDescriptionKey(),
                card.GetRarityObject(),
                effects,
                traits,
                triggers,
                character);
        }

        public void Restore(CardData card)
        {
            card.SetCost(_cost);
            if (_nameKey != null)
                card.SetNameKey(_nameKey);
            if (_descKey != null)
                card.SetDescriptionKey(_descKey);
            if (_rarity != null)
                card.SetRarityObject(_rarity);

            card.SetEffects(_effects.Select(DataCloner.CloneEffect).ToList());
            card.SetTraits(new List<CardTraitData>(_traits));
            card.SetTriggers(_triggers.Select(DataCloner.CloneCardTrigger).ToList());

            var spawn = card.GetSpawnCharacterData();
            if (spawn != null && _character != null)
                _character.Restore(spawn);
        }
    }

    internal sealed class CharacterSnapshot
    {
        private readonly int _health;
        private readonly int _attack;
        private readonly int _size;
        private readonly List<string> _subtypes;
        private readonly List<CharacterTriggerData> _triggers;
        private readonly CardSnapshot? _ability;

        private CharacterSnapshot(
            int health,
            int attack,
            int size,
            List<string> subtypes,
            List<CharacterTriggerData> triggers,
            CardSnapshot? ability)
        {
            _health = health;
            _attack = attack;
            _size = size;
            _subtypes = subtypes;
            _triggers = triggers;
            _ability = ability;
        }

        public static CharacterSnapshot Capture(CharacterData character)
        {
            var triggers = character.GetTriggerList()
                .Where(t => t != null)
                .Select(DataCloner.CloneCharacterTrigger)
                .ToList();

            CardSnapshot? ability = null;
            var abilityCard = character.GetUnitAbilityCardData();
            if (abilityCard != null)
                ability = CardSnapshot.Capture(abilityCard);

            return new CharacterSnapshot(
                character.GetHealth(),
                character.GetAttackDamage(),
                character.GetSize(),
                new List<string>(character.GetSubtypeKeys()),
                triggers,
                ability);
        }

        public void Restore(CharacterData character)
        {
            character.SetHealth(_health);
            character.SetAttackDamage(_attack);
            character.SetSize(_size);
            character.SetSubtypeKeys(new List<string>(_subtypes));
            character.SetTriggers(_triggers.Select(DataCloner.CloneCharacterTrigger).ToList());

            var abilityCard = character.GetUnitAbilityCardData();
            if (abilityCard != null && _ability != null)
                _ability.Restore(abilityCard);
        }
    }
}
