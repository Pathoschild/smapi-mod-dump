/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Utilities;
using FishingTrawler.Patches;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Text;

namespace FishingTrawler.Framework.Patches.Objects
{
    internal class RingPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Ring);

        public RingPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, "get_DisplayName", null), postfix: new HarmonyMethod(GetType(), nameof(GetNamePostfix)));
            harmony.Patch(AccessTools.Method(_object, "getDescription", null), postfix: new HarmonyMethod(GetType(), nameof(GetDescriptionPostfix)));

            harmony.Patch(AccessTools.Method(_object, nameof(Ring.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Ring.drawTooltip)), prefix: new HarmonyMethod(GetType(), nameof(DrawTooltipPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Ring.getExtraSpaceNeededForTooltipSpecialIcons), new[] { typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(StringBuilder), typeof(string), typeof(int) }), prefix: new HarmonyMethod(GetType(), nameof(GetExtraSpaceNeededForTooltipSpecialIconsPrefix)));

            harmony.Patch(AccessTools.Method(_object, nameof(Ring.onEquip), new[] { typeof(Farmer), typeof(GameLocation) }), prefix: new HarmonyMethod(GetType(), nameof(OnEquipPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Ring.onUnequip), new[] { typeof(Farmer), typeof(GameLocation) }), prefix: new HarmonyMethod(GetType(), nameof(OnUnequipPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Ring.onDayUpdate), new[] { typeof(Farmer), typeof(GameLocation) }), prefix: new HarmonyMethod(GetType(), nameof(OnDayUpdatePrefix)));
        }

        private static void GetNamePostfix(Ring __instance, ref string __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANGLER_RING_KEY))
            {
                __result = _helper.Translation.Get("item.angler_ring.name");
                return;
            }
        }

        private static void GetDescriptionPostfix(Ring __instance, ref string __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANGLER_RING_KEY))
            {
                return;
            }
        }

        private static bool DrawInMenuPrefix(Ring __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANGLER_RING_KEY))
            {
                spriteBatch.Draw(FishingTrawler.assetManager.anglerRingTexture, location + new Vector2(32f, 32f) * scaleSize, new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, scaleSize * 3f, SpriteEffects.None, layerDepth);

                return false;
            }

            return true;
        }

        private static bool DrawTooltipPrefix(Ring __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, StringBuilder overrideText)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANGLER_RING_KEY))
            {
                Utility.drawTextWithShadow(spriteBatch, Game1.parseText(_helper.Translation.Get("item.angler_ring.description"), Game1.smallFont, GetDescriptionWidth(__instance)), font, new Vector2(x + 16, y + 16 + 4), Game1.textColor);
                y += (int)font.MeasureString(Game1.parseText(_helper.Translation.Get("item.angler_ring.description"), Game1.smallFont, GetDescriptionWidth(__instance))).Y;

                Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2(x + 16 + 4, y + 16 + 4), new Rectangle(20, 428, 10, 10), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 1f);
                Utility.drawTextWithShadow(spriteBatch, "+2 Fishing", font, new Vector2(x + 16 + 52, y + 16 + 12), Game1.textColor * 0.9f * alpha);
                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);

                return false;
            }

            return true;
        }

        private static bool GetExtraSpaceNeededForTooltipSpecialIconsPrefix(Ring __instance, ref Point __result, SpriteFont font, int minWidth, int horizontalBuffer, int startingHeight, StringBuilder descriptionText, string boldTitleText, int moneyAmountToDisplayAtBottom)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANGLER_RING_KEY))
            {
                Point dimensions = new Point(0, startingHeight);
                int extra_rows_needed = 2;

                dimensions.X = (int)Math.Max(minWidth, font.MeasureString(_helper.Translation.Get("item.angler_ring.name")).X + (float)horizontalBuffer);
                dimensions.Y += extra_rows_needed * Math.Max((int)font.MeasureString("TT").Y, 48);
                __result = dimensions;

                return false;
            }

            return true;
        }

        private static int GetDescriptionWidth(Ring __instance)
        {
            int minimum_size = 272;
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
            {
                minimum_size = 384;
            }
            return Math.Max(minimum_size, (int)Game1.dialogueFont.MeasureString((__instance.DisplayName == null) ? "" : __instance.DisplayName).X);
        }

        private static bool OnEquipPrefix(Ring __instance, Farmer who, GameLocation location)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANGLER_RING_KEY))
            {
                who.addedFishingLevel.Value += 2;

                return false;
            }

            return true;
        }

        private static bool OnUnequipPrefix(Ring __instance, Farmer who, GameLocation location)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANGLER_RING_KEY))
            {
                who.addedFishingLevel.Value = Math.Max(0, who.addedFishingLevel.Value - 2);

                return false;
            }

            return true;
        }

        private static bool OnDayUpdatePrefix(Ring __instance, Farmer who, GameLocation location)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANGLER_RING_KEY))
            {
                OnEquipPrefix(__instance, who, location);

                return false;
            }

            return true;
        }
    }
}
