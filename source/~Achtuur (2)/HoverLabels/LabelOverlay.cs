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
using AchtuurCore.Framework;
using AchtuurCore.Utility;
using HoverLabels.Framework;
using HoverLabels.Labels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels;
internal class LabelOverlay : Overlay
{
    internal readonly Vector2 baseOffset = new Vector2(75f, 75f);

    Dictionary<string, Texture2D> MenuTextures = new Dictionary<string, Texture2D>();

    SpriteFont nameFont;
    SpriteFont descFont;

    public LabelOverlay() : base()
    {
        this.LoadMenuTextureAssets();
    }

    public override void Enable()
    {
        base.Enable();
        this.nameFont = Game1.dialogueFont;
        this.descFont = Game1.smallFont;
    }

    public override void Disable()
    {
        base.Disable();
    }

    protected override void DrawOverlayToScreen(SpriteBatch spriteBatch)
    {
        if (!ModEntry.Instance.LabelManager.HasLabel())
            return;

        // Draw additional stuff from label first, so other stuff is drawn over it
        ModEntry.Instance.LabelManager.CurrentLabel.DrawOnOverlay(spriteBatch);

        // Get coordinates of cursor on screen
        Vector2 cursorPos = ModEntry.Instance.Helper.Input.GetCursorPosition().AbsolutePixels;
        Vector2 offset = GetOffset(cursorPos);
        Vector2 cursorCoords = Drawing.GetPositionScreenCoords(cursorPos) + offset;

        // Draw box that contains label text
        DrawLabelBox(spriteBatch, cursorCoords);

        // Draw label text
        string displayName = ModEntry.Instance.LabelManager.CurrentLabel.GetName();
        string displayDesc = ModEntry.Instance.LabelManager.GetDescriptionAsString();
        Vector2 nameSize = ModEntry.Instance.LabelManager.GetNameSize(this.nameFont);
        Vector2 descOffset = new(0f, nameSize.Y + 32f); // the +32f is there so the description aligns with the box

        spriteBatch.DrawString(this.nameFont, displayName, cursorCoords, Color.Black);
        spriteBatch.DrawString(this.descFont, displayDesc, cursorCoords + descOffset, Color.Black);
    }

    /// <summary>
    /// Generate offset for label box relative to cursor
    /// </summary>
    /// <returns></returns>
    private Vector2 GetOffset(Vector2 cursorPos)
    {
        // Label defaults to bottom right of cursor
        Vector2 offset = baseOffset;

        Rectangle visibleRect = Drawing.GetVisibleArea();
        Vector2 visibleCoords = new(visibleRect.Width + visibleRect.X, visibleRect.Height + visibleRect.Y);

        Vector2 labelSize = ModEntry.Instance.LabelManager.GetLabelSize(this.nameFont, this.descFont);

        // Overflow on right side -> put label on left
        if (cursorPos.X + labelSize.X + baseOffset.X >= visibleCoords.X - 2f*Game1.tileSize)
            offset.X -= baseOffset.X * 1.2f + labelSize.X;

        // Overflow on bottom side -> put label on top
        if (cursorPos.Y + labelSize.Y + baseOffset.Y >= visibleCoords.Y - 3f*Game1.tileSize)
            offset.Y -= baseOffset.Y * 1.2f + labelSize.Y;

        // If putting label on top causes overflow on top size, go back to default offset
        // Overflow on bottom part of screen is preferred
        if (cursorPos.Y + offset.Y <= visibleRect.Y)
            offset.Y = baseOffset.Y;

        return offset;
    }

    private void DrawLabelBox(SpriteBatch spriteBatch, Vector2 cursorCoords)
    {
        int tileSize = (int) 64;
        float offsetFactor = 0.7f;
        Vector2 offset = new Vector2(64f, 64f) * offsetFactor;

        // 'unit' scale vector in x direction
        Vector2 vx = Vector2.UnitX * tileSize;
        // 'unit' scale vector in y direction
        Vector2 vy = Vector2.UnitY * tileSize;

        cursorCoords -= offset;

        Vector2 nameSize = ModEntry.Instance.LabelManager.GetNameSize(this.nameFont);
        Vector2 descSize = ModEntry.Instance.LabelManager.GetDescriptionSize(this.descFont);

        // Horizontal/Vertical scale for the top/left/right/bottom border part,
        // is equal to label string width - borders,
        // divided by 64 because the texture is 64 pixels wide
        Vector2 nameBarScale = (nameSize - 2f * Vector2.One * tileSize + 2f * offset) / tileSize;
        Vector2 descBarScale = (descSize - 2f * Vector2.One * tileSize + 2f * offset) / tileSize;

        // Bar scale should be biggest of namebar and descbar
        float xBarScale = Math.Max(nameBarScale.X, descBarScale.X);

        // helper vectors to calculate position of bars
        Vector2 middleX = vx;
        Vector2 rightX = middleX + vx * xBarScale;
        Vector2 midNameY = vy;
        Vector2 botNameY = midNameY + vy * nameBarScale.Y;
        Vector2 botBarY = botNameY;

        //bg scale is half both borders (they start halfway the tile) = 1x tile size + barscale
        DrawMenuTexture(spriteBatch, MenuTextures["Background"], cursorCoords + offset * offsetFactor, new Vector2(xBarScale, nameBarScale.Y) + Vector2.One);

        // Top part
        DrawMenuTexture(spriteBatch, MenuTextures["TopLeftBorder"], cursorCoords);
        DrawMenuTexture(spriteBatch, MenuTextures["TopBorder"], cursorCoords + middleX, new Vector2(xBarScale, 1f));
        DrawMenuTexture(spriteBatch, MenuTextures["TopRightBorder"], cursorCoords + rightX);
        
        DrawMenuTexture(spriteBatch, MenuTextures["LeftBorder"], cursorCoords + midNameY, new Vector2(1f, nameBarScale.Y));
        DrawMenuTexture(spriteBatch, MenuTextures["RightBorder"], cursorCoords + midNameY + rightX, new Vector2(1f, nameBarScale.Y));


        // Draw description border
        if (ModEntry.Instance.LabelManager.LabelHasDescription())
        {
            Vector2 midDescY = botNameY + vy;
            Vector2 botDescY = midDescY + vy * descBarScale.Y;

            DrawMenuTexture(spriteBatch, MenuTextures["Background"], cursorCoords + offset * offsetFactor + botNameY, new Vector2(xBarScale, descBarScale.Y) + Vector2.One);

            DrawMenuTexture(spriteBatch, MenuTextures["TSectionEast"], cursorCoords + botNameY);
            DrawMenuTexture(spriteBatch, MenuTextures["InnerHorizontal"], cursorCoords + botNameY + middleX, new Vector2(xBarScale, 1f));
            DrawMenuTexture(spriteBatch, MenuTextures["TSectionWest"], cursorCoords + botNameY + rightX);

            DrawMenuTexture(spriteBatch, MenuTextures["LeftBorder"], cursorCoords + midDescY, new Vector2(1f, descBarScale.Y));
            DrawMenuTexture(spriteBatch, MenuTextures["RightBorder"], cursorCoords + midDescY + rightX, new Vector2(1f, descBarScale.Y));

            // Update bottom to be under left/right border of description, instead of under name
            botBarY = botDescY;
        }


        // draw bottom part
        DrawMenuTexture(spriteBatch, MenuTextures["BottomLeftBorder"], cursorCoords + botBarY);
        DrawMenuTexture(spriteBatch, MenuTextures["BottomBorder"], cursorCoords + middleX + botBarY, new Vector2(xBarScale, 1f));
        DrawMenuTexture(spriteBatch, MenuTextures["BottomRightBorder"], cursorCoords + rightX + botBarY);
    }

    private void DrawMenuTexture(SpriteBatch spriteBatch, Texture2D texture, Vector2 pos, Vector2? scale=null, float? priority=null)
    {
        spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.Zero, scale ?? Vector2.One, SpriteEffects.None, priority ?? 0f);
    }

    internal void LoadMenuTextureAssets()
    {
        Texture2D menuTextures = ModEntry.Instance.Helper.GameContent.Load<Texture2D>("Maps/MenuTiles");

        Color[] fullAssetColors = new Color[menuTextures.Width * menuTextures.Height];
        menuTextures.GetData<Color>(fullAssetColors);

        AssetColors[] assetColors = new[]
        {
            new AssetColors(0, 0, "TopLeftBorder"),
            new AssetColors(0, 1, "TSectionEast"),
            new AssetColors(0, 2, "LeftBorder"),
            new AssetColors(0, 3, "BottomLeftBorder"),
            new AssetColors(1, 0, "TSectionSouth"),
            new AssetColors(1, 1, "InnerVertical"),
            new AssetColors(1, 2, "Background"),
            new AssetColors(1, 3, "TSectionNorth"),
            new AssetColors(2, 0, "TopBorder"),
            new AssetColors(2, 1, "InnerHorizontal"),
            new AssetColors(2, 3, "BottomBorder"),
            new AssetColors(3, 0, "TopRightBorder"),
            new AssetColors(3, 1, "TSectionWest"),
            new AssetColors(3, 2, "RightBorder"),
            new AssetColors(3, 3, "BottomRightBorder"),
        };  


        for (int i = 0; i < assetColors.Length; i++)
        {
            // Copy colors
            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    int idx = x + 64 * y;
                    int sourceIdx = assetColors[i].sheetBaseIndex + x + menuTextures.Width * y;
                    assetColors[i].Colors[idx] = fullAssetColors[sourceIdx];
                }
            }
            this.MenuTextures.Add(assetColors[i].name, new Texture2D(Game1.graphics.GraphicsDevice, 64, 64));
            this.MenuTextures[assetColors[i].name].SetData<Color>(assetColors[i].Colors);
        }
    }

    private record struct AssetColors(int tileX, int tileY, string name)
    {
        public Color[] Colors { get; set; } = new Color[64 * 64];

        public int sheetBaseIndex => tileX * 64 + tileY * 64 * 256;
    }
}
