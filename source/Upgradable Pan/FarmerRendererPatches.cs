/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/UpgradeablePan
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using StardewValley;
using System.Reflection;

namespace UpgradablePan
{
	public class FarmerRendererPatches
	{

		public static bool draw_Prefix(ref FarmerRenderer __instance, SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect,
								Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who,
								ref PanHat __state)
		{
			// Store the hat if it's a pan.
			if (who.hat.Value != null && who.hat.Value is PanHat panHat)
			{
				__state = panHat;
				who.hat.Set(null);
			}
			return true;
		}

		public static void draw_Postfix(ref FarmerRenderer __instance, SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect,
								Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who,
								Texture2D ___baseTexture, Vector2 ___positionOffset, ref PanHat __state)
		{
			// Draw our updated graphics over the original based on the upgrade level.
			if (who.UsingTool && who.CurrentTool is Pan pan)
			{
				switch (pan.UpgradeLevel)
				{
					case 2:
						sourceRect.Offset(48, 0);
						break;
					case 3:
						sourceRect.Offset(96, 0);
						break;
					case 4:
						sourceRect.Offset(-48, 0);
						break;
					default:
						return;
				}
				b.Draw(___baseTexture, position + origin + ___positionOffset + who.armOffset, sourceRect, overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 5.0E-05f);
			}

			// Draw updated hat graphics over the original.
			if (__state != null)
			{
				bool flip = who.FarmerSprite.CurrentAnimationFrame.flip;
				float layer_offset2 = 4.0E-05f;
				Rectangle hatSourceRect = new Rectangle(20 * (__state.UpgradeLevel - 2), 20 * (__state.UpgradeLevel - 2) / PanHat.panHatTexture.Width * 20 * 4, 20, 20);

				b.Draw(PanHat.panHatTexture, position + origin + ___positionOffset + new Vector2(-8 + ((!flip) ? 1 : (-1)) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((!__state.ignoreHairstyleOffset) ? FarmerRenderer.hairstyleHatOffset[(int)who.hair % 16] : 0) + 4 + __instance.heightOffset.Value), hatSourceRect, __state.isPrismatic ? Utility.GetPrismaticColor() : Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + layer_offset2);
				who.hat.Set(__state);
			}
		}
	}
}