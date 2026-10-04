using UnityEngine;

namespace Random_Clan.Plugin.Constants
{
    public static class ModificationTuning
    {
        /// <summary>Chance to pick the beneficial direction for polarity / toward-mutations.</summary>
        public const double BeneficialChance = 0.7;

        /// <summary>Chance MutateBalanced uses fixed steps (else scales).</summary>
        public const double BalancedFixedChance = 0.35;

        public static readonly int[] NumberSteps = [-2, -1, 1, 1, 2, 2];
        public static readonly float[] NumberScales = [0.5f, 1.5f, 1.5f, 2f, 2f];
        public static readonly int[] StatSteps = [-5, -3, -1, 1, 2, 3, 5, 5];
        public static readonly float[] StatScales = [0.5f, 0.75f, 1.25f, 1.5f, 1.5f, 2f, 2f];
        public static readonly int[] CostDeltas = [-1, -1, -1, 1];
        public static readonly int[] SizeSteps = [-2, -1, -1, 1];

        public static readonly int[] NumberStepsUp = [1, 1, 2, 2];
        public static readonly int[] NumberStepsDown = [-2, -1];
        public static readonly float[] NumberScalesUp = [1.5f, 1.5f, 2f, 2f];
        public static readonly float[] NumberScalesDown = [0.5f];

        public const int MinSize = 1;
        public const int MaxSize = 6;

        /// <summary>CardEffects Distortion = 2 (CardEffectsMaterial.EffectType).</summary>
        public const int CardDistortionEffectType = 2;
        public const int CardEffectsMaxLayers = 8;
        public const string CardEffectsShaderName = "Shiny Shoe/CardEffects";
        public const string CardEffectsTemplateMaterialName = "CardMaterial_PunkrockReveler";

        public static readonly Color CardGlitchLayerTint = new(0.55f, 1.15f, 1.35f, 1f);
        public static readonly Vector2 CardGlitchLinearSpeed = new(0.35f, -0.2f);
        public static readonly Vector2 CardGlitchScale = new(1.15f, 1.15f);

        public static readonly Color CharacterGlitchTint = new(0.65f, 1.25f, 1.45f, 1f);
        public const float CharacterGlitchGrayscale = 0.35f;
    }
}


