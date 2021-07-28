/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components.UI.Helpers
{
    /**
    This code is copied from Pathoschild.Stardew.Common.UI in https://github.com/Pathoschild/StardewMods,
    available under the MIT License. See that repository for the latest version.
    **/
    internal class DropdownList<TValue> : ClickableComponent
    {
        private const int DropdownPadding = 5;
        private DropListOption _selectedOption;
        private readonly DropListOption[] _options;
        
        private readonly int _labelWidth;

        private int _firstVisibleIndex;
        private int _maxItems;
        
        private int LastVisibleIndex => _firstVisibleIndex + _maxItems - 1;
        private int MaxFirstVisibleIndex => _options.Length - _maxItems;
        private bool CanScrollUp => _firstVisibleIndex > 0;
        private bool CanScrollDown => _firstVisibleIndex < MaxFirstVisibleIndex;

        private readonly SpriteFont _font;
        private ClickableTextureComponent _upArrow;
        private ClickableTextureComponent _downArrow;

        public TValue SelectedValue => _selectedOption.Value;
        public string SelectedLabel => _selectedOption.label;

        public int MaxLabelHeight { get; }
        public int MaxLabelWidth { get; private set; }
        public int TopComponentId => _options.First(p => p.visible).myID;


        public DropdownList(TValue selectedValue, TValue[] items, Func<TValue, string> getLabel, int x, int y,
            SpriteFont font)
            : base(new Rectangle(), nameof(DropdownList<TValue>))
        {
            _options = items
                .Select((i, index) => new DropListOption(Rectangle.Empty, index, getLabel(i), i, font))
                .ToArray();

            _font = font;
            _labelWidth = RenderHelpers.GetLabelWidth(_font);
            
            MaxLabelHeight = _options.Max(p => p.LabelHeight);

            var selectedIndex = Array.IndexOf(items, selectedValue);

            _selectedOption = selectedIndex >= 0 ? _options[selectedIndex] : _options.First();

            bounds.X = x;
            bounds.Y = y;

            ReinitializeComponents();
        }


        public void ReceiveScrollWheelAction(int direction)
        {
            Scroll(direction > 0 ? -1 : 1);
        }


        public bool TryClick(int x, int y, out bool itemClicked)
        {
            var option = _options.FirstOrDefault(p => p.visible && p.containsPoint(x, y));

            if (option != null)
            {
                _selectedOption = option;
                itemClicked = true;
                return true;
            }

            itemClicked = false;

            if (_upArrow.containsPoint(x, y))
            {
                Scroll(-1);
                return true;
            }

            if (_downArrow.containsPoint(x, y))
            {
                Scroll(1);
                return true;
            }

            return false;
        }


        public bool TrySelect(TValue value)
        {
            var entry = _options.FirstOrDefault(p =>
                p.Value == null && value == null || p.Value?.Equals(value) == true
            );

            if (entry == null)
            {
                return false;
            }


            _selectedOption = entry;
            return true;
        }


        public override bool containsPoint(int x, int y)
        {
            return base.containsPoint(x, y) || _upArrow.containsPoint(x, y) || _downArrow.containsPoint(x, y);
        }


        public void Draw(SpriteBatch sprites, float opacity = 1)
        {
            foreach (var option in _options)
            {
                if (!option.visible)
                {
                    continue;
                }

                if (option.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                {
                    sprites.Draw(CursorSprites.SpriteMap, option.bounds, CursorSprites.HoverBackground,
                        Color.White * opacity);
                }
                else if (option.Index == _selectedOption.Index)
                {
                    sprites.Draw(CursorSprites.SpriteMap, option.bounds, CursorSprites.ActiveBackground,
                        Color.White * opacity);
                }
                else
                {
                    sprites.Draw(CursorSprites.SpriteMap, option.bounds, CursorSprites.InactiveBackground,
                        Color.White * opacity);
                }

                var position =
                    new Vector2(option.bounds.X + DropdownPadding, option.bounds.Y + Game1.tileSize / 16);

                sprites.DrawString(_font, RenderHelpers.TruncateString(option.label, _font, _labelWidth), position,
                    Color.Black * opacity);
            }

            if (CanScrollUp)
            {
                _upArrow.draw(sprites, Color.White * opacity, 1);
            }

            if (CanScrollDown)
            {
                _downArrow.draw(sprites, Color.White * opacity, 1);
            }
        }

        public void ReinitializeComponents()
        {
            var x = bounds.X;
            var y = bounds.Y;

            var itemWidth = MaxLabelWidth =
                Math.Max(_options.Max(p => p.LabelWidth), Game1.tileSize * 2) + DropdownPadding * 2;
            var itemHeight = MaxLabelHeight;

            _maxItems = Math.Min((Game1.uiViewport.Height - y) / itemHeight, _options.Length);
            _firstVisibleIndex = GetValidFirstItem(_firstVisibleIndex, MaxFirstVisibleIndex);

            bounds.Width = itemWidth;
            bounds.Height = itemHeight * _maxItems;

            var itemY = y;

            foreach (var option in _options)
            {
                option.visible = option.Index >= _firstVisibleIndex && option.Index <= LastVisibleIndex;

                if (option.visible)
                {
                    option.bounds = new Rectangle(x, itemY, itemWidth, itemHeight);
                    itemY += itemHeight;
                }
            }

            var upSource = CursorSprites.UpArrow;
            var downSource = CursorSprites.DownArrow;

            _upArrow = new ClickableTextureComponent("up-arrow",
                new Rectangle(x - upSource.Width, y, upSource.Width, upSource.Height), "", "",
                CursorSprites.SpriteMap, upSource, 1);

            _downArrow = new ClickableTextureComponent("down-arrow",
                new Rectangle(x - downSource.Width, y + bounds.Height - downSource.Height, downSource.Width,
                    downSource.Height), "", "", CursorSprites.SpriteMap, downSource, 1);

            ReinitializeControllerFlow();
        }

        public void ReinitializeControllerFlow()
        {
            var firstIndex = _firstVisibleIndex;
            var lastIndex = LastVisibleIndex;

            const int initialId = 1_100_000;

            foreach (var option in _options)
            {
                var index = option.Index;
                var id = initialId + index;

                option.myID = id;
                option.upNeighborID = index > firstIndex ? id - 1 : -99999;
                option.downNeighborID = index < lastIndex ? id + 1 : -1;
            }
        }

        public IEnumerable<ClickableComponent> GetChildComponents()
        {
            return _options;
        }


        private void Scroll(int amount)
        {
            var firstItem = GetValidFirstItem(_firstVisibleIndex + amount, MaxFirstVisibleIndex);

            if (firstItem == _firstVisibleIndex)
            {
                return;
            }

            _firstVisibleIndex = firstItem;

            ReinitializeComponents();
        }


        private static int GetValidFirstItem(int value, int maxIndex)
        {
            return Math.Max(Math.Min(value, maxIndex), 0);
        }


        private class DropListOption : ClickableComponent
        {
            public int Index { get; }
            public TValue Value { get; }
            public int LabelWidth { get; }
            public int LabelHeight { get; }

            public DropListOption(Rectangle bounds, int index, string label, TValue value, SpriteFont font)
                : base(bounds, index.ToString(), label)
            {
                Index = index;
                Value = value;

                var labelSize = font.MeasureString(label);

                LabelWidth = RenderHelpers.GetLabelWidth(font) - 10;
                LabelHeight = (int) labelSize.Y;
            }
        }
    }
}