/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hakej/Animal-Pet-Status
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalPetStatus
{
    public class Drawer
    {
        private const int BORDER_OFFSET = 30;
        private const int ANIMAL_ICON_SIZE = 25;
        private const int SPACE_BETWEEN_ICON_AND_NAME = 5;

        private static SpriteBatch _spriteBatch;
        private static SpriteFont _spriteFont;

        public Drawer(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            _spriteBatch = spriteBatch;
            _spriteFont = spriteFont;
        }

        public void DrawString(string text, Vector2 position)
        {
            _spriteBatch.DrawString(_spriteFont, text, position);
        }

        public void DrawStringsInRectangle(IEnumerable<string> texts, Rectangle rectangle, Color color, Alignment alignment = Alignment.Center)
        {
            var offset = rectangle.Height / texts.Count();
            var currentRectangle = rectangle;

            foreach (var t in texts)
            {
                DrawStringAligned(t, currentRectangle, color, alignment);

                currentRectangle.Y += offset;
            }
        }

        public void DrawStringsWithBackground(IEnumerable<string> texts, Vector2 position, Color color, Texture2D backgroundTop, Texture2D backgroundMiddle, Texture2D backgroundBottom)
        {
            _spriteBatch.Draw(backgroundTop, position);

            var middlePosition = position;
            middlePosition.Y += backgroundTop.Height;   

            var drawingRectangle = new Rectangle((int)middlePosition.X, (int)middlePosition.Y, backgroundMiddle.Width, backgroundMiddle.Height);

            foreach (var t in texts)
            {
                _spriteBatch.Draw(backgroundMiddle, drawingRectangle);

                DrawStringAligned(t, drawingRectangle, color, Alignment.Center);

                drawingRectangle.Y += backgroundMiddle.Height;
            }

            _spriteBatch.Draw(backgroundBottom, drawingRectangle);
        }

        public void DrawAnimalNamesWithBackground(IEnumerable<FarmAnimal> animals, Vector2 position, Texture2D backgroundTop, Texture2D backgroundMiddle, Texture2D backgroundBottom)
        {
            _spriteBatch.Draw(backgroundTop, position);

            var middlePosition = position;
            middlePosition.Y += backgroundTop.Height;

            var drawingRectangle = new Rectangle((int)middlePosition.X, (int)middlePosition.Y, backgroundMiddle.Width, backgroundMiddle.Height);

            foreach (var a in animals)
            {
                _spriteBatch.Draw(backgroundMiddle, drawingRectangle);

                var s = a.Sprite;
                var texture = s.Texture;
                var iconDrawingRectangle = new Rectangle(drawingRectangle.X + BORDER_OFFSET, drawingRectangle.Y, ANIMAL_ICON_SIZE, ANIMAL_ICON_SIZE);

                SpriteEffects spriteEffect;

                if (s.textureUsesFlippedRightForLeft && a.FacingDirection == 3)
                {
                    spriteEffect = SpriteEffects.FlipHorizontally;    
                }
                else
                {
                    spriteEffect = SpriteEffects.None;
                }

                _spriteBatch.Draw(texture,
                    iconDrawingRectangle,
                    s.SourceRect,
                    Color.White, 
                    0, 
                    new Vector2(0, 0),
                    spriteEffect,
                    0);

                drawingRectangle.X += BORDER_OFFSET + ANIMAL_ICON_SIZE + SPACE_BETWEEN_ICON_AND_NAME;

                var color = GetTextColorForAnimal(a);
                DrawStringAligned(a.Name, drawingRectangle, color, Alignment.Left);

                drawingRectangle.X -= BORDER_OFFSET + ANIMAL_ICON_SIZE + SPACE_BETWEEN_ICON_AND_NAME;

                drawingRectangle.Y += backgroundMiddle.Height;
            }

            _spriteBatch.Draw(backgroundBottom, drawingRectangle);
        }

        private Color GetTextColorForAnimal(FarmAnimal animal)
        {
            if (animal.currentLocation == Game1.player.currentLocation)
            {
                return Color.Black;
            }
            else
            {
                return Color.Gray;
            }            
        }

        [Flags]
        public enum Alignment { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8 }

        private void DrawStringAligned(string text, Rectangle bounds, Color color, Alignment align)
        {
            Vector2 size = _spriteFont.MeasureString(text);
            Vector2 pos = new Vector2(bounds.Center.X, bounds.Center.Y);
            Vector2 origin = size * 0.5f;

            if (align.HasFlag(Alignment.Left))
                origin.X += bounds.Width / 2 - size.X / 2;

            if (align.HasFlag(Alignment.Right))
                origin.X -= bounds.Width / 2 - size.X / 2;

            if (align.HasFlag(Alignment.Top))
                origin.Y += bounds.Height / 2 - size.Y / 2;

            if (align.HasFlag(Alignment.Bottom))
                origin.Y -= bounds.Height / 2 - size.Y / 2;

            _spriteBatch.DrawString(_spriteFont, text, pos, color, origin);
        }
    }
}
