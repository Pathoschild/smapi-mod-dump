/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AchtuurCore.Framework.Borders;
public class GridLabel : Label
{
    private float Scale => 1f;
    private int ItemsPerRow => (num_columns is null) ? Math.Max(3, (int)Math.Sqrt(labels.Count)) : num_columns.Value;

    public override Vector2 DrawSize => CalculateDrawSize();

    public override float Width => DrawSize.X;
    public override float Height => DrawSize.Y;
    public int ItemCount => labels.Count;
    private int Columns => Math.Min(ItemsPerRow, labels.Count);
    private int Rows => (int)Math.Ceiling(labels.Count / (float)ItemsPerRow);

    private float GridMargin => Margin() * 0.5f;

    private bool FixedWidth;
    private bool FixedHeight;

    List<Label> labels;
    int? num_columns;

    private Vector2? cachedDrawSize = null;
    /// <summary>
    /// Max dimensions of a label in the grid, cached after calculation
    /// </summary>
    private Vector2? cachedMaxDimensions = null;
    public GridLabel(IEnumerable<Label> labels) : base(string.Empty)
    {
        this.labels = labels.ToList();
        cachedDrawSize = null;

        Vector2 max_dim = new(0, 0);
        for (int i = 0; i < this.labels.Count; i++)
        {
            max_dim.X = Math.Max(max_dim.X, this.labels[i].Width + GridMargin);
            max_dim.Y = Math.Max(max_dim.Y, this.labels[i].Height + GridMargin);
        }
        this.cachedMaxDimensions = max_dim;
    }

    public void SetNumberOfColumns(int columns)
    {
        this.num_columns = columns;
    }

    public void SetFixedWidth(bool fixed_width) 
    {
        this.FixedWidth = fixed_width;
    }

    public void SetFixedHeight(bool fixed_height)
    {
        this.FixedHeight = fixed_height;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        IEnumerable<float> x_offsets = CalculateWidths();
        IEnumerable<float> y_offsets = CalculateHeights();

        for (int i = 0; i < labels.Count; i++)
        {
            int col = i % ItemsPerRow;
            int row = i / ItemsPerRow;
            Vector2 offset = new Vector2(x_offsets.ElementAt(col), y_offsets.ElementAt(row));
            labels[i].Draw(spriteBatch, position + offset);
        }
    }
    private IEnumerable<float> CalculateWidths()
    {
        float cur_offset = GridMargin;
        for (int c = 0; c < Columns; c++)
        {
            yield return cur_offset;
            cur_offset += GetWidthForColumn(c);
        }
    }

    private IEnumerable<float> CalculateHeights()
    {
        float cur_offset = GridMargin;
        for (int r = 0; r < Rows; r++)
        {
            yield return cur_offset;
            cur_offset += GetHeightForRow(r);
        }
    }

    private float GetWidthForColumn(int col)
    {
        if (FixedWidth)
            return cachedMaxDimensions.Value.X;
        // width of column is max font x + sprite x of all names in that column
        // items in column are items at col + items_per_row * i
        float max_width = 0;
        for (int i = col; i < labels.Count; i += ItemsPerRow)
        {
            max_width = Math.Max(max_width, labels[i].Width + GridMargin);
        }
        return max_width;
    }

    private float GetHeightForRow(int row)
    {
        if (FixedHeight)
            return cachedMaxDimensions.Value.Y;
        // height of row is Max(font.y, spritedim.y) of that row
        // items in row are items at row * items_per_row + i
        float max_height = 0;
        for (int i = row*ItemsPerRow; i < Math.Min(labels.Count, (row+1)*ItemsPerRow); i++)
        {
            max_height = Math.Max(max_height, labels[i].Height + GridMargin);
        }
        return max_height;
    }

    private Vector2 CalculateDrawSize()
    {
        // this used to be a 10 line function lmao
        return ItemTextDrawSize();
    }

    /// <summary>
    /// The draw size of all items and their names
    /// </summary>
    /// <returns></returns>
    private Vector2 ItemTextDrawSize()
    {
        if (this.cachedDrawSize is Vector2 size)
            return size;

        Vector2 dim = Label.MarginSize;
        for (int c = 0; c < Columns; c++)
        {
            dim.X += GetWidthForColumn(c);
        }
        for (int r = 0; r < Rows; r++)
        {
            dim.Y += GetHeightForRow(r);
        }
        this.cachedDrawSize = dim;
        return dim;
    }
}