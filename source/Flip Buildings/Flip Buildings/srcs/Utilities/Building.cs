/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace FlipBuildings.Utilities
{
	internal class BuildingHelper
	{
		public static bool TryToFlip(Building building, bool drawLayers = false)
		{
			string modDataKey = !drawLayers ? ModDataKeys.FLIPPED : ModDataKeys.FLIPPED_DRAWLAYERS;
			string sound = !drawLayers ? "axchop" :  "axe";

			if (building != null)
			{
				if (!CanBeFlipped(building, out string cannotFlipMessage))
				{
					Game1.addHUDMessage(new HUDMessage(cannotFlipMessage, HUDMessage.error_type));
					Game1.playSound("cancel");
					return false;
				}
				if (!building.modData.ContainsKey(modDataKey))
				{
					building.modData.Add(modDataKey, "T");
				}
				else
				{
					building.modData.Remove(modDataKey);
				}
				if (!drawLayers)
				{
					Flip(building);
				}
				Game1.playSound(sound);
				return true;
			}
			return false;
		}

		public static void Flip(Building building)
		{
			GameLocation location = building.GetParentLocation();

			building.ReloadBuildingData();
			building.updateInteriorWarps();
			if (building.GetData().Name.Equals("Farmhouse"))
			{
				Game1.getFarm().UnsetFarmhouseValues();
			}

			// Reset cached data
			string workaround = building.buildingType.Value;
			building.buildingType.Value = null;
			building.buildingType.Value = workaround;

			for (int x = building.tileX.Value; x < building.tileX.Value + building.tilesWide.Value / 2; x++)
			{
				int mirroredX = building.tileX.Value + building.tilesWide.Value - 1 - (x - building.tileX.Value);

				for (int y = building.tileY.Value; y < building.tileY.Value + building.tilesHigh.Value; y++)
				{
					Object objectA = location.getObjectAtTile(x, y);
					Object objectB = location.getObjectAtTile(mirroredX, y);
					bool isNonFurnitureObjectA = objectA is not null && objectA is not Furniture;
					bool isNonFurnitureObjectB = objectB is not null && objectB is not Furniture;

					if (isNonFurnitureObjectA)
					{
						location.objects.Remove(objectA.TileLocation);
						objectA.TileLocation = new Vector2(mirroredX, y);
					}
					if (isNonFurnitureObjectB)
					{
						location.objects.Remove(objectB.TileLocation);
						objectB.TileLocation = new Vector2(x, y);
					}
					if (isNonFurnitureObjectA)
					{
						location.objects.Add(objectA.TileLocation, objectA);
					}
					if (isNonFurnitureObjectB)
					{
						location.objects.Add(objectB.TileLocation, objectB);
					}
				}
			}
			for (int i = location.furniture.Count - 1; i >= 0; i--)
			{
				Furniture furniture = location.furniture[i];
				int additionalPlacementTilesX = (furniture.rotations.Value % 2 == 0 ? furniture.boundingBox.Width : furniture.boundingBox.Height) / Game1.tileSize - 1;
				int additionalPlacementTilesY = (furniture.rotations.Value % 2 != 0 ? furniture.boundingBox.Width : furniture.boundingBox.Height) / Game1.tileSize - 1;

				if (building.tileX.Value <= furniture.TileLocation.X + additionalPlacementTilesX && furniture.TileLocation.X <= building.tileX.Value + building.tilesWide.Value - 1)
				{
					if (building.tileY.Value <= furniture.TileLocation.Y + additionalPlacementTilesY && furniture.TileLocation.Y <= building.tileY.Value + building.tilesHigh.Value - 1)
					{
						location.furniture.RemoveAt(i);
						furniture.TileLocation = new Vector2(building.tileX.Value + building.tilesWide.Value - 1 - (furniture.TileLocation.X - building.tileX.Value + additionalPlacementTilesX), furniture.TileLocation.Y);
						location.furniture.Add(furniture);
					}
				}
			}
			foreach (NPC npc in location.characters)
			{
				int buildingX = building.tileX.Value * Game1.tileSize;
				int buildingY = building.tileY.Value * Game1.tileSize;
				int buildingWidth = building.tilesWide.Value * Game1.tileSize;
				int buildingHeight = building.tilesHigh.Value * Game1.tileSize;
				int additionalPlacementTilesX = (npc.Sprite.SpriteWidth / 16 - 1) * Game1.tileSize;
				int additionalPlacementTilesY = (npc.Sprite.SpriteHeight / 16 - 1) * Game1.tileSize;

				if (buildingX <= npc.Position.X + additionalPlacementTilesX && npc.Position.X < buildingX + buildingWidth)
				{
					if (buildingY <= npc.Position.Y + additionalPlacementTilesY && npc.Position.Y < buildingY + buildingHeight)
					{
						npc.Position = new Vector2(buildingX + buildingWidth - 1 * Game1.tileSize - (npc.Position.X - buildingX + additionalPlacementTilesX), npc.Position.Y);
						npc.FacingDirection = npc.FacingDirection == 1 ? 3 : npc.FacingDirection == 3 ? 1 : npc.FacingDirection;
					}
				}
			}
			foreach (FarmAnimal farmAnimal in location.animals.Values)
			{
				int buildingX = building.tileX.Value * Game1.tileSize;
				int buildingWidth = building.tilesWide.Value * Game1.tileSize;
				int additionalPlacementTilesX = (farmAnimal.Sprite.SpriteWidth / 16 - 1) * Game1.tileSize;

				if (building.intersects(farmAnimal.GetBoundingBox()))
				{
					farmAnimal.Position = new Vector2(buildingX + buildingWidth - 1 * Game1.tileSize - (farmAnimal.Position.X - buildingX + additionalPlacementTilesX), farmAnimal.Position.Y);
					farmAnimal.FacingDirection = farmAnimal.FacingDirection == 1 ? 3 : farmAnimal.FacingDirection == 3 ? 1 : farmAnimal.FacingDirection;
				}
			}
		}

		public static bool CanBeFlipped(Building building)
		{
			return CanBeFlipped(building, out string _);
		}

		public static bool CanBeFlipped(Building building, out string cannotFlipMessage)
		{
			static bool IsBuildingFlippable(Building building)
			{
				if (building != null && building is GreenhouseBuilding && !Game1.getFarm().greenhouseUnlocked.Value)
				{
					return false;
				}
				return true;
			}

			static bool HasPermissionToFlip(Building building)
			{
				if (Game1.IsMasterGame)
				{
					return true;
				}
				if (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On)
				{
					return true;
				}
				if (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings)
				{
					if (building != null)
					{
						if (building.hasCarpenterPermissions())
						{
							return true;
						}
						if (building.GetIndoors() is FarmHouse farmHouse)
						{
							if (farmHouse.IsOwnedByCurrentPlayer || Game1.player.spouse == farmHouse.OwnerId.ToString())
							{
								return true;
							}
						}
					}
				}
				return false;
			}

			static bool IsPlayerHere(Building building)
			{
				if (building != null)
				{
					Rectangle buildingRect = new(building.tileX.Value * 64, building.tileY.Value * 64, building.tilesWide.Value * 64, building.tilesHigh.Value * 64);
					foreach (Farmer farmer in Game1.getOnlineFarmers())
					{
						if (farmer.GetBoundingBox().Intersects(buildingRect))
						{
							return true;
						}
					}
				}
				return false;
			}

			cannotFlipMessage = string.Empty;
			if (!IsBuildingFlippable(building))
			{
				cannotFlipMessage = ModEntry.Helper.Translation.Get("Carpenter_CannotFlip");
				return false;
			}
			if (!HasPermissionToFlip(building))
			{
				cannotFlipMessage = ModEntry.Helper.Translation.Get("Carpenter_CannotFlip_Permission");
				return false;
			}
			if (IsPlayerHere(building))
			{
				cannotFlipMessage = ModEntry.Helper.Translation.Get("Carpenter_CannotFlip_PlayerHere");
				return false;
			}
			return true;
		}
	}
}
