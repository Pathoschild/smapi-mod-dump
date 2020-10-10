/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/captncraig/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SadisticBundles
{
    class Acheivements
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        public Acheivements(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;
            Helper.Events.GameLoop.DayEnding += DayEnding;
            Helper.Events.Display.MenuChanged += MenuChanged;
        }

        private void DayEnding(object sender, DayEndingEventArgs e)
        {
        }

        private void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            var shop = e.NewMenu as ShopMenu;
            // casino always sells lucky purple shorts
            if (shop != null && Game1.currentLocation.Name == "Club")
            {
                using (var stock = StoreHelper.Read(shop))
                {
                    stock.AddItem(789, 1000000, 1, 4);
                }
            }
        }
    }
}
