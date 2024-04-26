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
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace AchtuurCore.Framework;

public abstract class Overlay
{

    /// <summary>
    /// Green tile placement texture, same one that is used when player is trying to place objects or charges up tools
    /// </summary>
    public static Texture2D GreenTilePlacementTexture;
    /// <summary>
    /// Red tile placement texture, same on that is used when player is trying to place object but is unable to
    /// </summary>
    public static Texture2D RedTilePlacementTexture;
    /// <summary>
    /// Grayscale version of green tile placement texture.
    /// </summary>
    public static Texture2D TilePlacementTexture;

    /// <summary>
    /// Size of each tile (in pixels?)
    /// </summary>
    protected int tileSize;

    /// <summary>
    /// Gap between tiles in pixels
    /// </summary>
    protected int tileGap;


    public bool Enabled { get; set; }


    public Overlay()
    {
        this.Enabled = false;
        this.tileGap = 0;
        this.tileSize = Game1.tileSize;
    }

    public virtual void Enable()
    {
        this.Enabled = true;
    }

    public virtual void Disable()
    {
        this.Enabled = false;
    }

    public void Toggle()
    {
        this.Enabled = !this.Enabled;
    }

    /// <summary>
    /// <para>Draw overlay if possible, heavy inspiration from Pathoschild's Automate.OverlayMenu.DrawWorld</para>
    /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Automate/Framework/OverlayMenu.cs#L14"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DrawOverlay(SpriteBatch spriteBatch)
    {
        // Check if overlay can be drawn
        if (!this.Enabled
            || !Context.IsWorldReady
            || Game1.player.EventActor
            )
            return;

        // Update tilesize
        this.tileSize = Game1.tileSize;

        Debug.DebugOnlyExecute(() =>
        {
            float textHeight = Game1.dialogueFont.MeasureString("a").Y;

            Rectangle visibleTiles = Drawing.GetVisibleArea();
            Vector2 visibleCoords = new(visibleTiles.Width - visibleTiles.X, visibleTiles.Height + visibleTiles.Y);

            spriteBatch.DrawString(Game1.dialogueFont, visibleCoords.ToString(), new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(Game1.dialogueFont, Game1.currentLocation.ToString(), new Vector2(0, textHeight), Color.White);
        });


        DrawOverlayToScreen(spriteBatch);
    }

    /// <summary>
    /// Draw the actual overlay to the screen
    /// </summary>
    /// <param name="spriteBatch"></param>
    protected abstract void DrawOverlayToScreen(SpriteBatch spriteBatch);

    /// <summary>
    /// <para>Draw borders for each unconnected edge of a tile.</para>
    /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Automate/Framework/OverlayMenu.cs#L14"/>
    /// </summary>
    /// <param name="spriteBatch">The sprite batch being drawn.</param>
    /// <param name="group">The machine group.</param>
    /// <param name="tile">The group tile.</param>
    /// <param name="color">The border color.</param>
    /// <param name="centerTile">Optional, indicate center tile. When drawing a range of tiles, borders be drawn 'away' from the center.</param>
    protected void DrawEdgeBorders(SpriteBatch spriteBatch, Vector2 tile, Color color)
    {
        int borderSize = 5;

        Vector2 screenCoord = Tiles.GetTileScreenCoords(tile);

        // top    
        spriteBatch.DrawLine(screenCoord.X, screenCoord.Y, new Vector2(tileSize, borderSize), color); // top

        // bottom    
        spriteBatch.DrawLine(screenCoord.X, screenCoord.Y + tileSize, new Vector2(tileSize, borderSize), color); // bottom

        // left    
        spriteBatch.DrawLine(screenCoord.X, screenCoord.Y, new Vector2(borderSize, tileSize), color); // left

        // right    
        spriteBatch.DrawLine(screenCoord.X + tileSize, screenCoord.Y, new Vector2(borderSize, tileSize), color); // right
    }

    /// <summary>
    /// <para>Draw borders for each unconnected edge of a tile.</para>
    /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Automate/Framework/OverlayMenu.cs#L14"/>
    /// </summary>
    /// <param name="spriteBatch">The sprite batch being drawn.</param>
    /// <param name="group">The machine group.</param>
    /// <param name="tile">The group tile.</param>
    /// <param name="color">The border color.</param>
    /// <param name="centerTile">Optional, indicate center tile. When drawing a range of tiles, borders be drawn 'away' from the center.</param>
    protected void DrawEdgeBorders(SpriteBatch spriteBatch, Vector2 tile, Color color, Vector2 centerTile)
    {
        int borderSize = 5;

        Vector2 screenCoord = Tiles.GetTileScreenCoords(tile);

        // top
        if (centerTile.Y >= tile.Y)
            spriteBatch.DrawLine(screenCoord.X, screenCoord.Y, new Vector2(tileSize, borderSize), color); // top

        // bottom
        if (centerTile.Y <= tile.Y)
            spriteBatch.DrawLine(screenCoord.X, screenCoord.Y + tileSize, new Vector2(tileSize, borderSize), color); // bottom

        // left
        if (centerTile.X >= tile.X)
            spriteBatch.DrawLine(screenCoord.X, screenCoord.Y, new Vector2(borderSize, tileSize), color); // left

        // right
        if (centerTile.X <= tile.X)
            spriteBatch.DrawLine(screenCoord.X + tileSize, screenCoord.Y, new Vector2(borderSize, tileSize), color); // right
    }

    /// <summary>
    /// <para>Draw borders for each unconnected edge of a tile.</para>
    /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Automate/Framework/OverlayMenu.cs#L14"/>
    /// </summary>
    /// <param name="spriteBatch">The sprite batch being drawn.</param>
    /// <param name="group">The machine group.</param>
    /// <param name="tile">The group tile.</param>
    /// <param name="color">The border color.</param>
    /// <param name="tileGroup">Optional: indicate the group this tile is part of. Borders will then be drawn around entire group.</param>
    protected void DrawEdgeBorders(SpriteBatch spriteBatch, Vector2 tile, Color color, List<Vector2> tileGroup)
    {
        int borderSize = 3;
        Vector2 screenCoord = Tiles.GetTileScreenCoords(tile);

        if (tileGroup is null)
            DrawEdgeBorders(spriteBatch, tile, color);

        // top
        if (!tileGroup.Contains(new Vector2(tile.X, tile.Y - 1)))
            spriteBatch.DrawLine(screenCoord.X, screenCoord.Y, new Vector2(tileSize, borderSize), color); // top

        // bottom
        if (!tileGroup.Contains(new Vector2(tile.X, tile.Y + 1)))
            spriteBatch.DrawLine(screenCoord.X, screenCoord.Y + tileSize, new Vector2(tileSize, borderSize), color); // bottom

        // left
        if (!tileGroup.Contains(new Vector2(tile.X - 1, tile.Y)))
            spriteBatch.DrawLine(screenCoord.X, screenCoord.Y, new Vector2(borderSize, tileSize), color); // left

        // right
        if (!tileGroup.Contains(new Vector2(tile.X + 1, tile.Y)))
            spriteBatch.DrawLine(screenCoord.X + tileSize, screenCoord.Y, new Vector2(borderSize, tileSize), color); // right
    }


    protected void DrawEdgeBordersToOutside(SpriteBatch spriteBatch, Vector2 tile, Color color, List<Vector2> tileGroup, Vector2 center)
    {
        int borderSize = 3;
        Vector2 screenCoord = Tiles.GetTileScreenCoords(tile);

        // top
        if (tile.Y <= center.Y && !tileGroup.Contains(new Vector2(tile.X, tile.Y - 1)))
            spriteBatch.DrawLine(screenCoord.X, screenCoord.Y, new Vector2(tileSize, borderSize), color); // top

        // bottom
        if (tile.Y >= center.Y && !tileGroup.Contains(new Vector2(tile.X, tile.Y + 1)))
            spriteBatch.DrawLine(screenCoord.X, screenCoord.Y + tileSize, new Vector2(tileSize, borderSize), color); // bottom

        // left
        if (tile.X <= center.X && !tileGroup.Contains(new Vector2(tile.X - 1, tile.Y)))
            spriteBatch.DrawLine(screenCoord.X, screenCoord.Y, new Vector2(borderSize, tileSize), color); // left

        // right
        if (tile.X >= center.X && !tileGroup.Contains(new Vector2(tile.X + 1, tile.Y)))
            spriteBatch.DrawLine(screenCoord.X + tileSize, screenCoord.Y, new Vector2(borderSize, tileSize), color); // right
    }

    public static void DrawPoint(SpriteBatch spriteBatch, Point point, Color? color = null, Texture2D tileTexture = null, float? tileSizePercentage = null)
    {
        DrawTile(spriteBatch, point.ToVector2(), color: color, tileTexture: tileTexture, tileSizePercentage: tileSizePercentage);
    }

    public static void DrawTile(SpriteBatch spriteBatch, Vector2 tile, Color? color = null, Texture2D tileTexture = null, float? tileSizePercentage = null, Vector2? offset = null, float? layerDepth = null)
    {
        int tilesize_offset = (tileSizePercentage is null) ? 0 : (int)((1 - tileSizePercentage.Value) * Game1.tileSize);

        tile = new Vector2(tile.X + tilesize_offset, tile.Y + tilesize_offset);

        Vector2 coords = Tiles.GetTileScreenCoords(tile) + (offset ?? Vector2.Zero);
        Vector2 size = Vector2.One * Game1.tileSize
            * ((tileSizePercentage is null) ? 1 : tileSizePercentage.Value);

        spriteBatch.DrawTexture(tileTexture ?? TilePlacementTexture, coords, size, color ?? Color.White, layerDepth: layerDepth);
    }

    public static void DrawTiles(SpriteBatch spriteBatch, IEnumerable<Vector2> tiles, Color? color = null, Texture2D tileTexture = null, float? tileSizePercentage = null)
    {
        foreach (Vector2 tile in tiles)
            DrawTile(spriteBatch, tile, color: color, tileTexture: tileTexture, tileSizePercentage: tileSizePercentage);
    }

    internal static void LoadPlacementTileTexture()
    {
        // Full asset is five 64x64 pixel tiles in a row, we only want the leftmost one of these tiles
        Texture2D fullAsset = ModEntry.Instance.Helper.GameContent.Load<Texture2D>("LooseSprites/buildingPlacementTiles");

        // Get color data of entire asset
        Color[] fullAssetColors = new Color[fullAsset.Width * fullAsset.Height];
        fullAsset.GetData<Color>(fullAssetColors);

        // Copy only leftmost tile to smaller array
        Color[] greenSliceAssetColors = new Color[64 * fullAsset.Height];
        Color[] redSliceAssetColors = new Color[64 * fullAsset.Height];
        Color[] grayScaleAssetColors = new Color[64 * fullAsset.Height];

        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                greenSliceAssetColors[x + y * 64] = fullAssetColors[x + y * fullAsset.Width];
                redSliceAssetColors[x + y * 64] = fullAssetColors[(64 + x) + y * fullAsset.Width];
                grayScaleAssetColors[x + y * 64] = fullAssetColors[x + y * fullAsset.Width].ToGrayScale();
            }
        }

        GreenTilePlacementTexture = new Texture2D(Game1.graphics.GraphicsDevice, 64, 64);
        GreenTilePlacementTexture.SetData<Color>(greenSliceAssetColors);

        TilePlacementTexture = new Texture2D(Game1.graphics.GraphicsDevice, 64, 64);
        TilePlacementTexture.SetData<Color>(grayScaleAssetColors);
    }
}
