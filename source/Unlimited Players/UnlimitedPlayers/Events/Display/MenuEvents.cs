/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Armitxes/StardewValley_UnlimitedPlayers
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Linq;
using static StardewValley.Menus.CarpenterMenu;

namespace UnlimitedPlayers.Events.Display
{
	public class MenuEvents
	{
		public void MenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.NewMenu is CarpenterMenu)
				MenuChanged_CarpenterMenu(sender, e);
		}

		public void MenuChanged_CarpenterMenu(object sender, MenuChangedEventArgs e)
		{
			if (!Context.IsWorldReady)
				return;

			if (Game1.IsMasterGame || Game1.IsServer)
			{
				int buildingsConstructed = Game1.getFarm().getNumberBuildingsConstructed("Cabin");
				if (buildingsConstructed <= 8) return;

				CarpenterMenu newMenu = e.NewMenu as CarpenterMenu;
				int index = newMenu.Blueprints.Last().Index + 1;

				var cabin = Game1.buildingData["Cabin"];
				var buildings = newMenu.Blueprints;

				buildings.Add(new BlueprintEntry(index, "Cabin", cabin, null));
			}
		}
	}
}
