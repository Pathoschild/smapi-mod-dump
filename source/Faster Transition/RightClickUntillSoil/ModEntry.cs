/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DotSharkTeeth/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley;
using HarmonyLib;

namespace RightClickUntillSoil
{
    /// <summary>The mod entry point.</summary>
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

            harmony.Patch(
               original: AccessTools.Method(typeof(Game1), nameof(Game1.pressActionButton)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Game1pressActionButton_postfix))
            );

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private static bool IsHoldingHoe()
        {
            return !Game1.player.UsingTool && Game1.player.CurrentTool is Hoe;
        }
        private static bool IsHoeDirt(Vector2 tile)
        {
            return Game1.currentLocation.terrainFeatures.ContainsKey(tile)
                && Game1.currentLocation.terrainFeatures[tile] is HoeDirt dirt
                && dirt.crop == null
                && !Game1.currentLocation.objects.ContainsKey(tile);
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
                name: () => "Use Tool Location",
                tooltip: () => "Use tool location instead of mouse location",
                getValue: () => Config.UseToolLocation,
                setValue: value => Config.UseToolLocation = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Tool Radius",
                tooltip: () => "Tool Radius",
                getValue: () => Config.ToolRadius,
                setValue: value => Config.ToolRadius = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Tool Speed",
                tooltip: () => "Tool Speed",
                getValue: () => Config.ToolSpeed,
                setValue: value => Config.ToolSpeed = value
            );
        }
    }
}
