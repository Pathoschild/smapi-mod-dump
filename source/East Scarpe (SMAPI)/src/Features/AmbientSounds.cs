/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarpe
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace EastScarpe
{
	public static class AmbientSounds
	{
		// private static IModHelper Helper => ModEntry.Instance.Helper;
		// private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.data;

		public static void Play ()
		{
			// World must be ready without an event active.
			if (!Context.IsWorldReady || Game1.eventUp)
				return;

			foreach (var sound in Data.AmbientSounds)
			{
				if (PlaySound (sound))
					break;
			}
		}

		private static bool PlaySound (AmbientSound sound)
		{
			// Must be in the right location and area.
			if (!sound.checkArea (Game1.player.currentLocation,
					Game1.player.getTileLocationPoint ()))
				return false;

			// Random roll must succeed.
			if (!(Game1.random.NextDouble () < sound.Chance))
				return false;

			// Conditions must hold.
			if (!sound.Conditions.check ())
				return false;

			// Play the sound.
			// Monitor.Log ($"Playing sound '{sound.Sound}' in location '{sound.Location}'.",
			// 	LogLevel.Trace);
			Game1.player.currentLocation.localSound (sound.Sound);
			return true;
		}
	}
}
