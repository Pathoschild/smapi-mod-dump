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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ChestEx.LanguageExtensions;

using JetBrains.Annotations;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley.Menus;

namespace ChestEx.Types.BaseTypes {
  public class CustomClickableMenu : IClickableMenu,
                                     ICustomComponent {
    // Protected:
  #region Protected

    protected List<ICustomComponent> mComponents { get; } = new();

    /// <summary>Called when the current menu is being closed.</summary>
    /// <remarks>
    /// <see cref="CustomClickableMenu"/> implementation calls <see cref="Dispose"/>.
    /// </remarks>
    protected virtual void OnClose() { this.Dispose(); }

  #endregion

    // Public:
  #region Public

    public Rectangle mBounds {
      get => new(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
      protected set {
        this.xPositionOnScreen = value.X;
        this.yPositionOnScreen = value.Y;
        this.width             = value.Width;
        this.height            = value.Height;
      }
    }
    public ICustomComponent.IData mData      { get; }
    public Boolean                mIsVisible { get; private set; } = true;

    // Virtuals:
  #region Virtuals

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks><see cref="CustomClickableMenu"/> implementation also passes this to <see cref="mComponents"/>.</remarks>
    public virtual void SetEnabled(Boolean isEnabled) {
      this.mData.SetEnabled(isEnabled);
      this.mComponents.ForEach(c => c.SetEnabled(isEnabled));
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks><see cref="CustomClickableMenu"/> implementation also passes this to <see cref="mComponents"/>.</remarks>
    public virtual void SetVisible(Boolean isVisible) {
      this.mIsVisible = isVisible;
      this.mComponents.ForEach(c => c.SetVisible(isVisible));
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableMenu"/> implementation:
    /// <para>1. Draws a menu background with <see cref="Colours.mBackgroundColour"/> colouring.</para>
    /// <para>2. Calls <see cref="ICustomComponent.Draw(SpriteBatch)"/> on <see cref="mComponents"/>.</para>
    /// </remarks>
    public virtual void Draw(SpriteBatch spriteBatch) {
      spriteBatch.DrawTextureBox(this.mBounds.GetTextureBoxRectangle(0), this.mData.mColours);
      this.mComponents.ForEach(c => {
        if (c.mIsVisible) c.Draw(spriteBatch);
      });
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableMenu"/> implementation also passes this to <see cref="mComponents"/>.
    /// </remarks>
    public virtual void OnButtonPressed(InputStateEx inputState) {
      // Inform items of ButtonPressed event if they are interactable
      this.mComponents.IgnoreExceptions(o => o.ForEach(i => {
        if (!i.mData.mIsEnabled || !i.mIsVisible) return;

        // Update cursor status of item
        if (inputState.mButton is SButton.MouseLeft or SButton.MouseMiddle or SButton.MouseRight) i.mData.UpdateCursorStatus(i.mBounds.Contains(inputState.mCursorPos), inputState);
        // Inform
        i.OnButtonPressed(inputState);
      }));
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableMenu"/> implementation also passes this to <see cref="mComponents"/>.
    /// </remarks>
    public virtual void OnButtonReleased(InputStateEx inputState) {
      // Inform items of ButtonReleased event if they are interactable
      this.mComponents.IgnoreExceptions(o => o.ForEach(i => {
        if (!i.mData.mIsEnabled || !i.mIsVisible) return;

        // Update cursor status of item
        if (inputState.mButton is SButton.MouseLeft or SButton.MouseMiddle or SButton.MouseRight) i.mData.UpdateCursorStatus(i.mBounds.Contains(inputState.mCursorPos), inputState);
        // Inform
        i.OnButtonReleased(inputState);
      }));
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableMenu"/> implementation also passes this to <see cref="mComponents"/>.
    /// </remarks>
    public virtual void OnCursorMoved(Vector2 cursorPos) {
      // Inform items of CursorMoved event if they are interactable
      this.mComponents.IgnoreExceptions(o => o.ForEach(i => {
        if (!i.mData.mIsEnabled || !i.mIsVisible) return;

        // Update cursor status of item
        i.mData.UpdateCursorStatus(i.mBounds.Contains(cursorPos));
        // Inform
        i.OnCursorMoved(cursorPos);
      }));
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableMenu"/> implementation also passes this to <see cref="mComponents"/>.
    /// </remarks>
    public virtual void OnMouseClick(InputStateEx inputState) {
      this.mComponents.IgnoreExceptions(o => o.ForEach(i => {
        if (!i.mData.mIsEnabled || !i.mIsVisible) return;
        if (!i.mBounds.Contains(inputState.mLastCursorPos) || !i.mBounds.Contains(inputState.mCursorPos)) return;

        // Update cursor status of item
        i.mData.UpdateCursorStatus(true, inputState);
        // Inform
        i.OnMouseClick(inputState);
      }));
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableMenu"/> implementation also passes this to <see cref="mComponents"/>.
    /// </remarks>
    public virtual void OnGameTick() {
      this.mComponents.IgnoreExceptions(o => o.ForEach(i => {
        if (i.mData.mIsEnabled && i.mIsVisible) i.OnGameTick();
      }));
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableMenu"/> implementation also passes this to <see cref="mComponents"/>.
    /// </remarks>
    public virtual void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
      this.mComponents.IgnoreExceptions(o => o.ForEach(i => {
        if (i.mData.mIsEnabled && i.mIsVisible) i.OnGameWindowSizeChanged(oldBounds, newBounds);
      }));
    }

  #endregion

    // Shadowed:
  #region Shadowed

    /// <summary>Calls <see cref="Draw(SpriteBatch)"/>.</summary>
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification                  = "Game code")]
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter", Justification = "Game code")]
    public new void draw(SpriteBatch b, Int32 red = -1, Int32 green = -1, Int32 blue = -1) { this.Draw(b); }

    /// <summary>Calls <see cref="Draw(SpriteBatch)"/>.</summary>
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Game code")]
    public new void draw(SpriteBatch b) { this.Draw(b); }

  #endregion

    // Overrides:
  #region Overrides

    /// <summary>Makes sure <see cref="IClickableMenu.upperRightCloseButton"/> is not drawn.</summary>
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Game code")]
    public override Boolean shouldDrawCloseButton() { return false; }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public CustomClickableMenu(Rectangle bounds, Colours colours)
      : base(bounds.X, bounds.Y, bounds.Width, bounds.Height) {
      this.exitFunction = this.OnClose;
      this.mData        = new CustomComponentData(colours, 0.86f + bounds.Y / 20000.0f, String.Empty);
    }

    public CustomClickableMenu(Rectangle bounds)
      : this(bounds, Colours.gLight) { }

  #endregion

    // IDisposable:
  #region IDisposable

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableMenu"/> implementation:
    /// <para>1. Calls <see cref="ICustomComponent.IData.SetEnabled(Boolean)"/> with 'false'.</para>
    /// <para>2. Calls <see cref="SetVisible(Boolean)"/> with 'false'.</para>
    /// <para>3. Calls <see cref="Dispose"/> on <see cref="mComponents"/>.</para>
    /// </remarks>
    public virtual void Dispose() {
      this.SetEnabled(false);
      this.SetVisible(false);

      this.mComponents.ForEach(c => c.Dispose());
    }

  #endregion
  }
}
