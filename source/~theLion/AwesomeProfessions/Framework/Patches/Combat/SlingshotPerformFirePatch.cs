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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using StardewValley.Tools;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;
using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class SlingshotPerformFirePatch : BasePatch
{
    private const float QUICK_FIRE_HANDICAP_F = 1.2f;

    private static readonly FieldInfo _XVelocity = typeof(BasicProjectile).Field("xVelocity");
    private static readonly FieldInfo _YVelocity = typeof(BasicProjectile).Field("yVelocity");
    private static readonly FieldInfo _CurrentTileSheetIndex = typeof(BasicProjectile).Field("currentTileSheetIndex");
    private static readonly FieldInfo _Position = typeof(BasicProjectile).Field("position");
    private static readonly FieldInfo _CollisionSound = typeof(BasicProjectile).Field("collisionSound");
    private static readonly FieldInfo _CollisionBehavior = typeof(BasicProjectile).Field("collisionBehavior");

    /// <summary>Construct an instance.</summary>
    internal SlingshotPerformFirePatch()
    {
        Original = RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
    }

    #region harmony patches

    /// <summary>Patch to perform Desperado Super Mode.</summary>
    [HarmonyPostfix]
    private static void SlingshotPerformFirePostfix(GameLocation location, Farmer who)
    {
        if (!who.HasProfession(Profession.Desperado) ||
            location.projectiles.LastOrDefault() is not BasicProjectile mainProjectile) return;

        // get bullet properties
        var damage = mainProjectile.damageToFarmer;
        var xVelocity = ((NetFloat) _XVelocity.GetValue(mainProjectile))!.Value;
        var yVelocity = ((NetFloat) _YVelocity.GetValue(mainProjectile))!.Value;
        var ammunitionIndex = ((NetInt) _CurrentTileSheetIndex.GetValue(mainProjectile))!.Value;
        var startingPosition = ((NetPosition) _Position.GetValue(mainProjectile))!.Value;
        var collisionSound = ((NetString) _CollisionSound.GetValue(mainProjectile))!.Value;
        var collisionBehavior = (BasicProjectile.onCollisionBehavior) _CollisionBehavior.GetValue(mainProjectile);

        var velocity = new Vector2(xVelocity * -1f, yVelocity * -1f);
        var speed = velocity.Length();
        velocity.Normalize();
        if (who.IsLocalPlayer && ModEntry.PlayerState.Value.SuperMode is PoacherColdBlood {IsActive: true})
        {
            // do Death Blossom
            for (var i = 0; i < 7; ++i)
            {
                velocity.Rotate(45);
                var blossom = new BasicProjectile(damage, ammunitionIndex, 0, 0,
                    (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - velocity.X * speed,
                    0f - velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                    true, location, who, true, collisionBehavior)
                {
                    IgnoreLocationCollision =
                        Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null
                };

                location.projectiles.Add(blossom);
                ModEntry.PlayerState.Value.AuxiliaryBullets.Add(blossom.GetHashCode());
            }
        }
        else if (Game1.random.NextDouble() < who.GetDesperadoDoubleStrafeChance())
        {
            if (who.HasProfession(Profession.Desperado, true))
            {
                // do spreadshot
                velocity.Rotate(15);
                var clockwise = new BasicProjectile(damage, ammunitionIndex, 0, 0,
                    (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - velocity.X * speed,
                    0f - velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                    true, location, who, true, collisionBehavior)
                {
                    IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                              Game1.currentMinigame is not null
                };

                location.projectiles.Add(clockwise);
                ModEntry.PlayerState.Value.AuxiliaryBullets.Add(clockwise.GetHashCode());

                velocity.Rotate(-30);
                var anticlockwise = new BasicProjectile(damage, ammunitionIndex, 0, 0,
                    (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - velocity.X * speed,
                    0f - velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                    true, location, who, true, collisionBehavior)
                {
                    IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                              Game1.currentMinigame is not null
                };

                location.projectiles.Add(anticlockwise);
                ModEntry.PlayerState.Value.AuxiliaryBullets.Add(anticlockwise.GetHashCode());
            }
            else
            {
                // do double strafe
                var secondary = new BasicProjectile((int) (damage.Value * 0.6f), ammunitionIndex, 0, 0,
                    (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - velocity.X * speed,
                    0f - velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                    true, location, who, true, collisionBehavior)
                {
                    IgnoreLocationCollision =
                        Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null
                };

                DelayedAction doubleStrafe = new(50, () => { location.projectiles.Add(secondary); });
                Game1.delayedActions.Add(doubleStrafe);
                ModEntry.PlayerState.Value.AuxiliaryBullets.Add(secondary.GetHashCode());
            }
        }
    }

    /// <summary>Patch to increment Desperado Temerity gauge + add Desperado quick fire projectile velocity bonus.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> SlingshotPerformFireTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: PerformFireSubroutine(this, damage, velocityTowardPoint, location, who)
        /// Before: if (ammunition.Category == -5) collisionSound = "slimedead";

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Stloc_S, $"{typeof(int)} (5)")
                )
                .GetOperand(out var damage)
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloca_S, $"{typeof(Vector2)} (3)")
                )
                .GetOperand(out var velocity) // copy reference to local 3 = v (velocity)
                .FindFirst( // find index of ammunition.Category
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Item).PropertyGetter(nameof(Item.Category)))
                )
                .Retreat()
                .StripLabels(out var labels) // backup and remove branch labels
                .Insert(
                    // restore backed-up labels
                    labels,
                    // load arguments
                    new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Slingshot instance
                    new CodeInstruction(OpCodes.Ldloca_S, damage),
                    new CodeInstruction(OpCodes.Ldloca_S, velocity),
                    new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = GameLocation location
                    new CodeInstruction(OpCodes.Ldarg_2), // arg 2 = Farmer who
                    new CodeInstruction(OpCodes.Call,
                        typeof(SlingshotPerformFirePatch).MethodNamed(nameof(PerformFireSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while injecting modded Desperado ammunition damage modifier, Temerity gauge and quick shots.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void PerformFireSubroutine(Slingshot slingshot, ref int damage, ref Vector2 velocity, GameLocation location, Farmer who)
    {
        if (!who.IsLocalPlayer || ModEntry.PlayerState.Value.SuperMode is not DesperadoTemerity {IsActive: false} desperadoTemerity ||
            !location.IsCombatZone()) return;

        var bulletPower = desperadoTemerity.GetShootingPower();
        velocity *= bulletPower;
        damage *= (int) (1 + (bulletPower - 1) / 2.5);

        var increment = 12 - 10 * who.health / who.maxHealth;
        if (Game1.currentGameTime.TotalGameTime.TotalSeconds - slingshot.pullStartTime <=
            slingshot.GetRequiredChargeTime() * QUICK_FIRE_HANDICAP_F)
            increment += 6;

        ModEntry.PlayerState.Value.SuperMode.ChargeValue += increment * ModEntry.Config.SuperModeGainFactor *
            SuperMode.MaxValue / SuperMode.INITIAL_MAX_VALUE_I;
    }

    #endregion injected subroutines
}