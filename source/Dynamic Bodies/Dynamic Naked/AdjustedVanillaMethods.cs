/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;
using System.IO;

namespace DynamicBodies
{
    internal class AdjustedVanillaMethods
    {
        /// <summary>
        /// Optimised from source code to be just the shirt
        /// </summary>
        /// <param name="farmerRenderer"></param>
        /// <param name="positionOffset"></param>
        /// <param name="rotationAdjustment"></param>
        /// <param name="___shirtSourceRect"></param>
        /// <param name="b"></param>
        /// <param name="facingDirection"></param>
        /// <param name="who"></param>
        /// <param name="position"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="currentFrame"></param>
        /// <param name="rotation"></param>
        /// <param name="overrideColor"></param>
        /// <param name="layerDepth"></param>
        internal static void DrawShirt(FarmerRenderer farmerRenderer, Texture2D shirtsTexture, Vector2 positionOffset, Vector2 rotationAdjustment, ref Rectangle ___shirtSourceRect, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth, bool dyeShirt = true, int shirtIndex = -1)
        {
            int clamped_shirt_index = shirtIndex;
            if (shirtIndex < 0)
            {
                //load from the item 
                clamped_shirt_index = (who.GetShirtIndex() > StardewValley.Objects.Clothing.GetMaxShirtValue() || who.GetShirtIndex() < 0) ? 0 : who.GetShirtIndex();
            }

            Rectangle shirtSourceRect = new Rectangle(clamped_shirt_index * 8 % 128, clamped_shirt_index * 8 / 128 * 32, 8, 8);

            Rectangle dyed_shirt_source_rect = shirtSourceRect;
            float dye_layer_offset = 1.2E-07f;
            switch (facingDirection)
            {
                case 0:
                    shirtSourceRect.Offset(0, 24);
                    dyed_shirt_source_rect = shirtSourceRect;
                    dyed_shirt_source_rect.Offset(128, 0);
                    b.Draw(shirtsTexture, position + origin + positionOffset + new Vector2(16f * scale + (float)(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float)(56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)farmerRenderer.heightOffset.Value * scale), shirtSourceRect, overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.8E-07f);
                    if(dyeShirt) b.Draw(shirtsTexture, position + origin + positionOffset + new Vector2(16f * scale + (float)(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float)(56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)farmerRenderer.heightOffset.Value * scale), dyed_shirt_source_rect, overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.8E-07f + dye_layer_offset);
                    break;
                case 1:
                    shirtSourceRect.Offset(0, 8);
                    dyed_shirt_source_rect = shirtSourceRect;
                    dyed_shirt_source_rect.Offset(128, 0);
                    b.Draw(shirtsTexture, position + origin + positionOffset + rotationAdjustment + new Vector2(16f * scale + (float)(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), 56f * scale + (float)(FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)farmerRenderer.heightOffset.Value * scale), shirtSourceRect, overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, 4f * scale + ((rotation != 0f) ? 0f : 0f), SpriteEffects.None, layerDepth + 1.8E-07f);
                    if (dyeShirt) b.Draw(shirtsTexture, position + origin + positionOffset + rotationAdjustment + new Vector2(16f * scale + (float)(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), 56f * scale + (float)(FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)farmerRenderer.heightOffset.Value * scale), dyed_shirt_source_rect, overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, 4f * scale + ((rotation != 0f) ? 0f : 0f), SpriteEffects.None, layerDepth + 1.8E-07f + dye_layer_offset);
                    break;
                case 2:
                    dyed_shirt_source_rect = shirtSourceRect;
                    dyed_shirt_source_rect.Offset(128, 0);
                    b.Draw(shirtsTexture, position + origin + positionOffset + new Vector2(16 + FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, (float)(56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)farmerRenderer.heightOffset.Value * scale - (float)(who.IsMale ? 0 : 0)), shirtSourceRect, overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.5E-07f);
                    if (dyeShirt) b.Draw(shirtsTexture, position + origin + positionOffset + new Vector2(16 + FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, (float)(56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)farmerRenderer.heightOffset.Value * scale - (float)(who.IsMale ? 0 : 0)), dyed_shirt_source_rect, overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.5E-07f + dye_layer_offset);
                    break;
                case 3:
                    shirtSourceRect.Offset(0, 16);
                    dyed_shirt_source_rect = shirtSourceRect;
                    dyed_shirt_source_rect.Offset(128, 0);
                    b.Draw(shirtsTexture, position + origin + positionOffset + rotationAdjustment + new Vector2(16 - FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)farmerRenderer.heightOffset.Value), shirtSourceRect, overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, 4f * scale + ((rotation != 0f) ? 0f : 0f), SpriteEffects.None, layerDepth + 1.5E-07f);
                    if (dyeShirt) b.Draw(shirtsTexture, position + origin + positionOffset + rotationAdjustment + new Vector2(16 - FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)farmerRenderer.heightOffset.Value), dyed_shirt_source_rect, overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, 4f * scale + ((rotation != 0f) ? 0f : 0f), SpriteEffects.None, layerDepth + 1.5E-07f + dye_layer_offset);
                    break;
            }
        }
        /// <summary>
        /// Optimised from the source code, only renders accessories (beards handled elsewhere)
        /// </summary>
        /// <param name="farmerRenderer"></param>
        /// <param name="positionOffset"></param>
        /// <param name="rotationAdjustment"></param>
        /// <param name="___accessorySourceRect"></param>
        /// <param name="b"></param>
        /// <param name="facingDirection"></param>
        /// <param name="who"></param>
        /// <param name="position"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="currentFrame"></param>
        /// <param name="rotation"></param>
        /// <param name="overrideColor"></param>
        /// <param name="layerDepth"></param>
        internal static void DrawAccessory(FarmerRenderer farmerRenderer, Vector2 positionOffset, Vector2 rotationAdjustment, ref Rectangle ___accessorySourceRect, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth)
        {

            Rectangle accessorySourceRect = new Rectangle((int)who.accessory.Value * 16 % FarmerRenderer.accessoriesTexture.Width, (int)who.accessory.Value * 16 / FarmerRenderer.accessoriesTexture.Width * 32, 16, 16);

            switch (facingDirection)
            {
                case 0:
                    break;
                case 1:
                    accessorySourceRect.Offset(0, 16);

                    if (rotation == -(float)Math.PI / 32f)
                    {
                        rotationAdjustment.X = 6f;
                        rotationAdjustment.Y = -2f;
                    }
                    else if (rotation == (float)Math.PI / 32f)
                    {
                        rotationAdjustment.X = -6f;
                        rotationAdjustment.Y = 1f;
                    }
                    b.Draw(FarmerRenderer.accessoriesTexture, position + origin + positionOffset + rotationAdjustment + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 4 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)farmerRenderer.heightOffset.Value), accessorySourceRect, (overrideColor.Equals(Color.White) && (int)who.accessory.Value < 6) ? ((Color)who.hairstyleColor.Value) : overrideColor, rotation, origin, 4f * scale + ((rotation != 0f) ? 0f : 0f), SpriteEffects.None, layerDepth + (((int)who.accessory.Value < 8) ? 1.9E-05f : 2.9E-05f));

                    break;
                case 2:
                    b.Draw(FarmerRenderer.accessoriesTexture, position + origin + positionOffset + rotationAdjustment + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 8 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)farmerRenderer.heightOffset.Value - 4), accessorySourceRect, (overrideColor.Equals(Color.White) && (int)who.accessory.Value < 6) ? ((Color)who.hairstyleColor.Value) : overrideColor, rotation, origin, 4f * scale + ((rotation != 0f) ? 0f : 0f), SpriteEffects.None, layerDepth + (((int)who.accessory.Value < 8) ? 1.9E-05f : 2.9E-05f));
                    break;
                case 3:
                    accessorySourceRect.Offset(0, 16);

                    if (rotation == -(float)Math.PI / 32f)
                    {
                        rotationAdjustment.X = 6f;
                        rotationAdjustment.Y = -2f;
                    }
                    else if (rotation == (float)Math.PI / 32f)
                    {
                        rotationAdjustment.X = -5f;
                        rotationAdjustment.Y = 1f;
                    }
                    b.Draw(FarmerRenderer.accessoriesTexture, position + origin + positionOffset + rotationAdjustment + new Vector2(-FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 4 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)farmerRenderer.heightOffset.Value), accessorySourceRect, (overrideColor.Equals(Color.White) && (int)who.accessory.Value < 6) ? ((Color)who.hairstyleColor.Value) : overrideColor, rotation, origin, 4f * scale + ((rotation != 0f) ? 0f : 0f), SpriteEffects.FlipHorizontally, layerDepth + (((int)who.accessory.Value < 8) ? 1.9E-05f : 2.9E-05f));
                    break;
            }
        }
        /// <summary>
        /// Mostly unadjusted from sourcecode for drawing hats
        /// </summary>
        /// <param name="farmerRenderer"></param>
        /// <param name="positionOffset"></param>
        /// <param name="___hatSourceRect"></param>
        /// <param name="b"></param>
        /// <param name="facingDirection"></param>
        /// <param name="who"></param>
        /// <param name="position"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="currentFrame"></param>
        /// <param name="rotation"></param>
        /// <param name="layerDepth"></param>
        internal static void DrawHat(FarmerRenderer farmerRenderer, Vector2 positionOffset, ref Rectangle ___hatSourceRect, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, float layerDepth)
        {
            Rectangle hatSourceRect = new Rectangle(20 * (int)who.hat.Value.which.Value % FarmerRenderer.hatsTexture.Width, 20 * (int)who.hat.Value.which.Value / FarmerRenderer.hatsTexture.Width * 20 * 4, 20, 20);

            switch (facingDirection)
            {
                case 0:
                    if (who.hat.Value != null)
                    {
                        hatSourceRect.Offset(0, 60);
                    }
                    break;
                case 1:
                    if (who.hat.Value != null)
                    {
                        hatSourceRect.Offset(0, 20);
                    }
                    break;
                case 2:
                    break;
                case 3:
                    if (who.hat.Value != null)
                    {
                        hatSourceRect.Offset(0, 40);
                    }
                    break;
            }
            if (who.hat.Value != null && !who.bathingClothes.Value)
            {
                bool flip = who.FarmerSprite.CurrentAnimationFrame.flip;
                float layer_offset = 3.9E-05f;
                if (who.hat.Value.isMask && who.facingDirection == 0)
                {
                    Rectangle mask_draw_rect = hatSourceRect;
                    mask_draw_rect.Height -= 11;
                    mask_draw_rect.Y += 11;
                    b.Draw(FarmerRenderer.hatsTexture, position + origin + positionOffset + new Vector2(0f, 44f) + new Vector2(-8 + ((!flip) ? 1 : (-1)) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((!who.hat.Value.ignoreHairstyleOffset.Value) ? FarmerRenderer.hairstyleHatOffset[(int)who.hair.Value % 16] : 0) + 4 + (int)farmerRenderer.heightOffset.Value), mask_draw_rect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + layer_offset);
                    mask_draw_rect = hatSourceRect;
                    mask_draw_rect.Height = 11;
                    layer_offset = -1E-06f;
                    b.Draw(FarmerRenderer.hatsTexture, position + origin + positionOffset + new Vector2(-8 + ((!flip) ? 1 : (-1)) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((!who.hat.Value.ignoreHairstyleOffset.Value) ? FarmerRenderer.hairstyleHatOffset[(int)who.hair.Value % 16] : 0) + 4 + (int)farmerRenderer.heightOffset.Value), mask_draw_rect, who.hat.Value.isPrismatic.Value ? Utility.GetPrismaticColor() : Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + layer_offset);
                }
                else
                {
                    b.Draw(FarmerRenderer.hatsTexture, position + origin + positionOffset + new Vector2(-8 + ((!flip) ? 1 : (-1)) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((!who.hat.Value.ignoreHairstyleOffset.Value) ? FarmerRenderer.hairstyleHatOffset[(int)who.hair.Value % 16] : 0) + 4 + (int)farmerRenderer.heightOffset.Value), hatSourceRect, who.hat.Value.isPrismatic.Value ? Utility.GetPrismaticColor() : Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + layer_offset);
                }
            }
        }

        public static int ClampPants(Farmer who)
        {
            return (who.GetPantsIndex() > StardewValley.Objects.Clothing.GetMaxPantsValue() || who.GetPantsIndex() < 0) ? 0 : who.GetPantsIndex();
        }

		public static void drawBase(FarmerRenderer farmerRenderer, ref Vector2 _rotationAdjustment, ref Vector2 _positionOffset, ref Texture2D _baseTexture, SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, ref Rectangle sourceRect, ref Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who)
		{
			position = new Vector2((float)Math.Floor(position.X), (float)Math.Floor(position.Y));
			_rotationAdjustment = Vector2.Zero;
			_positionOffset.Y = animationFrame.positionOffset * 4;
			_positionOffset.X = animationFrame.xOffset * 4;
			if (!FarmerRenderer.isDrawingForUI && (bool)who.swimming)
			{
				sourceRect.Height /= 2;
				sourceRect.Height -= (int)who.yOffset / 4;
				position.Y += 64f;
			}
			if (facingDirection == 3 || facingDirection == 1)
			{
				facingDirection = ((!animationFrame.flip) ? 1 : 3);
			}
			b.Draw(_baseTexture, position + origin + _positionOffset, sourceRect, overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
			if (!FarmerRenderer.isDrawingForUI && (bool)who.swimming)
			{
				if (who.currentEyes != 0 && who.FacingDirection != 0 && (Game1.timeOfDay < 2600 || (who.isInBed.Value && who.timeWentToBed.Value != 0)) && ((!who.FarmerSprite.PauseForSingleAnimation && !who.UsingTool) || (who.UsingTool && who.CurrentTool is FishingRod)))
				{
					b.Draw(_baseTexture, position + origin + _positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4 + 20 + ((who.FacingDirection == 1) ? 12 : ((who.FacingDirection == 3) ? 4 : 0)), FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 40), new Rectangle(5, 16, (who.FacingDirection == 2) ? 6 : 2, 2), overrideColor, 0f, origin, 4f * scale, SpriteEffects.None, layerDepth + 5E-08f);
					b.Draw(_baseTexture, position + origin + _positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4 + 20 + ((who.FacingDirection == 1) ? 12 : ((who.FacingDirection == 3) ? 4 : 0)), FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 40), new Rectangle(264 + ((who.FacingDirection == 3) ? 4 : 0), 2 + (who.currentEyes - 1) * 2, (who.FacingDirection == 2) ? 6 : 2, 2), overrideColor, 0f, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.2E-07f);
				}
				farmerRenderer.drawHairAndAccesories(b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);
				b.Draw(Game1.staminaRect, new Rectangle((int)position.X + (int)who.yOffset + 8, (int)position.Y - 128 + sourceRect.Height * 4 + (int)origin.Y - (int)who.yOffset, sourceRect.Width * 4 - (int)who.yOffset * 2 - 16, 4), Game1.staminaRect.Bounds, Color.White * 0.75f, 0f, Vector2.Zero, SpriteEffects.None, layerDepth + 0.001f);
				return;
			}
		}
		public static void drawPants(FarmerRenderer farmerRenderer, ref Vector2 _rotationAdjustment, ref Vector2 _positionOffset, ref Texture2D _baseTexture, SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect, Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who)
		{
			Rectangle pants_rect = new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height);
			pants_rect.X += ClampPants(who) % 10 * 192;
			pants_rect.Y += ClampPants(who) / 10 * 688;
			if (!who.IsMale)
			{
				pants_rect.X += 96;
			}
			b.Draw(FarmerRenderer.pantsTexture, position + origin + _positionOffset, pants_rect, overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetPantsColor()) : overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + ((who.FarmerSprite.CurrentAnimationFrame.frame == 5) ? 0.00092f : 9.2E-08f));
		}
		public static void drawEyes(FarmerRenderer farmerRenderer, ref Vector2 _rotationAdjustment, ref Vector2 _positionOffset, ref Texture2D _baseTexture, SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect, Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who)
		{
			sourceRect.Offset(288, 0);
			if (who.currentEyes != 0 && facingDirection != 0 && (Game1.timeOfDay < 2600 || (who.isInBed.Value && who.timeWentToBed.Value != 0)) && ((!who.FarmerSprite.PauseForSingleAnimation && !who.UsingTool) || (who.UsingTool && who.CurrentTool is FishingRod)) && (!who.UsingTool || !(who.CurrentTool is FishingRod fishing_rod) || fishing_rod.isFishing))
			{
				int x_adjustment = 5;
				x_adjustment = (animationFrame.flip ? (x_adjustment - FarmerRenderer.featureXOffsetPerFrame[currentFrame]) : (x_adjustment + FarmerRenderer.featureXOffsetPerFrame[currentFrame]));
				switch (facingDirection)
				{
					case 1:
						x_adjustment += 3;
						break;
					case 3:
						x_adjustment++;
						break;
				}
				x_adjustment *= 4;
				b.Draw(_baseTexture, position + origin + _positionOffset + new Vector2(x_adjustment, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && who.FacingDirection != 2) ? 36 : 40)), new Rectangle(5, 16, (facingDirection == 2) ? 6 : 2, 2), overrideColor, 0f, origin, 4f * scale, SpriteEffects.None, layerDepth + 5E-08f);
				b.Draw(_baseTexture, position + origin + _positionOffset + new Vector2(x_adjustment, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.FacingDirection == 1 || who.FacingDirection == 3) ? 40 : 44)), new Rectangle(264 + ((facingDirection == 3) ? 4 : 0), 2 + (who.currentEyes - 1) * 2, (facingDirection == 2) ? 6 : 2, 2), overrideColor, 0f, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.2E-07f);
			}
		}

		public static void drawArms(FarmerRenderer farmerRenderer, ref Vector2 _rotationAdjustment, ref Vector2 _positionOffset, ref Texture2D _baseTexture, SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect, Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who)
		{
			float arm_layer_offset = 4.9E-05f;
			if (facingDirection == 0)
			{
				arm_layer_offset = -1E-07f;
			}
			sourceRect.Offset(animationFrame.secondaryArm ? 192 : 96, 0);
			b.Draw(_baseTexture, position + origin + _positionOffset + who.armOffset, sourceRect, overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + arm_layer_offset);
			if (!who.usingSlingshot || !(who.CurrentTool is Slingshot))
			{
				return;
			}
			Slingshot slingshot = who.CurrentTool as Slingshot;
			Point point = Utility.Vector2ToPoint(slingshot.AdjustForHeight(Utility.PointToVector2(slingshot.aimPos.Value)));
			int mouseX = point.X;
			int y = point.Y;
			int backArmDistance = slingshot.GetBackArmDistance(who);
			Vector2 shoot_origin = slingshot.GetShootOrigin(who);
			float frontArmRotation = (float)Math.Atan2((float)y - shoot_origin.Y, (float)mouseX - shoot_origin.X) + (float)Math.PI;
			if (!Game1.options.useLegacySlingshotFiring)
			{
				frontArmRotation -= (float)Math.PI;
				if (frontArmRotation < 0f)
				{
					frontArmRotation += (float)Math.PI * 2f;
				}
			}
			switch (facingDirection)
			{
				case 0:
					b.Draw(_baseTexture, position + new Vector2(4f + frontArmRotation * 8f, -44f), new Rectangle(173, 238, 9, 14), Color.White, 0f, new Vector2(4f, 11f), 4f * scale, SpriteEffects.None, layerDepth + ((facingDirection != 0) ? 5.9E-05f : (-0.0005f)));
					break;
				case 1:
					{
						b.Draw(_baseTexture, position + new Vector2(52 - backArmDistance, -32f), new Rectangle(147, 237, 10, 4), Color.White, 0f, new Vector2(8f, 3f), 4f * scale, SpriteEffects.None, layerDepth + ((facingDirection != 0) ? 5.9E-05f : 0f));
						b.Draw(_baseTexture, position + new Vector2(36f, -44f), new Rectangle(156, 244, 9, 10), Color.White, frontArmRotation, new Vector2(0f, 3f), 4f * scale, SpriteEffects.None, layerDepth + ((facingDirection != 0) ? 1E-08f : 0f));
						int slingshotAttachX = (int)(Math.Cos(frontArmRotation + (float)Math.PI / 2f) * (double)(20 - backArmDistance - 8) - Math.Sin(frontArmRotation + (float)Math.PI / 2f) * -68.0);
						int slingshotAttachY = (int)(Math.Sin(frontArmRotation + (float)Math.PI / 2f) * (double)(20 - backArmDistance - 8) + Math.Cos(frontArmRotation + (float)Math.PI / 2f) * -68.0);
						Utility.drawLineWithScreenCoordinates((int)(position.X + 52f - (float)backArmDistance), (int)(position.Y - 32f - 4f), (int)(position.X + 32f + (float)(slingshotAttachX / 2)), (int)(position.Y - 32f - 12f + (float)(slingshotAttachY / 2)), b, Color.White);
						break;
					}
				case 3:
					{
						b.Draw(_baseTexture, position + new Vector2(40 + backArmDistance, -32f), new Rectangle(147, 237, 10, 4), Color.White, 0f, new Vector2(9f, 4f), 4f * scale, SpriteEffects.FlipHorizontally, layerDepth + ((facingDirection != 0) ? 5.9E-05f : 0f));
						b.Draw(_baseTexture, position + new Vector2(24f, -40f), new Rectangle(156, 244, 9, 10), Color.White, frontArmRotation + (float)Math.PI, new Vector2(8f, 3f), 4f * scale, SpriteEffects.FlipHorizontally, layerDepth + ((facingDirection != 0) ? 1E-08f : 0f));
						int slingshotAttachX = (int)(Math.Cos(frontArmRotation + (float)Math.PI * 2f / 5f) * (double)(20 + backArmDistance - 8) - Math.Sin(frontArmRotation + (float)Math.PI * 2f / 5f) * -68.0);
						int slingshotAttachY = (int)(Math.Sin(frontArmRotation + (float)Math.PI * 2f / 5f) * (double)(20 + backArmDistance - 8) + Math.Cos(frontArmRotation + (float)Math.PI * 2f / 5f) * -68.0);
						Utility.drawLineWithScreenCoordinates((int)(position.X + 4f + (float)backArmDistance), (int)(position.Y - 32f - 8f), (int)(position.X + 26f + (float)slingshotAttachX * 4f / 10f), (int)(position.Y - 32f - 8f + (float)slingshotAttachY * 4f / 10f), b, Color.White);
						break;
					}
				case 2:
					b.Draw(_baseTexture, position + new Vector2(4f, -32 - backArmDistance / 2), new Rectangle(148, 244, 4, 4), Color.White, 0f, Vector2.Zero, 4f * scale, SpriteEffects.None, layerDepth + ((facingDirection != 0) ? 5.9E-05f : 0f));
					Utility.drawLineWithScreenCoordinates((int)(position.X + 16f), (int)(position.Y - 28f - (float)(backArmDistance / 2)), (int)(position.X + 44f - frontArmRotation * 10f), (int)(position.Y - 16f - 8f), b, Color.White);
					Utility.drawLineWithScreenCoordinates((int)(position.X + 16f), (int)(position.Y - 28f - (float)(backArmDistance / 2)), (int)(position.X + 56f - frontArmRotation * 10f), (int)(position.Y - 16f - 8f), b, Color.White);
					b.Draw(_baseTexture, position + new Vector2(44f - frontArmRotation * 10f, -16f), new Rectangle(167, 235, 7, 9), Color.White, 0f, new Vector2(3f, 5f), 4f * scale, SpriteEffects.None, layerDepth + ((facingDirection != 0) ? 5.9E-05f : 0f));
					break;
			}
		}

		
	}
}
