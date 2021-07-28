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
using System.Linq;

using ChestEx.Types.BaseTypes;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley.Objects;

namespace ChestEx.Types.CustomTypes.ChestExMenu.Items {
  public class ChestSettings : CustomClickableMenu {
  #region Private

    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
    private readonly CustomTextBox nameTextBox;
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
    private readonly CustomTextBox descriptionTextBox;
    private readonly CustomNumericUpDownBox rowsNumericUpDown;
    private readonly CustomNumericUpDownBox columnsNumericUpDown;
    private readonly CustomButton           applyButton;

  #endregion

    // Public:
  #region Public

    public override void SetEnabled(Boolean isEnabled) {
      this.mData.SetEnabled(isEnabled);
      this.mComponents.ForEach(c => {
        if (c != this.applyButton) c.SetEnabled(isEnabled);
      });
    }

  #endregion

    // Constructors:
  #region Constructors

    public ChestSettings(Rectangle bounds, Colours colours, Chest targetChest)
      : base(bounds, colours) {
      if (targetChest is null) return;

      this.nameTextBox = new CustomTextBox(new Rectangle(bounds.X + 12, bounds.Y + 12, bounds.Width - 20, -1),
                                           colours * 1.5f,
                                           "Name:",
                                           text: targetChest.GetCustomConfigName(),
                                           inputFilterFunction: CustomTextBox.gSVAcceptedInput,
                                           onTextChangedHandler: text => {
                                             targetChest.SetCustomConfigName(String.IsNullOrWhiteSpace(text) ? CustomChestConfig.CONST_DEFAULT_NAME : text);
                                           });

      this.descriptionTextBox = new CustomTextBox(new Rectangle(bounds.X + 12, this.nameTextBox.mBounds.Bottom + 12, bounds.Width - 20, -1),
                                                  colours * 1.5f,
                                                  "Description:",
                                                  text: targetChest.GetCustomConfigDescription(),
                                                  inputFilterFunction: CustomTextBox.gSVAcceptedInput,
                                                  onTextChangedHandler: targetChest.SetCustomConfigDescription);

      if (Context.IsMainPlayer) {
        this.rowsNumericUpDown = new CustomNumericUpDownBox(new Rectangle(bounds.X + 12, this.descriptionTextBox.mBounds.Bottom + 12, -1, -1),
                                                            colours * 1.5f,
                                                            "Rows:",
                                                            1,
                                                            24,
                                                            Config.Get().mRows,
                                                            _ => { this.applyButton.SetEnabled(true); });

        this.columnsNumericUpDown = new CustomNumericUpDownBox(new Rectangle(this.rowsNumericUpDown.mBounds.Right + 12, this.rowsNumericUpDown.mBounds.Y, -1, -1),
                                                               colours * 1.5f,
                                                               "Columns:",
                                                               1,
                                                               24,
                                                               Config.Get().mColumns,
                                                               _ => { this.applyButton.SetEnabled(true); });

        this.applyButton = new CustomButton(new Rectangle(bounds.Right - 92, this.columnsNumericUpDown.mBounds.Y + 4, 92 - 10, this.columnsNumericUpDown.mBounds.Height),
                                            colours * 0.5f,
                                            "Apply",
                                            String.Empty,
                                            () => {
                                              Config.Get().mRows    = this.rowsNumericUpDown.mValue;
                                              Config.Get().mColumns = this.columnsNumericUpDown.mValue;
                                              Config.Save();
                                              targetChest.ShowMenu();
                                            });
        this.applyButton.SetEnabled(false);
      }

      this.mComponents.AddRange(new List<ICustomComponent> { this.applyButton, this.columnsNumericUpDown, this.rowsNumericUpDown, this.descriptionTextBox, this.nameTextBox }
                                  .Where(c => c is not null));
    }

  #endregion
  }
}
