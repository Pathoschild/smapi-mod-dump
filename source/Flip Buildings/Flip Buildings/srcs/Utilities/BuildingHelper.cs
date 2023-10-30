/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using StardewValley;
using StardewValley.Buildings;

namespace FlipBuildings.Utilities
{
	internal class BuildingHelper
	{
		internal static void Update()
		{
			foreach (Building building in Game1.getFarm().buildings)
			{
				Update(building);
			}
		}

		internal static void Update(Building building)
		{
			if (CompatibilityHelper.IsSolidFoundationsLoaded && CompatibilityHelper.GenericBuildingType.IsAssignableFrom(building.GetType()))
				return;
			BluePrint bluePrint = new(building.buildingType.Value);
			if (bluePrint != null)
			{
				Reset(building);
				if (building.modData.ContainsKey(ModDataKeys.FLIPPED))
					Flip(building);
				building.updateInteriorWarps();
			}
		}

		private static void Flip(Building building)
		{
			if (building.humanDoor.X >= 0 && building.humanDoor.Y >= 0)
			{
				building.humanDoor.X = building.tilesWide.Value - building.humanDoor.X - 1;
			}
			if (building.animalDoor.X >= 0 && building.animalDoor.Y >= 0)
			{
				building.animalDoor.X = building.tilesWide.Value - building.animalDoor.X - 1 - (building is Barn ? 1 : 0);
			}
			building.updateInteriorWarps();
		}

		internal static void Reset()
		{
			foreach (Building building in Game1.getFarm().buildings)
			{
				Reset(building);
			}
		}

		internal static void Reset(Building building)
		{
			if (CompatibilityHelper.IsSolidFoundationsLoaded && CompatibilityHelper.GenericBuildingType.IsAssignableFrom(building.GetType()))
				return;
			BluePrint bluePrint = new(building.buildingType.Value);
			if (bluePrint != null)
			{
				building.humanDoor.X = bluePrint.humanDoor.X;
				building.humanDoor.Y = bluePrint.humanDoor.Y;
				building.animalDoor.X = bluePrint.animalDoor.X;
				building.animalDoor.Y = bluePrint.animalDoor.Y;
				building.updateInteriorWarps();
			}
		}
	}
}
