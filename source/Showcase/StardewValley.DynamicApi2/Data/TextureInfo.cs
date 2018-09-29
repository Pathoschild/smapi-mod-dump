using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;

namespace Igorious.StardewValley.DynamicApi2.Data
{
    public sealed class TextureInfo
    {
        public static TextureInfo Furnitures { get; } = new TextureInfo("Furniture", () => Furniture.furnitureTexture);
        public static TextureInfo Objects { get; } = new TextureInfo("Objects", () => Game1.objectSpriteSheet);
        public static TextureInfo Weapons { get; } = new TextureInfo("Weapons", () => Tool.weaponsTexture);
        public static TextureInfo Tools { get; } = new TextureInfo("Tools", () => Game1.toolSpriteSheet);

        private readonly Func<Texture2D> _getTexture;

        public TextureInfo(string name, Func<Texture2D> getTexture, int spriteWidth = 16, int spriteHeigth = 16)
        {
            _getTexture = getTexture;
            Name = name;
            SpriteWidth = spriteWidth;
            SpriteHeigth = spriteHeigth;
        }

        public string Name { get; }
        public int SpriteWidth { get; }
        public int SpriteHeigth { get; }
        public Texture2D Texture => _getTexture();

        public Rectangle GetSourceRect(int index, int spriteTileWidth = 1, int spriteTileHeight = 1) => GetSourceRect(Texture, index, spriteTileWidth, spriteTileHeight);

        public Rectangle GetSourceRect(Texture2D texture, int index, int spriteTileWidth = 1, int spriteTileHeight = 1)
        {
            var rowLength = texture.Width / SpriteWidth;
            var x = index % rowLength * SpriteWidth;
            var spritePixelWidth = SpriteWidth * spriteTileWidth;
            if (x + spritePixelWidth > texture.Width)
            {
                spriteTileHeight += (x + spritePixelWidth) / texture.Width;
                x = 0;
                spritePixelWidth = texture.Width;
            }
            return new Rectangle(x, index / rowLength * SpriteHeigth, spritePixelWidth, SpriteHeigth * spriteTileHeight);
        }
    }
}