/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FleemUmbleem/DisplayPlayerStats
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley;

namespace ModSandbox
{
	public class UpdateHud
	{
		private int currentStamina;
		private int currentHealth;
		private Vector2 staminaTextPosition;
		private Vector2 healthTextPosition;
		private Rectangle sourceStaminaRect;
		private Rectangle destStaminaRect;
		private Rectangle sourceHealthRect;
		private Rectangle destHealthRect;
		private Rectangle windowSafeArea;
		ModConstants modConstants = new ModConstants();

		public void UpdateHudStatusText(Farmer player, Texture2D tileSheet, RenderingHudEventArgs e)
		{
			// Get the safe area
			windowSafeArea = Utility.getSafeArea();
			int statusIconsDestX = windowSafeArea.Right - modConstants.viewportMarginRightIcons;
			int staminaDestY = windowSafeArea.Bottom - modConstants.viewportMarginBottomStamina;
			int healthDestY = windowSafeArea.Bottom - modConstants.viewportMarginBottomHealth;
			int statusTextX = windowSafeArea.Right - modConstants.viewportMarginRightText;

			// Get stamina and health values
			currentHealth = player.health;
			currentStamina = (int)player.stamina;

			// Create the source rectangles for cropping
			sourceStaminaRect = new Rectangle(modConstants.staminaIconX, modConstants.staminaIconY, modConstants.statusIconWxH, modConstants.statusIconWxH);
			sourceHealthRect = new Rectangle(modConstants.healthIconX, modConstants.healthIconY, modConstants.statusIconWxH, modConstants.statusIconWxH);

			// Scale up the icons
			int destWxH = (int)(modConstants.statusIconWxH * modConstants.scaleFactor);

			// Draw icons in altered position if the player has less than MaxHP
			if (player.health != player.maxHealth)
			{
				SetIconDrawPos(statusIconsDestX, staminaDestY, healthDestY, destWxH, statusTextX, staminaDestY, healthDestY);
            }
			else
			{
				// Draw icons in a different place if on full HP and in one of the locations where the health bar spawns
				switch (Game1.currentLocation)
				{
					case MineShaft _:
					case Woods _:
					case SlimeHutch _:
					case VolcanoDungeon _:
					SetIconDrawPos(statusIconsDestX, staminaDestY, healthDestY, destWxH, statusTextX, staminaDestY, healthDestY);
						break;
					default:
					SetIconDrawPos(statusIconsDestX + modConstants.healthBarOffset, staminaDestY, healthDestY, destWxH, statusTextX + modConstants.healthBarOffset, staminaDestY, healthDestY);
                        break;
				}
            }

			// Draw value and icon to the screen
			SpriteBatch spriteBatch = e.SpriteBatch;
			spriteBatch.DrawString(Game1.smallFont, $"{currentStamina}", staminaTextPosition, Color.White);
			spriteBatch.Draw(tileSheet, destStaminaRect, sourceStaminaRect, Color.White);
			spriteBatch.DrawString(Game1.smallFont, $"{currentHealth}", healthTextPosition, Color.White);
			spriteBatch.Draw(tileSheet, destHealthRect, sourceHealthRect, Color.White);
		}

		private void SetIconDrawPos(int xPosIcon, int yPosStamina, int YPosHealth, int destWxH, int statusTextX, int staminaDestY, int healthDestY)
		{
			destStaminaRect = new Rectangle(xPosIcon, yPosStamina, destWxH, destWxH);
			destHealthRect = new Rectangle(xPosIcon, YPosHealth, destWxH, destWxH);
			staminaTextPosition = new Vector2(statusTextX, staminaDestY);
			healthTextPosition = new Vector2(statusTextX, healthDestY);
		}
	}
}
