using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using xTile.Dimensions;

namespace StardewMods.ArchaeologyHouseContentManagementHelper.Framework.Menus
{
    internal class MuseumMenuNoInventory : IClickableMenu
    {
        private bool holdingMuseumPiece;

        // Original location of the held item to enable item swapping.
        private int oldX1;
        private int oldY1;

        private string selectedItemDescription;
        private string selectedItemTitle;

        private Item selectedItem;

        private Item heldItem;

        private double granularity;

        private double infoFadeTimerStartValue = 100;
        private double infoFadeTimerCurrentValue = 100;

        private static Timer infoFadeTimer;

        private readonly System.Object lockInfoFadeTimer = new System.Object();

        public int fadeTimer;
        public int state;
        public bool fadeIntoBlack;
        public float blackFadeAlpha;

        public MuseumMenuNoInventory()
        {
            this.fadeTimer = 800;
            this.fadeIntoBlack = true;

            int duration = ModEntry.ModConfig.MuseumItemDisplayTime;
            if (duration > 0)
            {
                infoFadeTimer = new System.Timers.Timer
                {
                    Interval = 200
                };

                if (duration < 200)
                    duration = 200;

                granularity = infoFadeTimerStartValue / (duration / infoFadeTimer.Interval);

                // Hook up the Elapsed event for the timer. 
                infoFadeTimer.Elapsed += InfoFadeTimer_OnTimedEvent;

                // Have the timer fire repeated events (true is the default)
                infoFadeTimer.AutoReset = true;
            }

            /* Don't display item tooltip */
            else if (duration == 0)
            {
                infoFadeTimerCurrentValue = 0;
            }

            /* Display item tooltip without time limits */
            else
            {
                // We only need to set it to a value > 0, specific value doesn't matter
                infoFadeTimerCurrentValue = 1;
            }

            this.movePosition(0, Game1.viewport.Height - this.yPositionOnScreen - this.height);
            Game1.player.forceCanMove();
            if (!Game1.options.SnappyMenus)
                return;

            //this.populateClickableComponentList();
            //this.currentlySnappedComponent = this.getComponentWithID(0);
            //this.snapCursorToCurrentSnappedComponent();
        }

        private void InfoFadeTimer_OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            infoFadeTimerCurrentValue -= granularity;
        }

        public void movePosition(int dx, int dy)
        {
            this.xPositionOnScreen += dx;
            this.yPositionOnScreen += dy;
            //this.inventory.movePosition(dx, dy);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (this.fadeTimer > 0)
            {
                return;
            }

            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
            {
                this.state = 2;
                this.fadeTimer = 500;
                this.fadeIntoBlack = true;
            }

            else if (Game1.options.SnappyMenus)
            {
                base.receiveKeyPress(key);
            }

            if (Game1.options.SnappyMenus)
            {
                return;
            }

            if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                Game1.panScreen(0, 4);
            else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                Game1.panScreen(4, 0);
            else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
            {
                Game1.panScreen(0, -4);
            }
            else
            {
                if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                    return;
                Game1.panScreen(-4, 0);
            }
        }

        // Enable free cursor movement even if you don't hold an item
        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return true;
        }

        // Added [item swap] support
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            Item heldItem = this.heldItem;
            if (!holdingMuseumPiece)
            {
                this.heldItem = null;
            }

            LibraryMuseum museum = Game1.currentLocation as LibraryMuseum;

            // Place item at a museum slot
            if (heldItem != null && this.heldItem != null
                && (y < Game1.viewport.Height - (height - (IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192))))
            {
                int x1 = (x + Game1.viewport.X) / 64;
                int y1 = (y + Game1.viewport.Y) / 64;

                // Place item at an empty museum slot
                if (museum.isTileSuitableForMuseumPiece(x1, y1)
                    && museum.isItemSuitableForDonation(this.heldItem))
                {
                    int parentSheetIndex = this.heldItem.ParentSheetIndex;
                    int count = museum.getRewardsForPlayer(Game1.player).Count;

                    // Add item to the current position
                    museum.museumPieces.Add(new Vector2((float)x1, (float)y1), (this.heldItem as StardewValley.Object).ParentSheetIndex);


                    Game1.playSound("stoneStep");
                    holdingMuseumPiece = false;

                    // Clear hover information
                    selectedItemDescription = null;
                    selectedItemTitle = null;
                    selectedItem = null;

                    // Play item placement sound
                    Game1.playSound("newArtifact");

                    // If the player has no donatable item in his inventory, hide the inventory
                    //if (!museum.doesFarmerHaveAnythingToDonate(Game1.player))
                    //{
                    //    showInventory = false;
                    //}

                    //Game1.player.completeQuest(24);
                    --this.heldItem.Stack;
                    if (this.heldItem.Stack <= 0)
                    {
                        this.heldItem = (Item)null;
                    }

                    //int num = museum.museumPieces.Count();
                    //if (num >= 95)
                    //{
                    //    Game1.getAchievement(5);
                    //}
                    //else if (num >= 40)
                    //{
                    //    Game1.getAchievement(28);
                    //}
                }

                // Place item at an already in-use museum slot and swap it with its current item
                else if (LibraryMuseumHelper.IsTileSuitableForMuseumPiece(x1, y1)
                    && museum.isItemSuitableForDonation(this.heldItem))
                {
                    Vector2 keySrc = new Vector2((float)oldX1, (float)oldY1);
                    Vector2 keyDest = new Vector2((float)x1, (float)y1);

                    Item swapItem = null;

                    // Remove current item at museum slot
                    LibraryMuseum currentLocation = Game1.currentLocation as LibraryMuseum;
                    if (currentLocation.museumPieces.ContainsKey(keyDest))
                    {
                        swapItem = (Item)new StardewValley.Object(currentLocation.museumPieces[keyDest], 1, false, -1, 0);
                        currentLocation.museumPieces.Remove(keyDest);
                    }

                    // Place held item at the museum slot
                    currentLocation.museumPieces.Add(keyDest, (this.heldItem as StardewValley.Object).ParentSheetIndex);

                    // Place removed item at the old location of the held item to complete the swap
                    if (swapItem != null)
                    {
                        currentLocation.museumPieces.Add(keySrc, (swapItem as StardewValley.Object).ParentSheetIndex);
                    }

                    Game1.playSound("stoneStep");
                    holdingMuseumPiece = false;

                    // Clear hover information
                    selectedItemDescription = null;
                    selectedItemTitle = null;
                    selectedItem = null;

                    Game1.playSound("newArtifact");

                    --this.heldItem.Stack;
                    if (this.heldItem.Stack <= 0)
                    {
                        this.heldItem = (Item)null;
                    }
                }
            }

            // Grab an item which is already located in the museum
            else if (this.heldItem == null)
            {
                Vector2 key = new Vector2((float)((x + Game1.viewport.X) / 64), (float)((y + Game1.viewport.Y) / 64));
                LibraryMuseum currentLocation = Game1.currentLocation as LibraryMuseum;
                if (currentLocation.museumPieces.ContainsKey(key))
                {
                    // save current slot of the item so it can be swapped
                    this.heldItem = (Item)new StardewValley.Object(currentLocation.museumPieces[key], 1, false, -1, 0);
                    oldX1 = (int)key.X;
                    oldY1 = (int)key.Y;

                    // Hover information
                    selectedItemDescription = this.heldItem?.getDescription();
                    selectedItemTitle = this.heldItem?.DisplayName;

                    selectedItem = this.heldItem;

                    // Restart the info fade timer
                    lock (lockInfoFadeTimer)
                    {
                        if (infoFadeTimer != null)
                        {
                            infoFadeTimerCurrentValue = infoFadeTimerStartValue;
                            infoFadeTimer.Start();
                        }
                    }

                    currentLocation.museumPieces.Remove(key);
                    holdingMuseumPiece = !currentLocation.museumAlreadyHasArtifact(this.heldItem.ParentSheetIndex);
                }
            }

            //if (this.heldItem != null && heldItem == null)
            //{
            //    this.menuMovingDown = true;
            //}

            //if (!readyToClose())
            //{
            //    return;
            //}

            //state = 2;
            //fadeTimer = 800;
            //fadeIntoBlack = true;
            //Game1.playSound("bigDeSelect");
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Item heldItem = this.heldItem;
            if (this.fadeTimer <= 0)
            {
                base.receiveRightClick(x, y, true);
            }

            //if (this.heldItem == null || heldItem != null)
            //{
            //    return;
            //}

            //this.menuMovingDown = true;
        }

        public override bool readyToClose()
        {
            return !this.holdingMuseumPiece;
        }

        protected override void cleanupBeforeExit()
        {
            if (this.heldItem == null)
            {
                return;
            }

            //this.heldItem = Game1.player.addItemToInventory(this.heldItem);
            //if (this.heldItem == null)
            //    return;
            //Game1.createItemDebris(this.heldItem, Game1.player.Position, -1, (GameLocation)null, -1);

            this.heldItem = (Item)null;
        }

        public override void update(GameTime time)
        {
            base.update(time);

            if (this.fadeTimer > 0)
            {
                this.fadeTimer -= time.ElapsedGameTime.Milliseconds;
                this.blackFadeAlpha = !this.fadeIntoBlack ? (float)(1.0 - (1500.0 - (double)this.fadeTimer) / 1500.0) 
                                                          : (float)(0.0 + (1500.0 - (double)this.fadeTimer) / 1500.0);
                if (this.fadeTimer <= 0)
                {
                    switch (this.state)
                    {
                        case 0:
                            this.state = 1;
                            Game1.viewportFreeze = true;
                            Game1.viewport.Location = new Location(1152, 128);
                            Game1.clampViewportToGameMap();
                            this.fadeTimer = 800;
                            this.fadeIntoBlack = false;
                            break;
                        case 2:
                            Game1.viewportFreeze = false;
                            this.fadeIntoBlack = false;
                            this.fadeTimer = 800;
                            this.state = 3;
                            break;
                        case 3:
                            this.exitThisMenuNoSound();
                            break;
                    }
                }
            }

            //if (this.menuMovingDown && this.menuPositionOffset < this.height / 3)
            //{
            //    this.menuPositionOffset += 8;
            //    this.movePosition(0, 8);
            //}
            //else if (!this.menuMovingDown && this.menuPositionOffset > 0)
            //{
            //    this.menuPositionOffset -= 8;
            //    this.movePosition(0, -8);
            //}


            int num1 = Game1.getOldMouseX() + Game1.viewport.X;
            int num2 = Game1.getOldMouseY() + Game1.viewport.Y;
            if (num1 - Game1.viewport.X < 64)
            {
                Game1.panScreen(-4, 0);
            }
            else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= -64)
            {
                Game1.panScreen(4, 0);
            }

            if (num2 - Game1.viewport.Y < 64)
            {
                Game1.panScreen(0, -4);
            }
            else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
            {
                Game1.panScreen(0, 4);
            }

            foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
            {
                this.receiveKeyPress(pressedKey);
            }
        }

        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.movePosition(0, Game1.viewport.Height - this.yPositionOnScreen - this.height);

            Game1.player.forceCanMove();
        }

        public override void draw(SpriteBatch b)
        {
            if ((fadeTimer <= 0 || !fadeIntoBlack) && state != 3)
            {
                if (heldItem != null)
                {
                    for (int y = Game1.viewport.Y / 64 - 1; y < (Game1.viewport.Y + Game1.viewport.Height) / 64 + 2; ++y)
                    {
                        for (int x = Game1.viewport.X / 64 - 1; x < (Game1.viewport.X + Game1.viewport.Width) / 64 + 1; ++x)
                        {
                            var tileClassification = LibraryMuseumHelper.GetTileMuseumClassification(x, y, ModEntry.ModConfig.ShowVisualSwapIndicator);
                            if (tileClassification != MuseumTileClassification.Invalid)
                            {
                                Color tileBorderColor = Color.LightGreen;

                                if (tileClassification == MuseumTileClassification.Limited)
                                {
                                    tileBorderColor = Color.Black;
                                }

                                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)x, (float)y) * 64f),
                                    new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29, -1, -1)),
                                    tileBorderColor);
                            }
                        }
                    }
                }

                heldItem?.drawInMenu(b, new Vector2((float)(Game1.getOldMouseX() + 8), (float)(Game1.getOldMouseY() + 8)), 1f);

                drawMouse(b);

                //draw clicked item information
                if (selectedItemDescription != null && !selectedItemDescription.Equals(""))
                {
                    if (infoFadeTimerCurrentValue > 0)
                    {
                        // Show selected item information
                        drawToolTip(b, this.selectedItemDescription, this.selectedItemTitle, selectedItem,
                            selectedItem != null, -1, 0, -1, -1, (CraftingRecipe)null, -1);
                    }
                    else
                    {
                        infoFadeTimer?.Stop();
                    }
                }
            }

            b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), 
                Color.Black * blackFadeAlpha);
        }
    }
}
