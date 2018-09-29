using System.Linq;
using Igorious.StardewValley.ShowcaseMod.ModConfig;
using Microsoft.Xna.Framework;

namespace Igorious.StardewValley.ShowcaseMod.Core.Layouts
{
    internal class ShowcaseAutoLayout : ShowcaseGridLayoutBase
    {
        public ShowcaseAutoLayout(float scaleSize, Rectangle sourceRect, ItemGridProvider itemProvider, LayoutConfig config) 
            : base(scaleSize, sourceRect, itemProvider, config) { }

        protected override int GetTopRow()
        {
            return Enumerable.Range(0, ItemProvider.Rows).FirstOrDefault(i => Enumerable.Range(0, ItemProvider.Columns).Any(j => ItemProvider[i, j] != null));
        }

        protected override int GetBottomRow()
        {
            return Enumerable.Range(0, ItemProvider.Rows).Reverse().FirstOrDefault(i => Enumerable.Range(0, ItemProvider.Columns).Any(j => ItemProvider[i, j] != null));
        }

        protected override int GetLeftColumn()
        {
            return Enumerable.Range(0, ItemProvider.Columns).FirstOrDefault(j => Enumerable.Range(0, ItemProvider.Rows).Any(i => ItemProvider[i, j] != null));
        }

        protected override int GetRightColumn()
        {
            return Enumerable.Range(0, ItemProvider.Columns).Reverse().FirstOrDefault(j => Enumerable.Range(0, ItemProvider.Rows).Any(i => ItemProvider[i, j] != null));
        }
    }
}