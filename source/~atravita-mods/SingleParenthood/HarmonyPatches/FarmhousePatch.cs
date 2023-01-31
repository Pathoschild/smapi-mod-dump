/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;
using HarmonyLib;
using StardewValley.Locations;

namespace SingleParenthood.HarmonyPatches;

/// <summary>
/// Patches farmhouse to prevent the player from removing the bed if they're expecting a kid.
/// </summary>
[HarmonyPatch(typeof(FarmHouse))]
internal static class FarmhousePatch
{
    [HarmonyPatch(nameof(FarmHouse.CanModifyCrib))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static bool Prefix(FarmHouse __instance, ref bool __result)
    {
        if (__instance.owner.modData?.GetInt(ModEntry.CountUp) is > 0 )
        {
            __result = false;
            return false;
        }
        return true;
    }
}
