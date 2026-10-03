namespace Random_Clan.Plugin.CardTraits
{
    /// <summary>Marker only: after copy, apply param_int room modifications.</summary>
    public sealed class CardTraitRoomModifier : CardTraitState
    {
        public override PropDescriptions CreateEditorInspectorDescriptions() => new();
    }
}
