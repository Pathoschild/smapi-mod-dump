using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Linq;

namespace StardewConfigMenu {
	public class Utilities {

		// Adds newlines into string
		// Useful for splitting long descriptions in hoverTextDictionary
		public static string GetWordWrappedString(string input, int charCount = 40) {

			// early out if les than 1 line length
			if (input.Length <= charCount) {
				return string.Copy(input);
			}

			string output = string.Empty;
			var count = 0;

			foreach (char ch in input) {
				if ((count > charCount && (ch == ' ' || ch == '\t')) || ch == '\n') {
					output += '\n';
					count = 0;
				} else if (ch == '\t') {
					output += ' '; // remove tabs, might mess stuff up
					count++;
				} else {
					output += ch;
					count++;
				}
			}

			return output;
		}

		public static void drawHoverTextWithoutShadow(SpriteBatch b, string text, SpriteFont font, int xOffset = 0, int yOffset = 0, int moneyAmountToDisplayAtBottom = -1, string boldTitleText = null, int healAmountToDisplay = -1, string[] buffIconsToDisplay = null, Item hoveredItem = null, int currencySymbol = 0, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, int overrideX = -1, int overrideY = -1, float alpha = 1f, CraftingRecipe craftingIngredients = null) {
			if (text == null || text.Length == 0) {
				return;
			}
			if (boldTitleText != null && boldTitleText.Length == 0) {
				boldTitleText = null;
			}
			int num = 20;
			int num2 = Math.Max((healAmountToDisplay == -1) ? 0 : ((int) font.MeasureString(healAmountToDisplay + "+ Energy" + Game1.tileSize / 2).X), Math.Max((int) font.MeasureString(text).X, (boldTitleText == null) ? 0 : ((int) Game1.dialogueFont.MeasureString(boldTitleText).X))) + Game1.tileSize / 2;
			int num3 = Math.Max(num * 3, (int) font.MeasureString(text).Y + Game1.tileSize / 2 + (int) ((moneyAmountToDisplayAtBottom <= -1) ? 0f : (font.MeasureString(moneyAmountToDisplayAtBottom + string.Empty).Y + 4f)) + (int) ((boldTitleText == null) ? 0f : (Game1.dialogueFont.MeasureString(boldTitleText).Y + (float) (Game1.tileSize / 4))) + ((healAmountToDisplay == -1) ? 0 : 38));
			if (extraItemToShowIndex != -1) {
				string[] array = Game1.objectInformation[extraItemToShowIndex].Split(new char[] {
			'/'
		});
				string text2 = array[0];
				if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en) {
					text2 = array[array.Length - 1];
				}
				string text3 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", new object[] {
			extraItemToShowAmount,
			text2
		});
				int num4 = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, extraItemToShowIndex, 16, 16).Width * 2 * Game1.pixelZoom;
				num2 = Math.Max(num2, num4 + (int) font.MeasureString(text3).X);
			}
			if (buffIconsToDisplay != null) {
				for (int i = 0; i < buffIconsToDisplay.Length; i++) {
					string text4 = buffIconsToDisplay[i];
					if (!text4.Equals("0")) {
						num3 += 34;
					}
				}
				num3 += 4;
			}
			string text5 = null;
			if (hoveredItem != null) {
				num3 += (Game1.tileSize + 4) * hoveredItem.attachmentSlots();
				text5 = hoveredItem.getCategoryName();
				if (text5.Length > 0) {
					num2 = Math.Max(num2, (int) font.MeasureString(text5).X + Game1.tileSize / 2);
					num3 += (int) font.MeasureString("T").Y;
				}
				int num5 = 9999;
				int num6 = 15 * Game1.pixelZoom + Game1.tileSize / 2;
				if (hoveredItem is MeleeWeapon) {
					MeleeWeapon meleeWeapon = hoveredItem as MeleeWeapon;
					num3 = Math.Max(num * 3, (int) ((boldTitleText == null) ? 0f : (Game1.dialogueFont.MeasureString(boldTitleText).Y + (float) (Game1.tileSize / 4))) + Game1.tileSize / 2) + (int) font.MeasureString("T").Y + (int) ((moneyAmountToDisplayAtBottom <= -1) ? 0f : (font.MeasureString(moneyAmountToDisplayAtBottom + string.Empty).Y + 4f));
					num3 += ((!(hoveredItem.Name == "Scythe")) ? ((hoveredItem as MeleeWeapon).getNumberOfDescriptionCategories() * Game1.pixelZoom * 12) : 0);
					num3 += (int) font.MeasureString(Game1.parseText((hoveredItem as MeleeWeapon).description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4)).Y;
					num2 = (int) Math.Max((float) num2, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Damage", new object[] {
				num5,
				num5
			})).X + (float) num6, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Speed", new object[] {
				num5
			})).X + (float) num6, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", new object[] {
				num5
			})).X + (float) num6, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", new object[] {
				num5
			})).X + (float) num6, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", new object[] {
				num5
			})).X + (float) num6, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Weight", new object[] {
				num5
			})).X + (float) num6))))));
				} else if (hoveredItem is Boots) {
					num3 -= (int) font.MeasureString(text).Y;
					num3 += (int) ((float) ((hoveredItem as Boots).getNumberOfDescriptionCategories() * Game1.pixelZoom * 12) + font.MeasureString(Game1.parseText((hoveredItem as Boots).description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4)).Y);
					num2 = (int) Math.Max((float) num2, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", new object[] {
				num5
			})).X + (float) num6, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", new object[] {
				num5
			})).X + (float) num6));
				} else if (hoveredItem is StardewValley.Object && (hoveredItem as StardewValley.Object).edibility != -300) {
					if (healAmountToDisplay == -1) {
						num3 += (Game1.tileSize / 2 + Game1.pixelZoom * 2) * ((healAmountToDisplay <= 0) ? 1 : 2);
					} else {
						num3 += Game1.tileSize / 2 + Game1.pixelZoom * 2;
					}
					healAmountToDisplay = (int) Math.Ceiling((double) (hoveredItem as StardewValley.Object).Edibility * 2.5) + (hoveredItem as StardewValley.Object).quality * (hoveredItem as StardewValley.Object).Edibility;
					num2 = (int) Math.Max((float) num2, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Energy", new object[] {
				num5
			})).X + (float) num6, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Health", new object[] {
				num5
			})).X + (float) num6));
				}
				if (buffIconsToDisplay != null) {
					for (int j = 0; j < buffIconsToDisplay.Length; j++) {
						if (!buffIconsToDisplay[j].Equals("0") && j <= 11) {
							num2 = (int) Math.Max((float) num2, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + j, new object[] {
						num5
					})).X + (float) num6);
						}
					}
				}
			}
			if (craftingIngredients != null) {
				num2 = Math.Max((int) Game1.dialogueFont.MeasureString(boldTitleText).X + Game1.pixelZoom * 3, Game1.tileSize * 6);
				num3 += craftingIngredients.getDescriptionHeight(num2 - Game1.pixelZoom * 2) + ((healAmountToDisplay != -1) ? 0 : (-Game1.tileSize / 2)) + Game1.pixelZoom * 3;
			}
			if (hoveredItem is FishingRod && moneyAmountToDisplayAtBottom > -1) {
				num3 += (int) font.MeasureString("T").Y;
			}
			int num7 = Game1.getOldMouseX() + Game1.tileSize / 2 + xOffset;
			int num8 = Game1.getOldMouseY() + Game1.tileSize / 2 + yOffset;
			if (overrideX != -1) {
				num7 = overrideX;
			}
			if (overrideY != -1) {
				num8 = overrideY;
			}
			if (num7 + num2 > Utility.getSafeArea().Right) {
				num7 = Utility.getSafeArea().Right - num2;
				num8 += Game1.tileSize / 4;
			}
			if (num8 + num3 > Utility.getSafeArea().Bottom) {
				num7 += Game1.tileSize / 4;
				if (num7 + num2 > Utility.getSafeArea().Right) {
					num7 = Utility.getSafeArea().Right - num2;
				}
				num8 = Utility.getSafeArea().Bottom - num3;
			}
			IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), num7, num8, num2 + ((craftingIngredients == null) ? 0 : (Game1.tileSize / 3)), num3, Color.White * alpha, 1f, false);
			if (boldTitleText != null) {
				IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), num7, num8, num2 + ((craftingIngredients == null) ? 0 : (Game1.tileSize / 3)), (int) Game1.dialogueFont.MeasureString(boldTitleText).Y + Game1.tileSize / 2 + (int) ((hoveredItem == null || text5.Length <= 0) ? 0f : font.MeasureString("asd").Y) - Game1.pixelZoom, Color.White * alpha, 1f, false);
				b.Draw(Game1.menuTexture, new Rectangle(num7 + Game1.pixelZoom * 3, num8 + (int) Game1.dialogueFont.MeasureString(boldTitleText).Y + Game1.tileSize / 2 + (int) ((hoveredItem == null || text5.Length <= 0) ? 0f : font.MeasureString("asd").Y) - Game1.pixelZoom, num2 - Game1.pixelZoom * ((craftingIngredients != null) ? 1 : 6), Game1.pixelZoom), new Rectangle?(new Rectangle(44, 300, 4, 4)), Color.White);
				b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)) + new Vector2(2f, 2f), Game1.textShadowColor);
				b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)) + new Vector2(0f, 2f), Game1.textShadowColor);
				b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)), Game1.textColor);
				num8 += (int) Game1.dialogueFont.MeasureString(boldTitleText).Y;
			}
			if (hoveredItem != null && text5.Length > 0) {
				num8 -= 4;
				Utility.drawTextWithShadow(b, text5, font, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)), hoveredItem.getCategoryColor(), 1f, -1f, 2, 2, 1f, 3);
				num8 += (int) font.MeasureString("T").Y + ((boldTitleText == null) ? 0 : (Game1.tileSize / 4)) + Game1.pixelZoom;
			} else {
				num8 += ((boldTitleText == null) ? 0 : (Game1.tileSize / 4));
			}
			if (hoveredItem != null && hoveredItem is Boots) {
				Boots boots = hoveredItem as Boots;
				Utility.drawTextWithShadow(b, Game1.parseText(boots.description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4), font, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
				num8 += (int) font.MeasureString(Game1.parseText(boots.description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4)).Y;
				if (boots.defenseBonus > 0) {
					Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4 + 4)), new Rectangle(110, 428, 10, 10), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, false, 1f, -1, -1, 0.35f);
					Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", new object[] {
				boots.defenseBonus
			}), font, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom * 13), (float) (num8 + Game1.tileSize / 4 + Game1.pixelZoom * 3)), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3);
					num8 += (int) Math.Max(font.MeasureString("TT").Y, (float) (12 * Game1.pixelZoom));
				}
				if (boots.immunityBonus > 0) {
					Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4 + 4)), new Rectangle(150, 428, 10, 10), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, false, 1f, -1, -1, 0.35f);
					Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", new object[] {
				boots.immunityBonus
			}), font, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom * 13), (float) (num8 + Game1.tileSize / 4 + Game1.pixelZoom * 3)), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3);
					num8 += (int) Math.Max(font.MeasureString("TT").Y, (float) (12 * Game1.pixelZoom));
				}
			} else if (hoveredItem != null && hoveredItem is MeleeWeapon) {
				MeleeWeapon meleeWeapon2 = hoveredItem as MeleeWeapon;
				Utility.drawTextWithShadow(b, Game1.parseText(meleeWeapon2.description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4), font, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
				num8 += (int) font.MeasureString(Game1.parseText(meleeWeapon2.description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4)).Y;
				if (meleeWeapon2.indexOfMenuItemView != 47) {
					Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4 + 4)), new Rectangle(120, 428, 10, 10), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, false, 1f, -1, -1, 0.35f);
					Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Damage", new object[] {
				meleeWeapon2.minDamage,
				meleeWeapon2.maxDamage
			}), font, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom * 13), (float) (num8 + Game1.tileSize / 4 + Game1.pixelZoom * 3)), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3);
					num8 += (int) Math.Max(font.MeasureString("TT").Y, (float) (12 * Game1.pixelZoom));
					if (meleeWeapon2.speed != ((meleeWeapon2.type != 2) ? 0 : -8)) {
						Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4 + 4)), new Rectangle(130, 428, 10, 10), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, false, 1f, -1, -1, 0.35f);
						bool flag = (meleeWeapon2.type == 2 && meleeWeapon2.speed < -8) || (meleeWeapon2.type != 2 && meleeWeapon2.speed < 0);
						Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Speed", new object[] {
					((((meleeWeapon2.type != 2) ? meleeWeapon2.speed : (meleeWeapon2.speed - -8)) <= 0) ? string.Empty : "+") + ((meleeWeapon2.type != 2) ? meleeWeapon2.speed : (meleeWeapon2.speed - -8)) / 2
				}), font, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom * 13), (float) (num8 + Game1.tileSize / 4 + Game1.pixelZoom * 3)), (!flag) ? (Game1.textColor * 0.9f * alpha) : Color.DarkRed, 1f, -1f, -1, -1, 1f, 3);
						num8 += (int) Math.Max(font.MeasureString("TT").Y, (float) (12 * Game1.pixelZoom));
					}
					if (meleeWeapon2.addedDefense > 0) {
						Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4 + 4)), new Rectangle(110, 428, 10, 10), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, false, 1f, -1, -1, 0.35f);
						Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", new object[] {
					meleeWeapon2.addedDefense
				}), font, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom * 13), (float) (num8 + Game1.tileSize / 4 + Game1.pixelZoom * 3)), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3);
						num8 += (int) Math.Max(font.MeasureString("TT").Y, (float) (12 * Game1.pixelZoom));
					}
					if ((double) meleeWeapon2.critChance / 0.02 >= 2.0) {
						Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4 + 4)), new Rectangle(40, 428, 10, 10), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, false, 1f, -1, -1, 0.35f);
						Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", new object[] {
					(int)((double)meleeWeapon2.critChance / 0.02)
				}), font, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom * 13), (float) (num8 + Game1.tileSize / 4 + Game1.pixelZoom * 3)), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3);
						num8 += (int) Math.Max(font.MeasureString("TT").Y, (float) (12 * Game1.pixelZoom));
					}
					if ((double) (meleeWeapon2.critMultiplier - 3f) / 0.02 >= 1.0) {
						Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)), new Rectangle(160, 428, 10, 10), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, false, 1f, -1, -1, 0.35f);
						Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", new object[] {
					(int)((double)(meleeWeapon2.critMultiplier - 3f) / 0.02)
				}), font, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom * 11), (float) (num8 + Game1.tileSize / 4 + Game1.pixelZoom * 3)), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3);
						num8 += (int) Math.Max(font.MeasureString("TT").Y, (float) (12 * Game1.pixelZoom));
					}
					if (meleeWeapon2.knockback != meleeWeapon2.defaultKnockBackForThisType(meleeWeapon2.type)) {
						Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4 + 4)), new Rectangle(70, 428, 10, 10), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, false, 1f, -1, -1, 0.35f);
						Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Weight", new object[] {
					(((float)((int)Math.Ceiling ((double)(Math.Abs (meleeWeapon2.knockback - meleeWeapon2.defaultKnockBackForThisType (meleeWeapon2.type)) * 10f))) <= meleeWeapon2.defaultKnockBackForThisType (meleeWeapon2.type)) ? string.Empty : "+") + (int)Math.Ceiling ((double)(Math.Abs (meleeWeapon2.knockback - meleeWeapon2.defaultKnockBackForThisType (meleeWeapon2.type)) * 10f))
				}), font, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom * 13), (float) (num8 + Game1.tileSize / 4 + Game1.pixelZoom * 3)), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3);
						num8 += (int) Math.Max(font.MeasureString("TT").Y, (float) (12 * Game1.pixelZoom));
					}
				}
			} else if (!string.IsNullOrEmpty(text) && text != " ") {
				b.DrawString(font, text, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
				b.DrawString(font, text, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)) + new Vector2(0f, 2f), Game1.textShadowColor * alpha);
				b.DrawString(font, text, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)) + new Vector2(2f, 0f), Game1.textShadowColor * alpha);
				b.DrawString(font, text, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)), Game1.textColor * 0.9f * alpha);
				num8 += (int) font.MeasureString(text).Y + 4;
			}
			if (craftingIngredients != null) {
				craftingIngredients.drawRecipeDescription(b, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 - Game1.pixelZoom * 2)), num2);
				num8 += craftingIngredients.getDescriptionHeight(num2);
			}
			if (healAmountToDisplay != -1) {
				if (healAmountToDisplay > 0) {
					Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4)), new Rectangle((healAmountToDisplay >= 0) ? 0 : 140, 428, 10, 10), Color.White, 0f, Vector2.Zero, 3f, false, 0.95f, -1, -1, 0.35f);
					Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", new object[] {
				((healAmountToDisplay <= 0) ? string.Empty : "+") + healAmountToDisplay
			}), font, new Vector2((float) (num7 + Game1.tileSize / 4 + 34 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4 + 8)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
					num8 += 34;
					Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4)), new Rectangle(0, 438, 10, 10), Color.White, 0f, Vector2.Zero, 3f, false, 0.95f, -1, -1, 0.35f);
					Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Health", new object[] {
				((healAmountToDisplay <= 0) ? string.Empty : "+") + (int)((float)healAmountToDisplay * 0.4f)
			}), font, new Vector2((float) (num7 + Game1.tileSize / 4 + 34 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4 + 8)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
					num8 += 34;
				} else if (healAmountToDisplay != -300) {
					Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4)), new Rectangle(140, 428, 10, 10), Color.White, 0f, Vector2.Zero, 3f, false, 0.95f, -1, -1, 0.35f);
					Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", new object[] {
				string.Empty + healAmountToDisplay
			}), font, new Vector2((float) (num7 + Game1.tileSize / 4 + 34 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4 + 8)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
					num8 += 34;
				}
			}
			if (buffIconsToDisplay != null) {
				for (int k = 0; k < buffIconsToDisplay.Length; k++) {
					if (!buffIconsToDisplay[k].Equals("0")) {
						Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 4 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4)), new Rectangle(10 + k * 10, 428, 10, 10), Color.White, 0f, Vector2.Zero, 3f, false, 0.95f, -1, -1, 0.35f);
						string text6 = ((Convert.ToInt32(buffIconsToDisplay[k]) <= 0) ? string.Empty : "+") + buffIconsToDisplay[k] + " ";
						if (k <= 11) {
							text6 = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + k, new object[] {
						text6
					});
						}
						Utility.drawTextWithShadow(b, text6, font, new Vector2((float) (num7 + Game1.tileSize / 4 + 34 + Game1.pixelZoom), (float) (num8 + Game1.tileSize / 4 + 8)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
						num8 += 34;
					}
				}
			}
			if (hoveredItem != null && hoveredItem.attachmentSlots() > 0) {
				num8 += 16;
				hoveredItem.drawAttachments(b, num7 + Game1.tileSize / 4, num8);
				if (moneyAmountToDisplayAtBottom > -1) {
					num8 += Game1.tileSize * hoveredItem.attachmentSlots();
				}
			}
			if (moneyAmountToDisplayAtBottom > -1) {
				b.DrawString(font, moneyAmountToDisplayAtBottom + string.Empty, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)) + new Vector2(2f, 2f), Game1.textShadowColor);
				b.DrawString(font, moneyAmountToDisplayAtBottom + string.Empty, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)) + new Vector2(0f, 2f), Game1.textShadowColor);
				b.DrawString(font, moneyAmountToDisplayAtBottom + string.Empty, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)) + new Vector2(2f, 0f), Game1.textShadowColor);
				b.DrawString(font, moneyAmountToDisplayAtBottom + string.Empty, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.tileSize / 4 + 4)), Game1.textColor);
				if (currencySymbol == 0) {
					b.Draw(Game1.debrisSpriteSheet, new Vector2((float) (num7 + Game1.tileSize / 4) + font.MeasureString(moneyAmountToDisplayAtBottom + string.Empty).X + 20f, (float) (num8 + Game1.tileSize / 4 + 16)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16)), Color.White, 0f, new Vector2(8f, 8f), (float) Game1.pixelZoom, SpriteEffects.None, 0.95f);
				} else if (currencySymbol == 1) {
					b.Draw(Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 8) + font.MeasureString(moneyAmountToDisplayAtBottom + string.Empty).X + 20f, (float) (num8 + Game1.tileSize / 4 - 5)), new Rectangle?(new Rectangle(338, 400, 8, 8)), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, SpriteEffects.None, 1f);
				} else if (currencySymbol == 2) {
					b.Draw(Game1.mouseCursors, new Vector2((float) (num7 + Game1.tileSize / 8) + font.MeasureString(moneyAmountToDisplayAtBottom + string.Empty).X + 20f, (float) (num8 + Game1.tileSize / 4 - 7)), new Rectangle?(new Rectangle(211, 373, 9, 10)), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, SpriteEffects.None, 1f);
				}
				num8 += Game1.tileSize * 3 / 4;
			}
			if (extraItemToShowIndex != -1) {
				IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), num7, num8 + Game1.pixelZoom, num2, Game1.tileSize * 3 / 2, Color.White, 1f, true);
				num8 += Game1.pixelZoom * 5;
				string[] array2 = Game1.objectInformation[extraItemToShowIndex].Split(new char[] {
			'/'
		});
				string text7 = array2[4];
				string text8 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", new object[] {
			extraItemToShowAmount,
			text7
		});
				b.DrawString(font, text8, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.pixelZoom)) + new Vector2(2f, 2f), Game1.textShadowColor);
				b.DrawString(font, text8, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.pixelZoom)) + new Vector2(0f, 2f), Game1.textShadowColor);
				b.DrawString(font, text8, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.pixelZoom)) + new Vector2(2f, 0f), Game1.textShadowColor);
				b.DrawString(Game1.smallFont, text8, new Vector2((float) (num7 + Game1.tileSize / 4), (float) (num8 + Game1.pixelZoom)), Game1.textColor);
				b.Draw(Game1.objectSpriteSheet, new Vector2((float) (num7 + Game1.tileSize / 4 + (int) font.MeasureString(text8).X + Game1.tileSize / 3), (float) num8), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, extraItemToShowIndex, 16, 16)), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, SpriteEffects.None, 1f);
			}
		}


	}
}
