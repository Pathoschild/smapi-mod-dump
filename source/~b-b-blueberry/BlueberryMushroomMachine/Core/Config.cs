/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace BlueberryMushroomMachine
{
	public class Config
	{
		public bool DisabledForFruitCave { get; set; } = true;
		public bool RecipeAlwaysAvailable { get; set; } = false;
		public int MaximumDaysToMature { get; set; } = 4;
		public bool MaximumQuantityLimitsDoubled { get; set; } = false;
		public bool OnlyToolsCanRemoveRootMushrooms = false;
		public bool PulseWhenGrowing = true;
		public List<string> OtherObjectsThatCanBeGrown = new List<string>
		{
			"Example Mushroom Name",
			"Example Item Not Called Fungus",
		};

		public bool WorksInCellar { get; set; } = true;
		public bool WorksInFarmCave { get; set; } = true;
		public bool WorksInBuildings { get; set; } = false;
		public bool WorksInFarmHouse { get; set; } = false;
		public bool WorksInGreenhouse { get; set; } = false;
		public bool WorksOutdoors { get; set; } = false;

		public bool DebugMode { get; set; } = false;
		public SButton DebugGivePropagatorKey { get; set; } = SButton.OemComma;
	}
}