using ModSettingsTab.Framework.Components;

namespace ModSettingsTab.Menu
{
    public class OptionsModPage : BaseOptionsModPage
    {
        public OptionsModPage(int x, int y, int width, int height) : base(x, y, width, height)
        {
            Options = ModData.Options;
            FilterTextBox = new FilterTextBox(this, FilterTextBox.FilterType.Mod,xPositionOnScreen + width / 2 + 112, yPositionOnScreen + 40);
        }
    }
}