using System.Collections.Generic;
using System.Linq;
using ModSettingsTab.Framework;
using ModSettingsTab.Framework.Components;

namespace ModSettingsTab.Menu
{
    public class ModPackPage : BaseOptionsModPage
    {
        public ModPackPage(int x, int y, int width, int height, string modPack) : base(x, y, width, height)
        {
            var opt = new List<OptionsElement>();
            var mpOpt = ModManager.Options.Where(o => ((ModManagerToggle) o).ModPack.Exists(p => p == modPack)).ToList();
            opt.Add(new ModManagerHeading(modPack));
            opt.AddRange(mpOpt);
            Options = opt;
            FilterTextBox = new FilterTextBox(this, FilterTextBox.FilterType.Options,
                xPositionOnScreen + width / 2 + 112, yPositionOnScreen + 40);
        }
    }
}