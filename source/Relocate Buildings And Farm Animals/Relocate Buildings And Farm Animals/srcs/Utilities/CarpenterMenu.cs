/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/RelocateFarmAnimals
**
*************************************************/

using StardewModdingAPI.Utilities;
using StardewValley;

namespace RelocateBuildingsAndFarmAnimals.Utilities
{
	internal class CarpenterMenuUtility
	{
		private static readonly PerScreen<GameLocation> mainTargetLocation = new();

		public static GameLocation MainTargetLocation
		{
			get => mainTargetLocation.Value;
			set => mainTargetLocation.Value = value;
		}
	}
}
