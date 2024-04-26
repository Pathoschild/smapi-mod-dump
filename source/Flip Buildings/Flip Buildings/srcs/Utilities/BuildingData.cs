/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Force.DeepCloner;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;

namespace FlipBuildings.Utilities
{
	internal class BuildingDataUtility
	{
		/// <summary>The cached data for flipped buildings from <c>Data/Buildings</c>.</summary>
		public static IDictionary<string, BuildingData> flippedBuildingData;

		internal static void LoadContent()
		{
			flippedBuildingData = Game1.buildingData.DeepClone();

			foreach (BuildingData buildingData in flippedBuildingData.Values)
			{
				if (buildingData.UpgradeSignTile != new Vector2(-1, -1))
				{
					buildingData.UpgradeSignTile.X = buildingData.Size.X - 1 - buildingData.UpgradeSignTile.X;
				}
				if (buildingData.DrawOffset != Vector2.Zero)
				{
					buildingData.DrawOffset.X = -buildingData.DrawOffset.X;
				}
				if (buildingData.CollisionMap is not null)
				{
					buildingData.CollisionMap = string.Join('\n', buildingData.CollisionMap.Trim().Split('\n', StringSplitOptions.TrimEntries).Select(line => new string(line.Reverse().ToArray())));
				}
				if (buildingData.AdditionalPlacementTiles is not null)
				{
					foreach (BuildingPlacementTile buildingPlacementTile in buildingData.AdditionalPlacementTiles)
					{
						buildingPlacementTile.TileArea.X = buildingData.Size.X - 1 - (buildingPlacementTile.TileArea.X + buildingPlacementTile.TileArea.Width - 1);
					}
				}
				if (buildingData.HumanDoor != new Point(-1, -1))
				{
					buildingData.HumanDoor.X = buildingData.Size.X - 1 - buildingData.HumanDoor.X;
				}
				if (buildingData.AnimalDoor != new Rectangle(-1, -1, 0, 0))
				{
					buildingData.AnimalDoor.X = buildingData.Size.X - 1 - (buildingData.AnimalDoor.X + buildingData.AnimalDoor.Width - 1);
				}
				if (buildingData.Chests is not null)
				{
					foreach (BuildingChest buildingChest in buildingData.Chests)
					{
						buildingChest.DisplayTile.X = buildingData.Size.X - 1 - buildingChest.DisplayTile.X;
					}
				}
				if (buildingData.ActionTiles is not null)
				{
					foreach (BuildingActionTile buildingActionTile in buildingData.ActionTiles)
					{
						buildingActionTile.Tile.X = buildingData.Size.X - 1 - buildingActionTile.Tile.X;
					}
				}
				if (buildingData.TileProperties is not null)
				{
					foreach (BuildingTileProperty buildingTileProperty in buildingData.TileProperties)
					{
						buildingTileProperty.TileArea.X = buildingData.Size.X - 1 - (buildingTileProperty.TileArea.X + buildingTileProperty.TileArea.Width - 1);
					}
				}
				if (buildingData.DrawLayers is not null)
				{
					foreach (BuildingDrawLayer buildingDrawLayer in buildingData.DrawLayers)
					{
						buildingDrawLayer.DrawPosition.X = buildingData.Size.X * 16f - buildingDrawLayer.DrawPosition.X - buildingDrawLayer.SourceRect.Width;
					}
				}
				if (buildingData.Metadata is not null)
				{
					ProcessMetadata(buildingData, buildingData.Metadata);
				}
				if (buildingData.Skins is not null)
				{
					foreach (BuildingSkin buildingSkin in buildingData.Skins)
					{
						if (buildingSkin.Metadata is not null)
						{
							ProcessMetadata(buildingData, buildingSkin.Metadata);
						}
					}
				}
				HandleFarmhouseSpecifics(buildingData);
			}
		}

		private static void ProcessMetadata(BuildingData buildingData, Dictionary<string, string> metadata)
		{
			foreach (KeyValuePair<string, string> buildingMetadata in metadata)
			{
				if (buildingMetadata.Key.StartsWith("ChimneyPosition"))
				{
					const int chimneySourceRectWidth = 10;
					string[] array = ArgUtility.SplitBySpace(buildingMetadata.Value);
					Point chimneyPosition = new(int.Parse(array[0]), int.Parse(array[1]));

					chimneyPosition.X = buildingData.Size.X * 16 - chimneyPosition.X - chimneySourceRectWidth / 4;
					metadata[buildingMetadata.Key] = $"{chimneyPosition.X} {chimneyPosition.Y}";
				}
			}
		}

		private static void	HandleFarmhouseSpecifics(BuildingData buildingData)
		{
			if (buildingData.Name.Equals("Farmhouse"))
			{
				if (buildingData.DrawOffset != Vector2.Zero)
				{
					buildingData.DrawOffset.X = 0;
				}
				if (buildingData.CollisionMap is not null)
				{
					buildingData.CollisionMap = "\nXXXXXXXXX\nXXXXXXXXX\nXXXXXXXXX\nXOOOOOOOX\nXXOOOXXXXX\n";
				}
				if (buildingData.AdditionalPlacementTiles is not null)
				{
					buildingData.AdditionalPlacementTiles.First().TileArea.X = 9;
				}
				if (buildingData.ActionTiles is not null)
				{
					foreach (BuildingActionTile buildingActionTile in buildingData.ActionTiles)
					{
						if (buildingActionTile.Id.Equals("Default_Mailbox"))
						{
							buildingActionTile.Tile.X = 9;
						}
					}
				}
				if (buildingData.DrawLayers is not null)
				{
					foreach (BuildingDrawLayer buildingDrawLayer in buildingData.DrawLayers)
					{
						if (buildingDrawLayer.Id.Equals("Default_Mailbox"))
						{
							buildingDrawLayer.DrawPosition.X = 146;
						}
					}
				}
				if (buildingData.Metadata is not null)
				{
					HandleFarmhouseMetadataSpecifics(buildingData.Metadata);
				}
				if (buildingData.Skins is not null)
				{
					foreach (BuildingSkin buildingSkin in buildingData.Skins)
					{
						if (buildingSkin.Metadata is not null)
						{
							HandleFarmhouseMetadataSpecifics(buildingSkin.Metadata);
						}
					}
				}
			}
		}

		private static void HandleFarmhouseMetadataSpecifics(Dictionary<string, string> metadata)
		{
			foreach (KeyValuePair<string, string> buildingMetadata in metadata)
			{
				if (buildingMetadata.Key.StartsWith("ChimneyPosition"))
				{
					string[] array = ArgUtility.SplitBySpace(buildingMetadata.Value);
					Point chimneyPosition = new(int.Parse(array[0]), int.Parse(array[1]));

					chimneyPosition.X += Game1.tileSize / 4;
					metadata[buildingMetadata.Key] = $"{chimneyPosition.X} {chimneyPosition.Y}";
				}
			}
		}

		public static BuildingData GetFlippedData(Building __instance)
		{
			if (!TryGetFlippedData(__instance.buildingType.Value, out BuildingData data))
			{
				return null;
			}
			return data;
		}

		public static bool TryGetFlippedData(string buildingType, out BuildingData data)
		{
			if (buildingType == null)
			{
				data = null;
				return false;
			}
			return flippedBuildingData.TryGetValue(buildingType, out data);
		}
	}
}
