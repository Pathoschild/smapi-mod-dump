/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolActionWhenStopBeingHeldPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolActionWhenStopBeingHeldPatcher"/> class.</summary>
    internal ToolActionWhenStopBeingHeldPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.actionWhenStopBeingHeld));
    }

    #region harmony patches

    /// <summary>Reset combo hit counter.</summary>
    [HarmonyPostfix]
    private static void ToolActionWhenStopBeingHeldPostfix(Tool __instance, Farmer who)
    {
        if (__instance is not MeleeWeapon || !who.IsLocalPlayer)
        {
            return;
        }

        WeaponsModule.State.ComboHitQueued = ComboHitStep.Idle;
        WeaponsModule.State.ComboHitStep = ComboHitStep.Idle;
    }

    #endregion harmony patches
}
