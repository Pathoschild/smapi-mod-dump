using Microsoft.Xna.Framework;

namespace Igorious.StardewValley.ShowcaseMod.Core.Layouts
{
    public interface IShowcaseLayout
    {
        Vector2? GetItemViewRelativePosition(int i, int j);
    }
}