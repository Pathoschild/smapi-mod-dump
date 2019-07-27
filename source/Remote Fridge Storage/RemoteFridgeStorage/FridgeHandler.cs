using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RemoteFridgeStorage.API;
using RemoteFridgeStorage.CraftingPage;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace RemoteFridgeStorage
{
    public class FridgeHandler
    {
        private readonly Config _config;
        private readonly bool _offsetIcon;
        private readonly ClickableTextureComponent _fridgeSelected;
        private readonly ClickableTextureComponent _fridgeDeselected;


        /// <summary>
        /// The chests to be used by the fridge.
        /// </summary>
        public HashSet<Chest> Chests { get; private set; }

        /// <summary>
        /// List with all the items contained by <see cref="SaveData"/>, this list will be updated 
        /// when a chest is added or removed or the inventory is changed.
        /// </summary>
        public IList<Item> FridgeList { get; private set; }

        public ICookingSkillApi CookingSkillApi { get; set; }

        /// <summary>
        /// If the chest is currently open.
        /// </summary>
        private bool _opened;

        private bool _dragging;

        public bool MenuEnabled { get; set; }

        public FridgeHandler(Texture2D fridgeSelectedIcon, Texture2D fridgeDeselectedIcon, bool offsetIcon,
            Config config)
        {
            _config = config;
            _offsetIcon = offsetIcon;
            _fridgeSelected = new ClickableTextureComponent(Rectangle.Empty, fridgeSelectedIcon, Rectangle.Empty, 1f);
            _fridgeDeselected =
                new ClickableTextureComponent(Rectangle.Empty, fridgeDeselectedIcon, Rectangle.Empty, 1f);

            MenuEnabled = true;
            Chests = new HashSet<Chest>();
            FridgeList = new FridgeVirtualList(this);
        }

        /// <summary>
        /// Update the position of the button.
        /// </summary>
        private void UpdatePos()
        {
            var menu = Game1.activeClickableMenu;
            if (menu == null) return;

            var xOffset = 0.0;
            var yOffset = 1.0;
            if (_offsetIcon)
            {
                xOffset = -1.0;
                yOffset = -0.25;
            }
            
            var xScaledOffset = (int) (xOffset * Game1.tileSize);
            var yScaledOffset = (int) (yOffset * Game1.tileSize);

            var screenX = menu.xPositionOnScreen - 17 * Game1.pixelZoom + xScaledOffset;
            var screenY = menu.yPositionOnScreen + yScaledOffset + Game1.pixelZoom * 5;
            
            if (_config.OverrideOffset)
            {
                screenX = _config.XOffset;
                screenY = _config.YOffset;
            }
            _fridgeSelected.bounds = _fridgeDeselected.bounds = new Rectangle(screenX, screenY,
                (int) (_config.ImageScale * 16 * Game1.pixelZoom), (int) (_config.ImageScale * 16 * Game1.pixelZoom));
        }

        /// <summary>
        /// Handle the click event if it was on the fridge icon.
        /// </summary>
        /// <param name="cursor">The current cursor position.</param>
        public void HandleClick(ICursorPosition cursor)
        {
            var chest = GetOpenChest();
            if (chest == null) return;

            var screenPixels = cursor.ScreenPixels;

            if (!_fridgeSelected.containsPoint((int) screenPixels.X, (int) screenPixels.Y)) return;

            Game1.playSound("smallSelect");

            if (Chests.Contains(chest))
            {
                Chests.Remove(chest);
            }
            else
            {
                Chests.Add(chest);
            }
        }

        /// <summary>
        /// Gets the chest that is currently open or null if no chest is open.
        /// </summary>
        /// <returns>The chest that is open</returns>
        private static Chest GetOpenChest()
        {
            if (Game1.activeClickableMenu == null)
                return null;

            if (!(Game1.activeClickableMenu is ItemGrabMenu)) return null;

            var menu = (ItemGrabMenu) Game1.activeClickableMenu;
            if (menu.behaviorOnItemGrab?.Target is Chest chest)
                return chest;

            return null;
        }

        /// <summary>
        /// Draw the fridge button
        /// </summary>
        public void DrawFridge()
        {
            var openChest = GetOpenChest();
            if (openChest == null) return;

            var farmHouse = Game1.getLocationFromName("farmHouse") as FarmHouse;

            if (openChest == farmHouse?.fridge.Value || Game1.activeClickableMenu == null ||
                !openChest.playerChest.Value) return;

            UpdatePos();
            if (Chests.Contains(openChest))
                _fridgeSelected.draw(Game1.spriteBatch);
            else
                _fridgeDeselected.draw(Game1.spriteBatch);

            Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero,
                4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0);
        }


        /// <summary>
        /// Load all fridges.
        /// </summary>
        public void AfterLoad()
        {
            //
            Chests.Clear();
            var farmHouse = Game1.getLocationFromName("farmHouse") as FarmHouse;
            foreach (var gameLocation in GetLocations())
            {
                foreach (var gameLocationObject in gameLocation.objects.Values)
                {
                    LoadChest(gameLocationObject, farmHouse);
                }
            }
        }

        public static IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }

        private void LoadChest(Object gameLocationObject, FarmHouse farmHouse)
        {
            if (!(gameLocationObject is Chest chest)) return;

            if (!chest.fridge.Value || chest == farmHouse?.fridge.Value) return;

            Chests.Add(chest);
            chest.fridge.Value = false;
        }

        /// <summary>
        /// Hacky way to store which chests are selected
        /// </summary>
        public void BeforeSave()
        {
            foreach (var chest in Chests)
            {
                chest.fridge.Value = true;
            }
        }

        /// <summary>
        /// Reset the fridge booleans
        /// </summary>
        public void AfterSave()
        {
            //Reset the fridge flag for all chests.
            foreach (var chest in Chests)
            {
                chest.fridge.Value = false;
            }
        }

        /// <summary>
        /// Listen to ticks update to determine when a chest opens or closes.
        /// </summary>
        public void Game_Update()
        {
            UpdateChest();
            UpdateOffset();
        }

        /// <summary>
        /// Update the position of the icon when dragging it while holding the right mouse button 
        /// </summary>
        private void UpdateOffset()
        {
            if (GetOpenChest() == null) return;
            var mouseButton = new InputButton(false).ToSButton();
            if (ModEntry.Instance.Helper.Input.IsDown(mouseButton))
            {
                var screenPixels = ModEntry.Instance.Helper.Input.GetCursorPosition().ScreenPixels;
                if (_dragging)
                {
                    _config.XOffset = (int) screenPixels.X;
                    _config.YOffset = (int) screenPixels.Y;
                    _config.OverrideOffset = true;
                    ModEntry.Instance.Helper.Input.Suppress(mouseButton);
                }
                else if (_fridgeSelected.containsPoint((int) screenPixels.X, (int) screenPixels.Y))
                {
                    _dragging = true;
                }
            }
            else
            {
                _dragging = false;
                ModEntry.Instance.Helper.WriteConfig(_config);
            }
        }

        private void UpdateChest()
        {
            var chest = GetOpenChest();
            if (chest == null && _opened)
            {
                //Chest close event;
                UpdateFridgeContents();
            }

            if (chest != null && !_opened)
            {
                //Chest open event
                UpdateFridgeContents();
                UpdatePos();
            }

            _opened = chest != null;
        }

        public void UpdateFridgeContents()
        {
            FridgeList = new FridgeVirtualList(this);
        }

        /// <summary>
        /// Replace the menu.
        /// </summary>
        /// <param name="newMenu">The new menu to replace.</param>
        public void LoadMenu(IClickableMenu newMenu)
        {
            FridgeList = new FridgeVirtualList(this);
            if (!MenuEnabled) return;
            if (CookingSkillApi != null) return;
            Game1.activeClickableMenu = new RemoteFridgeCraftingPage(newMenu, this);
        }
    }
}