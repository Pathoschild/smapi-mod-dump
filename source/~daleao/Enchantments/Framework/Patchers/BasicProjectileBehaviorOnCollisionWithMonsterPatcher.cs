/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Core.Framework.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
internal sealed class BasicProjectileBehaviorOnCollisionWithMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BasicProjectileBehaviorOnCollisionWithMonsterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal BasicProjectileBehaviorOnCollisionWithMonsterPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<BasicProjectile>(nameof(BasicProjectile.behaviorOnCollisionWithMonster));
    }

    #region harmony patches

    /// <summary>Do thunder strike.</summary>
    [HarmonyPrefix]
    private static void BasicProjectileBehaviorOnCollisionWithMonsterPrefix(BasicProjectile __instance, NPC n, GameLocation location)
    {
        if (n is Monster && Data.ReadAs<bool>(__instance, DataKeys.Energized))
        {
            location.DoLightningBarrage(n.Tile, 6, __instance.GetPlayerWhoFiredMe(location));
        }
    }

    #endregion harmony patches
}
