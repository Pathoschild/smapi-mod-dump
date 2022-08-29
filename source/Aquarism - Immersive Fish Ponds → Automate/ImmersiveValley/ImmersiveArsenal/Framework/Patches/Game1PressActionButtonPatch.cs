/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley.Tools;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1PressActionButtonPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal Game1PressActionButtonPatch()
    {
        Target = RequireMethod<Game1>(nameof(Game1.pressActionButton));
    }

    #region harmony patches

    /// <summary>Immersively adjust Marlon's intro event.</summary>
    [HarmonyPostfix]
    private static void Game1PressActionButtonPostfix(ref bool __result, KeyboardState currentKBState, MouseState currentMouseState, GamePadState currentPadState)
    {
        if (!__result || !ModEntry.Config.EnableSlingshotSpecialMove) return;

        var player = Game1.player;
        if (player.CurrentTool is not Slingshot slingshot || slingshot.get_IsOnSpecial() || player.usingSlingshot ||
            !player.CanMove || player.canOnlyWalk || Game1.eventUp || player.onBridge.Value ||
            !Game1.didPlayerJustRightClick(true)) return;

        slingshot.AnimateSpecialMove();
        __result = false;
    }

    #endregion harmony patches
}