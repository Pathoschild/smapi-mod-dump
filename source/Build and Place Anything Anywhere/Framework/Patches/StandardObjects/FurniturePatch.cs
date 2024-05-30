/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using Common.Helpers;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace AnythingAnywhere.Framework.Patches.StandardObjects
{
    internal sealed class FurniturePatch : PatchHelper
    {
        internal FurniturePatch() : base(typeof(Furniture)) { }
        internal void Apply()
        {
            Patch(PatchType.Postfix, nameof(Furniture.GetAdditionalFurniturePlacementStatus), nameof(GetAdditionalFurniturePlacementStatusPostfix), [typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer)]);
            Patch(PatchType.Postfix, nameof(Furniture.canBePlacedHere), nameof(CanBePlacedHerePostfix), [typeof(GameLocation), typeof(Vector2), typeof(CollisionMask), typeof(bool)]);
            Patch(PatchType.Postfix, nameof(Furniture.canBeRemoved), nameof(CanBeRemovedPostfix), [typeof(Farmer)]);
        }

        // Enables disabling wall furniture in all places in decoratable locations. It can be annoying indoors.
        private static void GetAdditionalFurniturePlacementStatusPostfix(Furniture __instance, GameLocation location, int x, int y, Farmer who, ref int __result)
        {
            // Check if the furniture is wall furniture
            bool isWallFurniture =
                (__instance.furniture_type.Value == 6 ||
                __instance.furniture_type.Value == 17 ||
                __instance.furniture_type.Value == 13 ||
                __instance.QualifiedItemId == "(F)1293");
            if (!ModEntry.Config.EnableWallFurnitureIndoors && location is DecoratableLocation && !ModEntry.Config.EnableBuildAnywhere)
            {
                if (!isWallFurniture)
                {
                    __result = 0;
                }
            }
            else
            {
                __result = 0;
            }
        }

        //Enable placing furniture in walls
        private static void CanBePlacedHerePostfix(Furniture __instance, GameLocation l, Vector2 tile, ref bool __result, CollisionMask collisionMask = CollisionMask.All, bool showError = false)
        {
            if (ModEntry.Config.EnableFreePlace)
                __result = true;
        }

        private static void CanBeRemovedPostfix(Furniture __instance, Farmer who, ref bool __result)
        {
            if (!ModEntry.Config.EnableRugRemovalBypass)
                return;

            GameLocation location = __instance.Location;
            if (location == null)
                return;

            if (__instance.isPassable())
                __result = true;
        }
    }
}
