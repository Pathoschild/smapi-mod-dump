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
using MoreFertilizers.Framework;
using StardewValley.Locations;

namespace MoreFertilizers.HarmonyPatches.FishFood;

/// <summary>
/// Classes that holds patches against Submarine's GetFish.
/// </summary>
[HarmonyPatch(typeof(Submarine))]
internal static class SubmarineGetFish
{
    [HarmonyPatch(nameof(Submarine.getFish))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Required for Harmony")]
    private static bool Prefix(GameLocation __instance, ref SObject __result)
    {
        try
        {
            if (__instance?.modData?.GetInt(CanPlaceHandler.FishFood) > 0)
            {
                int[] fishies = new[] { 800, 799, 798, 154, 155, 149 };
                __result = new SObject(fishies[Game1.random.Next(fishies.Length)], 1);
                return false;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in replacing submarine getfish!{ex}", LogLevel.Error);
        }
        return true;
    }
}