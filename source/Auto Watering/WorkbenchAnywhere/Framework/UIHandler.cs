using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace WorkbenchAnywhere.Framework
{
    internal class UIHandler
    {
        private readonly ClickableTextureComponent _onIcon;
        private readonly ClickableTextureComponent _offIcon;
        private ModConfig _config;
        private readonly MaterialStorage _materialStorage;
        private readonly IModHelper _helper;
        private readonly IMonitor _monitor;
        private double _xOffset = 0.0;
        private double _yOffset = 1.0;

        public UIHandler(Texture2D onIcon, Texture2D offIcon, ModConfig config, MaterialStorage materialStorage, IModHelper helper, IMonitor monitor)
        {
            _onIcon = new ClickableTextureComponent(Rectangle.Empty, onIcon, Rectangle.Empty, 1f);
            _offIcon = new ClickableTextureComponent(Rectangle.Empty, offIcon, Rectangle.Empty, 1f); ;
            _materialStorage = materialStorage;
            _helper = helper;
            _monitor = monitor;
            LoadConfig(config);
            DetermineIconLocation();
        }

        public void LoadConfig(ModConfig config)
        {
            _config = config;
        }

        private void DetermineIconLocation()
        {
            var categorizeChestsLoaded = _helper.ModRegistry.IsLoaded("CategorizeChests");
            var convenientChestsLoaded = _helper.ModRegistry.IsLoaded("aEnigma.ConvenientChests");
            var megaStorageLoaded = _helper.ModRegistry.IsLoaded("Alek.MegaStorage");
            var remoteFridgeLoaded = _helper.ModRegistry.IsLoaded("EternalSoap.RemoteFridgeStorage");

            if (categorizeChestsLoaded || convenientChestsLoaded || megaStorageLoaded)
            {
                _xOffset = -1.0;
                _yOffset = -0.25;
            }

            if (remoteFridgeLoaded)
            {
                _xOffset -= 1.0;
            }
        }

        public void RegisterUIEvents(IModHelper helper)
        {
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady || e.NewMenu == null) return;

            if (_config.ReplaceCraftMenu && e.NewMenu is GameMenu gameMenu)
                _materialStorage.ReplaceGameMenu(gameMenu);
            else if (_config.ReplaceWorkbench && e.NewMenu is CraftingPage craftingPage)
            {
                var cooking = _helper.Reflection.GetField<bool>(craftingPage, "cooking", false)?.GetValue() ?? false;
                if (!cooking)
                    _materialStorage.ReplaceCraftingPage(craftingPage);
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || e.Button != SButton.MouseLeft) return;

            var chest = GetOpenChest();
            if (chest == null) return;

            var screenPixels = e.Cursor.ScreenPixels;
            if (!_onIcon.containsPoint((int)screenPixels.X, (int)screenPixels.Y)) return;

            Game1.playSound("smallSelect");
            _materialStorage.ToggleMaterialStorageChest(chest);
        }

        private void SetIconLocations(Chest chest)
        {
            var menu = Game1.activeClickableMenu;
            if (menu == null) return;

            var xOffsetBasedOnChest = _xOffset;
            if (chest.fridge.Value)
                xOffsetBasedOnChest += 1.0;

            var xScaledOffset = (int)(xOffsetBasedOnChest * Game1.tileSize);
            var yScaledOffset = (int)(_yOffset * Game1.tileSize);

            var screenX = menu.xPositionOnScreen - 17 * Game1.pixelZoom + xScaledOffset;
            var screenY = menu.yPositionOnScreen + yScaledOffset + Game1.pixelZoom * 5;

            _onIcon.bounds = _offIcon.bounds = new Rectangle(screenX, screenY,
                16 * Game1.pixelZoom, 16 * Game1.pixelZoom);
        }

        private Chest GetOpenChest()
        {
            if (Game1.activeClickableMenu == null || // no menu open
                !(Game1.activeClickableMenu is ItemGrabMenu menu) || // not an item grab menu
                !(menu.behaviorOnItemGrab?.Target is Chest chest))  // not chest
                return null;

            return chest;
        }

        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            var chest = GetOpenChest();
            if (chest == null) return;

            SetIconLocations(chest);

            var targetIcon = _materialStorage.IsMaterialStorageChest(chest) ?
                _onIcon : _offIcon;

            targetIcon.draw(Game1.spriteBatch);
        }
    }
}
