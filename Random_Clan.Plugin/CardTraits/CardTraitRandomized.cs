namespace Random_Clan.Plugin.CardTraits
{
    /// <summary>Marker: at SetupRun, copy this card from a donor without Randomized.</summary>
    public sealed class CardTraitRandomized : CardTraitState
    {
        public override PropDescriptions CreateEditorInspectorDescriptions() => new();

        public override string GetCardText()
        {
            var text = LocalizeTraitKey("CardTraitRandomized_CardText");
            return string.IsNullOrEmpty(text) ? string.Empty : text;
        }

        public override string GetCardTooltipTitle()
            => LocalizeTraitKey("CardTraitRandomized_TooltipTitle");

        public override string GetCardTooltipText()
        {
            var text = "CardTraitRandomized_TooltipText".Localize();
            return string.IsNullOrEmpty(text) ? string.Empty : text;
        }
    }
}
