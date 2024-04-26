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

namespace FlipBuildings.Utilities
{
	internal class CompatibilityUtility
	{
		internal static readonly bool IsAlternativeTexturesLoaded = ModEntry.Helper.ModRegistry.IsLoaded("PeacefulEnd.AlternativeTextures");
		internal static readonly bool IsSolidFoundationsLoaded = ModEntry.Helper.ModRegistry.IsLoaded("PeacefulEnd.SolidFoundations");

		// Get AT types
		internal static readonly Type BuildingPatchType = Type.GetType("AlternativeTextures.Framework.Patches.Buildings.BuildingPatch, AlternativeTextures");

		// Get SF types
		// internal static readonly Type GenericBuildingType = Type.GetType("SolidFoundations.Framework.Models.ContentPack.GenericBuilding, SolidFoundations");
		// internal static readonly Type BuildingDrawLayerType = Type.GetType("SolidFoundations.Framework.Models.Backport.BuildingDrawLayer, SolidFoundations");
		// internal static readonly Type ExtendedBuildingModelType = Type.GetType("SolidFoundations.Framework.Models.ContentPack.ExtendedBuildingModel, SolidFoundations");
		// internal static readonly Type ExtendedBuildingDrawLayerType = Type.GetType("SolidFoundations.Framework.Models.ContentPack.ExtendedBuildingDrawLayer, SolidFoundations");
		// internal static readonly Type BuildingDataType = Type.GetType("SolidFoundations.Framework.Models.Backport.BuildingData, SolidFoundations");
	}
}
