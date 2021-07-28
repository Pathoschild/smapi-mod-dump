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
using ItemResearchSpawner.Components.UI.Helpers;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

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
    internal class ItemCategorySelectorTab
    {
        private readonly IMonitor _monitor;
        private readonly Dropdown<string> _categoryDropdown;

        private readonly string[] _availableCategories;

        public delegate void DropdownToggle(bool expanded);

        public event DropdownToggle OnDropdownToggle;

        public ItemCategorySelectorTab(IContentHelper content, IMonitor monitor, SpawnableItem[] spawnableItems, int x,
            int y)
        {
            _monitor = monitor;
            _availableCategories = GetDisplayCategories(spawnableItems).ToArray();

            _categoryDropdown = new Dropdown<string>(x, y, Game1.smallFont, _categoryDropdown?.Selected ?? I18n.Category_All(),
                _availableCategories, p => p);

            _categoryDropdown.IsExpanded = false;
        }

        public int Right => Bounds.Left + Bounds.Width - 5 + 4 * Game1.pixelZoom + CursorSprites.DropdownButton.Width;

        public Rectangle Bounds => _categoryDropdown.bounds;
        public int MyID => _categoryDropdown.myID;
        public string SelectedCategory => _categoryDropdown.Selected;
        public bool IsExpanded => _categoryDropdown.IsExpanded;

        public bool TryClick(int x, int y)
        {
            if (_categoryDropdown.TryClick(x, y, out var itemClicked, out var dropdownToggled))
            {
                if (dropdownToggled)
                {
                    _categoryDropdown.IsExpanded = !_categoryDropdown.IsExpanded;
                    OnDropdownToggle?.Invoke(_categoryDropdown.IsExpanded);
                }

                if (itemClicked)
                {
                    var category = _categoryDropdown.Selected;

                    if (!_categoryDropdown.TrySelect(category))
                    {
                        _monitor.Log($"Failed selecting category filter category '{category}'.", LogLevel.Warn);
                    }

                    ModManager.Instance.Category = category;
                }

                return true;
            }

            return false;
        }

        public void Close()
        {
            _categoryDropdown.IsExpanded = false;
            OnDropdownToggle?.Invoke(_categoryDropdown.IsExpanded);
        }
 

        public void HandleScroll(int direction)
        {
            if (_categoryDropdown.IsExpanded)
            {
                _categoryDropdown.ReceiveScrollWheelAction(direction);
            }
            else
            {
                NextCategory((int) MathHelper.Clamp(direction, -1, 1));
            }
        }

        public void NextCategory(int direction)
        {
            direction = direction < 0 ? -1 : 1;

            var last = _availableCategories.Length - 1;

            var index = Array.IndexOf(_availableCategories, _categoryDropdown.Selected) + direction;

            if (index < 0)
            {
                index = last;
            }

            if (index > last)
            {
                index = 0;
            }

            _categoryDropdown.TrySelect(_availableCategories[index]);
            ModManager.Instance.Category = _availableCategories[index];
        }

        public void ResetCategory()
        {
            _categoryDropdown.TrySelect(I18n.Category_All());
            ModManager.Instance.Category = I18n.Category_All();
        }

        public void SelectCategory(string category)
        {
            if (!_categoryDropdown.TrySelect(category))
            {
                _monitor.LogOnce($"Failed selecting category '{category}'.", LogLevel.Warn);
                
                category = I18n.Category_All();
                _categoryDropdown.TrySelect(category);
            }

            ModManager.Instance.Category = category;
        }

        public void ReceiveScrollWheelAction(int direction)
        {
            _categoryDropdown.ReceiveScrollWheelAction(direction);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var sourceRect = CursorSprites.DropdownButton;

            var position = new Vector2(
                x: _categoryDropdown.bounds.X + _categoryDropdown.bounds.Width - 5,
                y: _categoryDropdown.bounds.Y + 10
            );

            spriteBatch.Draw(Game1.mouseCursors, position, sourceRect, Color.White, 0, Vector2.Zero, Game1.pixelZoom,
                SpriteEffects.None, 1f);

            if (_categoryDropdown.IsExpanded)
            {
                spriteBatch.Draw(Game1.mouseCursors,
                    new Vector2(position.X + 2 * Game1.pixelZoom, position.Y + 3 * Game1.pixelZoom),
                    new Rectangle(sourceRect.X + 2, sourceRect.Y + 3, 5, 6), Color.White, 0, Vector2.Zero,
                    Game1.pixelZoom, SpriteEffects.FlipVertically, 1f);
            }

            _categoryDropdown.Draw(spriteBatch);
        }

        private IEnumerable<string> GetDisplayCategories(SpawnableItem[] items)
        {
            var all = I18n.Category_All();
            var misc = I18n.Category_Misc();
            
            var categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
            {
                if (item.Category.ToLower().Equals(all) || item.Category.ToLower().Equals(misc))
                {
                    continue;
                }

                categories.Add(item.Category);
            }

            yield return all;

            foreach (var category in categories.OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
            {
                yield return category;
            }

            yield return misc;
        }
    }
}