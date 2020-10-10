/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Igorious.StardewValley.DynamicAPI.Menu
{
    public abstract class GridToolTip
    {
        private const int Margin = 6;
        protected static int TileSize => Game1.tileSize;

        private List<MenuCell> Cells { get; } = new List<MenuCell>();
        private Rectangle[,] GridSizes { get; set; } = new Rectangle[0, 0];
        private MenuCell[,] GridCells { get; set; } = new MenuCell[0, 0];
        private int Heigth { get; set; }
        private int Width { get; set; }
        private int Rows => GridSizes.GetLength(0);
        private int Columns => GridSizes.GetLength(1);

        public virtual bool IsExclusive { get; protected set; }

        protected static int MouseX => Game1.getMouseX();
        protected static int MouseY => Game1.getMouseY();
        protected static int NormalizedMouseX => MouseX + Game1.viewport.X;
        protected static int NormalizedMouseY => MouseY + Game1.viewport.Y;
        protected Rectangle LastBounds { get; private set; }

        protected MenuCell GetCell(int x, int y)
        {
            for (var i = 0; i < Rows; ++i)
            {
                for (var j = 0; j < Columns; ++j)
                {
                    var rect = GridSizes[i, j];
                    if (rect.Contains(x, y))
                    {
                        return GridCells[i, j];
                    }
                }
            }
            return null;
        }

        protected void Clear()
        {
            Cells.Clear();
        }

        protected void RegisterCell(MenuCell cell)
        {
            Cells.Add(cell);
        }

        protected void Recalculate()
        {
            var rows = Cells.Max(c => c.Row) + 1;
            var columns = Cells.Max(c => c.Column) + 1;
            GridCells = CellsToGrid(Cells, rows, columns);

            var rowsHeight = GetRowsHeight(GridCells, rows, columns);
            Heigth = rowsHeight.Sum() + Margin * (rows - 1);

            var columnsWidth = GetColumnsWidth(GridCells, rows, columns);
            ProcessColumnSpans(GridCells, columnsWidth, rows, columns);
            Width = columnsWidth.Sum() + Margin * (columns - 1);

            GridSizes = GetCellsSize(GridCells, rowsHeight, columnsWidth, rows, columns);
        }

        public abstract void Draw();

        public abstract bool NeedDraw();

        protected void Draw(int x, int y)
        {
            LastBounds = new Rectangle(x - 18, y - 20, Width + 36, Heigth + 36);
            
            IClickableMenu.drawTextureBox(
                Game1.spriteBatch,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                LastBounds.X,
                LastBounds.Y,
                LastBounds.Width,
                LastBounds.Height,
                Color.White);

            for (var i = 0; i < Rows; ++i)
            {
                for (var j = 0; j < Columns; ++j)
                {
                    var cell = GridCells[i, j];
                    if (cell == null) continue;
                    var rect = GridSizes[i, j];
                    cell.Draw(new Rectangle(x + rect.X, y + rect.Y, rect.Width, rect.Height));
                }
            }
        }

        #region	Auxiliary Methods

        private static MenuCell[,] CellsToGrid(IEnumerable<MenuCell> cells, int rows, int columns)
        {
            var gridCells = new MenuCell[rows, columns];
            foreach (var cell in cells)
            {
                gridCells[cell.Row, cell.Column] = cell;
            }
            return gridCells;
        }

        private static int[] GetRowsHeight(MenuCell[,] gridCells, int rows, int columns)
        {
            var rowsHeight = new int[rows];
            for (var i = 0; i < rows; ++i)
            {
                var row = Enumerable.Range(0, columns)
                    .Select(j => gridCells[i, j])
                    .Where(c => c != null)
                    .ToList();

                rowsHeight[i] = row.Count != 0 ? row.Max(c => c.Height) : 0;
            }
            return rowsHeight;
        }

        private static int[] GetColumnsWidth(MenuCell[,] gridCells, int rows, int columns)
        {
            var columnsWidth = new int[columns];
            for (var j = 0; j < columns; ++j)
            {
                var column = Enumerable.Range(0, rows)
                    .Select(i => gridCells[i, j])
                    .Where(c => c != null && c.ColumnSpan == 1)
                    .ToList();

                columnsWidth[j] = column.Count != 0 ? column.Max(c => c.Width) : 0;
            }
            return columnsWidth;
        }

        private static void ProcessColumnSpans(MenuCell[,] gridCells, IList<int> columnsWidth, int rows, int columns)
        {
            for (var j = 0; j < columns; ++j)
            {
                var spannedCells = Enumerable.Range(0, rows)
                    .Select(i => gridCells[i, j])
                    .Where(c => c != null && c.ColumnSpan > 1)
                    .ToList();

                if (spannedCells.Count == 0) continue;
                foreach (var spannedCell in spannedCells)
                {
                    var upBound = Math.Min(spannedCell.Column + spannedCell.ColumnSpan, columnsWidth.Count) - spannedCell.Column;
                    var currentWidth = Enumerable.Range(spannedCell.Column, upBound).Sum(k => columnsWidth[k]);
                    if (currentWidth < spannedCell.Width)
                    {
                        columnsWidth[spannedCell.Column + upBound - 1] += spannedCell.Width - currentWidth;
                    }
                }
            }
        }

        private Rectangle[,] GetCellsSize(MenuCell[,] gridCells, IList<int> rowsHeight, IList<int> columnsWidth, int rows, int columns)
        {
            var gridSizes = new Rectangle[rows, columns];
            var y0 = 0;
            for (var i = 0; i < rows; ++i)
            {
                var x0 = 0;
                var height = rowsHeight[i];
                for (var j = 0; j < columns; ++j)
                {
                    var width = columnsWidth[j];
                    var cell = gridCells[i, j];
                    if (cell != null)
                    {
                        var spanWidth = width;
                        var upBound = Math.Min(cell.Column + cell.ColumnSpan, columnsWidth.Count) - cell.Column;
                        for (var k = 1; k < upBound; ++k)
                        {
                            spanWidth += columnsWidth[j + k];
                        }

                        var x = x0;
                        if (cell.Aligment.HasFlag(Aligment.Right))
                        {
                            x += spanWidth - cell.Width;
                        }
                        else if (cell.Aligment.HasFlag(Aligment.HorizontalCenter))
                        {
                            x += (spanWidth - cell.Width) / 2;
                        }

                        var y = y0;
                        if (cell.Aligment.HasFlag(Aligment.Bottom))
                        {
                            y += height - cell.Height;
                        }
                        else if (cell.Aligment.HasFlag(Aligment.VerticalCenter))
                        {
                            y += (height - cell.Height) / 2;
                        }

                        gridSizes[i, j] = new Rectangle(x, y, cell.Width, cell.Height);
                    }
                    x0 += width + Margin;
                }
                y0 += height + Margin;
            }
            return gridSizes;
        }

        #endregion     
    }
}
