using Igorious.StardewValley.ShowcaseMod.ModConfig;
using Microsoft.Xna.Framework;

namespace Igorious.StardewValley.ShowcaseMod.Core.Layouts
{
    internal class ShowcaseFixedLayout : ShowcaseGridLayoutBase
    {
        public ShowcaseFixedLayout(float scaleSize, Rectangle sourceRect, ItemGridProvider itemProvider, LayoutConfig config)
            : base(scaleSize, sourceRect, itemProvider, config) { }

        protected override int GetTopRow() => 0;

        protected override int GetBottomRow() => ItemProvider.Rows - 1;

        protected override int GetLeftColumn() => 0;

        protected override int GetRightColumn() => ItemProvider.Columns - 1;
    }
}