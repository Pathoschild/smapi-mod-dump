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
	public static class SeaMonster
	{
		// private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.data;

		public static void CheckSpawns ()
		{
			// World must be ready without an event active.
			if (!Context.IsWorldReady || Game1.eventUp)
				return;

			foreach (var spawn in Data.SeaMonsterSpawns)
			{
				if (CheckSpawn (spawn))
					break;
			}
		}

		private static bool CheckSpawn (SeaMonsterSpawn spawn)
		{
			// Must be in the right location.
			var location = Game1.player.currentLocation;
			if (location.Name != spawn.Location)
				return false;

			// Random roll must succeed.
			if (!(Game1.random.NextDouble () < spawn.Chance))
				return false;

			// Conditions must hold.
			if (!spawn.Conditions.check ())
				return false;

			// Must not have any Sea Monster currently spawned.
			if (location.temporarySprites.Exists ((sprite) =>
					sprite is SeaMonsterTemporarySprite))
				return false;

			// Randomly find a starting position within the area.
			Rectangle area = spawn.adjustArea (location);
			int x = Game1.random.Next (area.Left, area.Right + 1);
			int y = Game1.random.Next (area.Top, area.Bottom + 1);

			// Confirm the monster can swim offscreen from there.
			bool canSwimOut = true;
			int height = location.map.Layers[0]?.LayerHeight ?? 0;
			for (int dy = y; dy < height; ++dy)
			{
				if (location.doesTileHaveProperty (x, dy, "Water", "Back") == null ||
					location.doesTileHaveProperty (x - 1, dy, "Water", "Back") == null ||
					location.doesTileHaveProperty (x, dy, "Water", "Back") == null)
				{
					canSwimOut = false;
					break;
				}
			}
			if (!canSwimOut)
				return false;

			// Spawn the monster.
			Monitor.Log ($"Spawning sea monster in location '{location.Name}' at ({x},{y}).",
				LogLevel.Trace);
			location.temporarySprites.Add (new SeaMonsterTemporarySprite
				(250f, 4, Game1.random.Next (7), 64f * new Vector2 (x, y)));
			return true;
		}
	}
}
