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

namespace HoverLabels.Framework;
internal abstract class BaseLabel : IHoverLabel
{
    protected Vector2 CursorTile { get; set; }
    protected string Name { get; set; }
    protected List<string> Description { get; set; }
    public virtual int Priority { get; set; }

    public BaseLabel(int? priority)
    {
        this.Name = string.Empty;
        this.Description = new List<string>();
        this.Priority = priority ?? 0;
    }

    public void UpdateCursorTile(Vector2 cursorTile)
    {
        this.ResetLabel();
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

    public bool HasDescription()
    {
        return Description.Count > 0;
    }

    public virtual string GetName()
    {
        return this.Name;
    }

    public IEnumerable<string> GetDescription()
    {
        return this.Description;
    }

    public virtual bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return false;
    }

    public virtual void DrawOnOverlay(SpriteBatch spriteBatch)
    {
    }

    protected virtual void ResetLabel()
    {
        this.Name = String.Empty;
        this.Description = new List<string>();
    }
}
