/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace StardewMods.ArchaeologyHouseContentManagementHelper.Framework.Menus
{
    internal class MuseumMenuEx : MuseumMenu
    {
        private bool showInventory;
        private readonly IReflectedField<bool> holdingMuseumPieceRef;

        private readonly Multiplayer multiplayer;

        // Original location of the held item to enable item swapping.
        private int oldX1;
        private int oldY1;

        private string selectedItemDescription;
        private string selectedItemTitle;

        private Item selectedItem;

        private bool selectedInventoryItem;

        private double granularity;

        private double infoFadeTimerStartValue = 100;
        private double infoFadeTimerCurrentValue = 100;

        private static Timer infoFadeTimer;

        private readonly System.Object lockInfoFadeTimer = new System.Object();

        public MuseumMenuEx()
        {
            holdingMuseumPieceRef = ModEntry.CommonServices.ReflectionHelper.GetField<bool>(this, "holdingMuseumPiece");
            multiplayer = ModEntry.CommonServices.ReflectionHelper.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

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
           

            showInventory = true;
        }

        private void InfoFadeTimer_OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            infoFadeTimerCurrentValue -= granularity;
        }

        // Enable free cursor movement when inventory is not shown
        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return !showInventory || this.heldItem != null;
        }

        private void PrepareHideItemInfoTooltip()
        {
            // Stop the info fade timer
            infoFadeTimer?.Stop();

            // Clear hover information
            selectedItemDescription = null;
            selectedItemTitle = null;
            selectedItem = null;
        }

        // Added [item swap] support
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.fadeTimer > 0)
            {
                return;
            }

            Item heldItem = this.heldItem;
            if (!holdingMuseumPieceRef.GetValue())
            {
                this.heldItem = this.inventory.leftClick(x, y, this.heldItem, true);
                selectedInventoryItem = this.heldItem != null;
            }

            LibraryMuseum museum = Game1.currentLocation as LibraryMuseum;

            // Place item at a museum slot
            if (heldItem != null && this.heldItem != null 
                && (y < Game1.viewport.Height - (height - (IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192)) 
                    || this.menuMovingDown || !selectedInventoryItem || !inventory.isWithinBounds(x, y)))
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
                    holdingMuseumPieceRef.SetValue(false);

                    PrepareHideItemInfoTooltip();

                    // Rewards
                    if (museum.getRewardsForPlayer(Game1.player).Count > count)
                    {
                        this.sparkleText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:NewReward"), Color.MediumSpringGreen, Color.White, false, 0.1, 2500, -1, 500);
                        Game1.playSound("reward");
                        this.globalLocationOfSparklingArtifact = new Vector2((float)(x1 * 64 + 32) - this.sparkleText.textWidth / 2f, (float)(y1 * 64 - 48));
                    }

                    else
                    {
                        Game1.playSound("newArtifact");
                    }

                    // If the player has no donatable item in his inventory, hide the inventory
                    if (!museum.doesFarmerHaveAnythingToDonate(Game1.player))
                    {
                        showInventory = false;
                    }

                    Game1.player.completeQuest(24);
                    --this.heldItem.Stack;
                    if (this.heldItem.Stack <= 0)
                    {
                        this.heldItem = (Item)null;
                    }

                    this.menuMovingDown = false;
                    int num = museum.museumPieces.Count();
                    if (num >= 95)
                    {
                        Game1.getAchievement(5);
                    }
                    else if (num >= 40)
                    {
                        Game1.getAchievement(28);
                    }
                    else if (selectedInventoryItem)
                    {
                        multiplayer.globalChatInfoMessage("donation", Game1.player.Name, "object:" + (object)parentSheetIndex);
                    }
                }
                
                // Place item at an already in-use museum slot and swap it with its current item
                else if (LibraryMuseumHelper.IsTileSuitableForMuseumPiece(x1, y1) 
                    && museum.isItemSuitableForDonation(this.heldItem) && !selectedInventoryItem)
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
                    holdingMuseumPieceRef.SetValue(false);

                    PrepareHideItemInfoTooltip();

                    Game1.playSound("newArtifact");

                    --this.heldItem.Stack;
                    if (this.heldItem.Stack <= 0)
                    {
                        this.heldItem = (Item)null;
                    }
                }
            }

            // Grab an item which is already located in the museum
            else if (this.heldItem == null && (!inventory.isWithinBounds(x, y) || !showInventory)
                && (okButton == null || !okButton.containsPoint(x, y)))
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
                    holdingMuseumPieceRef.SetValue(!currentLocation.museumAlreadyHasArtifact(this.heldItem.ParentSheetIndex));
                }
            }

            if (this.heldItem != null && heldItem == null)
            {
                this.menuMovingDown = true;
            }

            if (!showInventory || okButton == null || !okButton.containsPoint(x, y) || !readyToClose())
            {
                return;
            }

            state = 2;
            fadeTimer = 800;
            fadeIntoBlack = true;
            Game1.playSound("bigDeSelect");
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
                            var tileClassification = LibraryMuseumHelper.GetTileMuseumClassification(
                                x, y, (!selectedInventoryItem && ModEntry.ModConfig.ShowVisualSwapIndicator));
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

                if (!holdingMuseumPieceRef.GetValue() && showInventory)
                {
                    base.draw(b, false, false);
                }

                if (!hoverText.Equals(""))
                {
                    drawHoverText(b, hoverText, Game1.smallFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
                }

                heldItem?.drawInMenu(b, new Vector2((float)(Game1.getOldMouseX() + 8), (float)(Game1.getOldMouseY() + 8)), 1f);

                drawMouse(b);
                sparkleText?.draw(b, Game1.GlobalToLocal(Game1.viewport, globalLocationOfSparklingArtifact));

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

            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * blackFadeAlpha);
        }
    }
}
