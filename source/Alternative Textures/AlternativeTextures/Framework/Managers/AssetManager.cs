/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeTextures.Framework.Managers
{
    internal class AssetManager
    {
        // Tilesheet related for decorations (wallpaper / floor)
        private TextureManager _textureManager;

        internal string assetFolderPath;
        internal Dictionary<string, Texture2D> toolNames = new Dictionary<string, Texture2D>();

        private Texture2D _paintBucketTexture;
        private Texture2D _scissorsTexture;
        private Texture2D _paintBrushEmptyTexture;
        private Texture2D _paintBrushFilledTexture;

        public AssetManager(IModHelper helper, TextureManager textureManager)
        {
            // Get the asset folder path
            assetFolderPath = helper.ModContent.GetInternalAssetName(Path.Combine("Framework", "Assets")).Name;

            // Load in the assets
            _paintBucketTexture = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "PaintBucket.png"));
            _scissorsTexture = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "Scissors.png"));
            _paintBrushEmptyTexture = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "PaintBrushEmpty.png"));
            _paintBrushFilledTexture = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "PaintBrushFilled.png"));

            // Setup toolNames
            toolNames.Add("PaintBucket", _paintBucketTexture);
            toolNames.Add("Scissors", _scissorsTexture);
            toolNames.Add("PaintBrush_Empty", _paintBrushEmptyTexture);
            toolNames.Add("PaintBrush_Filled", _paintBrushFilledTexture);
            toolNames.Add($"{AlternativeTextures.TOOL_TOKEN_HEADER}PaintBucket", _paintBucketTexture);
            toolNames.Add($"{AlternativeTextures.TOOL_TOKEN_HEADER}Scissors", _scissorsTexture);
            toolNames.Add($"{AlternativeTextures.TOOL_TOKEN_HEADER}PaintBrush_Empty", _paintBrushEmptyTexture);
            toolNames.Add($"{AlternativeTextures.TOOL_TOKEN_HEADER}PaintBrush_Filled", _paintBrushFilledTexture);

            // Get the TextureMananger
            _textureManager = textureManager;
        }

        internal Texture2D GetPaintBucketTexture()
        {
            return _paintBucketTexture;
        }

        internal Texture2D GetScissorsTexture()
        {
            return _scissorsTexture;
        }

        internal Texture2D GetPaintBrushEmptyTexture()
        {
            return _paintBrushEmptyTexture;
        }

        internal Texture2D GetPaintBrushFilledTexture()
        {
            return _paintBrushFilledTexture;
        }
    }
}
