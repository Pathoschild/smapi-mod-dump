/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/BerrySeasonReminder
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System;

namespace BerrySeasonReminder
{
	public class BillboardPatches
	{
		private const int BEARS_KNOWLEDGE_EVENT = 2120303;

		public static void performHoverAction_Postfix(ref Billboard __instance, int x, int y, ref string ___hoverText)
		{
			if (__instance.calendarDays != null && Game1.player.eventsSeen.Contains(BEARS_KNOWLEDGE_EVENT))
			{
				for (int i = 0; i < __instance.calendarDays.Count; i++)
				{
					ClickableTextureComponent c = __instance.calendarDays[i];
					if (c.bounds.Contains(x, y))
					{
						if (Game1.currentSeason.Equals("fall") && i >= 7 && i <= 10)
						{
							if (___hoverText.Length > 0)
							{
								___hoverText += Environment.NewLine;
							}
							___hoverText += "Blackberry Season";
						}
						if (Game1.currentSeason.Equals("spring") && i >= 14 && i <= 17)
						{
							if (___hoverText.Length > 0)
							{
								___hoverText += Environment.NewLine;
							}
							___hoverText += "Salmonberry Season";
						}
					}
				}
			}
		}

		public static void draw_Postfix(ref Billboard __instance, SpriteBatch b, bool ___dailyQuestBoard, ref string ___hoverText)
		{
			if (!___dailyQuestBoard)
			{
				// If this player has Bear's Knowledge, draw berry seasons on the calendar.
				if (Game1.player.eventsSeen.Contains(BEARS_KNOWLEDGE_EVENT))
				{
					if (Game1.currentSeason.Equals("fall"))
					{
						// Add a blackberry icon to days within blackberry season.  (Fall 8 - 11)
						for (int i = 7; i <= 10; i++)
						{
							Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2(__instance.calendarDays[i].bounds.X + 12,
								(float)(__instance.calendarDays[i].bounds.Y + 60) - Game1.dialogueButtonScale / 2f), new Rectangle(32, 272, 16, 16),
								Color.White, 0f, Vector2.Zero, 2f, flipped: false, 1f);
						}
					}
					else if (Game1.currentSeason.Equals("spring"))
					{
						// Add a salmonberry icon to days within salmonberry season.  (Spring 15 - 18)
						for (int i = 14; i <= 17; i++)
						{
							Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2(__instance.calendarDays[i].bounds.X + 12,
								(float)(__instance.calendarDays[i].bounds.Y + 60) - Game1.dialogueButtonScale / 2f), new Rectangle(128, 192, 16, 16),
								Color.White, 0f, Vector2.Zero, 2f, flipped: false, 1f);
						}
					}

					Game1.mouseCursorTransparency = 1f;
					__instance.drawMouse(b);
					if (___hoverText.Length > 0)
					{
						IClickableMenu.drawHoverText(b, ___hoverText, Game1.dialogueFont);
					}
				}
			}
		}
	}
}