using System;
using System.Collections.Generic;
using System.Reflection;
using Igorious.StardewValley.DynamicApi2.Extensions;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Igorious.StardewValley.ShowcaseMod.Core
{
    internal sealed class ShowcaseContainer : StorageContainer
    {
        private static readonly Lazy<FieldInfo> ItemChangeBehaviorField = typeof(StorageContainer).GetLazyInstanceField("itemChangeBehavior");

        private List<Item> Items { get; }
        private DiscreteColorPicker ColorPicker { get; }

        public ShowcaseContainer(
            Showcase showcase,
            List<Item> items,
            int capacity,
            int rows,
            InventoryMenu.highlightThisItem isItemEnabled,
            bool allowColoring)
            : base(items, capacity, rows, null, isItemEnabled)
        {
            Items = items;
            this.SetFieldValue<behaviorOnItemChange>(ItemChangeBehaviorField, ProcessItemChanged);
            ItemsToGrabMenu.movePosition(0, (3 - rows) * Game1.tileSize);

            if (!allowColoring) return;
            ColorPicker = new ShowcaseColorPicker(xPositionOnScreen, yPositionOnScreen - Game1.tileSize - borderWidth * 2, showcase);
        }

        private bool ProcessItemChanged(Item newItem, int position, Item oldItem, StorageContainer container, bool isRemoving)
        {
            return isRemoving? OnItemRemoved(newItem, position, oldItem) : OnItemAdded(newItem, position, oldItem, container);
        }

        private bool OnItemRemoved(Item containerItem, int position, Item handItem)
        {
            if (handItem?.Stack > 1 && !handItem.Equals(containerItem))
            {
                return false;
            }

            var newCellItem = handItem != null && !handItem.Equals(containerItem) ? containerItem : null;
            Items[position] = newCellItem;
            return true;
        }

        private bool OnItemAdded(Item handItem, int position, Item containerItem, StorageContainer container)
        {
            if (handItem.Stack > 1 || handItem.Stack == 1 && containerItem?.Stack == 1 && handItem.canStackWith(containerItem))
            {
                if (containerItem != null)
                {
                    if (containerItem.canStackWith(handItem))
                    {
                        container.ItemsToGrabMenu.actualInventory[position].Stack = 1;
                        container.heldItem = containerItem;
                    }
                    else
                    {
                        Utility.addItemToInventory(containerItem, position, container.ItemsToGrabMenu.actualInventory);
                        container.heldItem = handItem;
                    }
                    return false;
                }

                var newStack = handItem.Stack - 1;
                var one = handItem.getOne();
                one.Stack = newStack;
                container.heldItem = one;
                handItem.Stack = 1;
            }

            if (position < Items.Count)
            {
                Items[position] = handItem;
            }
            return true;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            ColorPicker?.receiveLeftClick(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            ((Action<SpriteBatch>)base.draw)(b);
            ColorPicker?.draw(b);
            drawMouse(b);
        }
    }
}