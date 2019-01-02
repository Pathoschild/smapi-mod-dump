using BetterShippingBox.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace BetterShippingBox
{
    public class BetterShippingBox : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if ((e.NewMenu is ItemGrabMenu menu) && (menu.shippingBin))
            {
                Game1.activeClickableMenu = new BetterShippingMenu();
            }
        }
    }
}
