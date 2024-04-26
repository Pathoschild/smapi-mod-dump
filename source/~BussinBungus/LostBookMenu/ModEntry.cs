/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BussinBungus/BungusSDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LostBookMenu
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        public static IModHelper SHelper;
        public static ModConfig Config;

        public static string coverPath = "bungus.BookMenu/covers";
        public static Dictionary<string, CoverData> bookData = new Dictionary<string, CoverData>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            if (!Config.ModEnabled)
                return;

            SHelper = helper;
            BookMenu.Init(Monitor);

            helper.Events.Content.AssetRequested += Content_AssetRequested;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Content_AssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(coverPath))
            {
                e.LoadFrom(() => coverDictionary, StardewModdingAPI.Events.AssetLoadPriority.Exclusive);
            }
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            bookData = Helper.GameContent.Load<Dictionary<string, CoverData>>(coverPath);
            foreach (var key in bookData.Keys.ToArray())
            {
                bookData[key].scale = Config.CoverScale;
                bookData[key].title = SHelper.Translation.Get($"BookName.{key}");
                if (!string.IsNullOrEmpty(bookData[key].texturePath))
                {
                    bookData[key].texture = Helper.GameContent.Load<Texture2D>(bookData[key].texturePath);
                }
                else
                {
                    bookData[key].texture = Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "cover.png"));
                }

            }
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod configs
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Basic Settings"
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Mod Enabled",
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Menu Book in Library",
                getValue: () => Config.MenuInLibrary,
                setValue: value => Config.MenuInLibrary = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Menu Key",
                getValue: () => Config.MenuKey,
                setValue: value => Config.MenuKey = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Use Original Titles",
                tooltip: () => "Use the original 'always on' titles instead of the new 'tooltip' titles.",
                getValue: () => Config.LegacyTitles,
                setValue: value => Config.LegacyTitles = value
            );

            configMenu.AddPageLink(
                mod: ModManifest,
                pageId: "Advanced",
                text: () => "Go to Advanced Settings"
            );
            configMenu.AddPage(
                mod: ModManifest,
                pageId: "Advanced",
                pageTitle: () => "Advanced Settings"
            );
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Advanced Settings"
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Window Width",
                getValue: () => Config.WindowWidth,
                setValue: value => Config.WindowWidth = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Window Height",
                getValue: () => Config.WindowHeight,
                setValue: value => Config.WindowHeight = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Number of Columns",
                getValue: () => Config.GridColumns,
                setValue: value => Config.GridColumns = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "X Offset",
                tooltip: () => "Distance from the left border to start displaying books. '-1' uses the default calculation to center.",
                getValue: () => Config.xOffset,
                setValue: value => Config.xOffset = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Y Offset",
                tooltip: () => "Distance from the upper border to start displaying books.",
                getValue: () => Config.yOffset,
                setValue: value => Config.yOffset = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Cover Scale",
                getValue: () => Config.CoverScale,
                setValue: value => Config.CoverScale = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Horizontal Spacing",
                tooltip: () => "The horizontal space between two books.",
                getValue: () => Config.HorizontalSpace,
                setValue: value => Config.HorizontalSpace = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Vertical Spacing",
                tooltip: () => "The vertical space between two books.",
                getValue: () => Config.VerticalSpace,
                setValue: value => Config.VerticalSpace = value
            );
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!Config.ModEnabled)
                return;
            if (e.Button == Config.MenuKey)
            {
                OpenMenu();
            }
            if (e.Button == SButton.MouseRight && e.Cursor.GrabTile == new Vector2(7, 9) && Game1.player.currentLocation.Name == "ArchaeologyHouse")
            {
                OpenMenu();
            }
            if (e.Button == SButton.ControllerA && e.Cursor.GrabTile == new Vector2(7, 10) && Game1.player.currentLocation.Name == "ArchaeologyHouse")
            {
                Helper.Input.Suppress(SButton.ControllerA);
                OpenMenu();
            }
        }

        private void OpenMenu()
        {
            if(Config.ModEnabled && Context.IsPlayerFree)
            {
                Game1.activeClickableMenu = new BookMenu();
            }
        }
    }
}