/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/TeleportPoint
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace TeleportPoint.Framework.Gui
{
    public class NamingScreen : NamingMenu
    {
        
        public string Title { get; set; }
        
        public NamingScreen(doneNamingBehavior b, string title, string defaultName = null) : base(b, title, defaultName)
        {
            Title = title;
            textBox.Width = 800;
            textBox.X = (Game1.viewport.Width - textBox.Width) / 2;
            doneNamingButton.bounds = new Rectangle(textBox.X + textBox.Width + 32, doneNamingButton.bounds.Y, doneNamingButton.bounds.Width, doneNamingButton.bounds.Height);
        }
        
        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, Title, Game1.viewport.Width / 2, Game1.viewport.Height / 2 - 128, Title);
            textBox.Draw(b);
            doneNamingButton.draw(b);
            drawMouse(b);
        }
    }
}