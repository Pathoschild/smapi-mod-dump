/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockChests.Utilities
{
    /// <summary>
    /// Based on the dropdown by spacechase0 => https://github.com/spacechase0/StardewValleyMods
    /// </summary>
    internal class Dropdown<T> : ClickableComponent
    {
        private int _x;
        private int _y;
        private int _width;
        private int _height;
        private T _selected;
        private Dictionary<T, string> _options;
        private bool _isOpen;
        private int hoveredIndex;
        private int selectedIndex;

        public readonly Rectangle ButtonSourceRect = OptionsDropDown.dropDownButtonSource;
        public readonly Rectangle BackgroundSourceRect = OptionsDropDown.dropDownBGSource;

        public event EventHandler SelectionChanged;

        public int X
        {
            get => _x;
            set => _x = value;
        }
        public int Y
        {
            get => _y;
            set => _y = value;
        }
        public int Width
        {
            get => _width;
            set => _width = value;
        }
        public int Height
        {
            get => _height;
            set => _height = value;
        }
        public T Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                selectedIndex = Options.Keys.ToList().IndexOf(value);
                SelectionChanged?.Invoke(this, new());
            }
        }
        public Dictionary<T, string> Options
        {
            get => _options;
            set => _options = value;
        }
        public bool IsOpen
        {
            get => _isOpen;
            set => _isOpen = value;
        }

        public int SelectedIndex => selectedIndex;

        public int HoveredIndex => hoveredIndex;

        public new Rectangle bounds
        {
            get => Bounds;
            set
            {
                X = value.X;
                Y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }
        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

        public Dropdown(T selected, Dictionary<T, string> options, Rectangle bounds) : this(selected, options, bounds.X, bounds.Y, bounds.Width, bounds.Height) { }

        public Dropdown(T selected, Dictionary<T, string> options, int x, int y, int width, int height, int myId = -1) : base (new Rectangle(x, y, width, height), "")
        {
            Options = options;
            Selected = selected;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            IsOpen = false;
            selectedIndex = Options.Keys.ToList().IndexOf(selected);
            myID = myId;
        }

        public bool tryHover(int x, int y)
        {
            hoveredIndex = -1;
            if (IsOpen)
            {
                for (int i = 0; i < Options.Count; i++)
                {
                    var bounds = Bounds;
                    bounds.Y += Height + (i * Height);
                    if (bounds.Contains(x, y))
                    {
                        hoveredIndex = i;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool tryLeftClick(int x, int y, bool playSound = true)
        {
            if (Bounds.Contains(x, y))
            {
                IsOpen = !IsOpen;
                Game1.playSound("shwip");
                return true;
            }

            if (IsOpen)
            {
                for (int i = 0; i < Options.Count; i++)
                {
                    var bounds = Bounds;
                    bounds.Y += Height + (i * Height);
                    if (bounds.Contains(x, y))
                    {
                        Selected = Options.Keys.ToList()[i];
                        IsOpen = !IsOpen;
                        Game1.playSound("smallSelect");
                        return true;
                    }
                }
            }

            return false;
        }

        public void Draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, BackgroundSourceRect, X, Y, Width - 48, Height, Color.White, 4f, false);
            b.DrawString(Game1.smallFont, Options[Selected], new Vector2(X + 4, Y + 8), Game1.textColor);
            b.Draw(Game1.mouseCursors, new Vector2(X + Width - 48, Y), ButtonSourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0);

            if (IsOpen)
            {
                int dropdownHeight = Options.Count * Height;
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, BackgroundSourceRect, X, Y + Height, Width - 48, dropdownHeight, Color.White, 4f, false, 999f);
                for (int i = 0; i < Options.Count; i++)
                {
                    if (i == selectedIndex || i == hoveredIndex)
                        b.Draw(Game1.staminaRect, new Rectangle(X + 4, Y + Height + (i * Height) + (i == 0 ? 4 : 0), Width - 48 - 8, Height - (i == 0 || i == Options.Count - 1 ? 4 : 0)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);
                    b.DrawString(Game1.smallFont, Options.Values.ToList()[i], new Vector2(X + 4, Y + Height + (i * Height) + 8), Game1.textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
            }
        }
    }
}
