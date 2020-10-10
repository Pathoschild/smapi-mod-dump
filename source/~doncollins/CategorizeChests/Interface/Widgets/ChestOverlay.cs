/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Linq;
using StardewValleyMods.CategorizeChests.Framework;

namespace StardewValleyMods.CategorizeChests.Interface.Widgets
{
    class ChestOverlay : Widget
    {
        private readonly ItemGrabMenu ItemGrabMenu;
        private readonly InventoryMenu InventoryMenu;
        private readonly InventoryMenu.highlightThisItem DefaultChestHighlighter;
        private readonly InventoryMenu.highlightThisItem DefaultInventoryHighlighter;

        private readonly Config Config;
        private readonly IChestDataManager ChestDataManager;
        private readonly IChestFiller ChestFiller;
        private readonly IItemDataManager ItemDataManager;
        private readonly ITooltipManager TooltipManager;
        private readonly Chest Chest;

        private TextButton OpenButton;
        private TextButton StashButton;
        private CategoryMenu CategoryMenu;

        public ChestOverlay(ItemGrabMenu menu,
            Chest chest,
            Config config,
            IChestDataManager chestDataManager,
            IChestFiller chestFiller,
            IItemDataManager itemDataManager,
            ITooltipManager tooltipManager)
        {
            Config = config;
            ItemDataManager = itemDataManager;
            ChestDataManager = chestDataManager;
            ChestFiller = chestFiller;
            TooltipManager = tooltipManager;

            Chest = chest;

            ItemGrabMenu = menu;
            InventoryMenu = menu.ItemsToGrabMenu;

            DefaultChestHighlighter = ItemGrabMenu.inventory.highlightMethod;
            DefaultInventoryHighlighter = InventoryMenu.highlightMethod;

            AddButtons();
        }

        protected override void OnParent(Widget parent)
        {
            base.OnParent(parent);

            if (parent != null)
            {
                Width = parent.Width;
                Height = parent.Height;
            }
        }

        private void AddButtons()
        {
            OpenButton = new TextButton("Categorize", Sprites.LeftProtrudingTab);
            OpenButton.OnPress += ToggleMenu;
            AddChild(OpenButton);

            StashButton = new TextButton(ChooseStashButtonLabel(), Sprites.LeftProtrudingTab);
            StashButton.OnPress += StashItems;
            AddChild(StashButton);

            PositionButtons();
        }

        private void PositionButtons()
        {
            StashButton.Width = OpenButton.Width = Math.Max(StashButton.Width, OpenButton.Width);

            OpenButton.Position = new Point(
                ItemGrabMenu.xPositionOnScreen + ItemGrabMenu.width / 2 - OpenButton.Width - 112 * Game1.pixelZoom,
                ItemGrabMenu.yPositionOnScreen + 22 * Game1.pixelZoom
            );

            StashButton.Position = new Point(
                OpenButton.Position.X + OpenButton.Width - StashButton.Width,
                OpenButton.Position.Y + OpenButton.Height
            );
        }

        private string ChooseStashButtonLabel()
        {
            var stashKey = Config.StashKey;

            if (stashKey == Keys.None)
            {
                return "Stash";
            }
            else
            {
                var keyName = Enum.GetName(typeof(Keys), stashKey);
                return $"Stash ({keyName})";
            }
        }

        private void ToggleMenu()
        {
            if (CategoryMenu == null)
            {
                OpenCategoryMenu();
            }
            else
            {
                CloseCategoryMenu();
            }
        }

        private void OpenCategoryMenu()
        {
            var chestData = ChestDataManager.GetChestData(Chest);
            CategoryMenu = new CategoryMenu(chestData, ItemDataManager, TooltipManager);
            CategoryMenu.Position = new Point(
                ItemGrabMenu.xPositionOnScreen + ItemGrabMenu.width / 2 - CategoryMenu.Width / 2 - 6 * Game1.pixelZoom,
                ItemGrabMenu.yPositionOnScreen - 10 * Game1.pixelZoom
            );
            CategoryMenu.OnClose += CloseCategoryMenu;
            AddChild(CategoryMenu);

            SetItemsClickable(false);
        }

        private void CloseCategoryMenu()
        {
            RemoveChild(CategoryMenu);
            CategoryMenu = null;

            SetItemsClickable(true);
        }

        private void StashItems()
        {
            if (!GoodTimeToStash())
                return;

            ChestFiller.DumpItemsToChest(Chest);
        }

        public override bool ReceiveKeyPress(Keys input)
        {
            if (input == Config.StashKey)
            {
                StashItems();
                return true;
            }

            return PropagateKeyPress(input);
        }

        public override bool ReceiveLeftClick(Point point)
        {
            var hit = PropagateLeftClick(point);

            if (!hit && CategoryMenu != null) // Are they clicking outside the menu to try to exit it?
                CloseCategoryMenu();

            return hit;
        }
        
        private void SetItemsClickable(bool clickable)
        {
            if (clickable)
            {
                ItemGrabMenu.inventory.highlightMethod = DefaultChestHighlighter;
                InventoryMenu.highlightMethod = DefaultInventoryHighlighter;
            }
            else
            {
                ItemGrabMenu.inventory.highlightMethod = item => false;
                InventoryMenu.highlightMethod = item => false;
            }
        }

        private bool GoodTimeToStash()
        {
            return ItemsAreClickable() && ItemGrabMenu.heldItem == null;
        }

        private bool ItemsAreClickable()
        {
            var items = ItemGrabMenu.inventory.actualInventory;
            var highlighter = ItemGrabMenu.inventory.highlightMethod;
            return items.Any(item => highlighter(item));
        }
    }
}