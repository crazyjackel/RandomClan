namespace Random_Clan.Plugin.CardTraits
{
    /// <summary>Marker only: after copy, apply param_int unit modifications.</summary>
    public sealed class CardTraitUnitModifier : CardTraitState
    {
        public override PropDescriptions CreateEditorInspectorDescriptions() => new();
    }
}
