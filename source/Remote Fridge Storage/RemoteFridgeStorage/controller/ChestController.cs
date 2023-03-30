/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SoapStuff/Remote-Fridge-Storage
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace RemoteFridgeStorage.controller
{
    /// <summary>
    /// Used for drawing and selecting chests
    /// </summary>
    public class ChestController
    {
        private readonly ClickableTextureComponent _fridgeSelected;
        private readonly ClickableTextureComponent _fridgeDeselected;
        private readonly bool _offsetIcon;

        private readonly HashSet<Chest> _chests;
        private readonly bool _chestAnywhereLoaded;

        private Chest _openChest;

        /// <summary>
        /// New ChestController
        /// </summary>
        /// <param name="textures">This is a struct that contains all the textures used by the mod</param>
        /// <param name="compatibilityInfo">The struct that contains information about loaded mods</param>
        public ChestController(ModEntry.Textures textures, ModEntry.CompatibilityInfo compatibilityInfo)
        {
            _openChest = null;
            _chests = new HashSet<Chest>();
            _offsetIcon =
                compatibilityInfo.CategorizeChestLoaded || compatibilityInfo.ConvenientChestLoaded ||
                compatibilityInfo.MegaStorageLoaded;
            _chestAnywhereLoaded = compatibilityInfo.ChestAnywhereLoaded;
            var sourceRect = new Rectangle(0, 0, textures.FridgeSelected.Width, textures.FridgeSelected.Height);
            _fridgeSelected =
                new ClickableTextureComponent(sourceRect, textures.FridgeSelected, sourceRect, 1f);
            _fridgeDeselected =
                new ClickableTextureComponent(sourceRect, textures.FridgeDeselected, sourceRect, 1f);
        }

        /// <summary>
        /// Gets the chest that is currently open or null if no chest is open.
        /// </summary>
        /// <returns>The chest that is open</returns>
        private Chest GetOpenChest()
        {
            var clickableMenu = Game1.activeClickableMenu;


            var itemGrabMenu = clickableMenu as ItemGrabMenu;
            if (itemGrabMenu?.behaviorOnItemGrab?.Target == null) return null;
            if (itemGrabMenu.behaviorOnItemGrab.Target is Chest chest) return chest;
            
            return _chestAnywhereLoaded && itemGrabMenu.behaviorOnItemGrab.Target.GetType().ToString()
                .Equals("Pathoschild.Stardew.ChestsAnywhere.Framework.Containers.ChestContainer")
                ? ChestsAnywhere(itemGrabMenu.behaviorOnItemGrab.Target)
                : null;
        }

        private static Chest ChestsAnywhere(object target)
        {
            var field = target.GetType().GetField("Chest", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return field.GetValue(target) as Chest;
            }

            ModEntry.Instance.Monitor.Log($"GetOpenChest failed: {target.GetType()}.Chest not found.",
                LogLevel.Warn);
            return null;
        }

        /// <summary>
        /// Update the position of the button based on the settings in the config.
        /// </summary>
        private void UpdatePos()
        {
            //Number values here are based on trial and error
            var menu = Game1.activeClickableMenu;
            if (menu == null) return;

            var xOffset = 0.0;
            var yOffset = 1.0;
            //A mod is loaded that places an icon on the same location so we change it.
            if (_offsetIcon)
            {
                xOffset = -1.0;
                yOffset = -0.25;
            }

            var xScaledOffset = (int)(xOffset * Game1.tileSize);
            var yScaledOffset = (int)(yOffset * Game1.tileSize);

            var screenX = menu.xPositionOnScreen - 17 * Game1.pixelZoom + xScaledOffset;
            var screenY = menu.yPositionOnScreen + yScaledOffset + Game1.pixelZoom * 5;

            var rectangle = new Rectangle(screenX, screenY,
                (int)(16 * Game1.pixelZoom),
                (int)(16 * Game1.pixelZoom));

            _fridgeSelected.bounds = _fridgeDeselected.bounds = rectangle;
        }

        /// <summary>
        /// Handle the click event if it was on the fridge icon.
        /// </summary>
        /// <param name="cursor">The current cursor position.</param>
        public void HandleClick(ICursorPosition cursor)
        {
            var chest = GetOpenChest();
            if (chest == null) return;

            var screenPixels = Utility.ModifyCoordinatesForUIScale(cursor.ScreenPixels);

            if (!_fridgeSelected.containsPoint((int)screenPixels.X, (int)screenPixels.Y)) return;

            Game1.playSound("smallSelect");

            if (_chests.Contains(chest))
            {
                _chests.Remove(chest);
            }
            else
            {
                _chests.Add(chest);
            }
        }

        /// <summary>
        /// Draw the icon.
        /// </summary>
        /// <param name="e"></param>
        public void DrawFridgeIcon(RenderedActiveMenuEventArgs e)
        {
            var openChest = this._openChest;
            if (openChest == null) return;

            var farmHouse = Game1.getLocationFromName("farmHouse") as FarmHouse;

            if (openChest == farmHouse?.fridge.Value || Game1.activeClickableMenu == null ||
                !openChest.playerChest.Value) return;

            UpdatePos();
            if (_chests.Contains(openChest))
            {
                _fridgeSelected.draw(e.SpriteBatch, Color.White, 0);
            }
            else
            {
                _fridgeDeselected.draw(e.SpriteBatch, Color.White, 0);
            }

            Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero,
                4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Returns the chests currently selected
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Chest> GetChests()
        {
            return _chests;
        }

        /// <summary>
        /// Update the chests
        /// </summary>
        /// <param name="chests"></param>
        public void SetChests(IEnumerable<Chest> chests)
        {
            _chests.Clear();
            foreach (var chest in chests)
            {
                _chests.Add(chest);
            }
        }

        /// <summary>
        /// Clears the list of chests.
        /// </summary>
        public void ClearChests()
        {
            _chests.Clear();
        }

        public void UpdateChest()
        {
            _openChest = GetOpenChest();
        }
    }
}