/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarp
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace EastScarp
{
	public static class RainWatering
	{
		// private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.data;

		public static void DayUpdate ()
		{
			if (!Context.IsMainPlayer)
				return;

			foreach (var area in Data.RainWateringAreas)
			{
				GameLocation location = Game1.getLocationFromName (area.Location);
				if (location != null && Game1.IsRainingHere (location))
				{
					Monitor.Log ($"Watering crops in area {area.adjustArea (location)} of location '{location.Name}'",
						LogLevel.Trace);
					foreach (var feature in location.terrainFeatures.Values)
					{
						if (feature is HoeDirt hoeDirt &&
							area.checkArea (location, hoeDirt.currentTileLocation))
						{
							hoeDirt.state.Value = HoeDirt.watered;
						}
					}
				}
			}
		}
	}
}
