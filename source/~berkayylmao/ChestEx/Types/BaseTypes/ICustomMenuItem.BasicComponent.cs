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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChestEx.Types.BaseTypes {
   public partial class ICustomMenuItem {
      public class BasicComponent : IDisposable {
         // Protected enums:
         #region Protected enums

         protected enum CursorStatus {
            None,
            Hovering,
            LMousePressed,
            RMousePressed,
            MMousePressed
         }

         #endregion

         // Private:
         #region Private

         private CursorStatus _cursorStatus;
         private Boolean _cursorImmutable;

         #endregion

         // Protected:
         #region Protected

         protected String hoverText;
         protected EventHandler<ICustomMenu.MouseStateEx> onMouseClickEventHandler;

         protected CursorStatus cursorStatus {
            get { return _cursorStatus; }
            set {
               if (!_cursorImmutable)
                  _cursorStatus = value;
            }
         }

         protected Color textureTintColourCurrent {
            get {
               return this.cursorStatus switch
               {
                  CursorStatus.None => this.textureTintColours.ForegroundColour,
                  CursorStatus.Hovering => this.textureTintColours.HoverColour,
                  CursorStatus.LMousePressed => this.textureTintColours.PressedColour,
                  CursorStatus.RMousePressed => this.textureTintColours.ForegroundColour,
                  CursorStatus.MMousePressed => this.textureTintColours.ForegroundColour,
                  _ => Color.Transparent
               };
            }
         }

         protected Texture2D texture;
         protected Colours textureTintColours;

         // Virtuals:
         #region Virtuals

         /// <summary>Base implementation returns '<see cref="texture"/>'.</summary>
         /// <returns>The texture to be drawn for this component.</returns>
         protected virtual Texture2D GetTexture(GraphicsDevice device) {
            return this.texture;
         }

         #endregion

         #endregion

         // Public:
         #region Public

         public ICustomMenuItem HostMenuItem { get; private set; }

         public Rectangle Bounds { get; protected set; }

         public Boolean IsVisible { get; protected set; }

         public String Name { get; protected set; }

         public Boolean RaiseMouseClickEventOnRelease { get; protected set; }

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
         /// <para>Base implementation (if '<see cref="IsVisible"/>' is true):</para>
         /// <para>1. Draws the returned texture from '<see cref="GetTexture(GraphicsDevice)"/>', applying '<see cref="textureTintColourCurrent"/>'.</para>
         /// <para>2. Draws the hover text if needed.</para>
         /// </summary>
         public virtual void Draw(SpriteBatch b) {
            if (!this.IsVisible)
               return;

            // draw texture
            if (this.GetTexture(b.GraphicsDevice) is Texture2D texture)
               b.Draw(texture, this.Bounds, this.textureTintColourCurrent);

            // draw hover text
            if (this.cursorStatus != CursorStatus.None && !String.IsNullOrWhiteSpace(this.hoverText))
               StardewValley.Menus.IClickableMenu.drawHoverText(b, this.hoverText, StardewValley.Game1.smallFont);
         }

         /// <summary>Base implementation disposes of '<see cref="texture"/>'.</summary>
         public virtual void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) { this.texture?.Dispose(); }

         // Input event handlers:
         #region Input event handlers

         /// <summary>Base implementation updates the underlying cursor status.</summary>
         /// <remarks>MUST BE CALLED!</remarks>
         public virtual void OnCursorMoved(StardewModdingAPI.Events.CursorMovedEventArgs e) {
            if (!this.IsVisible)
               return;

            // check if within bounds
            if (this.Bounds.Contains(new Point(Convert.ToInt32(e.NewPosition.ScreenPixels.X), Convert.ToInt32(e.NewPosition.ScreenPixels.Y))))
               this.cursorStatus = CursorStatus.Hovering;
            else
               this.cursorStatus = CursorStatus.None;
         }

         /// <summary>Base implementation calls the supplied OnMouseClick event handler.</summary>
         public virtual void OnMouseClick(ICustomMenu.MouseStateEx mouseState) {
            this.onMouseClickEventHandler?.Invoke(this, mouseState);
         }

         /// <summary>Base implementation updates the underlying cursor status.</summary>
         /// <remarks>MUST BE CALLED!</remarks>
         public virtual void OnButtonPressed(StardewModdingAPI.Events.ButtonPressedEventArgs e) {
            if (!this.IsVisible)
               return;

            // check if within bounds
            if (this.Bounds.Contains(new Point(Convert.ToInt32(e.Cursor.ScreenPixels.X), Convert.ToInt32(e.Cursor.ScreenPixels.Y)))
               && e.Button == StardewModdingAPI.SButton.MouseLeft) {
               this.cursorStatus = CursorStatus.LMousePressed;
               _cursorImmutable = true;
            }
         }

         /// <summary>Base implementation updates the underlying cursor status.</summary>
         /// <remarks>MUST BE CALLED!</remarks>
         public virtual void OnButtonReleased(StardewModdingAPI.Events.ButtonReleasedEventArgs e) {
            if (!this.IsVisible)
               return;

            if (e.Button == StardewModdingAPI.SButton.MouseLeft) {
               _cursorImmutable = false;
               this.cursorStatus = this.Bounds.Contains(new Point(Convert.ToInt32(e.Cursor.ScreenPixels.X), Convert.ToInt32(e.Cursor.ScreenPixels.Y))) ? CursorStatus.Hovering : CursorStatus.None;
            }
         }

         #endregion

         #endregion

         #endregion

         // Constructors:
         #region Constructors

         public BasicComponent(ICustomMenuItem hostMenuItem, Rectangle bounds, Boolean raiseMouseClickEventOnRelease, String componentName, EventHandler<ICustomMenu.MouseStateEx> onMouseClickHandler, String hoverText, Colours textureTintColours) {
            _cursorImmutable = false;

            this.cursorStatus = CursorStatus.None;
            this.hoverText = hoverText;
            this.onMouseClickEventHandler = onMouseClickHandler;
            this.texture = null;
            this.textureTintColours = textureTintColours ?? Colours.Default;

            this.Bounds = bounds;
            this.HostMenuItem = hostMenuItem;
            this.Name = componentName;
            this.RaiseMouseClickEventOnRelease = raiseMouseClickEventOnRelease;

            this.SetVisible(true);
         }

         public BasicComponent(ICustomMenuItem hostMenuItem, Point point, Boolean raiseMouseClickEventOnRelease = true, String componentName = "", EventHandler<ICustomMenu.MouseStateEx> onMouseClickHandler = null, String hoverText = "", Colours textureTintColours = null)
            : this(hostMenuItem, new Rectangle(point.X, point.Y, -1, -1), raiseMouseClickEventOnRelease, componentName, onMouseClickHandler, hoverText, textureTintColours) { }

         #endregion

         // IDisposable:
         #region IDisposable

         /// <summary>
         /// <para>Base implementation:</para>
         /// <para>1. Calls '<see cref="SetVisible(Boolean)"/>' with 'false'.</para>
         /// <para>2. Disposes of '<see cref="texture"/>'.</para>
         /// </summary>
         public virtual void Dispose() {
            // hide
            this.SetVisible(false);
            // dispose of texture
            this.texture?.Dispose();
         }

         #endregion
      }
   }
}
