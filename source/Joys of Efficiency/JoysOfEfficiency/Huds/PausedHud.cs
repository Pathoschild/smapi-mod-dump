using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace JoysOfEfficiency.Huds
{
    internal class PausedHud
    {
        private static ITranslationHelper Translation => InstanceHolder.Translation;
        public static void DrawPausedHud()
        {
            SpriteFont font = Game1.dialogueFont;
            string text = Translation.Get("hud.paused");
            Vector2 stringSize = font.MeasureString(text);
            int x = InstanceHolder.Config.PauseNotificationX;
            int y = InstanceHolder.Config.PauseNotificationY;
            int width = 16 + (int)stringSize.X + 16;
            int height = 16 + (int)stringSize.Y + 16;

            Util.DrawWindow(x, y, width, height);
            Utility.drawTextWithShadow(Game1.spriteBatch, text, font, new Vector2(x + 16, y + 16 + 8), Color.Black);
        }
    }
}
