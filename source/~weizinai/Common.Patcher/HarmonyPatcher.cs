/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;

namespace weizinai.StardewValleyMod.Common.Patcher;

/// <summary>Simplifies applying <see cref="IPatcher"/> instances to the game.</summary>
internal static class HarmonyPatcher
{
    /// <summary>Apply the given Harmony patchers.</summary>
    /// <param name="mod">The mod applying the patchers.</param>
    /// <param name="patchers">The patchers to apply.</param>
    public static void Apply(Mod mod, params IPatcher[] patchers)
    {
        var harmony = new Harmony(mod.ModManifest.UniqueID);

        foreach (var patcher in patchers)
        {
            try
            {
                patcher.Apply(harmony);
            }
            catch (Exception e)
            {
                mod.Monitor.Log($"Failed to apply '{patcher.GetType().FullName}' patcher. Technical details:\n{e}", LogLevel.Error);
            }
        }
    }
}