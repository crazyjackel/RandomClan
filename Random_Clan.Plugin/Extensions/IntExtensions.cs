using Random_Clan.Plugin.Constants;

namespace Random_Clan.Plugin.Extensions
{
    public static class IntExtensions
    {
        public static int MutateBalanced(this int value, Random rng)
        {
            if (rng.NextDouble() < 0.5)
                return value.MutateFixed(rng, ModificationTuning.NumberSteps);
            return value.MutateScaled(rng, ModificationTuning.NumberScales);
        }

        public static int MutateFixed(this int value, Random rng)
            => value.MutateFixed(rng, ModificationTuning.StatSteps);

        public static int MutateFixed(this int value, Random rng, int[] steps)
            => Math.Max(1, value + steps[rng.Next(steps.Length)]);

        public static int MutateScaled(this int value, Random rng)
            => value.MutateScaled(rng, ModificationTuning.StatScales);

        public static int MutateScaled(this int value, Random rng, float[] scales)
            => Math.Max(1, (int)Math.Round(value * scales[rng.Next(scales.Length)]));
    }
}
