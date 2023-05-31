/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces.Internal.Events;
using Archery.Framework.Models.Weapons;
using Archery.Framework.Objects;
using Archery.Framework.Objects.Items;
using Archery.Framework.Objects.Weapons;
using Archery.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using Object = StardewValley.Object;

namespace Archery.Framework.Patches.Objects
{
    internal class SlingshotPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Slingshot);

        public SlingshotPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Slingshot.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Slingshot.drawAttachments), new[] { typeof(SpriteBatch), typeof(int), typeof(int) }), prefix: new HarmonyMethod(GetType(), nameof(DrawAttachmentsPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Slingshot.PerformFire), new[] { typeof(GameLocation), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(PerformFirePrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Slingshot.tickUpdate), new[] { typeof(GameTime), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(TickUpdatePrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Slingshot.beginUsing), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(BeginUsingPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Slingshot.canThisBeAttached), new[] { typeof(Object) }), postfix: new HarmonyMethod(GetType(), nameof(CanThisBeAttachedPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Slingshot.attach), new[] { typeof(Object) }), postfix: new HarmonyMethod(GetType(), nameof(AttachPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Slingshot.GetSlingshotChargeTime), null), postfix: new HarmonyMethod(GetType(), nameof(GetSlingshotChargeTimePostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Slingshot.CanAutoFire), null), postfix: new HarmonyMethod(GetType(), nameof(CanAutoFirePostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Slingshot.GetShootOrigin), new[] { typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(GetShootOriginPostfix)));

            harmony.CreateReversePatcher(AccessTools.Method(_object, "updateAimPos", null), new HarmonyMethod(GetType(), nameof(UpdateAimPosReversePatch))).Patch();
        }

        private static bool DrawInMenuPrefix(Slingshot __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (Bow.GetModel<WeaponModel>(__instance) is WeaponModel weaponModel && weaponModel is not null)
            {
                bool isRecipe = InstancedObject.IsRecipe(__instance);

                if (isRecipe)
                {
                    transparency = 0.5f;
                    scaleSize *= 0.75f;
                }

                float coolDownLevel = 0f;
                float addedScale = 0f;
                bool drawingAsDebris = drawShadow && drawStackNumber == StackDrawType.Hide;
                if (weaponModel.SpecialAttack is not null)
                {
                    if (Bow.ActiveCooldown > 0)
                    {
                        coolDownLevel = Bow.ActiveCooldown / (float)(Archery.internalApi.GetSpecialAttackCooldown(weaponModel.SpecialAttack.Id, weaponModel.SpecialAttack.Arguments));
                        coolDownLevel = Utility.Clamp(coolDownLevel, 0f, 1f);
                    }

                    if (Bow.CooldownAdditiveScale > 0)
                    {
                        addedScale = Bow.CooldownAdditiveScale;
                    }

                    if (!drawShadow || drawingAsDebris)
                    {
                        addedScale = 0f;
                    }
                }

                var weaponIcon = weaponModel.GetIcon(Game1.player, __instance);
                spriteBatch.Draw(weaponModel.Texture, location + (new Vector2(34f, 32f) + weaponIcon.Offset), weaponIcon.Source, color * transparency, 0f, new Vector2(8f, 8f) * (1f + addedScale), weaponIcon.Scale * (1f + addedScale), weaponIcon.GetSpriteEffects(), layerDepth);

                int ammoCount = Bow.GetAmmoCount(__instance);
                if (weaponModel.UsesInternalAmmo() is false && drawStackNumber != 0 && ammoCount > 0)
                {
                    Utility.drawTinyDigits(ammoCount, spriteBatch, location + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(ammoCount, 3f * scaleSize)) + 3f * scaleSize, 64f - 18f * scaleSize + 2f), 3f * scaleSize, 1f, Color.White);
                }

                int loadedAmmoCount = Bow.GetLoaded(__instance);
                if (loadedAmmoCount > 0)
                {
                    Utility.drawTinyDigits(loadedAmmoCount, spriteBatch, location + new Vector2(50f, 4f), 3f * scaleSize, 1f, Color.White);
                }

                if (isRecipe)
                {
                    spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(16f, 16f), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 451, 16, 16), color, 0f, Vector2.Zero, 3f, SpriteEffects.None, layerDepth + 0.0001f);
                }
                else
                {
                    if (coolDownLevel > 0f && drawShadow && !drawingAsDebris && (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is ShopMenu) || scaleSize != 1f))
                    {
                        spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)location.X, (int)location.Y + (64 - (int)(coolDownLevel * 64f)), 64, (int)(coolDownLevel * 64f)), Color.Red * 0.66f);
                    }
                }

                return false;
            }

            return true;
        }

        private static bool DrawAttachmentsPrefix(Slingshot __instance, SpriteBatch b, int x, int y)
        {
            if (Bow.GetModel<WeaponModel>(__instance) is WeaponModel weaponModel && weaponModel.UsesInternalAmmo())
            {
                return false;
            }

            return true;
        }

        [HarmonyPriority(Priority.High)]
        private static bool TickUpdatePrefix(Slingshot __instance, ref bool ___canPlaySound, ref Farmer ___lastUser, NetEvent0 ___finishEvent, GameTime time, Farmer who)
        {
            if (Bow.IsValid(__instance))
            {
                Bow.TickUpdate(__instance, ref ___canPlaySound, ref ___lastUser, ___finishEvent, time, who);

                return false;
            }

            return true;
        }

        [HarmonyPriority(Priority.High)]
        private static bool PerformFirePrefix(Slingshot __instance, ref bool ___canPlaySound, GameLocation location, Farmer who)
        {
            if (Bow.IsValid(__instance))
            {
                if (Bow.IsUsingSpecialAttack(__instance) is false)
                {
                    Bow.PerformFire(__instance, location, who);
                }

                return false;
            }

            return true;
        }

        [HarmonyPriority(Priority.High)]
        private static void BeginUsingPostfix(Slingshot __instance, GameLocation location, int x, int y, Farmer who)
        {
            var weaponModel = Bow.GetModel<WeaponModel>(__instance);
            if (weaponModel is not null && Bow.IsLoaded(__instance) is false)
            {
                // Play charging sound
                Toolkit.PlaySound(weaponModel.StartChargingSound, weaponModel.Id, who.getStandingPosition());
            }
        }

        private static void CanThisBeAttachedPostfix(Slingshot __instance, ref bool __result, Object o)
        {
            if (Bow.IsValid(__instance))
            {
                __result = Bow.CanThisBeAttached(__instance, o);
            }
        }

        private static void AttachPostfix(Slingshot __instance, Object o)
        {
            if (Bow.GetModel<WeaponModel>(__instance) is WeaponModel weaponModel && Arrow.GetModel<AmmoModel>(o) is AmmoModel ammoModel)
            {
                // Trigger event
                Archery.internalApi.TriggerOnAmmoChanged(new AmmoChangedEventArgs() { WeaponId = weaponModel.Id, AmmoId = ammoModel.Id, Origin = Game1.player.getStandingPosition() });
            }
        }

        private static void GetSlingshotChargeTimePostfix(Slingshot __instance, ref float __result)
        {
            if (Bow.IsValid(__instance))
            {
                __result = Bow.GetSlingshotChargeTime(__instance);
            }
        }

        private static void CanAutoFirePostfix(Slingshot __instance, ref bool __result)
        {
            if (Bow.IsValid(__instance) && Bow.GetModel<WeaponModel>(__instance) is WeaponModel weaponModel && weaponModel.CanAutoFire)
            {
                __result = true;
            }
        }

        private static void GetShootOriginPostfix(Slingshot __instance, ref Vector2 __result, Farmer who)
        {
            if (Bow.IsValid(__instance) && Bow.GetModel<WeaponModel>(__instance) is WeaponModel weaponModel)
            {
                __result += weaponModel.GetProjectileOrigin(who);
            }
        }

        internal static void UpdateAimPosReversePatch(Slingshot __instance)
        {
            new NotImplementedException("It's a stub!");
        }
    }
}
