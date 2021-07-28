/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jn84/QualitySmash
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.DynamicData;
using Microsoft.Build.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace QualitySmash
{
    internal class SingleSmashHandler
    {
        private string hoverTextColor;
        private string hoverTextQuality;
        private readonly Texture2D cursorColor;
        private readonly Texture2D cursorQuality;
        private readonly ModEntry modEntry;
        private readonly ModConfig config;

        private IList<Item> actualItems;

        public SingleSmashHandler(ModEntry modEntry, ModConfig config, Texture2D cursorColor, Texture2D cursorQuality)
        {
            this.modEntry = modEntry;
            this.config = config;
            this.cursorColor = cursorColor;
            this.cursorQuality = cursorQuality;

            this.hoverTextColor = "";
            this.hoverTextQuality = "";
        }

        internal void HandleClick(ButtonPressedEventArgs e)
        {
            var menu = modEntry.GetValidKeybindSmashMenu();

            if (menu == null || !config.EnableSingleItemSmashKeybinds)
                return;

            ModEntry.SmashType smashType;
            if (modEntry.helper.Input.IsDown(config.ColorSmashKeybind))
                smashType = ModEntry.SmashType.Color;
            else if (modEntry.helper.Input.IsDown(config.QualitySmashKeybind))
                smashType = ModEntry.SmashType.Quality;
            else
                return;

            bool oldUiMode = Game1.uiMode;
            Game1.uiMode = true;
            var cursorPos = e.Cursor.GetScaledScreenPixels();
            Game1.uiMode = oldUiMode;

            // If the cursor was over a valid item when left clicked, initiate smash
            var cursorHoverItem = CheckInventoriesForCursorHoverItem(menu, cursorPos);

            if (cursorHoverItem == null)
                return;

            var itemToSmash = GetActualItem(cursorHoverItem);

            if (itemToSmash != null)
                DoSmash(itemToSmash, smashType);
        }

        private Item GetActualItem(ClickableComponent clickableItem)
        {
            var itemSlotNumber = Convert.ToInt32(clickableItem.name);

            if (actualItems == null)
                return null;

            if (itemSlotNumber < actualItems.Count && actualItems[itemSlotNumber] != null)
                return actualItems[itemSlotNumber];
            return null;
        }

        private ClickableComponent CheckInventoriesForCursorHoverItem(IClickableMenu menu, Vector2 cursorPos)
        {
            ClickableComponent itemToSmash;

            if (menu is ItemGrabMenu grabMenu)
            {
                itemToSmash = ScanForHoveredItem(grabMenu.inventory.inventory, cursorPos);
                if (itemToSmash != null)
                {
                    this.actualItems = grabMenu.inventory.actualInventory;
                    return itemToSmash;
                }

                itemToSmash = ScanForHoveredItem(grabMenu.ItemsToGrabMenu.inventory, cursorPos);
                if (itemToSmash != null)
                {
                    this.actualItems = grabMenu.ItemsToGrabMenu.actualInventory;
                    return itemToSmash;
                }
            }
            if (menu is GameMenu gameMenu)
            {
                if (!(gameMenu.GetCurrentPage() is InventoryPage inventoryPage))
                    return null;

                itemToSmash = ScanForHoveredItem(inventoryPage.inventory.inventory, cursorPos);
                if (itemToSmash != null)
                {
                    actualItems = inventoryPage.inventory.actualInventory;
                    return itemToSmash;
                }
            }

            return null;
        }

        private ClickableComponent ScanForHoveredItem(List<ClickableComponent> clickableItems, Vector2 cursorPos)
        {
            foreach (var clickableItem in clickableItems)
            {
                if (!clickableItem.containsPoint((int)cursorPos.X, (int)cursorPos.Y))
                    continue;
                return clickableItem;
            }
            return null;
        }

        private void DoSmash(Item item, ModEntry.SmashType smashType)
        {
            if (item.maximumStackSize() <= 1)
                return;

            Game1.playSound("clubhit");

            if (smashType == ModEntry.SmashType.Color)
            {
                if (item.category == -80 && item is ColoredObject c)
                    c.color.Value = default;
            }

            if (smashType == ModEntry.SmashType.Quality)
            {
                if (item is StardewValley.Object o && o.Quality != 0)
                    o.Quality /= 2;
            }
        }

        private bool IsSmashable(Item item, ModEntry.SmashType smashType)
        {
            if (item == null)
                return false;

            if (smashType == ModEntry.SmashType.Color)
            {
                if (item.category == -80 && item is ColoredObject c)
                    return true;
            }

            if (smashType == ModEntry.SmashType.Quality)
            {
                if (item is StardewValley.Object o && o.Quality != 0)
                    return true;
            }
            return false;
        }

        // Should be reworked to hover over any item in any inventory
        internal bool TryHover(float x, float y)
        {
            var menu = modEntry.GetValidKeybindSmashMenu();

            if (menu == null || !config.EnableSingleItemSmashKeybinds)
                return false;

            this.hoverTextColor = "";
            this.hoverTextQuality = "";

            var cursorPos = new Vector2(x, y);

            var item = CheckInventoriesForCursorHoverItem(menu, cursorPos);

            if (item != null)
            {
                if (modEntry.helper.Input.IsDown(config.ColorSmashKeybind))
                {
                    if (item.containsPoint((int) x, (int) y) && IsSmashable(GetActualItem(item), ModEntry.SmashType.Color))
                    {
                         this.hoverTextColor = modEntry.helper.Translation.Get("hoverTextColor");
                         return true;
                    }
                }
                else if (modEntry.helper.Input.IsDown(config.QualitySmashKeybind))
                {
                    if (item.containsPoint((int) x, (int) y) && IsSmashable(GetActualItem(item), ModEntry.SmashType.Quality))
                    {
                        this.hoverTextQuality = modEntry.helper.Translation.Get("hoverTextQuality");
                        return true;
                    }
                }
            }
            return false;
        }

        public void DrawHoverText()
        {
            Texture2D cursor = null;
            var yOffset = 0;
            var xOffset = 0;

            if (this.hoverTextColor != "")
            {
                IClickableMenu.drawHoverText(Game1.spriteBatch, hoverTextColor, Game1.smallFont, 57, -87);
                cursor = this.cursorColor;
                yOffset = -50;
                xOffset = 32;
            }

            else if (this.hoverTextQuality != "")
            {
                IClickableMenu.drawHoverText(Game1.spriteBatch, hoverTextQuality, Game1.smallFont, 57, -87);
                cursor = this.cursorQuality;
                yOffset = -50;
                xOffset = 32;
            }
            else
            {
                cursor = Game1.mouseCursors;
                yOffset = 0;
                xOffset = 0;
            }

            // Draws cursor over the GUI element
            Game1.spriteBatch.Draw(cursor, new Vector2(Game1.getOldMouseX() + xOffset, Game1.getOldMouseY() + yOffset),
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero,
                4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0);
        }
    }
}
