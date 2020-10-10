/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

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