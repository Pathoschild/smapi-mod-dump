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

// clang-format off
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
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.
// 
// clang-format on

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ChestEx.LanguageExtensions;

using Harmony;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;

using Object = System.Object;

namespace ChestEx.Types.BaseTypes {
  public partial class CustomMenu : IClickableMenu,
                                    IDisposable {
    // Private:
  #region Private

    // Input events:
  #region Input events

    private readonly struct MouseStateData {
      public readonly MouseStateEx mLeft;
      public readonly MouseStateEx mRight;
      public readonly MouseStateEx mMiddle;

      public MouseStateData(MouseStateEx left, MouseStateEx right, MouseStateEx middle) {
        this.mLeft   = left;
        this.mRight  = right;
        this.mMiddle = middle;
      }
    }

    private MouseStateData lastMouseStates;

    private void _eventCursorMoved(Object sender, CursorMovedEventArgs e) {
      if (this.OnCursorMoved(e) == InformStatus.InformItems) {
        this.mMenuItems.ForEach(i => {
          if (!i.mIsVisible) return;

          Vector2 correct_pos = Utility.ModifyCoordinatesForUIScale(e.NewPosition.ScreenPixels);

          i.OnCursorMoved(correct_pos);
          i.mComponents.ForEach(c => {
            if (c.mIsVisible) c.OnCursorMoved(correct_pos);
          });
        });
      }
    }

    private void _eventButtonPressed(Object sender, ButtonPressedEventArgs e) {
      if (this.OnButtonPressed(e) == InformStatus.InformItems) {
        this.mMenuItems.ForEach(i => {
          i.OnButtonPressed(e);
          i.mComponents.ForEach(c => c.OnButtonPressed(e));
        });
      }

      MouseStateEx last_mouse_state = e.Button switch {
        SButton.MouseLeft   => this.lastMouseStates.mLeft,
        SButton.MouseRight  => this.lastMouseStates.mRight,
        SButton.MouseMiddle => this.lastMouseStates.mMiddle,
        _                   => MouseStateEx.gDefault
      };

      if (!last_mouse_state.Equals(MouseStateEx.gDefault)) {
        Vector2 correct_pos = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);

        last_mouse_state.mPos         = correct_pos;
        last_mouse_state.mButtonState = SButtonState.Pressed;

        Point xna_cursor_pos = last_mouse_state.mPos.AsXNAPoint();

        // Call OnMouseClick for items
        this.mMenuItems.ForEach(i => {
          if (!i.mIsVisible) return;
          if (!i.mBounds.Contains(xna_cursor_pos)) return;
          if (!i.mRaiseMouseClickEventOnRelease) i.OnMouseClick(last_mouse_state);

          // Call OnMouseClick for this item's components
          i.mComponents.ForEach(c => {
            if (!c.mIsVisible) return;
            if (!c.mBounds.Contains(xna_cursor_pos)) return;
            if (!c.mRaiseMouseClickEventOnRelease) c.OnMouseClick(last_mouse_state);
          });
        });
      }
    }

    private void _eventButtonReleased(Object sender, ButtonReleasedEventArgs e) {
      if (this.OnButtonReleased(e) == InformStatus.InformItems) {
        this.mMenuItems.ForEach(i => {
          i.OnButtonReleased(e);
          i.mComponents.ForEach(c => c.OnButtonReleased(e));
        });
      }

      // check data for _eventMouseClick
      MouseStateEx last_mouse_state = e.Button switch {
        SButton.MouseLeft   => this.lastMouseStates.mLeft,
        SButton.MouseRight  => this.lastMouseStates.mRight,
        SButton.MouseMiddle => this.lastMouseStates.mMiddle,
        _                   => MouseStateEx.gDefault
      };

      if (!last_mouse_state.Equals(MouseStateEx.gDefault)) {         // is a mouse activity
        if (last_mouse_state.mButtonState == SButtonState.Pressed) { // is a click
          Vector2 correct_pos = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
          // save OnPressed pos
          Point mouse_pressed_point = last_mouse_state.mPos.AsXNAPoint();
          // update mouse state pos
          last_mouse_state.mPos = correct_pos;
          // gen xna cursor pos
          Point xna_cursor_pos = last_mouse_state.mPos.AsXNAPoint();

          // Call OnMouseClick for items
          this.mMenuItems.ForEach(i => {
            if (!i.mIsVisible) return;

            if (i.mBounds.Contains(mouse_pressed_point) /* user pressed while hovering item */
                && i.mBounds.Contains(xna_cursor_pos) /* user released while hovering item */) {
              if (i.mRaiseMouseClickEventOnRelease) i.OnMouseClick(last_mouse_state);

              // Call OnMouseClick for this item's components
              i.mComponents.ForEach(c => {
                if (!c.mIsVisible) return;

                if (c.mBounds.Contains(mouse_pressed_point) && c.mBounds.Contains(xna_cursor_pos))
                  if (c.mRaiseMouseClickEventOnRelease)
                    c.OnMouseClick(last_mouse_state);
              });
            }
          });
        }

        last_mouse_state.mButtonState = SButtonState.Released;
      }
    }

  #endregion

  #endregion

    // Protected:
  #region Protected

    protected Color mColMenuBackground;

    // Input event handlers:
  #region Input event handlers

    /// <summary>
    /// Base implementation informs this menu's items if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    /// <returns>Whether this menu's items are informed of this event.</returns>
    protected virtual InformStatus OnMouseClick(MouseStateEx mouseState) { return this.mIsVisible ? InformStatus.InformItems : InformStatus.DontInformItems; }

    /// <summary>
    /// Base implementation informs this menu's items if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    /// <returns>Whether this menu's items are informed of this event.</returns>
    protected virtual InformStatus OnCursorMoved(CursorMovedEventArgs e) { return this.mIsVisible ? InformStatus.InformItems : InformStatus.DontInformItems; }

    /// <summary>
    /// Base implementation informs this menu's items if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    /// <returns>Whether this menu's items are informed of this event.</returns>
    protected virtual InformStatus OnButtonPressed(ButtonPressedEventArgs e) { return this.mIsVisible ? InformStatus.InformItems : InformStatus.DontInformItems; }

    /// <summary>
    /// Base implementation informs this menu's items if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    /// <returns>Whether this menu's items are informed of this event.</returns>
    protected virtual InformStatus OnButtonReleased(ButtonReleasedEventArgs e) { return this.mIsVisible ? InformStatus.InformItems : InformStatus.DontInformItems; }

  #endregion

  #endregion

    // Public:
  #region Public

    public Rectangle mBounds {
      get => new(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
      private set {
        this.xPositionOnScreen = value.X;
        this.yPositionOnScreen = value.Y;
        this.width             = value.Width;
        this.height            = value.Height;
      }
    }

    public Boolean mIsVisible { get; protected set; }

    public List<CustomMenuItem> mMenuItems { get; protected set; }

    /// <summary>
    /// The inner content rectangle of the game's dialogue box backgroud.
    /// </summary>
    public Rectangle mSafeContentRegion {
      get {
        Rectangle bounds = this.mBounds;

        return new Rectangle(bounds.X + 36, bounds.Y + spaceToClearTopBorder + 4, bounds.Width - borderWidth - 32, bounds.Height - spaceToClearTopBorder - 40);
      }
    }

    // Virtuals:
  #region Virtuals

    /// <summary>
    /// Base implementation sets '<see cref="mIsVisible"/>' and this menu's items' visibility to '<paramref name="isVisible"/>'.
    /// </summary>
    /// <param name="isVisible">Whether this menu should be visible.</param>
    public virtual void SetVisible(Boolean isVisible) {
      this.mIsVisible = isVisible;
      this.mMenuItems.ForEach(i => i.SetVisible(isVisible));
    }

    /// <summary>
    /// Base implementation calls '<see cref="CustomMenu.Dispose()"/>'.
    /// </summary>
    public virtual void OnClose() { this.Dispose(); }

  #endregion

    // Overrides:
  #region Overrides

    /// <summary>
    /// Base implementation draws the dialogue box background if '<see cref="mIsVisible"/>' is true and likewise with this menu's items.
    /// </summary>
    /// <remarks>The dialogue box is drawn using '<see cref=mBoundss"/>' and '<see cref=mColMenuBackgroundd"/>'.</remarks>
    public override void draw(SpriteBatch b) {
      if (!this.mIsVisible) return;

      Rectangle bounds = this.mBounds;

      // dialogue box background, provided by the game
      Game1.drawDialogueBox(bounds.X,
                            bounds.Y,
                            bounds.Width,
                            bounds.Height,
                            false,
                            true,
                            null,
                            false,
                            true,
                            this.mColMenuBackground.R,
                            this.mColMenuBackground.G,
                            this.mColMenuBackground.B);

      this.mMenuItems.ForEach(i => {
        if (i.mIsVisible) i.Draw(b);
      });

      // draw mouse
      Game1.mouseCursorTransparency = 1.0f;
      this.drawMouse(b);
    }

    /// <summary>
    /// Base implementation calls '<see cref="draw(SpriteBatch)"/>' ignoring the params.
    /// </summary>
    public override void draw(SpriteBatch b, Int32 red = -1, Int32 green = -1, Int32 blue = -1) { this.draw(b); }

    /// <summary>
    /// <para>Base implementation:</para>
    /// <para>1. Calls base ('<see cref="IClickableMenu.gameWindowSizeChanged(Rectangle, Rectangle)"/>').</para>
    /// <para>2. Informs this menu's items.</para>
    /// </summary>
    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
      // let game handle resize
      base.gameWindowSizeChanged(oldBounds, newBounds);
      // inform items
      this.mMenuItems.ForEach(i => i.OnGameWindowSizeChanged(oldBounds, newBounds));
    }

  #endregion

    // Statics:
  #region Statics

    public static void DrawHoverText(SpriteBatch b,                     SpriteFont font,                       String text,                 String title = "",
                                     Point       textPadding = default, Color      backgroundColour = default, Color  textColour = default, Single alpha = 1.0f) {
      if (String.IsNullOrWhiteSpace(text)) return;

      if (textPadding == default) textPadding           = new Point(8, 8);
      if (backgroundColour == default) backgroundColour = Color.White;
      if (textColour == default) textColour             = Color.Black;

      Point mouse_pos  = Game1.getMousePosition();
      Point text_size  = font.MeasureString(text).AsXNAPoint();
      Point title_size = String.IsNullOrWhiteSpace(title) ? Point.Zero : Game1.dialogueFont.MeasureString(title).AsXNAPoint();
      var box_rect = new Rectangle(mouse_pos.X + 32,
                                   mouse_pos.Y + 32,
                                   Math.Max(text_size.X, title_size.X) + 22 + textPadding.X * 2,
                                   title_size.Y + text_size.Y + 12 + textPadding.Y * 2);

      // clamp to game viewport
      Rectangle safe_area                                             = GlobalVars.gGameViewport;
      if (box_rect.X + box_rect.Width > safe_area.Right) box_rect.X   = safe_area.Right - box_rect.Width;
      if (box_rect.Y + box_rect.Height > safe_area.Bottom) box_rect.Y = safe_area.Bottom - box_rect.Height;

      drawTextureBox(b,
                     Game1.uncoloredMenuTexture,
                     new Rectangle(0, 256, 60, 60),
                     box_rect.X,
                     box_rect.Y,
                     box_rect.Width,
                     box_rect.Height,
                     backgroundColour * alpha,
                     1.0f,
                     false);

      if (!String.IsNullOrWhiteSpace(title))
        b.DrawString(Game1.dialogueFont, title, new Vector2(box_rect.X + 12.0f + textPadding.X, box_rect.Y + 8.0f + textPadding.Y), textColour);
      b.DrawString(font, text, new Vector2(box_rect.X + 12.0f + textPadding.X, box_rect.Y + 8.0f + title_size.Y + textPadding.Y), textColour);
    }

    public static void DrawHoverText(SpriteBatch b,                        SpriteFont font, StringBuilder text, Color bgColour,
                                     Int32       xOffset             = 0,  Int32      yOffset            = 0, Int32 moneyAmountToDisplayAtBottom = -1, String boldTitleText = null,
                                     Int32       healAmountToDisplay = -1, String[]   buffIconsToDisplay = null, Item hoveredItem = null, Int32 currencySymbol = 0) {
      if (text == null || text.Length == 0) return;
      Color text_colour        = bgColour.ContrastColour();
      Color text_shadow_colour = Color.Multiply(text_colour.ContrastColour(), 0.275f);

      String bold_title_subtext                                             = null;
      if (boldTitleText != null && boldTitleText.Length == 0) boldTitleText = null;
      const Int32 num                                                       = 20;
      Int32 width = Math.Max(healAmountToDisplay != -1 ? (Int32)font.MeasureString(healAmountToDisplay + "+ Energy" + 32).X : 0,
                             Math.Max((Int32)font.MeasureString(text).X, boldTitleText != null ? (Int32)Game1.dialogueFont.MeasureString(boldTitleText).X : 0))
                    + 32;
      Int32 height = Math.Max(num * 3,
                              (Int32)font.MeasureString(text).Y
                              + 32
                              + (Int32)(moneyAmountToDisplayAtBottom > -1 ? font.MeasureString(String.Concat(moneyAmountToDisplayAtBottom)).Y + 4f : 8f)
                              + (Int32)(boldTitleText != null ? Game1.dialogueFont.MeasureString(boldTitleText).Y + 16f : 0f));

      if (buffIconsToDisplay != null) {
        height += buffIconsToDisplay.Where(buffIcon => !buffIcon.Equals("0")).Sum(_ => 34);
        height += 4;
      }

      String category_name = null;

      if (hoveredItem != null) {
        height        += 68 * hoveredItem.attachmentSlots();
        category_name =  hoveredItem.getCategoryName();

        if (category_name.Length > 0) {
          width  =  Math.Max(width, (Int32)font.MeasureString(category_name).X + 32);
          height += (Int32)font.MeasureString("T").Y;
        }

        const Int32 max_stat = 9999;
        const Int32 buffer   = 92;
        Point p = hoveredItem.getExtraSpaceNeededForTooltipSpecialIcons(font,
                                                                        width,
                                                                        buffer,
                                                                        height,
                                                                        text,
                                                                        boldTitleText,
                                                                        moneyAmountToDisplayAtBottom);
        width  = p.X != 0 ? p.X : width;
        height = p.Y != 0 ? p.Y : height;

        switch (hoveredItem) {
          case MeleeWeapon weapon: {
            if (weapon.GetTotalForgeLevels() > 0) height                        += (Int32)font.MeasureString("T").Y;
            if (weapon.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0) height += (Int32)font.MeasureString("T").Y;

            break;
          }
          case StardewValley.Object obj when obj.edibility != -300: {
            height              += healAmountToDisplay != -1 ? 40 * (healAmountToDisplay > 0 ? 2 : 1) : 40;
            healAmountToDisplay =  obj.staminaRecoveredOnConsumption();
            width = (Int32)Math.Max(width,
                                    Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Energy", max_stat)).X + buffer,
                                             font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Health", max_stat)).X + buffer));

            break;
          }
        }

        if (buffIconsToDisplay != null) {
          for (Int32 i = 0; i < buffIconsToDisplay.Length; i++)
            if (!buffIconsToDisplay[i].Equals("0") && i <= 11)
              width = (Int32)Math.Max(width, font.MeasureString(Game1.content.LoadString($"Strings\\UI:ItemHover_Buff{i}", max_stat)).X + buffer);
        }
      }

      Vector2 small_text_size = Vector2.Zero;

      if (bold_title_subtext != null && boldTitleText != null) {
        small_text_size = Game1.smallFont.MeasureString(bold_title_subtext);
        width           = (Int32)Math.Max(width, Game1.dialogueFont.MeasureString(boldTitleText).X + small_text_size.X + 12f);
      }

      Int32 x = Game1.getOldMouseX() + 32 + xOffset;
      Int32 y = Game1.getOldMouseY() + 32 + yOffset;

      if (x + width > Utility.getSafeArea().Right) {
        x =  Utility.getSafeArea().Right - width;
        y += 16;
      }

      if (y + height > Utility.getSafeArea().Bottom) {
        x += 16;
        if (x + width > Utility.getSafeArea().Right) x = Utility.getSafeArea().Right - width;
        y = Utility.getSafeArea().Bottom - height;
      }

      drawTextureBox(b,
                     Game1.uncoloredMenuTexture,
                     new Rectangle(0, 256, 60, 60),
                     x,
                     y,
                     width,
                     height,
                     bgColour);

      if (boldTitleText != null) {
        Vector2 bold_text_size = Game1.dialogueFont.MeasureString(boldTitleText);
        drawTextureBox(b,
                       Game1.uncoloredMenuTexture,
                       new Rectangle(0, 256, 60, 60),
                       x,
                       y,
                       width,
                       (Int32)Game1.dialogueFont.MeasureString(boldTitleText).Y
                       + 32
                       + (Int32)(hoveredItem != null && category_name.Length > 0 ? font.MeasureString("asd").Y : 0f)
                       - 4,
                       bgColour,
                       1f,
                       false);
        b.Draw(Game1.uncoloredMenuTexture,
               new Rectangle(x + 12,
                             y
                             + (Int32)Game1.dialogueFont.MeasureString(boldTitleText).Y
                             + 32
                             + (Int32)(hoveredItem != null && category_name.Length > 0 ? font.MeasureString("asd").Y : 0f)
                             - 4,
                             width - 24,
                             4),
               new Rectangle(44, 300, 4, 4),
               bgColour);
        b.DrawStringEx(Game1.dialogueFont, boldTitleText, new Point(x + 16, y + 16 + 4), text_colour, drawShadow: true, textShadowColour: text_shadow_colour);

        if (!String.IsNullOrEmpty(bold_title_subtext)) {
          b.DrawStringEx(font,
                         bold_title_subtext,
                         new Point(x + 16 + (Int32)bold_text_size.X, (Int32)(y + 16 + 4 + bold_text_size.Y / 2f - small_text_size.Y / 2f)),
                         text_colour,
                         drawShadow: true,
                         textShadowColour: text_shadow_colour);
        }

        y += (Int32)Game1.dialogueFont.MeasureString(boldTitleText).Y;
      }

      if (hoveredItem != null && category_name.Length > 0) {
        y -= 4;

        b.DrawStringEx(font,
                       category_name,
                       new Point(x + 16, y + 16 + 4),
                       text_colour,
                       drawShadow: true,
                       textShadowColour: text_shadow_colour,
                       textShadowAlpha: 0.9f);

        y += (Int32)font.MeasureString("T").Y + (boldTitleText != null ? 16 : 0) + 4;

        if (hoveredItem is Tool tool && tool.GetTotalForgeLevels() > 0) {
          String forged_string = $"> {Game1.content.LoadString("Strings\\UI:Item_Tooltip_Forged")}";
          b.DrawStringEx(font, forged_string, new Point(x + 16, y + 16 + 4), text_colour, drawShadow: true, textShadowColour: text_shadow_colour);
          Int32 forges = tool.GetTotalForgeLevels();

          if (forges < tool.GetMaxForges() && !tool.hasEnchantmentOfType<DiamondEnchantment>()) {
            b.DrawStringEx(font,
                           $@" ({forges}/{tool.GetMaxForges()})",
                           new Point(x + 16 + (Int32)font.MeasureString(forged_string).X, y + 16 + 4),
                           text_colour,
                           drawShadow: true,
                           textShadowColour: text_shadow_colour);
          }

          y += (Int32)font.MeasureString("T").Y;
        }

        if (hoveredItem is MeleeWeapon weapon && weapon.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0) {
          var    enchantment   = weapon.GetEnchantmentOfType<GalaxySoulEnchantment>();
          String forged_string = $"> {Game1.content.LoadString("Strings\\UI:Item_Tooltip_GalaxyForged")}";
          b.DrawStringEx(font, forged_string, new Point(x + 16, y + 16 + 4), text_colour, drawShadow: true, textShadowColour: text_shadow_colour);
          Int32 level = enchantment.GetLevel();

          if (level < enchantment.GetMaximumLevel()) {
            b.DrawStringEx(font,
                           $@" ({level}/{enchantment.GetMaximumLevel()})",
                           new Point(x + 16 + (Int32)font.MeasureString(forged_string).X, y + 16 + 4),
                           text_colour,
                           drawShadow: true,
                           textShadowColour: text_shadow_colour);
          }

          y += (Int32)font.MeasureString("T").Y;
        }
      }
      else
        y += boldTitleText != null ? 16 : 0;

      if (!String.IsNullOrWhiteSpace(text.ToString())) {
        switch (hoveredItem) {
          case Boots boots: {
            Int32  desc_width = Traverse.Create(boots).Method("getDescriptionWidth").GetValue<Int32>();
            String desc_text  = Game1.parseText(boots.description, Game1.smallFont, desc_width);

            b.DrawStringEx(font, desc_text, new Point(x + 16, y + 16 + 4), text_colour, drawShadow: true, textShadowColour: text_shadow_colour);
            y += (Int32)font.MeasureString(desc_text).Y;

            // Defence
            {
              if (boots.defenseBonus > 0) {
                Utility.drawWithShadow(b,
                                       Game1.mouseCursors,
                                       new Point(x + 16 + 4, y + 16 + 4).AsXNAVector2(),
                                       new Rectangle(110, 428, 10, 10),
                                       Color.White,
                                       0f,
                                       Vector2.Zero,
                                       4f,
                                       false,
                                       1f);
                b.DrawStringEx(font,
                               Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", boots.defenseBonus),
                               new Point(x + 16 + 52, y + 16 + 12),
                               text_colour,
                               0.9f,
                               drawShadow: true,
                               textShadowColour: text_shadow_colour);
                y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
              }
            }

            // Immunity
            {
              if (boots.immunityBonus > 0) {
                Utility.drawWithShadow(b,
                                       Game1.mouseCursors,
                                       new Point(x + 16 + 4, y + 16 + 4).AsXNAVector2(),
                                       new Rectangle(150, 428, 10, 10),
                                       Color.White,
                                       0f,
                                       Vector2.Zero,
                                       4f,
                                       false,
                                       1f);
                b.DrawStringEx(font,
                               Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", boots.immunityBonus),
                               new Point(x + 16 + 52, y + 16 + 12),
                               text_colour,
                               0.9f,
                               drawShadow: true,
                               textShadowColour: text_shadow_colour);
                y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
              }
            }

            break;
          }
          case Ring ring: {
            Int32  desc_width = Traverse.Create(ring).Method("getDescriptionWidth").GetValue<Int32>();
            String desc_text  = Game1.parseText(ring.description, Game1.smallFont, desc_width);

            b.DrawStringEx(font, desc_text, new Point(x + 16, y + 16 + 4), text_colour, drawShadow: true, textShadowColour: text_shadow_colour);
            y += (Int32)font.MeasureString(desc_text).Y;

            // Defence
            if (ring.GetsEffectOfRing(810)) {
              Utility.drawWithShadow(b,
                                     Game1.mouseCursors,
                                     new Point(x + 16 + 4, y + 16 + 4).AsXNAVector2(),
                                     new Rectangle(110, 428, 10, 10),
                                     Color.White,
                                     0f,
                                     Vector2.Zero,
                                     4f,
                                     false,
                                     1f);
              b.DrawStringEx(font,
                             Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", 5 * ring.GetEffectsOfRingMultiplier(810)),
                             new Point(x + 16 + 52, y + 16 + 12),
                             text_colour,
                             0.9f,
                             drawShadow: true,
                             textShadowColour: text_shadow_colour);
              y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // Immunity
            if (ring.GetsEffectOfRing(887)) {
              Utility.drawWithShadow(b,
                                     Game1.mouseCursors,
                                     new Point(x + 16 + 4, y + 16 + 4).AsXNAVector2(),
                                     new Rectangle(150, 428, 10, 10),
                                     Color.White,
                                     0f,
                                     Vector2.Zero,
                                     4f,
                                     false,
                                     1f);
              b.DrawStringEx(font,
                             Game1.content.LoadString("Strings\\UI:ItemHover_ImmunityBonus", 4 * ring.GetEffectsOfRingMultiplier(887)),
                             new Point(x + 16 + 52, y + 16 + 12),
                             text_colour,
                             0.9f,
                             drawShadow: true,
                             textShadowColour: text_shadow_colour);
              y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // Luck
            if (ring.GetsEffectOfRing(859)) {
              Utility.drawWithShadow(b,
                                     Game1.mouseCursors,
                                     new Point(x + 16 + 4, y + 16 + 4).AsXNAVector2(),
                                     new Rectangle(50, 428, 10, 10),
                                     Color.White,
                                     0f,
                                     Vector2.Zero,
                                     4f,
                                     false,
                                     1f);
              b.DrawStringEx(font,
                             $"+{Game1.content.LoadString("Strings\\UI:ItemHover_Buff4", ring.GetEffectsOfRingMultiplier(859))}",
                             new Point(x + 16 + 52, y + 16 + 12),
                             text_colour,
                             0.9f,
                             drawShadow: true,
                             textShadowColour: text_shadow_colour);
              y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            break;
          }
          case MeleeWeapon weapon: {
            // Description
            {
              Int32  desc_width = Traverse.Create(weapon).Method("getDescriptionWidth").GetValue<Int32>();
              String desc_text  = Game1.parseText(weapon.description, Game1.smallFont, desc_width);
              b.DrawStringEx(font, desc_text, new Point(x + 16, y + 16 + 4), text_colour, drawShadow: true, textShadowColour: text_shadow_colour);
              y += (Int32)font.MeasureString(desc_text).Y;
            }

            if (!weapon.isScythe(weapon.IndexOfMenuItemView)) {
              // Damage
              {
                String desc_text = Game1.content.LoadString("Strings\\UI:ItemHover_Damage", weapon.minDamage.Value, weapon.maxDamage.Value);
                if (weapon.hasEnchantmentOfType<RubyEnchantment>()) desc_text = $"= {desc_text}";

                Utility.drawWithShadow(b,
                                       Game1.mouseCursors,
                                       new Vector2(x + 16 + 4, y + 16 + 4),
                                       new Rectangle(120, 428, 10, 10),
                                       Color.White,
                                       0f,
                                       Vector2.Zero,
                                       4f,
                                       false,
                                       1f);
                b.DrawStringEx(font,
                               desc_text,
                               new Point(x + 16 + 52, y + 16 + 12),
                               text_colour,
                               0.9f,
                               drawShadow: true,
                               textShadowColour: text_shadow_colour);
                y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
              }

              // Speed
              {
                if (weapon.speed.Value != (weapon.type.Value == MeleeWeapon.club ? -8 : 0)) {
                  Int32  weapon_real_speed = weapon.speed.Value + (weapon.type.Value == MeleeWeapon.club ? 8 : 0);
                  String desc_text = Game1.content.LoadString("Strings\\UI:ItemHover_Speed", $"{(weapon_real_speed > 0 ? "+" : "")}{weapon_real_speed / 2}");
                  if (weapon.hasEnchantmentOfType<EmeraldEnchantment>()) desc_text = $"= {desc_text}";

                  Utility.drawWithShadow(b,
                                         Game1.mouseCursors,
                                         new Vector2(x + 16 + 4, y + 16 + 4),
                                         new Rectangle(130, 428, 10, 10),
                                         Color.White,
                                         0f,
                                         Vector2.Zero,
                                         4f,
                                         false,
                                         1f);
                  b.DrawStringEx(font,
                                 desc_text,
                                 new Point(x + 16 + 52, y + 16 + 12),
                                 text_colour,
                                 0.9f,
                                 drawShadow: true,
                                 textShadowColour: text_shadow_colour);
                  y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
                }
              }

              // Defence
              {
                if (weapon.addedDefense.Value > 0) {
                  String desc_text                                               = Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", weapon.addedDefense.Value);
                  if (weapon.hasEnchantmentOfType<TopazEnchantment>()) desc_text = $"= {desc_text}";

                  Utility.drawWithShadow(b,
                                         Game1.mouseCursors,
                                         new Vector2(x + 16 + 4, y + 16 + 4),
                                         new Rectangle(110, 428, 10, 10),
                                         Color.White,
                                         0f,
                                         Vector2.Zero,
                                         4f,
                                         false,
                                         1f);
                  b.DrawStringEx(font,
                                 desc_text,
                                 new Point(x + 16 + 52, y + 16 + 12),
                                 text_colour,
                                 0.9f,
                                 drawShadow: true,
                                 textShadowColour: text_shadow_colour);
                  y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
                }
              }

              // Crit chance
              {
                Single effective_crit_chance = weapon.critChance.Value;

                if (weapon.type.Value == 1) {
                  effective_crit_chance += 0.005f;
                  effective_crit_chance *= 1.12f;
                }

                if (effective_crit_chance / 0.02f >= 1.1f) {
                  String desc_text = Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", (Int32)Math.Round((effective_crit_chance - 0.001f) / 0.02f));
                  if (weapon.hasEnchantmentOfType<AquamarineEnchantment>()) desc_text = $"= {desc_text}";

                  Utility.drawWithShadow(b,
                                         Game1.mouseCursors,
                                         new Vector2(x + 16 + 4, y + 16 + 4),
                                         new Rectangle(40, 428, 10, 10),
                                         Color.White,
                                         0f,
                                         Vector2.Zero,
                                         4f,
                                         false,
                                         1f);
                  b.DrawStringEx(font,
                                 desc_text,
                                 new Point(x + 16 + 52, y + 16 + 12),
                                 text_colour,
                                 0.9f,
                                 drawShadow: true,
                                 textShadowColour: text_shadow_colour);
                  y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
                }
              }

              // Crit multiplier
              {
                Single crit_multiplier = (weapon.critMultiplier.Value - 3f) / 0.02f;

                if (crit_multiplier >= 1.0f) {
                  String desc_text                                              = Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", (Int32)crit_multiplier);
                  if (weapon.hasEnchantmentOfType<JadeEnchantment>()) desc_text = $"= {desc_text}";

                  Utility.drawWithShadow(b,
                                         Game1.mouseCursors,
                                         new Vector2(x + 16, y + 16 + 4),
                                         new Rectangle(160, 428, 10, 10),
                                         Color.White,
                                         0f,
                                         Vector2.Zero,
                                         4f,
                                         false,
                                         1f);
                  b.DrawStringEx(font,
                                 desc_text,
                                 new Point(x + 16 + 52, y + 16 + 12),
                                 text_colour,
                                 0.9f,
                                 drawShadow: true,
                                 textShadowColour: text_shadow_colour);
                  y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
                }
              }

              // Knockback
              {
                if (!weapon.knockback.Value.NearlyEquals(weapon.defaultKnockBackForThisType(weapon.type.Value))) {
                  Double knockback_diff = Math.Ceiling(Math.Abs(weapon.knockback.Value - weapon.defaultKnockBackForThisType(weapon.type.Value)) * 10f);
                  String desc_text = Game1.content.LoadString("Strings\\UI:ItemHover_Weight",
                                                              $"{(knockback_diff > weapon.defaultKnockBackForThisType(weapon.type.Value) ? "+" : "")}{knockback_diff}");
                  if (weapon.hasEnchantmentOfType<AmethystEnchantment>()) desc_text = $"= {desc_text}";

                  Utility.drawWithShadow(b,
                                         Game1.mouseCursors,
                                         new Vector2(x + 16 + 4, y + 16 + 4),
                                         new Rectangle(70, 428, 10, 10),
                                         Color.White,
                                         0f,
                                         Vector2.Zero,
                                         4f,
                                         false,
                                         1f);
                  b.DrawStringEx(font,
                                 desc_text,
                                 new Point(x + 16 + 52, y + 16 + 12),
                                 text_colour,
                                 0.9f,
                                 drawShadow: true,
                                 textShadowColour: text_shadow_colour);
                  y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
                }
              }

              // Diamond enchantment (randoms)
              {
                if (weapon.enchantments.LastOrDefault() is DiamondEnchantment) {
                  Int32 random_forges = weapon.GetMaxForges() - weapon.GetTotalForgeLevels();
                  String desc_text =
                    "= "
                    + Game1.content.LoadString(random_forges == 1 ? "Strings\\UI:ItemHover_DiamondForge_Singular" : "Strings\\UI:ItemHover_DiamondForge_Plural", random_forges);
                  b.DrawStringEx(font,
                                 desc_text,
                                 new Point(x + 16, y + 16 + 12),
                                 text_colour,
                                 0.9f,
                                 drawShadow: true,
                                 textShadowColour: text_shadow_colour);
                  y += (Int32)Math.Max(font.MeasureString("TT").Y, 48f);
                }
              }

              foreach (BaseEnchantment enchantment in weapon.enchantments.Where(enchantment => enchantment.ShouldBeDisplayed())) {
                Utility.drawWithShadow(b,
                                       Game1.mouseCursors2,
                                       new Vector2(x + 16 + 4, y + 16 + 4),
                                       new Rectangle(127, 35, 10, 10),
                                       Color.White,
                                       0f,
                                       Vector2.Zero,
                                       4f,
                                       false,
                                       1f);
                b.DrawStringEx(font,
                               BaseEnchantment.hideEnchantmentName ? "???" : $"= {enchantment.GetDisplayName()}",
                               new Point(x + 16 + 52, y + 16 + 12),
                               text_colour,
                               0.9f,
                               drawShadow: true,
                               textShadowColour: text_shadow_colour);
                y += (Int32)Math.Max(font.MeasureString("TT").Y, 48.0f);
              }
            }

            break;
          }
          default: {
            b.DrawStringEx(font, text, new Point(x + 16, y + 16 + 4), text_colour, drawShadow: true, textShadowColour: text_shadow_colour);
            y += (Int32)font.MeasureString(text).Y + 4;

            if (hoveredItem is Tool tool) {
              foreach (BaseEnchantment enchantment in tool.enchantments.Where(enchantment => enchantment.ShouldBeDisplayed())) {
                Utility.drawWithShadow(b,
                                       Game1.mouseCursors2,
                                       new Vector2(x + 16 + 4, y + 16 + 4),
                                       new Rectangle(127, 35, 10, 10),
                                       Color.White,
                                       0f,
                                       Vector2.Zero,
                                       4f,
                                       false,
                                       1f);
                b.DrawStringEx(font,
                               BaseEnchantment.hideEnchantmentName ? "???" : $"= {enchantment.GetDisplayName()}",
                               new Point(x + 16 + 52, y + 16 + 12),
                               text_colour,
                               0.9f,
                               drawShadow: true,
                               textShadowColour: text_shadow_colour);
                y += (Int32)Math.Max(font.MeasureString("TT").Y, 48.0f);
              }
            }

            break;
          }
        }
      }

      if (healAmountToDisplay != -1 && hoveredItem is StardewValley.Object hovered_object) {
        Int32 stamina_recovery = hovered_object.staminaRecoveredOnConsumption();

        if (stamina_recovery >= 0) {
          Int32 health_recovery = hovered_object.healthRecoveredOnConsumption();
          // draw energy icon
          Utility.drawWithShadow(b,
                                 Game1.mouseCursors,
                                 new Vector2(x + 16 + 4, y + 16),
                                 new Rectangle(0, 428, 10, 10),
                                 Color.White,
                                 0f,
                                 Vector2.Zero,
                                 3f,
                                 false,
                                 0.95f);
          b.DrawStringEx(font,
                         Game1.content.LoadString("Strings\\UI:ItemHover_Energy", $"+{stamina_recovery}"),
                         new Point(x + 16 + 34 + 4, y + 16),
                         text_colour,
                         0.9f,
                         drawShadow: true,
                         shadowDistance: 2.0f,
                         textShadowColour: text_shadow_colour,
                         textShadowAlpha: 0.85f);
          y += 34;

          if (health_recovery > 0) {
            // draw health icon
            Utility.drawWithShadow(b,
                                   Game1.mouseCursors,
                                   new Vector2(x + 16 + 4, y + 16),
                                   new Rectangle(0, 438, 10, 10),
                                   Color.White,
                                   0f,
                                   Vector2.Zero,
                                   3f,
                                   false,
                                   0.95f);

            b.DrawStringEx(font,
                           Game1.content.LoadString("Strings\\UI:ItemHover_Health", $"+{health_recovery}"),
                           new Point(x + 16 + 34 + 4, y + 16),
                           text_colour,
                           0.9f,
                           drawShadow: true,
                           shadowDistance: 2.0f,
                           textShadowColour: text_shadow_colour,
                           textShadowAlpha: 0.85f);
            y += 34;
          }
        }
        else if (stamina_recovery != -300) {
          // draw energy icon
          Utility.drawWithShadow(b,
                                 Game1.mouseCursors,
                                 new Vector2(x + 16 + 4, y + 16),
                                 new Rectangle(140, 428, 10, 10),
                                 Color.White,
                                 0f,
                                 Vector2.Zero,
                                 3f,
                                 false,
                                 0.95f);
          b.DrawStringEx(font,
                         Game1.content.LoadString("Strings\\UI:ItemHover_Energy", $"{stamina_recovery}"),
                         new Point(x + 16 + 34 + 4, y + 16),
                         text_colour,
                         0.9f,
                         drawShadow: true,
                         shadowDistance: 2.0f,
                         textShadowColour: text_shadow_colour,
                         textShadowAlpha: 0.85f);
          y += 34;
        }
      }

      if (buffIconsToDisplay is not null) {
        for (Int32 j = 0; j < buffIconsToDisplay.Length; j++) {
          if (buffIconsToDisplay[j].Equals("0")) continue;

          Utility.drawWithShadow(b,
                                 Game1.mouseCursors,
                                 new Vector2(x + 16 + 4, y + 16),
                                 new Rectangle(10 + j * 10, 428, 10, 10),
                                 Color.White,
                                 0f,
                                 Vector2.Zero,
                                 3f,
                                 false,
                                 0.95f);
          String buff_name       = $"{(Convert.ToInt32(buffIconsToDisplay[j]) > 0 ? "+" : "")}{buffIconsToDisplay[j]} ";
          if (j <= 11) buff_name = Game1.content.LoadString($"Strings\\UI:ItemHover_Buff{j}", buff_name);
          b.DrawStringEx(font,
                         buff_name,
                         new Point(x + 16 + 34 + 4, y + 16),
                         text_colour,
                         0.9f,
                         drawShadow: true,
                         textShadowColour: text_shadow_colour);
          y += 34;
        }
      }

      if (hoveredItem is Tool && hoveredItem.attachmentSlots() > 0) {
        switch (hoveredItem) {
          case FishingRod fishing_rod: {
            Int32 y_offset = fishing_rod.enchantments.Any() ? 8 : 4;

            if (fishing_rod.upgradeLevel > 1) {
              if (fishing_rod.attachments[0] is null) {
                b.Draw(Game1.uncoloredMenuTexture,
                       new Vector2(x + 16, y + 16 + y_offset),
                       Game1.getSourceRectForStandardTileSheet(Game1.uncoloredMenuTexture, 36),
                       text_colour,
                       0f,
                       Vector2.Zero,
                       1f,
                       SpriteEffects.None,
                       0.86f);
              }
              else {
                b.Draw(Game1.uncoloredMenuTexture,
                       new Vector2(x + 16, y + 16 + y_offset),
                       Game1.getSourceRectForStandardTileSheet(Game1.uncoloredMenuTexture, 10),
                       text_colour,
                       0f,
                       Vector2.Zero,
                       1f,
                       SpriteEffects.None,
                       0.86f);
                fishing_rod.attachments[0].drawInMenu(b, new Vector2(x + 16, y + 16 + y_offset), 1f);
              }
            }

            if (fishing_rod.upgradeLevel > 2) {
              if (fishing_rod.attachments[1] is null) {
                b.Draw(Game1.uncoloredMenuTexture,
                       new Vector2(x + 16, y + 16 + 64 + 4 + y_offset),
                       Game1.getSourceRectForStandardTileSheet(Game1.uncoloredMenuTexture, 37),
                       text_colour,
                       0f,
                       Vector2.Zero,
                       1f,
                       SpriteEffects.None,
                       0.86f);
              }
              else {
                b.Draw(Game1.uncoloredMenuTexture,
                       new Vector2(x + 16, y + 16 + 64 + 4 + y_offset),
                       Game1.getSourceRectForStandardTileSheet(Game1.uncoloredMenuTexture, 10),
                       text_colour,
                       0f,
                       Vector2.Zero,
                       1f,
                       SpriteEffects.None,
                       0.86f);
                fishing_rod.attachments[1].drawInMenu(b, new Vector2(x + 16, y + 16 + 64 + 4 + y_offset), 1f);
              }
            }

            break;
          }
          case Slingshot slingshot:
            if (slingshot.attachments[0] is null) {
              b.Draw(Game1.uncoloredMenuTexture,
                     new Vector2(x + 16, y + 16),
                     Game1.getSourceRectForStandardTileSheet(Game1.uncoloredMenuTexture, 43),
                     text_colour,
                     0f,
                     Vector2.Zero,
                     1f,
                     SpriteEffects.None,
                     0.86f);
            }
            else {
              b.Draw(Game1.uncoloredMenuTexture,
                     new Vector2(x + 16, y + 16),
                     Game1.getSourceRectForStandardTileSheet(Game1.uncoloredMenuTexture, 10),
                     text_colour,
                     0f,
                     Vector2.Zero,
                     1f,
                     SpriteEffects.None,
                     0.86f);
              slingshot.attachments[0].drawInMenu(b, new Vector2(x + 16, y + 16), 1f);
            }

            break;
        }

        if (moneyAmountToDisplayAtBottom > -1) y += 68 * hoveredItem.attachmentSlots();
      }

      if (moneyAmountToDisplayAtBottom > -1) {
        b.DrawStringEx(font, Convert.ToString(moneyAmountToDisplayAtBottom), new Point(x + 16, y + 16 + 4), text_colour, drawShadow: true, textShadowColour: text_shadow_colour);

        switch (currencySymbol) {
          case 0:
            b.Draw(Game1.debrisSpriteSheet,
                   new Vector2(x + 16 + font.MeasureString(String.Concat(moneyAmountToDisplayAtBottom)).X + 20f, y + 16 + 16),
                   Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16),
                   Color.White,
                   0f,
                   new Vector2(8f, 8f),
                   4f,
                   SpriteEffects.None,
                   0.95f);

            break;
          case 1:
            b.Draw(Game1.mouseCursors,
                   new Vector2(x + 8 + font.MeasureString(String.Concat(moneyAmountToDisplayAtBottom)).X + 20f, y + 16 - 5),
                   new Rectangle(338, 400, 8, 8),
                   Color.White,
                   0f,
                   Vector2.Zero,
                   4f,
                   SpriteEffects.None,
                   1f);

            break;
          case 2:
            b.Draw(Game1.mouseCursors,
                   new Vector2(x + 8 + font.MeasureString(String.Concat(moneyAmountToDisplayAtBottom)).X + 20f, y + 16 - 7),
                   new Rectangle(211, 373, 9, 10),
                   Color.White,
                   0f,
                   Vector2.Zero,
                   4f,
                   SpriteEffects.None,
                   1f);

            break;
          case 4:
            b.Draw(Game1.objectSpriteSheet,
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

    public static void DrawToolTip(SpriteBatch b,          SpriteFont font,        Color   bgColour,         String hoverText,
                                   String      hoverTitle, Item       hoveredItem, Boolean heldItem = false, Int32  currencySymbol = 0,
                                   Int32       moneyAmountToShowAtBottom = -1) {
      var      hovered_object = hoveredItem as StardewValley.Object;
      Boolean  edible_item    = hovered_object != null && hovered_object.edibility != -300;
      Int32    heal_amount    = edible_item ? hovered_object?.edibility : -1;
      String[] buff_icons     = null;

      if (edible_item && Game1.objectInformation[hovered_object.parentSheetIndex].Split('/') is var obj_info && obj_info.Length > 7)
        buff_icons = hoveredItem.ModifyItemBuffs(obj_info[7].Split(' '));

      DrawHoverText(b,
                    font,
                    hoverText is null ? null : new StringBuilder(hoverText),
                    bgColour,
                    heldItem ? 40 : 0,
                    heldItem ? 40 : 0,
                    moneyAmountToShowAtBottom,
                    hoverTitle,
                    heal_amount,
                    buff_icons,
                    hoveredItem,
                    currencySymbol);
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public CustomMenu(Color menuBackgroundColour, Rectangle bounds) : base(bounds.X, bounds.Y, bounds.Width, bounds.Height) {
      this.lastMouseStates = new MouseStateData(new MouseStateEx(SButton.MouseLeft), new MouseStateEx(SButton.MouseRight), new MouseStateEx(SButton.MouseMiddle));

      this.mColMenuBackground = menuBackgroundColour;

      this.mMenuItems = new List<CustomMenuItem>();
      this.SetVisible(true);

      // register menu exit function
      this.exitFunction = this.OnClose;

      // register SMAPI events
      GlobalVars.gSMAPIHelper.Events.Input.CursorMoved    += this._eventCursorMoved;
      GlobalVars.gSMAPIHelper.Events.Input.ButtonPressed  += this._eventButtonPressed;
      GlobalVars.gSMAPIHelper.Events.Input.ButtonReleased += this._eventButtonReleased;
    }

  #endregion

    // IDisposable:
  #region IDisposable

    /// <summary>
    /// <para>Base implementation:</para>
    /// <para>1. Unregisters '<see cref="IClickableMenu.exitFunction"/>'.</para>
    /// <para>2. Calls '<see cref="SetVisible(Boolean)"/>' with 'false'.</para>
    /// <para>3. Disposes of this menu's items.</para>
    /// <para>4. Unregisters SMAPI input events.</para>
    /// </summary>
    public virtual void Dispose() {
      // unregister menu exit function
      this.exitFunction = null;

      // hide
      this.SetVisible(false);

      // dispose items
      this.mMenuItems.ForEach(i => i.Dispose());
      this.mMenuItems.Clear();

      // unregister SMAPI events
      GlobalVars.gSMAPIHelper.Events.Input.CursorMoved    -= this._eventCursorMoved;
      GlobalVars.gSMAPIHelper.Events.Input.ButtonPressed  -= this._eventButtonPressed;
      GlobalVars.gSMAPIHelper.Events.Input.ButtonReleased -= this._eventButtonReleased;
    }

  #endregion
  }
}
