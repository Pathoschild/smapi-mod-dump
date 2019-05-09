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
