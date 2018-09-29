using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;

namespace StackSplitX.MenuHandlers
{
    public abstract class ShopAction : IShopAction
    {
        /// <summary>Reflection helper.</summary>
        protected readonly IReflectionHelper Reflection;
        
        /// <summary>Monitor for logging.</summary>
        protected readonly IMonitor Monitor;

        /// <summary>Native shope menu.</summary>
        protected ShopMenu NativeShopMenu { get; private set; }

        /// <summary>Native inventory menu.</summary>
        protected InventoryMenu Inventory { get; private set; }

        /// <summary>Currency type of the shop.</summary>
        protected int ShopCurrencyType { get; private set; }

        /// <summary>The item to be bought/sold.</summary>
        protected Item ClickedItem = null;

        /// <summary>The number of items in the transaction.</summary>
        protected int Amount { get; set; } = 0;

        /// <summary>The number of items in the transaction.</summary>
        public int StackAmount => this.Amount;


        /// <summary>Constructor.</summary>
        /// <param name="reflection">ReflectionHelper</param>
        /// <param name="monitor">Monitor.</param>
        /// <param name="menu">Native shop menu.</param>
        /// <param name="item">Clicked item that this action will act on.</param>
        public ShopAction(IReflectionHelper reflection, IMonitor monitor, ShopMenu menu, Item item)
        {
            this.Reflection = reflection;
            this.Monitor = monitor;
            this.NativeShopMenu = menu;
            this.ClickedItem = item;

            try
            {
                this.Inventory = this.NativeShopMenu.inventory;
                this.ShopCurrencyType = this.Reflection.GetField<int>(this.NativeShopMenu, "currency").GetValue();
            }
            catch (Exception e)
            {
                this.Monitor.Log($"Failed to get native shop data: {e}", LogLevel.Error);
            }
        }

        /// <summary>Gets the size of the stack the action is acting on.</summary>
        public int GetStackAmount()
        {
            return this.Amount;
        }

        /// <summary>Verifies the conditions to perform te action.</summary>
        public abstract bool CanPerformAction();

        /// <summary>Does the action.</summary>
        /// <param name="amount">Number of items.</param>
        /// <param name="clickLocation">Where the player clicked.</param>
        public abstract void PerformAction(int amount, Point clickLocation);

        /// <summary>Creates an instance of the action.</summary>
        /// <param name="reflection">Reflection helper.</param>
        /// <param name="monitor">Monitor for logging.</param>
        /// <param name="shopMenu">Native shop menu.</param>
        /// <param name="mouse">Mouse position.</param>
        /// <returns>The instance or null if no valid item was selected.</returns>
        public static ShopAction Create(IReflectionHelper reflection, IMonitor monitor, ShopMenu shopMenu, Point mouse)
        {
            return null;
        }
    }
}
