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

using StardewValley;

using Object = StardewValley.Object;

namespace ChestEx.Types.BaseTypes {
  public class SVObjectInCustomMenu : CustomMenuItem.BasicComponent {
    // Protected:
  #region Protected

    protected Boolean mObjectDrawShadow;
    protected Single  mObjectScale;

    // Virtuals:
  #region Virtuals

    /// <summary>
    /// Base implementation draws the hover text via '<see cref="CustomMenu"/>.DrawHoverText()'.
    /// </summary>
    protected virtual void DrawHoverText(SpriteBatch b) {
      CustomMenu.DrawHoverText(b,
                               Game1.smallFont,
                               this.mHoverText,
                               backgroundColour: this.mHostMenuItem.mColours.mBackgroundColour,
                               textColour: this.mHostMenuItem.mColours.mForegroundColour);
    }

    /// <summary>
    /// Base implementation draws the object via '<see cref="StardewValley.Item.drawInMenu(SpriteBatch, Vector2, Single, Single, Single, StardewValley.StackDrawType, Color, Boolean)"/>'.
    /// </summary>
    protected virtual void DrawObject(SpriteBatch b) {
      this.mSVObject.drawInMenu(b,
                                this.mBounds.ExtractXYAsXNAVector2(),
                                this.mObjectScale,
                                this.mTextureTintColourCurrent.A / 255.0f,
                                1.0f,
                                StackDrawType.Hide,
                                this.mTextureTintColourCurrent,
                                this.mObjectDrawShadow);
    }

    /// <summary>Base implementation does nothing.</summary>
    protected virtual void PlayObjectAnimation() { }

    /// <summary>Base implementation does nothing.</summary>
    protected virtual void RevertObjectAnimation() { }

  #endregion

  #endregion

    // Public:
  #region Public

    public Object mSVObject { get; set; }

    public T GetSVObjectAs<T>() where T : Object { return this.mSVObject as T; }

    // Overrides:
  #region Overrides

    /// <summary>
    /// <para>Returns if this object is not visible.</para>
    /// <para>1. Draws object via '<see cref="DrawObject"/>'.</para>
    /// <para>2. Draws hover text.</para>
    /// </summary>
    /// <param name="b"></param>
    public override void Draw(SpriteBatch b) {
      if (!this.mIsVisible) return;

      this.DrawObject(b);

      if (this.mCursorStatus != CursorStatus.None && !String.IsNullOrWhiteSpace(this.mHoverText)) this.DrawHoverText(b);
    }

    /// <summary>
    /// Calls '<see cref="PlayObjectAnimation"/>' if cursor has activity on this object, '<see cref="RevertObjectAnimation"/>' if not.
    /// </summary>
    public override void OnCursorMoved(Vector2 cursorPos) {
      base.OnCursorMoved(cursorPos);

      if (this.mCursorStatus != CursorStatus.None)
        this.PlayObjectAnimation();
      else
        this.RevertObjectAnimation();
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public SVObjectInCustomMenu(CustomMenuItem hostMenuItem, Rectangle bounds, Object svObject, Boolean svObjectDrawShadow = false,
                                Single         svObjectScale = -1.0f, String componentName = "", EventHandler<CustomMenu.MouseStateEx> onMouseClick = null, String hoverText = "",
                                Colours        textureTintColours = null) : base(hostMenuItem,
                                                                                 bounds,
                                                                                 true,
                                                                                 componentName,
                                                                                 onMouseClick,
                                                                                 hoverText,
                                                                                 textureTintColours) {
      this.mObjectDrawShadow = svObjectDrawShadow;
      this.mObjectScale      = svObjectScale;

      this.mSVObject = svObject;
    }

  #endregion
  }
}
