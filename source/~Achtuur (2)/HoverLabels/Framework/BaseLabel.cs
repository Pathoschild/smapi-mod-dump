/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using HoverLabels.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace HoverLabels.Framework;
internal abstract class BaseLabel : IHoverLabel
{
    protected Vector2 CursorTile { get; set; }
    protected List<Border> Borders { get; set; }
    public virtual int Priority { get; set; }

    public BaseLabel(int? priority)
    {
        this.Borders = new();
        this.Priority = priority ?? 0;
    }

    public void UpdateCursorTile(Vector2 cursorTile)
    {
        this.ResetBorders();
        this.SetCursorTile(cursorTile);
        this.GenerateLabel();
    }

    /// <summary>
    /// This method should set the cursor tile and get the info from the tile that is necessary in <see cref="GenerateLabel"/>
    /// </summary>
    /// <param name="cursorTile"></param>
    public virtual void SetCursorTile(Vector2 cursorTile)
    {
        this.CursorTile = cursorTile;
    }

    /// <summary>
    /// Generates objectname and description, is called in the constructor
    /// </summary>
    public abstract void GenerateLabel();

    public IEnumerable<Border> GetContents()
    {
        return this.Borders;
    }

    public virtual bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return false;
    }

    public virtual void DrawOnOverlay(SpriteBatch spriteBatch)
    {
    }

    protected virtual void ResetBorders()
    {
        this.Borders = new();
    }

    protected void NewBorder()
    {
        if (Borders is null)
            Borders = new();
        Borders.Add(new Border());
    }

    protected void AddBorder(Border border)
    {
        if (Borders is null)
            Borders = new();

        if (!border.IsEmpty)
            Borders.Add(border);
    }

    /// <summary>
    /// Creates a new border with a small font label containing <c>text</c>
    /// </summary>
    /// <param name="text"></param>
    protected void AddBorder(string text)
    {
        if (Borders is null)
            Borders = new();
        AddBorder(new Border(new LabelText(text)));
    }

    /// <summary>
    /// Creates a new border with a label
    /// </summary>
    /// <param name="label"></param>
    protected void AddBorder(LabelText label)
    {
        if (Borders is null)
            Borders = new();
        AddBorder(new Border(label));
    }

    /// <summary>
    ///  Creates a new border with multiple labels
    /// </summary>
    /// <param name="labels"></param>
    protected void AddBorder(IEnumerable<LabelText> labels)
    {
        if (Borders is null) 
            Borders = new();
        AddBorder(new Border(labels));
    }

    /// <summary>
    /// Append label to the newest border
    /// </summary>
    /// <param name="label"></param>
    protected void AppendLabelToBorder(LabelText label)
    {
        if (Borders is null)
            Borders = new();
        if (Borders.Count == 0)
            Borders.Add(new Border());
        Borders.Last().AddLabelText(label);
    }

    /// <summary>
    /// Creates a label containing <c>text</c> (small font size) and appends it to the newest border
    /// </summary>
    /// <param name="text"></param>
    protected void AppendLabelToBorder(string text)
    {
        AppendLabelToBorder(new LabelText(text));
    }
}
