/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Text;
using ItemExtensions.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Enchantments;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ItemExtensions.Patches;

public partial class ShopMenuPatches
{
    private static (Item, List<ExtraTrade>) _lastHoverItem;
    
    // ReSharper disable once UnusedParameter.Global
    internal static void Post_drawHoverText(IClickableMenu __instance, 
        SpriteBatch b, 
        StringBuilder text, 
        SpriteFont font, 
        int xOffset = 0, 
        int yOffset = 0, 
        int moneyAmountToDisplayAtBottom = -1, 
        string boldTitleText = null, 
        int healAmountToDisplay = -1, 
        string[] buffIconsToDisplay = null, 
        Item hoveredItem = null, 
        int currencySymbol = 0, 
        string extraItemToShowIndex = null, 
        int extraItemToShowAmount = -1, 
        int overrideX = -1, 
        int overrideY = -1, 
        float alpha = 1f, 
        CraftingRecipe craftingIngredients = null, 
        // ReSharper disable once InconsistentNaming
        IList<Item> additional_craft_materials = null, 
        Texture2D boxTexture = null, 
        Rectangle? boxSourceRect = null, 
        Color? textColor = null, 
        Color? textShadowColor = null, 
        float boxScale = 1f, 
        int boxWidthOverride = -1, 
        int boxHeightOverride = -1) 
    { 
        if(Game1.activeClickableMenu is not ShopMenu) 
            return;
        
        if(extraItemToShowIndex == null) 
            return;
        
        //if no salable data
        if(ExtraBySalable is not { Count: > 0 })
            return;
        
        List<ExtraTrade> data;
        if (_lastHoverItem is (null,null) || _lastHoverItem.Item1 != hoveredItem)
        {
            if(!InDictionary(hoveredItem, out data))
                return;

            _lastHoverItem = (hoveredItem, data);
        }
        else
        {
            data = _lastHoverItem.Item2;
        }
      
        boxTexture ??= Game1.menuTexture;
        boxSourceRect ??= new Rectangle(0, 256, 60, 60);
        ref var local1 = ref textColor;
        var nullable = textColor;
        var color1 = nullable ?? Game1.textColor;
        local1 = color1;
        ref var local2 = ref textShadowColor;
        nullable = textShadowColor;
        var color2 = nullable ?? Game1.textShadowColor;
        local2 = color2;
        
        if (text == null || text.Length == 0) 
            return;
        
        var text1 = (string) null;
        
        if (boldTitleText is { Length: 0 }) 
            boldTitleText = null;
        
        int index1;
        int val1;
        
        if (healAmountToDisplay == -1) 
        { 
            val1 = 0;
        }
        else
        {
            var spriteFont = font;
            var str1 = healAmountToDisplay.ToString();
            index1 = 32;
            var str2 = index1.ToString();
            var text2 = str1 + "+ Energy" + str2;
            val1 = (int) spriteFont.MeasureString(text2).X;
        }
        
        var val2 = Math.Max((int) font.MeasureString(text).X, boldTitleText != null ? (int) Game1.dialogueFont.MeasureString(boldTitleText).X : 0);
        var num1 = Math.Max(val1, val2) + 32;
        var num2 = Math.Max(20 * 3, (int) font.MeasureString(text).Y + 32 + (moneyAmountToDisplayAtBottom > -1 ? (int) (font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").Y + 4.0) : 8) + (boldTitleText != null ? (int) (Game1.dialogueFont.MeasureString(boldTitleText).Y + 16.0) : 0));
        
        if (extraItemToShowIndex != null)
        {
            var dataOrErrorItem = ItemRegistry.GetDataOrErrorItem("(O)" + extraItemToShowIndex);
            var displayName = dataOrErrorItem.DisplayName;
            var sourceRect = dataOrErrorItem.GetSourceRect();
            var text3 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", extraItemToShowAmount,
                extraItemToShowAmount > 1 ? Lexicon.makePlural(displayName) : displayName);
            var num3 = sourceRect.Width * 2 * 4;
            num1 = Math.Max(num1, num3 + (int) font.MeasureString(text3).X);
        }
        if (buffIconsToDisplay != null)
        {
            var strArray = buffIconsToDisplay;
            for (index1 = 0; index1 < strArray.Length; ++index1)
            {
              var str = strArray[index1];
              if (!str.Equals("0") && str != "")
                num2 += 34;
            }
            num2 += 4;
        }

        if (craftingIngredients != null && Game1.options.showAdvancedCraftingInformation &&
            craftingIngredients.getCraftCountText() != null)
        {
            num2 += (int) font.MeasureString("T").Y;
        }
        
        var text4 = (string) null;
        if (hoveredItem != null)
        {
            var startingHeight = num2 + 68 * hoveredItem.attachmentSlots();
            text4 = hoveredItem.getCategoryName();
            if (text4.Length > 0)
            {
              num1 = Math.Max(num1, (int) font.MeasureString(text4).X + 32);
              startingHeight += (int) font.MeasureString("T").Y;
            }
            var sub1 = 9999;
            var horizontalBuffer = 92;
            var tooltipSpecialIcons = hoveredItem.getExtraSpaceNeededForTooltipSpecialIcons(font, num1, horizontalBuffer, startingHeight, text, boldTitleText, moneyAmountToDisplayAtBottom);
            num1 = tooltipSpecialIcons.X != 0 ? tooltipSpecialIcons.X : num1;
            num2 = tooltipSpecialIcons.Y != 0 ? tooltipSpecialIcons.Y : startingHeight;
            
            switch (hoveredItem)
            {
                case MeleeWeapon meleeWeapon:
                {
                    if (meleeWeapon.GetTotalForgeLevels() > 0)
                        num2 += (int) font.MeasureString("T").Y;
                    if (meleeWeapon.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
                        num2 += (int) font.MeasureString("T").Y;
                    break;
                }
                case Object @object when @object.Edibility != -300:
                {
                    if (healAmountToDisplay != -1)
                        num2 += 40 * (healAmountToDisplay > 0 ? 2 : 1);
                    else
                        num2 += 40;
                
                    healAmountToDisplay = @object.staminaRecoveredOnConsumption();
                    num1 = (int) Math.Max(num1, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Energy", sub1)).X + horizontalBuffer, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Health", sub1)).X + horizontalBuffer));
                    break;
                }
            }

            if (buffIconsToDisplay != null)
            {
              for (var index2 = 0; index2 < buffIconsToDisplay.Length; ++index2)
              { 
                  if (!buffIconsToDisplay[index2].Equals("0") && index2 <= 11) 
                      num1 = (int) Math.Max(num1, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + index2, sub1)).X + horizontalBuffer);
              }
            }
        }
        var vector21 = Vector2.Zero;
        if (craftingIngredients != null)
        {
            if (Game1.options.showAdvancedCraftingInformation)
            {
                var craftableCount = craftingIngredients.getCraftableCount(additional_craft_materials);
                if (craftableCount > 1) 
                { 
                    text1 = " (" + craftableCount + ")"; 
                    vector21 = Game1.smallFont.MeasureString(text1);
                }
            }
            num1 = (int) Math.Max((float) (Game1.dialogueFont.MeasureString(boldTitleText).X + (double) vector21.X + 12.0), 384f);
            num2 += craftingIngredients.getDescriptionHeight(num1 - 8) + (healAmountToDisplay == -1 ? -32 : 0);
            
            if (craftingIngredients != null && hoveredItem != null && hoveredItem.getDescription().Equals(text.ToString())) 
                num2 -= (int) font.MeasureString(text.ToString()).Y;
        }
        else if (text1 != null && boldTitleText != null)
        {
            vector21 = Game1.smallFont.MeasureString(text1);
            num1 = (int) Math.Max(num1, (float) (Game1.dialogueFont.MeasureString(boldTitleText).X + (double) vector21.X + 12.0));
        }
        var x = Game1.getOldMouseX() + 32 + xOffset;
        var y1 = Game1.getOldMouseY() + 32 + yOffset;
        if (overrideX != -1)
            x = overrideX;
        if (overrideY != -1)
            y1 = overrideY;
        var num4 = x + num1;
        var safeArea = Utility.getSafeArea();
        var right1 = safeArea.Right;
        if (num4 > right1)
        {
            safeArea = Utility.getSafeArea();
            x = safeArea.Right - num1;
            y1 += 16;
        }
        var num5 = y1 + num2;
        safeArea = Utility.getSafeArea();
        var bottom = safeArea.Bottom;
        if (num5 > bottom)
        {
            x += 16;
            var num6 = x + num1;
            safeArea = Utility.getSafeArea();
            var right2 = safeArea.Right;
            if (num6 > right2)
            { 
                safeArea = Utility.getSafeArea(); 
                x = safeArea.Right - num1;
            }
            safeArea = Utility.getSafeArea();
            y1 = safeArea.Bottom - num2;
        }
        var width = boxWidthOverride != -1 ? boxWidthOverride : num1 + (craftingIngredients != null ? 21 : 0);
        var height1 = boxHeightOverride != -1 ? boxHeightOverride : num2;
        IClickableMenu.drawTextureBox(b, boxTexture, boxSourceRect.Value, x, y1, width, height1, Color.White * alpha, boxScale);
        if (boldTitleText != null)
        {
            var vector22 = Game1.dialogueFont.MeasureString(boldTitleText);
            IClickableMenu.drawTextureBox(b, boxTexture, boxSourceRect.Value, x, y1, num1 + (craftingIngredients != null ? 21 : 0), (int) Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (hoveredItem == null || text4.Length <= 0 ? 0 : (int) font.MeasureString("asd").Y) - 4, Color.White * alpha, drawShadow: false);
            b.Draw(Game1.menuTexture, new Rectangle(x + 12, y1 + (int) Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (hoveredItem == null || text4.Length <= 0 ? 0 : (int) font.MeasureString("asd").Y) - 4, num1 - 4 * (craftingIngredients == null ? 6 : 1), 4), new Rectangle(44, 300, 4, 4), Color.White);
            b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y1 + 16 + 4) + new Vector2(2f, 2f), textShadowColor.Value);
            b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y1 + 16 + 4) + new Vector2(0.0f, 2f), textShadowColor.Value);
            b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y1 + 16 + 4), textColor.Value);
            
            if (text1 != null) 
                Utility.drawTextWithShadow(b, text1, Game1.smallFont, new Vector2(x + 16 + vector22.X, (int) (y1 + 16 + 4 + vector22.Y / 2.0 - vector21.Y / 2.0)), Game1.textColor);
            
            y1 += (int) Game1.dialogueFont.MeasureString(boldTitleText).Y;
        }
        int y2;
        if (hoveredItem != null && text4.Length > 0)
        {
            var num7 = y1 - 4;
            Utility.drawTextWithShadow(b, text4, font, new Vector2(x + 16, num7 + 16 + 4), hoveredItem.getCategoryColor(), horizontalShadowOffset: 2, verticalShadowOffset: 2);
            y2 = num7 + ((int) font.MeasureString("T").Y + (boldTitleText != null ? 16 : 0) + 4);
            if (hoveredItem is Tool tool && tool.GetTotalForgeLevels() > 0)
            { 
                var text5 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_Forged"); 
                Utility.drawTextWithShadow(b, text5, font, new Vector2(x + 16, y2 + 16 + 4), Color.DarkRed, horizontalShadowOffset: 2, verticalShadowOffset: 2); 
                var totalForgeLevels = tool.GetTotalForgeLevels(); 
                if (totalForgeLevels < tool.GetMaxForges() && !tool.hasEnchantmentOfType<DiamondEnchantment>()) 
                {
                    var b1 = b;
                    var strArray = new[]
                    {
                        " (",
                        totalForgeLevels.ToString(),
                        "/",
                        null,
                        null
                    };
                    index1 = tool.GetMaxForges();
                    strArray[3] = index1.ToString();
                    strArray[4] = ")";
                    var text6 = string.Concat(strArray);
                    var font1 = font;
                    var position = new Vector2(x + 16 + font.MeasureString(text5).X, y2 + 16 + 4);
                    var dimGray = Color.DimGray;
                    Utility.drawTextWithShadow(b1, text6, font1, position, dimGray, horizontalShadowOffset: 2, verticalShadowOffset: 2); 
                } 
                y2 += (int) font.MeasureString("T").Y;
            }
            if (hoveredItem is MeleeWeapon meleeWeapon && meleeWeapon.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
            { 
                var enchantmentOfType = meleeWeapon.GetEnchantmentOfType<GalaxySoulEnchantment>();
                var text7 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_GalaxyForged");
                Utility.drawTextWithShadow(b, text7, font, new Vector2(x + 16, y2 + 16 + 4), Color.DarkRed, horizontalShadowOffset: 2, verticalShadowOffset: 2);
                var level = enchantmentOfType.GetLevel();
                if (level < enchantmentOfType.GetMaximumLevel())
                {
                    var b2 = b;
                    var strArray = new[]
                    {
                        " (",
                        level.ToString(),
                        "/",
                        null,
                        null
                    };
                    index1 = enchantmentOfType.GetMaximumLevel();
                    strArray[3] = index1.ToString();
                    strArray[4] = ")";
                    var text8 = string.Concat(strArray);
                    var font2 = font;
                    var position = new Vector2(x + 16 + font.MeasureString(text7).X, y2 + 16 + 4);
                    var dimGray = Color.DimGray;
                    Utility.drawTextWithShadow(b2, text8, font2, position, dimGray, horizontalShadowOffset: 2, verticalShadowOffset: 2);
                }
                y2 += (int) font.MeasureString("T").Y;
            }
        }
        else
        {
            y2 = y1 + (boldTitleText != null ? 16 : 0);
        }

        if (hoveredItem != null && craftingIngredients == null)
        {
            hoveredItem.drawTooltip(b, ref x, ref y2, font, alpha, text);
        }
        
        else if (text != null && text.Length != 0 && (text.Length != 1 || text[0] != ' ') && (craftingIngredients == null || hoveredItem == null || !hoveredItem.getDescription().Equals(text.ToString())))
        {
            b.DrawString(font, text, new Vector2(x + 16, y2 + 16 + 4) + new Vector2(2f, 2f), textShadowColor.Value * alpha);
            b.DrawString(font, text, new Vector2(x + 16, y2 + 16 + 4) + new Vector2(0.0f, 2f), textShadowColor.Value * alpha);
            b.DrawString(font, text, new Vector2(x + 16, y2 + 16 + 4) + new Vector2(2f, 0.0f), textShadowColor.Value * alpha);
            b.DrawString(font, text, new Vector2(x + 16, y2 + 16 + 4), textColor.Value * 0.9f * alpha);
            y2 += (int) font.MeasureString(text).Y + 4;
        }
        if (craftingIngredients != null)
        {
            craftingIngredients.drawRecipeDescription(b, new Vector2(x + 16, y2 - 8), num1, additional_craft_materials);
            y2 += craftingIngredients.getDescriptionHeight(num1 - 8);
        }
        if (healAmountToDisplay != -1)
        {
            var num8 = (hoveredItem as Object).staminaRecoveredOnConsumption();
            if (num8 >= 0)
            { 
                var num9 = (hoveredItem as Object).healthRecoveredOnConsumption();
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, y2 + 16), new Rectangle(num8 < 0 ? 140 : 0, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f); 
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", (num8 > 0 ? "+" : "") + num8), font, new Vector2(x + 16 + 34 + 4, y2 + 16), Game1.textColor); 
                y2 += 34; 
                if (num9 > 0)
                {
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, y2 + 16), new Rectangle(0, 438, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Health", (num9 > 0 ? "+" : "") + num9), font, new Vector2(x + 16 + 34 + 4, y2 + 16), Game1.textColor);
                    y2 += 34;
                }
            }
            else if (num8 != -300)
            { 
                Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, y2 + 16), new Rectangle(140, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f); 
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", num8.ToString() ?? ""), font, new Vector2(x + 16 + 34 + 4, y2 + 16), Game1.textColor); 
                y2 += 34;
            }
        }
        if (buffIconsToDisplay != null)
        {
            for (var index3 = 0; index3 < buffIconsToDisplay.Length; ++index3)
            {
                if (buffIconsToDisplay[index3].Equals("0") || buffIconsToDisplay[index3] == "") 
                    continue;
                
                Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, y2 + 16), new Rectangle(10 + index3 * 10, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
                var str = (Convert.ToInt32(buffIconsToDisplay[index3]) > 0 ? "+" : "") + buffIconsToDisplay[index3] + " ";
                
                if (index3 <= 11)
                {
                    str = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + index3, str);
                }
                
                Utility.drawTextWithShadow(b, str, font, new Vector2(x + 16 + 34 + 4, y2 + 16), Game1.textColor);
                y2 += 34;
            }
        }
        if (hoveredItem != null && hoveredItem.attachmentSlots() > 0)
        {
            hoveredItem.drawAttachments(b, x + 16, y2 + 16);
            if (moneyAmountToDisplayAtBottom > -1)
            {
                y2 += 68 * hoveredItem.attachmentSlots();
            }
        }
        if (moneyAmountToDisplayAtBottom > -1)
        {
            var text9 = moneyAmountToDisplayAtBottom.ToString();
            b.DrawString(font, text9, new Vector2(x + 16, y2 + 16 + 4) + new Vector2(2f, 2f), textShadowColor.Value);
            b.DrawString(font, text9, new Vector2(x + 16, y2 + 16 + 4) + new Vector2(0.0f, 2f), textShadowColor.Value);
            b.DrawString(font, text9, new Vector2(x + 16, y2 + 16 + 4) + new Vector2(2f, 0.0f), textShadowColor.Value);
            b.DrawString(font, text9, new Vector2(x + 16, y2 + 16 + 4), textColor.Value);
            switch (currencySymbol)
            { 
                case 0: 
                    b.Draw(Game1.debrisSpriteSheet, new Vector2((float) (x + 16 + (double) font.MeasureString(text9).X + 20.0), y2 + 16 + 16), Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.95f); 
                    break; 
                case 1: 
                    b.Draw(Game1.mouseCursors, new Vector2((float) (x + 8 + (double) font.MeasureString(text9).X + 20.0), y2 + 16 - 5), new Rectangle(338, 400, 8, 8), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f); 
                    break; 
                case 2: 
                    b.Draw(Game1.mouseCursors, new Vector2((float) (x + 8 + (double) font.MeasureString(text9).X + 20.0), y2 + 16 - 7), new Rectangle(211, 373, 9, 10), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f); 
                    break; 
                case 4: 
                    b.Draw(Game1.objectSpriteSheet, new Vector2((float) (x + 8 + (double) font.MeasureString(text9).X + 20.0), y2 + 16 - 7), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f); 
                    break;
            }
            y2 += 48;
        }
        if (extraItemToShowIndex != null)
        {
            if (moneyAmountToDisplayAtBottom == -1)
            {
                y2 += 8;
            }
            
            var dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(extraItemToShowIndex);
            var displayName = dataOrErrorItem.DisplayName;
            var texture = dataOrErrorItem.GetTexture();
            var sourceRect = dataOrErrorItem.GetSourceRect();
            var requirements = GetRequiredItems(extraItemToShowAmount, displayName, data);
            
            //Log($"text: {text}");
            
            var text10 = GiveSameWidth(requirements, text.ToString()); //Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", extraItemToShowAmount, displayName);
            var height2 = Math.Max(font.MeasureString(text10).Y + 21f, 96f);
            var position = new Vector2(x + 16 + (int)font.MeasureString(text10).X + 21, y2);
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y2 + 4, num1 + (craftingIngredients != null ? 21 : 0), (int) height2, Color.White);
            y2 += 20;
            b.DrawString(font, text10, new Vector2(x + 16, y2 + 4) + new Vector2(2f, 2f), textShadowColor.Value);
            b.DrawString(font, text10, new Vector2(x + 16, y2 + 4) + new Vector2(0.0f, 2f), textShadowColor.Value);
            b.DrawString(font, text10, new Vector2(x + 16, y2 + 4) + new Vector2(2f, 0.0f), textShadowColor.Value);
            b.DrawString(Game1.smallFont, text10, new Vector2(x + 16, y2 + 4), textColor.Value);
            
            //draw items
            var p = new Vector2(position.X - 32, position.Y);
            b.Draw(texture, p, sourceRect, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            DrawExtraItems(data, b, p);
        }
        
        if (craftingIngredients == null || !Game1.options.showAdvancedCraftingInformation)
            return;
        
        Utility.drawTextWithShadow(b, craftingIngredients.getCraftCountText(), font, new Vector2(x + 16, y2 + 16 + 4), Game1.textColor, horizontalShadowOffset: 2, verticalShadowOffset: 2);
        // ReSharper disable once RedundantAssignment
        y2 += (int) font.MeasureString("T").Y + 4;
    }

    private static string GiveSameWidth(string requirements, string text)
    {
        var raw = requirements.Replace('\n', ' ');
        var rawSplit = raw.Split(' ');

        var split = text.Split('\n');
        var maxPerLine = split[0].Length;
        foreach (var separation in split)
        {
            if (separation.Length > maxPerLine)
                maxPerLine = separation.Length;
        }
        
        string fixedString = null;
        var track = 0;
        
        foreach (var word in rawSplit)
        {
            fixedString += word;
            fixedString += ' ';
            track += word.Length + 1;

            if (track >= maxPerLine)
            {
                fixedString += '\n';
                track = 0;
            }
        }

        return fixedString;
    }

    private static void DrawExtraItems(List<ExtraTrade> data, SpriteBatch spriteBatch, Vector2 position)
    {
        var fixedPosition = position; 
        fixedPosition.X = position.X;
        
        foreach (var item in data)
        {
            fixedPosition.X += 50;
            if (fixedPosition.X > position.X + 50)
            {
                fixedPosition.X = position.X;
                fixedPosition.Y += 50;
            }
            spriteBatch.Draw(item.Data.GetTexture(), fixedPosition, item.Data.GetSourceRect(), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }
    }

    private static string GetRequiredItems(int extraItemToShowAmount, string displayName, List<ExtraTrade> data)
    { 
        var baseItems = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", extraItemToShowAmount, extraItemToShowAmount > 1 ? Lexicon.makePlural(displayName) : displayName); 
        var all = baseItems.Remove(baseItems.Length - 1, 1);
        var forExtraItems = ModEntry.Help.Translation.Get("ItemHover_Requirements_Extra");
        
        foreach (var item in data)
        {
            all += ModEntry.Comma;
            
            var toAdd = string.Format(
                forExtraItems, 
                item.Count, 
                item.Count > 1 ? Lexicon.makePlural(item.Data.DisplayName) : item.Data.DisplayName
                );

            if (item.Count == 1 && IsFrenchOrSpanish())
                toAdd = $"{item.Count} {item.Data.DisplayName}";
            
            all += $"{toAdd}";
        }
        //all += '.';
        //Log($"Full msg: {all}");
      
        return all;
    }

    private static bool IsFrenchOrSpanish()
    {
        var spanish = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es;
        var french = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr;

        return spanish || french;
    }
}