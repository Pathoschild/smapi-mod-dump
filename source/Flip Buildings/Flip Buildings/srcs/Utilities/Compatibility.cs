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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;

namespace FlipBuildings.Utilities
{
	internal class CompatibilityUtility
	{
		internal static readonly bool IsAlternativeTexturesLoaded = ModEntry.Helper.ModRegistry.IsLoaded("PeacefulEnd.AlternativeTextures");
		internal static readonly bool IsSolidFoundationsLoaded = ModEntry.Helper.ModRegistry.IsLoaded("PeacefulEnd.SolidFoundations");

		// Get AT types
		internal static readonly Type ATBuildingPatchType = Type.GetType("AlternativeTextures.Framework.Patches.Buildings.BuildingPatch, AlternativeTextures");

		// Get SF types
		internal static readonly Type SFSolidFoundationsType = Type.GetType("SolidFoundations.SolidFoundations, SolidFoundations");
		internal static readonly Type SFBuildingManagerType = Type.GetType("SolidFoundations.Framework.Managers.BuildingManager, SolidFoundations");
		internal static readonly Type SFLightManagerType = Type.GetType("SolidFoundations.Framework.Managers.LightManager, SolidFoundations");
		internal static readonly Type SFBuildingPatchType = Type.GetType("SolidFoundations.Framework.Patches.Buildings.BuildingPatch, SolidFoundations");
		internal static readonly Type SFBuildingExtensionsType = Type.GetType("SolidFoundations.Framework.Extensions.BuildingExtensions, SolidFoundations");
		internal static readonly Type SFSpecialActionType = Type.GetType("SolidFoundations.Framework.Models.ContentPack.Actions.SpecialAction, SolidFoundations");
		internal static readonly Type SFExtendedBuildingModelType = Type.GetType("SolidFoundations.Framework.Models.ContentPack.ExtendedBuildingModel, SolidFoundations");
		internal static readonly Type SFExtendedBuildingActionTilesType = Type.GetType("SolidFoundations.Framework.Models.ContentPack.ExtendedBuildingActionTiles, SolidFoundations");
		internal static readonly Type SFChestActionTileType = Type.GetType("SolidFoundations.Framework.Models.ContentPack.ChestActionTile, SolidFoundations");
		internal static readonly Type SFLightType = Type.GetType("SolidFoundations.Framework.Models.ContentPack.Light, SolidFoundations");

		public static void ProcessSFExtendedBuildingModel(BuildingData buildingData)
		{
			if (IsSolidFoundationsLoaded && SFExtendedBuildingModelType.IsInstanceOfType(buildingData))
			{
				List<Point> auxiliaryHumanDoors = (List<Point>)SFExtendedBuildingModelType.GetField("AuxiliaryHumanDoors", BindingFlags.Public | BindingFlags.Instance).GetValue(buildingData);
				List<Point> tunnelDoors = (List<Point>)SFExtendedBuildingModelType.GetField("TunnelDoors", BindingFlags.Public | BindingFlags.Instance).GetValue(buildingData);
				object loadChestTiles = SFExtendedBuildingModelType.GetField("LoadChestTiles", BindingFlags.Public | BindingFlags.Instance).GetValue(buildingData);
				object collectChestTiles = SFExtendedBuildingModelType.GetField("CollectChestTiles", BindingFlags.Public | BindingFlags.Instance).GetValue(buildingData);
				object eventTiles = SFExtendedBuildingModelType.GetField("EventTiles", BindingFlags.Public | BindingFlags.Instance).GetValue(buildingData);
				object lights = SFExtendedBuildingModelType.GetField("Lights", BindingFlags.Public | BindingFlags.Instance).GetValue(buildingData);

				if (auxiliaryHumanDoors is not null)
				{
					for (int i = 0; i < auxiliaryHumanDoors.Count; i++)
					{
						auxiliaryHumanDoors[i] = new Point(buildingData.Size.X - 1 - auxiliaryHumanDoors[i].X, auxiliaryHumanDoors[i].Y);
					}
				}
				if (tunnelDoors is not null)
				{
					for (int i = 0; i < tunnelDoors.Count; i++)
					{
						tunnelDoors[i] = new Point(buildingData.Size.X - 1 - tunnelDoors[i].X, tunnelDoors[i].Y);
					}
				}
				if (loadChestTiles is not null && loadChestTiles is IEnumerable loadChestTilesEnumerable)
				{
					Type elementType = loadChestTilesEnumerable.GetType().GetGenericArguments().FirstOrDefault();

					if (elementType is not null && elementType.Equals(SFChestActionTileType))
					{
						PropertyInfo tileProperty = elementType.GetProperty("Tile", BindingFlags.Public | BindingFlags.Instance);

						foreach (object loadChestTile in loadChestTilesEnumerable)
						{
							Point tile = (Point)tileProperty.GetValue(loadChestTile);

							tile.X = buildingData.Size.X - 1 - tile.X;
							tileProperty.SetValue(loadChestTile, tile);
						}
					}
				}
				if (collectChestTiles is not null && collectChestTiles is IEnumerable collectChestTilesEnumerable)
				{
					Type elementType = collectChestTilesEnumerable.GetType().GetGenericArguments().FirstOrDefault();

					if (elementType is not null && elementType.Equals(SFChestActionTileType))
					{
						PropertyInfo tileProperty = elementType.GetProperty("Tile", BindingFlags.Public | BindingFlags.Instance);

						foreach (object collectChestTile in collectChestTilesEnumerable)
						{
							Point tile = (Point)tileProperty.GetValue(collectChestTile);

							tile.X = buildingData.Size.X - 1 - tile.X;
							tileProperty.SetValue(collectChestTile, tile);
						}
					}
				}
				if (eventTiles is not null && eventTiles is IEnumerable eventTilesEnumerable)
				{
					Type elementType = eventTilesEnumerable.GetType().GetGenericArguments().FirstOrDefault();

					if (elementType is not null && elementType.Equals(SFExtendedBuildingActionTilesType))
					{
						FieldInfo tileField = elementType.GetField("Tile", BindingFlags.Public | BindingFlags.Instance);

						foreach (object eventTile in eventTilesEnumerable)
						{
							Point tile = (Point)tileField.GetValue(eventTile);

							tile.X = buildingData.Size.X - 1 - tile.X;
							tileField.SetValue(eventTile, tile);
						}
					}
				}
				if (lights is not null && lights is IEnumerable lightsEnumerable)
				{
					Type elementType = lightsEnumerable.GetType().GetGenericArguments().FirstOrDefault();

					if (elementType is not null && elementType.Equals(SFLightType))
					{
						PropertyInfo tileField = elementType.GetProperty("Tile", BindingFlags.Public | BindingFlags.Instance);
						PropertyInfo tileOffsetInPixelsField = elementType.GetProperty("TileOffsetInPixels", BindingFlags.Public | BindingFlags.Instance);

						foreach (object light in lightsEnumerable)
						{
							Point tile = (Point)tileField.GetValue(light);
							Point tileOffsetInPixels = (Point)tileOffsetInPixelsField.GetValue(light);

							tile.X = buildingData.Size.X - 1 - tile.X + 1;
							tileOffsetInPixels.X = buildingData.Size.X - 1 - tileOffsetInPixels.X;
							tileField.SetValue(light, tile);
							tileOffsetInPixelsField.SetValue(light, tileOffsetInPixels);
						}
					}
				}
			}
		}

		public static void InvokeSFBuildingExtensionsResetLightMethod(Building building, GameLocation location)
		{
			if (IsSolidFoundationsLoaded)
			{
				SFBuildingExtensionsType.GetMethod("ResetLights", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { building, location });
			}
		}
	}
}
