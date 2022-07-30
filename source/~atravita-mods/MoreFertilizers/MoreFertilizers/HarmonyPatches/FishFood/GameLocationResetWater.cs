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

namespace MoreFertilizers.HarmonyPatches.FishFood;

/// <summary>
/// Resets the water color to the fed fish color if needed.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class GameLocationResetWater
{
    [UsedImplicitly]
    [HarmonyPatch("resetForPlayerEntry")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void Postfix(GameLocation __instance)
    {
        try
        {
            if (__instance?.modData?.GetInt(CanPlaceHandler.FishFood) is > 0)
            {
                __instance.waterColor.Value = ModEntry.Config.WaterOverlayColor;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Error in adjusting water color at location {__instance.NameOrUniqueName}:\n\n{ex}", LogLevel.Error);
        }
    }
}