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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JsonAssets.Data;
using Netcode;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
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

    private static Exception? FinalizeBootDisplayFields(Boots __instance, Exception __exception)
    {
        if (__exception is not null)
        {
            Log.Warn($"{__instance.indexInTileSheet} corresponds to a pair of boots not found in Data/Boots!");
        }
        return null;
    }
}
