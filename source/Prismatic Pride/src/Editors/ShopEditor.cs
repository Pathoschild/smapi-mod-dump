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
	// This editor adds the shop to Emily's fabric shelves, if STF is present.
	internal class ShopEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		private static readonly Location[] TileLocations = new Location[]
		{
			new (16, 23),
			new (17, 23),
		};

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals ("Maps\\HaleyHouse") &&
				Helper.ModRegistry.IsLoaded ("Cherry.ShopTileFramework");
		}

		public void Edit<T> (IAssetData asset)
		{
			var data = asset.AsMap ().Data;
			var layer = data.GetLayer ("Buildings");
			foreach (var tileLocation in TileLocations)
			{
				var tile = layer.PickTile (tileLocation * Game1.tileSize,
					Game1.viewport.Size);
				tile.Properties["Action"] = "";
				tile.Properties["Shop"] = "kdau.PrismaticPride.shop";
			}
		}
	}
}
