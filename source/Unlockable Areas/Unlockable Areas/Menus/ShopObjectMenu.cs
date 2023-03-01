/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Menus;
using StardewValley;
using Unlockable_Areas.Lib;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using Netcode;

namespace Unlockable_Areas.Menus
{
    public class ShopObjectMenu : IClickableMenu
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
        }

        public Unlockable Unlockable;

        int x;
        int y;

        int xPositionOfCostArea;
        int yPositionOfCostArea;
        int widthOfCostArea;
        int heightOfCostArea;

        int costPageIndex = 0;

        ClickableComponent YesButton;
        ClickableComponent NoButton;
        ClickableTextureComponent CostUpArrow;
        ClickableTextureComponent CostDownArrow;

        Farmer who;

        public ShopObjectMenu(Farmer who, Unlockable unlockable)
        {
            Game1.playSound("bigSelect");
            Game1.dialogueUp = true;
            Game1.player.CanMove = false;

            Unlockable = unlockable;
            this.who = who;

            if (Game1.options.SnappyMenus) {
                Game1.mouseCursorTransparency = 0f;
            }

            resetUI();
        }

        public void attemptPurchase()
        {
            //Supposed to prevent two people purchasing the same Unlockable with as little control as I have over the netcode race condition
            if (!ShopPlacement.shopExists(Unlockable) || ModData.isUnlockablePurchased(Unlockable.ID, Unlockable.LocationUnique)) {
                exitThisMenu();
                return;
            }


            bool success = true;
            KeyValuePair<string, int> lackingItem = new KeyValuePair<string, int>("", 0);
            foreach (var requirement in Unlockable._price.Pairs) {
                if (!hasEnoughItemsInInventory(requirement)) {
                    lackingItem = requirement;
                    success = false;
                    break;
                }
            }

            if (!success) {
                exitThisMenu(false);
                string text = "";
                if (lackingItem.Key.ToLower() == "money")
                    text = Game1.content.LoadString("Strings\\UI:NotEnoughMoney" + new Random().Next(1, 4));
                else {
                    var displayName = new StardewValley.Object(int.Parse(lackingItem.Key.Split(',').First()), lackingItem.Value).DisplayName;
                    text = Helper.Translation.Get("ua_not_enough" + new Random().Next(1, 3), new { displayName = displayName });
                }

                Game1.activeClickableMenu = new DialogueBox(text);
                return;
            }

            ModData.setUnlockablePurchased(Unlockable.ID, Unlockable.LocationUnique);
            ModEntry._API.raiseShopPurchased(new API.ShopPurchasedEventArgs(who, Unlockable.Location, Unlockable.LocationUnique, Unlockable.ID, true));
            Helper.Multiplayer.SendMessage((UnlockableModel)Unlockable, "ApplyUnlockable/Purchased", modIDs: new[] { Mod.ModManifest.UniqueID });
            exitThisMenu();
            Task.Delay(800).ContinueWith(t => ShopPlacement.removeShop(Unlockable));
            removeAllRequiredItemsFromInventory();
            who.completelyStopAnimatingOrDoingAction();
            if (Unlockable.ShopEvent == "")
                Game1.globalFadeToBlack(playPurchasedEvent);
            else if (Unlockable.ShopEvent.ToLower() == "none")
                return;
            else
                Game1.globalFadeToBlack(() => who.currentLocation.startEvent(new Event(Unlockable.ShopEvent, -1, who)));
        }

        public void resetUI()
        {
            width = 1000;
            height = SpriteText.getHeightOfString(Unlockable.ShopDescription, width - 20) + 200;
            x = (int)Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height).X;
            y = Game1.uiViewport.Height - base.height - 64;

            xPositionOfCostArea = x + 300;
            yPositionOfCostArea = y + height - 126;
            widthOfCostArea = x + width - xPositionOfCostArea;
            heightOfCostArea = y + height - yPositionOfCostArea;

            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(x + width - 12, y - 32, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);

            int buttonHeight = (int)(heightOfCostArea / 2);
            YesButton = new ClickableComponent(new Rectangle(x, yPositionOfCostArea, width - widthOfCostArea, buttonHeight), "YesButton") {
                myID = 10,
                downNeighborID = 11,
                rightNeighborID = 100
            };

            NoButton = new ClickableComponent(new Rectangle(x, yPositionOfCostArea + buttonHeight + 1, width - widthOfCostArea, buttonHeight), "NoButton") {
                myID = 11,
                upNeighborID = 10,
                rightNeighborID = 101
            };

            CostUpArrow = new ClickableTextureComponent(new Rectangle(x + width - 48, y + height - heightOfCostArea, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f) {
                myID = 100,
                downNeighborID = 101,
                leftNeighborID = 10
            };
            CostDownArrow = new ClickableTextureComponent(new Rectangle(x + width - 48, y + height - 42, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f) {
                myID = 101,
                upNeighborID = 101,
                leftNeighborID = 11
            };

            currentlySnappedComponent = YesButton;
            if (Game1.options.SnappyMenus)
                snapCursorToCurrentSnappedComponent();
        }

        public void playPurchasedEvent()
        {
            Game1.freezeControls = true;
            DelayedAction.playSoundAfterDelay("crafting", 1000);
            DelayedAction.playSoundAfterDelay("crafting", 1500);
            DelayedAction.playSoundAfterDelay("crafting", 2000);
            DelayedAction.playSoundAfterDelay("crafting", 2500);
            DelayedAction.playSoundAfterDelay("axchop", 3000);
            DelayedAction.playSoundAfterDelay("Ship", 3200);
            Game1.viewportFreeze = true;
            Game1.viewport.X = -10000;
            Game1.pauseThenDoFunction(4000, doneWithPurchasedEvent);
        }

        public void doneWithPurchasedEvent()
        {
            UpdateHandler.applyUnlockable(Unlockable);
            Game1.globalFadeToClear();
            Game1.viewportFreeze = false;
            Game1.freezeControls = false;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) => resetUI();

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (YesButton.containsPoint(x, y)) {
                attemptPurchase();
            } else if (NoButton.containsPoint(x, y))
                exitThisMenu();
            else if (upperRightCloseButton.containsPoint(x, y))
                exitThisMenu();
            else if (CostUpArrow.containsPoint(x, y) && costPageIndex != 0) {
                costPageIndex--;
                Game1.playSound("shwip");
            } else if (CostDownArrow.containsPoint(x, y) && Unlockable.Price.Count > (costPageIndex + 1) * 4) {
                costPageIndex++;
                Game1.playSound("shwip");
            } else if (upperRightCloseButton.containsPoint(x, y)) {

            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            exitThisMenu();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if ((b == Buttons.LeftShoulder || b == Buttons.LeftTrigger) && costPageIndex != 0) {
                costPageIndex--;
                Game1.playSound("shwip");
            } else if ((b == Buttons.RightShoulder || b == Buttons.RightTrigger) && Unlockable.Price.Count > (costPageIndex + 1) * 4) {
                costPageIndex++;
                Game1.playSound("shwip");
            } else if (b == Buttons.LeftThumbstickUp || b == Buttons.DPadUp)
                moveSnappedCursorInDirection(0);
            else if (b == Buttons.LeftThumbstickDown || b == Buttons.DPadDown)
                moveSnappedCursorInDirection(2);

            base.receiveGamePadButton(b);
        }
        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && costPageIndex != 0) {
                costPageIndex--;
                Game1.playSound("shiny4");
            } else if (direction < 0 && Unlockable.Price.Count > (costPageIndex + 1) * 4) {
                costPageIndex++;
                Game1.playSound("shiny4");
            }
        }

        public override void performHoverAction(int x, int y)
        {
            this.CostUpArrow.tryHover(x, y, 0.5f);
            this.CostDownArrow.tryHover(x, y, 0.5f);
            this.upperRightCloseButton.tryHover(x, y, 0.5f);
        }

        public void moveSnappedCursorInDirection(int direction)
        {
            Game1.playSound("shwip");

            if (direction == 0)
                currentlySnappedComponent = YesButton;
            else if (direction == 2)
                currentlySnappedComponent = NoButton;
            snapCursorToCurrentSnappedComponent();
        }

        public string shortenCostName(string displayName)
        {
            var SplitString = Game1.parseText(displayName, Game1.dialogueFont, (int)((widthOfCostArea - 160) / 2)).Split(Environment.NewLine);
            return SplitString.Count() == 1 ? SplitString.First() : SplitString.First() + "..";
        }

        public override bool readyToClose()
        {
            GamePadState currentPadState = Game1.input.GetGamePadState();
            KeyboardState keyState = Game1.GetKeyboardState();

            if (((currentPadState.IsButtonDown(Buttons.Start) && !Game1.oldPadState.IsButtonDown(Buttons.Start)) || (currentPadState.IsButtonDown(Buttons.B) && !Game1.oldPadState.IsButtonDown(Buttons.B))))
                exitThisMenu();

            if (keyState.IsKeyDown(Keys.Escape))
                exitThisMenu();

            return false;
        }

        public new void exitThisMenu(bool playSound = true)
        {
            Game1.dialogueUp = false;
            Game1.player.CanMove = true;
            base.exitThisMenu(playSound);
        }

        public override void draw(SpriteBatch b)
        {
            drawBox(b, x, y, width, height);

            drawCost(b);
            drawDescription(b);

            drawResponse(b, Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes"), y + height - 110, YesButton.bounds);
            drawResponse(b, Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"), y + height - 50, NoButton.bounds);

            if (costPageIndex != 0)
                CostUpArrow.draw(b);
            if (Unlockable.Price.Count > (costPageIndex + 1) * 4)
                CostDownArrow.draw(b);

            base.draw(b);
            base.drawMouse(b, true);
        }

        public void drawDescription(SpriteBatch b)
        {
            SpriteText.drawString(b, Unlockable.ShopDescription, x + 8, y + 12, 999999, base.width - 16);
        }

        public void drawCost(SpriteBatch b)
        {
            //Horizontal Border
            b.Draw(Game1.mouseCursors, new Rectangle(x, yPositionOfCostArea - 20, width, 24), new Rectangle(275, 313, 1, 6), Color.White);
            b.Draw(Game1.mouseCursors, new Vector2(x - 44, yPositionOfCostArea - 20), new Rectangle(261, 313, 14, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2(x + width - 8, yPositionOfCostArea - 20), new Rectangle(291, 313, 12, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);

            //Vertical Border
            b.Draw(Game1.mouseCursors, new Rectangle(xPositionOfCostArea - 40, yPositionOfCostArea, 36, heightOfCostArea), new Rectangle(278, 324, 9, 1), Color.White);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOfCostArea - 40, yPositionOfCostArea - 20), new Rectangle(278, 313, 10, 7), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOfCostArea - 40, y + base.height), new Rectangle(278, 328, 10, 8), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);

            //Required items of the current cost page index
            for (int i = costPageIndex * 4; i < Unlockable.Price.Count && i != (costPageIndex + 1) * 4; i++) {
                int xPos = i % 2 == 0 ? xPositionOfCostArea : xPositionOfCostArea + (int)(widthOfCostArea / 2);
                int yPos = i % 4 < 2 ? yPositionOfCostArea : yPositionOfCostArea + (int)(heightOfCostArea / 2);

                var requirement = Unlockable._price.Pairs.ElementAt(i);

                if (requirement.Key.ToLower() == "money") {
                    b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos, 64, 64), new Rectangle(280, 412, 15, 14), Color.White);
                    Utility.drawTextWithShadow(b, "x " + requirement.Value.ToString("# ### ##0") + "g", Game1.dialogueFont, new Vector2(xPos + 64f + 12f, yPos + 12f), who.Money >= requirement.Value ? Game1.textColor : Color.Red);
                    continue;
                }

                var hasItems = hasEnoughItemsInInventory(requirement);

                var obj = new StardewValley.Object(int.Parse(requirement.Key.Split(",").First()), requirement.Value);
                obj.drawInMenu(b, new Vector2(xPos, yPos), 1f, 1f, 1f, StackDrawType.Hide, color: Color.White, false);
                if (requirement.Value > 1)
                    Utility.drawTinyDigits(requirement.Value, b, new Vector2(xPos, yPos) + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(requirement.Value, 3f)) + 3f, 64f - 18f + 1f), 3f, 1f, hasItems ? Color.White : Color.Red);
                Utility.drawTextWithShadow(b, shortenCostName(obj.DisplayName), Game1.dialogueFont, new Vector2(xPos + 64f + 12f, yPos + 12f), hasItems ? Game1.textColor : Color.Red);
            }
        }

        public bool hasEnoughItemsInInventory(KeyValuePair<string, int> requirement)
        {
            string currentKey = "";
            int countedItems = 0;
            try {
                var split = requirement.Key.Split(",");
                foreach (var item in split) {
                    currentKey = item;
                    countedItems += getItemCountInInventory(item);

                }
            } catch {
                Monitor.LogOnce($"Unlockable requirement key contains a invalid item id: {(requirement.Key == currentKey ? requirement.Key : requirement.Key + " -> " + currentKey)}", LogLevel.Error);
            }

            return countedItems >= requirement.Value;
        }

        public void removeAllRequiredItemsFromInventory()
        {
            foreach (var requirement in Unlockable._price.Pairs) {
                int removedItems = 0;

                foreach (var item in requirement.Key.Split(",")) {
                    int left = requirement.Value - removedItems;

                    var count = getItemCountInInventory(item);
                    if (count < left) {
                        subtractItemsFromInventory(item, count);
                        removedItems += count;
                    } else {
                        subtractItemsFromInventory(item, left);
                        break;
                    }
                }
            }
        }
        public void subtractItemsFromInventory(string key, int amount)
        {
            if (key.ToLower() == "money") {
                who.Money -= amount;
                return;
            }

            int id = int.Parse(key);
            who.removeItemsFromInventory(id, amount);
        }
        public int getItemCountInInventory(string key)
        {
            if (key.ToLower() == "money")
                return who.Money;

            int id = int.Parse(key);
            if (id == 858)
                return who.QiGems;
            else if (id == 73)
                return Game1.netWorldState.Value.GoldenWalnuts.Value;
            else
                return who.getItemCount(id);
        }

        public void drawResponse(SpriteBatch b, string text, int responseY, Rectangle bounds)
        {
            var responseSelected = bounds.Contains(Game1.getMouseX(), Game1.getMouseY());
            if (responseSelected)
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), x, responseY - 8, width - widthOfCostArea - 32, SpriteText.getHeightOfString(text, base.width) + 16, Color.White, 4f, drawShadow: false);
            SpriteText.drawString(b, text, x + 18, responseY, 999999, base.width, 999999, responseSelected ? 1f : 0.6f);
            responseY += SpriteText.getHeightOfString(text, base.width) + 16;
        }

        public void drawBox(SpriteBatch b, int xPos, int yPos, int boxWidth, int boxHeight)
        {
            b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos, boxWidth, boxHeight), new Rectangle(306, 320, 16, 16), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos - 20, boxWidth, 24), new Rectangle(275, 313, 1, 6), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPos + 12, yPos + boxHeight, boxWidth - 20, 32), new Rectangle(275, 328, 1, 8), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPos - 32, yPos + 24, 32, boxHeight - 28), new Rectangle(264, 325, 8, 1), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPos + boxWidth, yPos, 28, boxHeight), new Rectangle(293, 324, 7, 1), Color.White);
            b.Draw(Game1.mouseCursors, new Vector2(xPos - 44, yPos - 28), new Rectangle(261, 311, 14, 13), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2(xPos + boxWidth - 8, yPos - 28), new Rectangle(291, 311, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2(xPos + boxWidth - 8, yPos + boxHeight - 8), new Rectangle(291, 326, 12, 12), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2(xPos - 44, yPos + boxHeight - 4), new Rectangle(261, 327, 14, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
        }

    }
}
