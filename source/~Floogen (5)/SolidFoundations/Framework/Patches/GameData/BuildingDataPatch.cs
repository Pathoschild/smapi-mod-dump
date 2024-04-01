/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using HarmonyLib;
using SolidFoundations.Framework.Models.ContentPack;
using StardewModdingAPI;
using StardewValley.GameData.Buildings;
using System;

namespace SolidFoundations.Framework.Patches.GameData
{
    internal class BuildingDataPatch : PatchTemplate
    {

        private readonly Type _object = typeof(BuildingData);

        internal BuildingDataPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(BuildingData.GetActionAtTile), new[] { typeof(int), typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(GetActionAtTilePostfix)));
        }

        internal static void GetActionAtTilePostfix(BuildingData __instance, int relativeX, int relativeY, ref string __result)
        {
            if (__instance is ExtendedBuildingModel extendedModel && extendedModel is not null && string.IsNullOrEmpty(__result))
            {
                __result = extendedModel.GetActionAtTile(relativeX, relativeY);
                if (string.IsNullOrEmpty(__result) && extendedModel.GetSpecialActionAtTile(relativeX, relativeY) is not null)
                {
                    __result = "SpecialAction";
                }
                if (string.IsNullOrEmpty(__result) && extendedModel.GetCollectChestActionAtTile(relativeX, relativeY) is not null || extendedModel.GetLoadChestActionAtTile(relativeX, relativeY) is not null)
                {
                    __result = "BuildingChest";
                }
            }
        }
    }
}
