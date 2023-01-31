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

            ConvenientInventory.QuickStackButtonIcon = helper.ModContent.Load<Texture2D>(@"assets\icon.png");
            ConvenientInventory.FavoriteItemsCursorTexture = helper.ModContent.Load<Texture2D>(@"assets\favoriteCursor.png");
            ConvenientInventory.FavoriteItemsHighlightTexture = helper.ModContent.Load<Texture2D>($@"assets\favoriteHighlight_{Config.FavoriteItemsHighlightTextureChoice}.png");
            ConvenientInventory.FavoriteItemsBorderTexture = helper.ModContent.Load<Texture2D>(@"assets\favoriteBorder.png");

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Input.ButtonReleased += OnButtonReleased;

            helper.ConsoleCommands.Add("player_fixinventory",
                "Resizes the player's inventory to its correct maximum size, dropping any extra items contained in inventory. (Some mods directly modify the player's inventory size, " +
                "causing compatibility issues and/or leaving extra null items when uninstalled; this command should fix these issues.)" +
                "\n\nUsage: player_fixinventory",
                FixInventory);
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

        /// <summary>
        /// Resizes player's inventory to player.MaxItems, dropping any extra items contained in inventory.
        /// </summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void FixInventory(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Please load a save before using this command.", LogLevel.Info);
                return;
            }

            var who = StardewValley.Game1.player;
            var items = who.Items;

            if (items.Count == who.MaxItems)
            {
                Monitor.Log($"Inventory size is already correct, no fix needed. (Inventory size: {items.Count}, Max items: {who.MaxItems})", LogLevel.Info);
                return;
            }

            Monitor.Log($"Resizing inventory from {items.Count} => {who.MaxItems}...", LogLevel.Info);

            while (items.Count > who.MaxItems)
            {
                int index = items.Count - 1;

                if (items[index] != null)
                {
                    StardewValley.Game1.playSound("throwDownITem");

                    StardewValley.Game1.createItemDebris(items[index], who.getStandingPosition(), who.FacingDirection)
                        .DroppedByPlayerID.Value = who.UniqueMultiplayerID;

                    Monitor.Log($"Found non-null item: '{items[index].Name}' (x {items[index].Stack}) at index: {index} when resizing inventory."
                        + " The item was manually dropped; this may have resulted in unexpected behavior.",
                        LogLevel.Warn);
                }

                // Remove the last item of the list
                items.RemoveAt(index);
            }
        }
    }
}
