using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Elevator
{
	static class CabinHelper
	{
		public static Cabin FindCabinInside(Farmer farmhand)
		{
			foreach (Cabin item in GetCabinsInsides())
			{
				if (item.getFarmhand().Value.UniqueMultiplayerID == farmhand.UniqueMultiplayerID)
				{
					return item;
				}
			}
			return null;
		}

		public static Building FindCabinOutside(Farmer farmhand)
		{
			foreach (Building item in GetCabinsOutsides())
			{
				if ((item.indoors.Value as Cabin)?.getFarmhand().Value.UniqueMultiplayerID == farmhand.UniqueMultiplayerID)
					return item;
			}
			return null;
		}

		public static IEnumerable<Cabin> GetCabinsInsides()
		{
			foreach (Building building in GetCabinsOutsides())
			{
				yield return building.indoors.Value as Cabin;
			}
		}

		public static IEnumerable<Building> GetCabinsOutsides()
		{
			if (Game1.getFarm() != null)
			{
				foreach (Building building in Game1.getFarm().buildings)
				{
					if ((int)building.daysOfConstructionLeft.Value <= 0 && building.indoors.Value is Cabin) 
					{
						yield return building;
					}
				}
			}
		}

		public static void AddNewCabin(int type = 3)
		{
			//"Stone Cabin"/"Plank Cabin"/"Log Cabin"
			var blueprint = new BluePrint(type == 1 ? "Stone Cabin" : type == 2 ? "Plank Cabin" : "Log Cabin");
			var building = new Building(blueprint, new Vector2(-10000, 0));
			Game1.getFarm().buildings.Add(building);
			

			foreach (var warp in building.indoors.Value.warps)
			{
				var d = GetDoorPositionOfFirstElevatorBuilding();
				warp.TargetX = d.X;
				warp.TargetY = d.Y;
			}
		}

		public static void SpawnElevatorBuilding()
		{
			var blueprint = new BluePrint("Shed")
			{
				daysToConstruct = 0,
				magical = true,

				tilesWidth = ModEntry.ElevatorBuildingTexture.Width / 16
			};

			var building = new Building(blueprint, new Vector2(Game1.player.getTileX(), Game1.player.getTileY()));

			//Use this to set it apart from an actual shed (UPDATE: see IsElevatorBuilding instead)
			building.indoors.Value.GetType()
					.GetField("uniqueName", BindingFlags.Instance | BindingFlags.Public)//readonly
					.SetValue(building.indoors.Value, new NetString("ElevatorBuilding"));//Don't set this in the blueprint or it will try to load an XNB "ElevatorBuilding"

			//building.humanDoor
			building.GetType()
					.GetField("humanDoor", BindingFlags.Instance | BindingFlags.Public)//readonly
					.SetValue(building, new NetPoint(new Point(building.humanDoor.Value.X + 2, building.humanDoor.Value.Y)));

			building.resetTexture();

			Game1.getFarm().buildings.Add(building);			
		}

		public static bool IsElevatorBuilding(Building building)
		{
			return (building.nameOfIndoors == "ElevatorBuilding") || (building.indoors.Value is Shed && building.tilesWide.Value == ModEntry.ElevatorBuildingTexture.Width / 16);
		}

		public static Point GetDoorPositionOfFirstElevatorBuilding()
		{
			foreach (Building building in Game1.getFarm().buildings)
			{
				if (IsElevatorBuilding(building))
				{
					return new Point(building.tileX.Value + 5, building.tileY.Value + 3);
				}
			}

			return Point.Zero;
		}

	}
}
