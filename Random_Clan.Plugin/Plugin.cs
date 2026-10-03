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
    [BepInDependency("TrainworksReloaded.Plugin")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new(MyPluginInfo.PLUGIN_GUID);
        internal static ClanRandomizer? ClanRandomizer { get; private set; }

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
                        "json/spells/champion_a_starter_card.json",
                        "json/spells/basic_common_spell_card.json",
                        "json/spells/errant_bolt.json",
                        "json/spells/capricious_flare.json",
                        "json/spells/whimfire.json",
                        "json/spells/scattershot_curse.json",
                        "json/spells/haphazard_jolt.json",
                        "json/spells/dicebolt.json",
                        "json/spells/luckless_brand.json",
                        "json/spells/fickle_tempest.json",
                        "json/spells/arbitrary_hex.json",
                        "json/spells/roulette_lance.json",
                        "json/spells/happenstance_wave.json",
                        "json/spells/unfixed_blight.json",
                        "json/spells/contingent_spark.json",
                        "json/spells/chaos_cataclysm.json",
                        "json/spells/entropic_nova.json",
                        "json/spells/wild_decree.json",
                        "json/spells/fortuitous_ruin.json",
                        "json/units/basic_banner_unit.json",
                        "json/units/roaming_shade.json",
                        "json/units/errant_goblin.json",
                        "json/units/wandering_brute.json",
                        "json/units/unmoored_imp.json",
                        "json/units/stray_hound.json",
                        "json/units/basic_ability_unit.json",
                        "json/units/rambling_colossus.json",
                        "json/units/indeterminate_fiend.json",
                        "json/units/wayward_grunt.json",
                        "json/units/vagrant_tick.json",
                        "json/units/basic_draft_unit.json",
                        "json/units/loose_wisp.json",
                        "json/units/mislaid_gremlin.json",
                        "json/units/chance_larva.json",
                        "json/units/arbitrary_horror.json",
                        "json/units/stochastic_beast.json",
                        "json/rooms/rooms.json",
                        "json/equipment/basic_equipment.json",
                        "json/enhancers/basic_enhancer.json"
                    );
                }
            );

            // Register before any PostAction resolves services (SimpleInjector locks on first GetInstance).
            Railend.ConfigurePreAction(RegisterServices);
            Railend.ConfigurePostAction(ResolveServices);

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

        private static void ResolveServices(Container container)
        {
            ClanRandomizer = container.GetInstance<ClanRandomizer>();
            Logger.LogInfo("Resolved ClanRandomizer from Railend container.");
        }
    }
}
