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
internal sealed class SlingshotDrawAttachmentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotDrawAttachmentsPatcher"/> class.</summary>
    internal SlingshotDrawAttachmentsPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.drawAttachments));
    }

    #region harmony patches

    /// <summary>Add small buffer space between stats and ammo slots.</summary>
    [HarmonyPrefix]
    private static void SlingshotDrawAttachmentsPrefix(Slingshot __instance, ref int y)
    {
        y += __instance.Get_SpaceBeforeAmmoSlots();
    }

    #endregion harmony patches
}
