using Microsoft.Xna.Framework;

namespace StardewValleyEsp.Labels
{
    class ColorManager
    {
        private static readonly Color[] colors;

        static ColorManager()
        {
            colors = new Color[]
            {
                new Color(85, 85, byte.MaxValue),
                new Color(119, 191, byte.MaxValue),
                new Color(0, 170, 170),
                new Color(0, 234, 175),
                new Color(0, 170, 0),
                new Color(159, 236, 0),
                new Color(byte.MaxValue, 234, 18),
                new Color(byte.MaxValue, 167, 18),
                new Color(byte.MaxValue, 105, 18),
                new Color(byte.MaxValue, 0, 0),
                new Color(135, 0, 35),
                new Color(byte.MaxValue, 173, 199),
                new Color(byte.MaxValue, 117, 195),
                new Color(172, 0, 198),
                new Color(143, 0, byte.MaxValue),
                new Color(89, 11, 142),
                new Color(64, 64, 64),
                new Color(100, 100, 100),
                new Color(200, 200, 200),
                new Color(254, 254, 254)
            };
        }

        public Color ColorFromInt(int i)
        {
            if (i < 1 || i > colors.Length) return Color.Black;
            return colors[i - 1];
        }
    }
}
