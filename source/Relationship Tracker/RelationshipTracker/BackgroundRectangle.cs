using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RelationshipTracker
{
    class BackgroundRectangle
    {
        public int X { get; set; } // x coord of block
        public int Y { get; set; } // y coord of block
        public int Width { get; set; } // Width of block
        public int Height { get; set; } // Height of block
        public Color color { get; set; } // Color of the block

        private Texture2D Pixel { get; set; }
        private SpriteBatch spriteBatch;
        private GraphicsDevice graphicsDevice;

        public BackgroundRectangle(int x, int y, int width, int height, Color color, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Texture2D pixel)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            this.color = color;
            this.spriteBatch = spriteBatch;
            this.graphicsDevice = graphicsDevice;
            Pixel = pixel;
            Pixel.SetData(new Color[] { Color.White });
        }

        public void Draw()
        {
            spriteBatch.Draw(Pixel, new Rectangle(X, Y, Width, Height), null, color);
        }

        public void DrawBorder()
        {
            spriteBatch.Draw(Pixel, new Rectangle(X+1, Y+1, Width - 2, 2), new Color(143, 69, 30)); //top border
            spriteBatch.Draw(Pixel, new Rectangle(X+1, Y+1, 2, Height - 2), new Color(143, 69, 30)); // left border
            spriteBatch.Draw(Pixel, new Rectangle(X+1, Y+52, Width - 2, 2), new Color(143, 69, 30)); // heading border
            spriteBatch.Draw(Pixel, new Rectangle(Width - 1, Y+1, 2, Height - 2), new Color(143, 69, 30)); // right border
            spriteBatch.Draw(Pixel, new Rectangle(X + 1, Y + Height - 3, Width - 2, 2), new Color(143, 69, 30)); // bottom border
        }
    }
}
