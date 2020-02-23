using ModSettingsTab.Framework;
using ModSettingsTab.Framework.Components;

namespace ModSettingsTab.Menu
{
    public class FavoriteOptionsModPage : BaseOptionsModPage
    {
        public FavoriteOptionsModPage(int x, int y, int width, int height, int id) : base(x, y, width, height)
        {
            Options = FavoriteData.ModList[id].Options;
            FilterTextBox = new FilterTextBox(this, FilterTextBox.FilterType.Options,xPositionOnScreen + width / 2 + 112, yPositionOnScreen + 40);
        }
    }
}