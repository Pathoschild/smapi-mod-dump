/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;

using Extensions;
using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class GreenSlimeCollisionWithFarmerBehaviorPatch : BasePatch
{
    private const int FARMER_INVINCIBILITY_FRAMES_I = 72;

    /// <summary>Construct an instance.</summary>
    internal GreenSlimeCollisionWithFarmerBehaviorPatch()
    {
        Original = RequireMethod<GreenSlime>(nameof(GreenSlime.collisionWithFarmerBehavior));
    }

    #region harmony patches

    /// <summary>Patch to increment Piper Eubstance gauge on contact with Slime.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeCollisionWithFarmerBehaviorPostfix(GreenSlime __instance)
    {
        if (!__instance.currentLocation.IsCombatZone()) return;

        var who = __instance.Player;
        if (!who.IsLocalPlayer || ModEntry.PlayerState.Value.SuperMode is not PiperEubstance eubstance ||
            ModEntry.PlayerState.Value.SlimeContactTimer > 0) return;

        if (!eubstance.IsActive)
            eubstance.ChargeValue +=
                Game1.random.Next(1, 4) * ModEntry.Config.SuperModeGainFactor * SuperMode.MaxValue / SuperMode.INITIAL_MAX_VALUE_I;

        ModEntry.PlayerState.Value.SlimeContactTimer = FARMER_INVINCIBILITY_FRAMES_I;
    }

    #endregion harmony patches
}