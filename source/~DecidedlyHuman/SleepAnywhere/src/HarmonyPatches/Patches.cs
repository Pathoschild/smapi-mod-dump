/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using xTile.Dimensions;
using SObject = StardewValley.Object;
// ReSharper disable InconsistentNaming

namespace SleepAnywhere.HarmonyPatches
{
    public static class Patches
	{
		private static IMonitor logger;

		public static void Initialise(IMonitor logger)
		{
			Patches.logger = logger;
		}

		public static void BedFurniture_PlacementAction_Postfix(BedFurniture __instance, ref bool __result)
		{
            if (__instance.Name.Equals("DecidedlyHuman.SleepAnywhereItems/Portable Bed"))
            {
                logger.Log($"BedFurniture.placementAction returning: {__result}.");

                __result = true;
            }
		}

        public static void Furniture_PlacementAction_Postfix(Furniture __instance, ref bool __result)
        {
            if (__instance.Name.Equals("DecidedlyHuman.SleepAnywhereItems/Portable Bed"))
            {
                logger.Log($"Furniture.placementAction returning: {__result}.");

                __result = true;
            }
        }

        public static void Object_PlacementAction_Postfix(SObject __instance, ref bool __result)
        {
            if (__instance.Name.Equals("DecidedlyHuman.SleepAnywhereItems/Portable Bed"))
            {
                logger.Log($"Object.placementAction returning: {__result}.");

                __result = true;
            }
        }

        public static void GameLocation_CanPlaceThisFurnitureHere_Postfix(GameLocation __instance, Furniture furniture, ref bool __result)
        {
            if (furniture.Name.Equals("DecidedlyHuman.SleepAnywhereItems/Portable Bed"))
            {
                // We want to return true here in every circumstance.

                __result = true;
            }
        }
        
        public static void MineShaft_CanPlaceThisFurnitureHere_Postfix(MineShaft __instance, Furniture furniture, ref bool __result)
        {
            if (furniture.Name.Equals("DecidedlyHuman.SleepAnywhereItems/Portable Bed"))
            {
                // We want to return true here in every circumstance.

                __result = true;
            }
        }
        
        public static void VolcanoDungeon_CanPlaceThisFurnitureHere_Postfix(VolcanoDungeon __instance, Furniture furniture, ref bool __result)
        {
            if (furniture.Name.Equals("DecidedlyHuman.SleepAnywhereItems/Portable Bed"))
            {
                // We want to return true here in every circumstance.

                __result = true;
            }
        }

        public static void Utility_PlayerCanPlaceItemHere_Postfix(Item item, ref bool __result)
        {
            if (item.Name.Equals("DecidedlyHuman.SleepAnywhereItems/Portable Bed"))
            {
                // logger.Log($"Utility.playerCanPlaceItem returning: {__result}.");

                __result = true;
            }
        }

        public static void Game1_PressActionButton_Postfix(Game1 __instance, ref bool __result)
        {
            
        }
    }
}