using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using static StardewValley.Menus.ItemGrabMenu;

namespace ChefHelperAddToFridges.AddToFridges
{
    class AddToFridgesHandler
    {
        private ModEntry modEntry;
        private ClickableTextureComponent button;
        private string hoverText;
        internal GameLocation currentLocation;
        private Texture2D image;
        private Texture2D imageDisabled;
        private List<TransferredItemSprite> transferredItemSprites;

        /// <summary>
        /// Initializes stuff for the mod.
        /// </summary>
        /// <param name="modEntry"></param>
        /// <param name="image"></param>
        /// <param name="imageDisabled"></param>
        public AddToFridgesHandler(ModEntry modEntry, Texture2D image, Texture2D imageDisabled)
        {
            this.modEntry = modEntry;
            this.image = image;
            this.imageDisabled = imageDisabled;
            modEntry.Monitor.Log($"Handler created.");
            button = new ClickableTextureComponent(Rectangle.Empty, null, new Rectangle(0, 0, 16, 16), 4f)
            {
                hoverText = modEntry.helper.Translation.Get("hoverText.enabled")
            };

            transferredItemSprites = new List<TransferredItemSprite>();
        }

        /// <summary>
        /// Updates the location of the button, in case window is moved
        /// or resized.
        /// </summary>
        private void UpdatePos()
        {
            // Fill Stacks Button Bounds
            // new Rectangle(xPositionOnScreen + width, yPositionOnScreen + height / 3 - 64 - 64 - 16, 64, 64)

            var menu = Game1.activeClickableMenu;
            if (menu == null) return;

            var length = 16 * Game1.pixelZoom;
            const int positionFromBottom = 3;
            const int gapSize = 16;

            var screenX = menu.xPositionOnScreen + menu.width;
            var screenY = menu.yPositionOnScreen + menu.height / 3 - (length * positionFromBottom) - (gapSize * (positionFromBottom - 1));

            button.bounds = new Rectangle(screenX, screenY, length, length);
        }

        /// <summary>
        /// Checks to see if any other fridges in the area are in use.
        /// Only returns true if not.
        /// </summary>
        /// <returns></returns>
        internal bool FridgesAreFree()
        {
            if (currentLocation == null)
                return false;

            // A Farmhouse location has a main fridge so we'll have to check that
            if (currentLocation is StardewValley.Locations.FarmHouse)
            {
                var farmHouse = currentLocation as StardewValley.Locations.FarmHouse;

                // we need a check for IsLockHeld since whatever fridge the player is
                // looking at will be locked by them.
                if (farmHouse.fridge.Value != null && farmHouse.fridge.Value.mutex.IsLocked() && !farmHouse.fridge.Value.mutex.IsLockHeld())
                    return false;
            }

            // Now we're checking any mini-fridges
            foreach (StardewValley.Object item in currentLocation.objects.Values)
            {
                if (item != null && item is Chest)
                {
                    var chest = item as Chest;
                    if (chest.fridge.Value)
                    {
                        // we need a check for IsLockHeld since whatever fridge the player is
                        // looking at will be locked by them.
                        if (chest.mutex.IsLocked() && !chest.mutex.IsLockHeld())
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Draws button on top of the fridge menu GUI
        /// </summary>
        public void DrawButton()
        {
            UpdatePos();

            // If Fridges aren't free, we use a desaturated image to
            // indicate the button is disabled.
            button.texture = FridgesAreFree() ? image : imageDisabled;
            button.draw(Game1.spriteBatch);

            if (hoverText != "")
                IClickableMenu.drawHoverText(Game1.spriteBatch, hoverText, Game1.smallFont);

            // Draws cursor over the GUI element
            Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero,
            4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0);
        }

        internal bool TryHover(float x, float y)
        {
            this.hoverText = "";
            var menu = modEntry.ReturnFridgeMenu();

            if (menu != null)
            {
                if (FridgesAreFree())
                    button.tryHover((int)x, (int)y, 0.25f);

                if (button.containsPoint((int)x, (int)y))
                {
                    this.hoverText = FridgesAreFree() ? button.hoverText : modEntry.helper.Translation.Get("hoverText.disabled");
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Modified version of game's ItemGrabMenu.FillOutStacks().
        /// Works with any given chest instead of just the one
        /// the Farmer is currently interacting with.
        /// 
        /// Because the Expanded Fridge doesn't come with any support for
        /// FillOutStacks() (and thus has no support for transferred
        /// item sprites/shakeItem), for now if the main fridge is
        /// accessed while Expanded Fridge is installed, the visual
        /// nuances will not display.
        /// </summary>
        /// <param name="chest"></param>
        public void FillOutStacks(Chest chest)
        {
            ItemGrabMenu menu = null;
            if (modEntry.ReturnFridgeMenu() is ItemGrabMenu)
                menu = modEntry.ReturnFridgeMenu() as ItemGrabMenu;
            var inventory = modEntry.ReturnFridgeMenu().inventory;

            // Goes through each item in the chest
            for (int i = 0; i < chest.items.Count; i++)
            {
                Item chest_item = chest.items[i];
                // If there's no item in the slot, or the item cannot be stacked (such as weapons), move on
                if (chest_item == null || chest_item.maximumStackSize() <= 1)
                {
                    continue;
                }

                // Now we'll search through the inventory for a matching item to stack
                for (int j = 0; j < inventory.actualInventory.Count; j++)
                {
                    Item inventory_item = inventory.actualInventory[j];
                    if (inventory_item == null || !chest_item.canStackWith(inventory_item))
                    {
                        continue;
                    }

                    // This handles the visual effect of the items in the inventory drifting up and disappearing as they're put in the stack.
                    // This is ItemGrabMenu stuff, and since ExpandedFridgeMenu extends MenuWithInventory (the parent of ItemGrabMenu) it
                    // doesn't have this functionality.
                    if (menu != null)
                    {
                        TransferredItemSprite item_sprite = new TransferredItemSprite(inventory_item.getOne(), inventory.inventory[j].bounds.X, inventory.inventory[j].bounds.Y);
                        var transferredItemSprites = modEntry.helper.Reflection.GetField<List<TransferredItemSprite>>(menu, "_transferredItemSprites").GetValue();
                        transferredItemSprites.Add(item_sprite);
                    } else { 
                        TransferredItemSprite item_sprite = new TransferredItemSprite(inventory_item.getOne(), inventory.inventory[j].bounds.X, inventory.inventory[j].bounds.Y);
                        //modEntry.Monitor.Log($"Added {item_sprite.item.DisplayName}");
                        transferredItemSprites.Add(item_sprite);
                        //modEntry.Monitor.Log($"transferredItemSprites.Count = {transferredItemSprites.Count}");
                    }

                    int stack_count2 = inventory_item.Stack;

                    // Now we add that item to the stack...
                    if (chest_item.getRemainingStackSpace() > 0)
                    {
                        stack_count2 = chest_item.addToStack(inventory_item);

                        // Same deal here, ItemsToGrabMenu is from ItemGrabMenu and ExpandedFridgeMenu has no ShakeItem...
                        menu?.ItemsToGrabMenu?.ShakeItem(chest_item);
                    }
                    inventory_item.Stack = stack_count2;

                    // Have we hit the item's stack limit/is there overflow?
                    while (inventory_item.Stack > 0)
                    {
                        Item overflow_stack = null;
                        // If so, is there still space left in the chest for it?
                        if (!Utility.canItemBeAddedToThisInventoryList(chest_item.getOne(), chest.items, Chest.capacity))
                        {
                            break;
                        }

                        // First iteration through, we're looking to see if there are other stacks of this item that aren't full
                        if (overflow_stack == null)
                        {
                            for (int l = 0; l < chest.items.Count; l++)
                            {
                                if (chest.items[l] != null && chest.items[l].canStackWith(chest_item) && chest.items[l].getRemainingStackSpace() > 0)
                                {
                                    overflow_stack = chest.items[l];
                                    break;
                                }
                            }
                        }

                        // If there aren't any, we get an empty slot to fill.
                        // First we look to see if there are empty spaces between
                        // items (where the user may have taken something out)
                        if (overflow_stack == null)
                        {
                            for (int k = 0; k < chest.items.Count; k++)
                            {
                                if (chest.items[k] == null)
                                {
                                    Item item = chest.items[k] = chest_item.getOne();
                                    overflow_stack = item;
                                    overflow_stack.Stack = 0;
                                    break;
                                }
                            }
                        }

                        // If there aren't any, we get the first empty space after all
                        // the other chest items.
                        if (overflow_stack == null && chest.items.Count < Chest.capacity)
                        {
                            overflow_stack = chest_item.getOne();
                            overflow_stack.Stack = 0;
                            chest.items.Add(overflow_stack);
                        }

                        // There is no room for the overflow; the rest of the stack will
                        // remain in the player's inventory.
                        if (overflow_stack == null)
                        {
                            break;
                        }

                        // Then we set whatever remains in the user's inventory and do the ShakeItem thing.
                        stack_count2 = overflow_stack.addToStack(inventory_item);
                        menu?.ItemsToGrabMenu?.ShakeItem(chest_item);
                        inventory_item.Stack = stack_count2;
                    }

                    // If there's no overflow, the slot taken by the item in the user's inventory becomes empty.
                    if (inventory_item.Stack == 0)
                    {
                        inventory.actualInventory[j] = null;
                    }
                }
            }
        }

        internal void UpdateTransferredItemSprites()
        {
            if (transferredItemSprites.Count > 0)
                modEntry.Monitor.Log($"transferredItemSprites.Count: {transferredItemSprites.Count}");
            for (int i = 0; i < transferredItemSprites.Count; i++)
            {
                if (transferredItemSprites[i].Update(Game1.currentGameTime))
                {
                    transferredItemSprites.RemoveAt(i);
                    i--;
                }
            }
        }

        internal void DrawTransferredItems(SpriteBatch spriteBatch)
        {
            foreach (TransferredItemSprite transferredItemSprite in transferredItemSprites)
            {
                transferredItemSprite.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Using the FillOutStacks method, fills each fridge in the area.
        /// </summary>
        private void FillFridges()
        {
            // Fill main fridge first
            if (currentLocation is StardewValley.Locations.FarmHouse) {
                var farmHouse = currentLocation as StardewValley.Locations.FarmHouse;

                // If it's an Expanded Fridge, we need to treat it specially...
                if (farmHouse.fridge.Value.GetType().FullName == "ExpandedFridge.ExpandedFridgeHub")
                {
                    List<Chest> hubChests = modEntry.helper.Reflection.GetProperty<List<Chest>>(farmHouse.fridge.Value, "connectedChests").GetValue();

                    // We fill out each page in the fridge
                    foreach (Chest chest in hubChests)
                        FillOutStacks(chest);

                // Here we just fill out a regular fridge, which doesn't get stored
                // in objects for the Game Location for whatever reason.
                } else if (farmHouse.fridge.Value != null)
                    FillOutStacks(farmHouse.fridge.Value);
            }

            // Then fill all mini-fridges
            foreach (StardewValley.Object item in currentLocation.objects.Values)
            {
                if (item != null && item is Chest) {
                    var chest = item as Chest;
                    if (chest.fridge.Value) {
                        FillOutStacks(chest);
                    }
                }
            }
        }

        /// <summary>
        /// Plays the sound and fills fridges if the user clicks on the button
        /// and the fridges are not in use by other players.
        /// </summary>
        /// <param name="cursor"></param>
        internal void HandleClick(ICursorPosition cursor)
        {
            var screenPixels = cursor.ScreenPixels;

            if (!button.containsPoint((int)screenPixels.X, (int)screenPixels.Y)) return;

            if (FridgesAreFree()) { 
                Game1.playSound("Ship");

                FillFridges();
            }
        }
    }
}
