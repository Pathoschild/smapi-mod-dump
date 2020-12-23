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
    public class TextField : Component
    {
        private TextBox _textBox;
        public string Text => _textBox.Text;

        public TextField(string description, int x, int y, int width, int height) : base("", description, x, y, width,
            height)
        {
            _textBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.dialogueFont,
                Game1.textColor)
            {
                X = x, Y = y, Width = width, Height = 0
            };
        }

        public override void Render(SpriteBatch b)
        {
            Hovered = Render2DUtils.IsHovered(Game1.getMouseX(), Game1.getMouseY(), X, Y, Width, Height);

            Render2DUtils.DrawBound(b, X - 5, Y - 5, Width + 20, Height + 25, Color.White);

            if (!Hovered)
            {
                _textBox.Selected = false;
            }

            _textBox.Draw(b);
        }

        public override void MouseLeftClicked(int x, int y)
        {
            _textBox.Selected = !_textBox.Selected;
            base.MouseLeftClicked(x, y);
        }
    }
}