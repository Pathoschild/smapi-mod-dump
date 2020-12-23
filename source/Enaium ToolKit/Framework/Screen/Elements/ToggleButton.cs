/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using System;
using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace EnaiumToolKit.Framework.Screen.Elements
{
    public class ToggleButton : Element
    {
        public bool Toggled;

        public ToggleButton(string title, string description) : base(title, description)
        {
        }

        public override void Render(SpriteBatch b, int x, int y)
        {
            Hovered = Render2DUtils.IsHovered(Game1.getMouseX(), Game1.getMouseY(), x, y, Width, Height);

            Color color;

            if (Toggled)
            {
                color = Color.Green;
            }
            else
            {
                color = Color.Red;
            }

            if (Hovered)
            {
                color = Color.Wheat;
            }

            Render2DUtils.DrawButton(b, x, y, Width, Height, color);
            FontUtils.DrawHvCentered(b, Title, x + Width / 2, y + Height / 2);
        }

        public override void MouseLeftClicked(int x, int y)
        {
            Toggled = !Toggled;
            base.MouseLeftClicked(x, y);
        }
    }
}