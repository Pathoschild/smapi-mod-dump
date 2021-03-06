/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kenny2892/StardewValleyMods
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

namespace FlowerDancing
{
    public class ModEntry : Mod
    {
        private static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            // Initialize Config
            Config = this.Helper.ReadConfig<ModConfig>();
            Helper.Events.GameLoop.GameLaunched += onLaunched;

            // Initialize Patches
            EventPatched.Initialize(Monitor, Config);
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch
            (
               original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.setUpFestivalMainEvent)),
               postfix: new HarmonyMethod(typeof(EventPatched), nameof(EventPatched.setUpFestivalMainEvent_Kelly))
            );

            harmony.Patch
            (
               original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.setUpPlayerControlSequence)),
               postfix: new HarmonyMethod(typeof(EventPatched), nameof(EventPatched.setUpPlayerControlSequence_Kelly))
            );

            Monitor.Log("Kelly's Flower Dancing started using Harmony.", LogLevel.Debug);
        }

        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if(api is null) // Doesn't have Generic Mod Config Menu installed
            {
                return;
            }

            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
            api.RegisterLabel(ModManifest, "Kelly's Flower Dancing", "Made by kelly2892");
            api.RegisterSimpleOption(ModManifest, "Auto Remove Attire", "Will automatically take off your clothes when you enter the Flower Dance. Once the dance is done, the clothes will be re-added.", () => Config.AutoRemoveClothes, (bool val) => Config.AutoRemoveClothes = val);
        }
    }
}
