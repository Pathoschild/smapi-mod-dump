/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Hashing;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;

namespace AchtuurCore.Utility;
internal class GameAssetLoader
{
    private static Texture2D LoadTexture(string path) => ModEntry.Instance.Helper.GameContent.Load<Texture2D>(path);

    Texture2D sourceTexture;
    Color[] sourceTextureColors;

    List<AssetColors> AssetColorsList = new();

    public GameAssetLoader(string assetPath) : this(LoadTexture(assetPath))
    { 
    }

    public GameAssetLoader(Texture2D sourceTexture)
    {
        this.sourceTexture = sourceTexture;
        sourceTextureColors = new Color[sourceTexture.Width * sourceTexture.Height];
        sourceTexture.GetData(sourceTextureColors);
    }

    public void AddAssetColor(IEnumerable<AssetColors> assetColors)
    {
        foreach (AssetColors ac in assetColors)
            AddAssetColor(ac);
    }

    public void AddAssetColor(AssetColors assetColors)
    {
        AssetColorsList.Add(assetColors);
    }

    public void AddAssetColor(string name, int tileX, int tileY, int tileSize=64)
    {
        AssetColorsList.Add(new(name, tileX, tileY, tileSize));
    }

    public Dictionary<string, Texture2D> GetAssetTextures()
    {
        // make sure all colors are extracted
        ExtractColors();

        return this.AssetColorsList
            .Select(ac => (ac.name, CreateTexture(ac)))
            .ToDictionary(t => t.Item1, t => t.Item2);
    }

    public void ExtractColors()
    {
        foreach (AssetColors assetColor in AssetColorsList)
            ExtractColorOfAsset(assetColor);
    }

    /// <summary>
    /// Resize the read assets to a new tile size
    /// </summary>
    /// <param name="new_tile_size"></param>
    public void ResizeAssets(int new_tile_size)
    {
        foreach (AssetColors assetColors in AssetColorsList)
            ResizeAsset(assetColors, new_tile_size);
    }

    private void ResizeAsset(AssetColors assetColors, int new_tile_size)
    {
        if (!assetColors.ColorsExtracted)
            ExtractColorOfAsset(assetColors);

        if (assetColors.TileSize == new_tile_size)
            return;
        Color[] new_color = new Color[new_tile_size * new_tile_size];
        int scale = new_tile_size / assetColors.TileSize;
        for(int y = 0; y < new_tile_size; y++)
        {
            for(int x = 0; x < new_tile_size; x++)
            {
                int target_idx = x + y * new_tile_size;
                int source_idx = x / scale + y / scale * assetColors.TileSize;
                new_color[target_idx] = assetColors.Colors[source_idx];
            }
        }
        assetColors.SetTileSize(new_tile_size);
        assetColors.SetColors(new_color);
    }

    private void ExtractColorOfAsset(AssetColors assetColor)
    {
        if (assetColor.ColorsExtracted)
            return;
        int columns = sourceTexture.Width / assetColor.TileSize;
        // top left corner of tile on the sheet
        int baseIndex = assetColor.SheetBaseIndex(columns);
        for(int y = 0; y < assetColor.TileSize; y++)
        {
            for(int x = 0; x < assetColor.TileSize; x++)
            {
                int targetIdx = x + y * assetColor.TileSize;
                int sourceIdx = baseIndex + x + y * sourceTexture.Width;
                assetColor.Colors[targetIdx] = sourceTextureColors[sourceIdx];
            }
        }
        assetColor.SetExtracted();
    }

    private static Texture2D CreateTexture(AssetColors ac)
    {
        Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, ac.TileSize, ac.TileSize);
        texture.SetData(ac.Colors);
        return texture;
    }
}


/// <summary>
/// Record struct to use when loading assets from menu spritesheet
/// </summary>
/// <param name="tileX"></param>
/// <param name="tileY"></param>
/// <param name="name"></param>
internal record class AssetColors(string name, int tileX, int tileY, int tileSize)
{
    public Color[] Colors { get; set; } = new Color[tileSize * tileSize];
    public bool ColorsExtracted = false;
    public int TileSize => resizeTileSize ?? tileSize;
    private int? resizeTileSize = null;

    /// <summary>
    /// Top left corner of this tile on the tilesheet, given that it has <c>columns</c> columns of <c>tileSize</c> pixels.
    /// </summary>
    public int SheetBaseIndex(int columns) => tileX * tileSize + tileY * tileSize * (tileSize * columns);

    public void SetExtracted() => ColorsExtracted = true;

    public void SetTileSize(int tileSize) => this.resizeTileSize = tileSize;

    public void SetColors(in Color[] colors)
    {
        Colors = colors;
    }
}