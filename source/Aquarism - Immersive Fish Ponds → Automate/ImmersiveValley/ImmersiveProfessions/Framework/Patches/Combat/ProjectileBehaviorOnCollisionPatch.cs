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

using DaLion.Common.Extensions;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class ProjectileBehaviorOnCollisionPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ProjectileBehaviorOnCollisionPatch()
    {
        Target = RequireMethod<Projectile>("behaviorOnCollision");
    }

    #region harmony patches

    /// <summary>Patch for Rascal chance to recover ammunition.</summary>
    [HarmonyPostfix]
    private static void ProjectileBehaviorOnCollisionPostfix(Projectile __instance, NetInt ___currentTileSheetIndex,
        NetPosition ___position, NetCharacterRef ___theOneWhoFiredMe, GameLocation location)
    {
        if (__instance is not BasicProjectile projectile) return;

        var hashCode = projectile.GetHashCode();
        ModEntry.PlayerState.BouncedBullets.Remove(hashCode);
        if (ModEntry.PlayerState.BlossomBullets.Remove(hashCode)) return;

        var firer = ___theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
        if (!firer.HasProfession(Profession.Rascal)) return;

        if ((___currentTileSheetIndex.Value - 1).IsIn(SObject.copper, SObject.iron, SObject.gold,
                SObject.iridium, SObject.stone) && Game1.random.NextDouble() < 0.6
            || ___currentTileSheetIndex.Value == SObject.wood + 1 && Game1.random.NextDouble() < 0.3)
            location.debris.Add(new(___currentTileSheetIndex.Value - 1,
                new((int)___position.X, (int)___position.Y), firer.getStandingPosition()));
    }

    #endregion harmony patches
}