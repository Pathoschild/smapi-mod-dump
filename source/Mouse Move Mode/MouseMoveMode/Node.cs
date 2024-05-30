/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ylsama/RightClickMoveMode
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MouseMoveMode
{
    /**
     * @brief This contain a rectangle that can be print to screen
     */
    class DrawableNode
    {
        private Rectangle box;
        public Color color { get; set; } = Color.White;

        public DrawableNode(Rectangle box)
        {
            this.box = box;
        }

        public DrawableNode(Rectangle box, Color color)
        {
            this.box = box;
            this.color = color;
        }

        public DrawableNode(Vector2 position, int width = 32, int height = 32)
        {
            this.box = new Rectangle((int)position.X - width / 2, (int)position.Y - height / 2, width, height);
        }

        public DrawableNode(int x, int y, int width = 32, int height = 32)
        {
            this.box = new Rectangle(x - width / 2, y - height / 2, width, height);
        }

        public DrawableNode(int x, int y, Color color, int width = 32, int height = 32)
        {
            this.box = new Rectangle(x - width / 2, y - height / 2, width, height);
            this.color = color;
        }

        public void draw(SpriteBatch b)
        {
            DrawHelper.drawBox(b, this.box, color);
        }

        public void draw(SpriteBatch b, Color color)
        {
            DrawHelper.drawBox(b, this.box, color);
        }

        public override String ToString()
        {
            var x = this.box.X;
            var y = this.box.Y;
            var w = this.box.Width;
            var h = this.box.Height;
            return String.Format("x: {0}, y: {1}, w: {2}, h: {3}", x, y, w, h);
        }
    }
}
