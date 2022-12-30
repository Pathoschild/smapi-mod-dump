/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeCollisionWithFarmerBehaviorPatcher : HarmonyPatcher
{
    private const int FarmerInvincibilityFrames = 72;

    /// <summary>Initializes a new instance of the <see cref="GreenSlimeCollisionWithFarmerBehaviorPatcher"/> class.</summary>
    internal GreenSlimeCollisionWithFarmerBehaviorPatcher()
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.collisionWithFarmerBehavior));
    }

    #region harmony patches

    /// <summary>Patch to increment Piper Ultimate meter on contact with Slime.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeCollisionWithFarmerBehaviorPostfix(GreenSlime __instance)
    {
        if (!__instance.currentLocation.IsDungeon())
        {
            return;
        }

        var who = __instance.Player;
        if (!who.IsLocalPlayer || who.Get_Ultimate() is not Concerto { IsActive: false } concerto ||
            concerto.SlimeContactTimer > 0)
        {
            return;
        }

        concerto.ChargeValue += Game1.random.Next(1, 4);
        concerto.SlimeContactTimer = FarmerInvincibilityFrames;
        EventManager.Enable<PiperUpdateTickedEvent>();
    }

    #endregion harmony patches
}
