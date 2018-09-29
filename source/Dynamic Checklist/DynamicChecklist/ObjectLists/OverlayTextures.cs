namespace DynamicChecklist.ObjectLists
{
    using System.IO;
    using Microsoft.Xna.Framework.Graphics;
    using StardewValley;

    public static class OverlayTextures
    {
        public static Texture2D ArrowRight { get; private set; }

        public static Texture2D Heart { get; private set; }

        public static Texture2D MilkPail { get; private set; }

        public static Texture2D Shears { get; private set; }

        public static Texture2D WateringCan { get; private set; }

        public static Texture2D Crab { get; private set; }

        public static Texture2D SpeechBubble { get; private set; }

        public static Texture2D Sign { get; private set; }

        public static Texture2D Empty { get; private set; }

        public static Texture2D Plus { get; private set; }

        public static Texture2D Hand { get; private set; }

        public static void LoadTextures(string directory)
        {
            ArrowRight = LoadTexture("arrowRight.png", directory);
            Heart = LoadTexture("heart.png", directory);
            MilkPail = LoadTexture("milkPail.png", directory);
            Shears = LoadTexture("shears.png", directory);
            WateringCan = LoadTexture("wateringCan.png", directory);
            Crab = LoadTexture("crab.png", directory);
            SpeechBubble = LoadTexture("speechBubble.png", directory);
            Sign = LoadTexture("Sign.png", directory);
            Empty = LoadTexture("empty.png", directory);
            Plus = LoadTexture("plus.png", directory);
            Hand = LoadTexture("hand.png", directory);
        }

        private static Texture2D LoadTexture(string texName, string directory)
        {
            var textureStream = new FileStream(Path.Combine(directory, "Resources", texName), FileMode.Open);
            var t = Texture2D.FromStream(Game1.graphics.GraphicsDevice, textureStream);
            return t;
        }
    }
}
