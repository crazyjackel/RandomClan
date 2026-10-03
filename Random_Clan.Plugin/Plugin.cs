using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Random_Clan.Plugin.Copying;
using Random_Clan.Plugin.Modifications;
using SimpleInjector;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Extensions;

namespace Random_Clan.Plugin
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new(MyPluginInfo.PLUGIN_GUID);

        public void Awake()
        {
            Logger = base.Logger;

            var builder = Railhead.GetBuilder();
            builder.Configure(
                MyPluginInfo.PLUGIN_GUID,
                c =>
                {
                    c.AddMergedJsonFile(
                        "json/global.json",
                        "json/traits/markers.json",
                        "json/clan/clan.json",
                        "json/clan/clan_banner.json",
                        "json/clan/clan_card_frame.json",
                        "json/clan/clan_subtypes.json",
                        "json/champions/basic_champion.json",
                        "json/spells/basic_starter.json",
                        "json/spells/basic_common.json",
                        "json/units/basic_ability_unit.json",
                        "json/units/basic_banner_unit.json",
                        "json/units/basic_draft_unit.json",
                        "json/equipment/basic_equipment.json",
                        "json/rooms/basic_room.json",
                        "json/enhancers/basic_enhancer.json"
                    );
                }
            );

            Railend.ConfigurePostAction(RegisterServices);

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }

        private static void RegisterServices(Container container)
        {
            container.RegisterSingleton<ModificationRegistry>();
            container.RegisterSingleton<CardDonorCopier>();
            container.RegisterSingleton<ClanRandomizer>();
            Logger.LogInfo("Registered Random Clan Railend services.");
        }
    }
}
