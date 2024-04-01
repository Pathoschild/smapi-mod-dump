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
    internal class IsSprinklerPatch
    {
        public static bool Prefix(ref Object __instance, ref bool __result)
        {
            __result = __instance.GetBaseRadiusForSprinkler() >= 0 ||
                       __instance.QualifiedItemId == $"(O){HaxorSprinkler.ItemID}";

        return __result;
        }
    }
}
