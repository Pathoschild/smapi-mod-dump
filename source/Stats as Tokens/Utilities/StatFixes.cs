/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/StatsAsTokens
**
*************************************************/

// Copyright (C) 2021 Vertigon
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

using StardewValley;

namespace StatsAsTokens
{
	internal class StatFixes
	{
		public static void IncrementAnimalProduceStat(int produce, FarmAnimal animal)
		{
			Globals.Monitor.Log($"Animal product: {produce}");

			switch (produce)
			{
				// don't need to handle products for animal with harvest type 1
				case 430 or 436 or 184 or <= 0:
					break;

				case 174 or 176 or 180 or 182:
					Game1.stats.ChickenEggsLayed++;
					break;

				// sheep wool stat is incremented when shearing sheep
				case 440:
					if (animal.displayType.Contains("Rabbit"))
					{
						Game1.stats.RabbitWoolProduced++;
					}
					break;

				case 442:
					Game1.stats.DuckEggsLayed++;
					break;

				// Duck Feathers
				case 444:
					if (Game1.stats.stat_dictionary.ContainsKey("duckFeathersDropped"))
					{
						Game1.stats.stat_dictionary["duckFeathersDropped"]++;
					}
					else
					{
						Game1.stats.stat_dictionary["duckFeathersDropped"] = 1;
					}
					break;

				// Ostrich Eggs
				case 289:
					if (Game1.stats.stat_dictionary.ContainsKey("ostrichEggsLayed"))
					{
						Game1.stats.stat_dictionary["ostrichEggsLayed"]++;
					}
					else
					{
						Game1.stats.stat_dictionary["ostrichEggsLayed"] = 1;
					}
					break;

				// Dinosaur Eggs
				case 107:
					if (Game1.stats.stat_dictionary.ContainsKey("dinosaurEggsLayed"))
					{
						Game1.stats.stat_dictionary["dinosaurEggsLayed"]++;
					}
					else
					{
						Game1.stats.stat_dictionary["dinosaurEggsLayed"] += 1;
					}
					break;

				// Rabbit's Foot
				case 446:
					if (Game1.stats.stat_dictionary.ContainsKey("rabbitsFeetDropped"))
					{
						Game1.stats.stat_dictionary["rabbitsFeetDropped"]++;
					}
					else
					{
						Game1.stats.stat_dictionary["rabbitsFeetDropped"] = 1;
					}
					break;

				default:
					Globals.Monitor.Log($"Attempted to record unknown animal produce: {produce}");
					break;
			}
		}
	}
}
