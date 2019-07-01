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
                DownButton.draw(b);
            }
            if (_currentRow > 0)
            {
                UpButton.draw(b);
            }
            drawMouse(b);
        }

        public override void Refresh()
        {
            ItemsToGrabMenu.actualInventory = CustomChest.items.Skip(ItemsPerRow * _currentRow).ToList();
            _maxRow = (CustomChest.items.Count - 1) / 12 + 1 - Rows;
            if (_currentRow > _maxRow)
                _currentRow = _maxRow;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            MegaStorageMod.Logger.VerboseLog("receiveScrollWheelAction");
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
            if (UpButton.containsPoint(x, y) && _currentRow > 0)
            {
                Game1.playSound("coin");
                _currentRow--;
                UpButton.scale = UpButton.baseScale;
            }
            if (DownButton.containsPoint(x, y) && _currentRow < _maxRow)
            {
                Game1.playSound("coin");
                _currentRow++;
                DownButton.scale = DownButton.baseScale;
            }
            Refresh();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            UpButton.scale = UpButton.containsPoint(x, y) ? Math.Min(UpButton.scale + 0.02f, UpButton.baseScale + 0.1f) : Math.Max(UpButton.scale - 0.02f, UpButton.baseScale);
            DownButton.scale = DownButton.containsPoint(x, y) ? Math.Min(DownButton.scale + 0.02f, DownButton.baseScale + 0.1f) : Math.Max(DownButton.scale - 0.02f, DownButton.baseScale);
        }

        protected override void ClearNulls()
        {
            MegaStorageMod.Logger.VerboseLog("ClearNulls (Magic). CurrentRow: " + _currentRow);
            var skippedItems = CustomChest.items.Take(ItemsPerRow * _currentRow).ToList();
            var shownItems = ItemsToGrabMenu.actualInventory.ToList();
            MegaStorageMod.Logger.VerboseLog("Skipped items: " + skippedItems.Count);
            MegaStorageMod.Logger.VerboseLog("Shown items: " + shownItems.Count);
            CustomChest.items.Clear();
            CustomChest.items.AddRange(skippedItems);
            CustomChest.items.AddRange(shownItems);
            CustomChest.clearNulls();
            Refresh();
        }

    }
}