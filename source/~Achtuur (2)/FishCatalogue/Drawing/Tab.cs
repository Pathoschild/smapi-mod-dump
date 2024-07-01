/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue.Drawing;
public class Tab
{
    static Rectangle TabSourceRect = new Rectangle(16, 368, 16, 16);
    ClickableComponent clickable;
    Texture2D image;
    bool Selected;

    public Vector2 Position => new Vector2(clickable.bounds.X, clickable.bounds.Y);

    public Tab(string name, string label) : this(name, label, null)
    {
    }

    public Tab(string name, string label, Texture2D image)
    {
        this.clickable = new ClickableComponent(new Rectangle(0, 0, 64, 64), name, label);
        this.image = image;
    }

    public void SetPosition(Vector2 pos) { SetPosition((int)pos.X, (int)pos.Y); }
    public void SetPosition(int x, int y) 
    { 
        this.clickable.bounds.X = x;
        this.clickable.bounds.Y = y;
    }

    public bool Contains(int x, int y) { return this.clickable.containsPoint(x, y); }

    public void Draw(SpriteBatch sb)
    {
        Vector2 offset = Vector2.Zero;
        if (Selected) // slightly move down when selected
            offset = new Vector2(0, 16f);
        sb.Draw(Game1.mouseCursors, Position + offset, TabSourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);

        if (image is null)
            return;
        Vector2 texture_offset = new Vector2(16f, 24f);
        sb.DrawSizedTexture(image, Position + offset + texture_offset, new Vector2(32f, 32f));
    }

    internal void Deselect()
    {
        this.Selected = false;
    }

    internal void Select()
    {
        this.Selected = true;
    }
}
