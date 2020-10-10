/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/Elevator
**
*************************************************/

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Network;
using System;

namespace Elevator
{
	class PlayerCreateListener : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Multiplayer), "addPlayer");

		public static bool Prefix(NetFarmerRoot f)
		{
			if (f.Value.Name.Length == 0)
			{
				//Don't create a cabin unless there is an elevator building
				bool elevatorBuildingExists = false;
				foreach (Building building in Game1.getFarm().buildings)
				{
					if (CabinHelper.IsElevatorBuilding(building))
					{
						elevatorBuildingExists = true;
						break;
					}
				}
				if (!elevatorBuildingExists)
					return true;

				//A new player has joined. If there is less then 10 spots availible, mark up until 10
				int emptyPlaces = 0;
				foreach (Farmer player in Game1.getAllFarmhands())
					if (player.Name.Length == 0)
						emptyPlaces++;
              
				Console.WriteLine($"Generating {10 - emptyPlaces} new cabins");

				if (emptyPlaces < 10)
					for (int i = 0; i <= 10 - emptyPlaces; i++)//Make up to 10
						CabinHelper.AddNewCabin(Game1.random.Next(1, 4));
			}

			return true;
		}
	}
}
