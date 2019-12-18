using System;
using System.Linq;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using StardewModdingAPI.Events;
using StardewValley;

namespace JoysOfEfficiency.Huds
{
    internal class FpsCounter
    {
        private static readonly Logger Logger = new Logger("FpsCounter");

        private static double _fps;

        private static int _lastMilliseconds = Environment.TickCount;

        private static int _frameCounter;

        public static void OnHudDraw(object sender, RenderingHudEventArgs args)
        {
            _frameCounter++;
            if (_frameCounter == 50)
            {
                int current = Environment.TickCount;
                int delta = current - _lastMilliseconds;
                _fps = (1000.0 * _frameCounter / delta);
                _frameCounter = 0;
                _lastMilliseconds = Environment.TickCount;
            }
        }

        public static void PostHudDraw(object sender, RenderedHudEventArgs args)
        {
            if (ModEntry.DebugMode)
            {
                Draw();
            }
        }

        public static void Draw()
        {
            string fpsString = $"{_fps:f1}fps";
            string str = Util.GetNearbyChests().Sum(c=>c.items.Count).ToString();
            Util.DrawSimpleTextbox(Game1.spriteBatch, str, Game1.smallFont, null);
            Util.DrawSimpleTextbox(Game1.spriteBatch, fpsString, 0, 0, Game1.smallFont, null);
        }
    }
}
