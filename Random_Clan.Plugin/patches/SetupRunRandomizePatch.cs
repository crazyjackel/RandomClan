using HarmonyLib;
using TrainworksReloaded.Core;

namespace Random_Clan.Plugin.Patches
{
    [HarmonyPatch(typeof(SaveManager), "SetupRun")]
    internal static class SetupRunRandomizePatch
    {
        static void Prefix(SaveManager __instance)
        {
            try
            {
                Railend.GetContainer().GetInstance<ClanRandomizer>().Randomize(__instance);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Random Clan SetupRun randomize failed: {ex}");
            }
        }
    }
}
