using System;
using EventSystem.Framework.FunctionEvents;
using FarmersMarketStall.Framework.MapEvents;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace FarmersMarketStall
{
    /// <summary>
    /// TODO:
    /// Make a farmers market menu
    /// MAke a way to store items to sell in a sort of inventory
    /// Make a map event to call the farmers market stall menu
    /// Make way to sell market items at a higher value
    /// Make a selling menu
    /// Make a minigame event for bonus money to earn.
    /// </summary>
    public class Class1 : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static Framework.MarketStall marketStall;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = this.Helper;
            ModMonitor = this.Monitor;

            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            marketStall = new Framework.MarketStall();
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, EventArgs e)
        {
            EventSystem.EventSystem.eventManager.addEvent(Game1.getLocationFromName("BusStop"), new ShopInteractionEvent("FarmersMarketStall", Game1.getLocationFromName("BusStop"), new Vector2(6, 11), new MouseButtonEvents(null, true), new MouseEntryLeaveEvent(null, null)));
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (marketStall.stock.Count > 0)
            {
                // Game1.endOfNightMenus.Push(new StardewValley.Menus.ShippingMenu(marketStall.stock));
                marketStall.sellAllItems();
            }
        }
    }
}
