/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using HarmonyLib;

namespace CommunityUpgradeFramework;

internal class HarmonyPatcher
{
    internal static readonly Harmony Harmony = new(Globals.UUID);

    internal static void ApplyPatches()
    {
        Log.Trace("Patching methods.");

        try
        {
            Harmony.PatchAll();
        }
        catch (Exception ex)
        {
            Log.Error($"Exception encountered while patching: {ex}");
        }

    }
}
