/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace NovoMundo.Menus
{
    public class NewJojaForm : IClickableMenu
    {
        public new const int width = 1280;
        public new const int height = 576;
        public const int buttonWidth = 147;
        public const int buttonHeight = 30;
        private Texture2D noteTexture;
        public List<ClickableComponent> checkboxes = new List<ClickableComponent>();
        private string hoverText;
        private bool boughtSomething;
        private int exitTimer = -1;

        public NewJojaForm(Texture2D noteTexture)
            : base(Game1.uiViewport.Width / 2 - 640, Game1.uiViewport.Height / 2 - 288, 1280, 576, showUpperRightCloseButton: true)
        {
            Game1.player.forceCanMove();
            this.noteTexture = noteTexture;
            int num = xPositionOnScreen + 4;
            int num2 = yPositionOnScreen + 208;
            for (int i = 0; i < 4; i++)
            {
                checkboxes.Add(new ClickableComponent(new Rectangle(num, num2, 588, 120), i.ToString() ?? "")
                {
                    myID = i,
                    rightNeighborID = ((i % 2 != 0 || i == 4) ? (-1) : (i + 1)),
                    leftNeighborID = ((i % 2 == 0) ? (-1) : (i - 1)),
                    downNeighborID = i + 2,
                    upNeighborID = i - 2
                });
                num += 592;
                if (num > xPositionOnScreen + 1184)
                {
                    num = xPositionOnScreen + 4;
                    num2 += 120;
                }
            }          
            if (Utility.doesAnyFarmerHaveOrWillReceiveMail("nmQuarry"))
            {
                checkboxes[0].name = "complete";
            }
            if (Utility.doesAnyFarmerHaveOrWillReceiveMail("nmCinema"))
            {
                checkboxes[1].name = "complete";
            }
            if (Utility.doesAnyFarmerHaveOrWillReceiveMail("nmFarm"))
            {
                checkboxes[2].name = "complete";
            }
            if (Utility.doesAnyFarmerHaveOrWillReceiveMail("nmLake"))
            {
                checkboxes[3].name = "complete";
            }
            exitFunction = onExitFunction;
            if (Game1.options.SnappyMenus)
            {
                populateClickableComponentList();
                snapToDefaultClickableComponent();
                Game1.mouseCursorTransparency = 1f;
            }
        }
        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }
        private void onExitFunction()
        {
            if (boughtSomething)
            {
                Game1.drawDialogue(Game1.getCharacterFromName("nmMorris"), ModEntry.ModHelper.Translation.Get("NPCDataMorrisBuySomethingMessage"));
            }
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (exitTimer >= 0)
            {
                return;
            }
            base.receiveLeftClick(x, y);
            foreach (ClickableComponent checkbox in checkboxes)
            {
                if (!checkbox.containsPoint(x, y) || checkbox.name.Equals("complete"))
                {
                    continue;
                }
                int num = Convert.ToInt32(checkbox.name);
                int priceFromButtonNumber = getPriceFromButtonNumber(num);
                if (Game1.player.Money >= priceFromButtonNumber)
                {
                    Game1.player.Money -= priceFromButtonNumber;
                    Game1.playSound("reward");
                    checkbox.name = "complete";
                    boughtSomething = true;
                    switch (num)
                    {
                        case 0:
                            Game1.addMailForTomorrow("nmQuarry", noLetter: true, sendToEveryone: true);
                            break;
                        case 1:
                            Game1.addMailForTomorrow("nmCinema", noLetter: true, sendToEveryone: true);
                            break;
                        case 2:
                            Game1.addMailForTomorrow("nmFarm", noLetter: true, sendToEveryone: true);
                            break;
                        case 3:
                            Game1.addMailForTomorrow("nmLake", noLetter: true, sendToEveryone: true);
                            break;
                    }
                    exitTimer = 1000;
                }
                else
                {
                    Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                }
            }
        }
        public override bool readyToClose()
        {
            return true;
        }
        public override void update(GameTime time)
        {
            base.update(time);
            if (exitTimer >= 0)
            {
                exitTimer -= time.ElapsedGameTime.Milliseconds;
                if (exitTimer <= 0)
                {
                    exitThisMenu();
                }
            }
            Game1.mouseCursorTransparency = 1f;
        }
        public int getPriceFromButtonNumber(int buttonNumber)
        {
            return buttonNumber switch
            {
                0 => 100000,
                1 => 500000,
                2 => 250000,
                3 => 100000,
                _ => -1,
            };
        }
        public string getDescriptionFromButtonNumber(int buttonNumber)
        {
            return ModEntry.ModHelper.Translation.Get("MenusNewJojaFormHoverMessage" + buttonNumber);
        }
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            hoverText = "";
            foreach (ClickableComponent checkbox in checkboxes)
            {
                if (checkbox.containsPoint(x, y))
                {
                    hoverText = (checkbox.name.Equals("complete") ? "" : Game1.parseText(getDescriptionFromButtonNumber(Convert.ToInt32(checkbox.name)), Game1.dialogueFont, 384));
                }
            }
        }
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            xPositionOnScreen = Game1.uiViewport.Width / 2 - 640;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - 288;
            int num = xPositionOnScreen + 4;
            int num2 = yPositionOnScreen + 208;
            checkboxes.Clear();
            for (int i = 0; i < 5; i++)
            {
                checkboxes.Add(new ClickableComponent(new Rectangle(num, num2, 588, 120), i.ToString() ?? ""));
                num += 592;
                if (num > xPositionOnScreen + 1184)
                {
                    num = xPositionOnScreen + 4;
                    num2 += 120;
                }
            }
        }
        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            b.Draw(noteTexture, Utility.getTopLeftPositionForCenteringOnScreen(1280, 576), new Rectangle(0, 0, 320, 144), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.79f);
            base.draw(b);
            foreach (ClickableComponent checkbox in checkboxes)
            {
                if (checkbox.name.Equals("complete"))
                {
                    b.Draw(noteTexture, new Vector2(checkbox.bounds.Left + 16, checkbox.bounds.Y + 16), new Rectangle(0, 144, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
                }
            }

            Game1.dayTimeMoneyBox.drawMoneyBox(b, Game1.uiViewport.Width - 300 - spaceToClearSideBorder * 2, 4);
            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);
            if (hoverText != null && !hoverText.Equals(""))
            {
                drawHoverText(b, hoverText, Game1.dialogueFont);
            }
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }
    }
}
