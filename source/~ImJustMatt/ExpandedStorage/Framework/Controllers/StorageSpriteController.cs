/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ImJustMatt.ExpandedStorage.Framework.Controllers
{
    internal class StorageSpriteController
    {
        private readonly int _depth;
        private readonly int _frames;
        private readonly string _path;
        private readonly bool _playerColor;

        private Texture2D _texture;

        public StorageSpriteController(StorageController storage)
        {
            _frames = storage.Frames;
            _depth = storage.Depth;
            _playerColor = storage.PlayerColor;
            _path = PathUtilities.NormalizePath(storage.Path);
        }

        /// <summary>Property to access the SpriteSheet image.</summary>
        public Texture2D Texture
        {
            get
            {
                if (_texture != null) return _texture;
                _texture = Game1.content.Load<Texture2D>(_path);
                Width = _texture.Width / Math.Max(1, _frames);
                Height = _playerColor ? _texture.Height / 3 : _texture.Height;
                TileWidth = Width / 16;
                TileHeight = (_depth is { } depth && depth > 0 ? depth : Height - 16) / 16;
                return _texture;
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        internal float ScaleSize
        {
            get
            {
                var tilesWide = Width / 16f;
                var tilesHigh = Height / 16f;
                return tilesWide switch
                {
                    >= 7 => 0.5f,
                    >= 6 => 0.66f,
                    >= 5 => 0.75f,
                    _ => tilesHigh switch
                    {
                        >= 5 => 0.8f,
                        >= 3 => 1f,
                        _ => tilesWide switch
                        {
                            <= 2 => 2f,
                            <= 4 => 1f,
                            _ => 0.1f
                        }
                    }
                };
            }
        }

        public void InvalidateCache()
        {
            _texture = null;
        }

        internal void ForEachPos(int x, int y, Action<Vector2> doAction)
        {
            for (var i = 0; i < TileWidth; i++)
            {
                for (var j = 0; j < TileHeight; j++)
                {
                    var pos = new Vector2(x + i, y + j);
                    doAction.Invoke(pos);
                }
            }
        }
    }
}