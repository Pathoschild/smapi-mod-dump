/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/SleepyEye
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using StardewModdingAPI.Events;
using SpaceShared;

namespace SleepyEye
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            Log.Monitor = Monitor;

            helper.Events.Display.MenuChanged += onMenuChanged;
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onMenuChanged( object sender, MenuChangedEventArgs e )
        {
            if (!(e.NewMenu is ShopMenu menu) || menu.portraitPerson.Name != "Pierre")
                return;

            Log.debug("Adding tent to shop");

            var forSale = menu.forSale;
            var itemPriceAndStock = menu.itemPriceAndStock;

            var item = new TentTool();
            forSale.Add(item);
            itemPriceAndStock.Add(item, new int[] { item.salePrice(), item.Stack });
        }
    }
}
