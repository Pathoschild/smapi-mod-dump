using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtifactSystemFixed
{
	public static class Utils
	{
		public static T ChooseWeightedProbability<T>(Dictionary<T, double> items, Random random, T otherwise = default(T))
		{
			double randomNum = random.NextDouble() * items.Values.Sum();// [0, sum)

			double iSum = 0;
			foreach (var pair in items)
			{
				iSum += pair.Value;
				if (iSum >= randomNum) return pair.Key;
			}

			return otherwise;
		}

		//Returns number FOUND - ie picked up (not donated)
		public static int GetNumberOfArtifactFound(int artifactItemID)
		{
			var arch = Game1.player.archaeologyFound;

			if (arch.ContainsKey(artifactItemID))
			{
				if (arch.TryGetValue(artifactItemID, out int[] value))
				{
					//The int[] seems to just be always 2 in length, with each value equal to number of times found
					return value[0];
				}
			}
			return 0;
		}

		//Returns number FOUND - ie won from geodes (not donated)
		public static int GetNumberOfMineralFound(int mineralID, Farmer player = null)
		{
			player = player ?? Game1.player;

			if (player.mineralsFound != null && player.mineralsFound.ContainsKey(mineralID))
				return player.mineralsFound[mineralID];

			return 0;
		}
	}
}
