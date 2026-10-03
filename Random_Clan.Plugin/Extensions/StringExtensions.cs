using Random_Clan.Plugin.Constants;

namespace Random_Clan.Plugin.Extensions
{
    public static class StringExtensions
    {
        public static string? TrySwapStatus(this string statusId, Random rng)
        {
            foreach (var family in StatusFamilies.Families)
            {
                var idx = Array.FindIndex(family, s => s.Equals(statusId, StringComparison.OrdinalIgnoreCase));
                if (idx < 0)
                    continue;
                if (family.Length < 2)
                    return null;
                string next;
                do
                {
                    next = family[rng.Next(family.Length)];
                } while (next.Equals(statusId, StringComparison.OrdinalIgnoreCase) && family.Length > 1);
                return next;
            }
            return null;
        }
    }
}