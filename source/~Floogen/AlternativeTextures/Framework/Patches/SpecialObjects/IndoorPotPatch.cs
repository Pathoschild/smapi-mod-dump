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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;

namespace AlternativeTextures.Framework.Patches.SpecialObjects
{
    internal class IndoorPotPatch : PatchTemplate
    {
        private readonly Type _object = typeof(IndoorPot);

        internal IndoorPotPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(IndoorPot.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
        }

        private static bool DrawPrefix(IndoorPot __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
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

                Vector2 scaleFactor = __instance.getScale();
                scaleFactor *= 4f;
                Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
                Rectangle destination = new Rectangle((int)(position.X - scaleFactor.X / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(position.Y - scaleFactor.Y / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(64f + scaleFactor.X), (int)(128f + scaleFactor.Y / 2f));
                spriteBatch.Draw(textureModel.GetTexture(textureVariation), destination, new Rectangle((__instance.showNextIndex ? 1 : 0) * textureModel.TextureWidth, textureOffset, 16, 32), Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (((int)__instance.parentSheetIndex == 105) ? 0.0035f : 0f) + (float)x * 1E-05f);

                if (__instance.hoeDirt.Value.fertilizer.Value != "0")
                {
                    Rectangle fertilizer_rect = __instance.hoeDirt.Value.GetFertilizerSourceRect();
                    fertilizer_rect.Width = 13;
                    fertilizer_rect.Height = 13;
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.tileLocation.X * 64f + 4f, __instance.tileLocation.Y * 64f - 12f)), fertilizer_rect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (__instance.tileLocation.Y + 0.65f) * 64f / 10000f + (float)x * 1E-05f);
                }
                if (__instance.hoeDirt.Value.crop != null)
                {
                    __instance.hoeDirt.Value.crop.drawWithOffset(spriteBatch, __instance.TileLocation, ((int)__instance.hoeDirt.Value.state == 1 && (int)__instance.hoeDirt.Value.crop.currentPhase == 0 && !__instance.hoeDirt.Value.crop.raisedSeeds) ? (new Color(180, 100, 200) * 1f) : Color.White, __instance.hoeDirt.Value.getShakeRotation(), new Vector2(32f, 8f));
                }
                if (__instance.heldObject.Value != null)
                {
                    __instance.heldObject.Value.draw(spriteBatch, x * 64, y * 64 - 48, (__instance.tileLocation.Y + 0.66f) * 64f / 10000f + (float)x * 1E-05f, 1f);
                }
                if (__instance.bush.Value != null)
                {
                    __instance.bush.Value.draw(spriteBatch, -24f);
                }

                return false;
            }
            return true;
        }
    }
}
