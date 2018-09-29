using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ClimatesOfFerngillRebuild
{
    public class CustomWeatherOld
    {
        private Vector2 snowPos; //snow elements

        public void DrawBlizzard()
        {
            snowPos = Game1.updateFloatingObjectPositionForMovement(snowPos, new Vector2(Game1.viewport.X, Game1.viewport.Y),
                        Game1.previousViewportPosition, -1f);
            snowPos.X = snowPos.X % (16 * Game1.pixelZoom);
            Vector2 position = new Vector2();
            float num1 = -16 * Game1.pixelZoom + snowPos.X % (16 * Game1.pixelZoom);
            while ((double)num1 < Game1.viewport.Width)
            {
                float num2 = -16 * Game1.pixelZoom + snowPos.Y % (16 * Game1.pixelZoom);
                while (num2 < (double)Game1.viewport.Height)
                {
                    position.X = (int)num1;
                    position.Y = (int)num2;
                    Game1.spriteBatch.Draw(Game1.mouseCursors, position, new Microsoft.Xna.Framework.Rectangle?
                        (new Microsoft.Xna.Framework.Rectangle
                        (368 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + 150) % 1200.0) / 75 * 16, 192, 16, 16)),
                        Color.White * Game1.options.snowTransparency, 0.0f, Vector2.Zero,
                        Game1.pixelZoom + 1f / 1000f, SpriteEffects.None, 1f);
                    num2 += 16 * Game1.pixelZoom;
                }
                num1 += 16 * Game1.pixelZoom;
            }
        }
    }
}
