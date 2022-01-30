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

using System;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
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

    /// <summary>Patch to increment Piper Eubstance gauge and heal on contact with slime.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeCollisionWithFarmerBehaviorPostfix(GreenSlime __instance)
    {
        var who = __instance.Player;
        if (!who.IsLocalPlayer || ModEntry.State.Value.SuperMode is not {Index: SuperModeIndex.Piper} superMode ||
            ModEntry.State.Value.SlimeContactTimer > 0) return;

        if (who.HasProfession(Profession.Piper, true))
        {
            var healed = __instance.DamageToFarmer / 2;
            healed += Game1.random.Next(Math.Min(-1, -healed / 8), Math.Max(1, healed / 8));
            healed = Math.Max(healed, 1);

            who.health = Math.Min(who.health + healed, who.maxHealth);
            __instance.currentLocation.debris.Add(new(healed,
                new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));
        }

        if (!superMode.IsActive)
            superMode.Gauge.CurrentValue +=
                Game1.random.Next(1, 5) * ModEntry.Config.SuperModeGainFactor * (double) SuperModeGauge.MaxValue / 500;

        ModEntry.State.Value.SlimeContactTimer = FARMER_INVINCIBILITY_FRAMES_I;
    }

    #endregion harmony patches
}