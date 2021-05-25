/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elfuun1/FlowerDanceFix
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FlowerDanceFix
{
    public class ModEntry : Mod
    {
        private static ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.GameLaunched += onLaunched;
            Config = Helper.ReadConfig<ModConfig>();

            // Initialize Patches
            EventPatched.Initialize(Monitor, Config, Helper);
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch
            (
               original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.setUpFestivalMainEvent)),
               postfix: new HarmonyMethod(typeof(EventPatched), nameof(EventPatched.setUpFestivalMainEvent_FDF))
            );

            Monitor.Log("Flower Dance Fix started using Harmony.", LogLevel.Trace);
        }
        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>();
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            // Missing Generic Mod Config Menu
            if (api is null)
            {
                return;
            }

            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
            api.RegisterLabel(ModManifest, "Flower Dance Fix", "By elfuun");
            api.RegisterParagraph(ModManifest, "Removes some of the hard-coded details of the flower dance festival main event, allowing for altered vanilla NPCs and custom NPCs to participate. Contains configurable options for dance pair generation. Only ''datable'' NPCs are considered valid for selection.");
            api.RegisterSimpleOption(ModManifest, "Maximum Dance Pairs (WIP)", "(WIP) Changes number of pairs dancing ***WILL NOT WORK FOR MORE THAN 6 PAIRS***", () => Config.MaxDancePairs, (int val) => Config.MaxDancePairs = val);


            api.RegisterSimpleOption(ModManifest, "NPCs Have Random Partners", "Pairs of NPCs dancing are arranged at random", () => Config.NPCsHaveRandomPartners, (bool val) => Config.NPCsHaveRandomPartners = val);
            api.RegisterSimpleOption(ModManifest, "Allow Mixed-Gendered Dance Lines (WIP)", "(WIP) Pairs of NPCs dancing are random pairs of random genders- will require additional sprites", () => Config.ForceHeteroPartners, (bool val) => Config.ForceHeteroPartners = val);
            api.RegisterSimpleOption(ModManifest, "Allow Non-Binary Dancers (WIP)", "(WIP) Allows NPCs that are not male or female (ie secret or unknown; gender is 2)- will require additional sprites.", () => Config.AllowNonBinaryPartners, (bool val) => Config.AllowNonBinaryPartners = val);
            api.RegisterSimpleOption(ModManifest, "Allow Tourist Dancers", "Allows NPCs living outside the valley (ie they do not live in town; homeRegion is not 2).", () => Config.AllowTouristPartners, (bool val) => Config.AllowTouristPartners = val);
            api.RegisterSimpleOption(ModManifest, "NPC Blacklist", "Configureable blacklist of NPCs preventing them from dancing- enclose NPC base name in quotes, deliniated by a forward slash, with no spaces.", () => Config.DancerBlackList, (string val) => Config.DancerBlackList = val);
            api.RegisterSimpleOption(ModManifest, "Allow Custom Crowd Animations", "Spectators in the crowd perform special animations during the dance. May shuffle NPCs around.", () => Config.AllowCrowdAnimation, (bool val) => Config.AllowCrowdAnimation = val);
        }
    }
}

