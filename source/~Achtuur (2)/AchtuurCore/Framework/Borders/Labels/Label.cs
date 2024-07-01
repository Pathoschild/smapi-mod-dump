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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Borders;

public class Label
{
    protected SpriteFont Font;
    public string Text { get; set; }

    /// <summary>
    /// Drawing dimensions of the label. Equal to <c>new Vector2(Width, Height)</c>
    /// </summary>
    public virtual Vector2 DrawSize => new Vector2(Width, Height);

    /// <summary>
    /// Width of the **drawn** label. Equal to <c>DrawSize.X</c>
    /// </summary>
    public virtual float Width => TextSize.X;
    /// <summary>
    /// Height of the **drawn** label. Equal to <c>DrawSize.Y</c>
    /// </summary>
    public virtual float Height => TextSize.Y;

    private Vector2 TextSize => Font.MeasureString(Text);

    public static Vector2 MarginSize => new Vector2(Margin());

    public static float Margin()
    {
        return 12f;
    }
    public Label(string text)
    {
        this.Font = Game1.smallFont;
        this.Text = text;
    }

    public void SetText(string text)
    {
        this.Text = text;
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        DrawText(spriteBatch, Text, position);

        // debug
        DebugDraw(spriteBatch, position);
    }

    protected void DrawText(SpriteBatch spriteBatch, string text, Vector2 position)
    {
        spriteBatch.DrawString(Font, text, position + new Vector2(2, 3), Game1.textShadowColor);
        spriteBatch.DrawString(Font, text, position, Game1.textColor);
    }

    protected void DebugDraw(SpriteBatch sb, Vector2 position)
    {
        AchtuurCore.Utility.Debug.DebugOnlyExecute(() =>
        {
            if (!ModEntry.DebugDraw)
                return;
            int seed = (int) (position.X + position.Y);
            sb.DrawBorder(position, DrawSize, color: AchtuurCore.Utility.ColorHelper.GetRandomColor(255, seed));
        });
    }
}
