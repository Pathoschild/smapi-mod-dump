/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarpe
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace EastScarp
{
	public static class WaterColors
	{
		// private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.data;

		public static void Apply ()
		{
			if (!Context.IsWorldReady)
				return;

			foreach (var color in Data.WaterColors)
				ApplyColor (color);
		}

		private static bool ApplyColor (WaterColor color)
		{
			// Must be in the right location.
			var location = Game1.player.currentLocation;
			if (location.Name != color.Location)
				return false;

			// Conditions must hold.
			if (!color.Conditions.check ())
				return false;

			// Set the color.
			Monitor.Log ($"Setting water color for location '{location.Name}' to {color.Color}.",
				LogLevel.Trace);
			location.waterColor.Value = color.Color;
			return true;
		}
	}
}
