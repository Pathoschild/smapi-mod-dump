/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GrowableGiantCrops.Framework;

using HarmonyLib;

namespace GrowableGiantCrops.HarmonyPatches.Niceties;

[HarmonyPatch(typeof(SObject))]
internal static class SObjectPatches
{
    [HarmonyPatch(nameof(SObject.performToolAction))]
    private static bool Prefix(SObject __instance, Tool t, GameLocation location, ref bool __result)
    {
        if (t is not ShovelTool)
        {
            return true;
        }

        return true;
    }
}
