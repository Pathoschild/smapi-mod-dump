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

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace ChestEx.Types.BaseTypes {
  public partial class CustomMenuItem {
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

      private CursorStatus cursorStatus;
      private Boolean      cursorImmutable;

    #endregion

      // Protected:
    #region Protected

      protected String                                mHoverText;
      protected EventHandler<CustomMenu.MouseStateEx> mOnMouseClickEventHandler;

      protected CursorStatus mCursorStatus {
        get => this.cursorStatus;
        set {
          if (!this.cursorImmutable) this.cursorStatus = value;
        }
      }

      protected Color mTextureTintColourCurrent {
        get {
          return this.mCursorStatus switch {
            CursorStatus.None          => this.mTextureTintColours.mForegroundColour,
            CursorStatus.Hovering      => this.mTextureTintColours.mHoverColour,
            CursorStatus.LMousePressed => this.mTextureTintColours.mPressedColour,
            CursorStatus.RMousePressed => this.mTextureTintColours.mForegroundColour,
            CursorStatus.MMousePressed => this.mTextureTintColours.mForegroundColour,
            _                          => Color.Transparent
          };
        }
      }
      protected Colours mTextureTintColours;

    #endregion

      // Public:
    #region Public

      public CustomMenuItem mHostMenuItem { get; private set; }

      public Rectangle mBounds { get; protected set; }

      public Boolean mIsVisible { get; protected set; }

      public String mName { get; protected set; }

      public Boolean mRaiseMouseClickEventOnRelease { get; protected set; }

      // Virtuals:
    #region Virtuals

      /// <summary>
      /// Base implementation sets '<see cref="mIsVisible"/>' to '<paramref name="isVisible"/>'.
      /// </summary>
      /// <param name="isVisible">Whether this menu should be visible.</param>
      public virtual void SetVisible(Boolean isVisible) { this.mIsVisible = isVisible; }

      /// <summary>
      /// <para>Base implementation (if '<see cref="mIsVisible"/>' is true):</para>
      /// <para>1. Draws the hover text if needed.</para>
      /// </summary>
      public virtual void Draw(SpriteBatch b) {
        if (!this.mIsVisible) return;

        // draw hover text
        if (this.mCursorStatus != CursorStatus.None && !String.IsNullOrWhiteSpace(this.mHoverText))
          CustomMenu.DrawHoverText(b,
                                   Game1.smallFont,
                                   this.mHoverText,
                                   backgroundColour: this.mHostMenuItem.mColours.mBackgroundColour,
                                   textColour: this.mHostMenuItem.mColours.mForegroundColour);
      }

      /// <summary>Base implementation does nothing.</summary>
      public virtual void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) { }

      // Input event handlers:
    #region Input event handlers

      /// <summary>Base implementation updates the underlying cursor status.</summary>
      /// <remarks>MUST BE CALLED!</remarks>
      public virtual void OnCursorMoved(Vector2 cursorPos) {
        if (!this.mIsVisible) return;

        this.mCursorStatus = this.mBounds.Contains(cursorPos.AsXNAPoint()) ? CursorStatus.Hovering : CursorStatus.None;
      }

      /// <summary>Base implementation calls the supplied OnMouseClick event handler.</summary>
      public virtual void OnMouseClick(CustomMenu.MouseStateEx mouseState) { this.mOnMouseClickEventHandler?.Invoke(this, mouseState); }

      /// <summary>Base implementation updates the underlying cursor status.</summary>
      /// <remarks>MUST BE CALLED!</remarks>
      public virtual void OnButtonPressed(ButtonPressedEventArgs e) {
        if (!this.mIsVisible) return;

        Vector2 correct_pos = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);

        // check if within bounds
        if (this.mBounds.Contains(correct_pos.AsXNAPoint()) && e.Button == SButton.MouseLeft) {
          this.mCursorStatus   = CursorStatus.LMousePressed;
          this.cursorImmutable = true;
        }
      }

      /// <summary>Base implementation updates the underlying cursor status.</summary>
      /// <remarks>MUST BE CALLED!</remarks>
      public virtual void OnButtonReleased(ButtonReleasedEventArgs e) {
        if (!this.mIsVisible) return;

        Vector2 correct_pos = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);

        if (e.Button == SButton.MouseLeft) {
          this.cursorImmutable = false;
          this.mCursorStatus   = this.mBounds.Contains(correct_pos.AsXNAPoint()) ? CursorStatus.Hovering : CursorStatus.None;
        }
      }

    #endregion

    #endregion

    #endregion

      // Constructors:
    #region Constructors

      public BasicComponent(CustomMenuItem                        hostMenuItem,        Rectangle bounds,    Boolean raiseMouseClickEventOnRelease, String componentName,
                            EventHandler<CustomMenu.MouseStateEx> onMouseClickHandler, String    hoverText, Colours textureTintColours) {
        this.cursorImmutable = false;

        this.mCursorStatus             = CursorStatus.None;
        this.mHoverText                = hoverText;
        this.mOnMouseClickEventHandler = onMouseClickHandler;
        this.mTextureTintColours       = textureTintColours ?? Colours.gDefault;

        this.mBounds                        = bounds;
        this.mHostMenuItem                  = hostMenuItem;
        this.mName                          = componentName;
        this.mRaiseMouseClickEventOnRelease = raiseMouseClickEventOnRelease;
      }

      public BasicComponent(CustomMenuItem                        hostMenuItem,               Point  point, Boolean raiseMouseClickEventOnRelease = true, String componentName = "",
                            EventHandler<CustomMenu.MouseStateEx> onMouseClickHandler = null, String hoverText = "", Colours textureTintColours = null) : this(hostMenuItem,
                                                                                                                                                               new
                                                                                                                                                                 Rectangle(point.X,
                                                                                                                                                                           point.Y,
                                                                                                                                                                           -1,
                                                                                                                                                                           -1),
                                                                                                                                                               raiseMouseClickEventOnRelease,
                                                                                                                                                               componentName,
                                                                                                                                                               onMouseClickHandler,
                                                                                                                                                               hoverText,
                                                                                                                                                               textureTintColours) { }

    #endregion

      // IDisposable:
    #region IDisposable

      /// <summary>
      /// <para>Base implementation:</para>
      /// <para>1. Calls '<see cref="SetVisible(Boolean)"/>' with 'false'.</para>
      /// </summary>
      public virtual void Dispose() {
        // hide
        this.SetVisible(false);
      }

    #endregion
    }
  }
}
