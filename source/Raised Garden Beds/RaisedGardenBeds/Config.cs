/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/RaisedGardenBeds
**
*************************************************/

namespace RaisedGardenBeds
{
	public class Config
	{
		public bool RaisedBedsMayBreakWithAge { get; set; } = true;
		public bool SprinklersEnabled { get; set; } = false;
		public bool RecipesAlwaysAvailable { get; set; } = false;
		public bool CanBePlacedInFarmHouse { get; set; } = false;
		public bool CanBePlacedInBuildings { get; set; } = false;
		public bool CanBePlacedInGreenHouse { get; set; } = true;
	}
}
