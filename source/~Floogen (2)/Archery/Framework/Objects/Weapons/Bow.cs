/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces.Internal;
using Archery.Framework.Interfaces.Internal.Events;
using Archery.Framework.Models.Enums;
using Archery.Framework.Models.Weapons;
using Archery.Framework.Objects.Items;
using Archery.Framework.Objects.Projectiles;
using Archery.Framework.Patches.Objects;
using Archery.Framework.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using static Archery.Framework.Interfaces.IFashionSenseApi;
using Object = StardewValley.Object;

namespace Archery.Framework.Objects.Weapons
{
    internal class Bow : InstancedObject
    {
        internal static int ActiveCooldown = 0;
        internal static float CooldownAdditiveScale = 0;

        public static Slingshot CreateInstance(WeaponModel weaponModel)
        {
            var bow = new Slingshot();
            bow.modData[ModDataKeys.WEAPON_FLAG] = weaponModel.Id;

            // Hide attachment slot from bows with internal ammo
            if (weaponModel.UsesInternalAmmo())
            {
                bow.numAttachmentSlots.Value = 0;
            }

            return bow;
        }

        public static Slingshot CreateRecipe(WeaponModel weaponModel)
        {
            var recipe = CreateInstance(weaponModel);
            recipe.modData[ModDataKeys.RECIPE_FLAG] = true.ToString();

            return recipe;
        }

        internal static bool Use(Tool tool, GameLocation location, int x, int y, Farmer who)
        {
            return false;
        }

        internal static bool IsFiring(Farmer who, Tool tool)
        {
            return Bow.IsLoaded(tool) && who.usingSlingshot;
        }

        internal static bool IsLoaded(Tool tool)
        {
            return Bow.GetLoaded(tool) > 0;
        }

        internal static int GetLoaded(Tool tool)
        {
            if (Bow.IsValid(tool) is true && tool.modData.TryGetValue(ModDataKeys.IS_LOADED_FLAG, out var rawLoadedAmmoCount) && int.TryParse(rawLoadedAmmoCount, out int parsedLoadedAmmoCount))
            {
                return parsedLoadedAmmoCount;
            }

            return 0;
        }

        internal static void SetLoaded(Tool tool, int ammoCount)
        {
            if (tool is not null)
            {
                tool.modData[ModDataKeys.IS_LOADED_FLAG] = ammoCount.ToString();
            }
        }

        internal static bool CanUseSpecialAttack(Tool tool)
        {
            if (Bow.GetModel<WeaponModel>(tool) is WeaponModel weaponModel && weaponModel.SpecialAttack is not null && Bow.IsUsingSpecialAttack(tool) is false && ActiveCooldown <= 0 && Game1.activeClickableMenu is null)
            {
                return true;
            }

            return false;
        }

        internal static bool IsUsingSpecialAttack(Tool tool)
        {
            if (Bow.IsValid(tool) && tool.modData.TryGetValue(ModDataKeys.IS_USING_SPECIAL_ATTACK_FLAG, out string rawIsUsingSpecial) && bool.TryParse(rawIsUsingSpecial, out bool parsedIsUsingSpecial))
            {
                return parsedIsUsingSpecial;
            }

            return false;
        }

        internal static void SetUsingSpecialAttack(Tool tool, bool state)
        {
            if (Bow.GetModel<WeaponModel>(tool) is WeaponModel weaponModel)
            {
                if (state)
                {
                    RefreshSpecialAttackCooldown(tool, weaponModel.SpecialAttack);
                }

                tool.modData[ModDataKeys.IS_USING_SPECIAL_ATTACK_FLAG] = state.ToString();
            }
        }

        internal static void RefreshSpecialAttackCooldown(Tool tool, SpecialAttackModel specialAttack)
        {
            Bow.ActiveCooldown = Archery.internalApi.GetSpecialAttackCooldown(specialAttack.Id, specialAttack.Arguments);

            if (tool.getLastFarmerToUse() is Farmer farmer && farmer is not null && farmer.professions.Contains(Farmer.acrobat))
            {
                Bow.ActiveCooldown /= 2;
            }
        }

        internal static IWeaponData GetData(Tool tool)
        {
            if (Bow.GetModel<WeaponModel>(tool) is WeaponModel weaponModel)
            {
                return new WeaponData()
                {
                    WeaponId = weaponModel.Id,
                    WeaponType = weaponModel.Type,
                    AmmoInMagazine = weaponModel.Type is WeaponType.Crossbow ? Bow.GetLoaded(tool) : null,
                    MagazineSize = weaponModel.Type is WeaponType.Crossbow ? weaponModel.MagazineSize : null,
                    ChargeTimeRequiredMilliseconds = weaponModel.ChargeTimeRequiredMilliseconds,
                    ProjectileSpeed = weaponModel.ProjectileSpeed,
                    DamageRange = weaponModel.DamageRange
                };
            }

            return null;
        }

        internal static bool CanThisBeAttached(Tool tool, Object item)
        {
            if (Bow.GetModel<WeaponModel>(tool) is WeaponModel weaponModel && weaponModel.UsesInternalAmmo() is false)
            {
                if (item is null)
                {
                    return true;
                }

                if (Arrow.GetModel<AmmoModel>(item) is AmmoModel ammoModel)
                {
                    if (weaponModel.IsFilterDefined())
                    {
                        return weaponModel.IsWithinFilter(ammoModel.Id);
                    }
                    else if (ammoModel.IsFilterDefined())
                    {
                        return ammoModel.IsWithinFilter(weaponModel.Id);
                    }

                    return weaponModel.IsValidAmmoType(ammoModel.Type);
                }
            }

            return false;
        }

        internal static float GetSlingshotChargeTime(Tool tool)
        {
            if (Bow.IsValid(tool) is true && tool is Slingshot slingshot && Bow.GetModel<WeaponModel>(tool) is WeaponModel weaponModel)
            {
                if (Bow.IsLoaded(tool))
                {
                    return 1f;
                }

                var requiredChargingTime = weaponModel.ChargeTimeRequiredMilliseconds;
                var pullStartTimeInMilliseconds = slingshot.pullStartTime * 1000;

                return Utility.Clamp((float)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds - pullStartTimeInMilliseconds) / (double)requiredChargingTime), 0f, 1f);
            }

            return 0f;
        }

        internal static void SetSlingshotChargeTime(Tool tool, float percentage)
        {
            if (Bow.IsValid(tool) is true && tool is Slingshot slingshot && Bow.GetModel<WeaponModel>(tool) is WeaponModel weaponModel)
            {
                var currentMilliseconds = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
                var requiredChargingTime = weaponModel.ChargeTimeRequiredMilliseconds;

                percentage = Utility.Clamp(percentage, 0f, 1f);
                slingshot.pullStartTime = Math.Abs(currentMilliseconds - (percentage * requiredChargingTime)) / 1000;
            }
        }

        internal static Object GetAmmoItem(Tool tool)
        {
            if (Bow.GetModel<WeaponModel>(tool) is WeaponModel weaponModel)
            {
                if (tool is Slingshot slingshot && slingshot.attachments is not null && slingshot.attachments[0] is not null)
                {
                    return slingshot.attachments[0];
                }
                else if (weaponModel.UsesInternalAmmo())
                {
                    return Arrow.CreateInstance(Archery.modelManager.GetSpecificModel<AmmoModel>(weaponModel.GetInternalAmmoId()));
                }
            }

            return null;
        }

        internal static int GetAmmoCount(Tool tool)
        {
            if (GetAmmoItem(tool) is Object attachment && attachment is not null)
            {
                return attachment.Stack;
            }

            return 0;
        }

        internal static void TickUpdate(Slingshot slingshot, ref bool canPlaySound, ref Farmer lastUser, NetEvent0 finishEvent, GameTime time, Farmer who)
        {
            var weaponModel = Bow.GetModel<WeaponModel>(slingshot);
            if (weaponModel is null)
            {
                return;
            }

            // Skip if the weapon isn't the current tool
            if (who.CurrentTool != slingshot)
            {
                return;
            }

            lastUser = who;
            finishEvent.Poll();

            if (who.usingSlingshot is false && who.CurrentTool == slingshot && Toolkit.AreSpecialAttackButtonsPressed() && Bow.CanUseSpecialAttack(slingshot) is true)
            {
                SetUsingSpecialAttack(slingshot, true);
                slingshot.pullStartTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;
            }
            else if (who.usingSlingshot is false && Bow.IsUsingSpecialAttack(slingshot) is false)
            {
                return;
            }

            if (who.IsLocalPlayer)
            {
                var currentChargeTime = slingshot.GetSlingshotChargeTime();
                if (weaponModel.Type is WeaponType.Crossbow && Bow.IsLoaded(slingshot) is false && currentChargeTime >= 1f)
                {
                    Toolkit.SuppressToolButtons();

                    var currentAmmoCount = Bow.GetAmmoCount(slingshot);
                    if (currentAmmoCount <= 0)
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
                        return;
                    }
                    Bow.SetLoaded(slingshot, currentAmmoCount < weaponModel.MagazineSize ? currentAmmoCount : weaponModel.MagazineSize);

                    // Trigger event
                    var ammoModel = Arrow.GetModel<AmmoModel>(Bow.GetAmmoItem(slingshot));
                    Archery.internalApi.TriggerOnCrossbowLoaded(new CrossbowLoadedEventArgs() { WeaponId = weaponModel.Id, AmmoId = ammoModel is null ? string.Empty : ammoModel.Id, Origin = who.getStandingPosition() });

                    return;
                }

                SlingshotPatch.UpdateAimPosReversePatch(slingshot);
                int mouseX = slingshot.aimPos.X;
                int mouseY = slingshot.aimPos.Y;

                Game1.debugOutput = "playerPos: " + who.getStandingPosition().ToString() + ", mousePos: " + mouseX + ", " + mouseY;
                slingshot.mouseDragAmount++;

                Vector2 shootOrigin = slingshot.GetShootOrigin(who);
                Vector2 aimOffset = slingshot.AdjustForHeight(new Vector2(mouseX, mouseY)) - shootOrigin;
                if (Math.Abs(aimOffset.X) > Math.Abs(aimOffset.Y))
                {
                    if (aimOffset.X < 0f)
                    {
                        who.faceDirection(3);
                    }

                    if (aimOffset.X > 0f)
                    {
                        who.faceDirection(1);
                    }
                }
                else
                {
                    if (aimOffset.Y < 0f)
                    {
                        who.faceDirection(0);
                    }

                    if (aimOffset.Y > 0f)
                    {
                        who.faceDirection(2);
                    }
                }

                // Trigger charging / charged events
                if (currentChargeTime >= 1f)
                {
                    Archery.internalApi.TriggerOnWeaponCharged(new WeaponChargeEventArgs() { WeaponId = weaponModel.Id, ChargePercentage = currentChargeTime, Origin = shootOrigin });
                }
                else
                {
                    Archery.internalApi.TriggerOnWeaponCharging(new WeaponChargeEventArgs() { WeaponId = weaponModel.Id, ChargePercentage = currentChargeTime, Origin = shootOrigin });
                }

                // Handle playing sounds
                if (canPlaySound && (currentChargeTime >= 1f || Bow.IsLoaded(slingshot)))
                {
                    Toolkit.PlaySound(weaponModel.FinishChargingSound, weaponModel.Id, who.getStandingPosition());
                    canPlaySound = false;
                }

                if (!slingshot.CanAutoFire())
                {
                    slingshot.lastClickX = mouseX;
                    slingshot.lastClickY = mouseY;
                }

                if (Bow.IsUsingSpecialAttack(slingshot))
                {
                    Bow.PerformSpecial(weaponModel, slingshot, time, who.currentLocation, who);
                    return;
                }

                if (slingshot.CanAutoFire())
                {
                    bool first_fire = false;
                    if ((currentChargeTime >= 1f || weaponModel.Type is WeaponType.Crossbow && Bow.IsLoaded(slingshot)) && slingshot.nextAutoFire < 0f)
                    {
                        slingshot.nextAutoFire = 0;
                        first_fire = true;
                    }
                    if (slingshot.nextAutoFire > 0f || first_fire)
                    {
                        slingshot.nextAutoFire -= (float)time.ElapsedGameTime.TotalMilliseconds;
                        if (slingshot.nextAutoFire <= 0f)
                        {
                            slingshot.PerformFire(who.currentLocation, who);
                            slingshot.nextAutoFire = weaponModel.AutoFireRateMilliseconds;
                        }
                    }
                }
            }

            int offset = ((who.FacingDirection == 3 || who.FacingDirection == 1) ? 1 : ((who.FacingDirection == 0) ? 2 : 0));
            who.FarmerSprite.setCurrentFrame(42 + offset);
        }

        // Note: ammoId can be null if called from Api.PerformFire(BasicProjectile...)
        internal static BasicProjectile PerformFire(BasicProjectile projectile, string ammoId, Slingshot slingshot, GameLocation location, Farmer who, bool suppressFiringSound = false)
        {
            var weaponModel = Bow.GetModel<WeaponModel>(slingshot);
            if (weaponModel is null || projectile is null)
            {
                return null;
            }

            // Handle Crossbow ammo loaded
            if (weaponModel.Type is WeaponType.Crossbow)
            {
                if (Bow.IsLoaded(slingshot) is false || Toolkit.AreToolButtonSuppressed() is true)
                {
                    return null;
                }

                Bow.SetLoaded(slingshot, Bow.GetLoaded(slingshot) - 1);
            }
            else if (slingshot.GetSlingshotChargeTime() < 1f)
            {
                return null;
            }

            // Get the ammo to be used
            if (weaponModel.UsesInternalAmmo() is false && (weaponModel.ShouldAlwaysConsumeAmmo() || Game1.random.NextDouble() < weaponModel.ConsumeAmmoChance))
            {
                slingshot.attachments[0].Stack--;
                if (slingshot.attachments[0].Stack <= 0)
                {
                    slingshot.attachments[0] = null;
                }
            }

            // Add the projectile to the current location
            location.projectiles.Add(projectile);

            // Play firing sound
            if (suppressFiringSound is false)
            {
                Toolkit.PlaySound(weaponModel.FiringSound, weaponModel.Id, slingshot.GetShootOrigin(who));
            }

            // Set charge percentage to 0
            Bow.SetSlingshotChargeTime(slingshot, 0f);

            // Trigger event
            Archery.internalApi.TriggerOnWeaponFired(new WeaponFiredEventArgs() { WeaponId = weaponModel.Id, AmmoId = ammoId, Projectile = projectile, Origin = slingshot.GetShootOrigin(who) });

            return projectile;
        }

        internal static BasicProjectile PerformFire(AmmoModel ammoModel, Slingshot slingshot, GameLocation location, Farmer who, bool suppressFiringSound = false)
        {
            var weaponModel = Bow.GetModel<WeaponModel>(slingshot);
            if (weaponModel is null)
            {
                return null;
            }

            ArrowProjectile arrow = null;
            if (Bow.GetAmmoCount(slingshot) > 0 && ammoModel is not null)
            {
                SlingshotPatch.UpdateAimPosReversePatch(slingshot);

                int mouseX = slingshot.aimPos.X;
                int mouseY = slingshot.aimPos.Y;

                Vector2 shootOrigin = slingshot.GetShootOrigin(who);
                Vector2 v = Utility.getVelocityTowardPoint(slingshot.GetShootOrigin(who), slingshot.AdjustForHeight(new Vector2(mouseX, mouseY)), weaponModel.ProjectileSpeed * (1f + who.weaponSpeedModifier));

                v.X *= -1f;
                v.Y *= -1f;

                arrow = new ArrowProjectile(weaponModel, ammoModel, who, 0f, 0f - v.X, 0f - v.Y, shootOrigin, String.Empty, String.Empty, damagesMonsters: true, location, spriteFromObjectSheet: true)
                {
                    IgnoreLocationCollision = (Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
                };
                arrow.startingRotation.Value = Bow.GetFrontArmRotation(who, slingshot);

                return PerformFire(arrow, ammoModel.Id, slingshot, location, who, suppressFiringSound);
            }
            else if (Bow.GetAmmoCount(slingshot) <= 0)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
            }

            Archery.modHelper.Reflection.GetField<bool>(slingshot, "canPlaySound").SetValue(true);

            return null;
        }

        internal static BasicProjectile PerformFire(Slingshot slingshot, GameLocation location, Farmer who, bool suppressFiringSound = false)
        {
            var ammoModel = Arrow.GetModel<AmmoModel>(Bow.GetAmmoItem(slingshot));

            return PerformFire(ammoModel, slingshot, location, who, suppressFiringSound);
        }

        internal static void PerformSpecial(WeaponModel weaponModel, Slingshot slingshot, GameTime time, GameLocation currentLocation, Farmer who)
        {
            // Set the required farmer flags
            who.usingSlingshot = true;
            who.UsingTool = true;
            who.CanMove = false;

            if (weaponModel.SpecialAttack.TriggerAfterButtonRelease && Toolkit.AreSpecialAttackButtonsPressed() is true)
            {
                RefreshSpecialAttackCooldown(slingshot, weaponModel.SpecialAttack);
                return;
            }

            if (Archery.internalApi.HandleSpecialAttack(weaponModel.Type, weaponModel.SpecialAttack.Id, weaponModel.SpecialAttack.Generate(slingshot, time, currentLocation, who)) is false)
            {
                // Reset the required farmer flags
                who.usingSlingshot = false;
                who.UsingTool = false;
                who.CanMove = true;

                Bow.SetUsingSpecialAttack(slingshot, false);
            }
        }

        internal static bool Draw(IDrawTool drawTool)
        {
            return Draw(drawTool.Farmer, drawTool.SpriteBatch, drawTool.LayerDepthSnapshot, drawTool.Position, drawTool.PositionOffset, drawTool.Origin, drawTool.Rotation, drawTool.Scale, drawTool.OverrideColor);
        }

        internal static bool Draw(Farmer who, SpriteBatch spriteBatch, float layerDepth, Vector2 position, Vector2 positionOffset, Vector2 origin, float rotation, float scale, Color overrideColor)
        {
            if (who.UsingTool is false || Bow.IsValid(who.CurrentTool) is false)
            {
                return false;
            }
            else if (Toolkit.AreToolButtonSuppressed())
            {
                return true;
            }

            // Get the required models
            var ammoItem = Bow.GetAmmoItem(who.CurrentTool);

            var bowModel = Bow.GetModel<WeaponModel>(who.CurrentTool);
            var ammoModel = Arrow.GetModel<AmmoModel>(ammoItem);

            if (bowModel is null)
            {
                return false;
            }

            // Get slingshot and related data
            Slingshot slingshot = who.CurrentTool as Slingshot;

            float frontArmRotation = GetFrontArmRotation(who, slingshot);

            // Get the arrow and bow sprites
            var ammoSprite = ammoModel is not null ? ammoModel.GetSpriteFromDirection(who, slingshot) : null;
            var bowSprite = bowModel.GetSpriteFromDirection(who, slingshot);
            if (bowSprite is null)
            {
                return false;
            }
            var bowSpriteDirection = bowModel.GetSpriteDirectionFromGivenDirection(who);

            var frontArmSprite = bowSprite.GetArmSprite(ArmType.Front);
            var backArmSprite = bowSprite.GetArmSprite(ArmType.Back);

            // Determine if arrow should be drawn
            bool shouldDrawArrow = ammoItem is not null && ammoItem.Stack > 0 && bowSprite.HideAmmo is false;

            // Establish the offsets
            var originOffset = Vector2.Zero;
            var specialOffset = Vector2.Zero;
            var baseOffset = position + origin + positionOffset + who.armOffset;

            // Get the flip effect
            var flipEffect = who.FacingDirection == Game1.left ? SpriteEffects.FlipVertically : SpriteEffects.None;
            var bowFlipOverride = bowSprite.GetSpriteEffects() is SpriteEffects.None ? flipEffect : bowSprite.GetSpriteEffects();
            var arrowFlipOverride = ammoSprite is null || ammoSprite.GetSpriteEffects() is SpriteEffects.None ? flipEffect : ammoSprite.GetSpriteEffects();
            var frontArmFlipOverride = frontArmSprite is null || frontArmSprite.GetSpriteEffects() is SpriteEffects.None ? flipEffect : frontArmSprite.GetSpriteEffects();
            var backArmFlipOverride = backArmSprite is null || backArmSprite.GetSpriteEffects() is SpriteEffects.None ? flipEffect : backArmSprite.GetSpriteEffects();

            switch (who.FacingDirection)
            {
                case Game1.down:
                    // Draw the back arm
                    if (backArmSprite is not null)
                    {
                        spriteBatch.Draw(bowModel.GetArmsTexture(), baseOffset + specialOffset + backArmSprite.Offset, backArmSprite.Source, overrideColor, rotation, origin, backArmSprite.Scale * scale, backArmFlipOverride, Toolkit.IncrementAndGetLayerDepth(ref layerDepth));
                    }

                    // Draw the bow
                    specialOffset = new Vector2(4f - frontArmRotation * 2f, 0f);
                    spriteBatch.Draw(bowModel.Texture, baseOffset + specialOffset + bowSprite.Offset, bowSprite.Source, Color.White, rotation, origin, bowSprite.Scale * scale, bowFlipOverride, Toolkit.IncrementAndGetLayerDepth(ref layerDepth));

                    // Draw the arrow
                    if (shouldDrawArrow && ammoSprite is not null)
                    {
                        spriteBatch.Draw(ammoModel.Texture, baseOffset + specialOffset + bowSprite.AmmoOffset, ammoSprite.Source, Color.White, rotation, origin + new Vector2(0f, -16f) - bowSprite.AmmoOffset, ammoSprite.Scale * scale, arrowFlipOverride, Toolkit.IncrementAndGetLayerDepth(ref layerDepth));
                    }

                    // Draw the front arm
                    specialOffset = Vector2.Zero;
                    if (frontArmSprite is not null)
                    {
                        spriteBatch.Draw(bowModel.GetArmsTexture(), baseOffset + specialOffset + frontArmSprite.Offset, frontArmSprite.Source, overrideColor, rotation, origin, frontArmSprite.Scale * scale, frontArmFlipOverride, Toolkit.IncrementAndGetLayerDepth(ref layerDepth));
                    }

                    return true;
                case Game1.right:
                case Game1.left:
                    originOffset = who.FacingDirection == Game1.left ? new Vector2(0f, -16f) : new Vector2(0f, -16f);
                    specialOffset = who.FacingDirection == Game1.left ? new Vector2(56f, -60f) : new Vector2(8f, -60f);

                    // Draw the back arm
                    if (backArmSprite is not null)
                    {
                        spriteBatch.Draw(bowModel.GetArmsTexture(), baseOffset + specialOffset + backArmSprite.Offset, backArmSprite.Source, overrideColor, backArmSprite.DisableRotation ? 0f : frontArmRotation, origin + originOffset, backArmSprite.Scale * scale, backArmFlipOverride, 5.9E-05f);
                    }

                    // Draw the bow
                    spriteBatch.Draw(bowModel.Texture, baseOffset + specialOffset, bowSprite.Source, Color.White, bowSprite.DisableRotation ? 0f : frontArmRotation, new Vector2(0, bowSprite.Source.Height / 2f) - new Vector2(bowSprite.Offset.X, bowSprite.Offset.Y * (bowSpriteDirection == Direction.Sideways && who.FacingDirection == Game1.left ? -1 : 1)), bowSprite.Scale * scale, bowFlipOverride, Toolkit.IncrementAndGetLayerDepth(ref layerDepth));

                    // Draw the arrow
                    if (shouldDrawArrow && ammoSprite is not null)
                    {
                        spriteBatch.Draw(ammoModel.Texture, baseOffset + specialOffset + ammoSprite.Offset, ammoSprite.Source, Color.White, ammoSprite.DisableRotation ? 0f : frontArmRotation, origin + new Vector2(-13f, -32f) - bowSprite.AmmoOffset, ammoSprite.Scale * scale, arrowFlipOverride, Toolkit.IncrementAndGetLayerDepth(ref layerDepth));
                    }

                    // Draw the front arm
                    if (frontArmSprite is not null)
                    {
                        spriteBatch.Draw(bowModel.GetArmsTexture(), baseOffset + specialOffset + frontArmSprite.Offset, frontArmSprite.Source, overrideColor, frontArmSprite.DisableRotation ? 0f : frontArmRotation, new Vector2(0, frontArmSprite.Source.Height / 2f), frontArmSprite.Scale * scale, frontArmFlipOverride, Toolkit.IncrementAndGetLayerDepth(ref layerDepth));
                    }

                    return true;
                case Game1.up:
                    // Draw the back arm
                    if (backArmSprite is not null)
                    {
                        spriteBatch.Draw(bowModel.GetArmsTexture(), baseOffset + specialOffset + backArmSprite.Offset, backArmSprite.Source, overrideColor, rotation, origin, backArmSprite.Scale * scale, backArmFlipOverride, Toolkit.IncrementAndGetLayerDepth(ref layerDepth));
                    }

                    // Draw the bow
                    specialOffset = new Vector2((frontArmRotation - 6f) * 4f, 0f);
                    spriteBatch.Draw(bowModel.Texture, baseOffset + specialOffset + bowSprite.Offset, bowSprite.Source, Color.White, rotation, origin, bowSprite.Scale * scale, bowFlipOverride, Toolkit.IncrementAndGetLayerDepth(ref layerDepth));

                    // Draw the arrow
                    if (shouldDrawArrow && ammoSprite is not null)
                    {
                        spriteBatch.Draw(ammoModel.Texture, baseOffset + specialOffset + bowSprite.AmmoOffset, ammoSprite.Source, Color.White, rotation, origin + new Vector2(-13f, -32f) - bowSprite.AmmoOffset, ammoSprite.Scale * scale, arrowFlipOverride, Toolkit.IncrementAndGetLayerDepth(ref layerDepth));
                    }

                    // Draw the front arm
                    specialOffset = Vector2.Zero;
                    if (frontArmSprite is not null)
                    {
                        spriteBatch.Draw(bowModel.GetArmsTexture(), baseOffset + specialOffset + frontArmSprite.Offset, frontArmSprite.Source, overrideColor, rotation, origin, frontArmSprite.Scale * scale, frontArmFlipOverride, Toolkit.IncrementAndGetLayerDepth(ref layerDepth));
                    }

                    return true;
                default:
                    return false;
            }
        }

        internal static float GetFrontArmRotation(Farmer who, Slingshot slingshot)
        {
            Point point = Utility.Vector2ToPoint(slingshot.AdjustForHeight(Utility.PointToVector2(slingshot.aimPos.Value)));
            int mouseX = point.X;
            int mouseY = point.Y;

            Vector2 shootOrigin = slingshot.GetShootOrigin(who);
            float frontArmRotation = (float)Math.Atan2((float)mouseY - shootOrigin.Y, (float)mouseX - shootOrigin.X) + (float)Math.PI;

            frontArmRotation -= (float)Math.PI;
            if (frontArmRotation < 0f)
            {
                frontArmRotation += (float)Math.PI * 2f;
            }

            return frontArmRotation;
        }
    }
}
