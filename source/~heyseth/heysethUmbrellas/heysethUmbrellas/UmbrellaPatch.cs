/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/heyseth/SDVMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Tools;

namespace heysethUmbrellas
{
    public class UmbrellaPatch
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static bool drawHoverTextPrefix(IClickableMenu __instance, SpriteBatch b, StringBuilder text, SpriteFont font, int xOffset, int yOffset, int moneyAmountToDisplayAtBottom, string boldTitleText, int healAmountToDisplay, string[] buffIconsToDisplay, ref Item hoveredItem, int currencySymbol, int extraItemToShowIndex, int extraItemToShowAmount, int overrideX, int overrideY, float alpha, CraftingRecipe craftingIngredients, IList<Item> additional_craft_materials)
        {
			try
			{
				if (hoveredItem is object)
				{
					string[] words = { };
					if (ModEntry.Config.enableWetness)
                    {
						if (ModEntry.customBestHats.Contains(hoveredItem.Name))
						{
							words = ("\n\nProvides major rain protection.").Split(' ');
						}
						else if (ModEntry.customGoodHats.Contains(hoveredItem.Name))
						{
							words = ("\n\nProvides moderate rain protection.").Split(' ');
						}
						else if (ModEntry.bestHatNames.Contains(hoveredItem.Name))
						{
							words = ("\n\nProvides major rain protection.").Split(' ');
						}
						else if (ModEntry.goodHatNames.Contains(hoveredItem.Name))
						{
							words = ("\n\nProvides moderate rain protection.").Split(' ');
						} else if (hoveredItem.Name == "Rain Coat")
                        {
							words = ("\n\nProvides slight rain protection.").Split(' ');
						}
					}
					
					if (words.Length > 0)
                    {
						// There has got to be a better way to do this
						int lengthCutoff = 0;
						lengthCutoff = text.ToString().Split('\n')[0].Length;

						text.Append("");

						string line = "";
						foreach (string word in words)
						{
							if ((line + word).Length > lengthCutoff)
							{
								text.AppendLine(line);
								line = "";
							}
							line += string.Format("{0} ", word);
						}

						if (line.Length > 0)
							text.Append(line);
					}

					if (ModEntry.umbrellaNames.Contains(hoveredItem.Name))
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(drawHoverTextPrefix)}:\n{ex}", LogLevel.Error);
			}
			return true;
        }

		//I should probably remove most of the below code as it's not neccessary for weapon tooltips, but I don't want to risk bugs
		public static void drawHoverTextPostfix(IClickableMenu __instance, SpriteBatch b, StringBuilder text, SpriteFont font, int xOffset, int yOffset, int moneyAmountToDisplayAtBottom, string boldTitleText, int healAmountToDisplay, string[] buffIconsToDisplay, Item hoveredItem, int currencySymbol, int extraItemToShowIndex, int extraItemToShowAmount, int overrideX, int overrideY, float alpha, CraftingRecipe craftingIngredients, IList<Item> additional_craft_materials)
		{
			try {
				if (hoveredItem is object)
				{
					if (ModEntry.umbrellaNames.Contains(hoveredItem.Name))
					{
						if (text == null || text.Length == 0)
						{
							return;
						}
						string bold_title_subtext = null;
						if (boldTitleText != null && boldTitleText.Length == 0)
						{
							boldTitleText = null;
						}
						int width = Math.Max((healAmountToDisplay != -1) ? ((int)font.MeasureString(healAmountToDisplay + "+ Energy" + 32).X) : 0, Math.Max((int)font.MeasureString(text).X, (boldTitleText != null) ? ((int)Game1.dialogueFont.MeasureString(boldTitleText).X) : 0)) + 32;
						int height2 = Math.Max(20 * 3, (int)font.MeasureString(text).Y + 32 + (int)((moneyAmountToDisplayAtBottom > -1) ? (font.MeasureString(string.Concat(moneyAmountToDisplayAtBottom)).Y + 4f) : 8f) + (int)((boldTitleText != null) ? (Game1.dialogueFont.MeasureString(boldTitleText).Y + 16f) : 0f));
						if (extraItemToShowIndex != -1)
						{
							string[] split = Game1.objectInformation[extraItemToShowIndex].Split('/');
							string objName = split[0];
							if (LocalizedContentManager.CurrentLanguageCode != 0)
							{
								objName = split[4];
							}
							string requirement2 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", extraItemToShowAmount, (extraItemToShowAmount > 1) ? Lexicon.makePlural(objName) : objName);
							int spriteWidth = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, extraItemToShowIndex, 16, 16).Width * 2 * 4;
							width = Math.Max(width, spriteWidth + (int)font.MeasureString(requirement2).X);
						}
						if (buffIconsToDisplay != null)
						{
							for (int k = 0; k < buffIconsToDisplay.Length; k++)
							{
								if (!buffIconsToDisplay[k].Equals("0"))
								{
									height2 += 34;
								}
							}
							height2 += 4;
						}
						if (craftingIngredients != null && Game1.options.showAdvancedCraftingInformation && craftingIngredients.getCraftCountText() != null)
						{
							height2 += (int)font.MeasureString("T").Y;
						}
						string categoryName = "Tool";
						if (hoveredItem != null)
						{
							height2 += 68 * hoveredItem.attachmentSlots();
							if (categoryName.Length > 0)
							{
								width = Math.Max(width, (int)font.MeasureString(categoryName).X + 32);
								height2 += (int)font.MeasureString("T").Y;
							}
							int maxStat = 9999;
							int buffer = 92;
							Point p = hoveredItem.getExtraSpaceNeededForTooltipSpecialIcons(font, width, buffer, height2, text, boldTitleText, moneyAmountToDisplayAtBottom);
							width = ((p.X != 0) ? p.X : width);
							height2 = ((p.Y != 0) ? p.Y : height2);
							if (hoveredItem is MeleeWeapon && (hoveredItem as MeleeWeapon).GetTotalForgeLevels() > 0)
							{
								height2 += (int)font.MeasureString("T").Y;
							}
							if (hoveredItem is MeleeWeapon && (hoveredItem as MeleeWeapon).GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
							{
								height2 += (int)font.MeasureString("T").Y;
							}
							if (buffIconsToDisplay != null)
							{
								for (int j = 0; j < buffIconsToDisplay.Length; j++)
								{
									if (!buffIconsToDisplay[j].Equals("0") && j <= 11)
									{
										width = (int)Math.Max(width, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + j, maxStat)).X + (float)buffer);
									}
								}
							}
						}
						Vector2 small_text_size = Vector2.Zero;
						if (craftingIngredients != null)
						{
							if (Game1.options.showAdvancedCraftingInformation)
							{
								int craftable_count = craftingIngredients.getCraftableCount(additional_craft_materials);
								if (craftable_count > 1)
								{
									bold_title_subtext = " (" + craftable_count + ")";
									small_text_size = Game1.smallFont.MeasureString(bold_title_subtext);
								}
							}
							width = (int)Math.Max(Game1.dialogueFont.MeasureString(boldTitleText).X + small_text_size.X + 12f, 384f);
							height2 += craftingIngredients.getDescriptionHeight(width - 8) + ((healAmountToDisplay == -1) ? (-32) : 0);
						}
						else if (bold_title_subtext != null && boldTitleText != null)
						{
							small_text_size = Game1.smallFont.MeasureString(bold_title_subtext);
							width = (int)Math.Max(width, Game1.dialogueFont.MeasureString(boldTitleText).X + small_text_size.X + 12f);
						}
						int x = Game1.getOldMouseX() + 32 + xOffset;
						int y4 = Game1.getOldMouseY() + 32 + yOffset;
						if (overrideX != -1)
						{
							x = overrideX;
						}
						if (overrideY != -1)
						{
							y4 = overrideY;
						}
						if (x + width > Utility.getSafeArea().Right)
						{
							x = Utility.getSafeArea().Right - width;
							y4 += 16;
						}
						height2 = height2 - 40; // Deal with tooltip offset
						if (y4 + height2 > Utility.getSafeArea().Bottom)
						{
							x += 16;
							if (x + width > Utility.getSafeArea().Right)
							{
								x = Utility.getSafeArea().Right - width;
							}
							y4 = Utility.getSafeArea().Bottom - height2;
						}
						StardewValley.Menus.IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y4, width + ((craftingIngredients != null) ? 21 : 0), height2, Color.White * alpha);
						if (boldTitleText != null)
						{
							Vector2 bold_text_size = Game1.dialogueFont.MeasureString(boldTitleText);
							StardewValley.Menus.IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y4, width + ((craftingIngredients != null) ? 21 : 0), (int)Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (int)((hoveredItem != null && categoryName.Length > 0) ? font.MeasureString("asd").Y : 0f) - 4, Color.White * alpha, 1f, drawShadow: false);
							b.Draw(Game1.menuTexture, new Rectangle(x + 12, y4 + (int)Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (int)((hoveredItem != null && categoryName.Length > 0) ? font.MeasureString("asd").Y : 0f) - 4, width - 4 * ((craftingIngredients != null) ? 1 : 6), 4), new Rectangle(44, 300, 4, 4), Color.White);
							b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y4 + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor);
							b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y4 + 16 + 4) + new Vector2(0f, 2f), Game1.textShadowColor);
							b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y4 + 16 + 4), Game1.textColor);
							if (bold_title_subtext != null)
							{
								Utility.drawTextWithShadow(b, bold_title_subtext, Game1.smallFont, new Vector2((float)(x + 16) + bold_text_size.X, (int)((float)(y4 + 16 + 4) + bold_text_size.Y / 2f - small_text_size.Y / 2f)), Game1.textColor);
							}
							y4 += (int)Game1.dialogueFont.MeasureString(boldTitleText).Y;
						}
						if (hoveredItem != null && categoryName.Length > 0)
						{
							y4 -= 4;
							Utility.drawTextWithShadow(b, categoryName, font, new Vector2(x + 16, y4 + 16 + 4), hoveredItem.getCategoryColor(), 1f, -1f, 2, 2);
							y4 += (int)font.MeasureString("T").Y + ((boldTitleText != null) ? 16 : 0) + 4;
							if (hoveredItem is Tool && (hoveredItem as Tool).GetTotalForgeLevels() > 0)
							{
								string forged_string2 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_Forged");
								Utility.drawTextWithShadow(b, forged_string2, font, new Vector2(x + 16, y4 + 16 + 4), Color.DarkRed, 1f, -1f, 2, 2);
								int forges = (hoveredItem as Tool).GetTotalForgeLevels();
								if (forges < (hoveredItem as Tool).GetMaxForges() && !(hoveredItem as Tool).hasEnchantmentOfType<DiamondEnchantment>())
								{
									Utility.drawTextWithShadow(b, " (" + forges + "/" + (hoveredItem as Tool).GetMaxForges() + ")", font, new Vector2((float)(x + 16) + font.MeasureString(forged_string2).X, y4 + 16 + 4), Color.DimGray, 1f, -1f, 2, 2);
								}
								y4 += (int)font.MeasureString("T").Y;
							}
							if (hoveredItem is MeleeWeapon && (hoveredItem as MeleeWeapon).GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
							{
								GalaxySoulEnchantment enchantment = (hoveredItem as MeleeWeapon).GetEnchantmentOfType<GalaxySoulEnchantment>();
								string forged_string = Game1.content.LoadString("Strings\\UI:Item_Tooltip_GalaxyForged");
								Utility.drawTextWithShadow(b, forged_string, font, new Vector2(x + 16, y4 + 16 + 4), Color.DarkRed, 1f, -1f, 2, 2);
								int level = enchantment.GetLevel();
								if (level < enchantment.GetMaximumLevel())
								{
									Utility.drawTextWithShadow(b, " (" + level + "/" + enchantment.GetMaximumLevel() + ")", font, new Vector2((float)(x + 16) + font.MeasureString(forged_string).X, y4 + 16 + 4), Color.DimGray, 1f, -1f, 2, 2);
								}
								y4 += (int)font.MeasureString("T").Y;
							}
						}
						else
						{
							y4 += ((boldTitleText != null) ? 16 : 0);
						}
						if (hoveredItem != null && craftingIngredients == null)
						{
							int descriptionWidth;
							int minimum_size = 272;
							if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
							{
								minimum_size = 384;
							}
							descriptionWidth = Math.Max(minimum_size, (int)Game1.dialogueFont.MeasureString((boldTitleText == null) ? "" : boldTitleText).X);

							// Hide the weapon tooltip
							Utility.drawTextWithShadow(b, text.ToString().Split('#')[0], font, new Vector2(x + 16, y4 + 16 + 4), Game1.textColor); ;
							//hoveredItem.drawTooltip(b, ref x, ref y4, font, alpha, text);
						}
						else if (text != null && text.Length != 0 && (text.Length != 1 || text[0] != ' '))
						{
							b.DrawString(font, text, new Vector2(x + 16, y4 + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
							b.DrawString(font, text, new Vector2(x + 16, y4 + 16 + 4) + new Vector2(0f, 2f), Game1.textShadowColor * alpha);
							b.DrawString(font, text, new Vector2(x + 16, y4 + 16 + 4) + new Vector2(2f, 0f), Game1.textShadowColor * alpha);
							b.DrawString(font, text, new Vector2(x + 16, y4 + 16 + 4), Game1.textColor * 0.9f * alpha);
							y4 += (int)font.MeasureString(text).Y + 4;
						}
						if (craftingIngredients != null)
						{
							craftingIngredients.drawRecipeDescription(b, new Vector2(x + 16, y4 - 8), width, additional_craft_materials);
							y4 += craftingIngredients.getDescriptionHeight(width - 8);
						}
						if (buffIconsToDisplay != null)
						{
							for (int i = 0; i < buffIconsToDisplay.Length; i++)
							{
								if (!buffIconsToDisplay[i].Equals("0"))
								{
									Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, y4 + 16), new Rectangle(10 + i * 10, 428, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
									string buffName = ((Convert.ToInt32(buffIconsToDisplay[i]) > 0) ? "+" : "") + buffIconsToDisplay[i] + " ";
									if (i <= 11)
									{
										buffName = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + i, buffName);
									}
									Utility.drawTextWithShadow(b, buffName, font, new Vector2(x + 16 + 34 + 4, y4 + 16), Game1.textColor);
									y4 += 34;
								}
							}
						}
						if (hoveredItem != null && hoveredItem.attachmentSlots() > 0)
						{
							hoveredItem.drawAttachments(b, x + 16, y4 + 16);
							if (moneyAmountToDisplayAtBottom > -1)
							{
								y4 += 68 * hoveredItem.attachmentSlots();
							}
						}
						y4 = y4 + 72; // Deal with tooltip offset
						if (moneyAmountToDisplayAtBottom > -1)
						{
							b.DrawString(font, string.Concat(moneyAmountToDisplayAtBottom), new Vector2(x + 16, y4 + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor);
							b.DrawString(font, string.Concat(moneyAmountToDisplayAtBottom), new Vector2(x + 16, y4 + 16 + 4) + new Vector2(0f, 2f), Game1.textShadowColor);
							b.DrawString(font, string.Concat(moneyAmountToDisplayAtBottom), new Vector2(x + 16, y4 + 16 + 4) + new Vector2(2f, 0f), Game1.textShadowColor);
							b.DrawString(font, string.Concat(moneyAmountToDisplayAtBottom), new Vector2(x + 16, y4 + 16 + 4), Game1.textColor);
							switch (currencySymbol)
							{
								case 0:
									b.Draw(Game1.debrisSpriteSheet, new Vector2((float)(x + 16) + font.MeasureString(string.Concat(moneyAmountToDisplayAtBottom)).X + 20f, y4 + 16 + 16), Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.95f);
									break;
								case 1:
									b.Draw(Game1.mouseCursors, new Vector2((float)(x + 8) + font.MeasureString(string.Concat(moneyAmountToDisplayAtBottom)).X + 20f, y4 + 16 - 5), new Rectangle(338, 400, 8, 8), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
									break;
								case 2:
									b.Draw(Game1.mouseCursors, new Vector2((float)(x + 8) + font.MeasureString(string.Concat(moneyAmountToDisplayAtBottom)).X + 20f, y4 + 16 - 7), new Rectangle(211, 373, 9, 10), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
									break;
								case 4:
									b.Draw(Game1.objectSpriteSheet, new Vector2((float)(x + 8) + font.MeasureString(string.Concat(moneyAmountToDisplayAtBottom)).X + 20f, y4 + 16 - 7), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
									break;
							}
							y4 += 48;
						}
						if (extraItemToShowIndex != -1)
						{
							if (moneyAmountToDisplayAtBottom == -1)
							{
								y4 += 8;
							}
							string displayName = Game1.objectInformation[extraItemToShowIndex].Split('/')[4];
							string requirement = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", extraItemToShowAmount, displayName);
							float minimum_box_height = Math.Max(font.MeasureString(requirement).Y + 21f, 96f);
							StardewValley.Menus.IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y4 + 4, width + ((craftingIngredients != null) ? 21 : 0), (int)minimum_box_height, Color.White);
							y4 += 20;
							b.DrawString(font, requirement, new Vector2(x + 16, y4 + 4) + new Vector2(2f, 2f), Game1.textShadowColor);
							b.DrawString(font, requirement, new Vector2(x + 16, y4 + 4) + new Vector2(0f, 2f), Game1.textShadowColor);
							b.DrawString(font, requirement, new Vector2(x + 16, y4 + 4) + new Vector2(2f, 0f), Game1.textShadowColor);
							b.DrawString(Game1.smallFont, requirement, new Vector2(x + 16, y4 + 4), Game1.textColor);
							b.Draw(Game1.objectSpriteSheet, new Vector2(x + 16 + (int)font.MeasureString(requirement).X + 21, y4), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, extraItemToShowIndex, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
						}
						if (craftingIngredients != null && Game1.options.showAdvancedCraftingInformation)
						{
							Utility.drawTextWithShadow(b, craftingIngredients.getCraftCountText(), font, new Vector2(x + 16, y4 + 16 + 4), Game1.textColor, 1f, -1f, 2, 2);
							y4 += (int)font.MeasureString("T").Y + 4;
						}
					}
				}
			} catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(drawHoverTextPostfix)}:\n{ex}", LogLevel.Error);
			}
		}
    }
}