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

using ChestEx.Types.BaseTypes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley.Objects;

namespace ChestEx.Types.CustomTypes.ChestExMenu.Items {
  public partial class ChestConfigPanel {
    private class ChestSettings : BasicComponent {
    #region Private

      private readonly CustomTextBox          nameTextBox;
      private readonly CustomTextBox          descriptionTextBox;
      private readonly CustomNumericUpDownBox rowsNumericUpDown;
      private readonly CustomNumericUpDownBox columnsNumericUpDown;
      private readonly CustomButton           applyButton;

    #endregion

    #region Public

      public override void Draw(SpriteBatch b) {
        if (!this.mIsVisible) return;

        this.nameTextBox?.Draw(b);
        this.descriptionTextBox?.Draw(b);
        this.rowsNumericUpDown?.Draw(b);
        this.columnsNumericUpDown?.Draw(b);
        this.applyButton?.draw(b);
      }

      public override void OnCursorMoved(Vector2 cursorPos) {
        base.OnCursorMoved(cursorPos);

        if (this.mIsVisible) this.applyButton?.OnCursorMoved(cursorPos);
      }

      public override void OnButtonReleased(ButtonReleasedEventArgs e) {
        base.OnButtonReleased(e);

        if (this.mIsVisible) this.applyButton?.OnButtonReleased(e);
      }

      public override void OnButtonPressed(ButtonPressedEventArgs e) {
        base.OnButtonPressed(e);

        if (!this.mIsVisible) return;
        this.nameTextBox?.OnButtonPressed(e);
        this.descriptionTextBox?.OnButtonPressed(e);
        this.rowsNumericUpDown?.OnButtonPressed(e);
        this.columnsNumericUpDown?.OnButtonPressed(e);
        this.applyButton?.OnButtonPressed(e);
      }

      public override void SetVisible(Boolean isVisible) {
        base.SetVisible(isVisible);
        this.nameTextBox?.SetVisible(isVisible);
        this.descriptionTextBox?.SetVisible(isVisible);
        this.rowsNumericUpDown?.SetVisible(isVisible);
        this.columnsNumericUpDown?.SetVisible(isVisible);
        this.applyButton?.SetVisible(isVisible);
      }

    #endregion

      // Constructors:
    #region Constructors

      public ChestSettings(CustomItemGrabMenuItem hostMenuItem, Rectangle bounds, String componentName = "", EventHandler<CustomMenu.MouseStateEx> onMouseClick = null)
        : base(hostMenuItem, bounds, true, componentName, onMouseClick) {
        this.nameTextBox = new CustomTextBox(new Rectangle(bounds.X + 12, bounds.Y + 8, 394, -1),
                                             hostMenuItem.mColours.mForegroundColour,
                                             hostMenuItem.mColours.mBackgroundColour,
                                             hostMenuItem.mHostMenu.GetSourceAs<Chest>().GetCustomConfigName(),
                                             String.Empty,
                                             -1,
                                             "Name:",
                                             Color.Black,
                                             CustomTextBox.gSVAcceptedInput,
                                             _ => { this.applyButton.SetEnabled(true); });
        this.nameTextBox.SetVisible(false);
        this.descriptionTextBox = new CustomTextBox(new Rectangle(this.nameTextBox.mBounds.X, this.nameTextBox.mBounds.Bottom + 12, 330, -1),
                                                    hostMenuItem.mColours.mForegroundColour,
                                                    hostMenuItem.mColours.mBackgroundColour,
                                                    hostMenuItem.mHostMenu.GetSourceAs<Chest>().GetCustomConfigDescription(),
                                                    String.Empty,
                                                    -1,
                                                    "Description:",
                                                    Color.Black,
                                                    CustomTextBox.gSVAcceptedInput,
                                                    _ => { this.applyButton.SetEnabled(true); });
        this.descriptionTextBox.SetVisible(false);

        if (Context.IsMainPlayer) {
          this.rowsNumericUpDown = new CustomNumericUpDownBox(new Point(this.descriptionTextBox.mBounds.X, this.descriptionTextBox.mBounds.Bottom + 12),
                                                              hostMenuItem.mColours.mForegroundColour,
                                                              hostMenuItem.mColours.mBackgroundColour,
                                                              1,
                                                              24,
                                                              Config.Get().mRows,
                                                              "Rows:",
                                                              Color.Black,
                                                              _ => { this.applyButton.SetEnabled(true); });
          this.rowsNumericUpDown.SetVisible(false);

          this.columnsNumericUpDown = new CustomNumericUpDownBox(new Point(this.rowsNumericUpDown.mBounds.Right + 12, this.rowsNumericUpDown.mBounds.Y),
                                                                 hostMenuItem.mColours.mForegroundColour,
                                                                 hostMenuItem.mColours.mBackgroundColour,
                                                                 1,
                                                                 24,
                                                                 Config.Get().mColumns,
                                                                 "Columns:",
                                                                 Color.Black,
                                                                 _ => { this.applyButton.SetEnabled(true); });
          this.columnsNumericUpDown.SetVisible(false);
        }

        this.applyButton = new CustomButton(new Rectangle(bounds.Right - 88, bounds.Bottom - 48, 80, 40),
                                            () => {
                                              // Update name
                                              hostMenuItem.mHostMenu.GetSourceAs<Chest>()
                                                          .SetCustomConfigName(String.IsNullOrWhiteSpace(this.nameTextBox.Text) ?
                                                                                 CustomChestConfig.CONST_DEFAULT_NAME :
                                                                                 this.nameTextBox.Text);
                                              // Update description
                                              if (!String.IsNullOrWhiteSpace(this.descriptionTextBox.Text))
                                                hostMenuItem.mHostMenu.GetSourceAs<Chest>().SetCustomConfigDescription(this.descriptionTextBox.Text);

                                              // Update Config
                                              Config.Get().mRows    = this.rowsNumericUpDown.mValue;
                                              Config.Get().mColumns = this.columnsNumericUpDown.mValue;
                                              Config.Save();
                                              hostMenuItem.mHostMenu.GetSourceAs<Chest>()?.ShowMenu();
                                            },
                                            "Apply");
        this.applyButton.SetEnabled(false);
        this.applyButton.SetVisible(false);
      }

    #endregion

    #region IDisposable

      public override void Dispose() {
        base.Dispose();

        this.rowsNumericUpDown?.Dispose();
        this.columnsNumericUpDown?.Dispose();
      }

    #endregion
    }
  }
}
