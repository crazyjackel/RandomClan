namespace Random_Clan.Plugin.CardTraits
{
    /// <summary>Shared card text / tooltips for *ModifierN marker traits (param_int = N).</summary>
    public abstract class CardTraitModifierBase : CardTraitState
    {
        protected abstract string LocalizationPrefix { get; }

        public override PropDescriptions CreateEditorInspectorDescriptions()
            => new()
            {
                [CardTraitFieldNames.ParamInt.GetFieldName()] = new PropDescription("Modification count"),
            };

        public override string GetCardText()
        {
            var text = LocalizeTraitKey($"{LocalizationPrefix}_CardText");
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return string.Format(text, GetParamInt());
        }

        public override string GetCardTooltipTitle()
            => LocalizeTraitKey($"{LocalizationPrefix}_TooltipTitle");

        public override string GetCardTooltipText()
        {
            var text = $"{LocalizationPrefix}_TooltipText".Localize();
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return string.Format(text, GetParamInt());
        }
    }
}
