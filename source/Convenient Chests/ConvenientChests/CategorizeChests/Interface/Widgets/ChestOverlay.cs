/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System;
using ConvenientChests.StashToChests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests.Interface.Widgets {
    internal class ChestOverlay : Widget {
        private ItemGrabMenu ItemGrabMenu { get; }
        private CategorizeChestsModule Module { get; }
        private Chest Chest { get; }
        private ITooltipManager TooltipManager { get; }

        private readonly InventoryMenu InventoryMenu;
        private readonly InventoryMenu.highlightThisItem DefaultChestHighlighter;
        private readonly InventoryMenu.highlightThisItem DefaultInventoryHighlighter;

        private TextButton CategorizeButton { get; set; }
        private TextButton StashButton { get; set; }
        private CategoryMenu CategoryMenu { get; set; }

        public ChestOverlay(CategorizeChestsModule module, Chest chest, ItemGrabMenu menu) {
            Module = module;
            Chest = chest;
            ItemGrabMenu = menu;
            InventoryMenu = menu.ItemsToGrabMenu;
            TooltipManager = new TooltipManager();

            DefaultChestHighlighter = ItemGrabMenu.inventory.highlightMethod;
            DefaultInventoryHighlighter = InventoryMenu.highlightMethod;

            AddButtons();
        }

        protected override void OnParent(Widget parent) {
            base.OnParent(parent);

            if (parent == null) return;
            Width = parent.Width;
            Height = parent.Height;
        }

        public override void Draw(SpriteBatch batch) {
            base.Draw(batch);
            TooltipManager.Draw(batch);
        }

        private void AddButtons() {
            CategorizeButton = new TextButton("Categorize", Sprites.LeftProtrudingTab);
            CategorizeButton.OnPress += ToggleMenu;
            AddChild(CategorizeButton);

            StashButton = new TextButton(ChooseStashButtonLabel(), Sprites.LeftProtrudingTab);
            StashButton.OnPress += StashItems;
            AddChild(StashButton);

            PositionButtons();
        }

        private void PositionButtons() {
            var delta = Chest.SpecialChestType == Chest.SpecialChestTypes.BigChest ? -128 : -112;

            StashButton.Width = CategorizeButton.Width = Math.Max(StashButton.Width, CategorizeButton.Width);

            CategorizeButton.Position = new Point(
                ItemGrabMenu.xPositionOnScreen + ItemGrabMenu.width / 2 - CategorizeButton.Width +
                delta * Game1.pixelZoom,
                ItemGrabMenu.yPositionOnScreen + 22 * Game1.pixelZoom
            );

            StashButton.Position = new Point(
                CategorizeButton.Position.X + CategorizeButton.Width - StashButton.Width,
                CategorizeButton.Position.Y + CategorizeButton.Height - 0
            );
        }

        private string ChooseStashButtonLabel() {
            return Module.Config.StashKey == SButton.None
                ? "Stash"
                : $"Stash ({Module.Config.StashKey})";
        }

        private void ToggleMenu() {
            if (CategoryMenu == null)
                OpenCategoryMenu();

            else
                CloseCategoryMenu();
        }

        private void OpenCategoryMenu() {
            var chestData = Module.ChestDataManager.GetChestData(Chest);
            CategoryMenu = new CategoryMenu(chestData, Module.ItemDataManager, TooltipManager, ItemGrabMenu.width - 24);
            CategoryMenu.Position = new Point(
                ItemGrabMenu.xPositionOnScreen - GlobalBounds.X - 12,
                ItemGrabMenu.yPositionOnScreen - GlobalBounds.Y - 60
            );

            CategoryMenu.OnClose += CloseCategoryMenu;
            AddChild(CategoryMenu);

            SetItemsClickable(false);
        }

        private void CloseCategoryMenu() {
            RemoveChild(CategoryMenu);
            CategoryMenu = null;

            SetItemsClickable(true);
        }

        private void StashItems() => StackLogic.StashToChest(Chest, ModEntry.StashNearby.AcceptingFunction);

        public override bool ReceiveLeftClick(Point point) {
            var hit = PropagateLeftClick(point);
            if (!hit && CategoryMenu != null)
                // Are they clicking outside the menu to try to exit it?
                CloseCategoryMenu();

            return hit;
        }

        private void SetItemsClickable(bool clickable) {
            if (clickable) {
                ItemGrabMenu.inventory.highlightMethod = DefaultChestHighlighter;
                InventoryMenu.highlightMethod = DefaultInventoryHighlighter;
            }
            else {
                ItemGrabMenu.inventory.highlightMethod = i => false;
                InventoryMenu.highlightMethod = i => false;
            }
        }
    }
}