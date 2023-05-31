/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models.Weapons;
using Archery.Framework.Objects.Items;
using Archery.Framework.Objects.Weapons;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Text;

namespace Archery.Framework.Patches.Objects
{
    internal class ToolPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Tool);

        public ToolPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, "get_DisplayName", null), postfix: new HarmonyMethod(GetType(), nameof(GetNamePostfix)));
            harmony.Patch(AccessTools.Method(_object, "get_description", null), postfix: new HarmonyMethod(GetType(), nameof(GetDescriptionPostfix)));
            harmony.Patch(AccessTools.Method(typeof(Item), nameof(Item.canBeTrashed), null), postfix: new HarmonyMethod(GetType(), nameof(CanBeTrashedPostfix)));

            harmony.Patch(AccessTools.Method(_object, nameof(Tool.getExtraSpaceNeededForTooltipSpecialIcons), new[] { typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(StringBuilder), typeof(string), typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(GetExtraSpaceNeededForTooltipSpecialIconsPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.attachmentSlots)), postfix: new HarmonyMethod(GetType(), nameof(AttachmentSlotsPostfix)));

            harmony.Patch(AccessTools.Method(_object, nameof(Tool.drawTooltip)), postfix: new HarmonyMethod(GetType(), nameof(DrawTooltipPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.beginUsing), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(BeginUsingPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.DoFunction), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(DoFunctionPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.endUsing), new[] { typeof(GameLocation), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(EndUsingPrefix)));
        }

        private static void GetNamePostfix(Tool __instance, ref string __result)
        {
            if (Bow.IsValid(__instance))
            {
                __result = Bow.GetName(__instance);
                return;
            }
        }

        private static void GetDescriptionPostfix(Tool __instance, ref string __result)
        {
            if (Bow.IsValid(__instance))
            {
                __result = Bow.GetDescription(__instance);
                return;
            }
        }

        private static void CanBeTrashedPostfix(Tool __instance, ref bool __result)
        {
            if (Bow.IsValid(__instance))
            {
                __result = true;
                return;
            }
        }

        private static void GetExtraSpaceNeededForTooltipSpecialIconsPostfix(Tool __instance, ref Point __result, SpriteFont font, int minWidth, int horizontalBuffer, int startingHeight, StringBuilder descriptionText, string boldTitleText, int moneyAmountToDisplayAtBottom)
        {
            if (Bow.IsValid(__instance) && Bow.GetModel<WeaponModel>(__instance) is WeaponModel weaponModel && weaponModel.UsesInternalAmmo())
            {
                __result.Y += 48;
            }
        }

        private static void AttachmentSlotsPostfix(Tool __instance, ref int __result)
        {
            if (Bow.IsValid(__instance) && Bow.GetModel<WeaponModel>(__instance) is WeaponModel weaponModel && weaponModel.UsesInternalAmmo())
            {
                __result = 0;
            }
        }

        private static void DrawTooltipPostfix(Tool __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, StringBuilder overrideText)
        {
            if (Bow.IsValid(__instance) && Bow.GetModel<WeaponModel>(__instance) is WeaponModel weaponModel)
            {
                int xOffset = 70;
                if (weaponModel.UsesInternalAmmo())
                {
                    xOffset = 0;
                }

                int minDamage = weaponModel.DamageRange.Min;
                int maxDamage = weaponModel.DamageRange.Max;
                if (Arrow.GetModel<AmmoModel>(Bow.GetAmmoItem(__instance)) is AmmoModel ammoModel)
                {
                    maxDamage += ammoModel.Damage;
                }

                // Draw damage values
                Color color = Bow.GetAmmoCount(__instance) > 0 ? new Color(216, 0, 59) : Game1.textColor;
                Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_Damage", minDamage, maxDamage), font, new Vector2(x + xOffset + 16, y + 32), color * 0.9f * alpha);
            }
        }

        private static bool BeginUsingPrefix(Tool __instance, ref bool __result, GameLocation location, int x, int y, Farmer who)
        {
            if (Bow.IsValid(__instance))
            {
                __result = true;
                return Bow.Use(__instance, location, x, y, who);
            }

            return true;
        }

        private static bool DoFunctionPrefix(Tool __instance, ref Farmer ___lastUser, GameLocation location, int x, int y, int power, Farmer who)
        {
            if (Bow.IsValid(__instance))
            {
                return false;
            }

            return true;
        }

        private static bool EndUsingPrefix(Tool __instance, GameLocation location, Farmer who)
        {
            if (Bow.IsValid(__instance))
            {
                who.forceCanMove();
                return false;
            }

            return true;
        }
    }
}
