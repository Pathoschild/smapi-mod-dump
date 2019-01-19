using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Common.StardewValley.LetterMenu
{
    /// <summary>
    /// A helper class for a [LetterViewerMenu]. Enables simplified creation and interaction such as a Show() method
    /// or a event raised when a [LetterViewerMenu] is closed.
    /// </summary>
    public class ItemLetterMenuHelper
    {
        /// <summary>
        /// Raised after the letter menu is closed. Exposes information such as the selected item, if any.
        /// </summary>
        public event EventHandler<ItemLetterMenuClosedEventArgs> MenuClosed;

        private ItemLetterViewerMenu itemMenu;

        public ItemLetterMenuHelper(string text, Item item)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            itemMenu = new ItemLetterViewerMenu(text, item);
        }

        public void Show()
        {
            itemMenu.exitFunction = new IClickableMenu.onExit(OnExit);
            Game1.activeClickableMenu = itemMenu;
        }

        private void OnExit()
        {
            MenuClosed?.Invoke(this, new ItemLetterMenuClosedEventArgs(itemMenu.SelectedItem));
        }

        private class ItemLetterViewerMenu : LetterViewerMenu
        {
            public Item SelectedItem { get; private set; }

            public ItemLetterViewerMenu(string text, Item item) : base(text)
            {
                Type letterViewerMenuType = typeof(LetterViewerMenu);
                FieldInfo isMailRef = letterViewerMenuType.GetField(
                    "isMail",
                    BindingFlags.NonPublic | BindingFlags.Instance)
                    ?? throw new MissingFieldException(nameof(LetterViewerMenu), "isMail");

                isMailRef.SetValue(this, true);

                if (item == null)
                {
                    return;
                }

                // Add item to mail
                this.itemsToGrab.Add(
                    new ClickableComponent(
                        new Rectangle(this.xPositionOnScreen + this.width / 2 - 48, this.yPositionOnScreen + this.height - 32 - 96, 96, 96),
                        item)
                    {
                        myID = region_itemGrabButton,
                        leftNeighborID = region_backButton,
                        rightNeighborID = region_forwardButton
                    });

                this.backButton.rightNeighborID = region_itemGrabButton;
                this.forwardButton.leftNeighborID = region_itemGrabButton;

                this.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }

            public override void receiveLeftClick(int x, int y, bool playSound = true)
            {
                foreach (ClickableComponent clickableComponent in this.itemsToGrab)
                {
                    if (clickableComponent.containsPoint(x, y) && clickableComponent.item != null)
                    {
                        // Set the selected item
                        SelectedItem = clickableComponent.item;

                        Game1.playSound("coin");
                        clickableComponent.item = (Item)null;

                        return;
                    }
                }

                base.receiveLeftClick(x, y, playSound);
            }
        }
    }
}
