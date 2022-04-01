/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Util;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>OptionsDropDown specialized for displaying numbers in a grid.</summary>
    public class OptionsNumericDropDown : OptionsDropDown
    {
        public enum WrapStyle
        {
            None,
            Horizontal,
            Vertical
        }

        private readonly FieldInfo _clickedField;

        public int Start { get; set; }

        public int End { get; set; }

        public WrapStyle Wrap { get; set; }

        public int WrapLimit { get; set; }

        protected bool Clicked
        {
            get
            {
                return (bool)(_clickedField.GetValue(this) ?? false);
            }

            set
            {
                _clickedField.SetValue(this, value);
            }
        }

        public OptionsNumericDropDown(string label, int start, int end, int x = -1, int y = -1) :
            this(label, start, end, WrapStyle.None, 10, x, y)
        {
        }

        public OptionsNumericDropDown(string label, int start, int end, WrapStyle wrap, int wrapLimit, int x = -1, int y = -1) :
            base(label, 0, x, y)
        {
            _clickedField = ReflectionHelper.TryGetField<OptionsDropDown>("clicked", BindingFlags.Instance | BindingFlags.NonPublic);
            Wrap = wrap;
            WrapLimit = wrapLimit;
            
            FillOptions(start, end);
        }

        public void FillOptions(int start, int end)
        {
            Start = start;
            End = end;

            for (int i = start; i <= end; i++)
            {
                dropDownOptions.Add(i.ToString());
                dropDownDisplayOptions.Add(i.ToString());
            }

            bounds.Width = 112;
            RecalculateBounds();
        }

        public override void RecalculateBounds()
        {
            base.RecalculateBounds();
            dropDownBounds.Y += bounds.Height;

            if (Wrap == WrapStyle.Horizontal)
            {
                dropDownBounds.Width *= Math.Min(dropDownOptions.Count, WrapLimit);
                dropDownBounds.Height = bounds.Height * (dropDownOptions.Count / WrapLimit);
            }
            else if (Wrap == WrapStyle.Vertical)
            {
                dropDownBounds.Width *= dropDownOptions.Count / WrapLimit;
                dropDownBounds.Height = bounds.Height * Math.Min(dropDownOptions.Count, WrapLimit);
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (!greyedOut)
            {
                Clicked = true;

                if (!Game1.options.SnappyMenus)
                {
                    int selectedX = (x - dropDownBounds.X) / (bounds.Width - 48);
                    int selectedY = (y - dropDownBounds.Y) / bounds.Height;

                    if (Wrap == WrapStyle.Horizontal)
                    {
                        selectedX = MathHelper.Clamp(selectedX, 0, WrapLimit - 1);
                        selectedY = MathHelper.Clamp(selectedY, 0, (dropDownOptions.Count - 1) / WrapLimit);
                        selectedOption = selectedX + selectedY * WrapLimit;
                    }
                    else if (Wrap == WrapStyle.Vertical)
                    {
                        selectedX = MathHelper.Clamp(selectedX, 0, (dropDownOptions.Count - 1) / WrapLimit);
                        selectedY = MathHelper.Clamp(selectedY, 0, WrapLimit - 1);
                        selectedOption = selectedY + selectedX * WrapLimit;
                    }
                    else
                    {
                        selectedOption = MathHelper.Clamp(selectedY, 0, dropDownOptions.Count - 1);
                    }
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!Game1.options.SnappyMenus || greyedOut)
            {
                return;
            }
            else if (Wrap == WrapStyle.None)
            {
                base.receiveKeyPress(key);
                return;
            }

            int rowIncrement = (Wrap == WrapStyle.Horizontal) ? WrapLimit : 1;
            int colIncrement = (Wrap == WrapStyle.Horizontal) ? 1 : WrapLimit;

            if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
            {
                selectedOption -= rowIncrement;
            }
            else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
            {
                selectedOption += rowIncrement;
            }
            else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
            {
                selectedOption += colIncrement;
            }
            else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
            {
                selectedOption -= colIncrement;
            }

            selectedOption = MathHelper.Clamp(selectedOption, 0, dropDownOptions.Count - 1);
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu? context = null)
        {
            int labelX = slotX + bounds.Right + 8 + (int)labelOffset.X;
            int labelY = slotY + bounds.Y + (int)labelOffset.Y;
            float scale = greyedOut ? 0.33f : 1f;
            bool clicked = Clicked;
            Vector2 buttonPosition = new Vector2(slotX + bounds.Right - 48, slotY + bounds.Y);

            Utility.drawTextWithShadow(b, label, Game1.dialogueFont, new Vector2(labelX, labelY), Game1.textColor * scale, 1f, 0.1f);

            IClickableMenu.drawTextureBox(b,
                Game1.mouseCursors,
                dropDownBGSource,
                slotX + bounds.X,
                slotY + bounds.Y,
                bounds.Width - 48,
                bounds.Height,
                Color.White * scale,
                4f,
                drawShadow: false);

            if (clicked)
            {
                Rectangle optionBounds = new Rectangle(slotX + dropDownBounds.X, slotY + dropDownBounds.Y, bounds.Width - 48, bounds.Height);
                int row, col;

                IClickableMenu.drawTextureBox(b,
                    Game1.mouseCursors,
                    dropDownBGSource,
                    slotX + dropDownBounds.X,
                    slotY + dropDownBounds.Y,
                    dropDownBounds.Width,
                    dropDownBounds.Height,
                    Color.White * scale,
                    4f,
                    drawShadow: false);

                for (int i = 0; i < dropDownDisplayOptions.Count; i++)
                {
                    if (Wrap == WrapStyle.Horizontal)
                    {
                        row = i / WrapLimit;
                        col = i % WrapLimit;
                    }
                    else if (Wrap == WrapStyle.Vertical)
                    {
                        row = i % WrapLimit;
                        col = i / WrapLimit;
                    }
                    else
                    {
                        row = i;
                        col = 0;
                    }

                    optionBounds.X = slotX + dropDownBounds.X + col * (bounds.Width - 48);
                    optionBounds.Y = slotY + dropDownBounds.Y + row * bounds.Height;

                    if (i == selectedOption)
                    {
                        b.Draw(Game1.staminaRect, optionBounds, Color.Wheat);
                    }

                    b.DrawString(Game1.smallFont,
                        dropDownDisplayOptions[i],
                        new Vector2(optionBounds.X + 4, optionBounds.Y + 8),
                        Game1.textColor * scale,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0.98f);
                }
            }
            
            if (selectedOption < dropDownDisplayOptions.Count && selectedOption >= 0)
            {
                b.DrawString(Game1.smallFont,
                    dropDownDisplayOptions[selectedOption],
                    new Vector2(slotX + bounds.X + 4, slotY + bounds.Y + 8),
                    Game1.textColor * scale,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.88f);
            }

            b.Draw(Game1.mouseCursors, buttonPosition, dropDownButtonSource, Color.White * scale, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
        }
    }
}
