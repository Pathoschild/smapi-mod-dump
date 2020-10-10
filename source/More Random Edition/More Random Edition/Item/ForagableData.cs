/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System;

namespace Randomizer
{
	/// <summary>
	/// Contains one set of items and their rarity
	/// </summary>
	public class ForagableData
	{
		public int ItemId { get; }
		public double ItemRarity { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="itemId">The item Id</param>
		public ForagableData(int itemId)
		{
			ItemId = itemId;

			Random rng = Globals.RNG;
			bool useNormalDistribution = rng.Next(0, 2) == 0;
			ItemRarity = useNormalDistribution ? (double)rng.Next(4, 8) / 10 : (double)rng.Next(1, 10) / 10;
		}

		/// <summary>
		/// The string representation to be used in the locationReplacement string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{ItemId} {ItemRarity}";
		}
	}
}
