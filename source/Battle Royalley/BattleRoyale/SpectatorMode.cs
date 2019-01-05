using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
    class SpectatorMode
	{
        //Each location UI component on the map has a myID field
		private static readonly Dictionary<int, string> pointIDsToLocation = new Dictionary<int, string>()
		{
			{ -500, "Beach" },
			{ 1001 ,"Desert" },
			{ 1002 ,"Farm" },
			{ 1003 ,"Backwoods" },
			{ 1004 ,"BusStop" },
			{ 1005 ,"WizardHouse" },
			{ 1006 ,"AnimalShop" },
			{ 1007 ,"LeahHouse" },
			{ 1008 ,"SamHouse" },
			{ 1009 ,"HaleyHouse" },
			{ 1010 ,"Town" },
			{ 1011 ,"Hospital" },
			{ 1012 ,"SeedShop" },
			{ 1013 ,"Blacksmith" },
			{ 1014 ,"Saloon" },
			{ 1015 ,"ManorHouse" },
			{ 1016 ,"ArchaeologyHouse" },
			{ 1017 ,"ElliottHouse" },
			{ 1018 ,"Sewer" },
			{ 1020 ,"Trailer" },
			{ 1021 ,"JoshHouse" },
			{ 1022 ,"ScienceHouse" },
			{ 1023 ,"Tent" },
			{ 1024 ,"Mine" },
			{ 1025 ,"AdventureGuild" },
			{ 1026, "Mountain" },
			{ 1027 ,"JojaMart" },
			{ 1028 ,"FishShop" },
			{ 1029 ,"Railroad" },
			{ 1030 ,"Woods" },
			{ 1031, "Forest" },
			{ 1032 ,"CommunityCenter" },
			{ 1033 ,"Sewer" },
			{ 1034 ,"Railroad" }
		};
		public static bool InSpectatorMode { get; private set; } = false;

		public static void EnterSpectatorMode(GameLocation gameLocation = null, xTile.Dimensions.Location? targetLocation = null)
		{
			InSpectatorMode = true;
			SendSpectatorToLocation(gameLocation ?? Game1.getFarm(), targetLocation);

			Game1.displayFarmer = false;
			Game1.viewportFreeze = true;
			Game1.displayHUD = false;
		}

		private static GameLocation tempTargetLocation = null;
		private static GameLocation lastWarpLocation = null;
		public static void SendSpectatorToLocation(GameLocation gameLocation, xTile.Dimensions.Location? targetLocation = null)
		{
			if (InSpectatorMode)
			{
				tempTargetLocation = gameLocation;
				lastWarpLocation = gameLocation;

				Game1.player.currentLocation.cleanupBeforePlayerExit();
				
				Game1.currentLocation = gameLocation;
				if (!Game1.IsServer)
					Game1.player.currentLocation = gameLocation;

				gameLocation.resetForPlayerEntry();


				int locationWidth = gameLocation.Map.Layers[0].LayerWidth * Game1.tileSize;
				int locationHeight = gameLocation.Map.Layers[0].LayerHeight * Game1.tileSize;
				Game1.viewport.Location = targetLocation ?? new xTile.Dimensions.Location(locationWidth / 2 - Game1.viewport.Width/2, 
																						locationHeight / 2 - Game1.viewport.Height / 2);

				Game1.panScreen(0, 0);

				Game1.player.Position = new Microsoft.Xna.Framework.Vector2(-10000, -10000);//So they don't accidentally show up
			}
			else
			{
				Console.WriteLine("Could not move spectator to new location because not in spectator mode");
			}
		}

		public static bool OnClickMapPoint(ClickableComponent point)
		{
			int id = point.myID;
			if (pointIDsToLocation.TryGetValue(id, out string locationName))
			{
				SendSpectatorToLocation(Game1.getLocationFromName(locationName));
				return true;
			}

			return false;
		}

		static int currentIndex = 0;
		static int timer = 0;

		public static void Update()
		{
			if (InSpectatorMode)
			{
				#region Camera movement
				float speed = 8;
				float x = 0;
				float y = 0;

				y += Game1.options.moveDownButton.Where(a => Keyboard.GetState().GetPressedKeys().Contains(a.key)).Count();
				x += Game1.options.moveRightButton.Where(a => Keyboard.GetState().GetPressedKeys().Contains(a.key)).Count();
				y -= Game1.options.moveUpButton.Where(a => Keyboard.GetState().GetPressedKeys().Contains(a.key)).Count();
				x -= Game1.options.moveLeftButton.Where(a => Keyboard.GetState().GetPressedKeys().Contains(a.key)).Count();

				var stick = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).ThumbSticks.Left;
				x += stick.X;
				y -= stick.Y;



				int mouseX = Game1.getOldMouseX() + Game1.viewport.X;
				int mouseY = Game1.getOldMouseY() + Game1.viewport.Y;
				if (mouseX - Game1.viewport.X < 64)
					x -= 1;
				else if (mouseX - (Game1.viewport.X + Game1.viewport.Width) >= -128)
					x += 1;
				if (mouseY - Game1.viewport.Y < 64)
					y -= 1;
				else if (mouseY - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
					y += 1;
				
				x *= speed;
				y *= speed;
				

				if (Game1.game1.IsActive && Game1.activeClickableMenu == null)
					Game1.panScreen((int)x, (int)y);
				#endregion

				timer--;

				if (Game1.activeClickableMenu == null && timer <= 0 && Game1.game1.IsActive && (Mouse.GetState().LeftButton == ButtonState.Pressed
								|| GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).IsButtonDown(Buttons.RightShoulder)))
				{
					currentIndex++;

					Console.WriteLine($"Next player spectate");
				}
				else if (Game1.activeClickableMenu == null && timer <= 0 && Game1.game1.IsActive && (Mouse.GetState().RightButton == ButtonState.Pressed
					|| GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).IsButtonDown(Buttons.LeftShoulder)))
				{
					currentIndex--;
					Console.WriteLine("Previous player spectate");
				}
				else
				{
					return;
				}

				timer = 30;
				var alivePlayers = ModEntry.BRGame.alivePlayers.Where(a => Game1.getFarmer(a).Position.X >= -1000).ToList();
				
			
				if (alivePlayers.Count != 0)
					currentIndex = currentIndex % alivePlayers.Count;
				if (currentIndex < 0)
					currentIndex += alivePlayers.Count;

				if (alivePlayers.Count > 0)
				{
					var warpTarget = alivePlayers[currentIndex];
					var farmer = Game1.getFarmer(warpTarget);
					if (farmer != null)
					{
						SendSpectatorToLocation(farmer.currentLocation,
							new xTile.Dimensions.Location(
								(int)farmer.Position.X - Game1.viewport.Width / 2,
								(int)farmer.Position.Y - Game1.viewport.Height / 2));
					}
				}
				
			}
		}

		public static void OnWarped(LocationRequest locationRequest)
		{
			if (InSpectatorMode)
			{
				if (Game1.currentLocation == locationRequest.Location)
				{
					Console.WriteLine("Detected warp to current location");
					lastWarpLocation.cleanupBeforePlayerExit();
					lastWarpLocation.resetForPlayerEntry();
				}

				Console.WriteLine("Exiting spectator mode");
				InSpectatorMode = false;
				Game1.displayHUD = true;
			}
		}
	}
}
