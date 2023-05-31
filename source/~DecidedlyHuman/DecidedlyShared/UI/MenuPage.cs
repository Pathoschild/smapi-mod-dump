/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework.Graphics;

namespace DecidedlyShared.Ui;

public class MenuPage
{
    public UiElement page;
    public TextElement pageText;

    public int TotalHeight
    {
        get
        {
            int height = this.page.Height;
            if (this.pageText != null)
                height += this.pageText.Height;

            return height;
        }
    }

    public int TotalWidth
    {
        get
        {
            int width = this.page.Width;
            if (this.pageText != null)
                width = Math.Max(this.pageText.Width, this.page.Width);

            return width;
        }
    }

    public void Draw(SpriteBatch sb)
    {
        if (this.page != null)
            this.page.Draw(sb);

        if (this.pageText != null)
            this.pageText.Draw(sb);
    }
}
