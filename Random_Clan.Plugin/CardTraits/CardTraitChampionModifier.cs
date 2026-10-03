namespace Random_Clan.Plugin.CardTraits
{
    /// <summary>Marker only: after copy, apply param_int champion modifications.</summary>
    public sealed class CardTraitChampionModifier : CardTraitState
    {
        public override PropDescriptions CreateEditorInspectorDescriptions() => new();
    }
}
