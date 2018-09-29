namespace DynamicChecklist.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using StardewValley;
    using StardewValley.Menus;

    public class DCOptionsInputListener : OptionsElement
    {
        private static Rectangle setButtonSource = new Rectangle(294, 428, 21, 11);
        private List<string> buttonNames = new List<string>();
        private string listenerMessage;
        private bool listening;
        private Rectangle setbuttonBounds;
        private ModConfig config;

        public DCOptionsInputListener(string label, int whichOption, int slotWidth, ModConfig config, int x = -1, int y = -1)
          : base(label, x, y, slotWidth, 11 * Game1.pixelZoom, whichOption)
        {
            this.config = config;
            this.setbuttonBounds = new Rectangle(x + slotWidth - 28 * Game1.pixelZoom, y, 21 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            if (whichOption == -1)
            {
                return;
            }

            switch (whichOption)
            {
                case 2:
                    this.buttonNames.Add(config.OpenMenuKey);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            int num = this.greyedOut ? 1 : 0;
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (this.greyedOut || this.listening || !this.setbuttonBounds.Contains(x, y))
            {
                return;
            }

            if (this.whichOption == -1)
            {
                Game1.options.setControlsToDefault();
                Game1.activeClickableMenu = (IClickableMenu)new GameMenu(6, 17);
            }
            else
            {
                this.listening = true;
                Game1.playSound("breathin");
                GameMenu.forcePreventClose = true;
                this.listenerMessage = "Press new key...";
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (this.greyedOut || !this.listening)
            {
                return;
            }

            if (key == Keys.Escape)
            {
                Game1.playSound("bigDeSelect");
                this.listening = false;
                GameMenu.forcePreventClose = false;
            }
            else if (!Game1.options.isKeyInUse(key) || new InputButton(key).ToString().Equals(this.buttonNames.First<string>()))
            {
                this.config.OpenMenuKey = key.ToString();
                this.buttonNames[0] = key.ToString();
                Game1.playSound("coin");
                this.listening = false;
                GameMenu.forcePreventClose = false;
            }
            else
            {
                this.listenerMessage = "Key already in use. Try again...";
            }
        }

        public override void draw(SpriteBatch b, int slotX, int slotY)
        {
            if (this.buttonNames.Count > 0 || this.whichOption == -1)
            {
                if (this.whichOption == -1)
                {
                    Utility.drawTextWithShadow(b, this.label, Game1.dialogueFont, new Vector2((float)(this.bounds.X + slotX), (float)(this.bounds.Y + slotY)), Game1.textColor, 1f, 0.15f, -1, -1, 1f, 3);
                }
                else
                {
                    Utility.drawTextWithShadow(b, this.label + ": " + this.buttonNames.Last<string>() + (this.buttonNames.Count > 1 ? ", " + this.buttonNames.First<string>() : string.Empty), Game1.dialogueFont, new Vector2((float)(this.bounds.X + slotX), (float)(this.bounds.Y + slotY)), Game1.textColor, 1f, 0.15f, -1, -1, 1f, 3);
                }
            }

            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float)(this.setbuttonBounds.X + slotX), (float)(this.setbuttonBounds.Y + slotY)), DCOptionsInputListener.setButtonSource, Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, false, 0.15f, -1, -1, 0.35f);
            if (!this.listening)
            {
                return;
            }

            b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.999f);
            b.DrawString(Game1.dialogueFont, this.listenerMessage, Utility.getTopLeftPositionForCenteringOnScreen(Game1.tileSize * 3, Game1.tileSize, 0, 0), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999f);
        }
    }
}
