using System;
using Microsoft.Xna.Framework;

namespace Igorious.StardewValley.DynamicAPI.Menu
{
    public class MenuCell
    {
        protected MenuCell(int row, int column, Aligment aligment)
        {
            Row = row;
            Column = column;
            Aligment = aligment;
        }

        public MenuCell(int row, int column, Aligment aligment, int width, int height, Action<Rectangle> draw) : this(row, column, aligment)
        {
            Width = width;
            Height = height;
            Draw = draw;
        }

        public int Row { get; }
        public int Column { get; }
        public int ColumnSpan { get; set; } = 1;
        public Aligment Aligment { get; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public Action<Rectangle> Draw { get; protected set; }
    }
}