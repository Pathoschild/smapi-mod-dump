/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

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