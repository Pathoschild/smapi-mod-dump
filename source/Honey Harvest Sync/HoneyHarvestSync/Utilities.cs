/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/voltaek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneyHarvestSync
{
	internal static class Utilities
	{
		/// <summary>
		/// Whether the given item has characteristics that identify it as a potential honey-flavor source.
		/// </summary>
		/// <param name="item">The item to check.</param>
		/// <returns>True if the item can flavor honey, False if not.</returns>
		internal static bool IsHoneyFlavorSource(Item item)
		{
			// The base game data has the flowers category and the "flower_item" tag on crop flowers and some forage flowers, but not all.
			// Better Beehouses tags the four base game forage flowers all with "honey_source".
			return item.Category == SObject.flowersCategory || item.HasContextTag("flower_item") || item.HasContextTag("honey_source") || ModEntry.Compat.IsAnythingHoney;
		}

		/// <summary>Filter to test locations with to see if they can and do have relevant bee houses in them.</summary>
		/// <param name="location">The location to test.</param>
		/// <returns>Whether the location has bee houses.</returns>
		internal static bool IsLocationWithBeeHouses(GameLocation location)
		{
			return (location.IsOutdoors || ModEntry.Compat.SyncIndoorBeeHouses) && location.Objects.Values.Any(x => x.QualifiedItemId == Constants.beeHouseQualifiedItemID);
		}

		/// <summary>
		/// Output text directly to the console - only for debug builds - using optional specific text and background colors.
		/// </summary>
		/// <param name="message">The message to output to the console. Gets no prefixing like SMAPI might do.</param>
		/// <param name="textColor">Optional. The text color to use for the message. Defaults to the 'DarkGray' that SMAPI uses for DEBUG and TRACE logs.</param>
		/// <param name="backColor">Optional. The background color to use for the message. If the default 'Black' is left or passed, will skip setting the background color.</param>
		[Conditional("DEBUG")]
		internal static void DebugConsoleLog(string message, ConsoleColor textColor = ConsoleColor.DarkGray, ConsoleColor backColor = ConsoleColor.Black)
		{
			if (backColor != ConsoleColor.Black)
			{
				Console.BackgroundColor = backColor;
			}

			Console.ForegroundColor = textColor;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		/// <summary>Checks if a given location is within the effective range of a flower.</summary>
		/// <param name="checkLocation">The tile location to check.</param>
		/// <param name="flowerLocation">The location of the flower.</param>
		/// <returns>True if the location is within range, False if not.</returns>
		internal static bool IsWithinFlowerRange(Vector2 checkLocation, Vector2 flowerLocation)
		{
			int flowerRange = ModEntry.Compat.FlowerRange;

			// Start with a quick check to see if it's in a square of the radius size since that's much faster to check
			if (!(checkLocation.X <= flowerLocation.X + flowerRange && checkLocation.X >= Math.Max(flowerLocation.X - flowerRange, 0)
				&& checkLocation.Y <= flowerLocation.Y + flowerRange && checkLocation.Y >= Math.Max(flowerLocation.Y - flowerRange, 0)))
			{
				return false;
			}

			int yCheck = 0;
			int xCheck = flowerRange;

			// This does kind of "middle out" checking of the diamond shape so we hit the horizontal rows with the most tiles first.
			// We start with the full-width middle row, then check the row above AND below that one at once, but with one less tile on each horizontal side,
			// then continue checking above and below those ones, each time checking less horizontal tiles, until we finish by checking the topmost tile and bottommost tile.
			// In testing, doing it this way takes on average about half the checks versus scanning from topmost tile down each row until bottommost tile.
			while (yCheck <= flowerRange)
			{
				if ((checkLocation.Y == flowerLocation.Y + yCheck || (yCheck != 0 && checkLocation.Y == Math.Max(flowerLocation.Y - yCheck, 0)))
					&& checkLocation.X >= Math.Max(flowerLocation.X - xCheck, 0)
					&& checkLocation.X <= flowerLocation.X + xCheck)
				{
					return true;
				}

				yCheck += 1;
				xCheck -= 1;
			}

			return false;
		}

		internal static void TestIsWithinFlowerRange(bool shouldTestDebugLocations = true, bool shouldTestRandomLocations = false)
		{
			// NOTE - If testing this function elsewhere (such as https://dotnetfiddle.net), will need to include
			// the 'MonoGame.Framework.Gtk' v3.8.0 Nuget package, add `using Microsoft.Xna.Framework;`, and set `flowerRange` to a constant value.

			int flowerRange = ModEntry.Compat.FlowerRange;

			// This location should have at least double the `flowerRange` value for both axis to not break the below debug locations.
			Vector2 flower = new(flowerRange * 2, flowerRange * 2);

			// Debug Locations - these should show whether the algorithm is working or not
			System.Collections.Generic.List<Vector2> insideDiamondLocations = new() {
				new Vector2(flower.X, flower.Y + flowerRange),
				new Vector2(flower.X, flower.Y - flowerRange),
				new Vector2(flower.X - flowerRange, flower.Y),
				new Vector2(flower.X + flowerRange, flower.Y),
				new Vector2(flower.X + flowerRange / 2, flower.Y + flowerRange / 2),
				new Vector2(flower.X - flowerRange / 2, flower.Y + flowerRange / 2),
				new Vector2(flower.X - flowerRange / 2, flower.Y - flowerRange / 2),
				new Vector2(flower.X + flowerRange / 2, flower.Y - flowerRange / 2),
				new Vector2(flower.X - 1, flower.Y + flowerRange - 1),
				new Vector2(flower.X + 1, flower.Y + flowerRange - 1),
				new Vector2(flower.X - 1, flower.Y - flowerRange + 1),
				new Vector2(flower.X + 1, flower.Y - flowerRange + 1),
				new Vector2(flower.X - flowerRange + 1, flower.Y - 1),
				new Vector2(flower.X + flowerRange - 1, flower.Y - 1),
				new Vector2(flower.X - flowerRange + 1, flower.Y + 1),
				new Vector2(flower.X + flowerRange - 1, flower.Y + 1),
			};
			System.Collections.Generic.List<Vector2> outsideDiamondInsideSquareLocations = new() {
				new Vector2(flower.X + flowerRange, flower.Y + flowerRange),
				new Vector2(flower.X - flowerRange, flower.Y + flowerRange),
				new Vector2(flower.X - flowerRange, flower.Y - flowerRange),
				new Vector2(flower.X + flowerRange, flower.Y - flowerRange),
				new Vector2(flower.X + flowerRange, flower.Y + 1),
				new Vector2(flower.X + flowerRange, flower.Y - 1),
				new Vector2(flower.X - flowerRange, flower.Y + 1),
				new Vector2(flower.X - flowerRange, flower.Y - 1),
				new Vector2(flower.X + 1, flower.Y + flowerRange),
				new Vector2(flower.X - 1, flower.Y + flowerRange),
				new Vector2(flower.X + 1, flower.Y - flowerRange),
				new Vector2(flower.X - 1, flower.Y - flowerRange),
			};
			System.Collections.Generic.List<Vector2> outsideSquareLocations = new() {
				new Vector2(flower.X, flower.Y + flowerRange + 1),
				new Vector2(flower.X, flower.Y - flowerRange - 1),
				new Vector2(flower.X - flowerRange - 1, flower.Y),
				new Vector2(flower.X + flowerRange + 1, flower.Y),
				new Vector2(flower.X + flowerRange + 1, flower.Y + flowerRange + 1),
				new Vector2(flower.X - flowerRange - 1, flower.Y + flowerRange + 1),
				new Vector2(flower.X - flowerRange - 1, flower.Y - flowerRange - 1),
				new Vector2(flower.X + flowerRange + 1, flower.Y - flowerRange - 1),
			};

			// Random Locations - these can test real-world speed differences between algorithms
			System.Collections.Generic.List<Vector2> randomLocations = new();

			// Can mess with this to test checking locations at various max distances from the flower location
			int maxDistanceAway = flowerRange * 2;

			int minX = Math.Max(Convert.ToInt32(flower.X) - maxDistanceAway, 0);
			int maxX = Convert.ToInt32(flower.X) + maxDistanceAway;
			int minY = Math.Max(Convert.ToInt32(flower.Y) - maxDistanceAway, 0);
			int maxY = Convert.ToInt32(flower.Y) + maxDistanceAway;
			Random rand = new();

			for (int i = 0; i < 50; i++)
			{
				randomLocations.Add(new Vector2(rand.Next(minX, maxX + 1), rand.Next(minY, maxY + 1)));
			}

			System.Collections.Generic.List<string> fails = new();

			System.Collections.Generic.List<string> ins = new();
			System.Collections.Generic.List<string> outs = new();

			// TESTING STARTS

			System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

			if (shouldTestDebugLocations)
			{
				Console.WriteLine("DEBUG LOCATIONS\n-- Within Diamond --");
				foreach (Vector2 test in insideDiamondLocations)
				{
					bool result = IsWithinFlowerRange(test, flower);

					if (!result)
					{
						fails.Add($"{test}");
					}
				}
				if (fails.Count > 0)
				{
					Console.WriteLine($"FAILS: {String.Join(" | ", fails)}");
					fails.Clear();
				}

				Console.WriteLine("\n-- Outside Diamond, but Inside Square --");
				foreach (Vector2 test in outsideDiamondInsideSquareLocations)
				{
					bool result = IsWithinFlowerRange(test, flower);

					if (result)
					{
						fails.Add($"{test}");
					}
				}
				if (fails.Count > 0)
				{
					Console.WriteLine($"FAILS: {String.Join(" | ", fails)}");
					fails.Clear();
				}

				Console.WriteLine("\n-- Outside Square --");
				foreach (Vector2 test in outsideSquareLocations)
				{
					bool result = IsWithinFlowerRange(test, flower);

					if (result)
					{
						fails.Add($"{test}");
					}
				}
				if (fails.Count > 0)
				{
					Console.WriteLine($"FAILS: {String.Join(" | ", fails)}");
					fails.Clear();
				}

				sw.Stop();
				Console.WriteLine($"\nTested {insideDiamondLocations.Count + outsideDiamondInsideSquareLocations.Count + outsideSquareLocations.Count} locations "
					+ $"in {sw.ElapsedTicks} ticks ({sw.ElapsedMilliseconds}ms)");
			}

			if (shouldTestRandomLocations)
			{
				if (shouldTestDebugLocations)
				{
					sw.Start();
				}

				Console.WriteLine("\n\nRANDOMLY GENERATED LOCATIONS");
				foreach (Vector2 test in randomLocations)
				{
					bool result = IsWithinFlowerRange(test, flower);

					if (result)
					{
						ins.Add($"{test}");
					}
					else
					{
						outs.Add($"{test}");
					}
				}
				sw.Stop();

				Console.WriteLine($"Tested {randomLocations.Count} randomly generated locations in {sw.ElapsedTicks} ticks ({sw.ElapsedMilliseconds}ms)");
				Console.WriteLine($"Ins: {ins.Count} | Outs: {outs.Count}");
			}
		}
	}
}
