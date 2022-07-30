/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotPerformFirePatch : Common.Harmony.HarmonyPatch
{
    private static Action<Slingshot>? _UpdateAimPos;

    /// <summary>Construct an instance.</summary>
    internal SlingshotPerformFirePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
        Prefix!.after = new[] { "DaLion.ImmersiveProfessions" };
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and Ultimate.</summary>
    [HarmonyPrefix]
    [HarmonyAfter("DaLion.ImmersiveProfessions")]
    private static bool SlingshotPerformFirePrefix(Slingshot __instance, ref bool ___canPlaySound, GameLocation location, Farmer who)
    {
        try
        {
            if (__instance.attachments[0] is null)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
                ___canPlaySound = true;
                return false; // don't run original logic
            }

            var backArmDistance = __instance.GetBackArmDistance(who);
            if (backArmDistance <= 4 || ___canPlaySound)
                return false; // don't run original logic

            _UpdateAimPos ??= typeof(Slingshot).RequireMethod("updateAimPos")
                .CompileUnboundDelegate<Action<Slingshot>>();
            _UpdateAimPos(__instance);
            var mouseX = __instance.aimPos.X;
            var mouseY = __instance.aimPos.Y;
            var shootOrigin = __instance.GetShootOrigin(who);
            var (x, y) = Utility.getVelocityTowardPoint(shootOrigin, __instance.AdjustForHeight(new(mouseX, mouseY)),
                (15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));

            var ammo = __instance.attachments[0].getOne();
            if (--__instance.attachments[0].Stack <= 0)
                __instance.attachments[0] = null;

            var damageBase = ammo.ParentSheetIndex switch
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
                _ => 1
            };

            BasicProjectile.onCollisionBehavior? collisionBehavior;
            string collisionSound;
            switch (ammo.ParentSheetIndex)
            {
                case 441:
                    collisionBehavior = BasicProjectile.explodeOnImpact;
                    collisionSound = "explosion";
                    break;
                case 909:
                    collisionBehavior = null;
                    collisionSound = "hammer";
                    break;
                default:
                    collisionBehavior = null;
                    collisionSound = ammo.Category == -4 ? "slimedead" : "hammer";
                    ++ammo.ParentSheetIndex;
                    break;
            }

            var damageMod = __instance.InitialParentTileIndex switch
            {
                33 => 2f,
                34 => 4f,
                _ => 1f
            } * (1f + __instance.GetEnchantmentLevel<RubyEnchantment>() + who.attackIncreaseModifier);

            if (Game1.options.useLegacySlingshotFiring)
            {
                x *= -1f;
                y *= -1f;
            }

            var startingPosition = shootOrigin - new Vector2(32f, 32f);
            var damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
            var projectile = new ImmersiveProjectile(__instance, (int)damage, ammo.ParentSheetIndex, 0, 0,
                (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), x, y, startingPosition,
                collisionSound, "", false, true, location, who, true, collisionBehavior)
            {
                IgnoreLocationCollision = Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null
            };
            location.projectiles.Add(projectile);

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