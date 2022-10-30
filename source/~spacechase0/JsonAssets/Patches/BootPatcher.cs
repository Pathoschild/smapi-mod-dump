/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;

using HarmonyLib;

using Spacechase.Shared.Patching;

using SpaceShared;

using StardewModdingAPI;

using StardewValley.Objects;

namespace JsonAssets.Patches;
internal class BootPatcher : BasePatcher
{

    public override void Apply(Harmony harmony, IMonitor monitor)
    {
        harmony.Patch(
            original: this.RequireMethod<Boots>("loadDisplayFields"),
            finalizer: this.GetHarmonyMethod(nameof(FinalizeBootDisplayFields))
            );
    }

#nullable enable
    private static Exception? FinalizeBootDisplayFields(Boots __instance, Exception __exception)
    {
        if (__exception is not null)
        {
            Log.Warn($"{__instance.indexInTileSheet} corresponds to a pair of boots not found in Data/Boots!");
        }
        return null;
    }
}
