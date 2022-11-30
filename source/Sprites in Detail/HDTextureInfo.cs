/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BleakCodex/SpritesInDetail
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpritesInDetail
{
    public class HDTextureInfo
    {
        public bool Enabled { get; set; }
        public string Target { get; set; }
        public Texture2D? HDTexture { get; set; }


        public int SpriteWidth { get; set; }
        public int SpriteHeight { get; set; }

        public int SpriteOriginX { get; set; }
        public int SpriteOriginY { get; set; }


        public int WidthScale { get; set; }
        public int HeightScale { get; set; }

        public bool DisableBreath { get; set; }
        public int ChestSourceX { get; set; }
        public int ChestSourceY { get; set; }
        public int ChestSourceWidth { get; set; }
        public int ChestSourceHeight { get; set; }
        public int ChestAdjustX { get; set; }
        public int ChestAdjustY { get; set; }

        public bool IsFarmer { get; set; }

        public IManifest ContentPackManifest { get; set; }
        public Dictionary<string, string?> Conditionals { get; set; } = new Dictionary<string, string?>();

        public Dictionary<Vector2, Texture2D> PixelReplacements { get; set; } = new Dictionary<Vector2, Texture2D>();

        public HDTextureInfo(Sprite sprite, IManifest contentPackManifest, Texture2D? hdTexture, Dictionary<string, string?> conditionals, bool isFarmer = false)
        {
            ContentPackManifest = contentPackManifest;

            Target = sprite.Target;
            HDTexture = hdTexture;

            WidthScale = sprite.WidthScale ?? 4;
            HeightScale = sprite.HeightScale ?? 4;

            SpriteWidth = sprite.SpriteWidth ?? 32;
            SpriteHeight = sprite.SpriteHeight ?? 64;

            if (isFarmer)
            {
                SpriteOriginX = sprite.SpriteOriginX ?? 16;
                SpriteOriginY = sprite.SpriteOriginY ?? 128;
                IsFarmer = true;
            }
            else
            {
                //Origin will need to be a specific location on the sprite for proper positioning
                //For 1x resolution sprite it is (Sprite.SpriteWidth / 2, (float)Sprite.SpriteHeight * 3f / 4f) or (8,24)
                //For 2x resolution texture with 2x resolution sprites it is (16, 48)
                //For 4x resolution textures with 4x resolution sprites it is (32, 96)
                //For 4x resolution textures with 2x resolution sprites moved to the bottom it would be (32, 112)
                SpriteOriginX = sprite.SpriteOriginX ?? 32;
                SpriteOriginY = sprite.SpriteOriginY ?? 112;
            }

            if (!sprite.BreathType.HasValue || sprite.BreathType == BreathType.Male)
            {
                ChestSourceX = sprite.ChestSourceX ?? 24;
                ChestSourceY = sprite.ChestSourceY ?? 98;
                ChestSourceWidth = sprite.ChestSourceWidth ?? 16;
                ChestSourceHeight = sprite.ChestSourceHeight ?? 16;
                ChestAdjustX = sprite.ChestAdjustX ?? 0;
                ChestAdjustY = sprite.ChestAdjustY ?? 0;
            } else if (sprite.BreathType == BreathType.Female)
            {
                ChestSourceX = sprite.ChestSourceX ?? 24;
                ChestSourceY = sprite.ChestSourceY ?? 100;
                ChestSourceWidth = sprite.ChestSourceWidth ?? 16;
                ChestSourceHeight = sprite.ChestSourceHeight ?? 8;
                ChestAdjustX = sprite.ChestAdjustX ?? 0;
                ChestAdjustY = sprite.ChestAdjustY ?? -4;
            } else
            {
                DisableBreath = true;
            }

            Conditionals = conditionals;
        }
    }
}
