using System;
using System.Collections.Generic;
using AutoGrabberMod.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace AutoGrabberMod.UserInterfaces
{
    public class DropDown : OptionsElement
    {
        public const int pixelsHigh = 11;

        public static DropDown selected;

        public List<string> dropDownDisplayOptions = new List<string>();

        public int selectedOption;

        public int recentSlotY;

        public int startingSelected;

        private readonly Action<int> SetValue;

        private bool clicked;
        private Rectangle dropDownBounds;
        public static Rectangle dropDownBGSource = new Rectangle(433, 451, 3, 3);
        public static Rectangle dropDownButtonSource = new Rectangle(437, 450, 10, 11);

        public DropDown(string label, List<string> displayOptions, Action<int> setValue, int selected = 0, int x = -1, int y = -1) : base(label, x, y, (int)Game1.smallFont.MeasureString("Windowed Borderless Mode   ").X + 48, 44)
        {
            this.dropDownDisplayOptions = displayOptions;
            this.dropDownBounds = new Rectangle(base.bounds.X, base.bounds.Y, base.bounds.Width - 48, base.bounds.Height * this.dropDownDisplayOptions.Count);
            this.SetValue = setValue;
            this.selectedOption = selected;
        }

        public override void leftClickHeld(int x, int y)
        {
            if (!base.greyedOut)
            {
                base.leftClickHeld(x, y);
                this.clicked = true;
                this.dropDownBounds.Y = Math.Min(this.dropDownBounds.Y, Game1.viewport.Height - this.dropDownBounds.Height - this.recentSlotY);
                this.selectedOption = (int)Math.Max(Math.Min((float)(y - this.dropDownBounds.Y) / (float)base.bounds.Height, (float)(this.dropDownDisplayOptions.Count - 1)), 0f);
            }
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (!base.greyedOut)
            {
                base.receiveLeftClick(x, y);
                this.startingSelected = this.selectedOption;
                this.leftClickHeld(x, y);
                Game1.playSound("shwip");
                DropDown.selected = this;
            }
        }

        public override void leftClickReleased(int x, int y)
        {
            if (!base.greyedOut && this.dropDownDisplayOptions.Count > 0)
            {
                base.leftClickReleased(x, y);
                this.clicked = false;
                if (this.dropDownBounds.Contains(x, y))
                {
                    //Game1.options.changeDropDownOption(base.whichOption, this.selectedOption, this.dropDownOptions);
                    this.SetValue(this.selectedOption);
                }
                else
                {
                    this.selectedOption = this.startingSelected;
                }
                OptionsDropDown.selected = null;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
                if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                {
                    this.selectedOption++;
                    if (this.selectedOption >= this.dropDownDisplayOptions.Count)
                    {
                        this.selectedOption = 0;
                    }
                }
                else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                {
                    this.selectedOption--;
                    if (this.selectedOption < 0)
                    {
                        this.selectedOption = this.dropDownDisplayOptions.Count - 1;
                    }
                }
            }
        }

        public override void draw(SpriteBatch b, int slotX, int slotY)
        {
            this.recentSlotY = slotY;
            base.draw(b, slotX, slotY);
            float scale = base.greyedOut ? 0.33f : 1f;
            if (this.clicked)
            {
                Utilities.drawTextureBox(
                    b,
                    Game1.mouseCursors,
                    OptionsDropDown.dropDownBGSource,
                    slotX + this.dropDownBounds.X,
                    slotY + this.dropDownBounds.Y,
                    this.dropDownBounds.Width,
                    this.dropDownBounds.Height,
                    Color.White * scale,
                    4f,
                    false
                );

                for (int i = 0; i < this.dropDownDisplayOptions.Count; i++)
                {
                    if (i == this.selectedOption)
                    {
                        b.Draw(
                            Game1.staminaRect,
                            new Rectangle(slotX + this.dropDownBounds.X, slotY + this.dropDownBounds.Y + i * base.bounds.Height, this.dropDownBounds.Width, base.bounds.Height),
                            new Rectangle(0, 0, 1, 1),
                            Color.Wheat,
                            0f,
                            Vector2.Zero,
                            SpriteEffects.None,
                            0.975f
                        );
                    }

                    b.DrawString(
                        Game1.smallFont,
                        this.dropDownDisplayOptions[i],
                        new Vector2((float)(slotX + this.dropDownBounds.X + 4), (float)(slotY + this.dropDownBounds.Y + 8 + base.bounds.Height * i)),
                        Game1.textColor * scale,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0.98f
                    );
                }
                
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2((float)(slotX + base.bounds.X + base.bounds.Width - 48), (float)(slotY + base.bounds.Y)),
                    OptionsDropDown.dropDownButtonSource,
                    Color.Wheat * scale,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    0.981f
                );
            }
            else
            {
                IClickableMenu.drawTextureBox(
                    b,
                    Game1.mouseCursors,
                    OptionsDropDown.dropDownBGSource,
                    slotX + base.bounds.X, slotY + base.bounds.Y,
                    base.bounds.Width - 48,
                    base.bounds.Height,
                    Color.White * scale,
                    4f,
                    false
                );

                if (OptionsDropDown.selected == null || OptionsDropDown.selected.Equals(this))
                {
                    b.DrawString(
                        Game1.smallFont,
                        (this.selectedOption < this.dropDownDisplayOptions.Count && this.selectedOption >= 0) ? this.dropDownDisplayOptions[this.selectedOption] : "",
                        new Vector2((float)(slotX + base.bounds.X + 4), (float)(slotY + base.bounds.Y + 8)),
                        Game1.textColor * scale,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0.88f
                    );
                }
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2((float)(slotX + base.bounds.X + base.bounds.Width - 48), (float)(slotY + base.bounds.Y)),
                    OptionsDropDown.dropDownButtonSource,
                    Color.White * scale,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    0.88f
                );
            }
        }
    }
}
