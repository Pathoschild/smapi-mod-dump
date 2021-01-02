/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
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

namespace CombineMachines.Helpers
{
    public static class TextureHelpers
    {
        private static Texture2D _EmojiSpritesheet;
        public static Texture2D EmojiSpritesheet
        {
            get
            {
                if (_EmojiSpritesheet == null || _EmojiSpritesheet.IsDisposed)
                {
                    _EmojiSpritesheet = ModEntry.ModInstance.Helper.Content.Load<Texture2D>("LooseSprites/emojis", ContentSource.GameContent);
                }
                return _EmojiSpritesheet;
            }
        }

        private static Texture2D _PlayerStatusList;
        public static Texture2D PlayerStatusList
        {
            get
            {
                if (_PlayerStatusList == null || _PlayerStatusList.IsDisposed)
                {
                    _PlayerStatusList = ModEntry.ModInstance.Helper.Content.Load<Texture2D>("LooseSprites/PlayerStatusList", ContentSource.GameContent);
                }
                return _PlayerStatusList;
            }
        }

        private static Dictionary<uint, SolidColorTexture> IndexedColorTextures { get; } = new Dictionary<uint, SolidColorTexture>();

        public static SolidColorTexture GetSolidColorTexture(GraphicsDevice GD, Color color)
        {
            if (IndexedColorTextures.TryGetValue(color.PackedValue, out SolidColorTexture ExistingTexture))
                return ExistingTexture;
            else
            {
                SolidColorTexture Texture = new SolidColorTexture(GD, color);
                IndexedColorTextures.Add(color.PackedValue, Texture);
                return Texture;
            }
        }
    }

    /// <summary>Creates a 1x1 pixel texture of a solid color</summary>
    public class SolidColorTexture : Texture2D
    {
        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                if (value != _color)
                {
                    _color = value;
                    SetData<Color>(new Color[] { _color });
                }
            }
        }

        public SolidColorTexture(GraphicsDevice GraphicsDevice) : base(GraphicsDevice, 1, 1) { }
        public SolidColorTexture(GraphicsDevice GraphicsDevice, Color color)
            : base(GraphicsDevice, 1, 1)
        {
            Color = color;
        }
    }
}