/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotAttachPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotAttachPatcher"/> class.</summary>
    internal SlingshotAttachPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.attach));
    }

    #region harmony patches

    /// <summary>Recalculate bonuses when arrows are attached.</summary>
    [HarmonyPostfix]
    private static void SlingshotAttachPostfix(Slingshot __instance, SObject? o)
    {
        __instance.Invalidate();
    }

    #endregion harmony patches
}
