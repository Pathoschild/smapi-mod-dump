/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnNewLocationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnNewLocationPatcher"/> class.</summary>
    internal CombinedRingOnNewLocationPatcher()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onNewLocation));
    }

    #region harmony patches

    /// <summary>Add Infinity Band resonance location effects.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnNewLocationPostfix(CombinedRing __instance, GameLocation environment)
    {
        __instance.Get_Chord()?.OnNewLocation(environment);
    }

    #endregion harmony patches
}
