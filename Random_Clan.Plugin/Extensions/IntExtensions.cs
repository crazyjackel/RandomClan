using Random_Clan.Plugin.Constants;

namespace Random_Clan.Plugin.Extensions
{
    public static class IntExtensions
    {
        public static int MutateBalanced(this int value, Random rng)
        {
            if (rng.NextDouble() < 0.5)
                return Math.Max(1, value + ModificationTuning.NumberSteps[rng.Next(ModificationTuning.NumberSteps.Length)]);
            return Math.Max(1, (int)Math.Round(value * ModificationTuning.NumberScales[rng.Next(ModificationTuning.NumberScales.Length)]));
        }
    }
}
