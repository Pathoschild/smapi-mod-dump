// These declare which packages you are using.
using StardewModdingAPI; // SMAPI library, used here for loading the mod.
using Bookcase.Utils; // Bookcase's utils, used here for getting item sell price with ItemUtils
using Bookcase.Events; // Bookcase's events, used here to modify tooltips.

namespace PriceTooltips {

    // The class definition. A main mod class must extend Mod
    public class PriceTooltipsMod : Mod {

        // The mod initialization method. This is called by SMAPI when it loads your mod.
        // This is called before the game data has been loaded. 
        // This method is mandatory for loading your mod.
        public override void Entry(IModHelper helper) {

            // This adds a listener to Bookcase's item tooltip event.
            // OnItemTooltip is now called when the game renders an item tooltip.
            BookcaseEvents.OnItemTooltip.Add(this.OnItemTooltip);
        }

        // This method is an event listener. It will be called when an item tooltip is rendered.
        // The ItemTooltipEvent parameter contains the original tooltip data. 
        public void OnItemTooltip(ItemTooltipEvent evt) {

            // Check if the item is not null. Also checks if the tooltip already has price info.
            // In this case, -1 means there is no price data in the tooltip.
            if (evt.Item != null && evt.MoneyToShow == -1) {

                // Uses bookcase's ItemUtils to get the price of the item. 
                int price = ItemUtils.GetSellPrice(evt.Item, true);

                // Checks if the item actually has a sell value.
                if (price > 0) {

                    // Changes the amount of money shown on the tooltip to reflect the price.
                    // The money symbol graphic is already part of the vanilla tooltip. It's normally just disabled (-1).
                    // By changing this value to something greater than 0, the info becomes visible.
                    evt.MoneyToShow = price;
                }
            }
        }
    }
}
