/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerForceCanMovePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerForceCanMovePatcher"/> class.</summary>
    internal FarmerForceCanMovePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.forceCanMove));
    }

    #region harmony patches

    /// <summary>Reset animation state.</summary>
    [HarmonyPostfix]
    private static void FarmerForceCanMovePostfix(Farmer __instance)
    {
        if (!__instance.IsLocalPlayer)
        {
            return;
        }

        CombatModule.State.FarmerAnimating = false;
        if (CombatModule.Config.ControlsUi.SlickMoves)
        {
            CombatModule.State.DriftVelocity = Vector2.Zero;
        }
    }

    #endregion harmony patches
}
