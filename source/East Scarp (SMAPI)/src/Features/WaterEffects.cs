/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarp
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace EastScarp
{
	public static class WaterEffects
	{
		// private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.data;

		public static void Apply ()
		{
			if (!Context.IsWorldReady)
				return;

			foreach (var effect in Data.WaterEffects)
				ApplyEffect (effect);
		}

		private static bool ApplyEffect (WaterEffect effect)
		{
			// Must be in the right location.
			var location = Game1.player.currentLocation;
			if (location.Name != effect.Location)
				return false;

			// Conditions must hold.
			if (!effect.Conditions.check ())
				return false;

			// Handle a missing waterTiles array.
			if (location.waterTiles == null)
			{
				if (effect.Apply)
				{
					int mapWidth = location.map.Layers[0]?.LayerWidth ?? 0;
					int mapHeight = location.map.Layers[0]?.LayerHeight ?? 0;
					location.waterTiles = new bool[mapWidth, mapHeight];
				}
				else
				{
					// If removing, nothing to do since there are none to begin with.
					return true;
				}
			}

			// Apply or remove the effect.
			Rectangle area = effect.adjustArea (location);
			Monitor.Log ($"{(effect.Apply ? "Applying" : "Removing")} water effects in area {area} of location '{location.Name}'.",
				LogLevel.Trace);
			for (int x = area.Left; x <= area.Right; ++x)
			{
				for (int y = area.Top; y <= area.Bottom; ++y)
				{
					location.waterTiles[x, y] = effect.Apply;
				}
			}
			return true;
		}
	}
}
