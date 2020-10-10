/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GilarF/SVM
**
*************************************************/

using System.Collections.Generic;
using ModSettingsTab.Framework;
using ModSettingsTab.Framework.Components;

namespace ModSettingsTab.Menu
{
    public class ModManagerPage : BaseOptionsModPage
    {
        public ModManagerPage(int x, int y, int width, int height) : base(x, y, width, height)
        {
            LoadOptions();
            ModManager.UpdateMod += LoadOptions;
            FilterTextBox = new FilterTextBox(this, FilterTextBox.FilterType.Options,xPositionOnScreen + width / 2 + 112, yPositionOnScreen + 40);
        }

        private void LoadOptions()
        {
            var list = new List<OptionsElement> {new ModManagerToggleDd()};
            list.AddRange(ModManager.Options);
            Options = list;
        }
    }
}