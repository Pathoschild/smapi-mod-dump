/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingUpdatePatcher"/> class.</summary>
    internal CombinedRingUpdatePatcher()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.update));
    }

    #region harmony patches

    /// <summary>Update Infinity Band resonances.</summary>
    [HarmonyPostfix]
    private static void CombinedRingUpdatePostfix(CombinedRing __instance, Farmer who)
    {
        __instance.Get_Chord()?.Update(who);
    }

    #endregion harmony patches
}
