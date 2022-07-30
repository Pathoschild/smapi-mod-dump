/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Globalization;
using Object = StardewValley.Object;

namespace SubmergedCrabPots
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {

        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;
        public static ModEntry context;

        private static Texture2D bobberTexture;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            context = this;

            SMonitor = Monitor;
            SHelper = helper;

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            try
            {
                bobberTexture = Game1.content.Load<Texture2D>("aedenthorn.SubmergedCrabPots/bobber");
            }
            catch
            {
                bobberTexture = Helper.Content.Load<Texture2D>("assets/bobber.png");
            }
        }
        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Mod Enabled",
                getValue: () => Config.EnableMod,
                setValue: value => Config.EnableMod = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Submerge When Harvestable",
                getValue: () => Config.SubmergeHarvestable,
                setValue: value => Config.SubmergeHarvestable = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Show Ripples",
                tooltip: () => "Only shown when no harvest",
                getValue: () => Config.ShowRipples,
                setValue: value => Config.ShowRipples = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Bobber Scale",
                getValue: () => Config.BobberScale+"",
                setValue: delegate (string value) { try { Config.BobberScale = float.Parse(value, CultureInfo.InvariantCulture); } catch { } }
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Red Tint",
                getValue: () => Config.BobberTint.R,
                setValue: value => Config.BobberTint = new Color(value, Config.BobberTint.G, Config.BobberTint.B, Config.BobberTint.A),
                min: 0,
                max: 255
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Green Tint",
                getValue: () => Config.BobberTint.G,
                setValue: value => Config.BobberTint = new Color(Config.BobberTint.R, value, Config.BobberTint.B, Config.BobberTint.A),
                min: 0,
                max: 255
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Blue Tint",
                getValue: () => Config.BobberTint.B,
                setValue: value => Config.BobberTint = new Color(Config.BobberTint.R, Config.BobberTint.G, value, Config.BobberTint.A),
                min: 0,
                max: 255
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Opacity",
                getValue: () => Config.BobberOpacity,
                setValue: value => Config.BobberOpacity = value,
                min: 0,
                max: 100
            );
        }
    }
}