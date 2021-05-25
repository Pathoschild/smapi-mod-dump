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
using System.Diagnostics.CodeAnalysis;

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;

namespace ChestEx.Types.BaseTypes {
  public class CustomButton : ClickableTextureComponent,
                              IDisposable {
    // Private:
  #region Private

    private Color currentTint;

  #endregion

    // Protected:
  #region Protected

    protected Texture2D mOverlayTexture;
    protected Boolean   mIsSelected;
    protected Action    mOnClickHandler;

  #endregion

    // Public:
  #region Public

    public Rectangle mBounds {
      get => this.bounds;
      set => this.bounds = value;
    }

    public Colours mColours   { get; set; }
    public String  mText      { get; }
    public Boolean mIsEnabled { get; private set; }
    public Boolean mIsVisible { get; private set; }

    // Virtuals:
  #region Virtuals

    /// <summary>
    /// Must be called on each event to function.
    /// </summary>
    /// <param name="cursorPos"><see cref="CustomMenu"/> adjusted cursor position.</param>
    public virtual void OnCursorMoved(Vector2 cursorPos) {
      if (!this.mIsVisible || !this.mIsEnabled) return;
      this.currentTint = this.mColours.mBackgroundColour;

      if (this.mBounds.Contains(cursorPos.AsXNAPoint())) this.currentTint = this.mIsSelected ? this.mColours.mPressedColour : this.mColours.mHoverColour;
    }

    /// <summary>
    /// Must be called on each event to function.
    /// </summary>
    /// <param name="e">SMAPI generated EventArgs.</param>
    public virtual void OnButtonPressed(ButtonPressedEventArgs e) {
      this.mIsSelected = false;

      if (!this.mIsVisible || !this.mIsEnabled || e.Button != SButton.MouseLeft) return;
      if (!this.mBounds.Contains(Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels).AsXNAPoint())) return;

      GlobalVars.gSMAPIHelper.Input.Suppress(e.Button);
      this.mIsSelected = true;
    }

    /// <summary>
    /// Must be called on each event to function.
    /// </summary>
    /// <param name="e">SMAPI generated EventArgs.</param>
    public virtual void OnButtonReleased(ButtonReleasedEventArgs e) {
      if (!this.mIsVisible || !this.mIsEnabled || e.Button != SButton.MouseLeft) return;
      if (!this.mBounds.Contains(Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels).AsXNAPoint())) return;

      if (this.mIsSelected) this.mOnClickHandler?.Invoke();
      this.mIsSelected = false;
    }

    /// <summary>
    /// Base implementation sets '<see cref="mIsEnabled"/>' to '<paramref name="isEnabled"/>'.
    /// </summary>
    /// <param name="isEnabled">Whether this item should be enabled.</param>
    public virtual void SetEnabled(Boolean isEnabled) { this.mIsEnabled = isEnabled; }

    /// <summary>
    /// Base implementation sets '<see cref="mIsVisible"/>' to '<paramref name="isVisible"/>'.
    /// </summary>
    /// <param name="isVisible">Whether this item should be visible.</param>
    public virtual void SetVisible(Boolean isVisible) { this.mIsVisible = this.visible = isVisible; }

  #endregion

    // Shadowed:
  #region Shadowed

    /// <summary>
    /// Draws '<see cref="ClickableTextureComponent.texture"/>' and '<see cref="mOverlayTexture"/>' if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public new void draw(SpriteBatch b, Color c, Single layerDepth, Int32 frameOffset = 0) {
      if (!this.mIsVisible || this.texture is null) return;
      Single mult = this.mIsEnabled ? 1.0f : 0.45f;

      b.Draw(this.texture,
             this.mBounds,
             this.texture.Bounds,
             c * mult,
             0.0f,
             Vector2.Zero,
             SpriteEffects.None,
             layerDepth);
      if (this.mOverlayTexture is not null) b.Draw(this.mOverlayTexture, this.mBounds, Color.White * mult);

      if (!String.IsNullOrWhiteSpace(this.mText)) {
        Vector2 text_size                  = Game1.smallFont.MeasureString(this.mText);
        if (this.mText == "+") text_size.Y = 26.0f;
        b.DrawStringEx(Game1.smallFont,
                       this.mText,
                       new Point((Int32)(this.mBounds.Center.X - text_size.X / 2.0f), (Int32)(this.mBounds.Center.Y - text_size.Y / 2.0f)),
                       c.ContrastColour() * mult,
                       drawShadow: true);
      }
    }

    /// <summary>
    /// Calls '<see cref="draw(SpriteBatch, Color, float, int)"/>' if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public new void draw(SpriteBatch b) {
      if (this.mIsVisible) this.draw(b, this.currentTint, 0.86f + this.mBounds.Y / 20000.0f);
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public CustomButton(Rectangle bounds, Color backgroundColour, Action onClickHandler, String text,
                        Texture2D overlayTexture) : base(bounds,
                                                         TexturePresets.gButtonBackgroundTexture,
                                                         TexturePresets.gButtonBackgroundTexture.Bounds,
                                                         Convert.ToSingle(bounds.Width) / TexturePresets.gButtonBackgroundTexture.Bounds.Width) {
      this.currentTint = backgroundColour;

      this.mColours        = Colours.GenerateFrom(backgroundColour);
      this.mOnClickHandler = onClickHandler;
      this.mText           = text;
      this.mOverlayTexture = overlayTexture;
    }

    public CustomButton(Rectangle bounds, Action onClickHandler = null, String text = "", Texture2D overlayTexture = null) : this(bounds,
                                                                                                                                  Color.White,
                                                                                                                                  onClickHandler,
                                                                                                                                  text,
                                                                                                                                  overlayTexture) { }

  #endregion

    // IDisposable:
  #region IDisposable

    /// <summary>
    /// <para>Base implementation:</para>
    /// <para>1. Calls '<see cref="SetVisible(Boolean)"/>' with 'false'.</para>
    /// <para>2. Disposes of <see cref="ClickableTextureComponent.texture"/>.</para>
    /// </summary>
    public virtual void Dispose() {
      // hide
      this.SetVisible(false);
      // dispose of textures
      this.mOverlayTexture?.Dispose();
      this.texture?.Dispose();
    }

  #endregion
  }
}
