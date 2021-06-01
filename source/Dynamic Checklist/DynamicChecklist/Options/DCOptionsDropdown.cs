/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gunnargolf/DynamicChecklist
**
*************************************************/

namespace DynamicChecklist.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewValley;
    using StardewValley.Menus;

    public class DCOptionsDropDown : OptionsElement
    {
        private static DCOptionsDropDown selected;
        private static Rectangle dropDownBGSource = new Rectangle(433, 451, 3, 3);
        private static Rectangle dropDownButtonSource = new Rectangle(437, 450, 10, 11);
        private List<string> dropDownOptions = new List<string>();
        private int selectedOption;
        private int recentSlotY;
        private int startingSelected;
        private bool clicked;
        private Rectangle dropDownBounds;
        private bool held;
        private ModConfig config;

        public DCOptionsDropDown(string label, int whichOption, ModConfig config, int x = -1, int y = -1)
          : base(label, x, y, (int)Game1.smallFont.MeasureString("aaaaaaaaaaaaaaaaaaaaa").X + Game1.pixelZoom * 12, 11 * Game1.pixelZoom, 0)
        {
            switch (whichOption)
            {
                case 1:
                    this.dropDownOptions = new List<string>(Enum.GetNames(typeof(ModConfig.ButtonLocation)));
                    this.selectedOption = (int)config.OpenChecklistButtonLocation;
                    break;
                default:
                    throw new NotImplementedException();
            }

            this.whichOption = whichOption;
            this.config = config;
            this.dropDownBounds = new Rectangle(this.bounds.X, this.bounds.Y, this.bounds.Width - Game1.pixelZoom * 12, this.bounds.Height * this.dropDownOptions.Count);
        }

        public override void leftClickHeld(int x, int y)
        {
            if (this.greyedOut || !this.held)
            {
                return;
            }

            base.leftClickHeld(x, y);
            this.clicked = true;
            this.dropDownBounds.Y = Math.Min(this.dropDownBounds.Y, Game1.viewport.Height - this.dropDownBounds.Height - this.recentSlotY);
            this.selectedOption = (int)Math.Max(Math.Min((float)(y - this.dropDownBounds.Y) / (float)this.bounds.Height, (float)(this.dropDownOptions.Count - 1)), 0.0f);
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (this.greyedOut)
            {
                return;
            }

            this.held = true;
            base.receiveLeftClick(x, y);
            this.startingSelected = this.selectedOption;
            this.leftClickHeld(x, y);
            Game1.playSound("shwip");
            DCOptionsDropDown.selected = this;
        }

        public override void leftClickReleased(int x, int y)
        {
            if (this.greyedOut || this.dropDownOptions.Count <= 0 || !this.held)
            {
                return;
            }

            base.leftClickReleased(x, y);
            this.clicked = false;
            if (this.dropDownBounds.Contains(x, y))
            {
                switch (this.whichOption)
                {
                    case 1:
                        this.config.OpenChecklistButtonLocation = (ModConfig.ButtonLocation)this.selectedOption;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                this.selectedOption = this.startingSelected;
            }

            DCOptionsDropDown.selected = (DCOptionsDropDown)null;
            this.held = false;
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            this.recentSlotY = slotY;
            base.draw(b, slotX, slotY);
            float num = this.greyedOut ? 0.33f : 1f;
            if (this.clicked)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, DCOptionsDropDown.dropDownBGSource, slotX + this.dropDownBounds.X, slotY + this.dropDownBounds.Y, this.dropDownBounds.Width, this.dropDownBounds.Height, Color.White * num, (float)Game1.pixelZoom, false);
                for (int index = 0; index < this.dropDownOptions.Count; ++index)
                {
                    if (index == this.selectedOption)
                    {
                        b.Draw(Game1.staminaRect, new Rectangle(slotX + this.dropDownBounds.X, slotY + this.dropDownBounds.Y + index * this.bounds.Height, this.dropDownBounds.Width, this.bounds.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Wheat, 0.0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                    }

                    b.DrawString(Game1.smallFont, this.dropDownOptions[index], new Vector2((float)(slotX + this.dropDownBounds.X + Game1.pixelZoom), (float)(slotY + this.dropDownBounds.Y + Game1.pixelZoom * 2 + this.bounds.Height * index)), Game1.textColor * num, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.98f);
                }

                b.Draw(Game1.mouseCursors, new Vector2((float)(slotX + this.bounds.X + this.bounds.Width - Game1.pixelZoom * 12), (float)(slotY + this.bounds.Y)), new Rectangle?(DCOptionsDropDown.dropDownButtonSource), Color.Wheat * num, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.981f);
            }
            else
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, DCOptionsDropDown.dropDownBGSource, slotX + this.bounds.X, slotY + this.bounds.Y, this.bounds.Width - Game1.pixelZoom * 12, this.bounds.Height, Color.White * num, (float)Game1.pixelZoom, false);
                if (DCOptionsDropDown.selected == null || DCOptionsDropDown.selected == this)
                {
                    b.DrawString(Game1.smallFont, this.selectedOption >= this.dropDownOptions.Count || this.selectedOption < 0 ? string.Empty : this.dropDownOptions[this.selectedOption], new Vector2((float)(slotX + this.bounds.X + Game1.pixelZoom), (float)(slotY + this.bounds.Y + Game1.pixelZoom * 2)), Game1.textColor * num, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.88f);
                }

                b.Draw(Game1.mouseCursors, new Vector2((float)(slotX + this.bounds.X + this.bounds.Width - Game1.pixelZoom * 12), (float)(slotY + this.bounds.Y)), new Rectangle?(DCOptionsDropDown.dropDownButtonSource), Color.White * num, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
            }
        }
    }
}
