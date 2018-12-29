using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elevator
{
	class ModEntry : Mod
	{
		public static Texture2D ElevatorBuildingTexture { get; private set; }

		public override void Entry(IModHelper helper)
		{
			ElevatorBuildingTexture = helper.Content.Load<Texture2D>("Hotel.png");//Must be before PatchAll
			
			//Harmony patch everything
			Patch.PatchAll("me.ilyaki.elevator");

			//Commands
			helper.ConsoleCommands.Add("elevator_goto", "Warp to cabin. first arg is name of player", this.Goto);
			helper.ConsoleCommands.Add("elevator_relocateCabin", "Move cabin off the map and mark it for use by the elevator/hotel. first arg is name of player", this.MoveCabinFarAway);
			helper.ConsoleCommands.Add("elevator_addCabin", "Builds a cabin outside the map for elevator/hotel", this.AddNewCabin);
			helper.ConsoleCommands.Add("elevator_bringBackCabin", "Moves a cabin to your location, so it will not be used by the elevator. First arg is name of player", this.BringBackCabin);
			helper.ConsoleCommands.Add("elevator_makeBuildingHere", "Spawn an elevator building on top of you", (o, e) => { if (Context.IsMainPlayer) CabinHelper.SpawnElevatorBuilding(); });
			helper.ConsoleCommands.Add("elevator_removeUnusedElevatorCabins", "Remove any empty cabins inside the elevator system.", this.RemoveEmptyElevatorCabins);


			//Events
			Helper.Events.GameLoop.DayStarted += (o, e) => UpdateWarpsAndReloadTextures();


			Helper.Events.Input.ButtonPressed += (o,e) =>
			{
				if (Game1.IsServer && e.Button.TryGetKeyboard(out Keys key) && key == Keys.F7)
					UpdateWarpsAndReloadTextures();
			};
			
			Helper.Events.Player.Warped += (o, e) =>
			{
				if (Game1.player.getTileX() <= -9000)//The door position is a bit to the right of -10000
				{
					//Something broke: this seems to happen when a cabin has been upgraded
					var d = CabinHelper.GetDoorPositionOfFirstElevatorBuilding();
					Game1.player.setTileLocation(new Microsoft.Xna.Framework.Vector2(d.X, d.Y));
				}
			};
		}

		private void UpdateWarpsAndReloadTextures()
		{
			UpdateCabinWarps();

			foreach (Building building in Game1.getFarm().buildings)
				if (CabinHelper.IsElevatorBuilding(building))
				{
					Monitor.Log("(Re)loading an elevator building texture");
					building.resetTexture();//Otherwise the clients will just see a shed
				}
		}

		private void UpdateCabinWarps()
		{
			foreach (Warp warp in CabinHelper.GetCabinsOutsides().Where(x => x.tileX.Value <= -10000).SelectMany(x => x.indoors.Value.warps))
			{
				var d = CabinHelper.GetDoorPositionOfFirstElevatorBuilding();
				warp.TargetX = d.X;
				warp.TargetY = d.Y;
			}
		}

		private void Goto(string command, string[] args)
		{
			if (!Context.IsMainPlayer)
			{
				Monitor.Log("You must be the server to do that");
				return;
			}


			if (args.Length < 1)
				return;

			string farmerName = args[0];
			Monitor.Log($"Attempting to warp to {farmerName}'s house");

			foreach (Farmer player in Game1.getAllFarmers())
			{
				if (player.Name.ToLower() == farmerName.ToLower())
				{
					Console.WriteLine($"Found a match: {player.Name}, ID={player.UniqueMultiplayerID}");
					Cabin cabin = CabinHelper.FindCabinInside(player);

					Game1.warpFarmer(cabin.uniqueName.Value, cabin.warps[0].X, cabin.warps[0].Y - 1, 0);//Does this work when the cabin is upgraded?
					return;
				}
			}
		}

		private void MoveCabinFarAway(string command, string[] args)
		{
			if (!Context.IsMainPlayer)
			{
				Monitor.Log("You must be the server to do that");
				return;
			}

			if (args.Length < 1)
				return;

			string farmerName = args[0];
			Monitor.Log($"Attempting to move {farmerName}'s house (very) far away");

			foreach (Farmer player in Game1.getAllFarmers())
			{
				if (player.Name.ToLower() == farmerName.ToLower())
				{
					Monitor.Log($"Found a match: {player.Name}, ID={player.UniqueMultiplayerID}");
					
					Building targetBuilding = CabinHelper.FindCabinOutside(player);
					Cabin cabin = CabinHelper.FindCabinInside(player);
					if (cabin != null && targetBuilding != null)
					{

						targetBuilding.GetType()
						   .GetField("tileX", BindingFlags.Instance | BindingFlags.Public)//tileX is readonly
						   .SetValue(targetBuilding, new NetInt(-10000));

						foreach (var warp in cabin.warps)
						{
							var d = CabinHelper.GetDoorPositionOfFirstElevatorBuilding();
							warp.TargetX = d.X;
							warp.TargetY = d.Y;
						}

						Monitor.Log($"Deleted the house (technically moved it out of bounds)");
						return;
					}else
					{
						Monitor.Log("Could not find the cabin");
					}
				}
			}
		}

		private void BringBackCabin(string command, string[] args)
		{
			if (!Context.IsMainPlayer)
			{
				Monitor.Log("You must be the server to do that");
				return;
			}

			if (args.Length < 1)
			{
				Monitor.Log("Which player's house?");
				return;
			}

			if (Game1.getFarm() != Game1.player.currentLocation)
			{
				Monitor.Log("You need to be on the farm");
				return;
			}

			string farmerName = args[0];
			Monitor.Log($"Attempting to move {farmerName}'s house back");

			foreach (Farmer player in Game1.getAllFarmers())
			{
				if (player.Name.ToLower() == farmerName.ToLower())
				{
					Monitor.Log($"Found a match: {player.Name}, ID={player.UniqueMultiplayerID}");

					Building targetBuilding = CabinHelper.FindCabinOutside(player);
					Cabin cabin = CabinHelper.FindCabinInside(player);
					if (cabin != null && targetBuilding != null)
					{

						targetBuilding.GetType()
						   .GetField("tileX", BindingFlags.Instance | BindingFlags.Public)//tileX is readonly
						   .SetValue(targetBuilding, new NetInt(Game1.player.getTileX()));

						targetBuilding.GetType()
						   .GetField("tileY", BindingFlags.Instance | BindingFlags.Public)//tileY is readonly
						   .SetValue(targetBuilding, new NetInt(Game1.player.getTileY() - 1));

						foreach (var warp in cabin.warps)
						{
							warp.TargetX = targetBuilding.humanDoor.X + (int)targetBuilding.tileX.Value;
							warp.TargetY = targetBuilding.humanDoor.Y + (int)targetBuilding.tileY.Value + 1;
						}

						Monitor.Log($"Moved it");
					}
					else
					{
						Monitor.Log("Could not find the cabin");
					}
				}
			}
		}
		
		private void AddNewCabin(string command, string[] args)
		{
			if (!Context.IsMainPlayer)
			{
				Monitor.Log("You must be the server to do that");
				return;
			}

			CabinHelper.AddNewCabin();
			Monitor.Log("Added a new cabin");
		}

		private void RemoveEmptyElevatorCabins(string command, string[] args)
		{
			if (!Context.IsMainPlayer)
			{
				Monitor.Log("You must be the server to do that");
				return;
			}

			var toRemove = new List<Building>();

			foreach(Cabin cabin in CabinHelper.GetCabinsInsides())
			{
				if (cabin.getFarmhand().Value.Name.Length == 0)
				{
					toRemove.Add(CabinHelper.FindCabinOutside(cabin.getFarmhand()));
					
				}
			}

			foreach (Building building in toRemove)
				Game1.getFarm().buildings.Remove(building);

			Monitor.Log($"Removed {toRemove.Count} unused cabins");
		}
	}
}
