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
//    Copyright (c) 2022 Berkay Yigit <berkaytgy@gmail.com>
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

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;

namespace ChestEx.Types.BaseTypes {
  public class CustomComponentData : ICustomComponent.IData {
    public Boolean mIsActive { get; protected set; }
    public Boolean mIsCursorHovering { get; private set; }
    public Boolean mIsEnabled { get; private set; } = true;
    public Colours mColours { get; set; }
    public String mHoverText { get; set; }
    public Single mLayerDepth { get; }

    public Color mBGColour =>
      this.mIsEnabled ?
        this.mIsActive ? this.mColours.mActiveColour :
        this.mIsCursorHovering ? this.mColours.mHoverColour : this.mColours.mBackgroundColour :
        this.mColours.mBackgroundColour.MultRGB(0.5f);

    public Color mFGColour => this.mColours.mForegroundColour.MultRGB(this.mIsEnabled ? 1.0f : 0.5f);

    public void SetEnabled(Boolean isEnabled) { this.mIsEnabled = isEnabled; }

    public virtual void UpdateCursorStatus(Boolean isCursorInBounds, InputStateEx inputState = null) { this.mIsCursorHovering = isCursorInBounds; }

    public CustomComponentData(Colours colours, Single layerDepth, String hoverText) {
      this.mColours = colours;
      this.mHoverText = hoverText;
      this.mLayerDepth = layerDepth;
    }
  }
}
