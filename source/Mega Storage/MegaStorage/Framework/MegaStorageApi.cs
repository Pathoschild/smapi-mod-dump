using MegaStorage.Framework.UI;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace MegaStorage.Framework
{
    public class MegaStorageApi : IMegaStorageApi
    {
        public static MegaStorageApi Instance { get; private set; }

        /*********
        ** Events
        *********/
        public event EventHandler<ICustomChestEventArgs> VisibleItemsRefreshed;
        public event EventHandler<ICustomChestEventArgs> ColorPickerToggleButtonClicked;
        public event EventHandler<ICustomChestEventArgs> BeforeFillStacksButtonClicked;
        public event EventHandler<ICustomChestEventArgs> AfterFillStacksButtonClicked;
        public event EventHandler<ICustomChestEventArgs> BeforeOrganizeButtonClicked;
        public event EventHandler<ICustomChestEventArgs> AfterOrganizeButtonClicked;
        public event EventHandler<ICustomChestEventArgs> BeforeStarButtonClicked;
        public event EventHandler<ICustomChestEventArgs> AfterStarButtonClicked;
        public event EventHandler<ICustomChestEventArgs> BeforeOkButtonClicked;
        public event EventHandler<ICustomChestEventArgs> AfterOkButtonClicked;
        public event EventHandler<ICustomChestEventArgs> BeforeTrashCanClicked;
        public event EventHandler<ICustomChestEventArgs> AfterTrashCanClicked;
        public event EventHandler<ICustomChestEventArgs> BeforeCategoryChanged;
        public event EventHandler<ICustomChestEventArgs> AfterCategoryChanged;

        /*********
        ** Public methods
        *********/
        public MegaStorageApi()
        {
            Instance = this;
        }
        public Rectangle? GetItemsToGrabMenuBounds() => I?.GetItemsToGrabMenuBounds;
        public Rectangle? GetInventoryBounds() => I?.GetInventoryBounds;
        public Vector2? GetItemsToGrabMenuDimensions() => I?.GetItemsToGrabMenuDimensions;
        public Vector2? GetInventoryDimensions() => I?.GetInventoryDimensions;
        public Vector2? GetItemsToGrabMenuPosition() => I?.GetItemsToGrabMenuPosition;
        public Vector2? GetInventoryPosition() => I?.GetInventoryPosition;
        public void RefreshItems() => I?.RefreshItems();
        public void ClickColorPickerToggleButton() => I?.ClickColorPickerToggleButton();
        public void ClickFillStacksButton() => I?.ClickFillStacksButton();
        public void ClickOrganizeButton() => I?.ClickOrganizeButton();
        public void ClickStarButton() => I?.ClickStarButton();
        public void ClickOkButton() => I?.ClickOkButton();
        public void ClickTrashCan() => I?.ClickTrashCan();
        public void ClickCategoryButton(string categoryName) => I?.ClickCategoryButton(categoryName);
        public void ScrollCategory(int direction) => I?.ScrollCategory(direction);

        /*********
        ** Private methods
        *********/
        private static CustomItemGrabMenu I =>
            (Game1.activeClickableMenu is CustomItemGrabMenu customItemGrabMenu) ? customItemGrabMenu : null;

        internal static void InvokeVisibleItemsRefreshed(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.VisibleItemsRefreshed?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeColorPickerToggleButtonClicked(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.ColorPickerToggleButtonClicked?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeBeforeFillStacksButtonClicked(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeFillStacksButtonClicked?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeAfterFillStacksButtonClicked(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterFillStacksButtonClicked?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeBeforeOrganizeButtonClicked(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeOrganizeButtonClicked?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeAfterOrganizeButtonClicked(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterOrganizeButtonClicked?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeBeforeStarButtonClicked(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeStarButtonClicked?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeAfterStarButtonClicked(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterStarButtonClicked?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeBeforeOkButtonClicked(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeOkButtonClicked?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeAfterOkButtonClicked(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterOkButtonClicked?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeBeforeTrashCanClicked(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeTrashCanClicked?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeAfterTrashCanClicked(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterTrashCanClicked?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeBeforeCategoryChanged(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeCategoryChanged?.Invoke(itemGrabMenu, customChestEventArgs);

        internal static void InvokeAfterCategoryChanged(ItemGrabMenu itemGrabMenu,
            CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterCategoryChanged?.Invoke(itemGrabMenu, customChestEventArgs);
    }

    public class CustomChestEventArgs : EventArgs, ICustomChestEventArgs
    {
        public IList<Item> VisibleItems { get; set; }
        public IList<Item> AllItems { get; set; }
        public string CurrentCategory { get; set; }
        public Item HeldItem { get; set; }
        public Chest RemoteChest { get; set; }
    }
}
