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
using System.Collections.Generic;
using System.IO;

namespace AlternativeTextures.Framework.Managers
{
    internal class AssetManager
    {
        internal string assetFolderPath;
        internal Dictionary<string, TextureData> toolKeyToData = new Dictionary<string, TextureData>();

        private IModHelper _helper;

        internal class TextureData
        {
            public string FilePath { get; set; }
            public Texture2D Texture { get; set; }
        }

        public AssetManager(IModHelper helper)
        {
            _helper = helper;

            // Get the asset folder path
            assetFolderPath = helper.ModContent.GetInternalAssetName(Path.Combine("Framework", "Assets")).Name;

            // Setup toolNames
            toolKeyToData.Add($"{AlternativeTextures.TOOL_TOKEN_HEADER}PaintBucket", new TextureData() { FilePath = Path.Combine(assetFolderPath, "PaintBucket.png") });
            toolKeyToData.Add($"{AlternativeTextures.TOOL_TOKEN_HEADER}Scissors", new TextureData() { FilePath = Path.Combine(assetFolderPath, "Scissors.png") });
            toolKeyToData.Add($"{AlternativeTextures.TOOL_TOKEN_HEADER}SprayCan", new TextureData() { FilePath = Path.Combine(assetFolderPath, "SprayCan.png") });
            toolKeyToData.Add($"{AlternativeTextures.TOOL_TOKEN_HEADER}SprayCanRare", new TextureData() { FilePath = Path.Combine(assetFolderPath, "SprayCanRare.png") });
            toolKeyToData.Add($"{AlternativeTextures.TOOL_TOKEN_HEADER}PaintBrush_Empty", new TextureData() { FilePath = Path.Combine(assetFolderPath, "PaintBrushEmpty.png") });
            toolKeyToData.Add($"{AlternativeTextures.TOOL_TOKEN_HEADER}PaintBrush_Filled", new TextureData() { FilePath = Path.Combine(assetFolderPath, "PaintBrushFilled.png") });
            toolKeyToData.Add($"{AlternativeTextures.TOOL_TOKEN_HEADER}Catalogue", new TextureData() { FilePath = Path.Combine(assetFolderPath, "Catalogue.png") });
        }

        internal Texture2D GetPaintBucketTexture()
        {
            return toolKeyToData[$"{AlternativeTextures.TOOL_TOKEN_HEADER}PaintBucket"].Texture;
        }

        internal Texture2D GetScissorsTexture()
        {
            return toolKeyToData[$"{AlternativeTextures.TOOL_TOKEN_HEADER}Scissors"].Texture;
        }

        internal Texture2D GetSprayCanTexture(bool getRareTexture = false)
        {
            return getRareTexture ? toolKeyToData[$"{AlternativeTextures.TOOL_TOKEN_HEADER}SprayCanRare"].Texture : toolKeyToData[$"{AlternativeTextures.TOOL_TOKEN_HEADER}SprayCan"].Texture;
        }

        internal Texture2D GetPaintBrushEmptyTexture()
        {
            return toolKeyToData[$"{AlternativeTextures.TOOL_TOKEN_HEADER}PaintBrush_Empty"].Texture;
        }

        internal Texture2D GetPaintBrushFilledTexture()
        {
            return toolKeyToData[$"{AlternativeTextures.TOOL_TOKEN_HEADER}PaintBrush_Filled"].Texture;
        }

        internal Texture2D GetCatalogueTexture()
        {
            return toolKeyToData[$"{AlternativeTextures.TOOL_TOKEN_HEADER}Catalogue"].Texture;
        }
    }
}
