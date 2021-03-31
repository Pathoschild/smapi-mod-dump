/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/heyseth/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using System;

namespace heysethUmbrellas
{
	public class FarmerRendererPatches
	{
		private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
		{
			Monitor = monitor;
		}

		public static void draw_Postfix(ref FarmerRenderer __instance, SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect,
								Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who,
								Texture2D ___baseTexture, Vector2 ___positionOffset)
		{
			try
			{
				if (ModEntry.drawUmbrella)
                {

					if (ModEntry.isMaleFarmer)
                    {
						if (Game1.player.FarmerSprite.currentFrame == 12)
						{ //standing back
							b.Draw(ModEntry.umbrellaOverlayTextureBack, position + origin + ___positionOffset + who.armOffset + new Vector2(0, -4 * scale), new Rectangle(0, 0, 16, 16), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 5.0E-05f);
						}
						else if (Game1.player.FarmerSprite.currentFrame == 13 || Game1.player.FarmerSprite.currentFrame == 14 || Game1.player.FarmerSprite.currentFrame == 22 || Game1.player.FarmerSprite.currentFrame == 23)
						{ //moving back
							b.Draw(ModEntry.umbrellaOverlayTextureBack, position + origin + ___positionOffset + who.armOffset, new Rectangle(0, 0, 16, 16), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 5.0E-05f);
						}
						else if (Game1.player.FarmerSprite.currentFrame == 113)
						{ //sitting back
							b.Draw(ModEntry.umbrellaOverlayTextureBack, position + origin + ___positionOffset + who.armOffset + new Vector2(0, -20 * scale), new Rectangle(0, 0, 16, 16), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 5.0E-05f);
						}
						else if (Game1.player.FarmerSprite.currentFrame == 117)
						{ //sitting side
							b.Draw(ModEntry.umbrellaOverlayTextureSide, position + origin + ___positionOffset + who.armOffset + new Vector2(0, -20 * scale), new Rectangle(0, 0, 16, 16), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 5.0E-05f);
						}
						else if (Game1.player.FarmerSprite.currentFrame == 107)
						{ //sitting front
							b.Draw(ModEntry.umbrellaOverlayTextureBack, position + origin + ___positionOffset + who.armOffset + new Vector2(0, -24 * scale), new Rectangle(0, 0, 16, 16), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 5.0E-05f);
						}
					} else
                    {
						if (Game1.player.FarmerSprite.currentFrame == 12)
						{ //standing back
							b.Draw(ModEntry.umbrellaOverlayTextureBack, position + origin + ___positionOffset + who.armOffset, new Rectangle(0, 0, 16, 16), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 5.0E-05f);
						}
						else if (Game1.player.FarmerSprite.currentFrame == 13 || Game1.player.FarmerSprite.currentFrame == 14 || Game1.player.FarmerSprite.currentFrame == 22 || Game1.player.FarmerSprite.currentFrame == 23)
						{ //moving back
							b.Draw(ModEntry.umbrellaOverlayTextureBack, position + origin + ___positionOffset + who.armOffset + new Vector2(0, 4 * scale), new Rectangle(0, 0, 16, 16), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 5.0E-05f);
						}
						else if (Game1.player.FarmerSprite.currentFrame == 113)
						{ //sitting back
							b.Draw(ModEntry.umbrellaOverlayTextureBack, position + origin + ___positionOffset + who.armOffset + new Vector2(0, -16 * scale), new Rectangle(0, 0, 16, 16), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 5.0E-05f);
						}
						else if (Game1.player.FarmerSprite.currentFrame == 117)
						{ //sitting side
							b.Draw(ModEntry.umbrellaOverlayTextureSide, position + origin + ___positionOffset + who.armOffset + new Vector2(0, -16 * scale), new Rectangle(0, 0, 16, 16), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 5.0E-05f);
						}
						else if (Game1.player.FarmerSprite.currentFrame == 107)
						{ //sitting front
							b.Draw(ModEntry.umbrellaOverlayTextureBack, position + origin + ___positionOffset + who.armOffset + new Vector2(0, -20 * scale), new Rectangle(0, 0, 16, 16), overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 5.0E-05f);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(draw_Postfix)}:\n{ex}", LogLevel.Error);
			}
		}
	}
}