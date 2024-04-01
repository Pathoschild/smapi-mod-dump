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
using Unlockable_Bundles.Lib;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using Netcode;

namespace Unlockable_Bundles.Lib.ShopTypes
{
    public class DialogueShopMenu : IClickableMenu
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
        private long scrollCounter = 0; //Used to scroll cost name

        ClickableComponent YesButton;
        ClickableComponent NoButton;
        ClickableTextureComponent CostUpArrow;
        ClickableTextureComponent CostDownArrow;

        Farmer who;
        public ScreenSwipe ScreenSwipe;
        public bool CanClick = true;
        public bool Complete = false;
        public int CompletionTimer;

        private Item HoverItem;
        private string HoverText;

        public DialogueShopMenu(Farmer who, Unlockable unlockable)
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
                if (!Inventory.hasEnoughItems(who, requirement)) {
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
                    var displayName = Unlockable.parseItem(Unlockable.getIDFromReqSplit(lackingItem.Key.Split(',').First()), lackingItem.Value).DisplayName;
                    text = Helper.Translation.Get("ub_not_enough" + new Random().Next(1, 3), new { displayName = displayName });
                }

                Game1.activeClickableMenu = new DialogueBox(text);
                return;
            }

            Inventory.removeAllRequiredItems(who, Unlockable._price.Pairs);

            Unlockable.processPurchase();
            ScreenSwipe = new ScreenSwipe(0);
            CompletionTimer = 800;
            Complete = true;
            CanClick = false;
        }

        public void resetUI()
        {
            width = 1000;
            height = SpriteText.getHeightOfString(Unlockable.getTranslatedShopDescription(), width - 20) + 200;
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

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) => resetUI();

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!CanClick)
                return;

            if (YesButton.containsPoint(x, y))
                attemptPurchase();
            else if (NoButton.containsPoint(x, y))
                exitThisMenu();
            else if (upperRightCloseButton.containsPoint(x, y))
                exitThisMenu();
            else if (CostUpArrow.containsPoint(x, y) && costPageIndex != 0) {
                costPageIndex--;
                Game1.playSound("shwip");
            } else if (CostDownArrow.containsPoint(x, y) && Unlockable._price.Count() > (costPageIndex + 1) * 4) {
                costPageIndex++;
                Game1.playSound("shwip");
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!CanClick)
                return;

            exitThisMenu();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if (!CanClick)
                return;

            if ((b == Buttons.LeftShoulder || b == Buttons.LeftTrigger) && costPageIndex != 0) {
                costPageIndex--;
                Game1.playSound("shwip");
            } else if ((b == Buttons.RightShoulder || b == Buttons.RightTrigger) && Unlockable._price.Count() > (costPageIndex + 1) * 4) {
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
            } else if (direction < 0 && Unlockable._price.Count() > (costPageIndex + 1) * 4) {
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
            var maxChars = ModEntry.Config.ScrollCharacterLength;
            var delay = ModEntry.Config.ScrollDelay;

            if (displayName.Length <= maxChars)
                return displayName;

            var i = (int)(scrollCounter / delay) % (displayName.Length + 1);
            var s = displayName + " " + displayName + " ";

            return s.Substring(i, maxChars);
        }

        public override bool readyToClose()
        {
            if (!CanClick)
                return false;

            GamePadState currentPadState = Game1.input.GetGamePadState();
            KeyboardState keyState = Game1.GetKeyboardState();

            if (((currentPadState.IsButtonDown(Buttons.Start) && !Game1.oldPadState.IsButtonDown(Buttons.Start)) || (currentPadState.IsButtonDown(Buttons.B) && !Game1.oldPadState.IsButtonDown(Buttons.B))))
                exitThisMenu();

            if (keyState.IsKeyDown(Keys.Escape))
                exitThisMenu();

            if (Game1.options.menuButton.Any(e => keyState.IsKeyDown(e.key)))
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
            if (Unlockable._price.Count() > (costPageIndex + 1) * 4)
                CostDownArrow.draw(b);

            base.draw(b);
            drawToolTip(b);

            if (CanClick)
                drawMouse(b, true);

            updateScreenWipe(b);
        }

        private void drawToolTip(SpriteBatch b)
        {
            if (HoverItem is not null)
                drawToolTip(b, HoverItem.getDescription(), HoverItem.DisplayName, HoverItem);
            else if (HoverText is not null)
                drawHoverText(b, HoverText, Game1.dialogueFont);
        }


        public void updateScreenWipe(SpriteBatch b)
        {
            var time = Game1.currentGameTime;

            if (CompletionTimer > 0 && ScreenSwipe == null) {
                CompletionTimer -= time.ElapsedGameTime.Milliseconds;
                if (CompletionTimer <= 0) {
                    CanClick = true;
                    Game1.dialogueUp = false;
                    Game1.player.CanMove = true;
                    Unlockable.processShopEvent();
                }
            }

            if (ScreenSwipe != null) {
                CanClick = false;
                if (ScreenSwipe.update(time))
                    ScreenSwipe = null;
            }

            if (ScreenSwipe != null)
                ScreenSwipe.draw(b);
        }

        public void drawDescription(SpriteBatch b)
        {
            SpriteText.drawString(b, Unlockable.getTranslatedShopDescription(), x + 8, y + 12, 999999, base.width - 16);
        }

        public void drawCost(SpriteBatch b)
        {
            scrollCounter++;

            //Horizontal Border
            b.Draw(Game1.mouseCursors, new Rectangle(x, yPositionOfCostArea - 20, width, 24), new Rectangle(275, 313, 1, 6), Color.White);
            b.Draw(Game1.mouseCursors, new Vector2(x - 44, yPositionOfCostArea - 20), new Rectangle(261, 313, 14, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2(x + width - 8, yPositionOfCostArea - 20), new Rectangle(291, 313, 12, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);

            //Vertical Border
            b.Draw(Game1.mouseCursors, new Rectangle(xPositionOfCostArea - 40, yPositionOfCostArea, 36, heightOfCostArea), new Rectangle(278, 324, 9, 1), Color.White);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOfCostArea - 40, yPositionOfCostArea - 20), new Rectangle(278, 313, 10, 7), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOfCostArea - 40, y + base.height), new Rectangle(278, 328, 10, 8), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);

            HoverItem = null;
            HoverText = null;
            //Required items of the current cost page index
            for (int i = costPageIndex * 4; i < Unlockable._price.Count() && i != (costPageIndex + 1) * 4; i++) {
                int xPos = i % 2 == 0 ? xPositionOfCostArea : xPositionOfCostArea + (int)(widthOfCostArea / 2);
                int yPos = i % 4 < 2 ? yPositionOfCostArea : yPositionOfCostArea + (int)(heightOfCostArea / 2);

                var requirement = Unlockable._price.Pairs.ElementAt(i);

                if (requirement.Key.ToLower() == "money") {
                    b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos + 8, 54, 54), new Rectangle(280, 412, 15, 14), Color.White);
                    var gold = requirement.Value.ToString("# ### ##0") + "g  ";
                    Utility.drawTextWithShadow(b, "x " + gold, Game1.dialogueFont, new Vector2(xPos + 64f + 12f, yPos + 12f), who.Money >= requirement.Value ? Game1.textColor : Color.Red);
                    if (new Rectangle(xPos, yPos, 54, 54).Contains(Game1.getMousePosition()))
                        HoverText = gold;
                    continue;
                }

                var hasItems = Inventory.hasEnoughItems(who, requirement);
                var items = Unlockable.getRequiredItemsAllowExceptions(requirement.Key);
                items.First()?.drawInMenu(b, new Vector2(xPos, yPos), 0.9f, 1f, 1f, StackDrawType.HideButShowQuality, color: Color.White, false);
                if (requirement.Value > 1)
                    Utility.drawTinyDigits(requirement.Value, b, new Vector2(xPos, yPos) + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(requirement.Value, 3f)) + 3f, 64f - 18f + 1f), 3f, 1f, hasItems ? Color.White : Color.Red);
                Utility.drawTextWithShadow(b, shortenCostName(items.First()?.DisplayName), Game1.dialogueFont, new Vector2(xPos + 64f + 12f, yPos + 12f), hasItems ? Game1.textColor : Color.Red);

                if (items.Any() && new Rectangle(xPos, yPos, 58, 58).Contains(Game1.getMousePosition()))
                    HoverItem = items.First();
            }
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
