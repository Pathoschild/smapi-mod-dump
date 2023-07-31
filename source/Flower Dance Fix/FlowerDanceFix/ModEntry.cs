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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using FlowerDanceFix.GenericModConfigMenu;

namespace FlowerDanceFix
{
    public class ModEntry : Mod
    {
        private static ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.GameLaunched += onLaunched;
            Helper.Events.GameLoop.DayStarted += onDayStarted;

            Config = Helper.ReadConfig<ModConfig>();

            // Initialize Patches
            EventPatched.Initialize(Monitor, Config, Helper);
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch
            (
               original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.setUpFestivalMainEvent)),
               postfix: new HarmonyMethod(typeof(EventPatched), nameof(EventPatched.setUpFestivalMainEvent_FDF))
            );
            Monitor.Log("Flower Dance Fix has used Harmony to patch a postfix onto the method Event.setUpFestivalMainEvent.", LogLevel.Trace);

           // harmony.Patch
           // (
           //     original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.command_changeSprite)),
           //     prefix: new HarmonyMethod(typeof(EventPatched), nameof(EventPatched.command_changeSprite_FDF))
           // );

           // Monitor.Log("Flower Dance Fix has used Harmony to patch a prefix onto the method Event.command_changeSprite.", LogLevel.Trace);

        }
        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            Dictionary<string, string> SpriteDictionary = new Dictionary<string, string>();

            Config = this.Helper.ReadConfig<ModConfig>();

            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
                );           
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Flower Dance Fix By elfuun"
                );
            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "Removes some of the hard-coded details of the flower dance festival main event, allowing for altered vanilla NPCs and custom NPCs to participate. Contains configurable options for dance pair generation. Only ''datable'', non-child NPCs are considered valid for selection."
                );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Maximum Dance Pairs (WIP)",
                tooltip: () => "(WIP) Changes number of pairs dancing ***WILL NOT WORK FOR MORE THAN 6 PAIRS***",
                getValue: () => Config.MaxDancePairs,
                setValue: value => Config.MaxDancePairs = value,
                min: 1,
                max: 6
                );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "NPCs Have Random Partners",
                tooltip: () => "Pairs of NPCs dancing are arranged at random.",
                getValue: () => Config.NPCsHaveRandomPartners,
                setValue: value => Config.NPCsHaveRandomPartners = value
                );
            //configMenu.AddBoolOption(
                //mod: this.ModManifest,
                //name: () => "Allow Mixed-Gendered Dance Lines (WIP)",
                //tooltip: () => "(WIP) Pairs of NPCs dancing are random pairs of random genders- will require additional sprites.",
                //getValue: () => Config.ForceHeteroPartners,
                //setValue: value => Config.ForceHeteroPartners = value
                //);
            //configMenu.AddBoolOption(
                //mod: this.ModManifest,
                //name: () => "Allow Non-Binary Dancers (WIP)",
                //tooltip: () => "(WIP) Allows NPCs that are not male or female (ie secret or unknown; gender is 2)- will require additional sprites.",
                //getValue: () => Config.ForceHeteroPartners,
                //setValue: value => Config.ForceHeteroPartners = value
                //);
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Allow Tourist Dancers",
                tooltip: () => "Allows NPCs living outside the valley (ie they do not live in town; homeRegion is not 2).",
                getValue: () => Config.AllowTouristPartners,
                setValue: value => Config.AllowTouristPartners = value
                );
            //configMenu.AddPageLink(
                //mod: this.ModManifest,
                //pageId: "fdfBlacklist",
                //text: () => "Dancer Denylist",
                //tooltip: () => "Add and remove NPCs from a denylist of dancers"
                //);
            //configMenu.AddPage(
                //mod: this.ModManifest,
                //pageId: "fdfBlacklist",
                //pageTitle: () => "Dancer Denylist");
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "NPC Blacklist",
                tooltip: () => "Configureable blacklist of NPCs preventing them from dancing- enclose NPC base name in quotes, deliniated by a forward slash, with no spaces.",
                getValue: () => Config.DancerBlackList,
                setValue: value => Config.DancerBlackList = value);
        }

        private void onDayStarted(object sender, DayStartedEventArgs e)
        {
            Helper.ReadConfig<ModConfig>();
        }
    }

}

