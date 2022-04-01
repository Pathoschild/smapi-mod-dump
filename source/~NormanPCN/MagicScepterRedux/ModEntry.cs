/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Tools;

namespace MagicScepterRedux	
{
	public class ModEntry : Mod
	{
		private List<Building> buildings;

//		private List<MiniObeliskObject> miniObelisks;

		public override void Entry(IModHelper helper)
		{
			helper.Events.Input.ButtonPressed += OnButtonPressed;
		}

		private static bool InOnScreenMenu(ICursorPosition cursor)
        {
			bool save = Game1.uiMode;
			Game1.uiMode = true;
			Vector2 v = cursor.GetScaledScreenPixels();
			Game1.uiMode = save;
			int x = (int)v.X;
			int y = (int)v.Y;
			for (int i = 0; i < Game1.onScreenMenus.Count; i++)
			{
				if (Game1.onScreenMenus[i].isWithinBounds(x, y))
				{
					return true;
				}
			}
			return false;
        }

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			Farmer player = Game1.player;
			if (
				Context.IsWorldReady &&
				(Game1.activeClickableMenu == null) &&
				player.IsLocalPlayer &&
				(player.CurrentTool is Wand) &&
				!player.isRidingHorse() &&
				!player.bathingClothes.Value &&
				SButtonExtensions.IsUseToolButton(e.Button) &&
				!InOnScreenMenu(e.Cursor)
			   )
			{
				ChooseWarpLocation();
			}
		}

		private void ChooseWarpLocation()
		{
			buildings = new List<Building>();
			List<Response> responses = new List<Response>();

			responses.Add(new Response(WarpLocationChoice.Farm.ToString(), "Farm"));
			foreach (Building building in Game1.getFarm().buildings)
			{
				switch ((string)building.buildingType.Value)
				{
				case "Water Obelisk":
					responses.Add(new Response(WarpLocationChoice.Beach.ToString(), "Beach"));
					AddBuilding(building);
					break;
				case "Earth Obelisk":
					responses.Add(new Response(WarpLocationChoice.Mountain.ToString(), "Mountain"));
					AddBuilding(building);
					break;
				case "Desert Obelisk":
					responses.Add(new Response(WarpLocationChoice.Desert.ToString(), "Desert"));
					AddBuilding(building);
					break;
				case "Island Obelisk":
					responses.Add(new Response(WarpLocationChoice.Island.ToString(), "Ginger Island"));
					AddBuilding(building);
					if ((bool)(Game1.locations.First((GameLocation loc) => (string)loc.Name == "IslandWest") as IslandWest)?.farmObelisk.Value)
					{
						responses.Add(new Response(WarpLocationChoice.IslandFarm.ToString(), "Ginger Island Farm"));
					}
					break;
				case "Woods Obelisk":
					responses.Add(new Response(WarpLocationChoice.DeepWoods.ToString(), "Deep Woods"));
					AddBuilding(building);
					break;
				}
			}

			//Vector2 obelisk1 = Vector2.Zero;
			//Vector2 obelisk2 = Vector2.Zero;
			//foreach (KeyValuePair<Vector2, StardewValley.Object> obj in Game1.player.currentLocation.objects.Pairs)
			//{
			//	if (obj.Value.bigCraftable.Value && (obj.Value.ParentSheetIndex == 238))
			//	{
			//		if (obelisk1.Equals(Vector2.Zero))
			//		{
			//			obelisk1 = obj.Key;
			//		}
			//		else if (obelisk2.Equals(Vector2.Zero))
			//		{
			//			obelisk2 = obj.Key;
			//			break;
			//		}
			//	}
			//}
			//if (!obelisk2.Equals(Vector2.Zero))
			//{
			//}

		    responses.Add(new Response(WarpLocationChoice.None.ToString(), "Cancel"));

			if (responses.Count == 2)
			{
				WarpLocation to = WarpLocations.GetWarpLocation(WarpLocationChoice.Farm);
				WarpPlayerTo(to);
			}
			else
			{
				Game1.currentLocation.createQuestionDialogue("Choose location:", responses.ToArray(), LocationAnswer);
			}
		}

		private void LocationAnswer(Farmer farmer, string answer)
		{
			Enum.TryParse<WarpLocationChoice>(answer, out var choice);
			if (choice == WarpLocationChoice.None)
			{
				return;
			}
			if (choice == WarpLocationChoice.Farm || choice == WarpLocationChoice.IslandFarm)
			{
				WarpLocation wlocation = WarpLocations.GetWarpLocation(choice);
				WarpPlayerTo(wlocation);
				return;
			}
			Building building = buildings.FirstOrDefault((Building b) => GetWarpLocationChoiceForBuildingType(b.buildingType.Value) == choice);
			building?.doAction(new Vector2((int)building.tileX.Value, (int)building.tileY.Value), farmer);
		}

		//private void MiniObeliskAnswer(Farmer farmer, string answer)
		//{
		//	MiniObeliskObject miniObelisk = miniObelisks.First((MiniObeliskObject o) => o.Name.Equals(answer));
		//	Vector2? warpLocationCoords = GetValidWarpTile(Game1.getFarm(), miniObelisk.CoordX, miniObelisk.CoordY);
		//	if (!warpLocationCoords.HasValue)
		//	{
		//		Game1.showRedMessage("Invalid Mini-Obelisk Warp Target Location");
		//		return;
		//	}
		//	WarpLocation warpLocation = new WarpLocation("Farm", (int)warpLocationCoords.Value.X, (int)warpLocationCoords.Value.Y);
		//	WarpPlayerTo(warpLocation);
		//}

		private void WarpPlayerTo(WarpLocation warpLocation)
		{
			DoBeforeWarpAnimation();
			DelayedAction.fadeAfterDelay(delegate{DoWarp(warpLocation);}, 1000);
			DoAfterWarpAnimation();
		}

		private void DoWarp(WarpLocation warpLocation)
		{
			Game1.warpFarmer(warpLocation.Name, warpLocation.CoordX, warpLocation.CoordY, flip: false);
			if (!Game1.isStartingToGetDarkOut())
			{
				Game1.playMorningSong();
			}
			else
			{
				Game1.changeMusicTrack("none");
			}
			Game1.fadeToBlackAlpha = 0.99f;
			Game1.screenGlow = false;
			Game1.player.temporarilyInvincible = false;
			Game1.player.temporaryInvincibilityTimer = 0;
			Game1.displayFarmer = true;
		}

		private void DoBeforeWarpAnimation()
		{
			for (int index = 0; index < 12; index++)
			{
				Game1.player.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next((int)Game1.player.position.X - 256, (int)Game1.player.position.X + 192), Game1.random.Next((int)Game1.player.position.Y - 256, (int)Game1.player.position.Y + 192)), flicker: false, Game1.random.NextDouble() < 0.5));
			}
			Game1.playSound("wand");
			Game1.displayFarmer = false;
			Game1.player.temporarilyInvincible = true;
			Game1.player.temporaryInvincibilityTimer = -2000;
			Game1.player.Halt();
			Game1.player.faceDirection(2);
			Game1.player.freezePause = 1000;
			Game1.flashAlpha = 1f;
		}

		private void DoAfterWarpAnimation()
		{
			new Rectangle(Game1.player.GetBoundingBox().X, Game1.player.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
			int num1 = 0;
			for (int index = Game1.player.getTileX() + 8; index >= Game1.player.getTileX() - 8; index--)
			{
				List<TemporaryAnimatedSprite> temporarySprites = Game1.player.currentLocation.temporarySprites;
				TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(6, new Vector2(index, Game1.player.getTileY()) * 64f, Color.White, 8, flipped: false, 50f);
				temporaryAnimatedSprite.layerDepth = 1f;
				int num2 = (temporaryAnimatedSprite.delayBeforeAnimationStart = num1 * 25);
				Vector2 vector2 = (temporaryAnimatedSprite.motion = new Vector2(-0.25f, 0f));
				temporarySprites.Add(temporaryAnimatedSprite);
				num1++;
			}
		}

		private void AddBuilding(Building building)
		{
			Building existingBuilding = buildings.FirstOrDefault((Building b) => b.buildingType == building.buildingType);
			if (existingBuilding == null)
			{
				buildings.Add(building);
			}
		}

		private WarpLocationChoice GetWarpLocationChoiceForBuildingType(string buildingType)
		{
			return buildingType switch
			{
				"Water Obelisk" => WarpLocationChoice.Beach, 
				"Earth Obelisk" => WarpLocationChoice.Mountain, 
				"Desert Obelisk" => WarpLocationChoice.Desert, 
				"Island Obelisk" => WarpLocationChoice.Island, 
				"Woods Obelisk" => WarpLocationChoice.DeepWoods, 
				_ => WarpLocationChoice.None, 
			};
		}

		private Vector2? GetValidWarpTile(GameLocation location, int x, int y)
		{
			Vector2 targetLocation = new Vector2(x, y + 1);
			if (location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(targetLocation))
			{
				return targetLocation;
			}
			targetLocation = new Vector2(x - 1, y);
			if (location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(targetLocation))
			{
				return targetLocation;
			}
			targetLocation = new Vector2(x + 1, y);
			if (location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(targetLocation))
			{
				return targetLocation;
			}
			targetLocation = new Vector2(x, y - 1);
			if (location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(targetLocation))
			{
				return targetLocation;
			}
			return null;
		}
	}
}
