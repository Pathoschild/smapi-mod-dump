/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/FarmExpansion
**
*************************************************/

using Harmony;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmExpansion.Framework
{
    [HarmonyPatch(typeof(NetBuildingRef))]
    [HarmonyPatch("Value", MethodType.Getter)]
    public class NetBuildingRefPatch
    {
        public static void Postfix(NetBuildingRef __instance, NetString ___nameOfIndoors, ref Building __result)
        {
            if (__result != null)
                return;
            var locationFromName = (FarmExpansion)Game1.getLocationFromName("FarmExpansion");
            if (locationFromName == null)
                return;
            __result = locationFromName.getBuildingByName((string)((NetFieldBase<string, NetString>)___nameOfIndoors));
        }
    }
}
