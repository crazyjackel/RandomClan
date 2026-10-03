using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Extensions;

namespace Random_Clan.Plugin
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new(MyPluginInfo.PLUGIN_GUID);
        
        // Plugin startup logic. This function is automatically called when your plugin initializes
        public void Awake()
        {
            Logger = base.Logger;

            var builder = Railhead.GetBuilder();
            builder.Configure(
                MyPluginInfo.PLUGIN_GUID,
                c =>
                {
                    // Be sure to include any new json files if you add more.
                    c.AddMergedJsonFile(
                        "json/global.json",
                        // Clan Definition
                        "json/clan/clan.json",
                        // Required: Clan Map Banner Node definition and Card Pool
                        "json/clan/clan_banner.json",
                        // Optional: Custom Card Border (if removed edit clan.json)
                        "json/clan/clan_card_frame.json",
                        // Optional: Custom Clan Subtypes used by units.
                        "json/clan/clan_subtypes.json",
                        // Required: A Clan Champion with one Upgrade Path.
                        "json/champions/basic_champion.json",
                        // Spell Cards
                        // Starter (required).
                        "json/spells/basic_starter.json",
                        "json/spells/basic_common.json",
                        // Units
                        "json/units/basic_ability_unit.json",
                        "json/units/basic_banner_unit.json",
                        // Non Banner Unit, that is draftable as a Battle Reward.
                        "json/units/basic_draft_unit.json",
                        // Equipment
                        "json/equipment/basic_equipment.json",
                        // Rooms
                        "json/rooms/basic_room.json",
                        // Shop Upgrades (Enhancer)
                        "json/enhancers/basic_enhancer.json",
                        // Artifacts
                        "json/relics/basic_artifact.json"
                    );
                }
            );

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

        }
    }
}
