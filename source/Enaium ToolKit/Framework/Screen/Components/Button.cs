/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace EnaiumToolKit.Framework.Screen.Components
{
    public class Button : Component
    {
        public Button(string title, string description, int x, int y, int width, int height) : base(title, description,
            x, y, width, height)
        {
        }

        public override void Render(SpriteBatch b)
        {
            Hovered = Render2DUtils.IsHovered(Game1.getMouseX(), Game1.getMouseY(), X, Y, Width, Height);

            Render2DUtils.DrawButton(b, X, Y, Width, Height, Hovered ? Color.Wheat : Color.White);
            FontUtils.DrawHvCentered(b, Title, X + Width / 2, Y + Height / 2);
            
            if (Hovered)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), X, Y, Width, Height,
                    Color.Red, 4f, false);
            }
        }
    }
}