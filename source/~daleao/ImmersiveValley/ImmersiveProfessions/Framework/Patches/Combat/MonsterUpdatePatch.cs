/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterUpdatePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterUpdatePatch()
    {
        Target = RequireMethod<Monster>(nameof(Monster.update), new[] { typeof(GameTime), typeof(GameLocation) });
        Prefix!.priority = Priority.First;
    }

    #region harmony patches

    /// <summary>Patch to implement slow.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static bool MonsterUpdatePrefix(Monster __instance, ref int ___invincibleCountdown, GameTime time, GameLocation location)
    {
        var slowTimer = __instance.get_SlowTimer();
        if (slowTimer.Value <= 0) return true; // run original logic

        slowTimer.Value -= time.ElapsedGameTime.Milliseconds;
        var slowIntensity = __instance.get_SlowIntensity();
        __instance.startGlowing(Color.LimeGreen, false, 0.05f);
        return time.TotalGameTime.Ticks % slowIntensity.Value == 0; // conditionally run original logic
    }

    #endregion harmony patches
}