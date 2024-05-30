/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using System;
using System.Linq;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValleyTodo.Config;
using StardewValleyTodo.Controllers;
using StardewValleyTodo.Game;
using StardewValleyTodo.Tracker;

namespace StardewValleyTodo {
    public class ModEntry : Mod {
        private ModConfig _config;

        private Inventory _inventory;
        private InventoryTracker _inventoryTracker;

        private JunimoBundles _junimoBundles;

        private CraftingMenuController _craftingMenuController;
        private CarpenterMenuController _carpenterMenuController;
        private JunimoBundleController _junimoBundleController;
        private BetterCraftingMenuController _betterCraftingMenuController;
        private QuestLogController _questLogController;

        public override void Entry(IModHelper helper) {
            _config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.Display.RenderedHud += Display_RenderedHud;

            helper.Events.Player.InventoryChanged += Player_InventoryChanged;
            helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
        }

        private void Shutdown() {
            var helper = Helper;

            helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
            helper.Events.Display.RenderedHud -= Display_RenderedHud;

            helper.Events.Player.InventoryChanged -= Player_InventoryChanged;
            helper.Events.GameLoop.OneSecondUpdateTicked -= GameLoop_OneSecondUpdateTicked;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e) {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) {
                // Generic Mod Config Menu is not installed
                return;
            }

            configMenu.Register(this.ModManifest, () => _config = new ModConfig(), () => Helper.WriteConfig(_config));

            configMenu.AddNumberOption(
                ModManifest,
                () => _config.VerticalOffset,
                value => _config.VerticalOffset = value,
                () => "Vertical Offset"
            );

            configMenu.AddKeybindList(ModManifest, () => _config.TrackItem, value => _config.TrackItem = value,
                () => "Track Item (in menu)");
            configMenu.AddKeybindList(ModManifest, () => _config.ToggleVisibility,
                value => _config.ToggleVisibility = value, () => "Toggle Tracker UI Visibility (in game)");
            configMenu.AddKeybindList(ModManifest, () => _config.ClearTracker, value => _config.ClearTracker = value,
                () => "Clear Tracking List (in game)");
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e) {
            try {
                _inventory = new Inventory(Game1.player.Items);
                _inventoryTracker = new InventoryTracker();
                _junimoBundles = new JunimoBundles();

                _craftingMenuController = new CraftingMenuController(_inventoryTracker);
                _carpenterMenuController = new CarpenterMenuController(_inventoryTracker);
                _junimoBundleController = new JunimoBundleController(_inventoryTracker, _junimoBundles);
                _betterCraftingMenuController = new BetterCraftingMenuController(_inventoryTracker);
                _questLogController = new QuestLogController(_inventoryTracker);

                Game1.player.questLog.OnElementChanged += QuestLogOnOnElementChanged;
                Game1.player.team.specialOrders.OnElementChanged += SpecialOrdersOnOnElementChanged;
            } catch (Exception exception) {
                Shutdown();

                throw new Exception("Failed to initialize Recipe Tracker mod", exception);
            }
        }

        private void SpecialOrdersOnOnElementChanged(
            NetList<SpecialOrder, NetRef<SpecialOrder>> list, int index,
            SpecialOrder oldValue,
            SpecialOrder newValue
        ) {
            if (newValue != null) {
                return;
            }

            var trackedQuest = _inventoryTracker.Items
                .OfType<TrackableQuest>()
                .FirstOrDefault(x => x.Quest == oldValue);

            if (trackedQuest != null) {
                _inventoryTracker.Items.Remove(trackedQuest);
            }
        }

        private void QuestLogOnOnElementChanged(
            NetList<Quest, NetRef<Quest>> list, int index,
            Quest oldValue,
            Quest newValue
        ) {
            if (newValue != null) {
                return;
            }

            var trackedQuest = _inventoryTracker.Items
                .OfType<TrackableQuest>()
                .FirstOrDefault(x => x.Quest == oldValue);

            if (trackedQuest != null) {
                _inventoryTracker.Items.Remove(trackedQuest);
            }
        }

        private void Player_InventoryChanged(object sender, InventoryChangedEventArgs e) {
            if (!e.IsLocalPlayer) {
                return;
            }

            _inventory.Update();
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e) {
            if (!Context.IsWorldReady) {
                return;
            }

            _junimoBundles.Update();
            _inventoryTracker.Update();
        }

        private void Display_RenderedHud(object sender, RenderedHudEventArgs e) {
            if (Context.IsWorldReady && Game1.displayHUD) {
                DrawTracker();
            }
        }

        private void DrawTracker() {
            var sb = Game1.spriteBatch;
            if (_inventoryTracker.Items.Count == 0 || !_inventoryTracker.IsVisible) {
                return;
            }

            var offset = new Vector2(0, _config.VerticalOffset);
            var padding = 8;
            var contentOffset = new Vector2(offset.X + padding, offset.Y + padding);

            var size = _inventoryTracker.Draw(sb, contentOffset, _inventory);
            sb.Draw(
                Game1.menuTexture,
                new Rectangle((int) offset.X, (int) offset.Y, (int) size.X + padding * 2, (int) size.Y + padding * 2),
                new Rectangle(8, 256, 3, 4),
                Color.White);
            _inventoryTracker.Draw(sb, contentOffset, _inventory);
        }

        private IClickableMenu GetCurrentPage(IClickableMenu page) {
            if (page is GameMenu gameMenu) {
                return gameMenu.GetCurrentPage();
            }

            return page;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e) {
            if (!Context.IsWorldReady) {
                return;
            }

            var currentMenu = GetCurrentPage(Game1.activeClickableMenu);

            if (currentMenu != null && _config.TrackItem.JustPressed()) {
                var pageName = currentMenu.GetType().FullName;

                if (currentMenu is CraftingPage craftingPage) {
                    _craftingMenuController.ProcessInput(craftingPage);
                } else if (pageName == "Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage") {
                    _betterCraftingMenuController.ProcessInput(currentMenu);
                } else if (currentMenu is JunimoNoteMenu junimoNoteMenu) {
                    _junimoBundleController.ProcessInput(junimoNoteMenu);
                } else if (currentMenu is CarpenterMenu carpenterMenu) {
                    _carpenterMenuController.ProcessInput(carpenterMenu);
                } else if (currentMenu is QuestLog questLog) {
                    _questLogController.ProcessInput(questLog);
                }

                return;
            }

            if (_config.ClearTracker.JustPressed()) {
                ResetInventoryTracker();
                return;
            }

            if (_config.ToggleVisibility.JustPressed()) {
                _inventoryTracker.IsVisible = !_inventoryTracker.IsVisible;
            }
        }

        private void ResetInventoryTracker() {
            _inventoryTracker.Reset();
        }
    }
}
