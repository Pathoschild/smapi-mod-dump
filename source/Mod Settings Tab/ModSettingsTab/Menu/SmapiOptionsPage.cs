using ModSettingsTab.Framework.Components;

namespace ModSettingsTab.Menu
{
    public class SmapiOptionsPage : BaseOptionsModPage
    {
        public SmapiOptionsPage(int x, int y, int width, int height) : base(x, y, width, height)
        {
            Options = ModData.SMAPI.Options;
            FilterTextBox = new FilterTextBox(this, FilterTextBox.FilterType.Options,xPositionOnScreen + width / 2 + 112, yPositionOnScreen + 40);
        }
    }
}