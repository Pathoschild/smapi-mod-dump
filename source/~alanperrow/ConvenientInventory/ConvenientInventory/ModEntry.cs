/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using ConvenientInventory.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using ConvenientInventory.Compatibility;

namespace ConvenientInventory
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }

        public static ModConfig Config { get; set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>();

            ConvenientInventory.QuickStackButtonIcon = helper.Content.Load<Texture2D>(@"assets\icon.png");
            ConvenientInventory.FavoriteItemsCursorTexture = helper.Content.Load<Texture2D>(@"assets\favoriteCursor.png");
            ConvenientInventory.FavoriteItemsHighlightTexture = helper.Content.Load<Texture2D>($@"assets\favoriteHighlight_{Config.FavoriteItemsHighlightTextureChoice}.png");
            ConvenientInventory.FavoriteItemsBorderTexture = helper.Content.Load<Texture2D>(@"assets\favoriteBorder.png");

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Input.ButtonReleased += OnButtonReleased;
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves).
        /// All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            // Manually patch InventoryPage constructor, otherwise Harmony cannot find method.
            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.Menus.InventoryPage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(typeof(InventoryPageConstructorPatch), nameof(InventoryPageConstructorPatch.Postfix))
            );

            harmony.PatchAll();

            // Initialize mod(s)
            ModInitializer modInitializer = new(ModManifest, Helper);

            // Get Generic Mod Config Menu API (if it's installed)
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                modInitializer.Initialize(api, Config);
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Config.IsEnableFavoriteItems)
            {
                ConvenientInventory.LoadFavoriteItemSlots();
            }
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Config.IsEnableFavoriteItems)
            {
                ConvenientInventory.SaveFavoriteItemSlots();
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Handle favorite items hotkey being pressed
            if (Config.IsEnableFavoriteItems
                && Context.IsWorldReady
                && (e.Button == Config.FavoriteItemsKeyboardHotkey || e.Button == Config.FavoriteItemsControllerHotkey))
            {
                ConvenientInventory.IsFavoriteItemsHotkeyDown = true;
            }

            // Handle quick stack hotkey being pressed
            if (Config.IsEnableQuickStackHotkey
                && Context.IsWorldReady
                && StardewValley.Game1.CurrentEvent is null
                && (e.Button == Config.QuickStackKeyboardHotkey || e.Button == Config.QuickStackControllerHotkey))
            {
                ConvenientInventory.OnQuickStackHotkeyPressed();
            }
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            // Handle favorite items hotkey being released
            if (Config.IsEnableFavoriteItems && (e.Button == Config.FavoriteItemsKeyboardHotkey || e.Button == Config.FavoriteItemsControllerHotkey))
            {
                ConvenientInventory.IsFavoriteItemsHotkeyDown = false;
            }
        }
    }
}
