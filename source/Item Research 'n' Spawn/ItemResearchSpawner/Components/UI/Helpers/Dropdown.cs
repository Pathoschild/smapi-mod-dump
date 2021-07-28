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
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components.UI.Helpers
{
    /**
    This code is copied from Pathoschild.Stardew.Common.UI in https://github.com/Pathoschild/StardewMods,
    available under the MIT License. See that repository for the latest version.
    **/
    internal class Dropdown<TItem> : ClickableComponent
    {
        private readonly SpriteFont _font;

        private readonly DropdownList<TItem> _list;

        private readonly int _labelWidth;

        private bool _isExpanded;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                downNeighborID = value
                    ? _list.TopComponentId
                    : DefaultDownNeighborId;
            }
        }

        public TItem Selected => _list.SelectedValue;

        public int DefaultDownNeighborId { get; set; } = -99999;

        public Dropdown(int x, int y, SpriteFont font, TItem selectedItem, TItem[] items, Func<TItem, string> getLabel)
            : base(Rectangle.Empty, getLabel(selectedItem))
        {
            _font = font;

            _list = new DropdownList<TItem>(selectedItem, items, getLabel, x, y, font);

            bounds.X = x;
            bounds.Y = y;

            _list.bounds.X = bounds.X + UIConstants.BorderWidth;
            _list.bounds.Y = bounds.Y + UIConstants.BorderWidth;

            _list.ReinitializeComponents();

            _labelWidth = RenderHelpers.GetLabelWidth(_font);

            bounds.Height = (int) _font.MeasureString("ABOBA").Y + UIConstants.BorderWidth * 2;
            bounds.Width = _labelWidth + UIConstants.BorderWidth * 2;

            _list.ReinitializeControllerFlow();
            IsExpanded = IsExpanded;
        }

        public override bool containsPoint(int x, int y)
        {
            return base.containsPoint(x, y) || IsExpanded && _list.containsPoint(x, y);
        }

        public bool TryClick(int x, int y, out bool itemClicked, out bool dropdownToggled)
        {
            itemClicked = false;
            dropdownToggled = false;

            if (IsExpanded && _list.TryClick(x, y, out itemClicked))
            {
                if (itemClicked)
                {
                    dropdownToggled = true;
                }

                return true;
            }

            if (bounds.Contains(x, y) || IsExpanded)
            {
                dropdownToggled = true;

                return true;
            }

            return false;
        }

        public bool TrySelect(TItem value)
        {
            return _list.TrySelect(value);
        }

        public void ReceiveScrollWheelAction(int direction)
        {
            if (IsExpanded)
            {
                _list.ReceiveScrollWheelAction(direction);
            }
        }

        public void Draw(SpriteBatch sprites, float opacity = 1)
        {
            RenderHelpers.DrawMenuBox(bounds.X, bounds.Y, bounds.Width - UIConstants.BorderWidth * 2,
                _list.MaxLabelHeight, out var textPos);

            sprites.DrawString(_font, RenderHelpers.TruncateString(_list.SelectedLabel, _font, _labelWidth), textPos,
                Color.Black * opacity);

            if (IsExpanded)
            {
                _list.Draw(sprites, opacity);
            }
        }
    }
}