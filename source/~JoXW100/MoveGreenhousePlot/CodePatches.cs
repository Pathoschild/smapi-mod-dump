/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Linq;

namespace MoveGreenhousePlot
{
    public partial class ModEntry
    {
        public static bool checkingPos;
        public static Vector2 lastPos;
        public static Vector2 lastSpeed;
        public static Vector2 speed;

        [HarmonyPatch(typeof(CarpenterMenu), nameof(CarpenterMenu.hasPermissionsToMove))]
        public class CarpenterMenu_hasPermissionsToMove_Patch
        {
            public static bool Prefix(CarpenterMenu __instance, Building b, ref bool __result)
            {
                if (!Config.ModEnabled || b is not GreenhouseBuilding)
                    return true;

                __result = Game1.IsMasterGame || Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On || (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && b.hasCarpenterPermissions());
                return false;
            }
        }
        
   }
}