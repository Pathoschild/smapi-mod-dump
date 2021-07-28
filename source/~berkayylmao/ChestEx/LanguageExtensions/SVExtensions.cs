/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

#region License

// 
//    ChestEx (StardewValleyMods)
//    Copyright (c) 2021 Berkay Yigit <berkaytgy@gmail.com>
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY, without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.Linq;
using System.Text;

using ChestEx.Types.BaseTypes;
using ChestEx.Types.CustomTypes.ExtendedSVObjects;

using Harmony;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;

namespace ChestEx.LanguageExtensions {
  public static class SVExtensions {
    /// <summary>
    /// Gets a <see cref="Rectangle"/> to wrap the given rectangle.
    /// </summary>
    /// <returns>A <see cref="Rectangle"/> that can be used to draw a texture box around the given rectangle.</returns>
    public static Rectangle GetTextureBoxRectangle(this Rectangle rectangle, Int32 contentPadding = 8, Single borderScale = 1.0f) {
      Int32 border_diff = Convert.ToInt32(16.0f * borderScale);
      return new Rectangle(rectangle.X - contentPadding - border_diff,
                           rectangle.Y - contentPadding - border_diff,
                           rectangle.Width + contentPadding * 2 + border_diff * 2,
                           rectangle.Height + contentPadding * 2 + border_diff * 2);
    }

    /// <summary>
    /// Gets a <see cref="Rectangle"/> to wrap the given menu.
    /// </summary>
    /// <returns>A <see cref="Rectangle"/> that can be used to draw a texture box around the given rectangle.</returns>
    public static Rectangle GetTextureBoxRectangle(this InventoryMenu menu, Int32 contentPadding = 8, Single borderScale = 1.0f) {
      Int32 border_diff = Convert.ToInt32(16.0f * borderScale);

      var   slots      = menu.GetSlotDrawPositions();
      Point first_slot = slots.First().AsXNAPoint();
      Point last_slot  = slots.Last().AsXNAPoint();

      return new Rectangle(first_slot.X - contentPadding - border_diff,
                           first_slot.Y - contentPadding - border_diff,
                           last_slot.X + 64 - first_slot.X + contentPadding * 2 + border_diff * 2,
                           last_slot.Y + 64 - first_slot.Y + contentPadding * 2 + border_diff * 2);
    }

    /// <summary>
    /// Gets the bounds of the menu.
    /// </summary>
    /// <returns>A <see cref="Rectangle"/> that encompasses the menu.</returns>
    public static Rectangle GetBounds(this IClickableMenu menu) { return new(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height); }

    /// <summary>
    /// Sets the menu's bounds.
    /// </summary>
    public static void SetBounds(this IClickableMenu menu, Rectangle bounds) {
      menu.xPositionOnScreen = bounds.X;
      menu.yPositionOnScreen = bounds.Y;
      menu.width             = bounds.Width;
      menu.height            = bounds.Height;
    }

    /// <summary>
    /// Sets the menu's bounds.
    /// </summary>
    public static void SetBounds(this IClickableMenu menu, Int32 x, Int32 y, Int32 width,
                                 Int32               height) {
      menu.xPositionOnScreen = x;
      menu.yPositionOnScreen = y;
      menu.width             = width;
      menu.height            = height;
    }

    /// <summary>
    /// Sets the given menu's and its clickable components' visibiliy to '<paramref name="isVisible"/>'.
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="isVisible">Whether this menu should be visible.</param>
    public static void SetVisibleEx(this InventoryMenu menu, Boolean isVisible) { menu.inventory?.ForEach(cc => cc.visible = isVisible); }

    public static ExtendedChest.ChestType GetChestType(this Chest chest) {
      return chest.fridge.Value       ? ExtendedChest.ChestType.Fridge :
        chest.ParentSheetIndex == 130 ? ExtendedChest.ChestType.WoodenChest :
        chest.ParentSheetIndex == 232 ? ExtendedChest.ChestType.StoneChest : ExtendedChest.ChestType.None;
    }

    public static Color GetActualColour(this Chest chest) {
      Color col = chest.playerChoiceColor.Value;
      if (col == Color.Black)
        col = chest.GetChestType() == ExtendedChest.ChestType.WoodenChest ? Color.FromNonPremultiplied(206, 120, 41, 255) : Color.FromNonPremultiplied(207, 191, 179, 255);

      return col;
    }

    public static void DrawTextureBox(this SpriteBatch spriteBatch,       Rectangle bounds, Colours colours, Boolean drawGradient = false,
                                      Boolean          drawShadow = true, Single    borderScale = 1.0f) {
      if (colours == null) return;

      Texture2D texture                        = Game1.uncoloredMenuTexture;
      if (colours == Colours.gDefault) texture = Game1.menuTexture;

      void draw(Colours boxColours, Int32 padding = 0) {
        Single  centre_scale         = 64.0f * borderScale;
        Single  centre_scale_half    = centre_scale / 2.0f;
        Single  centre_scale_quarter = centre_scale / 4.0f;
        Single  bounds_diff          = centre_scale + centre_scale_half;
        Vector2 calc_pos             = new(bounds.X + centre_scale_quarter - padding, bounds.Y + centre_scale_quarter + padding);
        // TL corner
        spriteBatch.Draw(texture,
                         calc_pos,
                         new Rectangle(0, 0, 64, 64),
                         boxColours.mBorderColour,
                         0.0f,
                         new Vector2(32, 32),
                         borderScale,
                         SpriteEffects.None,
                         0.5f);
        // TR corner
        spriteBatch.Draw(texture,
                         calc_pos + new Vector2(bounds.Width - centre_scale_half, 0.0f),
                         new Rectangle(0, 0, 64, 64),
                         boxColours.mBorderColour,
                         0.0f,
                         new Vector2(32, 32),
                         borderScale,
                         SpriteEffects.FlipHorizontally,
                         0.5f);
        // BL corner
        spriteBatch.Draw(texture,
                         calc_pos + new Vector2(0.0f, bounds.Height - centre_scale_half),
                         new Rectangle(0, 192, 64, 64),
                         boxColours.mBorderColour,
                         0.0f,
                         new Vector2(32, 32),
                         borderScale,
                         SpriteEffects.None,
                         0.5f);
        // BR corner
        spriteBatch.Draw(texture,
                         calc_pos + new Vector2(bounds.Width - centre_scale_half, bounds.Height - centre_scale_half),
                         new Rectangle(0, 192, 64, 64),
                         boxColours.mBorderColour,
                         0.0f,
                         new Vector2(32, 32),
                         borderScale,
                         SpriteEffects.FlipHorizontally,
                         0.5f);
        // Top border
        spriteBatch.Draw(texture,
                         calc_pos + new Vector2(centre_scale / 2.0f, 0.0f),
                         new Rectangle(128, 0, 64, 64),
                         boxColours.mBorderColour,
                         0.0f,
                         new Vector2(0, 32),
                         new Vector2(Math.Max(0.0f, bounds.Width - bounds_diff) / 64.0f, borderScale),
                         SpriteEffects.None,
                         0.5f);
        // Bottom border
        spriteBatch.Draw(texture,
                         calc_pos + new Vector2(centre_scale / 2.0f, bounds.Height - centre_scale_half),
                         new Rectangle(128, 192, 64, 64),
                         boxColours.mBorderColour,
                         0.0f,
                         new Vector2(0, 32),
                         new Vector2(Math.Max(0.0f, bounds.Width - bounds_diff) / 64.0f, borderScale),
                         SpriteEffects.None,
                         0.5f);
        // Left border
        spriteBatch.Draw(texture,
                         calc_pos - new Vector2(centre_scale / 2.0f, centre_scale / -2.0f),
                         new Rectangle(0, 128, 64, 64),
                         boxColours.mBorderColour,
                         0.0f,
                         Vector2.Zero,
                         new Vector2(borderScale, Math.Max(0.0f, bounds.Height - bounds_diff) / 64.0f),
                         SpriteEffects.None,
                         0.5f);
        // Right border
        spriteBatch.Draw(texture,
                         calc_pos + new Vector2(bounds.Width - centre_scale, centre_scale / 2.0f),
                         new Rectangle(0, 128, 64, 64),
                         boxColours.mBorderColour,
                         0.0f,
                         Vector2.Zero,
                         new Vector2(borderScale, Math.Max(0.0f, bounds.Height - bounds_diff) / 64.0f),
                         SpriteEffects.FlipHorizontally,
                         0.5f);
        // Fill
        if (drawGradient)
          spriteBatch.Draw(TexturePresets.gVerticalGradient,
                           new Rectangle((Int32)calc_pos.X, (Int32)calc_pos.Y, (Int32)(bounds.Width - centre_scale_half), (Int32)(bounds.Height - centre_scale_half)),
                           boxColours.mBackgroundColour);
        else
          spriteBatch.Draw(texture,
                           new Rectangle((Int32)calc_pos.X, (Int32)calc_pos.Y, (Int32)(bounds.Width - centre_scale_half), (Int32)(bounds.Height - centre_scale_half)),
                           new Rectangle(64, 128, 64, 64),
                           boxColours.mBackgroundColour);
      }

      if (drawShadow) draw(Colours.gDarkShadow, 8);
      draw(colours);
    }

    public static void DrawEx(this InventoryMenu menu, SpriteBatch spriteBatch, Color slotBorderColour) {
      for (Int32 l = 0; l < menu.capacity; l++) {
        spriteBatch.Draw(TexturePresets.gMenuTextureGrayScale,
                         new Vector2(menu.xPositionOnScreen + l % (menu.capacity / menu.rows) * 64 + menu.horizontalGap * (l % (menu.capacity / menu.rows)),
                                     menu.yPositionOnScreen + l / (menu.capacity / menu.rows) * (64 + menu.verticalGap) + (l / (menu.capacity / menu.rows) - 1) * 4),
                         Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10),
                         slotBorderColour,
                         0.0f,
                         Vector2.Zero,
                         1.0f,
                         SpriteEffects.None,
                         0.5f);
      }
      menu.draw(spriteBatch, slotBorderColour.R, slotBorderColour.G, slotBorderColour.B);
    }

    public static void DrawHoverText(this SpriteBatch spriteBatch,           SpriteFont font,           String  text,              String title = "",
                                     Point            textPadding = default, Colours    colours = null, Boolean drawShadow = true, Single alpha = 1.0f,
                                     Single           borderScale = 1.0f) {
      if (String.IsNullOrWhiteSpace(text)) return;
      if (textPadding == default) textPadding = new Point(4, 2);
      colours ??= Colours.GenerateFromMenuTiles();
      colours =   colours.MultAlpha(alpha);

      Point  mouse_pos   = Game1.getMousePosition();
      Point  text_size   = font.MeasureString(text).AsXNAPoint();
      Point  title_size  = String.IsNullOrWhiteSpace(title) ? Point.Zero : Game1.dialogueFont.MeasureString(title).AsXNAPoint();
      Single border_diff = 16.0f * borderScale;
      // calc box target
      Rectangle box_rect = new((Int32)(mouse_pos.X + font.GetSize().X * (1.25f + borderScale)),
                               (Int32)(mouse_pos.Y + font.GetSize().Y * (0.5f + borderScale)),
                               Math.Max(text_size.X, title_size.X) + textPadding.X * 2 + (Int32)(border_diff * 2.0f),
                               title_size.Y + text_size.Y + textPadding.Y + (Int32)(border_diff * 2.0f));
      // clamp box to game viewport
      Rectangle safe_area                                = Utility.getSafeArea();
      if (box_rect.Right > safe_area.Right) box_rect.X   = safe_area.Right - box_rect.Width;
      if (box_rect.Bottom > safe_area.Bottom) box_rect.Y = safe_area.Bottom - box_rect.Height;
      // calc text target
      Vector2 text_pos = new(box_rect.X + textPadding.X + border_diff, box_rect.Y + textPadding.Y + border_diff);
      // draw box
      spriteBatch.DrawTextureBox(box_rect, colours, false, drawShadow, borderScale);
      // draw text
      if (!String.IsNullOrWhiteSpace(title))
        spriteBatch.DrawString(Game1.dialogueFont,
                               title,
                               text_pos,
                               colours.mForegroundColour,
                               0f,
                               Vector2.Zero,
                               1.0f,
                               SpriteEffects.None,
                               0.989f);
      spriteBatch.DrawString(font,
                             text,
                             new Vector2(text_pos.X, text_pos.Y + title_size.Y),
                             colours.mForegroundColour,
                             0f,
                             Vector2.Zero,
                             1.0f,
                             SpriteEffects.None,
                             0.999f);
    }

    public static void DrawHoverText(this SpriteBatch spriteBatch, SpriteFont font, StringBuilder text, Int32 xOffset = 0,
                                     Int32 yOffset = 0, Int32 moneyAmountToDisplayAtBottom = -1, String boldTitleText = null, Int32 healAmountToDisplay = -1,
                                     String[] buffIconsToDisplay = null, Item hoveredItem = null, Int32 currencySymbol = IClickableMenu.currency_g, Int32 contentPadding = 4,
                                     Colours colours = null, Single alpha = 1.0f, Single borderScale = 1.0f) {
      if (text == null || text.Length == 0) return;
      colours ??= Colours.GenerateFromMenuTiles();
      // Calculate negative colour (of the bg colour) to display coloured text
      Color negative_bg_colour;
      {
        colours.mBackgroundColour.AsDotNetColor().ToHSV(out Double hue, out Double sat, out Double val);
        hue = (hue + 180.0d) % 360.0d;
        if (colours.mForegroundColour == Color.Black) {
          sat = Math.Min(1.0d,
                         sat
                         * (sat < 0.01d ? 150.0d :
                           sat < 0.1d   ? 50.0d : 5.0d));
          if (val > 0.0d) val /= 2.375d * val;
        }
        else {
          val = Math.Min(1.0d,
                         val
                         * (val < 0.01d ? 150.0d :
                           val < 0.1d   ? 50.0d : 5.0d));
          if (sat > 0.0d) sat /= 2.5d * sat;
        }

        negative_bg_colour = DotNetExtensions.ColourFromHSV(hue, sat, val).AsXNAColor();
      }
      // Calculate shadow colour (from the bg colour)
      Color text_shadow_colour = colours.mBackgroundColour.MultRGB(colours.mForegroundColour == Color.White ? 0.3f : 0.7f);
      // Calc padding zones
      Single border_diff = 16.0f * borderScale;
      Int32  font_y_diff = Math.Max(font.GetSize().Y, 52);
      Int32  buffer      = contentPadding + (Int32)border_diff;

      String category_name                                   = null;
      String bold_title_subtext                              = null;
      if (String.IsNullOrEmpty(boldTitleText)) boldTitleText = null;
      String  money_text                                     = Convert.ToString(moneyAmountToDisplayAtBottom);
      Vector2 money_text_size                                = font.MeasureString(money_text);

      Int32 width = Math.Max(healAmountToDisplay != -1 ? (Int32)font.MeasureString(healAmountToDisplay + "+ Energy" + 32).X : 0,
                             Math.Max((Int32)font.MeasureString(text).X, boldTitleText != null ? (Int32)Game1.dialogueFont.MeasureString(boldTitleText).X : 0));
      Int32 height = (boldTitleText != null ? Game1.dialogueFont.GetSize().Y + 4 : 0)
                     + (moneyAmountToDisplayAtBottom > -1 ? (Int32)money_text_size.Y + 32 : 8)
                     + (hoveredItem is Tool && hoveredItem.attachmentSlots() > 0 ? buffer : 0)
                     + (buffIconsToDisplay?.Where(buffIcon => !buffIcon.Equals("0")).Sum(_ => 52) ?? 0);

      if (hoveredItem != null) {
        height += 68 * hoveredItem.attachmentSlots();
        // Category
        category_name = hoveredItem.getCategoryName();
        if (category_name.Length > 0) {
          width  =  Math.Max(width, (Int32)font.MeasureString(category_name).X + 32);
          height += font.GetSize().Y;
        }
        // Buffs
        if (buffIconsToDisplay != null) {
          for (Int32 i = 0; i < buffIconsToDisplay.Length; i++)
            if (!buffIconsToDisplay[i].Equals("0"))
              width = (Int32)Math.Max(width, font.MeasureString(Game1.content.LoadString($"Strings\\UI:ItemHover_Buff{i}", 9999)).X + buffer);
        }
        // Specific
        switch (hoveredItem) {
          case Boots boots: {
            // Description
            Int32  desc_width = Traverse.Create(boots).Method("getDescriptionWidth").GetValue<Int32>();
            String desc_text  = Game1.parseText(boots.description, Game1.smallFont, desc_width);
            height += (Int32)font.MeasureString(desc_text).Y;
            // Stat effects
            {
              // Categories
              height += boots.getNumberOfDescriptionCategories() * font_y_diff + contentPadding;
              // Defense
              if (boots.defenseBonus > 0) width = Math.Max(width, (Int32)font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", 9999, 9999)).X + buffer);
              // Immunity
              if (boots.immunityBonus > 0)
                width = Math.Max(width, (Int32)font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", 9999, 9999)).X + buffer);
            }

            break;
          }
          case Ring ring: {
            // Description
            Int32  desc_width = Traverse.Create(ring).Method("getDescriptionWidth").GetValue<Int32>();
            String desc_text  = Game1.parseText(ring.description, Game1.smallFont, desc_width);
            height += (Int32)font.MeasureString(desc_text).Y;
            // Stat effects
            {
              Int32 num_effects = 0;
              // Defense
              if (ring.GetsEffectOfRing(810)) {
                width = Math.Max(width, (Int32)font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", 9999)).X + buffer);
                num_effects++;
              }
              // Immunity
              if (ring.GetsEffectOfRing(887)) num_effects++;
              // Luck
              if (ring.GetsEffectOfRing(859)) num_effects++;

              height += num_effects * font_y_diff;
            }

            break;
          }
          case MeleeWeapon weapon: {
            // Description
            Int32  desc_width = Traverse.Create(weapon).Method("getDescriptionWidth").GetValue<Int32>();
            String desc_text  = Game1.parseText(weapon.description, Game1.smallFont, desc_width);
            height += (Int32)font.MeasureString(desc_text).Y;
            // Stat effects
            {
              if (!weapon.isScythe(weapon.IndexOfMenuItemView)) {
                height += weapon.getNumberOfDescriptionCategories() * font_y_diff + contentPadding;
                // Damage
                width = Math.Max(width, (Int32)font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Damage", 9999, 9999)).X + buffer);
                // Speed
                if (weapon.speed.Value != (weapon.type.Value == MeleeWeapon.club ? -8 : 0))
                  width = Math.Max(width, (Int32)font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Speed", 9999)).X + buffer);
                // Defense
                if (weapon.addedDefense.Value > 0)
                  width = Math.Max(width, (Int32)font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", 9999)).X + buffer);
                // Crit chance
                {
                  Single effective_crit_chance = weapon.critChance.Value;
                  if (weapon.type.Value == MeleeWeapon.dagger) {
                    effective_crit_chance += 0.005f;
                    effective_crit_chance *= 1.12f;
                  }

                  if (effective_crit_chance / 0.02f >= 1.1f)
                    width = Math.Max(width, (Int32)font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", 9999)).X + buffer);
                }
                // Crit multiplier
                if ((weapon.critMultiplier.Value - 3f) / 0.02f >= 1.0)
                  width = Math.Max(width, (Int32)font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", 9999)).X + buffer);
                // Knockback
                if (!weapon.knockback.Value.NearlyEquals(weapon.defaultKnockBackForThisType(weapon.type.Value)))
                  width = Math.Max(width, (Int32)font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Weight", 9999)).X + buffer);
              }
            }
            // Forges
            if (weapon.GetTotalForgeLevels() > 0) height += font.GetSize().Y + 4;
            // Enchantments
            height += weapon.enchantments.Where(enchantment => enchantment.ShouldBeDisplayed()).Sum(_ => font_y_diff);
            // Diamond enchantment (randoms)
            if (weapon.enchantments.LastOrDefault() is DiamondEnchantment)
              width = Math.Max(width, (Int32)font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural", weapon.GetMaxForges())).X);
            // Galaxy enchantment
            if (weapon.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0) height += 52;

            break;
          }
          case Tool tool: {
            // Description
            Int32  desc_width = Traverse.Create(tool).Method("getDescriptionWidth").GetValue<Int32>();
            String desc_text  = Game1.parseText(tool.description, Game1.smallFont, desc_width);
            height += (Int32)font.MeasureString(desc_text).Y;
            // Enchantments
            height += tool.enchantments.Where(enchantment => enchantment.ShouldBeDisplayed()).Sum(_ => font_y_diff);

            break;
          }
          case StardewValley.Object obj when obj.edibility != -300: {
            healAmountToDisplay =  obj.staminaRecoveredOnConsumption();
            height              += (Int32)font.MeasureString(text).Y;
            height              += healAmountToDisplay != -1 ? font_y_diff * (healAmountToDisplay > 0 ? 2 : 1) : 52;
            width = (Int32)Math.Max(width,
                                    Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Energy", 9999)).X + buffer,
                                             font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Health", 9999)).X + buffer));

            break;
          }
          default: {
            height += (Int32)font.MeasureString(text).Y;
            break;
          }
        }
      }

      Vector2 small_text_size = Vector2.Zero;
      if (bold_title_subtext is not null && boldTitleText is not null) {
        small_text_size = Game1.smallFont.MeasureString(bold_title_subtext);
        width           = (Int32)Math.Max(width, Game1.dialogueFont.MeasureString(boldTitleText).X + small_text_size.X + 12.0f);
      }

      Point     mouse_pos = Game1.getMousePosition();
      Rectangle safe_area = Utility.getSafeArea();
      // Get pos
      Int32 x = mouse_pos.X + 48 + xOffset;
      Int32 y = mouse_pos.Y + 48 + yOffset;
      // Pad width/height
      width  += contentPadding * 2;
      height += contentPadding * 2;
      Rectangle box_rect = new Rectangle(x, y, width, height).GetTextureBoxRectangle(0);
      // Clamp
      if (box_rect.Right > safe_area.Right) x   = safe_area.Right - width;
      if (box_rect.Bottom > safe_area.Bottom) y = safe_area.Bottom - height;
      box_rect = new Rectangle(x, y, width, height).GetTextureBoxRectangle(0);
      // Draw background
      spriteBatch.DrawTextureBox(box_rect, colours, true, borderScale: 0.5f);
      // Pad
      x += contentPadding;
      y += contentPadding;
      // Title (+ category, forge/enchantments, divider)
      if (boldTitleText is not null) {
        spriteBatch.DrawStringEx(Game1.dialogueFont, boldTitleText, new Vector2(x - 2, y - 2), colours.mForegroundColour, drawShadow: true, textShadowColour: text_shadow_colour);

        Vector2 bold_text_size = Game1.dialogueFont.MeasureString(boldTitleText);
        if (!String.IsNullOrEmpty(bold_title_subtext)) {
          spriteBatch.DrawStringEx(font,
                                   bold_title_subtext,
                                   new Vector2(x + bold_text_size.X, y + bold_text_size.Y / 2f - small_text_size.Y / 2f),
                                   colours.mForegroundColour,
                                   drawShadow: true,
                                   textShadowColour: text_shadow_colour);
        }

        y += Game1.dialogueFont.GetSize().Y;

        if (hoveredItem is not null && category_name.Length > 0) {
          spriteBatch.DrawStringEx(font, category_name, new Vector2(x, y), colours.mForegroundColour, drawShadow: true, textShadowColour: text_shadow_colour);
          y += font.GetSize().Y + 4;

          if (hoveredItem is Tool tool && tool.GetTotalForgeLevels() > 0) {
            String forged_string = $"> {Game1.content.LoadString("Strings\\UI:Item_Tooltip_Forged")}";
            spriteBatch.DrawStringEx(font, forged_string, new Vector2(x + 2, y), negative_bg_colour, drawShadow: true, textShadowColour: text_shadow_colour);
            Int32 forges = tool.GetTotalForgeLevels();

            if (forges < tool.GetMaxForges() && !tool.hasEnchantmentOfType<DiamondEnchantment>()) {
              spriteBatch.DrawStringEx(font,
                                       $@" ({forges}/{tool.GetMaxForges()})",
                                       new Vector2(x + font.MeasureString(forged_string).X, y),
                                       negative_bg_colour,
                                       drawShadow: true,
                                       textShadowColour: text_shadow_colour);
            }

            y += font.GetSize().Y + 4;
          }

          if (hoveredItem is MeleeWeapon weapon && weapon.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0) {
            var    enchantment   = weapon.GetEnchantmentOfType<GalaxySoulEnchantment>();
            String forged_string = $"> {Game1.content.LoadString("Strings\\UI:Item_Tooltip_GalaxyForged")}";
            spriteBatch.DrawStringEx(font, forged_string, new Vector2(x + 2, y), negative_bg_colour, drawShadow: true, textShadowColour: text_shadow_colour);
            Int32 level = enchantment.GetLevel();

            if (level < enchantment.GetMaximumLevel()) {
              spriteBatch.DrawStringEx(font,
                                       $@" ({level}/{enchantment.GetMaximumLevel()})",
                                       new Vector2(x + font.MeasureString(forged_string).X, y),
                                       negative_bg_colour,
                                       drawShadow: true,
                                       textShadowColour: text_shadow_colour);
            }

            y += font.GetSize().Y + 4;
          }
        }

        // Divider
        spriteBatch.Draw(Game1.uncoloredMenuTexture,
                         new Rectangle(x - (Int32)(4.0f * borderScale) - (Int32)border_diff - contentPadding,
                                       y,
                                       width + (Int32)(8.0f * borderScale) + (Int32)(border_diff * 2.0f),
                                       (Int32)border_diff),
                         new Rectangle(108, 224, 1, 16),
                         colours.mBorderColour);

        y += 20;
      }

      // Stat effects (+ description)
      if (!String.IsNullOrWhiteSpace(text.ToString())) {
        switch (hoveredItem) {
          case Boots boots: {
            Int32  desc_width = Traverse.Create(boots).Method("getDescriptionWidth").GetValue<Int32>();
            String desc_text  = Game1.parseText(boots.description, Game1.smallFont, desc_width);

            spriteBatch.DrawStringEx(font, desc_text, new Vector2(x, y), colours.mForegroundColour, drawShadow: true, textShadowColour: text_shadow_colour);
            y += (Int32)font.MeasureString(desc_text).Y + 4;

            // Defense
            {
              if (boots.defenseBonus > 0) {
                Utility.drawWithShadow(spriteBatch,
                                       Game1.mouseCursors,
                                       new Vector2(x, y),
                                       new Rectangle(110, 428, 10, 10),
                                       Color.White,
                                       0.0f,
                                       Vector2.Zero,
                                       4.0f,
                                       false,
                                       1.0f,
                                       shadowIntensity: 0.2f);
                spriteBatch.DrawStringEx(font,
                                         Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", boots.defenseBonus),
                                         new Vector2(x + 48, y + 8),
                                         colours.mForegroundColour,
                                         drawShadow: true,
                                         textShadowColour: text_shadow_colour);
                y += font_y_diff;
              }
            }

            // Immunity
            {
              if (boots.immunityBonus > 0) {
                Utility.drawWithShadow(spriteBatch,
                                       Game1.mouseCursors,
                                       new Vector2(x, y),
                                       new Rectangle(150, 428, 10, 10),
                                       Color.White,
                                       0.0f,
                                       Vector2.Zero,
                                       4.0f,
                                       false,
                                       1.0f,
                                       shadowIntensity: 0.2f);
                spriteBatch.DrawStringEx(font,
                                         Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", boots.immunityBonus),
                                         new Vector2(x + 48, y + 8),
                                         colours.mForegroundColour,
                                         drawShadow: true,
                                         textShadowColour: text_shadow_colour);
                y += font_y_diff;
              }
            }

            break;
          }
          case Ring ring: {
            Int32  desc_width = Traverse.Create(ring).Method("getDescriptionWidth").GetValue<Int32>();
            String desc_text  = Game1.parseText(ring.description, Game1.smallFont, desc_width);

            spriteBatch.DrawStringEx(font, desc_text, new Vector2(x, y), colours.mForegroundColour, drawShadow: true, textShadowColour: text_shadow_colour);
            y += (Int32)font.MeasureString(desc_text).Y + 4;

            // Defense
            if (ring.GetsEffectOfRing(810)) {
              Utility.drawWithShadow(spriteBatch,
                                     Game1.mouseCursors,
                                     new Vector2(x, y),
                                     new Rectangle(110, 428, 10, 10),
                                     Color.White,
                                     0.0f,
                                     Vector2.Zero,
                                     4.0f,
                                     false,
                                     1.0f,
                                     shadowIntensity: 0.2f);
              spriteBatch.DrawStringEx(font,
                                       Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", 5 * ring.GetEffectsOfRingMultiplier(810)),
                                       new Vector2(x + 48, y + 8),
                                       colours.mForegroundColour,
                                       drawShadow: true,
                                       textShadowColour: text_shadow_colour);
              y += font_y_diff;
            }

            // Immunity
            if (ring.GetsEffectOfRing(887)) {
              Utility.drawWithShadow(spriteBatch,
                                     Game1.mouseCursors,
                                     new Vector2(x, y),
                                     new Rectangle(150, 428, 10, 10),
                                     Color.White,
                                     0.0f,
                                     Vector2.Zero,
                                     4.0f,
                                     false,
                                     1.0f,
                                     shadowIntensity: 0.2f);
              spriteBatch.DrawStringEx(font,
                                       Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", 4 * ring.GetEffectsOfRingMultiplier(887)),
                                       new Vector2(x + 48, y + 8),
                                       colours.mForegroundColour,
                                       drawShadow: true,
                                       textShadowColour: text_shadow_colour);
              y += font_y_diff;
            }

            // Luck
            if (ring.GetsEffectOfRing(859)) {
              Utility.drawWithShadow(spriteBatch,
                                     Game1.mouseCursors,
                                     new Vector2(x, y),
                                     new Rectangle(50, 428, 10, 10),
                                     Color.White,
                                     0.0f,
                                     Vector2.Zero,
                                     4.0f,
                                     false,
                                     1.0f,
                                     shadowIntensity: 0.2f);
              spriteBatch.DrawStringEx(font,
                                       $"+{Game1.content.LoadString("Strings\\UI:ItemHover_Buff4", ring.GetEffectsOfRingMultiplier(859))}",
                                       new Vector2(x + 48, y + 8),
                                       colours.mForegroundColour,
                                       drawShadow: true,
                                       textShadowColour: text_shadow_colour);
              y += font_y_diff;
            }

            break;
          }
          case MeleeWeapon weapon: {
            // Description
            {
              Int32  desc_width = Traverse.Create(weapon).Method("getDescriptionWidth").GetValue<Int32>();
              String desc_text  = Game1.parseText(weapon.description, Game1.smallFont, desc_width);
              spriteBatch.DrawStringEx(font, desc_text, new Vector2(x, y), colours.mForegroundColour, drawShadow: true, textShadowColour: text_shadow_colour);
              y += (Int32)font.MeasureString(desc_text).Y + 4;
            }
            if (!weapon.isScythe(weapon.IndexOfMenuItemView)) {
              // Damage
              {
                String desc_text = Game1.content.LoadString("Strings\\UI:ItemHover_Damage", weapon.minDamage.Value, weapon.maxDamage.Value);

                Utility.drawWithShadow(spriteBatch,
                                       Game1.mouseCursors,
                                       new Vector2(x, y),
                                       new Rectangle(120, 428, 10, 10),
                                       Color.White,
                                       0.0f,
                                       Vector2.Zero,
                                       4.0f,
                                       false,
                                       1.0f,
                                       shadowIntensity: 0.2f);
                spriteBatch.DrawStringEx(font,
                                         desc_text,
                                         new Vector2(x + 48, y + 8),
                                         weapon.hasEnchantmentOfType<RubyEnchantment>() ? negative_bg_colour : colours.mForegroundColour,
                                         drawShadow: true,
                                         textShadowColour: text_shadow_colour);
                y += font_y_diff;
              }

              // Speed
              {
                if (weapon.speed.Value != (weapon.type.Value == MeleeWeapon.club ? -8 : 0)) {
                  Int32  weapon_real_speed = weapon.speed.Value + (weapon.type.Value == MeleeWeapon.club ? 8 : 0);
                  String desc_text         = Game1.content.LoadString("Strings\\UI:ItemHover_Speed", $"{(weapon_real_speed > 0 ? "+" : "")}{weapon_real_speed / 2}");

                  Utility.drawWithShadow(spriteBatch,
                                         Game1.mouseCursors,
                                         new Vector2(x, y),
                                         new Rectangle(130, 428, 10, 10),
                                         Color.White,
                                         0.0f,
                                         Vector2.Zero,
                                         4.0f,
                                         false,
                                         1.0f,
                                         shadowIntensity: 0.2f);
                  spriteBatch.DrawStringEx(font,
                                           desc_text,
                                           new Vector2(x + 48, y + 8),
                                           weapon.hasEnchantmentOfType<EmeraldEnchantment>() ? negative_bg_colour : colours.mForegroundColour,
                                           drawShadow: true,
                                           textShadowColour: text_shadow_colour);
                  y += font_y_diff;
                }
              }

              // Defense
              {
                if (weapon.addedDefense.Value > 0) {
                  String desc_text = Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", weapon.addedDefense.Value);

                  Utility.drawWithShadow(spriteBatch,
                                         Game1.mouseCursors,
                                         new Vector2(x, y),
                                         new Rectangle(110, 428, 10, 10),
                                         Color.White,
                                         0.0f,
                                         Vector2.Zero,
                                         4.0f,
                                         false,
                                         1.0f,
                                         shadowIntensity: 0.2f);
                  spriteBatch.DrawStringEx(font,
                                           desc_text,
                                           new Vector2(x + 48, y + 8),
                                           weapon.hasEnchantmentOfType<TopazEnchantment>() ? negative_bg_colour : colours.mForegroundColour,
                                           drawShadow: true,
                                           textShadowColour: text_shadow_colour);
                  y += font_y_diff;
                }
              }

              // Crit chance
              {
                Single effective_crit_chance = weapon.critChance.Value;

                if (weapon.type.Value == MeleeWeapon.dagger) {
                  effective_crit_chance += 0.005f;
                  effective_crit_chance *= 1.12f;
                }

                if (effective_crit_chance / 0.02f >= 1.1f) {
                  String desc_text = Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", (Int32)Math.Round((effective_crit_chance - 0.001f) / 0.02f));

                  Utility.drawWithShadow(spriteBatch,
                                         Game1.mouseCursors,
                                         new Vector2(x, y),
                                         new Rectangle(40, 428, 10, 10),
                                         Color.White,
                                         0.0f,
                                         Vector2.Zero,
                                         4.0f,
                                         false,
                                         1.0f,
                                         shadowIntensity: 0.2f);
                  spriteBatch.DrawStringEx(font,
                                           desc_text,
                                           new Vector2(x + 48, y + 8),
                                           weapon.hasEnchantmentOfType<AquamarineEnchantment>() ? negative_bg_colour : colours.mForegroundColour,
                                           drawShadow: true,
                                           textShadowColour: text_shadow_colour);
                  y += font_y_diff;
                }
              }

              // Crit multiplier
              {
                Single crit_multiplier = (weapon.critMultiplier.Value - 3f) / 0.02f;

                if (crit_multiplier >= 1.0f) {
                  String desc_text = Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", (Int32)crit_multiplier);

                  Utility.drawWithShadow(spriteBatch,
                                         Game1.mouseCursors,
                                         new Vector2(x - 2, y),
                                         new Rectangle(160, 428, 10, 10),
                                         Color.White,
                                         0.0f,
                                         Vector2.Zero,
                                         4.0f,
                                         false,
                                         1.0f,
                                         shadowIntensity: 0.2f);
                  spriteBatch.DrawStringEx(font,
                                           desc_text,
                                           new Vector2(x + 48, y + 8),
                                           weapon.hasEnchantmentOfType<JadeEnchantment>() ? negative_bg_colour : colours.mForegroundColour,
                                           drawShadow: true,
                                           textShadowColour: text_shadow_colour);
                  y += font_y_diff;
                }
              }

              // Knockback
              {
                if (!weapon.knockback.Value.NearlyEquals(weapon.defaultKnockBackForThisType(weapon.type.Value))) {
                  Double knockback_diff = Math.Ceiling(Math.Abs(weapon.knockback.Value - weapon.defaultKnockBackForThisType(weapon.type.Value)) * 10f);
                  String desc_text = Game1.content.LoadString("Strings\\UI:ItemHover_Weight",
                                                              $"{(knockback_diff > weapon.defaultKnockBackForThisType(weapon.type.Value) ? "+" : "")}{knockback_diff}");

                  Utility.drawWithShadow(spriteBatch,
                                         Game1.mouseCursors,
                                         new Vector2(x, y - 1),
                                         new Rectangle(70, 428, 10, 10),
                                         Color.White,
                                         0.0f,
                                         Vector2.Zero,
                                         4.0f,
                                         false,
                                         1.0f,
                                         shadowIntensity: 0.2f);
                  spriteBatch.DrawStringEx(font,
                                           desc_text,
                                           new Vector2(x + 48, y + 8),
                                           weapon.hasEnchantmentOfType<AmethystEnchantment>() ? negative_bg_colour : colours.mForegroundColour,
                                           drawShadow: true,
                                           textShadowColour: text_shadow_colour);
                  y += font_y_diff;
                }
              }

              // Diamond enchantment (randoms)
              {
                if (weapon.enchantments.LastOrDefault() is DiamondEnchantment) {
                  Int32 random_forges = weapon.GetMaxForges() - weapon.GetTotalForgeLevels();
                  String desc_text = Game1.content.LoadString(random_forges == 1 ? "Strings\\UI:ItemHover_DiamondForge_Singular" : "Strings\\UI:ItemHover_DiamondForge_Plural",
                                                              random_forges);
                  spriteBatch.DrawStringEx(font, desc_text, new Vector2(x, y), negative_bg_colour, drawShadow: true, textShadowColour: text_shadow_colour);
                  y += font_y_diff;
                }
              }

              foreach (BaseEnchantment enchantment in weapon.enchantments.Where(enchantment => enchantment.ShouldBeDisplayed())) {
                Utility.drawWithShadow(spriteBatch,
                                       Game1.mouseCursors2,
                                       new Vector2(x, y),
                                       new Rectangle(127, 35, 10, 10),
                                       Color.White,
                                       0.0f,
                                       Vector2.Zero,
                                       4.0f,
                                       false,
                                       1.0f,
                                       shadowIntensity: 0.2f);
                spriteBatch.DrawStringEx(font,
                                         BaseEnchantment.hideEnchantmentName ? "???" : $"{enchantment.GetDisplayName()}",
                                         new Vector2(x + 48, y + 8),
                                         negative_bg_colour,
                                         drawShadow: true,
                                         textShadowColour: text_shadow_colour);
                y += font_y_diff;
              }
            }

            break;
          }
          default: {
            spriteBatch.DrawStringEx(font, text, new Vector2(x, y), colours.mForegroundColour, drawShadow: true, textShadowColour: text_shadow_colour);
            y += (Int32)font.MeasureString(text).Y + 4;

            if (hoveredItem is Tool tool) {
              foreach (BaseEnchantment enchantment in tool.enchantments.Where(enchantment => enchantment.ShouldBeDisplayed())) {
                Utility.drawWithShadow(spriteBatch,
                                       Game1.mouseCursors2,
                                       new Vector2(x, y),
                                       new Rectangle(127, 35, 10, 10),
                                       Color.White,
                                       0.0f,
                                       Vector2.Zero,
                                       4.0f,
                                       false,
                                       1.0f,
                                       shadowIntensity: 0.2f);
                spriteBatch.DrawStringEx(font,
                                         BaseEnchantment.hideEnchantmentName ? "???" : $"{enchantment.GetDisplayName()}",
                                         new Vector2(x + 48, y + 8),
                                         negative_bg_colour,
                                         drawShadow: true,
                                         textShadowColour: text_shadow_colour);
                y += font_y_diff;
              }
            }

            break;
          }
        }
      }
      // Restoration
      if (healAmountToDisplay != -1 && hoveredItem is StardewValley.Object hovered_object) {
        Int32 stamina_recovery = hovered_object.staminaRecoveredOnConsumption();

        if (stamina_recovery >= 0) {
          Int32 health_recovery = hovered_object.healthRecoveredOnConsumption();
          // draw energy icon
          Utility.drawWithShadow(spriteBatch,
                                 Game1.mouseCursors,
                                 new Vector2(x, y),
                                 new Rectangle(0, 428, 10, 10),
                                 Color.White,
                                 0.0f,
                                 Vector2.Zero,
                                 4.0f,
                                 false,
                                 1.0f,
                                 shadowIntensity: 0.2f);
          spriteBatch.DrawStringEx(font,
                                   Game1.content.LoadString("Strings\\UI:ItemHover_Energy", $"+{stamina_recovery}"),
                                   new Vector2(x + 48, y + 8),
                                   negative_bg_colour,
                                   drawShadow: true,
                                   textShadowColour: text_shadow_colour);
          y += font_y_diff;

          if (health_recovery > 0) {
            // draw health icon
            Utility.drawWithShadow(spriteBatch,
                                   Game1.mouseCursors,
                                   new Vector2(x, y),
                                   new Rectangle(0, 438, 10, 10),
                                   Color.White,
                                   0.0f,
                                   Vector2.Zero,
                                   4.0f,
                                   false,
                                   1.0f,
                                   shadowIntensity: 0.2f);
            spriteBatch.DrawStringEx(font,
                                     Game1.content.LoadString("Strings\\UI:ItemHover_Health", $"+{health_recovery}"),
                                     new Vector2(x + 48, y + 8),
                                     negative_bg_colour,
                                     drawShadow: true,
                                     textShadowColour: text_shadow_colour);
            y += font_y_diff;
          }
        }
        else if (stamina_recovery != -300) {
          // draw energy icon
          Utility.drawWithShadow(spriteBatch,
                                 Game1.mouseCursors,
                                 new Vector2(x, y),
                                 new Rectangle(140, 428, 10, 10),
                                 Color.White,
                                 0.0f,
                                 Vector2.Zero,
                                 4.0f,
                                 false,
                                 1.0f,
                                 shadowIntensity: 0.2f);
          spriteBatch.DrawStringEx(font,
                                   Game1.content.LoadString("Strings\\UI:ItemHover_Energy", $"{stamina_recovery}"),
                                   new Vector2(x + 48, y + 8),
                                   negative_bg_colour,
                                   drawShadow: true,
                                   textShadowColour: text_shadow_colour);
          y += font_y_diff;
        }
      }
      // Buffs
      if (buffIconsToDisplay is not null) {
        for (Int32 j = 0; j < buffIconsToDisplay.Length; j++) {
          if (buffIconsToDisplay[j].Equals("0")) continue;

          String buff_name       = $"{(Convert.ToInt32(buffIconsToDisplay[j]) > 0 ? "+" : "")}{buffIconsToDisplay[j]}";
          if (j <= 11) buff_name = Game1.content.LoadString($"Strings\\UI:ItemHover_Buff{j}", buff_name);

          Utility.drawWithShadow(spriteBatch,
                                 Game1.mouseCursors,
                                 new Vector2(x, y),
                                 new Rectangle(10 + j * 10, 428, 10, 10),
                                 Color.White,
                                 0.0f,
                                 Vector2.Zero,
                                 4.0f,
                                 false,
                                 1.0f,
                                 shadowIntensity: 0.2f);
          spriteBatch.DrawStringEx(font, buff_name, new Vector2(x + 48, y + 8), negative_bg_colour, drawShadow: true, textShadowColour: text_shadow_colour);
          y += font_y_diff;
        }
      }
      // Attachments
      if (hoveredItem is Tool && hoveredItem.attachmentSlots() > 0) {
        switch (hoveredItem) {
          case FishingRod fishing_rod: {
            Int32 y_offset = fishing_rod.enchantments.Any() ? 8 : 4;

            if (fishing_rod.upgradeLevel > 1) {
              if (fishing_rod.attachments[0] is null) {
                spriteBatch.Draw(TexturePresets.gMenuTextureGrayScale,
                                 new Vector2(x, y + y_offset),
                                 Game1.getSourceRectForStandardTileSheet(TexturePresets.gMenuTextureGrayScale, 36),
                                 colours.mForegroundColour,
                                 0.0f,
                                 Vector2.Zero,
                                 1.0f,
                                 SpriteEffects.None,
                                 0.86f);
              }
              else {
                spriteBatch.Draw(TexturePresets.gMenuTextureGrayScale,
                                 new Vector2(x, y + y_offset),
                                 Game1.getSourceRectForStandardTileSheet(TexturePresets.gMenuTextureGrayScale, 10),
                                 colours.mForegroundColour,
                                 0.0f,
                                 Vector2.Zero,
                                 1.0f,
                                 SpriteEffects.None,
                                 0.86f);
                fishing_rod.attachments[0].drawInMenu(spriteBatch, new Vector2(x, y + y_offset), 1.0f);
              }

              y += 68;
            }

            if (fishing_rod.upgradeLevel > 2) {
              if (fishing_rod.attachments[1] is null) {
                spriteBatch.Draw(TexturePresets.gMenuTextureGrayScale,
                                 new Vector2(x, y + y_offset),
                                 Game1.getSourceRectForStandardTileSheet(TexturePresets.gMenuTextureGrayScale, 37),
                                 colours.mForegroundColour,
                                 0.0f,
                                 Vector2.Zero,
                                 1.0f,
                                 SpriteEffects.None,
                                 0.86f);
              }
              else {
                spriteBatch.Draw(TexturePresets.gMenuTextureGrayScale,
                                 new Vector2(x, y + y_offset),
                                 Game1.getSourceRectForStandardTileSheet(TexturePresets.gMenuTextureGrayScale, 10),
                                 colours.mForegroundColour,
                                 0.0f,
                                 Vector2.Zero,
                                 1.0f,
                                 SpriteEffects.None,
                                 0.86f);
                fishing_rod.attachments[1].drawInMenu(spriteBatch, new Vector2(x, y + y_offset), 1.0f);
              }
            }

            break;
          }
          case Slingshot slingshot:
            if (slingshot.attachments[0] is null) {
              spriteBatch.Draw(TexturePresets.gMenuTextureGrayScale,
                               new Vector2(x, y + 4),
                               Game1.getSourceRectForStandardTileSheet(TexturePresets.gMenuTextureGrayScale, 43),
                               colours.mForegroundColour,
                               0.0f,
                               Vector2.Zero,
                               1.0f,
                               SpriteEffects.None,
                               0.86f);
            }
            else {
              spriteBatch.Draw(TexturePresets.gMenuTextureGrayScale,
                               new Vector2(x, y + 4),
                               Game1.getSourceRectForStandardTileSheet(TexturePresets.gMenuTextureGrayScale, 10),
                               colours.mForegroundColour,
                               0.0f,
                               Vector2.Zero,
                               1.0f,
                               SpriteEffects.None,
                               0.86f);
              slingshot.attachments[0].drawInMenu(spriteBatch, new Vector2(x, y + 4), 1.0f);
            }

            break;
        }

        if (moneyAmountToDisplayAtBottom > -1) y += 68;
      }
      // Money
      if (moneyAmountToDisplayAtBottom > -1) {
        spriteBatch.DrawStringEx(font, money_text, new Vector2(x, y), colours.mForegroundColour, drawShadow: true, textShadowColour: text_shadow_colour);

        switch (currencySymbol) {
          case IClickableMenu.currency_g:
            spriteBatch.Draw(Game1.debrisSpriteSheet,
                             new Vector2(x + money_text_size.X + 20.0f, y + 16.0f),
                             Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16),
                             Color.White,
                             0.0f,
                             new Vector2(8.0f, 8.0f),
                             4.0f,
                             SpriteEffects.None,
                             0.95f);

            break;
          case IClickableMenu.currency_starTokens:
            spriteBatch.Draw(Game1.mouseCursors,
                             new Vector2(x + 8 + font.MeasureString(String.Concat(moneyAmountToDisplayAtBottom)).X + 20f, y + 16 - 5),
                             new Rectangle(338, 400, 8, 8),
                             Color.White,
                             0f,
                             Vector2.Zero,
                             4f,
                             SpriteEffects.None,
                             1f);

            break;
          case IClickableMenu.currency_qiCoins:
            spriteBatch.Draw(Game1.mouseCursors,
                             new Vector2(x + 8 + font.MeasureString(String.Concat(moneyAmountToDisplayAtBottom)).X + 20f, y + 16 - 7),
                             new Rectangle(211, 373, 9, 10),
                             Color.White,
                             0f,
                             Vector2.Zero,
                             4f,
                             SpriteEffects.None,
                             1f);

            break;
          case IClickableMenu.currency_qiGems:
            spriteBatch.Draw(Game1.objectSpriteSheet,
                             new Vector2(x + 8 + font.MeasureString(String.Concat(moneyAmountToDisplayAtBottom)).X + 20f, y + 16 - 7),
                             Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16),
                             Color.White,
                             0f,
                             Vector2.Zero,
                             4f,
                             SpriteEffects.None,
                             1f);

            break;
        }

        y += 48;
      }
    }

    public static void DrawToolTip(this SpriteBatch spriteBatch, SpriteFont font, String text, String title,
                                   Item             hoveredItem, Boolean heldItem = false, Int32 currencySymbol = IClickableMenu.currency_g, Int32 moneyAmountToShowAtBottom = -1,
                                   Int32            contentPadding = 4, Colours colours = null, Single alpha = 1.0f, Single borderScale = 1.0f) {
      var      hovered_object = hoveredItem as StardewValley.Object;
      Boolean  edible_item    = hovered_object != null && hovered_object.edibility != -300;
      Int32    heal_amount    = edible_item ? hovered_object.edibility : -1;
      String[] buff_icons     = null;

      if (edible_item && Game1.objectInformation[hovered_object.parentSheetIndex].Split('/') is var obj_info && obj_info.Length > 7)
        buff_icons = hoveredItem.ModifyItemBuffs(obj_info[7].Split(' '));

      DrawHoverText(spriteBatch,
                    font,
                    text == null ? null : new StringBuilder(text),
                    heldItem ? 40 : 0,
                    heldItem ? 40 : 0,
                    moneyAmountToShowAtBottom,
                    title,
                    heal_amount,
                    buff_icons,
                    hoveredItem,
                    currencySymbol,
                    contentPadding,
                    colours,
                    alpha,
                    borderScale);
    }
  }
}
