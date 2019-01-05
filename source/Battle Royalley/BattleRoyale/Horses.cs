using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
	class Horses
	{
		private const double horseChance = 0.2;

		//(Chest chest, GameLocation location, Vector2 tile)

		private readonly List<Tuple<Chest, GameLocation, Vector2>> unopenedChests;

		public Horses(List<Tuple<Chest, GameLocation, Vector2>> generatedChests)
		{
			unopenedChests = generatedChests;
		}

		public void Update(object o, object e)
		{
			var opened = unopenedChests.Where(c => c.Item1.mutex.IsLocked() || c.Item1.mutex.IsLockHeld()).ToArray();
			foreach (var l in opened)
			{
				var location = l.Item2;
				var chest = l.Item1;
				var tile = l.Item3;

				Console.WriteLine($"Opened chest, is outdoors = {location.IsOutdoors}");
				unopenedChests.Remove(l);
				if (location.IsOutdoors && Game1.random.NextDouble() < horseChance)
				{
					Console.WriteLine($"Spawning horse at {location.Name} @ ({tile.X}, {tile.Y})");

					var horse = new StardewValley.Characters.Horse(Guid.NewGuid(), (int)tile.X, (int)tile.Y);
					location.addCharacter(horse);
				}
			}

			if (unopenedChests.Count == 0)
			{
				ModEntry.Events.GameLoop.UpdateTicked -= Update;
			}
		}
	}
}
