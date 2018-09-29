using Microsoft.Xna.Framework;

namespace Igorious.StardewValley.DynamicApi2.Extensions
{
    public static class PointExtensions
    {
        public static Vector2 ToVector(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}