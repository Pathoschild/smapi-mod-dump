/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/BuildableGingerIslandFarm
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley.GameData.Locations;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class MenuUtility
	{
		public static void LocalizeGingerIslandFarmDisplayName(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
			{
				e.Edit(asset =>
				{
					asset.AsDictionary<string, LocationData>().Data["IslandWest"].DisplayName = ModEntry.Helper.Translation.Get("Menu.GingerIslandFarm.DisplayName");
				});
			}
		}

		public static void LocalizeCarpenterMenuChooseLocation(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Strings/Buildings"))
			{
				e.Edit(asset =>
				{
					asset.AsDictionary<string, string>().Data["Construction_ChooseLocation"] = ModEntry.Helper.Translation.Get("Menu.CarpenterMenu.ChooseLocation");
				});
			}
		}

		public static void LocalizePurchaseAnimalsMenuChooseLocation(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Strings/StringsFromCSFiles"))
			{
				e.Edit(asset =>
				{
					asset.AsDictionary<string, string>().Data["PurchaseAnimalsMenu.ChooseLocation"] = ModEntry.Helper.Translation.Get("Menu.PurchaseAnimalsMenu.ChooseLocation");
				});
			}
		}
	}
}
