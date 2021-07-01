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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components
{
    /**
        MIT License

        Copyright (c) 2018 CJBok

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
     **/
    internal class ItemSearchBarTab
    {
        private TextBox _searchBox;
        
        private readonly ClickableComponent _searchBoxArea;
        private readonly ClickableTextureComponent _searchIcon;

        private float _iconOpacity;
        private bool _persistFocus;

        public ItemSearchBarTab(IContentHelper content, IMonitor monitor, int x, int y, int width)
        {
            ModManager.Instance.SearchText = "";
            _iconOpacity = 1f;

            _searchBoxArea =
                new ClickableComponent(
                    new Rectangle(x, y, width + UIConstants.BorderWidth, 36 + UIConstants.BorderWidth - 2), "");

            _searchBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont,
                Game1.textColor)
            {
                X = _searchBoxArea.bounds.X,
                Y = _searchBoxArea.bounds.Y + 5,
                Height = 0,
                Width = _searchBoxArea.bounds.Width,
                Text = ModManager.Instance.SearchText
            };

            var iconRect = new Rectangle(80, 0, 13, 13);
            const float iconScale = 2.5f;

            var iconBounds = new Rectangle((int) (_searchBoxArea.bounds.Right - iconRect.Width * iconScale),
                (int) (_searchBoxArea.bounds.Center.Y  + UIConstants.BorderWidth / 2 - iconRect.Height / 2f * iconScale + 2),
                (int) (iconRect.Width * iconScale), (int) (iconRect.Height * iconScale)
            );

            _searchIcon = new ClickableTextureComponent(iconBounds, Game1.mouseCursors, iconRect, iconScale);
        }

        public void Focus(bool persist)
        {
            _searchBox.Selected = true;
            _persistFocus = persist;
        }

        public void Blur()
        {
            _searchBox.Selected = false;
            _persistFocus = false;
        }

        public bool Selected => _searchBox.Selected;
        public bool PersistFocus => _persistFocus;
        public bool IsSearchBoxSelectionChanging => _iconOpacity > 0 && _iconOpacity < 1;

        public Rectangle Bounds => _searchBoxArea.bounds;

        public void Draw(SpriteBatch spriteBatch)
        {
            RenderHelpers.DrawMenuBox(_searchBoxArea.bounds.X, _searchBoxArea.bounds.Y,
                _searchBoxArea.bounds.Width - UIConstants.BorderWidth,
                _searchBoxArea.bounds.Height - UIConstants.BorderWidth, out _);

            _searchBox.Draw(spriteBatch);

            spriteBatch.Draw(_searchIcon.texture, _searchIcon.bounds, _searchIcon.sourceRect,
                Color.White * _iconOpacity);
        }

        public void Update(GameTime time)
        {
            if (_persistFocus && !_searchBox.Selected)
            {
                Blur();
            }

            if (ModManager.Instance.SearchText != _searchBox.Text.Trim())
            {
                ModManager.Instance.SearchText = _searchBox.Text.Trim();
            }


            var delta = 1.5f / time.ElapsedGameTime.Milliseconds;

            if (!_searchBox.Selected && _iconOpacity < 1f)
            {
                _iconOpacity = Math.Min(1f, _iconOpacity + delta);
            }
            else if (_searchBox.Selected && _iconOpacity > 0f)
            {
                _iconOpacity = Math.Max(0f, _iconOpacity - delta);
            }
        }

        public void Clear()
        {
            _searchBox.Text = "";
        }

        public void SetText(string text)
        {
            _searchBox.Text = text;
        }
    }
}