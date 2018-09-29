using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Igorious.StardewValley.ColoredChestsMod.Utils
{
    public static class Textures
    {
        private static Texture2D _chestTint;
        public static Texture2D ChestTint => _chestTint ?? (_chestTint = LoadTexture(nameof(ChestTint)));

        private static Texture2D _colorIcon;
        public static Texture2D ColorIcon => _colorIcon ?? (_colorIcon = LoadTexture(nameof(ColorIcon)));

        private static Texture2D LoadTexture(string name)
        {
            using (var imageStream = new FileStream(Path.Combine(ColoredChestsMod.RootPath, $@"Resources\{name}.png"), FileMode.Open))
            {
                return Texture2D.FromStream(Game1.graphics.GraphicsDevice, imageStream);
            }
        }
    }
}