/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using System.Reflection;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace FlipBuildings.Utilities
{
	internal class GenericBuildingHelper
	{
		internal static void Update()
		{
			foreach (Building building in Game1.getFarm().buildings)
			{
				Update(building);
			}
		}

		internal static void Update(object building)
		{
			if (!CompatibilityHelper.IsSolidFoundationsLoaded || !CompatibilityHelper.GenericBuildingType.IsAssignableFrom(building.GetType()))
				return;
			Building buildingAsBuilding = building as Building;
			Point animalDoor = ((Rectangle)CompatibilityHelper.BuildingDataType.GetMethod("GetAnimalDoorRect").Invoke(building.GetType().GetProperty("Model").GetValue(building), new object[] {})).Location;
			if (!buildingAsBuilding.modData.ContainsKey(ModDataKeys.FLIPPED))
				buildingAsBuilding.animalDoor.Value = animalDoor;
			else
				buildingAsBuilding.animalDoor.Value = new Point(animalDoor.X * -1 + buildingAsBuilding.tilesWide.Value - 1, animalDoor.Y);
			CompatibilityHelper.GenericBuildingType.GetMethod("ResetLights", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(building, new object[] {});
			buildingAsBuilding.updateInteriorWarps();
		}
	}
}
