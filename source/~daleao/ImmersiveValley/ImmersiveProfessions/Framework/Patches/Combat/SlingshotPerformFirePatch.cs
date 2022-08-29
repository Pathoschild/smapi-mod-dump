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

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Xna;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Sounds;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Linq;
using System.Reflection;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotPerformFirePatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static readonly Lazy<Action<Slingshot>> _UpdateAimPos = new(() =>
        typeof(Slingshot).RequireMethod("updateAimPos").CompileUnboundDelegate<Action<Slingshot>>());

    /// <summary>Construct an instance.</summary>
    internal SlingshotPerformFirePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
        Prefix!.priority = Priority.High;
        Prefix!.after = new[] { "DaLion.ImmersiveArsenal" };
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and Ultimate.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyAfter("DaLion.ImmersiveArsenal")]
    private static bool SlingshotPerformFirePrefix(Slingshot __instance, ref bool ___canPlaySound,
        GameLocation location, Farmer who)
    {
        try
        {
            var hasQuincyEnchantment = __instance.enchantments.FirstOrDefault(e =>
                e.GetType().FullName?.Contains("Arsenal") == true &&
                e.GetType().FullName?.Contains("QuincyEnchantment") == true) is not null;
            if (__instance.attachments[0] is null && !hasQuincyEnchantment)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
                ___canPlaySound = true;
                return false; // don't run original logic
            }

            var backArmDistance = __instance.GetBackArmDistance(who);
            if (backArmDistance <= 4 || ___canPlaySound)
                return false; // don't run original logic

            // calculate projectile velocity
            _UpdateAimPos.Value(__instance);
            var mouseX = __instance.aimPos.X;
            var mouseY = __instance.aimPos.Y;
            var shootOrigin = __instance.GetShootOrigin(who);
            var (x, y) = StardewValley.Utility.getVelocityTowardPoint(shootOrigin, __instance.AdjustForHeight(new(mouseX, mouseY)),
                (15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));

            // calculate base ammo strength and properties
            var ammo = __instance.attachments[0]?.getOne();
            if (ammo is not null && --__instance.attachments[0].Stack <= 0)
                __instance.attachments[0] = null;

            var damageBase = ammo?.ParentSheetIndex switch
            {
                388 => 2, // wood
                390 => 5, // stone
                378 => 10, // copper ore
                380 => 20, // iron ore
                384 => 30, // gold ore
                386 => 50, // iridium ore
                909 => 75, // radioactive ore
                382 => 15, // coal
                441 => 20, // explosive
                766 => 5, // slime
                null => 5, // quincy
                _ => 1 // fish, fruit or vegetable
            };

            BasicProjectile.onCollisionBehavior? collisionBehavior;
            string collisionSound;
            switch (ammo?.ParentSheetIndex)
            {
                case 441:
                    collisionBehavior = BasicProjectile.explodeOnImpact;
                    collisionSound = "explosion";
                    break;
                case 909 or 766:
                    collisionBehavior = null;
                    collisionSound = ammo.ParentSheetIndex == 766 ? "slimedead" : "hammer";
                    break;
                case null:
                    collisionBehavior = null;
                    collisionSound = "debuffHit";
                    break;
                default:
                    collisionBehavior = null;
                    collisionSound = ammo.IsSquishyAmmo() ? "slimedead" : "hammer";
                    if (damageBase > 1) ++ammo.ParentSheetIndex;

                    break;
            }

            // calculate bonus ammo strength
            var damageMod = __instance.InitialParentTileIndex switch
            {
                33 => 2f,
                34 => 4f,
                _ => 1f
            } * (1f + __instance.GetEnchantmentLevel<RubyEnchantment>() + who.attackIncreaseModifier);

            if (who.HasProfession(Profession.Rascal))
                damageMod *= who.HasProfession(Profession.Rascal, true) ? 1.5f : 1.25f;

            // calculate overcharge
            var overcharge = 1f;
            if (who.HasProfession(Profession.Desperado) && !__instance.CanAutoFire())
                overcharge += __instance.GetDesperadoOvercharge(who);

            // adjust velocity
            if (overcharge > 1f)
            {
                x *= overcharge;
                y *= overcharge;
                who.stopJittering();
                SFX.SinWave?.Stop(AudioStopOptions.Immediate);
            }

            if (Game1.options.useLegacySlingshotFiring)
            {
                x *= -1f;
                y *= -1f;
            }

            // calculate bounces
            var bounces = 0;
            if (who.HasProfession(Profession.Rascal) && ammo?.IsMineralAmmo() == true &&
                ModEntry.Config.ModKey.IsDown())
            {
                ++bounces;
                damageMod *= 0.6f;
            }

            // add main projectile
            var startingPosition = shootOrigin - new Vector2(32f, 32f);
            var damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod * overcharge;
            var index = ammo?.ParentSheetIndex ?? 14;
            var projectile = new ImmersiveProjectile(__instance, overcharge, false, (int)damage, index,
                bounces, 0, (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), x, y, startingPosition,
                collisionSound, ammo is null ? "debuffSpell" : "", false, true, location, who, ammo is not null, collisionBehavior)
            {
                IgnoreLocationCollision = Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null
            };

            
            if (ammo is null) projectile.startingScale.Value *= overcharge * overcharge;

            location.projectiles.Add(projectile);

            // add auxiliary projectiles
            var velocity = new Vector2(x, y);
            var speed = velocity.Length();
            velocity.Normalize();
            if (who.IsLocalPlayer && who.get_Ultimate() is DeathBlossom { IsActive: true })
            {
                // do Death Blossom
                for (var i = 0; i < 7; ++i)
                {
                    velocity = velocity.Rotate(45);
                    if (i == 3) continue;

                    damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
                    var blossom = new ImmersiveProjectile(__instance, 1f, true, (int)damage, index, 0,
                        0, (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), velocity.X * speed,
                        velocity.Y * speed, startingPosition, collisionSound, string.Empty, false, true, location, who,
                        ammo is not null, collisionBehavior)
                    {
                        IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                                  Game1.currentMinigame is not null
                    };

                    location.projectiles.Add(blossom);
                }
            }
            else if (overcharge >= 1.5f && who.HasProfession(Profession.Desperado, true) &&
                     __instance.attachments[0].Stack >= 2)
            {
                // do spreadshot
                var angle = MathHelper.Lerp(1f, 0.5f, (overcharge - 1.5f) * 2f) * 15f;

                damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
                velocity = velocity.Rotate(angle);
                var clockwise = new ImmersiveProjectile(__instance, 1f, true, (int)damage, index, 0, index == 14 ? 5 : 0,
                    (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), velocity.X * speed, velocity.Y * speed,
                    startingPosition, collisionSound, string.Empty, false, true, location, who, ammo is not null, collisionBehavior)
                {
                    IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                              Game1.currentMinigame is not null
                };

                location.projectiles.Add(clockwise);

                damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
                velocity = velocity.Rotate(-2 * angle);
                var anticlockwise = new ImmersiveProjectile(__instance, 1f, true, (int)damage, index, 0, index == 14 ? 5 : 0,
                    (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), velocity.X * speed, velocity.Y * speed,
                    startingPosition, collisionSound, string.Empty, false, true, location, who, ammo is not null, collisionBehavior)
                {
                    IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                              Game1.currentMinigame is not null
                };

                location.projectiles.Add(anticlockwise);

                __instance.attachments[0].Stack -= 2;
                if (__instance.attachments[0].Stack <= 0)
                    __instance.attachments[0] = null;
            }

            ___canPlaySound = true;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}