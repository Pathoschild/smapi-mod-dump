/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.GameData.FloorsAndPaths;
using System;
using System.Collections.Generic;
using System.Linq;
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
    const int TileSize = 64;
    static Vector2 UnitTile = Vector2.One * TileSize;
    static Vector2 UnitTileX = Vector2.UnitX * TileSize;
    static Vector2 UnitTileY = Vector2.UnitY * TileSize;

    List<Label> Labels;
    List<Vector2> LabelPositions;
    public BorderType TopBorder = BorderType.TSection;
    public BorderType BottomBorder = BorderType.TSection;
    public Vector2 Position;
    public float FixedWidth;

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
    private float Width => Math.Max(FixedWidth, Labels.Max(text => text.DrawSize.X));
    /// <summary>
    /// Height of the border without taking into account the outside borders. This is the height of the 'inside' part.
    /// </summary>
    private float Height => Labels.Sum(text => text.DrawSize.Y) + BonusHeight;

    private float BonusHeight => (BottomBorder == BorderType.TSection) ? Label.Margin() : 0;
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
    public Border()
    {
        TopBorder = BorderType.TSection;
        BottomBorder = BorderType.TSection;
        Labels = new();
        LabelPositions = new();
    }

    public Border(Label label)
    {
        TopBorder = BorderType.TSection;
        BottomBorder = BorderType.TSection;
        Labels = new();
        LabelPositions = new();
        AddLabelText(label);
    }

    public Border(IEnumerable<Label> labels)
    {
        TopBorder = BorderType.TSection;
        BottomBorder = BorderType.TSection;
        Labels = new();
        LabelPositions = new();
        foreach (Label label in labels)
            AddLabelText(label);
    }

    public void AddLabelText(IEnumerable<Label> label_text)
    {
        foreach (Label label in label_text)
            AddLabelText(label);
    }

    public void AddLabelText(Label label_text)
    {
        if (label_text is null)
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
        Labels.Add(label_text);
    }
    
    public void AddLabelText(string text)
    {
        AddLabelText(new Label(text));
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
        BorderDrawer.DrawMenuTexture(spriteBatch, textures[0], Position);
        BorderDrawer.DrawMenuTexture(spriteBatch, textures[1], Position + UnitTileX, bar_scale);
        BorderDrawer.DrawMenuTexture(spriteBatch, textures[2], Position + bottom_right_x);
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
            spriteBatch.DrawBorder(position, Labels[i].DrawSize, Color.Red);
        }
    }

    private void DrawSides(SpriteBatch spriteBatch)
    {
        // same reasoning as top and bottom border, except now for the sides (height instead of width
        Vector2 scale = new Vector2(1f, HeightTiles - 1f);
        BorderDrawer.DrawMenuTexture(spriteBatch, BorderDrawer.BorderTextures["LeftBorder"], Position + UnitTileY, scale);
        BorderDrawer.DrawMenuTexture(spriteBatch, BorderDrawer.BorderTextures["RightBorder"], Position + UnitTileY + bottom_right_x, scale);
    }

    private void DrawBottomBorder(SpriteBatch spriteBatch)
    {
        List<Texture2D> textures = GetTexture(BottomBorder);
        // bar should go in between the top left and top right
        // so width is width_tiles minus 1 tile, since top left and top right overshoot about half a tile
        Vector2 bar_scale = new Vector2(WidthTiles - 1f, 1f);
        BorderDrawer.DrawMenuTexture(spriteBatch, textures[0], Position + bottom_right_y);
        BorderDrawer.DrawMenuTexture(spriteBatch, textures[1], Position + bottom_right_y + UnitTileX, bar_scale);
        BorderDrawer.DrawMenuTexture(spriteBatch, textures[2], Position + BottomRight);
    }

    private void DrawBackground(SpriteBatch spriteBatch)
    {
        Vector2 backgroundPosition = Position + UnitTileX * 0.5f + UnitTileY * 0.5f;
        // scale of 1 means 1 tile for background
        // so background should be scaled up to the middle parts of border (mid_top, side, mid_bottom)
        Vector2 bgscale = new Vector2(WidthTiles, HeightTiles);
        BorderDrawer.DrawMenuTexture(spriteBatch, BorderDrawer.BorderTextures["Background"], backgroundPosition, bgscale);
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
                    BorderDrawer.BorderTextures["TopLeftBorder"],
                    BorderDrawer.BorderTextures["TopBorder"],
                    BorderDrawer.BorderTextures["TopRightBorder"],
                };
            case BorderType.TSection:
                return new List<Texture2D>
                {
                    BorderDrawer.BorderTextures["TSectionEast"],
                    BorderDrawer.BorderTextures["InnerHorizontal"],
                    BorderDrawer.BorderTextures["TSectionWest"],
                };
            case BorderType.Bottom:
                return new List<Texture2D>
                {
                    BorderDrawer.BorderTextures["BottomLeftBorder"],
                    BorderDrawer.BorderTextures["BottomBorder"],
                    BorderDrawer.BorderTextures["BottomRightBorder"],
                };
            default:
                return new();
        }
    }
}
