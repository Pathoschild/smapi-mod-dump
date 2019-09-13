using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using SObject = StardewValley.Object;


namespace TravellingMerchantInventory
{
    public class ModEntry : Mod
    {
        private bool buyFromPage;
        private SButton buttonToBringUpMenu;
        private ModConfig modConfig;
        private Dictionary<Item, int[]> stock;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            modConfig = Helper.ReadConfig<ModConfig>();
            buyFromPage = modConfig.buyFromMenuPage;
            buttonToBringUpMenu = modConfig.button; 
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// 
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            DayOfWeek currentDay = Game1.Date.DayOfWeek;
            if (currentDay == DayOfWeek.Friday || currentDay == DayOfWeek.Sunday)
            {
                //this is how the source code generates the seed
                int seed = (int)((long)Game1.uniqueIDForThisGame + (long)Game1.stats.DaysPlayed);

                //initialize the stock
                stock = Utility.getTravelingMerchantStock(seed);
            }    
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if(e.Button.Equals(buttonToBringUpMenu))
            {
                bringUpThePage();
            }
        }

        private void bringUpThePage()
        {
            //check for the days the travelling merchant is in town
            DayOfWeek currentDay = Game1.Date.DayOfWeek;
            if (currentDay == DayOfWeek.Friday || currentDay == DayOfWeek.Sunday)
            {
                TravellingMerchantPage travellingMerchantPage = new TravellingMerchantPage(stock, buyFromPage);
                stock = this.Helper.Reflection.GetField<Dictionary<Item, int[]>>(travellingMerchantPage, "itemPriceAndStock").GetValue();
                Game1.activeClickableMenu = travellingMerchantPage;

            }
        }
    }
}
