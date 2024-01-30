/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hikari-BS/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using SObject = StardewValley.Object;

namespace FixedWeaponsDamage
{
    internal class SlingshotPatches
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static float DamageMultiplier;

        internal static void Initialize(IMonitor monitor, IModHelper helper, float damageMultiplier)
        {
            Monitor = monitor; Helper = helper; DamageMultiplier = damageMultiplier;
        }

        public static bool PerformFire_Prefix(Slingshot __instance, ref bool ___canPlaySound, GameLocation location, Farmer who)
        {
            try
            {
                if (__instance.attachments[0] != null)
                {
                    Helper.Reflection.GetMethod(new Slingshot(), "updateAimPos").Invoke();
                    int mouseX = __instance.aimPos.X;
                    int mouseY = __instance.aimPos.Y;
                    int backArmDistance = __instance.GetBackArmDistance(who);
                    Vector2 shoot_origin = __instance.GetShootOrigin(who);
                    Vector2 v = Utility.getVelocityTowardPoint(__instance.GetShootOrigin(who), __instance.AdjustForHeight(new Vector2(mouseX, mouseY)), (float)(15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));
                    if (backArmDistance > 4 && !___canPlaySound)
                    {
                        SObject ammunition = (SObject)__instance.attachments[0].getOne();
                        __instance.attachments[0].Stack--;
                        if (__instance.attachments[0].Stack <= 0)
                        {
                            __instance.attachments[0] = null;
                        }
                        int damage = 1;
                        BasicProjectile.onCollisionBehavior collisionBehavior = null;
                        string collisionSound = "hammer";
                        float damageMod = 1f;
                        if (__instance.InitialParentTileIndex == 33)
                        {
                            damageMod = 2f;
                        }
                        else if (__instance.InitialParentTileIndex == 34)
                        {
                            damageMod = 4f;
                        }
                        switch (ammunition.ParentSheetIndex)
                        {
                            case 388:
                                damage = 2;
                                ammunition.ParentSheetIndex++;
                                break;
                            case 390:
                                damage = 5;
                                ammunition.ParentSheetIndex++;
                                break;
                            case 378:
                                damage = 10;
                                ammunition.ParentSheetIndex++;
                                break;
                            case 380:
                                damage = 20;
                                ammunition.ParentSheetIndex++;
                                break;
                            case 384:
                                damage = 30;
                                ammunition.ParentSheetIndex++;
                                break;
                            case 382:
                                damage = 15;
                                ammunition.ParentSheetIndex++;
                                break;
                            case 386:
                                damage = 50;
                                ammunition.ParentSheetIndex++;
                                break;
                            case 441:
                                damage = 20;
                                collisionBehavior = BasicProjectile.explodeOnImpact;
                                collisionSound = "explosion";
                                break;
                        }
                        if (ammunition.Category == -5)
                        {
                            collisionSound = "slimedead";
                        }
                        if (!Game1.options.useLegacySlingshotFiring)
                        {
                            v.X *= -1f;
                            v.Y *= -1f;
                        }
                        location.projectiles.Add(new BasicProjectile((int)(damageMod * Math.Ceiling((damage * 2 + 1) * DamageMultiplier) * (1f + who.attackIncreaseModifier)), ammunition.ParentSheetIndex, 0, 0, (float)(Math.PI / (double)(64f + Game1.random.Next(-63, 64))), 0f - v.X, 0f - v.Y, shoot_origin - new Vector2(32f, 32f), collisionSound, "", explode: false, damagesMonsters: true, location, who, spriteFromObjectSheet: true, collisionBehavior)
                        {
                            IgnoreLocationCollision = (Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
                        });
                    }
                }
                else
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
                }
                ___canPlaySound = true;
                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error when patching PerformFire(): {ex}", LogLevel.Error);
                return true;
            }
        }
    }
}