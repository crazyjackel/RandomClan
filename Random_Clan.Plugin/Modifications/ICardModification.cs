namespace Random_Clan.Plugin.Modifications
{
    public interface ICardModification
    {
        bool CanModify(CardData card, RandomizeContext ctx);
        void Modify(CardData card, RandomizeContext ctx, Random rng);
    }
}
