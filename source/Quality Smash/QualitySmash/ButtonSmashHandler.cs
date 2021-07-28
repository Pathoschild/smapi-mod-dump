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
using System.Linq;
using StardewModdingAPI.Events;

namespace QualitySmash
{
    internal class ButtonSmashHandler
    {
        private readonly ModEntry modEntry;
        private readonly UiButtonHandler buttonHandler;
        private readonly ModConfig config;

        /// <summary>
        /// Initializes stuff for the mod.
        /// </summary>
        /// <param name="modEntry">The ModEntry</param>
        /// <param name="config">The mods config</param>
        /// <param name="imageColor">Button texture for the color smash button</param>
        /// <param name="imageQuality">Button texture for the quality smash button</param>
        public ButtonSmashHandler(ModEntry modEntry, ModConfig config)
        {
            this.modEntry = modEntry;
            this.config = config;

            this.buttonHandler = new UiButtonHandler(modEntry);
        }

        public void AddButton(ModEntry.SmashType smashType, Texture2D image, Rectangle clickableArea)
        {
            this.buttonHandler.AddButton(smashType, image, clickableArea);
        }

        public void RemoveButton(ModEntry.SmashType smashType)
        {
            this.buttonHandler.RemoveButton(smashType);
        }

        public void DrawButtons()
        {
            var menu = modEntry.GetValidButtonSmashMenu();

            if (menu == null)
                return;

            buttonHandler.UpdateBounds(menu);

            buttonHandler.DrawButtons();
           
        }

        internal void TryHover(float x, float y)
        {
            var menu = modEntry.GetValidButtonSmashMenu();

            if (menu != null)
            {
                buttonHandler.TryHover(x, y);
            }
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

            var buttonClicked = buttonHandler.GetButtonClicked((int)cursorPos.X, (int)cursorPos.Y);

            if (buttonClicked == ModEntry.SmashType.None)
                return;

            Game1.playSound("clubhit");
            DoSmash(menu, buttonClicked);
        }
        
        private bool IsFiltered(Item item)
        {
            if (item == null || !(item is StardewValley.Object)) return true;

            if (item.maximumStackSize() <= 1)
                return true;

            if (config.IgnoreItemsQuality.Contains(item.ParentSheetIndex) ||
                config.IgnoreItemsCategory.Contains(item.Category))
                return true;

            if (!config.IgnoreIridiumItemExceptions.Contains(item.ParentSheetIndex) &&
                !config.IgnoreIridiumCategoryExceptions.Contains(item.Category))
                if (config.IgnoreIridium && ((StardewValley.Object) item)?.Quality == 4) return true;

            if (config.IgnoreGold && ((StardewValley.Object) item)?.Quality == 2) return true;

            if (config.IgnoreSilver && ((StardewValley.Object) item)?.Quality == 1) return true;

            return false;
        }

        private void DoSmash(ItemGrabMenu menu, ModEntry.SmashType smashType)
        {
            var areItemsChanged = false;

            var containerInventory = menu.ItemsToGrabMenu.actualInventory.ToList();

            var itemsProcessed = new List<Item>();
            
            if (smashType == ModEntry.SmashType.Quality)
            {
                var itemsToSmash = containerInventory.FindAll(item => !IsFiltered(item));

                if (itemsToSmash.Count > 0) 
                {
                    areItemsChanged = true;
                    containerInventory.RemoveAll(item => !IsFiltered(item));
                    itemsToSmash.ForEach(i1 => { 
                        if (i1 is StardewValley.Object o) 
                            o.Quality = itemsToSmash
                                .FindAll(i2 => i2.ParentSheetIndex == i1.ParentSheetIndex)
                                .Cast<StardewValley.Object>()
                                .Min(i3 => i3.Quality);        
                    });
                    itemsProcessed = itemsToSmash;
                }
            } else if (smashType == ModEntry.SmashType.Color) 
            {
                for (var i = 0; i < containerInventory.Count; i++)
                {
                    if (containerInventory[i] == null || !(containerInventory[i] is StardewValley.Object))
                        continue;


                    if (config.EnableEggColorSmashing && containerInventory[i].Category == -5)
                    {
                        if (containerInventory[i].ParentSheetIndex == 180)
                            containerInventory[i].ParentSheetIndex = 176;

                        if (containerInventory[i].ParentSheetIndex == 182)
                            containerInventory[i].ParentSheetIndex = 174;

                        areItemsChanged = true;

                        itemsProcessed.Add(containerInventory[i]);

                        containerInventory.RemoveAt(i);
                        i--;

                        continue;
                    }

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
            }

            if (!areItemsChanged) return;

            menu.ItemsToGrabMenu.actualInventory.Clear();
            foreach (Item item in containerInventory)
                menu.ItemsToGrabMenu.actualInventory.Add(item);

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

