/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

using static StardewValley.Menus.OptionsDropDown;

namespace DeluxeJournal.Menus.Components
{
    public class DropDownComponent : ClickableComponent
    {
        public enum WrapStyle
        {
            None,
            Horizontal,
            Vertical
        }

        public const int ButtonWidth = 40;
        public const int TextBufferWidth = 16;

        private readonly List<string> _options;
        private readonly List<string> _displayNames;
        private readonly bool _fixedWidth;

        private Rectangle _optionsBounds;
        private int _selectedOption;
        private int _selectedOptionOld;
        private bool _clicked;
        private bool _active;

        /// <summary>List of underlying option values in the drop down.</summary>
        public IReadOnlyList<string> Options => _options;

        /// <summary>The wrap style of the options grid. <see cref="WrapStyle.None"/> is always drawn as a single column.</summary>
        public WrapStyle Wrap { get; }

        /// <summary>The number of options to draw in sequence before wrapping.</summary>
        public int WrapLimit { get; }

        /// <summary>Index of the option selected from the <see cref="Options"/> list.</summary>
        public int SelectedOption
        {
            get => _selectedOption;

            set
            {
                _selectedOption = Math.Max(Math.Min(value, _options.Count - 1), 0);
            }
        }

        /// <summary>Whether this component is active (interactable).</summary>
        public bool Active
        {
            get => _active && visible;
            set => _active = value;
        }

        public DropDownComponent(IEnumerable<string> options, Rectangle bounds, string name, bool fixedWidth = false)
            : this(options, options, bounds, name, WrapStyle.None, 1, fixedWidth)
        {
        }

        public DropDownComponent(IEnumerable<string> options, IEnumerable<string> displayNames, Rectangle bounds, string name, bool fixedWidth = false)
            : this(options, displayNames, bounds, name, WrapStyle.None, 1, fixedWidth)
        {
        }

        public DropDownComponent(IEnumerable<string> options, Rectangle bounds, string name, WrapStyle wrap, int wrapLimit, bool fixedWidth = false)
            : this(options, options, bounds, name, wrap, wrapLimit, fixedWidth)
        {
        }

        public DropDownComponent(IEnumerable<string> options, IEnumerable<string> displayNames, Rectangle bounds, string name, WrapStyle wrap, int wrapLimit, bool fixedWidth = false)
            : base(bounds, name)
        {
            _options = options.ToList();
            _displayNames = displayNames.ToList();
            _fixedWidth = fixedWidth;
            Wrap = wrap;
            WrapLimit = wrap == WrapStyle.None ? 1 : wrapLimit;
            Active = true;

            if (_options.Count != _displayNames.Count)
            {
                throw new ArgumentException($"{nameof(DropDownComponent)} must have an equal number of options and corresponding display names.");
            }
            else if (_options.Count <= 0)
            {
                throw new ArgumentException($"{nameof(DropDownComponent)} must have at least 1 option.");
            }

            RecalculateBounds();
        }

        public virtual void RecalculateBounds()
        {
            if (!_fixedWidth)
            {
                foreach (string name in _displayNames)
                {
                    float textWidth = Game1.smallFont.MeasureString(name).X;

                    if (textWidth >= bounds.Width - ButtonWidth)
                    {
                        bounds.Width = (int)textWidth + ButtonWidth + TextBufferWidth;
                    }
                }
            }

            _optionsBounds = Wrap switch
            {
                WrapStyle.Horizontal => new(
                    bounds.X,
                    bounds.Y + bounds.Height,
                    (bounds.Width - ButtonWidth) * Math.Min(_options.Count, WrapLimit),
                    bounds.Height * ((_options.Count + WrapLimit - 1) / WrapLimit)),

                WrapStyle.Vertical => new(
                    bounds.X,
                    bounds.Y + bounds.Height,
                    (bounds.Width - ButtonWidth) * ((_options.Count + WrapLimit - 1) / WrapLimit),
                    bounds.Height * Math.Min(_options.Count, WrapLimit)),

                _ => new(bounds.X, bounds.Y + bounds.Height, bounds.Width - ButtonWidth, bounds.Height * _options.Count)
            };
        }

        public virtual void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!Active)
            {
                return;
            }
            else if (playSound && !_clicked)
            {
                Game1.playSound("shwip");
            }

            _selectedOptionOld = SelectedOption;
            _optionsBounds.Y = Math.Min(_optionsBounds.Y, Game1.uiViewport.Height - _optionsBounds.Height);
            LeftClickHeld(x, y);
        }

        public virtual void LeftClickHeld(int x, int y)
        {
            if (!Active)
            {
                return;
            }

            _clicked = true;

            if (_optionsBounds.Contains(x, y) && !Game1.options.SnappyMenus)
            {
                int selectedX = (x - _optionsBounds.X) / (bounds.Width - ButtonWidth);
                int selectedY = (y - _optionsBounds.Y) / bounds.Height;

                SelectedOption = Wrap switch
                {
                    WrapStyle.Horizontal => selectedX + selectedY * WrapLimit,
                    WrapStyle.Vertical => selectedY + selectedX * WrapLimit,
                    _ => selectedY
                };
            }
        }

        public virtual void LeftClickReleased(int x, int y)
        {
            if (!Active)
            {
                return;
            }
            else if (_clicked)
            {
                Game1.playSound("drumkit6");
            }

            if (!_optionsBounds.Contains(x, y) && !Game1.options.SnappyMenus)
            {
                SelectedOption = _selectedOptionOld;
            }

            _clicked = false;
        }

        public virtual void ReceiveKeyPress(Keys key)
        {
            if (!Game1.options.SnappyMenus || !Active || !_clicked)
            {
                return;
            }

            int rowIncrement = Wrap == WrapStyle.Horizontal ? WrapLimit : 1;
            int colIncrement = Wrap == WrapStyle.Horizontal ? 1 : WrapLimit;

            if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
            {
                SelectedOption -= rowIncrement;
            }
            else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
            {
                SelectedOption += rowIncrement;
            }
            else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
            {
                SelectedOption -= colIncrement;
            }
            else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
            {
                SelectedOption += colIncrement;
            }
            else
            {
                return;
            }

            Game1.playSound("shiny4");
        }

        public virtual void Draw(SpriteBatch b)
        {
            if (!visible)
            {
                return;
            }

            Rectangle selectedBounds = new(bounds.X, bounds.Y, bounds.Width - ButtonWidth, bounds.Height);
            DrawBackground(b, selectedBounds, false);

            b.Draw(Game1.mouseCursors,
                new Rectangle(bounds.Right - ButtonWidth - 4, bounds.Y, ButtonWidth, bounds.Height),
                dropDownButtonSource,
                Active ? Color.White : Color.DimGray);

            if (_clicked)
            {
                Rectangle optionBounds = new(_optionsBounds.X, _optionsBounds.Y, bounds.Width - ButtonWidth, bounds.Height);

                DrawBackground(b, _optionsBounds, true);

                for (int i = 0, row, col; i < _options.Count; i++)
                {
                    row = Wrap != WrapStyle.Vertical ? i / WrapLimit : i % WrapLimit;
                    col = Wrap != WrapStyle.Vertical ? i % WrapLimit : i / WrapLimit;
                    optionBounds.X = _optionsBounds.X + col * (bounds.Width - ButtonWidth);
                    optionBounds.Y = _optionsBounds.Y + row * bounds.Height;

                    if (i == SelectedOption)
                    {
                        b.Draw(Game1.staminaRect, optionBounds, Color.Wheat);
                    }

                    DrawOption(b, optionBounds, i, 0.98f);
                }
            }

            if (_displayNames.Count > 0)
            {
                DrawOption(b, selectedBounds, SelectedOption, 0.88f);
            }
        }

        protected virtual void DrawBackground(SpriteBatch b, Rectangle bgBounds, bool dropDown)
        {
            IClickableMenu.drawTextureBox(b,
                Game1.mouseCursors,
                dropDownBGSource,
                bgBounds.X, bgBounds.Y, bgBounds.Width, bgBounds.Height,
                Active ? Color.White : Color.DimGray,
                4f, false);
        }

        protected virtual void DrawOption(SpriteBatch b, Rectangle optionBounds, int whichOption, float layerDepth)
        {
            b.DrawString(Game1.smallFont,
                _displayNames[whichOption],
                new(optionBounds.X + 4f, optionBounds.Y + 8f),
                Game1.textColor,
                0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth);
        }
    }
}
