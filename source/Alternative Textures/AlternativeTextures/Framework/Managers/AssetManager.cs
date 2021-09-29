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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeTextures.Framework.Managers
{
    internal class AssetManager : IAssetLoader
    {
        internal string assetFolderPath;
        internal Dictionary<string, Texture2D> toolNames = new Dictionary<string, Texture2D>();

        private Texture2D _paintBucketTexture;
        private Texture2D _scissorsTexture;
        private Texture2D _paintBrushEmptyTexture;
        private Texture2D _paintBrushFilledTexture;

        public AssetManager(IModHelper helper)
        {
            // Get the asset folder path
            assetFolderPath = helper.Content.GetActualAssetKey(Path.Combine("Framework", "Assets"), ContentSource.ModFolder);

            // Load in the assets
            _paintBucketTexture = helper.Content.Load<Texture2D>(Path.Combine(assetFolderPath, "PaintBucket.png"));
            _scissorsTexture = helper.Content.Load<Texture2D>(Path.Combine(assetFolderPath, "Scissors.png"));
            _paintBrushEmptyTexture = helper.Content.Load<Texture2D>(Path.Combine(assetFolderPath, "PaintBrushEmpty.png"));
            _paintBrushFilledTexture = helper.Content.Load<Texture2D>(Path.Combine(assetFolderPath, "PaintBrushFilled.png"));

            // Setup toolNames
            toolNames.Add("PaintBucket", _paintBucketTexture);
            toolNames.Add("Scissors", _scissorsTexture);
            toolNames.Add("PaintBrush_Empty", _paintBrushEmptyTexture);
            toolNames.Add("PaintBrush_Filled", _paintBrushFilledTexture);
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (toolNames.Any(n => asset.AssetNameEquals($"{AlternativeTextures.TOOL_TOKEN_HEADER}{n.Key}")))
            {
                return true;
            }
            return AlternativeTextures.textureManager.GetAllTextures().Any(t => asset.AssetNameEquals($"{AlternativeTextures.TEXTURE_TOKEN_HEADER}{t.GetTokenId()}"));
        }

        public T Load<T>(IAssetInfo asset)
        {
            if (toolNames.Any(n => asset.AssetNameEquals($"{AlternativeTextures.TOOL_TOKEN_HEADER}{n.Key}")))
            {
                return (T)(object)toolNames.First(n => asset.AssetNameEquals($"{AlternativeTextures.TOOL_TOKEN_HEADER}{n.Key}")).Value;
            }

            var textureModel = AlternativeTextures.textureManager.GetAllTextures().First(t => asset.AssetNameEquals($"{AlternativeTextures.TEXTURE_TOKEN_HEADER}{t.GetTokenId()}"));
            return (T)(object)textureModel.Textures.First();
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
