/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.GameModifications
{
    public class MultiSleepSelectionMenu : NumberSelectionMenu
    {
        private string _message;

        public MultiSleepSelectionMenu(string message, behaviorOnNumberSelect behaviorOnSelection, int price = -1, int minValue = 0, int maxValue = 99, int defaultNumber = 0)
            : base(message, behaviorOnSelection, price, minValue, maxValue, defaultNumber)
        {
            _message = message;
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            b.DrawString(Game1.dialogueFont, _message, new Vector2(xPositionOnScreen + borderWidth, yPositionOnScreen + spaceToClearTopBorder + borderWidth / 2), Game1.textColor);
            okButton.draw(b);
            cancelButton.draw(b);
            leftButton.draw(b);
            rightButton.draw(b);
            if (price > 0)
            {
                var totalPrice = price * (currentValue - 1);
                var text = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", totalPrice);
                var positionX = (float)(rightButton.bounds.Right + 32 + (priceShake > 0 ? Game1.random.Next(-1, 2) : 0));
                var positionY = (float)(rightButton.bounds.Y + (priceShake > 0 ? Game1.random.Next(-1, 2) : 0));
                var position = new Vector2(positionX, positionY);
                b.DrawString(Game1.dialogueFont, text, position, totalPrice > Game1.player.Money ? Color.Red : Game1.textColor);
            }
            numberSelectedBox.Draw(b);
            drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (rightButton.containsPoint(x, y))
            {
                var num = currentValue + 1;
                if (num <= maxValue && (price == -1 || currentValue * price <= Game1.player.Money))
                {
                    rightButton.scale = rightButton.baseScale;
                    currentValue = num;
                    numberSelectedBox.Text = currentValue.ToString() ?? "";
                    Game1.playSound("smallSelect");
                }
                return;
            }
            base.receiveLeftClick(x, y, playSound);
        }
    }
}
