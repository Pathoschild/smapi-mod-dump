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
	    private static int _widthOfPortraitArea = 444;
	    
	    public static void DrawPortrait(DialogueBox dialogueBox, SpriteBatch spriteBatch)
	    {
		    if (dialogueBox.width < 642) return;
		    
		    int xPositionOfPortraitArea;
		    if (ModEntry.Config.ShowPortraitOnTheLeft)
		    {
			    xPositionOfPortraitArea = dialogueBox.x;
		    }
		    else
		    {
			    xPositionOfPortraitArea = dialogueBox.x + dialogueBox.width - _widthOfPortraitArea;
		    }
		    
		    var portraitBoxX = xPositionOfPortraitArea + 76;
		    var portraitBoxY = dialogueBox.y + dialogueBox.height / 2 - 148 - 36;
		    var portraitScale = 4f;
		    
		    var portraitTexture = dialogueBox.characterDialogue.overridePortrait ?? dialogueBox.characterDialogue.speaker.Portrait;
		    var portraitSource = Game1.getSourceRectForStandardTileSheet(portraitTexture,
			    dialogueBox.characterDialogue.getPortraitIndex(), 64, 64);

			/* HD Portraits Compat */
			if(ModEntry.HdPortraitsApi != null)
            {
				var data = ModEntry.HdPortraitsApi.GetTextureAndRegion(
					dialogueBox.characterDialogue.speaker,
					dialogueBox.characterDialogue.getPortraitIndex(),
					Game1.currentGameTime.ElapsedGameTime.Milliseconds
					); //no need to force reset, HD portraits auto-resets after dialogue box close

				if (dialogueBox.characterDialogue.overridePortrait == null)
                {
					portraitTexture = data.Item2;
					portraitSource = data.Item1;
				}
            }
		    
		    if (!portraitTexture.Bounds.Contains(portraitSource)) portraitSource = new Rectangle(0, 0, 64, 64);

		    var xOffset = Utility.Utility.ShouldPortraitShake(dialogueBox, dialogueBox.characterDialogue)
			    ? Game1.random.Next(-1, 2)
			    : 0;
		    
		    /* Portrait */
			spriteBatch.Draw(portraitTexture, new Rectangle(portraitBoxX + 16 + xOffset, Game1.uiViewport.Height - 256, 256, 256), 
				portraitSource, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.88f);

		    var speakerNameX = xPositionOfPortraitArea + _widthOfPortraitArea / 2;
		    var speakerNameY = portraitBoxY + 50;

		    /* Speaker name */
		    if (ModEntry.Config.ShowSpeakerName)
		    {
			    // shadow
			    SpriteText.drawStringHorizontallyCenteredAt(spriteBatch,
				    dialogueBox.characterDialogue.speaker.getName(),
				    speakerNameX - 4, speakerNameY + 4, color: 8);
			    // actual text
			    SpriteText.drawStringHorizontallyCenteredAt(spriteBatch,
				    dialogueBox.characterDialogue.speaker.getName(),
				    speakerNameX, speakerNameY, color: 4);
		    }

		    /* Friendship jewel */
		    if (dialogueBox.shouldDrawFriendshipJewel() && ModEntry.Config.ShowFriendshipJewel)
		    {
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
			    
			    // friendship jewel tooltip
			    var hoverText = ModEntry.ModHelper.Reflection.GetField<string>(dialogueBox, "hoverText").GetValue();
			    if (hoverText.Length > 0 && ModEntry.Config.ShowFriendshipJewel)
			    {
				    SpriteText.drawStringWithScrollBackground(spriteBatch, hoverText, dialogueBox.friendshipJewel.Center.X - SpriteText.getWidthOfString(hoverText) / 2, dialogueBox.friendshipJewel.Y - 64);
			    }
		    }
	    }

	    public static bool DrawBox(DialogueBox dialogueBox, SpriteBatch spriteBatch, int xPos, int yPos, int boxWidth,
		    int boxHeight)
	    {
		    if (!dialogueBox.transitionInitialized) return true;
		    if (!dialogueBox.isPortraitBox()) return true;
		    if (dialogueBox.isQuestion) return true;

		    var viewportWidth = (int) Math.Ceiling(Game1.viewport.Width / Game1.options.uiScale * Game1.options.zoomLevel);
		    var viewportHeight = (int) Math.Ceiling(Game1.viewport.Height / Game1.options.uiScale * Game1.options.zoomLevel);
		    
		    dialogueBox.height = 250;
		    dialogueBox.y = viewportHeight - dialogueBox.height - 64;

		    var gradientBackground = Game1.currentSeason == "winter" && ModEntry.Config.DarkerBackgroundInWinter
			    ? ModEntry.DarkerGradientSample
			    : ModEntry.GradientSample;
		    
		    spriteBatch.Draw(gradientBackground, new Rectangle(0, yPos, viewportWidth, viewportHeight - yPos), Color.White);

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
			
			int textX;
			if (ModEntry.Config.ShowPortraitOnTheLeft)
			{
				textX = dialogueBox.x + _widthOfPortraitArea;
			}
			else
			{
				textX = dialogueBox.x;
			}
			
			var textY = dialogueBox.y + 58;

			var textWidth = dialogueBox.width - _widthOfPortraitArea;
				
			// shadow
			SpriteText.drawString(spriteBatch, dialogueBox.getCurrentString(), textX - 4, textY + 4, dialogueBox.characterIndexInDialogue, textWidth, color: 8);
			// actual text
			SpriteText.drawString(spriteBatch, dialogueBox.getCurrentString(), textX, textY, dialogueBox.characterIndexInDialogue, textWidth, color: 4);

			dialogueBox.drawMouse(spriteBatch);
				
			return false;

	    }
    }
}
