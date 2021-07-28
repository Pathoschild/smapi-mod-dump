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

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChestEx.Types.BaseTypes {
  public class ColouringMenuSwitcher : CustomClickableMenu {
    // Private:
  #region Private

    private readonly Action<String> onSwitchRequested;

    private readonly Dictionary<Int32, CustomButton> switcherButtons = new();
    private readonly Point                           switcherButtonSize;
    private readonly Int32                           switcherButtonsMaxAmount;

    private readonly Colours activeSwitcherButtonColours;
    private readonly Colours inactiveSwitcherButtonColours;

    private Int32 findNextId() {
      for (Int32 i = 0; i < this.switcherButtonsMaxAmount; i++) {
        if (!this.switcherButtons.ContainsKey(i)) return i;
      }

      return this.switcherButtonsMaxAmount;
    }

    private Rectangle getButtonBoundsFromId(Int32 idx) {
      return new(this.mBounds.X + 4, this.mBounds.Y + 6 + idx * (this.switcherButtonSize.Y + 6), this.switcherButtonSize.X, this.switcherButtonSize.Y);
    }

    private void refreshComponents() {
      this.setActiveSwitcher(0);

      this.mComponents.Clear();
      for (Int32 i = this.switcherButtonsMaxAmount; i >= 0; i--)
        if (this.switcherButtons.ContainsKey(i))
          this.mComponents.Add(this.switcherButtons[i]);
    }

    private void setActiveSwitcher(Int32 idx) {
      for (Int32 i = 0; i < this.switcherButtonsMaxAmount; i++) {
        if (this.switcherButtons.ContainsKey(i)) this.switcherButtons[i].mData.mColours = i == idx ? this.activeSwitcherButtonColours : this.inactiveSwitcherButtonColours;
      }
    }

  #endregion

    // Public:
  #region Public

    public void AddSwitcher(String name, Colours colours, String text, String hoverText = "",
                            Int32  idx = -1) {
      if (idx == -1)
        idx                                             = this.findNextId();
      else if (idx > this.switcherButtonsMaxAmount) idx = this.switcherButtonsMaxAmount;

      this.switcherButtons[idx] = new CustomButton(this.getButtonBoundsFromId(idx), colours, text, hoverText, () => this.ForceSwitch(idx, name));
      this.refreshComponents();
    }
    public void AddSwitcher(String name, Colours colours, Texture2D texture, String hoverText = "",
                            Int32  idx = -1) {
      if (idx == -1)
        idx                                             = this.findNextId();
      else if (idx > this.switcherButtonsMaxAmount) idx = this.switcherButtonsMaxAmount;

      this.switcherButtons[idx] = new CustomTextureButton(this.getButtonBoundsFromId(idx), colours, texture, hoverText, () => this.ForceSwitch(idx, name));
      this.refreshComponents();
    }

    public void ForceSwitch(Int32 idx, String requestedSwitcher) {
      this.setActiveSwitcher(idx);
      // Delegate to handler
      this.onSwitchRequested(requestedSwitcher);
    }

  #endregion

    // Constructors:
  #region Constructors

    public ColouringMenuSwitcher(Rectangle bounds, Colours colours, Action<String> onSwitchRequested)
      : base(bounds, colours) {
      this.onSwitchRequested = onSwitchRequested;

      this.switcherButtonSize       = new Point(bounds.Width - 8, bounds.Width);
      this.switcherButtonsMaxAmount = (bounds.Height - 6) / (bounds.Width + 6);

      this.activeSwitcherButtonColours   = Colours.GenerateFrom(Color.White);
      this.inactiveSwitcherButtonColours = Colours.GenerateFrom(colours.mActiveColour.MultAlpha(0.5f));
    }

  #endregion

    // IDisposable:
  #region IDisposable

    public override void Dispose() {
      base.Dispose();

      foreach (CustomButton btn in this.switcherButtons.Values) btn?.Dispose();
      this.switcherButtons.Clear();
    }

  #endregion
  }
}
