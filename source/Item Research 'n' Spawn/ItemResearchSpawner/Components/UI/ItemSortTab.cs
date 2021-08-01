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
using System.IO;
using System.Linq;
using ItemResearchSpawner.Models.Enums;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components.UI
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
    internal class ItemSortTab
    {
        private readonly Texture2D _sortTexture;

        private readonly ClickableComponent _sortButton;
        private readonly ClickableTextureComponent _sortIcon;

        public ItemSortTab(IContentHelper content, IMonitor monitor, int x, int y)
        {
            // _sortTexture = content.Load<Texture2D>("assets/images/sort-icon.png");
            _sortTexture = content.Load<Texture2D>(Path.Combine("assets", "images", "sort-icon.png"));
            
            _sortButton =
                new ClickableComponent(
                    new Rectangle(x, y,
                        GetMaxSortLabelWidth(Game1.smallFont) + _sortTexture.Width * Game1.pixelZoom +
                        5 + UIConstants.BorderWidth, Game1.tileSize), GetSortLabel(ModManager.Instance.SortOption));

            _sortIcon = new ClickableTextureComponent(
                new Rectangle(_sortButton.bounds.X + UIConstants.BorderWidth,
                    y + UIConstants.BorderWidth, _sortTexture.Width, Game1.tileSize), _sortTexture,
                new Rectangle(0, 0, _sortTexture.Width, _sortTexture.Height), Game1.pixelZoom);
        }

        public Rectangle Bounds => _sortButton.bounds;

        public void HandleLeftClick()
        {
            ModManager.Instance.SortOption = ModManager.Instance.SortOption.GetNext();
            _sortButton.label = _sortButton.name = GetSortLabel(ModManager.Instance.SortOption);
        }

        public void HandleRightClick()
        {
            ModManager.Instance.SortOption = ModManager.Instance.SortOption.GetPrevious();
            _sortButton.label = _sortButton.name = GetSortLabel(ModManager.Instance.SortOption);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            RenderHelpers.DrawTextMenuBox(_sortButton.bounds.X, _sortButton.bounds.Y, Game1.smallFont,
                _sortButton.name, _sortTexture.Width * Game1.pixelZoom + 5);
            _sortIcon.draw(spriteBatch);
        }

        private int GetMaxSortLabelWidth(SpriteFont font)
        {
            return
                (
                    from ItemSortOption key in Enum.GetValues(typeof(ItemSortOption))
                    let text = GetSortLabel(key)
                    select (int) font.MeasureString(text).X
                )
                .Max();
        }

        private string GetSortLabel(ItemSortOption sort)
        {
            return sort switch
            {
                ItemSortOption.Name => I18n.Sort_ByName(),
                ItemSortOption.Category => I18n.Sort_ByCategory(),
                ItemSortOption.ID => I18n.Sort_ById(),
                _ => throw new NotSupportedException($"Invalid sort type {sort}.")
            };
        }
    }
}