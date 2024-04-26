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
        /// Handle the click event if it was on the fridge icon.
        /// </summary>
        /// <param name="cursor">The current cursor position.</param>
        public void HandleClick(ICursorPosition cursor)
        {
            UpdateChest();
            var chest = _openChest;
            if (chest == null) return;

            var screenPixels = Utility.ModifyCoordinatesForUIScale(cursor.ScreenPixels);

            if (!_fridgeSelected.containsPoint((int) screenPixels.X, (int) screenPixels.Y)) return;

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
        /// Update the position of the button based on the settings in the config.
        /// </summary>
        /// <param name="e"></param>
        private void UpdateButtonPosition(RenderedActiveMenuEventArgs e)
        {
            if (!(Game1.activeClickableMenu is ItemGrabMenu menu)) return;

            var offset = _openChest.SpecialChestType == Chest.SpecialChestTypes.BigChest ? 3 : 2;
            var screenX = menu.xPositionOnScreen - Game1.pixelZoom * 16 * offset + Game1.pixelZoom;
            var screenY = menu.yPositionOnScreen + Game1.pixelZoom;

            var rectangle = new Rectangle(screenX, screenY,
                16 * Game1.pixelZoom,
                16 * Game1.pixelZoom);

            _fridgeSelected.bounds = _fridgeDeselected.bounds = rectangle;
        }

        /// <summary>
        /// Draw the icon.
        /// </summary>
        /// <param name="e"></param>
        public void DrawFridgeIcon(RenderedActiveMenuEventArgs e)
        {
            var openChest = _openChest;
            if (openChest == null) return;

            var farmHouse = Game1.getLocationFromName("farmHouse") as FarmHouse;

            if (openChest == farmHouse?.fridge.Value || Game1.activeClickableMenu == null ||
                !openChest.playerChest.Value) return;

            UpdateButtonPosition(e);
            if (_chests.Contains(openChest))
            {
                _fridgeSelected.draw(e.SpriteBatch, Color.White, 10f);
            }
            else
            {
                _fridgeDeselected.draw(e.SpriteBatch, Color.White, 10f);
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

        public void UpdateChest()
        {
            _openChest = GetOpenChest();
        }
    }
}