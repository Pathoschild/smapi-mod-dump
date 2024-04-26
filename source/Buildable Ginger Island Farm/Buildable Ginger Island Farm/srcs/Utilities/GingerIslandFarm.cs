/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/BuildableGingerIslandFarm
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Locations;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class GingerIslandFarmUtility
	{
		public static void MakeAlwaysActive(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
			{
				e.Edit(asset =>
				{
					asset.AsDictionary<string, LocationData>().Data["IslandWest"].CreateOnLoad.AlwaysActive = true;
					MakeBuildable();
				});
			}
		}

		public static void MakeBuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");

			if (location is not null)
			{
				if (!location.HasMapPropertyWithValue("CanBuildHere") && Game1.player.hasOrWillReceiveMail("Island_UpgradeHouse"))
				{
					location.Map.Properties.Add("CanBuildHere", "T");
					MakeInaccessibleAreasUnbuildable();
					MakeFarmAreaBuildable();
					UpdateSlimeArea();
				}
			}
		}

		private static void MakeInaccessibleAreasUnbuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");
			HashSet<Point> tiles = GetInaccessibleAreasTiles();

			foreach (Point tile in tiles)
			{
				location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "f");
			}
		}

		private static void MakeFarmAreaBuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");
			HashSet<Point> tiles = GetFarmAreaTiles();

			foreach (Point tile in tiles)
			{
				location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "true");
			}
		}

		public static void UpdateSlimeArea()
		{
			if (ModEntry.Config.AllowBuildingInSlimeArea)
			{
				MakeSlimeAreaBuildable();
			}
			else
			{
				MakeSlimeAreaUnbuildable();
			}
		}

		private static void MakeSlimeAreaUnbuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");
			HashSet<Point> tiles = GetSlimeAreaTiles();

			foreach (Point tile in tiles)
			{
				location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "f");
			}
		}

		private static void MakeSlimeAreaBuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");
			HashSet<Point> tiles = GetSlimeAreaTiles();

			foreach (Point tile in tiles)
			{
				location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "true");
			}
		}

		private static HashSet<Point> GetInaccessibleAreasTiles()
		{
			if (Compatibility.IsIslandOverhaulLoaded)
			{
				return TilesIslandOverhaulUtility.GetInaccessibleAreasTiles();
			}
			else if (Compatibility.IsModestMapsGingerIslandFarmLoaded)
			{
				return TilesModestMapsGingerIslandFarmUtility.GetInaccessibleAreasTiles();
			}
			else
			{
				return TilesDefaultUtility.GetInaccessibleAreasTiles();
			}
		}

		private static HashSet<Point> GetFarmAreaTiles()
		{
			if (Compatibility.IsIslandOverhaulLoaded)
			{
				return TilesIslandOverhaulUtility.GetFarmAreaTiles();
			}
			else if (Compatibility.IsModestMapsGingerIslandFarmLoaded)
			{
				return TilesModestMapsGingerIslandFarmUtility.GetFarmAreaTiles();
			}
			else
			{
				return TilesDefaultUtility.GetFarmAreaTiles();
			}
		}

		private static HashSet<Point> GetSlimeAreaTiles()
		{
			if (Compatibility.IsIslandOverhaulLoaded)
			{
				return TilesIslandOverhaulUtility.GetSlimeAreaTiles();
			}
			else if (Compatibility.IsModestMapsGingerIslandFarmLoaded)
			{
				return TilesModestMapsGingerIslandFarmUtility.GetSlimeAreaTiles();
			}
			else
			{
				return TilesDefaultUtility.GetSlimeAreaTiles();
			}
		}
	}
}
