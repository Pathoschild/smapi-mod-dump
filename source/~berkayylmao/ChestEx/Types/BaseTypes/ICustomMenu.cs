/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

//
//    Copyright (C) 2020 Berkay Yigit <berkaytgy@gmail.com>
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

using System;
using System.Collections.Generic;

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Menus;

namespace ChestEx.Types.BaseTypes {
   public partial class ICustomMenu : IClickableMenu, IDisposable {
      // Private:
      #region Private

      // Input events:
      #region Input events

      private (MouseStateEx left, MouseStateEx right, MouseStateEx middle) _lastMouseStates;

      private void _eventCursorMoved(Object sender, StardewModdingAPI.Events.CursorMovedEventArgs e) {
         if (this.onCursorMoved(e) == InformStatus.InformItems) {
            this.MenuItems.ForEach((i) =>
            {
               if (!i.IsVisible)
                  return;

               i.OnCursorMoved(e);
               i.Components.ForEach((c) => { if (c.IsVisible) c.OnCursorMoved(e); });
            });
         }
      }

      private void _eventButtonPressed(Object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e) {
         if (this.onButtonPressed(e) == InformStatus.InformItems) {
            this.MenuItems.ForEach((i) =>
            {
               i.OnButtonPressed(e);
               i.Components.ForEach((c) => c.OnButtonPressed(e));
            });
         }

         var last_mouse_state = e.Button switch
         {
            StardewModdingAPI.SButton.MouseLeft => _lastMouseStates.left,
            StardewModdingAPI.SButton.MouseRight => _lastMouseStates.right,
            StardewModdingAPI.SButton.MouseMiddle => _lastMouseStates.middle,
            _ => MouseStateEx.Default
         };

         if (!last_mouse_state.Equals(ICustomMenu.MouseStateEx.Default)) {
            last_mouse_state.Pos = e.Cursor.ScreenPixels;
            last_mouse_state.ButtonState = StardewModdingAPI.SButtonState.Pressed;

            // Call OnMouseClick for items
            this.MenuItems.ForEach((i) =>
            {
               if (!i.IsVisible)
                  return;

               if (i.Bounds.Contains(e.Cursor.ScreenPixels.AsXNAPoint())) {
                  if (!i.RaiseMouseClickEventOnRelease)
                     i.OnMouseClick(last_mouse_state);

                  // Call OnMouseClick for this item's components
                  i.Components.ForEach((c) =>
                  {
                     if (!c.IsVisible)
                        return;

                     if (c.Bounds.Contains(e.Cursor.ScreenPixels.AsXNAPoint())) {
                        if (!c.RaiseMouseClickEventOnRelease)
                           c.OnMouseClick(last_mouse_state);
                     }
                  });
               }
            });
         }
      }

      private void _eventButtonReleased(Object sender, StardewModdingAPI.Events.ButtonReleasedEventArgs e) {
         if (this.onButtonReleased(e) == InformStatus.InformItems) {
            this.MenuItems.ForEach((i) =>
            {
               i.OnButtonReleased(e);
               i.Components.ForEach((c) => c.OnButtonReleased(e));
            });
         }

         // check data for _eventMouseClick
         var last_mouse_state = e.Button switch
         {
            StardewModdingAPI.SButton.MouseLeft => _lastMouseStates.left,
            StardewModdingAPI.SButton.MouseRight => _lastMouseStates.right,
            StardewModdingAPI.SButton.MouseMiddle => _lastMouseStates.middle,
            _ => MouseStateEx.Default
         };

         if (!last_mouse_state.Equals(ICustomMenu.MouseStateEx.Default)) { // is a mouse activity
            if (last_mouse_state.ButtonState == StardewModdingAPI.SButtonState.Pressed) { // is a click
               // Call OnMouseClick for items
               this.MenuItems.ForEach((i) =>
               {
                  if (!i.IsVisible)
                     return;

                  var mousePressedPos = last_mouse_state.Pos;

                  if (i.Bounds.Contains(mousePressedPos.AsXNAPoint()) /* user pressed while hovering item */
                     && i.Bounds.Contains(e.Cursor.ScreenPixels.AsXNAPoint()) /* user released while hovering item */) {
                     last_mouse_state.Pos = e.Cursor.ScreenPixels;

                     if (i.RaiseMouseClickEventOnRelease)
                        i.OnMouseClick(last_mouse_state);

                     // Call OnMouseClick for this item's components
                     i.Components.ForEach((c) =>
                     {
                        if (!c.IsVisible)
                           return;

                        if (c.Bounds.Contains(mousePressedPos.AsXNAPoint())
                           && c.Bounds.Contains(e.Cursor.ScreenPixels.AsXNAPoint())) {
                           if (c.RaiseMouseClickEventOnRelease)
                              c.OnMouseClick(last_mouse_state);
                        }
                     });
                  }
               });
            }

            last_mouse_state.ButtonState = StardewModdingAPI.SButtonState.Released;
         }
      }

      #endregion

      #endregion

      // Protected:
      #region Protected

      protected Color colMenuBackground;

      // Input event handlers:
      #region Input event handlers

      /// <summary>
      /// Base implementation informs this menu's items if '<see cref="IsVisible"/>' is true.
      /// </summary>
      /// <returns>Whether this menu's items are informed of this event.</returns>
      protected virtual InformStatus onMouseClick(MouseStateEx mouseState) {
         return this.IsVisible ? InformStatus.InformItems : InformStatus.DontInformItems;
      }

      /// <summary>
      /// Base implementation informs this menu's items if '<see cref="IsVisible"/>' is true.
      /// </summary>
      /// <returns>Whether this menu's items are informed of this event.</returns>
      protected virtual InformStatus onCursorMoved(StardewModdingAPI.Events.CursorMovedEventArgs e) {
         return this.IsVisible ? InformStatus.InformItems : InformStatus.DontInformItems;
      }

      /// <summary>
      /// Base implementation informs this menu's items if '<see cref="IsVisible"/>' is true.
      /// </summary>
      /// <returns>Whether this menu's items are informed of this event.</returns>
      protected virtual InformStatus onButtonPressed(StardewModdingAPI.Events.ButtonPressedEventArgs e) {
         return this.IsVisible ? InformStatus.InformItems : InformStatus.DontInformItems;
      }

      /// <summary>
      /// Base implementation informs this menu's items if '<see cref="IsVisible"/>' is true.
      /// </summary>
      /// <returns>Whether this menu's items are informed of this event.</returns>
      protected virtual InformStatus onButtonReleased(StardewModdingAPI.Events.ButtonReleasedEventArgs e) {
         return this.IsVisible ? InformStatus.InformItems : InformStatus.DontInformItems;
      }

      #endregion

      #endregion

      // Public:
      #region Public

      public Rectangle Bounds {
         get {
            return new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
         }
         private set {
            this.xPositionOnScreen = value.X;
            this.yPositionOnScreen = value.Y;
            this.width = value.Width;
            this.height = value.Height;
         }
      }

      public Boolean IsVisible { get; protected set; }

      public List<ICustomMenuItem> MenuItems { get; protected set; }

      /// <summary>
      /// The inner content rectangle of the game's dialogue box backgroud.
      /// </summary>
      public Rectangle SafeContentRegion {
         get {
            var bounds = this.Bounds;
            return new Rectangle(
               bounds.X + 36,
               bounds.Y + IClickableMenu.spaceToClearTopBorder + 4,
               bounds.Width - IClickableMenu.borderWidth - 32,
               bounds.Height - IClickableMenu.spaceToClearTopBorder - 40
            );
         }
      }

      // Virtuals:
      #region Virtuals

      /// <summary>
      /// Base implementation sets '<see cref="IsVisible"/>' to '<paramref name="isVisible"/>'.
      /// </summary>
      /// <param name="isVisible">Whether this menu should be visible.</param>
      public virtual void SetVisible(Boolean isVisible) {
         this.IsVisible = isVisible;
      }

      /// <summary>
      /// Base implementation calls '<see cref="ICustomMenu.Dispose()"/>'.
      /// </summary>
      public virtual void OnClose() {
         this.Dispose();
      }

      #endregion

      // Overrides:
      #region Overrides

      /// <summary>
      /// Base implementation draws the dialogue box background if '<see cref="IsVisible"/>' is true and likewise with this menu's items.
      /// </summary>
      /// <remarks>The dialogue box is drawn using '<see cref="ICustomMenu.Bounds"/>' and '<see cref="ICustomMenu.colMenuBackground"/>'.</remarks>
      public override void draw(SpriteBatch b) {
         if (!this.IsVisible)
            return;

         var bounds = this.Bounds;

         // dialogue box background, provided by the game
         StardewValley.Game1.drawDialogueBox(
            bounds.X, bounds.Y, bounds.Width, bounds.Height,
            false, true, null, false, true, this.colMenuBackground.R, this.colMenuBackground.G, this.colMenuBackground.B);

         this.MenuItems.ForEach((i) =>
         {
            if (i.IsVisible)
               i.Draw(b);
         });

         // draw mouse
         StardewValley.Game1.mouseCursorTransparency = 1.0f;
         base.drawMouse(b);
      }

      /// <summary>
      /// Base implementation calls '<see cref="draw(SpriteBatch)"/>' ignoring the params.
      /// </summary>
      public override void draw(SpriteBatch b, Int32 red = -1, Int32 green = -1, Int32 blue = -1) {
         this.draw(b);
      }

      /// <summary>
      /// <para>Base implementation:</para>
      /// <para>1. Calls base ('<see cref="IClickableMenu.gameWindowSizeChanged(Rectangle, Rectangle)"/>').</para>
      /// <para>2. Informs this menu's items.</para>
      /// </summary>
      public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
         // let game handle resize
         base.gameWindowSizeChanged(oldBounds, newBounds);
         // inform items
         this.MenuItems.ForEach((i) => i.OnGameWindowSizeChanged(oldBounds, newBounds));
      }

      #endregion


      // Statics:
      #region Statics

      public static void DrawHoverText(SpriteBatch b, SpriteFont font, String text, (Int32 left, Int32 top, Int32 right, Int32 bottom) textPadding = default, Color backgroundColour = default, Color textColour = default, Single alpha = 1.0f) {
         if (String.IsNullOrWhiteSpace(text))
            return;

         if (backgroundColour == default)
            backgroundColour = Color.White;
         if (textColour == default)
            textColour = Color.Black;

         var mouse_pos = StardewValley.Game1.getMousePosition();
         var text_size = font.MeasureString(text).AsXNAPoint();
         var box_rect = new Rectangle(mouse_pos.X + 32,
                                      mouse_pos.Y + 32,
                                      text_size.X + 22 + textPadding.left + textPadding.right,
                                      text_size.Y + 12 + textPadding.top + textPadding.bottom);

         // clamp to game viewport
         var safe_area = StardewValley.Utility.getSafeArea();
         if (box_rect.X + box_rect.Width > safe_area.Right)
            box_rect.X = safe_area.Right - box_rect.Width;
         if (box_rect.Y + box_rect.Height > safe_area.Bottom)
            box_rect.Y = safe_area.Bottom - box_rect.Height;

         IClickableMenu.drawTextureBox(b,
                                       StardewValley.Game1.uncoloredMenuTexture,
                                       new Rectangle(0, 256, 60, 60),
                                       box_rect.X,
                                       box_rect.Y,
                                       box_rect.Width,
                                       box_rect.Height,
                                       backgroundColour * alpha, 1.0f, false);

         StardewValley.Utility.drawTextWithShadow(b, text, font, new Vector2(box_rect.X + 12.0f + textPadding.left, box_rect.Y + 8.0f + textPadding.top), textColour, 1.0f, -1.0f, 2, 2, 0.0f, 0);
      }

      #endregion

      #endregion

      // Constructors:
      #region Constructors

      public ICustomMenu(Color menuBackgroundColour, Rectangle bounds) : base(bounds.X, bounds.Y, bounds.Width, bounds.Height, false) {
         _lastMouseStates = (
            left: new MouseStateEx(StardewModdingAPI.SButton.MouseLeft),
            right: new MouseStateEx(StardewModdingAPI.SButton.MouseRight),
            middle: new MouseStateEx(StardewModdingAPI.SButton.MouseMiddle));

         this.colMenuBackground = menuBackgroundColour;

         this.MenuItems = new List<ICustomMenuItem>();
         this.SetVisible(true);

         // register menu exit function
         this.exitFunction = this.OnClose;

         // register SMAPI events
         GlobalVars.SMAPIHelper.Events.Input.CursorMoved += _eventCursorMoved;
         GlobalVars.SMAPIHelper.Events.Input.ButtonPressed += _eventButtonPressed;
         GlobalVars.SMAPIHelper.Events.Input.ButtonReleased += _eventButtonReleased;
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
         this.MenuItems.ForEach((i) => i.Dispose());
         this.MenuItems.Clear();

         // unregister SMAPI events
         GlobalVars.SMAPIHelper.Events.Input.CursorMoved -= _eventCursorMoved;
         GlobalVars.SMAPIHelper.Events.Input.ButtonPressed -= _eventButtonPressed;
         GlobalVars.SMAPIHelper.Events.Input.ButtonReleased -= _eventButtonReleased;
      }

      #endregion
   }
}
