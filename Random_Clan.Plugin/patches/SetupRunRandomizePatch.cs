using HarmonyLib;

namespace Random_Clan.Plugin.Patches
{
    [HarmonyPatch(typeof(SaveManager), "SetupRun")]
    internal static class SetupRunRandomizePatch
    {
        static void Prefix(SaveManager __instance)
        {
            try
            {
                var randomizer = Plugin.ClanRandomizer;
                if (randomizer == null)
                {
                    Plugin.Logger.LogError("ClanRandomizer was not resolved; skipping SetupRun randomize.");
                    return;
                }

                randomizer.Randomize(__instance);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Random Clan SetupRun randomize failed: {ex}");
            }
        }
    }
}
