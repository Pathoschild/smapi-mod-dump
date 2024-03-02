/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SAML.Events;
using SAML.Utilities.Collections;
using SAML.Utilities.Extensions;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SAML.Utilities
{
    /// <summary>
    /// Creates a dialoguebox similar to <see cref="Game1.drawDialogueBox()"/> but with accurate dimensions and instant column / row support
    /// </summary>
    public class MenuBox : INotifyPropertyChanged
    {
        private static readonly Texture2D menuTexture = Game1.menuTexture;
        private static readonly Texture2D menuTextureUncolored = Game1.uncoloredMenuTexture;
        private static readonly Regex pixelPartition = new(@"^([0-9]{1,})$");
        private static readonly Regex starredPartition = new(@"^([0-9]{0,}\*{0,1})$");

        private readonly ObservableList<GridElement> rows = [];
        private readonly ObservableList<GridElement> columns = [];
        private int width = 800;
        private int height = 600;
        private Color color = Color.White;
        private bool colorTexture = true;
        private Padding padding = new(-16, 32, -16, 32);

        private Texture2D texture = menuTexture;
        private bool useCachedBounds = false;

        public const int BorderThickness = 16;

        /// <summary>
        /// The rows to render
        /// </summary>
        /// <remarks>
        /// Purely decorative, does not affect layout
        /// </remarks>
        public ObservableList<GridElement> Rows => rows;
        /// <summary>
        /// The columns to render
        /// </summary>
        /// <remarks>
        /// Purely decorative, does not affect layout
        /// </remarks>
        public ObservableList<GridElement> Columns => columns;
        /// <summary>
        /// The width of the box (including borders)
        /// </summary>
        public int Width
        {
            get => width;
            set
            {
                width = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The height of the box (including borders)
        /// </summary>
        public int Height
        {
            get => height;
            set
            {
                height = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The color of the box's texture
        /// </summary>
        /// <remarks>
        /// Automatically toggles <see cref="ColorTexture"/> off when changed
        /// </remarks>
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                ColorTexture = false;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Whether or not to use a pre-colored texture
        /// </summary>
        public bool ColorTexture
        {
            get => colorTexture;
            set
            {
                colorTexture = value;
                texture = colorTexture ? menuTexture : menuTextureUncolored;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Additional spacing to apply to the bounds
        /// </summary>
        public Padding Padding
        {
            get => padding;
            set
            {
                padding = value;
                invokePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MenuBox()
        {
            Columns.CollectionChanged += onColumnsChanged;
            Rows.CollectionChanged += onRowsChanged;
        }

        public MenuBox(int width, int height, IEnumerable<GridElement>? columns = null, IEnumerable<GridElement>? rows = null) : this()
        {
            Width = width;
            Height = height;
            if (columns is not null)
                foreach (var col in columns)
                    Columns.Add(col);
            if (rows is not null)
                foreach (var row in rows)
                    Rows.Add(row);
        }

        /// <summary>
        /// Invoke the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">(Optional) The name of the property that has changed</param>
        /// <remarks>
        /// The name of the property is infered and does not need to be manually added if this call is made in the setter (see examples above)
        /// </remarks>
        protected void invokePropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));

        private void onColumnsChanged(object sender, CollectionChangedEventArgs e)
        {
            if (e.Added is IEnumerable<GridElement> added)
                foreach (var column in added)
                    column.PropertyChanged += onColumnChanged;
            ReArrangeColumns();
        }

        private void onColumnChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(GridElement.Decorate))
                ReArrangeColumns();
        }

        private void onRowsChanged(object sender, CollectionChangedEventArgs e)
        {
            if (e.Added is IEnumerable<GridElement> added)
                foreach (var row in added)
                    row.PropertyChanged += onRowChanged;
            ReArrangeRows();
        }

        private void onRowChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(GridElement.Decorate))
                ReArrangeRows();
        }

        #region Measurements
        /// <summary>
        /// Update column sizes
        /// </summary>
        protected void ReArrangeColumns()
        {
            if (columns.Count <= 1)
                return;
            int partitionSize = getPartitionSize(width + Padding.Left + Padding.Right, new(Columns.Select(x => x.Value)));

            foreach (var col in Columns)
            {
                if (pixelPartition.IsMatch(col.Value ?? ""))
                {
                    col.MeasuredSize = int.Parse(col.Value!);
                    continue;
                }
                if (string.IsNullOrWhiteSpace(col.Value) || starredPartition.IsMatch(col.Value))
                {
                    int colSize;
                    if (string.IsNullOrWhiteSpace(col.Value?.Trim('*')))
                        colSize = 1;
                    else
                        colSize = int.Parse(col.Value.Trim('*'));
                    int size = colSize * partitionSize;
                    col.MeasuredSize = size > col.MaxValue ? col.MaxValue : (size < col.MinValue ? col.MinValue : size);
                }
            }

            useCachedBounds = false;
        }

        /// <summary>
        /// Update row sizes
        /// </summary>
        protected void ReArrangeRows()
        {
            if (rows.Count <= 1)
                return;
            int partitionSize = getPartitionSize(height + Padding.Top + Padding.Bottom, new(Rows.Select(x => x.Value)));

            foreach (var row in Rows)
            {
                if (pixelPartition.IsMatch(row.Value ?? ""))
                {
                    row.MeasuredSize = int.Parse(row.Value!);
                    continue;
                }
                if (string.IsNullOrWhiteSpace(row.Value) || starredPartition.IsMatch(row.Value))
                {
                    int rowSize;
                    if (string.IsNullOrWhiteSpace(row.Value?.Trim('*')))
                        rowSize = 1;
                    else
                        rowSize = int.Parse(row.Value.Trim('*'));
                    int size = rowSize * partitionSize;
                    row.MeasuredSize = size > row.MaxValue ? row.MaxValue : (size < row.MinValue ? row.MinValue : size);
                }
            }

            useCachedBounds = false;
        }

        /// <summary>
        /// Get the maximum size of a 1* starred partition
        /// </summary>
        /// <param name="max">The maximum size to use (width for horizontal, height for vertical)</param>
        /// <param name="points">The points to traverse</param>
        /// <returns>The maximum size, in pixels, of a 1* partition</returns>
        private int getPartitionSize(int max, List<string> points)
        {
            int starSum = 0;
            int pixelSum = 0;

            for (int i = 0; i < points.Count; i++)
            {
                string partition = points[i];
                if (pixelPartition.IsMatch(partition))
                    pixelSum += int.Parse(partition);
                else if (partition is null || starredPartition.IsMatch(partition))
                {
                    if (string.IsNullOrWhiteSpace(partition?.Trim('*')))
                        partition = "1";
                    starSum += int.Parse(partition.Trim('*'));
                }
            }

            return (max - pixelSum) / starSum;
        }

        /// <summary>
        /// Reset the cached bounds when the size of the window changes
        /// </summary>
        public void OnGameWindowSizeChanged() => useCachedBounds = false;
        #endregion

        #region Border
        private void drawBorder(SpriteBatch b, Rectangle bounds)
        {
            drawBorderCorners(b, bounds);
            drawBorderCornerConnectors(b, bounds);
            drawBorderRegions(b, bounds);
        }

        private void drawBorderCorners(SpriteBatch b, Rectangle bounds)
        {
            drawPart(b, new(16, 16, 16, 16), getBorderPartRectAt(bounds.X, bounds.Y)); //Top Left
            drawPart(b, new(224, 16, 16, 16), getBorderPartRectAt(bounds.X + bounds.Width - BorderThickness, bounds.Y)); //Top Right
            drawPart(b, new(16, 224, 16, 16), getBorderPartRectAt(bounds.X, bounds.Y + bounds.Height - BorderThickness)); //Bottom Left
            drawPart(b, new(224, 224, 16, 16), getBorderPartRectAt(bounds.X + bounds.Width - BorderThickness, bounds.Y + bounds.Height - BorderThickness)); //Bottom Right
        }

        private void drawBorderCornerConnectors(SpriteBatch b, Rectangle bounds)
        {
            //Top Left
            drawPart(b, new(32, 16, 16, 16), getBorderPartRectAt(bounds.X + BorderThickness, bounds.Y)); //North
            drawPart(b, new(16, 32, 16, 16), getBorderPartRectAt(bounds.X, bounds.Y + BorderThickness)); //West

            //Top Right
            drawPart(b, new(208, 16, 16, 16), getBorderPartRectAt(bounds.X + bounds.Width - (BorderThickness * 2), bounds.Y)); //North
            drawPart(b, new(224, 32, 16, 16), getBorderPartRectAt(bounds.X + bounds.Width - BorderThickness, bounds.Y + BorderThickness)); //East

            //Bottom Left
            drawPart(b, new(32, 224, 16, 16), getBorderPartRectAt(bounds.X + BorderThickness, bounds.Y + bounds.Height - BorderThickness)); //South
            drawPart(b, new(16, 208, 16, 16), getBorderPartRectAt(bounds.X, bounds.Y + bounds.Height - (BorderThickness * 2))); //West

            //Bottom Right
            drawPart(b, new(208, 224, 16, 16), getBorderPartRectAt(bounds.X + bounds.Width - (BorderThickness * 2), bounds.Y + bounds.Height - BorderThickness)); //South
            drawPart(b, new(224, 208, 16, 16), getBorderPartRectAt(bounds.X + bounds.Width - BorderThickness, bounds.Y + bounds.Height - (BorderThickness * 2))); //East
        }

        private void drawBorderRegions(SpriteBatch b, Rectangle bounds)
        {
            drawPart(b, new(48, 16, 16, 16), new(bounds.X + (BorderThickness * 2), bounds.Y, bounds.Width - (BorderThickness * 4), BorderThickness)); //North
            drawPart(b, new(224, 48, 16, 16), new(bounds.X + bounds.Width - BorderThickness, bounds.Y + (BorderThickness * 2), BorderThickness, bounds.Height - (BorderThickness * 4))); //East
            drawPart(b, new(176, 224, 16, 16), new(bounds.X + (BorderThickness * 2), bounds.Y + bounds.Height - BorderThickness, bounds.Width - (BorderThickness * 4), BorderThickness)); //South
            drawPart(b, new(16, 192, 16, 16), new(bounds.X, bounds.Y + (BorderThickness * 2), BorderThickness, bounds.Height - (BorderThickness * 4))); //West
        }
        #endregion

        #region Content
        private void drawContent(SpriteBatch b, Rectangle bounds)
        {
            drawPart(b, new(64, 128, 64, 64), new(bounds.X + BorderThickness, bounds.Y + BorderThickness, bounds.Width - (BorderThickness * 2), bounds.Height - (BorderThickness * 2)));
            drawContentShadow(b, bounds);
        }

        private void drawContentShadow(SpriteBatch b, Rectangle bounds)
        {
            //North
            drawPart(b, new(32, 32, 1, 4), new(bounds.X + BorderThickness, bounds.Y + BorderThickness, bounds.Width - (BorderThickness * 2), (BorderThickness / 4)));

            //South
            drawPart(b, new(36, 220, 1, 4), new(bounds.X + BorderThickness + (BorderThickness / 4), bounds.Y + bounds.Height - BorderThickness - (BorderThickness / 4), bounds.Width - (BorderThickness * 2) - (BorderThickness / 4), (BorderThickness / 4)));
            drawPart(b, new(32, 220, 4, 4), new(bounds.X + BorderThickness, bounds.Y + bounds.Height - BorderThickness - (BorderThickness / 4), (BorderThickness / 4), (BorderThickness / 4)));

            //East
            drawPart(b, new(216, 32, 8, 1), new(bounds.X + bounds.Width - BorderThickness - (BorderThickness / 2), bounds.Y + BorderThickness, (BorderThickness / 2), bounds.Height - (BorderThickness * 2)));
            drawPart(b, new(212, 36, 4, 4), new(bounds.X + bounds.Width - BorderThickness - (BorderThickness / 2) - (BorderThickness / 4), bounds.Y + BorderThickness + (BorderThickness / 4), (BorderThickness / 4), (BorderThickness / 4)));

            //West
            drawPart(b, new(32, 36, 4, 1), new(bounds.X + BorderThickness, bounds.Y + BorderThickness + (BorderThickness / 4), (BorderThickness / 4), bounds.Height - (BorderThickness * 2) - (BorderThickness / 4)));
        }
        #endregion

        #region GridElements
        private void drawColumns(SpriteBatch b, Rectangle bounds)
        {
            int offset = 0;
            for (int i = 0; i < Columns.Count - 1; i++)
            {
                var col = Columns[i];
                offset += col.MeasuredSize;
                if (!useCachedBounds)
                    col.CachedBounds = new(bounds.X + offset - (BorderThickness / 2), bounds.Y + BorderThickness, BorderThickness, bounds.Height - (BorderThickness * 2));
                drawPart(b, new(88, 48, BorderThickness, BorderThickness), col.CachedBounds);
                if (col.Decorate)
                {
                    drawPartitionDecoration(b, bounds, col.EdgeDecoration, new(bounds.X + offset - (BorderThickness / 2), bounds.Y), DecorationSides.Left | DecorationSides.Down | DecorationSides.Right);
                    drawPartitionDecoration(b, bounds, col.EdgeDecoration, new(bounds.X + offset - (BorderThickness / 2), bounds.Y + bounds.Height - BorderThickness), DecorationSides.Left | DecorationSides.Up | DecorationSides.Right);
                }
            }
        }

        private void drawRows(SpriteBatch b, Rectangle bounds)
        {
            int offset = 0;
            for (int i = 0; i < Rows.Count - 1; i++)
            {
                var row = Rows[i];
                offset += row.MeasuredSize;
                if (!useCachedBounds)
                    row.CachedBounds = new(bounds.X + BorderThickness, bounds.Y + offset - (BorderThickness / 2), bounds.Width - (BorderThickness * 2), BorderThickness);
                drawPart(b, new(48, 88, 16, 16), row.CachedBounds);
                if (row.Decorate)
                {
                    drawPartitionDecoration(b, bounds, row.EdgeDecoration, new(bounds.X, bounds.Y + offset - (BorderThickness / 2)), DecorationSides.Up | DecorationSides.Right | DecorationSides.Down);
                    drawPartitionDecoration(b, bounds, row.EdgeDecoration, new(bounds.X + bounds.Width - BorderThickness, bounds.Y + offset - (BorderThickness / 2)), DecorationSides.Up | DecorationSides.Left | DecorationSides.Down);
                }
            }
        }

        private void drawPartitionDecoration(SpriteBatch b, Rectangle bounds, DecorationStyle style, Vector2 position, DecorationSides sides = DecorationSides.All)
        {
            switch (style)
            {
                case DecorationStyle.Smooth:
                    drawPartitionMerge(b, bounds, position, sides);
                    break;
                case DecorationStyle.Bauble:
                    drawPartitionNob(b, bounds, position, sides);
                    break;
            }
        }

        private void drawPartitionMerge(SpriteBatch b, Rectangle bounds, Vector2 position, DecorationSides sides)
        {
            int quarterThickness = BorderThickness / 4;
            if (sides.HasThisFlag(DecorationSides.Up))
                drawPart(b, new(20, 80, 8, 4), new((int)position.X + quarterThickness, (int)position.Y, BorderThickness / 2, quarterThickness));
            if (sides.HasThisFlag(DecorationSides.Down))
                drawPart(b, new(20, 80, 8, 4), new((int)position.X + quarterThickness, (int)position.Y + BorderThickness - quarterThickness, BorderThickness / 2, quarterThickness));
            if (sides.HasThisFlag(DecorationSides.Right))
                drawPart(b, new(108, 228, 4, 8), new((int)position.X + BorderThickness - quarterThickness, (int)position.Y + quarterThickness, quarterThickness, BorderThickness / 2));
            if (sides.HasThisFlag(DecorationSides.Left))
                drawPart(b, new(108, 228, 4, 8), new((int)position.X, (int)position.Y + quarterThickness, quarterThickness, BorderThickness / 2));
        }

        private void drawPartitionNob(SpriteBatch b, Rectangle bounds, Vector2 position, DecorationSides sides)
        {
            drawPart(b, new(88, 16, 16, 16), new((int)position.X, (int)position.Y, BorderThickness, BorderThickness));
            if (!sides.HasAllFlags())
                drawPartitionNobEndSide(b, position, sides);
            drawPartitionNobConnectors(b, position, sides);
        }

        private void drawPartitionNobEndSide(SpriteBatch b, Vector2 position, DecorationSides sides)
        {
            int quarterThickness = BorderThickness / 4;
            if (!sides.HasFlag(DecorationSides.Up))
            {
                drawPart(b, new(88, 12, 16, 4), new((int)position.X, (int)position.Y - quarterThickness, BorderThickness, quarterThickness));
                drawPartitionNobCorner(b, new(position.X - quarterThickness, position.Y + BorderThickness));
            }
            if (!sides.HasFlag(DecorationSides.Down))
            {
                drawPart(b, new(88, 240, 16, 4), new((int)position.X, (int)position.Y + BorderThickness, BorderThickness, quarterThickness));
                drawPartitionNobCorner(b, new(position.X - quarterThickness, position.Y - quarterThickness));
            }
            if (!sides.HasFlag(DecorationSides.Right))
            {
                drawPart(b, new(240, 88, 4, 16), new((int)position.X + BorderThickness, (int)position.Y, quarterThickness, BorderThickness));
                drawPartitionNobCorner(b, new(position.X - quarterThickness, position.Y + BorderThickness));
            }
            if (!sides.HasFlag(DecorationSides.Left))
            {
                drawPart(b, new(12, 88, 4, 16), new((int)position.X - quarterThickness, (int)position.Y, quarterThickness, BorderThickness));
                drawPartitionNobCorner(b, new(position.X + BorderThickness, position.Y + BorderThickness));
            }
        }

        private void drawPartitionNobCorner(SpriteBatch b, Vector2 position) => drawPart(b, new(84, 220, 4, 4), new((int)position.X, (int)position.Y, BorderThickness / 4, BorderThickness / 4));

        private void drawPartitionNobConnectors(SpriteBatch b, Vector2 position, DecorationSides sides)
        {
            int quarterThickness = BorderThickness / 4;
            if (sides.HasThisFlag(DecorationSides.Up))
                drawPart(b, new(16, 84, 16, 4), new((int)position.X, (int)position.Y - quarterThickness, BorderThickness, quarterThickness));
            if (sides.HasThisFlag(DecorationSides.Down))
                drawPart(b, new(16, 104, 16, 4), new((int)position.X, (int)position.Y + BorderThickness, BorderThickness, quarterThickness));
            if (sides.HasThisFlag(DecorationSides.Right))
                drawPart(b, new(104, 16, 4, 16), new((int)position.X + BorderThickness, (int)position.Y, quarterThickness, BorderThickness));
            if (sides.HasThisFlag(DecorationSides.Left))
                drawPart(b, new(84, 16, 4, 16), new((int)position.X - quarterThickness, (int)position.Y, quarterThickness, BorderThickness));
        }
        #endregion

        public virtual void Draw(SpriteBatch b, int x, int y)
        {
            Rectangle bounds = new(x + Padding.Left, y + Padding.Top, width + Padding.Right, height + Padding.Bottom);

            drawBorder(b, bounds);
            drawContent(b, bounds);

            drawColumns(b, bounds);
            drawRows(b, bounds);

            useCachedBounds = true;

            foreach (var row in Rows)
                foreach (var col in Columns)
                    if (row.CachedBounds.Intersects(col.CachedBounds, out Vector2 point))
                        drawPartitionDecoration(b, bounds, row.IntersectionDecoration, new(point.X + bounds.X + BorderThickness, point.Y + bounds.Y + BorderThickness));
        }

        /// <summary>
        /// Draw a single part of the texture
        /// </summary>
        /// <param name="source">The source rectangle of the texture</param>
        /// <param name="target">The target rectangle defining where on the screen to draw the part</param>
        protected void drawPart(SpriteBatch b, Rectangle source, Rectangle target) => b.Draw(texture, target, source, color);

        /// <summary>
        /// Get a target rectangle for a border part
        /// </summary>
        /// <param name="x">The x position at which the part should be drawn</param>
        /// <param name="y">The y position at which the part should be drawn</param>
        /// <returns>A rectangle with the default <see cref="BorderThickness"/> for the size, and the given x, y values for position</returns>
        protected Rectangle getBorderPartRectAt(int x, int y) => new(x, y, BorderThickness, BorderThickness);
    }
}
