/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DotSharkTeeth/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley;
using System.Reflection;
using StardewValley.BellsAndWhistles;
using HarmonyLib;

namespace FasterTransition
{
    public partial class ModEntry : Mod
    {
        public static ModConfig Config;
        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            SMonitor = Monitor;
            SHelper = Helper;
            var harmony = new Harmony(ModManifest.UniqueID);

            // TODO:: Add support for global fade
            harmony.Patch(
               original: AccessTools.Method(typeof(ScreenFade), nameof(ScreenFade.UpdateFadeAlpha)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.ScreenFadeUpdateFadeAlpha_prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(ScreenFade), nameof(ScreenFade.FadeScreenToBlack)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.FadeScreenToBlack_prefix))
            );

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable",
                tooltip: () => "Enable mod",
                getValue: () => Config.Enable,
                setValue: value => Config.Enable = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "No Transition",
                tooltip: () => "No transition",
                getValue: () => Config.NoTransition,
                setValue: value => Config.NoTransition = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Speed Increse",
                tooltip: () => "Speed transition",
                getValue: () => Config.Speed * 1000,
                setValue: value => Config.Speed = value/1000
            );
        }
    }
}
