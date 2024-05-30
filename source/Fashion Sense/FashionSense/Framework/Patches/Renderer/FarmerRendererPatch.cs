/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Interfaces.API;
using FashionSense.Framework.Managers;
using FashionSense.Framework.Models.Appearances.Body;
using FashionSense.Framework.Models.Appearances.Hair;
using FashionSense.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FashionSense.Framework.Patches.Renderer
{
    internal class FarmerRendererPatch : PatchTemplate
    {
        internal static bool AreColorMasksPendingRefresh = false;
        private readonly Type _entity = typeof(FarmerRenderer);

        internal FarmerRendererPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(FarmerRenderer.drawHairAndAccesories), new[] { typeof(SpriteBatch), typeof(int), typeof(Farmer), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(float), typeof(Color), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawHairAndAccesoriesPrefix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(FarmerRenderer.drawMiniPortrat), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(DrawMiniPortratPrefix)));
        }

        private static bool DrawHairAndAccesoriesPrefix(FarmerRenderer __instance, bool ___isDrawingForUI, Vector2 ___positionOffset, Vector2 ___rotationAdjustment, LocalizedContentManager ___farmerTextureManager, Texture2D ___baseTexture, NetInt ___skin, bool ____sickFrame, ref Rectangle ___hairstyleSourceRect, ref Rectangle ___shirtSourceRect, ref Rectangle ___accessorySourceRect, ref Rectangle ___hatSourceRect, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth)
        {
            if (AppearanceHelpers.GetCurrentlyEquippedModels(who, facingDirection).Count == 0)
            {
                return true;
            }

            return false;
        }

        private static bool DrawMiniPortratPrefix(FarmerRenderer __instance, LocalizedContentManager ___farmerTextureManager, Texture2D ___baseTexture, NetInt ___skin, bool ____sickFrame, SpriteBatch b, Vector2 position, float layerDepth, float scale, int facingDirection, Farmer who)
        {
            // Draw the player's custom face, if applicable
            bool hasDrawnCustomBody = false;
            BodyModel customBody = null;
            Vector2 portraitOffset = Vector2.Zero;
            if (who.modData.ContainsKey(ModDataKeys.CUSTOM_BODY_ID) && FashionSense.textureManager.GetSpecificAppearanceModel<BodyContentPack>(who.modData[ModDataKeys.CUSTOM_BODY_ID]) is BodyContentPack bodyPack && bodyPack is not null && bodyPack.FrontBody is BodyModel bodyModel && bodyModel is not null && bodyModel.StartingPosition is not null)
            {
                FashionSense.monitor.LogOnce($"Using custom body {bodyPack.Name} from {bodyPack.PackName} for profile draw!", LogLevel.Trace);
                customBody = bodyModel;

                Rectangle sourceRectangle = new Rectangle(bodyModel.StartingPosition.X, bodyModel.StartingPosition.Y, 16, 16);
                if (customBody.Portrait is not null)
                {
                    sourceRectangle = customBody.Portrait.SourceRectangle;
                    portraitOffset = customBody.Portrait.Offset;
                }

                // Adjust color if needed
                var colors = AppearanceHelpers.GetAllAppearanceColors(who, bodyModel);
                Color? colorOverride = null;
                Color modelColor = colors.Count == 0 ? Color.White : colors[0];
                if (bodyModel.DisableGrayscale)
                {
                    colorOverride = Color.White;
                }
                else if (bodyModel.IsPrismatic)
                {
                    colorOverride = Utility.GetPrismaticColor(speedMultiplier: bodyModel.PrismaticAnimationSpeedMultiplier);
                }

                // Draw the player's base texture
                b.Draw(bodyPack.Texture, position + portraitOffset, sourceRectangle, bodyModel.HasColorMask() ? Color.White : colorOverride is not null ? colorOverride.Value : modelColor, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);

                if (bodyModel.HasColorMask())
                {
                    DrawManager.DrawColorMask(b, bodyPack, bodyModel, false, position + portraitOffset, sourceRectangle, colorOverride, colors, 0f, Vector2.Zero, scale, layerDepth);
                }

                // Flag that we have drawn the custom body, to skip the base texture draw
                hasDrawnCustomBody = true;
            }

            if (!who.modData.ContainsKey(ModDataKeys.CUSTOM_HAIR_ID) || ___baseTexture is null || ___baseTexture.IsDisposed)
            {
                return true;
            }

            var hairPack = FashionSense.textureManager.GetSpecificAppearanceModel<HairContentPack>(who.modData[ModDataKeys.CUSTOM_HAIR_ID]);
            if (hairPack is null)
            {
                return customBody is null ? true : DrawVanillaHairForPortrait(b, who, position + portraitOffset, layerDepth, scale, facingDirection, customBody);
            }

            // Always force to face downwards
            facingDirection = 2;

            HairModel hairModel = hairPack.GetHairFromFacingDirection(facingDirection);
            if (hairModel is null)
            {
                return customBody is null ? true : DrawVanillaHairForPortrait(b, who, position + portraitOffset, layerDepth, scale, facingDirection, customBody);
            }
            Rectangle sourceRect = new Rectangle(hairModel.StartingPosition.X, hairModel.StartingPosition.Y, hairModel.HairSize.Width, Math.Min(hairModel.HairSize.Length, 16));

            // Execute recolor
            DrawPatch.ExecuteRecolorActionsReversePatch(__instance, who);

            // Get the hairs current color
            var hairColor = who.hairstyleColor.Value;
            if (hairModel.DisableGrayscale)
            {
                hairColor = Color.White;
            }
            else if (hairModel.IsPrismatic)
            {
                hairColor = Utility.GetPrismaticColor(speedMultiplier: hairModel.PrismaticAnimationSpeedMultiplier);
            }

            // Get hair metadata
            HairStyleMetadata hair_metadata = Farmer.GetHairStyleMetadata(who.hair.Value);

            // Vanilla logic to determine player's head position (though largely useless as it always executes facingDirection == 2)
            bool flip = false;
            int yOffset = 0;
            int feature_y_offset = 0;
            switch (facingDirection)
            {
                case 0:
                    yOffset = 64;
                    feature_y_offset = AppearanceHelpers.GetFarmerRendererYFeatureOffset(12);
                    break;
                case 3:
                    if (hair_metadata != null && hair_metadata.usesUniqueLeftSprite)
                    {
                        yOffset = 96;
                    }
                    else
                    {
                        yOffset = 32;
                    }
                    feature_y_offset = AppearanceHelpers.GetFarmerRendererYFeatureOffset(6);
                    break;
                case 1:
                    yOffset = 32;
                    feature_y_offset = AppearanceHelpers.GetFarmerRendererYFeatureOffset(6);
                    break;
                case 2:
                    yOffset = 0;
                    feature_y_offset = AppearanceHelpers.GetFarmerRendererYFeatureOffset(0);
                    break;
            }
            feature_y_offset -= who.IsMale ? 1 : 0;
            feature_y_offset += customBody is null ? 0 : customBody.GetFeatureOffset(IApi.Type.Hair, 0) / 4;

            // Draw the player's face, then the custom hairstyle
            if (hasDrawnCustomBody is false)
            {
                b.Draw(___baseTexture, position + portraitOffset, new Rectangle(0, yOffset, 16, who.IsMale ? 15 : 16), Color.White, 0f, Vector2.Zero, scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
            }

            // Draw the hair
            float hair_draw_layer = 2.2E-05f;
            b.Draw(hairPack.Texture, position + portraitOffset + new Vector2(0f, feature_y_offset * 4) * scale / 4f, sourceRect, hairColor, 0f, new Vector2(hairModel.HeadPosition.X, hairModel.HeadPosition.Y), scale, hairModel.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + hair_draw_layer);

            if (hairModel.HasColorMask())
            {
                DrawManager.DrawColorMask(b, hairPack, hairModel, FarmerRendererPatch.AreColorMasksPendingRefresh, position + portraitOffset + new Vector2(0f, feature_y_offset * 4) * scale / 4f, sourceRect, hairColor, new List<Color>(), 0f, new Vector2(hairModel.HeadPosition.X, hairModel.HeadPosition.Y), scale, layerDepth + hair_draw_layer + 0.01E-05f);
            }
            if (hairModel.HasSkinToneMask())
            {
                // Get skin tone
                var skinTone = DrawPatch.GetSkinTone(___farmerTextureManager, ___baseTexture, null, ___skin, ____sickFrame, who);
                DrawManager.DrawSkinToneMask(b, hairPack, hairModel, skinTone, FarmerRendererPatch.AreColorMasksPendingRefresh, position + new Vector2(0f, feature_y_offset * 4) * scale / 4f, sourceRect, hairColor, 0f, new Vector2(hairModel.HeadPosition.X, hairModel.HeadPosition.Y), scale, layerDepth + hair_draw_layer + 0.01E-05f);
            }

            return false;
        }

        private static bool DrawVanillaHairForPortrait(SpriteBatch b, Farmer who, Vector2 position, float layerDepth, float scale, int facingDirection, BodyModel customBody = null)
        {
            int hair_style = who.getHair();
            HairStyleMetadata hair_metadata = Farmer.GetHairStyleMetadata(who.hair.Value);
            if (who != null && who.hat.Value != null && who.hat.Value.hairDrawType.Value == 1 && hair_metadata != null && hair_metadata.coveredIndex != -1)
            {
                hair_style = hair_metadata.coveredIndex;
                hair_metadata = Farmer.GetHairStyleMetadata(hair_style);
            }

            Texture2D hairTexture = FarmerRenderer.hairStylesTexture;
            Rectangle hairstyleSourceRect = new Rectangle(hair_style * 16 % FarmerRenderer.hairStylesTexture.Width, hair_style * 16 / FarmerRenderer.hairStylesTexture.Width * 96, 16, 32);
            if (hair_metadata != null)
            {
                hairTexture = hair_metadata.texture;
                hairstyleSourceRect = new Rectangle(hair_metadata.tileX * 16, hair_metadata.tileY * 16, 16, 32);
            }

            int vanillaHairOffset = ((who.IsMale && hair_style >= 16) ? (-4) : ((!who.IsMale && hair_style < 16) ? 4 : 0));
            switch (facingDirection)
            {
                case 0:
                    hairstyleSourceRect.Offset(0, 64);
                    b.Draw(hairTexture, position + new Vector2(AppearanceHelpers.GetFarmerRendererXFeatureOffset(12) * 4, AppearanceHelpers.GetFarmerRendererYFeatureOffset(12) * 4 + 4 + (customBody is null ? vanillaHairOffset : customBody.GetFeatureOffset(IApi.Type.Hair, vanillaHairOffset))), hairstyleSourceRect, who.hairstyleColor.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
                    break;
                case 1:
                    hairstyleSourceRect.Offset(0, 32);
                    b.Draw(hairTexture, position + new Vector2(AppearanceHelpers.GetFarmerRendererXFeatureOffset(6) * 4, AppearanceHelpers.GetFarmerRendererYFeatureOffset(6) * 4 + (customBody is null ? vanillaHairOffset : customBody.GetFeatureOffset(IApi.Type.Hair, vanillaHairOffset))), hairstyleSourceRect, who.hairstyleColor.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
                    break;
                case 2:
                    b.Draw(hairTexture, position + new Vector2(AppearanceHelpers.GetFarmerRendererXFeatureOffset(0) * 4, AppearanceHelpers.GetFarmerRendererYFeatureOffset(0) * 4 + (customBody is null ? vanillaHairOffset : customBody.GetFeatureOffset(IApi.Type.Hair, vanillaHairOffset))), hairstyleSourceRect, who.hairstyleColor.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
                    break;
                case 3:
                    bool flip2 = true;
                    if (hair_metadata != null && hair_metadata.usesUniqueLeftSprite)
                    {
                        flip2 = false;
                        hairstyleSourceRect.Offset(0, 96);
                    }
                    else
                    {
                        hairstyleSourceRect.Offset(0, 32);
                    }
                    b.Draw(hairTexture, position + new Vector2(-AppearanceHelpers.GetFarmerRendererXFeatureOffset(6) * 4, AppearanceHelpers.GetFarmerRendererYFeatureOffset(6) * 4 + (customBody is null ? vanillaHairOffset : customBody.GetFeatureOffset(IApi.Type.Hair, vanillaHairOffset))), hairstyleSourceRect, who.hairstyleColor.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
                    break;
            }

            return false;
        }
    }
}
