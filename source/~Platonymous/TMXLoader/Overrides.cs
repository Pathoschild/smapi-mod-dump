/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TMXLoader
{
    class Overrides
    {
        internal static List<string> NPCs = new List<string>();

        [HarmonyPatch]
        internal class SpouseRoomTilesFix
        {
            internal static MethodInfo TargetMethod()
            {
               return PyUtils.getTypeSDV("Locations.FarmHouse").GetMethod("loadSpouseRoom");
            }

            internal static bool Prefix(FarmHouse __instance)
            {
                if (__instance != null && __instance.owner != null && __instance.owner.getSpouse() != null && NPCs.Contains(__instance.owner.getSpouse().Name))
                    return false;

                return true;
            }
        }


    }
}
