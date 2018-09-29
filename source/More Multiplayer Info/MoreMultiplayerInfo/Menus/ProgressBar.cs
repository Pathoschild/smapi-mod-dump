using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;

namespace MoreMultiplayerInfo
{
    public class ProgressBar : IClickableMenu
    {
        private int _current;
        private int _max;
        private Rectangle _position;
        private readonly Color _color;
        private bool _vertical;

        private double PercentComplete => Math.Round(((double) _current / _max) * 1, 2);

        private double PercentIncomplete => 1 - PercentComplete;

        public ProgressBar(int current, int max, Rectangle position, Color color, bool vertical)
        {
            _current = current;
            _max = max;
            _position = position;
            _color = color;
            _vertical = vertical;
        }

        public override void draw(SpriteBatch b)
        {
            var xPos = _position.X;
            var width = _position.Width;

            var yPos = _position.Y;
            var height = _position.Height;

            if (_vertical)
            {
                yPos = Convert.ToInt32(_position.Y + (_position.Height * PercentIncomplete));

                height = Convert.ToInt32(_position.Height * PercentComplete);
            }
            else
            {
                xPos = Convert.ToInt32(_position.X + _position.Width * PercentIncomplete);

                width = Convert.ToInt32(_position.Width * PercentComplete);
            }
            
            b.Draw(TextureHelper.WhitePixel, new Rectangle(xPos, yPos, width, height), _color);

            base.draw(b);
        }
    }
}