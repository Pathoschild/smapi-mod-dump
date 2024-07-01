/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework.Borders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.GUI;
public class Page
{
    public Vector2 Size => borderDrawer.BorderSize();
    public float Width => borderDrawer.BorderSize().X;
    public float Height => borderDrawer.BorderSize().Y;


    BorderDrawer borderDrawer;
    float scrollPercentage = 0;
    public Page()
    {
        borderDrawer = new();
        scrollPercentage = 0;
    }

    public void Draw(SpriteBatch sb, Vector2 pos)
    {
        borderDrawer.Draw(sb, pos);
    }

    internal void SetBorders(IEnumerable<Border> border)
    {
        this.borderDrawer.SetBorder(border);
    }
}
