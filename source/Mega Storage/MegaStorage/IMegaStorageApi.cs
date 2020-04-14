using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace MegaStorage
{
    public interface IMegaStorageApi
    {
        event EventHandler<ICustomChestEventArgs> VisibleItemsRefreshed;
        event EventHandler<ICustomChestEventArgs> ColorPickerToggleButtonClicked;
        event EventHandler<ICustomChestEventArgs> BeforeFillStacksButtonClicked;
        event EventHandler<ICustomChestEventArgs> AfterFillStacksButtonClicked;
        event EventHandler<ICustomChestEventArgs> BeforeOrganizeButtonClicked;
        event EventHandler<ICustomChestEventArgs> AfterOrganizeButtonClicked;
        event EventHandler<ICustomChestEventArgs> BeforeStarButtonClicked;
        event EventHandler<ICustomChestEventArgs> AfterStarButtonClicked;
        event EventHandler<ICustomChestEventArgs> BeforeOkButtonClicked;
        event EventHandler<ICustomChestEventArgs> AfterOkButtonClicked;
        event EventHandler<ICustomChestEventArgs> BeforeTrashCanClicked;
        event EventHandler<ICustomChestEventArgs> AfterTrashCanClicked;
        event EventHandler<ICustomChestEventArgs> BeforeCategoryChanged;
        event EventHandler<ICustomChestEventArgs> AfterCategoryChanged;

        /// <summary>Returns the Rectangle bounds of the ItemsToGrabMenu.</summary>
        Rectangle? GetItemsToGrabMenuBounds();

        /// <summary>Returns the Rectangle bounds of the Inventory.</summary>
        Rectangle? GetInventoryBounds();
        /// <summary>Returns the Vector2 dimensions of the ItemsToGrabMenu.</summary>
        Vector2? GetItemsToGrabMenuDimensions();

        /// <summary>Returns the Vector2 dimensions of the Inventory.</summary>
        Vector2? GetInventoryDimensions();
        /// <summary>Returns the Vector2 position of the ItemsToGrabMenu.</summary>
        Vector2? GetItemsToGrabMenuPosition();

        /// <summary>Returns the Vector2 position of the Inventory.</summary>
        Vector2? GetInventoryPosition();

        /// <summary>Refreshes the visible contents of a chest.</summary>
        void RefreshItems();

        /// <summary>Click the Color Picker Toggle button.</summary>
        void ClickColorPickerToggleButton();

        /// <summary>Click the Fill Stacks button.</summary>
        void ClickFillStacksButton();

        /// <summary>Click the Organize button.</summary>
        void ClickOrganizeButton();

        /// <summary>Click the Star button.</summary>
        void ClickStarButton();

        /// <summary>Click the OK Button.</summary>
        void ClickOkButton();

        /// <summary>Click the Trash Can.</summary>
        void ClickTrashCan();

        /// <summary>Click the Category Button.</summary>
        /// <param name="categoryName">The name of the category button to click.</param>
        void ClickCategoryButton(string categoryName);

        /// <summary>Scrolls the Category.</summary>
        /// <param name="direction">The direction you want to scroll.</param>
        void ScrollCategory(int direction);
    }

    public interface ICustomChestEventArgs
    {
        IList<Item> VisibleItems { get; set; }
        IList<Item> AllItems { get; set; }
        string CurrentCategory { get; set; }
        Item HeldItem { get; set; }
        Chest RemoteChest { get; set; }
    }
}
