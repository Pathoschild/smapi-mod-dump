/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly, Deprecated]
internal sealed class GreenSlimeBehaviorAtGameTickPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GreenSlimeBehaviorAtGameTickPatch()
    {
        Target = RequireMethod<GreenSlime>(nameof(GreenSlime.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to countdown jump timers.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeBehaviorAtGameTickPostfix(GreenSlime __instance, ref int ___readyToJump)
    {
        var timeLeft = __instance.Read<int>("Jumping");
        if (timeLeft <= 0) return;

        timeLeft -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        __instance.Write("Jumping", timeLeft <= 0 ? null : timeLeft.ToString());

        //if (!__instance.Player.HasProfession(Profession.Piper)) return;

        //___readyToJump = -1;
    }

    #endregion harmony patches
}