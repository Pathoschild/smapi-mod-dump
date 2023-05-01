/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.SpecialObjects
{
    internal class WoodChipperPatch : PatchTemplate
    {
        private readonly Type _object = typeof(WoodChipper);

        internal WoodChipperPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(WoodChipper.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
        }

        private static bool DrawPrefix(Phone __instance, NetRef<Object> ___depositedItem, bool ____isAnimatingChip, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_VARIATION]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
                var textureOffset = textureModel.GetTextureOffset(textureVariation);

                if (__instance.isTemporarilyInvisible)
                {
                    return false;
                }
                Vector2 scale_factor = Vector2.One;
                scale_factor *= 4f;
                Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
                Rectangle destination = new Rectangle((int)(position.X - scale_factor.X / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(position.Y - scale_factor.Y / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(64f + scale_factor.X), (int)(128f + scale_factor.Y / 2f));
                float draw_layer = Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f;
                spriteBatch.Draw(textureModel.GetTexture(textureVariation), destination, new Rectangle(__instance.readyForHarvest.Value ? 16 : 0, textureOffset, textureModel.TextureWidth, textureModel.TextureHeight), Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);
                if (__instance.shakeTimer > 0)
                {
                    spriteBatch.Draw(textureModel.GetTexture(textureVariation), new Rectangle(destination.X, destination.Y + 4, destination.Width, 60), new Rectangle(32, textureOffset, 16, 15), Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, draw_layer + 0.0035f);
                }
                if (___depositedItem.Value != null && __instance.shakeTimer > 0 && ____isAnimatingChip)
                {
                    float completion = 1f - (float)__instance.shakeTimer / 1000f;
                    Vector2 end_position = position + new Vector2(32f, 32f);
                    Vector2 start_position = end_position + new Vector2(0f, -16f);
                    Vector2 draw_position = default(Vector2);
                    draw_position.X = Utility.Lerp(start_position.X, end_position.X, completion);
                    draw_position.Y = Utility.Lerp(start_position.Y, end_position.Y, completion);
                    draw_position.X += Game1.random.Next(-1, 2) * 2;
                    draw_position.Y += Game1.random.Next(-1, 2) * 2;
                    float draw_scale = Utility.Lerp(1f, 0.75f, completion);
                    spriteBatch.Draw(Game1.objectSpriteSheet, draw_position, GameLocation.getSourceRectForObject(___depositedItem.Value.ParentSheetIndex), Color.White * alpha, 0f, new Vector2(8f, 8f), 4f * draw_scale, ___depositedItem.Value.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, draw_layer + 0.00175f);
                }
                if (___depositedItem.Value != null && __instance.MinutesUntilReady > 0)
                {
                    int frame = (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 200.0) / 50;
                    spriteBatch.Draw(textureModel.GetTexture(textureVariation), position + new Vector2(6f, 17f) * 4f, new Rectangle(32 + frame % 2 * 8, textureOffset + 16 + frame / 2 * 7, 8, 7), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, draw_layer + 1E-05f);
                    spriteBatch.Draw(textureModel.GetTexture(textureVariation), position + new Vector2(3f, 9f) * 4f + new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)), new Rectangle(3, textureOffset + 9, 10, 6), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, draw_layer + 1E-05f);
                }
                if (!__instance.readyForHarvest.Value)
                {
                    return false;
                }
                float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 - 8, (float)(y * 64 - 96 - 16) + yOffset)), new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((y + 1) * 64) / 10000f + 1E-06f + __instance.tileLocation.X / 10000f + (((int)__instance.parentSheetIndex == 105) ? 0.0015f : 0f));
                if (__instance.heldObject.Value != null)
                {
                    spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32, (float)(y * 64 - 64 - 8) + yOffset)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, __instance.heldObject.Value.parentSheetIndex, 16, 16), Color.White * 0.75f, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float)((y + 1) * 64) / 10000f + 1E-05f + __instance.tileLocation.X / 10000f + (((int)__instance.parentSheetIndex == 105) ? 0.0015f : 0f));
                    if (__instance.heldObject.Value is ColoredObject)
                    {
                        spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32, (float)(y * 64 - 64 - 8) + yOffset)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int)__instance.heldObject.Value.parentSheetIndex + 1, 16, 16), (__instance.heldObject.Value as ColoredObject).color.Value * 0.75f, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float)((y + 1) * 64) / 10000f + 1E-05f + __instance.tileLocation.X / 10000f + (((int)__instance.parentSheetIndex == 105) ? 0.0015f : 1E-05f));
                    }
                }
                return false;
            }
            return true;
        }
    }
}
