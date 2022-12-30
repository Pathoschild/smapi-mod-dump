/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCanMoveNowPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCanMoveNowPatcher"/> class.</summary>
    internal FarmerCanMoveNowPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.canMoveNow));
    }

    #region harmony patches

    /// <summary>Reset animation state.</summary>
    [HarmonyPostfix]
    private static void FarmerCanMoveNowPostfix(Farmer who)
    {
        if (!who.IsLocalPlayer)
        {
            return;
        }

        ArsenalModule.State.FarmerAnimating = false;
        if (ArsenalModule.Config.SlickMoves)
        {
            ArsenalModule.State.DriftVelocity = Vector2.Zero;
        }
    }

    #endregion harmony patches
}
