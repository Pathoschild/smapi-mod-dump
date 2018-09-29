using Igorious.StardewValley.ShowcaseMod.ModConfig;
using Microsoft.Xna.Framework;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.ShowcaseMod.Core.Layouts
{
    internal abstract class ShowcaseGridLayoutBase : IShowcaseLayout
    {
        protected const int SpriteSheetTileSize = Object.spriteSheetTileSize;

        private int? _bottomRow;
        private int? _topRow;
        private int? _leftColumn;
        private int? _rightColumn;

        private bool IsPrecalculated { get; set; }

        protected float ScaleSize { get; }
        protected Rectangle SourceRect { get; }
        protected ItemGridProvider ItemProvider { get; }
        protected LayoutConfig Config { get; }

        protected ShowcaseGridLayoutBase(float scaleSize, Rectangle sourceRect, ItemGridProvider itemProvider, LayoutConfig config)
        {
            ScaleSize = scaleSize;
            SourceRect = sourceRect;
            ItemProvider = itemProvider;
            Config = config;
        }

        private void Precalculate()
        {
            var bounds = ItemProvider.IsHorizontal ? Config.AltSpriteBounds : Config.SpriteBounds;

            float workWidth = SourceRect.Width - bounds.Left - bounds.Right;
            float workHeigth = SourceRect.Height - bounds.Top - bounds.Bottom;

            var rowsCount = BottomRow - TopRow + 1;
            var columnsCount = RightColumn - LeftColumn + 1;

            var minRequiredWidthDelta = (workWidth - SpriteSheetTileSize * Config.Scale) / 2;
            var minRequiredHeightDelta = (workHeigth - SpriteSheetTileSize * Config.Scale) / 2;

            var leftEmptyOffset = (minRequiredWidthDelta > 0 && columnsCount > 1)? 0 : minRequiredWidthDelta;
            var topEmptyOffset = (minRequiredHeightDelta > 0 && rowsCount > 1)? 0 : minRequiredHeightDelta;

            HorizontalItemOffset = (columnsCount > 1)? (workWidth - leftEmptyOffset * 2 - SpriteSheetTileSize * Config.Scale) / (columnsCount - 1) : 0;
            VerticalItemOffset = (rowsCount > 1)? (workHeigth - topEmptyOffset * 2 - SpriteSheetTileSize * Config.Scale) / (rowsCount - 1) : 0;

            var isFlipped = (ItemProvider.CurrentRotation == 3);
            Offset = new Vector2((isFlipped ? bounds.Right : bounds.Left) + leftEmptyOffset, bounds.Top + topEmptyOffset) * ScaleSize;
        }

        private int BottomRow => _bottomRow ?? (_bottomRow = GetBottomRow()).Value;
        private int TopRow => _topRow ?? (_topRow = GetTopRow()).Value;
        private int LeftColumn => _leftColumn ?? (_leftColumn = GetLeftColumn()).Value;
        private int RightColumn => _rightColumn ?? (_rightColumn = GetRightColumn()).Value;

        private Vector2 Offset { get; set; }
        private float VerticalItemOffset { get; set; }
        private float HorizontalItemOffset { get; set; }

        protected abstract int GetBottomRow();
        protected abstract int GetTopRow();
        protected abstract int GetLeftColumn();
        protected abstract int GetRightColumn();

        public Vector2? GetItemViewRelativePosition(int i, int j)
        {
            if (!IsPrecalculated)
            {
                Precalculate();
                IsPrecalculated = true;
            }

            if (i < TopRow || i > BottomRow) return null;
            if (j < LeftColumn || j > RightColumn) return null;

            var tileDelta = new Vector2((j - LeftColumn) * HorizontalItemOffset, (i - TopRow) * VerticalItemOffset);
            return Offset + tileDelta * ScaleSize;
        }
    }
}