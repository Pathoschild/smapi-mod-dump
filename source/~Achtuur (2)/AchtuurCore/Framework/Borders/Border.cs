/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Extensions;
using AchtuurCore.Utility;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.GameData.FloorsAndPaths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Borders;

public enum BorderType
{
    Top,
    TSection,
    Bottom,
}
public class Border
{
    static Dictionary<string, Texture2D> BorderTextures;

    const int TileSize = 64;
    static Vector2 UnitTile = Vector2.One * TileSize;
    static Vector2 UnitTileX = Vector2.UnitX * TileSize;
    static Vector2 UnitTileY = Vector2.UnitY * TileSize;

    List<Label> Labels;
    List<Vector2> LabelPositions;
    public BorderType TopBorder = BorderType.TSection;
    public BorderType BottomBorder = BorderType.TSection;
    public Vector2 Position;
    public float? FixedWidth;

    public bool IsEmpty => Labels.Count == 0;
    public int LabelCount => Labels.Count;
    /// <summary>
    /// Total Width of the border, including the outside borders
    /// </summary>
    public float TotalWidth => Width;
    /// <summary>
    /// Total height of the border, including the outside borders
    /// </summary>
    public float TotalHeight => Height;
    /// <summary>
    /// Width of the border without taking into account the outside borders. This is the width of the 'inside' part.
    /// </summary>
    private float Width => Math.Max(FixedWidth ?? 0, Labels.Max(lab => lab.DrawSize.X)) + Label.Margin();
    /// <summary>
    /// Height of the border without taking into account the outside borders. This is the height of the 'inside' part.
    /// </summary>
    private float Height => Labels.Sum(lab => lab.DrawSize.Y + Label.Margin()) + BonusHeight;

    private float BonusHeight => (TopBorder == BorderType.TSection || BottomBorder == BorderType.TSection) ? Label.Margin() : 0;
    /// <summary>
    /// Width in tiles
    /// </summary>
    private float WidthTiles => (float)Width / (float)TileSize;
    /// <summary>
    /// Height in tiles
    /// </summary>
    private float HeightTiles => (float)Height / (float)TileSize;

    /// <summary>
    /// Coordinates of the top left of the bottom right corner tile of the border, relative to the top left corner
    /// </summary>
    public Vector2 BottomRight => UnitTile * new Vector2(WidthTiles, HeightTiles);
    private Vector2 bottom_right_x => Vector2.UnitX * BottomRight.X;
    private Vector2 bottom_right_y => Vector2.UnitY * BottomRight.Y;
    public Border() : this(Enumerable.Empty<Label>())
    {
    }

    public Border(Label label) : this(label.Yield())
    {
    }

    public Border(IEnumerable<Label> labels)
    {
        LoadBorderTextureAssets();
        TopBorder = BorderType.TSection;
        BottomBorder = BorderType.TSection;
        Labels = new();
        LabelPositions = new();
        foreach (Label lab in labels)
            AddLabel(lab);
    }

    public void AddLabel(IEnumerable<Label> labels)
    {
        foreach (Label lab in labels)
            AddLabel(lab);
    }

    public void AddLabel(Label label)
    {
        if (label is null)
            return;
        // height is current height, plus an additional margin for the new label
        float height;
        if (LabelPositions.Count == 0)
        {
            height = TileSize / 2;
        } 
        else
        {
            height = LabelPositions.Last().Y + Labels.Last().DrawSize.Y;
        }
        // x offset is left side of the border, plus the margin
        float x_offset = TileSize / 2;
        LabelPositions.Add(new Vector2(x_offset, height) + Label.MarginSize);
        Labels.Add(label);
    }
    
    public void AddLabel(string text)
    {
        AddLabel(new Label(text));
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        this.Position = position;
        DrawBackground(spriteBatch);
        DrawSides(spriteBatch);
        DrawTopBorder(spriteBatch);
        DrawBottomBorder(spriteBatch);
        DrawLabels(spriteBatch);
    }

    private void DrawTopBorder(SpriteBatch spriteBatch)
    {
        List<Texture2D> textures = GetTexture(TopBorder);
        // bar should go in between the top left and top right
        // so width is total width minus 1 tile, since top left and top right overshoot about half a tile
        Vector2 bar_scale = new Vector2(WidthTiles - 1f, 1f);
        DrawMenuTexture(spriteBatch, textures[0], Position);
        DrawMenuTexture(spriteBatch, textures[1], Position + UnitTileX, bar_scale);
        DrawMenuTexture(spriteBatch, textures[2], Position + bottom_right_x);
    }

    private void DrawLabels(SpriteBatch spriteBatch)
    {
        // T sections need a little bit of an extra offset to look the same as top section
        Vector2 extra_offset = Vector2.Zero;
        if (TopBorder == BorderType.TSection)
            extra_offset = Vector2.UnitY * Label.Margin()/2;

        for (int i = 0; i < Labels.Count; i++)
        {
            Vector2 position = Position + LabelPositions[i] + extra_offset;
            Labels[i].Draw(spriteBatch, position);

            // debug draw
            if (ModEntry.DebugDraw)
                spriteBatch.DrawBorder(position, Labels[i].DrawSize, Color.Red);
        }
    }

    private void DrawSides(SpriteBatch spriteBatch)
    {
        // same reasoning as top and bottom border, except now for the sides (height instead of width
        Vector2 scale = new Vector2(1f, HeightTiles - 1f);
        DrawMenuTexture(spriteBatch, BorderTextures["LeftBorder"], Position + UnitTileY, scale);
        DrawMenuTexture(spriteBatch, BorderTextures["RightBorder"], Position + UnitTileY + bottom_right_x, scale);
    }

    private void DrawBottomBorder(SpriteBatch spriteBatch)
    {
        List<Texture2D> textures = GetTexture(BottomBorder);
        // bar should go in between the top left and top right
        // so width is width_tiles minus 1 tile, since top left and top right overshoot about half a tile
        Vector2 bar_scale = new Vector2(WidthTiles - 1f, 1f);
        DrawMenuTexture(spriteBatch, textures[0], Position + bottom_right_y);
        DrawMenuTexture(spriteBatch, textures[1], Position + bottom_right_y + UnitTileX, bar_scale);
        DrawMenuTexture(spriteBatch, textures[2], Position + BottomRight);
    }

    private void DrawBackground(SpriteBatch spriteBatch)
    {
        Vector2 backgroundPosition = Position + UnitTileX * 0.5f + UnitTileY * 0.5f;
        // scale of 1 means 1 tile for background
        // so background should be scaled up to the middle parts of border (mid_top, side, mid_bottom)
        Vector2 bgscale = new Vector2(WidthTiles, HeightTiles);
        DrawMenuTexture(spriteBatch, BorderTextures["Background"], backgroundPosition, bgscale);
    }

    private static void DrawMenuTexture(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2? scale = null)
    {
        spriteBatch.DrawTexture(texture, position, scale ?? Vector2.One);
    }

    /// <summary>
    /// Returns border textures for given border type.
    /// 
    /// Returns 3 items, first the left texture, then the middle texture, then the right texture.
    /// </summary>
    /// <param name="border_type"></param>
    /// <returns></returns>
    private static List<Texture2D> GetTexture(BorderType border_type)
    {
        switch (border_type)
        {
            case BorderType.Top:
                return new List<Texture2D>
                {
                    BorderTextures["TopLeftBorder"],
                    BorderTextures["TopBorder"],
                    BorderTextures["TopRightBorder"],
                };
            case BorderType.TSection:
                return new List<Texture2D>
                {
                    BorderTextures["TSectionEast"],
                    BorderTextures["InnerHorizontal"],
                    BorderTextures["TSectionWest"],
                };
            case BorderType.Bottom:
                return new List<Texture2D>
                {
                    BorderTextures["BottomLeftBorder"],
                    BorderTextures["BottomBorder"],
                    BorderTextures["BottomRightBorder"],
                };
            default:
                return new();
        }
    }

    internal static void LoadBorderTextureAssets()
    {
        if (BorderTextures is not null)
            return;

        GameAssetLoader assetLoader = new("Maps/MenuTiles");

        AssetColors[] assetColors = new[]
        {
            new AssetColors("TopLeftBorder", 0, 0, tileSize: 64),
            new AssetColors("TSectionEast", 0, 1, tileSize: 64),
            new AssetColors("LeftBorder", 0, 2, tileSize: 64),
            new AssetColors("BottomLeftBorder", 0, 3, tileSize: 64),
            new AssetColors("TSectionSouth", 1, 0, tileSize: 64),
            new AssetColors("InnerVertical", 1, 1, tileSize: 64),
            new AssetColors("Background", 1, 2, tileSize: 64),
            new AssetColors("TSectionNorth", 1, 3, tileSize: 64),
            new AssetColors("TopBorder", 2, 0, tileSize: 64),
            new AssetColors("InnerHorizontal", 2, 1, tileSize: 64),
            new AssetColors("BottomBorder", 2, 3, tileSize: 64),
            new AssetColors("TopRightBorder", 3, 0, tileSize: 64),
            new AssetColors("TSectionWest", 3, 1, tileSize: 64),
            new AssetColors("RightBorder", 3, 2, tileSize: 64),
            new AssetColors("BottomRightBorder", 3, 3, tileSize: 64),
        };
        assetLoader.AddAssetColor(assetColors);
        BorderTextures = assetLoader.GetAssetTextures();
    }

}

