/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace DialogueBoxRedesign.Patching
{
    public static class HarmonyPatchExecutors
    {
	    private static int widthOfPortraitArea = 444;
	    
	    public static void DrawPortrait(DialogueBox dialogueBox, SpriteBatch spriteBatch)
	    {
		    if (dialogueBox.width < 642) return;
		    
		    var xPositionOfPortraitArea = dialogueBox.x + dialogueBox.width - widthOfPortraitArea;
		    
		    var portraitBoxX = xPositionOfPortraitArea + 76;
		    var portraitBoxY = dialogueBox.y + dialogueBox.height / 2 - 148 - 36;

		    var portraitTexture = dialogueBox.characterDialogue.overridePortrait ?? dialogueBox.characterDialogue.speaker.Portrait;

		    var portraitScale = 4f;

		    var portraitSource = Game1.getSourceRectForStandardTileSheet(portraitTexture,
			    dialogueBox.characterDialogue.getPortraitIndex(), 64, 64);
		    
		    if (!portraitTexture.Bounds.Contains(portraitSource)) portraitSource = new Rectangle(0, 0, 64, 64);

		    var xOffset = Utility.Utility.ShouldPortraitShake(dialogueBox, dialogueBox.characterDialogue)
			    ? Game1.random.Next(-1, 2)
			    : 0;
		    
		    /* Portrait */
		    spriteBatch.Draw(portraitTexture, new Vector2(portraitBoxX + 16 + xOffset, Game1.uiViewport.Height - portraitSource.Height * portraitScale),
				    portraitSource, Color.White, 0f, Vector2.Zero, portraitScale, SpriteEffects.None, 0.88f);

		    var speakerNameX = xPositionOfPortraitArea + widthOfPortraitArea / 2;
		    var speakerNameY = portraitBoxY + 50;

		    /* Speaker name */
		    // shadow
		    SpriteText.drawStringHorizontallyCenteredAt(spriteBatch, dialogueBox.characterDialogue.speaker.getName(),
			    speakerNameX - 4, speakerNameY + 4, color: 8);
		    // actual text
		    SpriteText.drawStringHorizontallyCenteredAt(spriteBatch, dialogueBox.characterDialogue.speaker.getName(),
			    speakerNameX, speakerNameY, color: 4);

		    if (dialogueBox.shouldDrawFriendshipJewel())
		    {
			    /* Friendship jewel */
			    var jewelBottomOffset = 80;
			    var jewelLeftOffset = 40;
			    
			    dialogueBox.friendshipJewel.Y = Game1.uiViewport.Height - jewelBottomOffset;
			    dialogueBox.friendshipJewel.X = (int)(portraitBoxX + 64 * portraitScale + jewelLeftOffset);

			    spriteBatch.Draw(Game1.mouseCursors,
				    new Vector2(dialogueBox.friendshipJewel.X, dialogueBox.friendshipJewel.Y),
				    Game1.player.getFriendshipHeartLevelForNPC(dialogueBox.characterDialogue.speaker.Name) >= 10
					    ? new Rectangle(269, 494, 11, 11)
					    : new Rectangle(
						    Math.Max(140,
							    140 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 / 250.0) *
							    11),
						    Math.Max(532,
							    532 + Game1.player.getFriendshipHeartLevelForNPC(dialogueBox.characterDialogue.speaker
								    .Name) / 2 * 11), 11, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None,
				    0.88f);
		    }
	    }

	    public static bool DrawBox(DialogueBox dialogueBox, SpriteBatch spriteBatch, int xPos, int yPos, int boxWidth,
		    int boxHeight)
	    {
		    if (!dialogueBox.transitionInitialized) return true;
		    if (!dialogueBox.isPortraitBox()) return true;
		    if (dialogueBox.isQuestion) return true;

		    dialogueBox.height = 250;
		    dialogueBox.y = Game1.uiViewport.Height - dialogueBox.height - 64;

		    var gradientBackground = Game1.currentSeason == "winter" && ModEntry.Config.DarkerBackgroundInWinter
			    ? ModEntry.DarkerGradientSample
			    : ModEntry.GradientSample;
		    
		    spriteBatch.Draw(gradientBackground, new Rectangle(0, yPos, Game1.viewport.Width, Game1.viewport.Height - yPos), Color.White);

		    return false;
	    }

	    public static bool Draw(DialogueBox dialogueBox, SpriteBatch spriteBatch)
	    {
		    if (dialogueBox.width < 16 || dialogueBox.height < 16 || dialogueBox.transitioning || dialogueBox.isQuestion)
			{
				return true;
			}
		    if (!dialogueBox.isPortraitBox() || dialogueBox.isQuestion) return true;
		    
		    var viewportWidth = Game1.uiViewport.Width;
		    
		    dialogueBox.width = viewportWidth > 1600 ? 1500 : 1200;
		    dialogueBox.x = (viewportWidth - dialogueBox.width) / 2;
		    
		    dialogueBox.drawBox(spriteBatch, dialogueBox.x, dialogueBox.y, dialogueBox.width, dialogueBox.height);
			dialogueBox.drawPortrait(spriteBatch);
			
			var textX = dialogueBox.x;
			var textY = dialogueBox.y + 58;

			var textWidth = dialogueBox.width - widthOfPortraitArea;
				
			// shadow
			SpriteText.drawString(spriteBatch, dialogueBox.getCurrentString(), textX - 4, textY + 4, dialogueBox.characterIndexInDialogue, textWidth, color: 8);
			// actual text
			SpriteText.drawString(spriteBatch, dialogueBox.getCurrentString(), textX, textY, dialogueBox.characterIndexInDialogue, textWidth, color: 4);

			var hoverText = ModEntry.ModHelper.Reflection.GetField<string>(dialogueBox, "hoverText").GetValue();
			if (hoverText.Length > 0)
			{
				SpriteText.drawStringWithScrollBackground(spriteBatch, hoverText, dialogueBox.friendshipJewel.Center.X - SpriteText.getWidthOfString(hoverText) / 2, dialogueBox.friendshipJewel.Y - 64);
			}
				
			dialogueBox.drawMouse(spriteBatch);
				
			return false;

	    }
    }
}
