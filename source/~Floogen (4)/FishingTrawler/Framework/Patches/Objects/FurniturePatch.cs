/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Objects.Items.Rewards;
using FishingTrawler.Framework.Utilities;
using FishingTrawler.Patches;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;

namespace FishingTrawler.Framework.Patches.Objects
{
    internal class FurniturePatch : PatchTemplate
    {
        private readonly Type _object = typeof(Furniture);

        public FurniturePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(StardewValley.Object), "get_DisplayName", null), postfix: new HarmonyMethod(GetType(), nameof(GetNamePostfix)));
            harmony.Patch(AccessTools.Method(_object, "get_description", null), postfix: new HarmonyMethod(GetType(), nameof(GetDescriptionPostfix)));

            harmony.Patch(AccessTools.Method(_object, nameof(Furniture.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Furniture.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
            harmony.Patch(AccessTools.Method(typeof(StardewValley.Object), "drawPlacementBounds", new[] { typeof(SpriteBatch), typeof(GameLocation) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPlacementBoundsPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Furniture.placementAction), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(PlacementActionPrefix)));
        }

        private static void GetNamePostfix(StardewValley.Object __instance, ref string __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANCIENT_FLAG_KEY))
            {
                __result = AncientFlag.GetFlagName(AncientFlag.GetFlagType(__instance));
                return;
            }
        }

        private static void GetDescriptionPostfix(Furniture __instance, ref string __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANCIENT_FLAG_KEY))
            {
                __result = AncientFlag.GetFlagDescription(AncientFlag.GetFlagType(__instance));
                return;
            }
        }

        private static bool DrawPrefix(Furniture __instance, NetVector2 ___drawPosition, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANCIENT_FLAG_KEY))
            {
                if (__instance.isTemporarilyInvisible)
                {
                    return true;
                }

                var flagType = AncientFlag.GetFlagType(__instance);
                var flagTexture = FishingTrawler.assetManager.AncientFlagsTexture;
                var sourceRectangle = new Rectangle(32 * (int)flagType % flagTexture.Width, 32 * (int)flagType / flagTexture.Width * 32, 32, 32); ;
                if (Furniture.isDrawingLocationFurniture)
                {
                    spriteBatch.Draw(flagTexture, Game1.GlobalToLocal(Game1.viewport, ___drawPosition.Value + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)), sourceRectangle, Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((int)__instance.furniture_type.Value == 12) ? (2E-09f + __instance.tileLocation.Y / 100000f) : ((float)(__instance.boundingBox.Value.Bottom - (((int)__instance.furniture_type.Value == 6 || (int)__instance.furniture_type.Value == 17 || (int)__instance.furniture_type.Value == 13) ? 48 : 8)) / 10000f));
                }
                else
                {
                    spriteBatch.Draw(flagTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - (sourceRectangle.Height * 4 - __instance.boundingBox.Height) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), sourceRectangle, Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((int)__instance.furniture_type.Value == 12) ? (2E-09f + __instance.tileLocation.Y / 100000f) : ((float)(__instance.boundingBox.Value.Bottom - (((int)__instance.furniture_type.Value == 6 || (int)__instance.furniture_type.Value == 17 || (int)__instance.furniture_type.Value == 13) ? 48 : 8)) / 10000f));
                }

                return false;
            }

            return true;
        }

        private static bool DrawInMenuPrefix(Furniture __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANCIENT_FLAG_KEY))
            {
                var flagType = AncientFlag.GetFlagType(__instance);
                var flagTexture = FishingTrawler.assetManager.AncientFlagsTexture;
                var sourceRectangle = new Rectangle(32 * (int)flagType % flagTexture.Width, 32 * (int)flagType / flagTexture.Width * 32, 32, 32); ;

                spriteBatch.Draw(flagTexture, location + new Vector2(32f, 32f), sourceRectangle, color * transparency, 0f, new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2), 2f * scaleSize, SpriteEffects.None, layerDepth);
                if (((drawStackNumber == StackDrawType.Draw && __instance.maximumStackSize() > 1 && __instance.Stack > 1) || drawStackNumber == StackDrawType.Draw_OneInclusive) && (double)scaleSize > 0.3 && __instance.Stack != int.MaxValue)
                {
                    Utility.drawTinyDigits(__instance.Stack, spriteBatch, location + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(__instance.Stack, 3f * scaleSize)) + 3f * scaleSize, 64f - 18f * scaleSize + 2f), 3f * scaleSize, 1f, color);
                }
                return false;
            }

            return true;
        }

        private static bool DrawPlacementBoundsPrefix(StardewValley.Object __instance, SpriteBatch spriteBatch, GameLocation location)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANCIENT_FLAG_KEY) && location is Beach)
            {
                // Draw nothing to avoid covering up Murphy when attempting to give him an ancient flag
                return false;
            }

            return true;
        }

        private static bool PlacementActionPrefix(Furniture __instance, GameLocation location, int x, int y, Farmer who = null)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ANCIENT_FLAG_KEY))
            {
                var flagType = AncientFlag.GetFlagType(__instance);
                if (flagType is FlagType.Unknown)
                {
                    Game1.showRedMessage(_helper.Translation.Get("game_message.identify_flag_first"));
                    return false;
                }
            }

            return true;
        }
    }
}
