using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.Interface
{
    public class MagicItemGrabMenu : LargeItemGrabMenu
    {
        private protected List<ClickableComponent> CategoryComponents;
        private protected ClickableTextureComponent UpArrow;
        private protected ClickableTextureComponent DownArrow;

        private readonly List<ChestCategory> _chestCategories = new List<ChestCategory>();
        private ChestCategory _hoverCategory;
        private protected ChestCategory SelectedCategory;

        private int _currentRow;
        private int _maxRow;

        public MagicItemGrabMenu(CustomChest customChest) : base(customChest)
        {
            _currentRow = 0;

            UpArrow = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 840, yPositionOnScreen + 8, 64, 64), Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12), 1f)
            {
                myID = 88,
                downNeighborID = 89
            };
            DownArrow = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 840, yPositionOnScreen + 272, 64, 64),
                Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11), 1f)
            {
                myID = 89,
                upNeighborID = 88
            };

            SetupCategories();
            SetupControllerSupport();
            Refresh();
        }

        private void SetupCategories()
        {
            if (!ModConfig.Instance.EnableCategories)
            {
                SelectedCategory = new AllCategory(0, "All", xPositionOnScreen, yPositionOnScreen);
                return;
            }

            _chestCategories.AddRange(
                new List<ChestCategory>()
                {
                    new AllCategory(0,
                        "All",
                        xPositionOnScreen,
                        yPositionOnScreen),
                    new ChestCategory(1,
                        "Crops",
                        new Vector2(640, 80),
                        new[] {-81, -80, -79, -75},
                        xPositionOnScreen,
                        yPositionOnScreen),
                    new ChestCategory(2,
                        "Seeds",
                        new Vector2(656, 64),
                        new[] {-74, -19},
                        xPositionOnScreen,
                        yPositionOnScreen),
                    new ChestCategory(3,
                        "Materials",
                        new Vector2(672, 64),
                        new[] {-15, -16, -2, -12, -8, -28},
                        xPositionOnScreen,
                        yPositionOnScreen),
                    new ChestCategory(4,
                        "Cooking",
                        new Vector2(688, 64),
                        new[] {-25, -7, -18, -14, -6, -5, -27, -26},
                        xPositionOnScreen,
                        yPositionOnScreen),
                    new ChestCategory(5,
                        "Fishing",
                        new Vector2(640, 64),
                        new[] {-4, -21, -22},
                        xPositionOnScreen,
                        yPositionOnScreen),
                    new MiscCategory(6,
                        "Misc",
                        new Vector2(672, 80),
                        new[] {-24, -20},
                        xPositionOnScreen,
                        yPositionOnScreen)
                });

            SelectedCategory = _chestCategories.First();
        }

        private void SetupControllerSupport()
        {
            var rightItems =
                Enumerable.Range(0, 6)
                    .Select(i => ItemsToGrabMenu.inventory.ElementAt(i * 12 + 11))
                    .ToList();

            rightItems[0].rightNeighborID = UpArrow.myID;
            rightItems[1].rightNeighborID = UpArrow.myID;
            rightItems[2].rightNeighborID = colorPickerToggleButton?.myID ?? organizeButton.myID;
            rightItems[3].rightNeighborID = organizeButton.myID;
            rightItems[4].rightNeighborID = DownArrow.myID;
            rightItems[5].rightNeighborID = DownArrow.myID;

            if (!(colorPickerToggleButton is null))
            {
                colorPickerToggleButton.upNeighborID = UpArrow.myID;
                UpArrow.rightNeighborID = colorPickerToggleButton.myID;
            }

            UpArrow.leftNeighborID = rightItems[0].myID;
            DownArrow.rightNeighborID = organizeButton.myID;
            DownArrow.leftNeighborID = rightItems[4].myID;
            DownArrow.downNeighborID = rightItems[5].myID;
            organizeButton.downNeighborID = DownArrow.myID;

            if (!ModConfig.Instance.EnableCategories) return;

            var leftItems =
                Enumerable.Range(0, 6)
                    .Select(i => ItemsToGrabMenu.inventory.ElementAt(i * 12))
                    .ToList();

            CategoryComponents =
                Enumerable.Range(0, _chestCategories.Count)
                    .Select(i => (ClickableComponent)_chestCategories[i])
                    .ToList();

            for (var i = 0; i < CategoryComponents.Count; ++i)
            {
                CategoryComponents[i].myID = i + 239865;
                CategoryComponents[i].rightNeighborID = leftItems[i < 3 ? i : i - 1].myID;
                if (i <= 0) continue;
                leftItems[i - 1].leftNeighborID = CategoryComponents[i < 4 ? i - 1 : i].myID;
                CategoryComponents[i - 1].downNeighborID = CategoryComponents[i].myID;
                CategoryComponents[i].upNeighborID = CategoryComponents[i - 1].myID;
            }

            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }

        protected internal void Refresh()
        {
            var filteredItems = SelectedCategory.Filter(CustomChest.items);
            ItemsToGrabMenu.actualInventory = filteredItems.Skip(ItemsPerRow * _currentRow).ToList();
            _maxRow = (filteredItems.Count - 1) / 12 + 1 - Rows;
            if (_currentRow > _maxRow)
            {
                _currentRow = _maxRow;
            }
        }

        public override void draw(SpriteBatch b)
        {
            Draw(b);

            foreach (var chestCategory in _chestCategories)
            {
                var xOffset = chestCategory == SelectedCategory ? 8 : 0;
                chestCategory.Draw(b, xPositionOnScreen + xOffset, yPositionOnScreen);
            }

            if (_currentRow < _maxRow)
            {
                DownArrow.draw(b);
            }

            if (_currentRow > 0)
            {
                UpArrow.draw(b);
            }

            _hoverCategory?.DrawTooltip(b);

            DrawHover(b);

            drawMouse(b);
        }

        protected internal void ChangeCategory(ChestCategory cat)
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
            if (!(_hoverCategory is null))
            {
                ChangeCategory(_hoverCategory);
            }

            base.receiveLeftClick(x, y);

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
            if (ModConfig.Instance.EnableCategories)
            {
                _hoverCategory = _chestCategories.FirstOrDefault(c => c.containsPoint(x, y));
            }
            UpArrow.scale = UpArrow.containsPoint(x, y) ? Math.Min(UpArrow.scale + 0.02f, UpArrow.baseScale + 0.1f) : Math.Max(UpArrow.scale - 0.02f, UpArrow.baseScale);
            DownArrow.scale = DownArrow.containsPoint(x, y) ? Math.Min(DownArrow.scale + 0.02f, DownArrow.baseScale + 0.1f) : Math.Max(DownArrow.scale - 0.02f, DownArrow.baseScale);
        }
    }
}