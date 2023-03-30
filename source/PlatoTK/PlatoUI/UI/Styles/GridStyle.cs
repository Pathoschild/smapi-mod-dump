/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using PlatoUI.UI.Components;

namespace PlatoUI.UI.Styles
{
    public class GridStyle : Style
    {
        public int Cols { get; set; } = 12;

        public int Rows { get; set; } = 12;

        public int Col { get; set; } = 0;

        public int Row { get; set; } = 0;

        public int ColSpan { get; set; } = 1;

        public int RowSpan { get; set; } = 1;

        public GridStyle(IPlatoUIHelper helper, string option = "")
            : base(helper, option)
        {

        }

        public override string[] PropertyNames => new string[] { "Grid", "Col", "Row", "ColSpan", "RowSpan" };

        public override void Apply(IComponent component)
        {
            Rectangle baseBounds = component.Parent.Bounds;
            Rectangle cellBounds = new Rectangle(0, 0, baseBounds.Width / Cols, baseBounds.Height / Rows);

            component.Bounds = new Rectangle(cellBounds.Width * Col, cellBounds.Height * Row, cellBounds.Width * ColSpan, cellBounds.Height * RowSpan);
            base.Apply(component);
        }

        public override void Parse(string property, string value, IComponent component)
        {
            switch (property.ToLower())
            {
                case "grid":
                    {
                        string[] parts = value.Split(' ');
                        if (parts.Length > 0 && int.TryParse(parts[0], out int cols))
                        {
                            Cols = cols;
                            Rows = cols;
                        }

                        if (parts.Length > 1 && int.TryParse(parts[1], out int rows))
                            Rows = rows;

                        break;
                    }
                case "col": Col = int.TryParse(value, out int col) ? col : Col; break;
                case "colspan": ColSpan = int.TryParse(value, out int colSpan) ? colSpan : ColSpan; break;
                case "row": Row = int.TryParse(value, out int row) ? row : Row; break;
                case "rowspan": RowSpan = int.TryParse(value, out int rowSpan) ? rowSpan : RowSpan; break;
            }
        }
    }
}
