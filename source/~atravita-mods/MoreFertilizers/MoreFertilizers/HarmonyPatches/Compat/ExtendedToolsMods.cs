/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using AtraShared.Utils.Extensions;
using HarmonyLib;

namespace MoreFertilizers.HarmonyPatches.Compat;

/// <summary>
/// Holds patches against radioactive tools and prismatic tools.
/// For some inexplicable reason, both these mods copy the vanilla
/// scarecrows function in a prefix that's quite out of date.
/// </summary>
/// <remarks>This is why atra doesn't like prefixing false unnecessarily, guys.</remarks>
internal static class ExtendedToolsMods
{
    /// <summary>
    /// Applies patches against Prismatic Tools and Radioactive Tools.
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    internal static void ApplyPatches(Harmony harmony)
    {
        HarmonyMethod prefix = new(typeof(ExtendedToolsMods), nameof(Prefix));
        Type prismaticPatches = AccessTools.TypeByName("PrismaticTools.Framework.PrismaticPatches");
        MethodInfo prismatcPrefix = AccessTools.Method(prismaticPatches, "Farm_AddCrows");

        if (prismatcPrefix is not null)
        {
            harmony.Patch(prismatcPrefix, prefix: prefix);
            ModEntry.ModMonitor.Log("Found Prismatic Tools, patching for compat", LogLevel.Info);
        }

        Type radioactivePatches = AccessTools.TypeByName("RadioactiveTools.Framework.RadioactivePatches");
        MethodInfo radioactivePrefix = AccessTools.Method(radioactivePatches, "Farm_AddCrows");

        if (radioactivePrefix is not null)
        {
            harmony.Patch(radioactivePrefix, prefix: prefix);
            ModEntry.ModMonitor.Log("Found Radioactive Tools, patching for compat", LogLevel.Info);
        }
    }

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static bool Prefix(ref bool __result)
    {
        ModEntry.ModMonitor.DebugOnlyLog("Disabling addCrows prefix for Prismatic Tools and Radioactive tools", LogLevel.Info);
        __result = true;
        return false;
    }
}
