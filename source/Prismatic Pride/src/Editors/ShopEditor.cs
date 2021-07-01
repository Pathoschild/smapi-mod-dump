/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/prismaticpride
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;

namespace PrismaticPride
{
	// This editor adds the shop tile to the left side of Emily's fabric shelves,
	// if STF is present to support the shop.
	internal class ShopEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals ("Maps\\HaleyHouse") &&
				Helper.ModRegistry.IsLoaded ("Cherry.ShopTileFramework");
		}

		public void Edit<T> (IAssetData asset)
		{
			var data = asset.AsMap ().Data;
			var layer = data.GetLayer ("Buildings");
			var tile = layer.PickTile (new Location (16, 23) * Game1.tileSize,
				Game1.viewport.Size);
			tile.Properties["Action"] = "";
			tile.Properties["Shop"] = "kdau.PrismaticPride.shop";
		}
	}
}
