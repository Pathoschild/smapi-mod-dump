using BetterShippingBox.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;

namespace BetterShippingBox
{
	public class BetterShippingBox : Mod
	{
		public override void Entry(IModHelper helper)
		{
            MenuEvents.MenuChanged += MenuEvents_OnMenuChanged;
		}

		private void MenuEvents_OnMenuChanged(object sender, EventArgsClickableMenuChanged e)
		{
            if ((e.NewMenu is ItemGrabMenu menu) && (menu.shippingBin))
            {
                Game1.activeClickableMenu = new BetterShippingMenu();
            }
		}
	}
}
