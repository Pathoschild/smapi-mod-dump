/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace OneSprinklerOneScarecrow.Framework.Overrides
{
    internal class GetBaseRadiusForSprinklerPatch
    {
        private static IMonitor _monitor;
        public GetBaseRadiusForSprinklerPatch(IMonitor monitor)
        {
            _monitor = monitor;
        }
        public static bool Prefix(ref Object __instance, ref int __result)
        {
            __result = -1;
            /*
            foreach (var obj in __instance.Objects.Pairs)
            {
                if (obj.Value.Name == "Haxarecrow")
                {
                    _monitor.Log("No crows ran");
                    return false;
                }

            }

            return true;*/

            if (Utility.IsNormalObjectAtParentSheetIndex(__instance, 599))
            {
                __result = 0;
            }
            if (Utility.IsNormalObjectAtParentSheetIndex(__instance, 621))
            {
                __result = 1;
            }
            if (Utility.IsNormalObjectAtParentSheetIndex(__instance, 645))
            {
                __result = 2;
            }

            if (__instance.Name.Contains("Haxor Sprinkler"))
            {
                __result = 999;
            }
            __result = __result >= 0 ? __result : -1;

            return __result <= 0;

        }
    }
}
