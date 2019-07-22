using System;
using System.Linq;
using MegaStorage.Models;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MegaStorage.UI
{
    public class MagicItemGrabMenu : LargeItemGrabMenu
    {
        private int _currentRow;
        private int _maxRow;

        public MagicItemGrabMenu(CustomChest customChest) : base(customChest)
        {
            _currentRow = 0;
            Refresh();
        }

        public override void draw(SpriteBatch b)
        {
            Draw(b);
            if (_currentRow < _maxRow)
            {
                DownArrow.draw(b);
            }
            if (_currentRow > 0)
            {
                UpArrow.draw(b);
            }
            drawMouse(b);
        }

        public override void Refresh()
        {
            MegaStorageMod.Instance.Monitor.VerboseLog("Category: " + SelectedCategory.name);
            var filteredItems = SelectedCategory.Filter(CustomChest.items);
            ItemsToGrabMenu.actualInventory = filteredItems.Skip(ItemsPerRow * _currentRow).ToList();
            _maxRow = (filteredItems.Count - 1) / 12 + 1 - Rows;
            if (_currentRow > _maxRow)
                _currentRow = _maxRow;
        }

        protected override void ChangeCategory(ChestCategory cat)
        {
            SelectedCategory = cat;
            _currentRow = 0;
            Refresh();
        }

        public override void receiveScrollWheelAction(int direction)
        {
            MegaStorageMod.Instance.Monitor.VerboseLog("receiveScrollWheelAction");
            if (direction < 0 && _currentRow < _maxRow)
            {
                _currentRow++;
            }
            else if (direction > 0 && _currentRow > 0)
            {
                _currentRow--;
            }
            Refresh();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, true);
            if (UpArrow.containsPoint(x, y) && _currentRow > 0)
            {
                Game1.playSound("coin");
                _currentRow--;
                UpArrow.scale = UpArrow.baseScale;
            }
            if (DownArrow.containsPoint(x, y) && _currentRow < _maxRow)
            {
                Game1.playSound("coin");
                _currentRow++;
                DownArrow.scale = DownArrow.baseScale;
            }
            Refresh();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            UpArrow.scale = UpArrow.containsPoint(x, y) ? Math.Min(UpArrow.scale + 0.02f, UpArrow.baseScale + 0.1f) : Math.Max(UpArrow.scale - 0.02f, UpArrow.baseScale);
            DownArrow.scale = DownArrow.containsPoint(x, y) ? Math.Min(DownArrow.scale + 0.02f, DownArrow.baseScale + 0.1f) : Math.Max(DownArrow.scale - 0.02f, DownArrow.baseScale);
        }

    }
}