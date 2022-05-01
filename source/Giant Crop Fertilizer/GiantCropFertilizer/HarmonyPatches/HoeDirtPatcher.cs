/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/GiantCropFertilizer
**
*************************************************/

using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace GiantCropFertilizer.HarmonyPatches;

/// <summary>
/// Holds patches against HoeDirt that replaces our fertlizer.
/// This way MultiFertlizer doesn't clear us...
/// </summary>
[HarmonyPatch]
internal static class HoeDirtPatcher
{

    /// <summary>
    /// Applies the patches for this class.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    internal static void ApplyPatches(Harmony harmony)
    {
        HarmonyMethod? prefix = new(
            typeof(HoeDirtPatcher).StaticMethodNamed(nameof(HoeDirtPatcher.PrefixMulti)),
            priority: Priority.First);
        HarmonyMethod? postfix = new(
            typeof(HoeDirtPatcher).StaticMethodNamed(nameof(HoeDirtPatcher.PostfixMulti)),
            priority: Priority.Last);

        harmony.Patch(
            typeof(HoeDirt).InstanceMethodNamed("applySpeedIncreases"),
            prefix: prefix,
            postfix: postfix);

        harmony.Patch(
            typeof(HoeDirt).InstanceMethodNamed(nameof(HoeDirt.dayUpdate)),
            prefix: prefix,
            postfix: postfix);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HoeDirt), nameof(HoeDirt.plant))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static bool PrefixCanPlant(HoeDirt __instance, int index, bool isFertilizer, ref bool __result)
    {
        if (isFertilizer && ModEntry.GiantCropFertilizerID != -1 && ModEntry.GiantCropFertilizerID == index && __instance.fertilizer.Value == index)
        {
            ModEntry.ModMonitor.Log("Blocked placement");
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13916-2"));
            __result = false;
            return false;
        }
        return true;
    }

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void PrefixMulti(HoeDirt __instance, out int? __state)
    {
        if(ModEntry.GiantCropFertilizerID != -1 && ModEntry.GiantCropFertilizerID == __instance.fertilizer.Value)
        {
            ModEntry.ModMonitor.DebugOnlyLog("Found fertilizer, saving");
            __state = ModEntry.GiantCropFertilizerID;
        }
        else
        {
            __state = null;
        }
    }

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void PostfixMulti(HoeDirt __instance, int? __state)
    {
        if (__state is not null && __state.Value == ModEntry.GiantCropFertilizerID)
        {
            ModEntry.ModMonitor.DebugOnlyLog("Found fertilizer, restoring");
            __instance.fertilizer.Value = __state.Value;
        }
    }
}