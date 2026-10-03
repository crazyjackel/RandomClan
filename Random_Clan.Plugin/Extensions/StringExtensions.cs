namespace Random_Clan.Plugin.Extensions
{
    public static class StringExtensions
    {
        public static string? TrySwapStatus(this string statusId, Random rng, IReadOnlyList<string> statusIds)
        {
            if (statusIds.Count < 2)
                return null;

            string next;
            do
            {
                next = statusIds[rng.Next(statusIds.Count)];
            } while (next.Equals(statusId, StringComparison.OrdinalIgnoreCase) && statusIds.Count > 1);

            return next;
        }
    }
}
