using Random_Clan.Plugin.Constants;

namespace Random_Clan.Plugin.Extensions
{
    public static class IntExtensions
    {
        public static int MutateBalanced(this int value, Random rng)
        {
            if (rng.NextDouble() < ModificationTuning.BalancedFixedChance)
                return value.MutateFixed(rng, ModificationTuning.NumberSteps);
            return value.MutateScaled(rng, ModificationTuning.NumberScales);
        }

        /// <summary>
        /// Mutate with ~BeneficialChance of moving in the favored direction (up or down).
        /// </summary>
        public static int MutateToward(this int value, Random rng, bool favorIncrease)
        {
            var useFavored = rng.NextDouble() < ModificationTuning.BeneficialChance;
            var increase = useFavored ? favorIncrease : !favorIncrease;

            if (rng.NextDouble() < ModificationTuning.BalancedFixedChance)
            {
                var steps = increase
                    ? ModificationTuning.NumberStepsUp
                    : ModificationTuning.NumberStepsDown;
                return value.MutateFixed(rng, steps);
            }

            var scales = increase
                ? ModificationTuning.NumberScalesUp
                : ModificationTuning.NumberScalesDown;
            return value.MutateScaled(rng, scales);
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
