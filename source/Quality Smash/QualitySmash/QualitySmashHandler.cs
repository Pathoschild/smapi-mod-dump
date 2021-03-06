/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jn84/QualitySmash
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;
using StardewModdingAPI.Events;

namespace QualitySmash
{
    internal class QualitySmashHandler
    {
        private string hoverTextColor;
        private string hoverTextQuality;
        private readonly ModEntry modEntry;
        private readonly ClickableTextureComponent buttonColor;
        private readonly ClickableTextureComponent buttonQuality;
        private readonly Texture2D imageColor;
        private readonly Texture2D imageQuality;
        private readonly ModConfig config;

        /// <summary>
        /// Initializes stuff for the mod.
        /// </summary>
        /// <param name="modEntry">The ModEntry</param>
        /// <param name="config">The mods config</param>
        /// <param name="imageColor">Button texture for the color smash button</param>
        /// <param name="imageQuality">Button texture for the quality smash button</param>
        public QualitySmashHandler(ModEntry modEntry, ModConfig config, Texture2D imageColor, Texture2D imageQuality)
        {
            this.modEntry = modEntry;
            this.config = config;
            this.imageColor = imageColor;
            this.imageQuality = imageQuality;
            buttonColor = new ClickableTextureComponent(Rectangle.Empty, null, new Rectangle(0, 0, 16, 16), 4f)
            {
                hoverText = modEntry.helper.Translation.Get("hoverTextColor"),
                myID = 102906,
                leftNeighborID = 27346,
                downNeighborID = 102907,
            };

            buttonQuality = new ClickableTextureComponent(Rectangle.Empty, null, new Rectangle(0, 0, 16, 16), 4f)
            {
                hoverText = modEntry.helper.Translation.Get("hoverTextQuality"),
                myID = 102907,
                leftNeighborID = 12952,
                upNeighborID = 102906,
            };
        }

        internal void PopulateIds(ItemGrabMenu menu)
        {

            if (menu.fillStacksButton != null)
            {
                buttonQuality.leftNeighborID = menu.fillStacksButton.myID;
                menu.fillStacksButton.rightNeighborID = 102907;
            }

            if (menu.colorPickerToggleButton != null)
            {
                menu.colorPickerToggleButton.rightNeighborID = 102906;
                buttonColor.leftNeighborID = menu.colorPickerToggleButton.myID;
            }
        }

        private void UpdateButtonPositions()
        {
            var menu = Game1.activeClickableMenu;

            if (menu == null) 
                return;

            if (menu is ItemGrabMenu grabMenu)
                PopulateIds(grabMenu);

            const int length = 64;
            const int positionFromBottom = 3;
            const int gapSize = 16;

            var screenX = menu.xPositionOnScreen + menu.width + gapSize + length;
            var screenY = menu.yPositionOnScreen + menu.height / 3 - (length * positionFromBottom) - (gapSize * (positionFromBottom - 1));

            buttonColor.bounds = new Rectangle(screenX, screenY, length, length);
            buttonQuality.bounds = new Rectangle(screenX, screenY + gapSize + length, length, length);
        }

        public void DrawButtons()
        {
            UpdateButtonPositions();

            buttonColor.texture = imageColor;
            buttonQuality.texture = imageQuality;

            buttonColor.draw(Game1.spriteBatch, Color.White, 0f, 0);
            buttonQuality.draw(Game1.spriteBatch, Color.White, 0f, 0);

            if (hoverTextColor != "")
                IClickableMenu.drawHoverText(Game1.spriteBatch, hoverTextColor, Game1.smallFont);

            if (hoverTextQuality != "")
                IClickableMenu.drawHoverText(Game1.spriteBatch, hoverTextQuality, Game1.smallFont);

            // Draws cursor over the GUI element
            Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero,
            4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0);
        }

        internal bool TryHover(float x, float y)
        {
            this.hoverTextColor = "";
            this.hoverTextQuality = "";
            var menu = modEntry.GetValidButtonSmashMenu();

            if (menu != null)
            { 
                buttonColor.tryHover((int)x, (int)y, 0.25f);
                if (buttonColor.containsPoint((int)x, (int)y))
                {
                    this.hoverTextColor = buttonColor.hoverText;
                    return true;
                }

                buttonQuality.tryHover((int)x, (int)y, 0.25f);
                if (buttonQuality.containsPoint((int)x, (int)y))
                {
                    this.hoverTextQuality = buttonQuality.hoverText;
                    return true;
                }
            }
            return false;
        }

        internal void HandleClick(ButtonPressedEventArgs e)
        {
            ItemGrabMenu menu = null;

            var oldUiMode = Game1.uiMode;
            Game1.uiMode = true;
            var cursorPos = e.Cursor.GetScaledScreenPixels();
            Game1.uiMode = oldUiMode;

            if (modEntry.GetValidButtonSmashMenu() is ItemGrabMenu)
                menu = modEntry.GetValidButtonSmashMenu() as ItemGrabMenu;

            if (menu == null)
                return;

            if (buttonColor.containsPoint((int)cursorPos.X, (int)cursorPos.Y))
            {
                Game1.playSound("clubhit");
                DoSmash(menu, ModEntry.SmashType.Color);
            }

            if (buttonQuality.containsPoint((int)cursorPos.X, (int)cursorPos.Y))
            {
                Game1.playSound("clubhit");
                DoSmash(menu, ModEntry.SmashType.Quality);
            }
        }

        private void DoSmash(ItemGrabMenu menu, ModEntry.SmashType smashType)
        {
            var areItemsChanged = false;

            var containerInventory = menu.ItemsToGrabMenu.actualInventory;

            var itemsProcessed = new List<Item>();

            for (var i = 0; i < containerInventory.Count; i++)
            {
                if (containerInventory[i] == null || !(containerInventory[i] is StardewValley.Object))
                    continue;
                if (smashType == ModEntry.SmashType.Color)
                {
                    if (!(containerInventory[i] is ColoredObject c) ||
                        c.Category != -80 ||
                        config.IgnoreItemsColor.Contains(containerInventory[i].ParentSheetIndex))
                        continue;

                    areItemsChanged = true;

                    c.color.Value = default;

                    itemsProcessed.Add(containerInventory[i]);

                    containerInventory.RemoveAt(i);
                    i--;
                }
                else if (smashType == ModEntry.SmashType.Quality)
                {
                    if ((containerInventory[i] as StardewValley.Object)?.Quality == 0) continue;

                    if (config.IgnoreItemsQuality.Contains(containerInventory[i].ParentSheetIndex) ||
                        config.IgnoreItemsCategory.Contains(containerInventory[i].Category))
                        continue;

                    if (!config.IgnoreIridiumItemExceptions.Contains(containerInventory[i].ParentSheetIndex) &&
                        !config.IgnoreIridiumCategoryExceptions.Contains(containerInventory[i].Category))
                        if (config.IgnoreIridium && (containerInventory[i] as StardewValley.Object)?.Quality == 4) continue;

                    if (config.IgnoreGold && (containerInventory[i] as StardewValley.Object)?.Quality == 2) continue;

                    if (config.IgnoreSilver && (containerInventory[i] as StardewValley.Object)?.Quality == 1) continue;

                    // Filtering complete 

                    areItemsChanged = true;

                    if (containerInventory[i] is StardewValley.Object o)
                        o.Quality = 0;

                    itemsProcessed.Add(containerInventory[i]);

                    containerInventory.RemoveAt(i);
                    i--;
                }
            }

            if (!areItemsChanged) return;

            // There's probably a simpler way to do this built into the game, but I don't see it.
            // Prime the container with some of each item
            AddSomeOfEach(menu, itemsProcessed);
            
            // Use a modified version of game's quick stack code to add the rest
            FillOutStacks(menu, itemsProcessed);
        }

        /// <summary>
        /// Modified version of the game's FillOutStacks method.
        /// </summary>
        /// <param name="menu">The active ItemGrabMenu (Chest, Fridge, etc.)</param>
        /// <param name="itemsToProcess">This list of items that were modified by the Smash methods</param>
        private void FillOutStacks(ItemGrabMenu menu, IList<Item> itemsToProcess)
        {
            var containerInventory = menu.ItemsToGrabMenu.actualInventory;

            for (var i = 0; i < containerInventory.Count; i++)
            {
                var containerItem = containerInventory[i];
                if (containerItem == null || containerItem.maximumStackSize() <= 1)
                    continue;

                for (var j = 0; j < itemsToProcess.Count; j++)
                {
                    var processingItem = itemsToProcess[j];
                    if (processingItem == null || !containerItem.canStackWith(processingItem))
                        continue;

                    var processingItemStackSize = processingItem.Stack;

                    if (containerItem.getRemainingStackSpace() > 0)
                    {
                        processingItemStackSize = containerItem.addToStack(processingItem);

                        menu?.ItemsToGrabMenu?.ShakeItem(containerItem);
                    }
                    processingItem.Stack = processingItemStackSize;

                    while (processingItem.Stack > 0)
                    {
                        Item overflowStack = null;

                        if (overflowStack == null)
                        {
                            for (var l = 0; l < containerInventory.Count; l++)
                            {
                                if (containerInventory[l] != null && containerInventory[l].canStackWith(containerItem) && containerInventory[l].getRemainingStackSpace() > 0)
                                {
                                    overflowStack = containerInventory[l];
                                    break;
                                }
                            }
                        }

                        if (overflowStack == null)
                        {
                            for (var k = 0; k < containerInventory.Count; k++)
                            {
                                if (containerInventory[k] == null)
                                {
                                    var item = containerInventory[k] = containerItem.getOne();
                                    overflowStack = item;
                                    overflowStack.Stack = 0;
                                    break;
                                }
                            }
                        }

                        if (overflowStack == null && containerInventory.Count < Chest.capacity)
                        {
                            overflowStack = containerItem.getOne();
                            overflowStack.Stack = 0;
                            containerInventory.Add(overflowStack);
                        }

                        if (overflowStack == null)
                        {
                            break;
                        }

                        processingItemStackSize = overflowStack.addToStack(processingItem);
                        menu.ItemsToGrabMenu.ShakeItem(containerItem);
                        processingItem.Stack = processingItemStackSize;
                    }

                    if (processingItem.Stack == 0)
                    {
                        itemsToProcess[j] = null;
                    }
                }
            }
        }

        /// <summary>
        /// This method is to "prime" the container with items so that FillOutStacks will work
        /// </summary>
        /// <param name="menu">The active ItemGrabMenu (Chest, Fridge, etc.)</param>
        /// <param name="itemsToProcess">This list of items that were modified by the Smash methods</param>
        private void AddSomeOfEach(ItemGrabMenu menu, IList<Item> itemsToProcess)
        {
            var containerInventory = menu.ItemsToGrabMenu.actualInventory;

            // Handle edge case where container is empty after modifying every item in the container.
            // When the container is empty, the inner loop will never proceed, and no items will be re-added to the container
            if (containerInventory.Count == 0)
            {
                // Make the container not empty
                containerInventory.Add(itemsToProcess[0]);
                itemsToProcess.RemoveAt(0);
            }

            for (var i = 0; i < itemsToProcess.Count; i++)
            {
                if (itemsToProcess[i] == null || itemsToProcess[i].maximumStackSize() <= 1) 
                    continue;

                for (var j = 0; j < containerInventory.Count; j++)
                {
                    // Reminder to myself to not change this
                    // This is a nested 'if' because otherwise in an edge case where the last item in a chest
                    // is not stackable, no items will be added since the code does not continue on to "if (j + 1 == containerInventory.Count)"
                    if (containerInventory[j] != null && containerInventory[j].maximumStackSize() > 1)
                        // Found a stackable match, process the next item
                        if (containerInventory[j].canStackWith(itemsToProcess[i]))
                            break;

                    // Reached the end, and no stackable match was found, so add the item to the container
                    if (j + 1 == containerInventory.Count)
                    {
                        containerInventory.Add(itemsToProcess[i]);
                        itemsToProcess[i] = null;
                        break;
                    }
                }
            }
        }
    }
}

