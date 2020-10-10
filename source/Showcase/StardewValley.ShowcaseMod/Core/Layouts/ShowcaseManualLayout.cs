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
using StardewValley;

namespace Igorious.StardewValley.ShowcaseMod.Core.Layouts
{
    public class ShowcaseManualLayout : IShowcaseLayout
    {
        protected const int SpriteSheetTileSize = Object.spriteSheetTileSize;

        private float ScaleSize { get; }
        private LayoutConfig Config { get; }

        public ShowcaseManualLayout(float scaleSize, LayoutConfig config)
        {
            ScaleSize = scaleSize;
            Config = config;
        }

        public Vector2? GetItemViewRelativePosition(int i, int j)
        {
            var index = i * Config.Columns + j;
            if (index >= Config.Positions.Count) return null;
            var position = Config.Positions[index];
            return new Vector2(position.X * ScaleSize, position.Y * ScaleSize);
        }
    }
}