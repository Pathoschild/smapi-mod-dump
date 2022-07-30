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

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;
using Ultimates;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeCollisionWithFarmerBehaviorPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private const int FARMER_INVINCIBILITY_FRAMES_I = 72;

    /// <summary>Construct an instance.</summary>
    internal GreenSlimeCollisionWithFarmerBehaviorPatch()
    {
        Target = RequireMethod<GreenSlime>(nameof(GreenSlime.collisionWithFarmerBehavior));
    }

    #region harmony patches

    /// <summary>Patch to increment Piper Ultimate meter on contact with Slime.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeCollisionWithFarmerBehaviorPostfix(GreenSlime __instance)
    {
        if (!__instance.currentLocation.IsDungeon()) return;

        var who = __instance.Player;
        if (!who.IsLocalPlayer ||
            ModEntry.PlayerState.RegisteredUltimate is not Pandemic { IsActive: false } pandemic ||
            ModEntry.PlayerState.SlimeContactTimer > 0) return;

        pandemic.ChargeValue += Game1.random.Next(1, 4);
        ModEntry.PlayerState.SlimeContactTimer = FARMER_INVINCIBILITY_FRAMES_I;
    }

    #endregion harmony patches
}