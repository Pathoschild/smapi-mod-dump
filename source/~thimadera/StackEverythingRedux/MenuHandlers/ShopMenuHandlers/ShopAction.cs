/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace StackEverythingRedux.MenuHandlers.ShopMenuHandlers
{
    public abstract class ShopAction : IShopAction
    {
        /// <summary>The ShopMenu instance we're overlaying over.</summary>
        protected ShopMenu NativeShopMenu { get; private set; }

        /// <summary>Native inventory menu.</summary>
        protected InventoryMenu InvMenu { get; private set; }

        /// <summary>Currency type of the shop.</summary>
        protected int ShopCurrencyType { get; private set; }

        /// <summary>Human-friendly name of the currency. Purely for logging purposes.</summary>
        protected string ShopCurrencyName => ShopCurrencyType switch
        {
            0 => "Money",
            1 => "festivalScore",
            2 => "clubCoins",
            4 => "QiGems",
            _ => "unknown"
        };

        /// <summary>The item to be bought/sold.</summary>
        protected Item ClickedItem = null;

        /// <summary>The number of items in the transaction.</summary>
        protected int Amount { get; set; } = 0;

        /// <summary>The number of items in the transaction.</summary>
        /// <remarks>This is to satisfy IShopAction which specifies a getter-only StackAmount property</remarks>
        public int StackAmount => Amount;


        /// <summary>Constructor.</summary>
        /// <param name="menu">Native shop menu.</param>
        /// <param name="item">Clicked item that this action will act on.</param>
        public ShopAction(ShopMenu menu, ISalable item)
        {
            Log.TraceIfD($"[{nameof(ShopAction)}..ctor] Instantiating for menu = {menu}, item = {item}");
            NativeShopMenu = menu;
            ClickedItem = (Item)item;

            try
            {
                InvMenu = NativeShopMenu.inventory;
                ShopCurrencyType = NativeShopMenu.currency;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(ShopAction)}..ctor] Failed to get native shop data. Exception:\n{e}");
            }
        }

        ~ShopAction()
        {
            Log.TraceIfD($"[{nameof(ShopAction)}] Finalized for menu = {NativeShopMenu}, item = {ClickedItem}");
        }

        /// <summary>Gets the size of the stack the action is acting on.</summary>
        public int GetStackAmount()
        {
            return Amount;
        }

        /// <summary>Verifies the conditions to perform the action.</summary>
        public abstract bool CanPerformAction();

        /// <summary>Does the action.</summary>
        /// <param name="amount">Number of items.</param>
        /// <param name="clickLocation">Where the player clicked.</param>
        public abstract void PerformAction(int amount, Point clickLocation);

        /// <summary>Creates an instance of the action.</summary>
        /// <returns>The instance or null if no valid item was selected.</returns>
        public static ShopAction Create()
        {
            return null;
        }
    }
}
