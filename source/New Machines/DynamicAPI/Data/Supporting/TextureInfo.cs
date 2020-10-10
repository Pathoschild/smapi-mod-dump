/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public sealed class TextureInfo
    {
        public static IReadOnlyDictionary<TextureType, TextureInfo> Default = new Dictionary<TextureType, TextureInfo>
        {
            { TextureType.Items, new TextureInfo(16, 16, () => Game1.objectSpriteSheet) },
            { TextureType.Craftables, new TextureInfo(16, 32, () => Game1.bigCraftableSpriteSheet) },
            { TextureType.Crops, new TextureInfo(128, 32, () => Game1.cropSpriteSheet) },
            { TextureType.Trees, new TextureInfo(432, 80, () => FruitTree.texture) },
        };

        private Func<Texture2D> GetTexture { get; }
        public int SpriteWidth { get; }
        public int SpriteHeight { get; }
        public Texture2D Texture => GetTexture();

        public TextureInfo(int spriteWidth, int spriteHeight, Func<Texture2D> getTexture)
        {
            SpriteWidth = spriteWidth;
            SpriteHeight = spriteHeight;
            GetTexture = getTexture;
        }

        public Rectangle GetSourceRect(int index, int length = 1, int height = 1)
        {
            return GetSourceRect(Texture, index, length, height);
        }

        public Rectangle GetSourceRect(Texture2D texture, int index, int length = 1, int height = 1)
        {
            var rowLength = texture.Width / SpriteWidth;
            var x = index % rowLength * SpriteWidth;
            var width = SpriteWidth * length;
            if (x + width > texture.Width)
            {
                height += (x + width) / texture.Width;
                x = 0;
                width = texture.Width;
            }
            return new Rectangle(x, index / rowLength * SpriteHeight, width, SpriteHeight * height);
        }
    }
}