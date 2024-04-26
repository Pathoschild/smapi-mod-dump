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
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Drawing;


internal class BorderDrawer
{
    public static Dictionary<string, Texture2D> BorderTextures;
    protected List<Border> borders = new List<Border>();


    public BorderDrawer()
    {
        borders = new List<Border>();
        LoadBorderTextureAssets();
    }

    public BorderDrawer(IEnumerable<Border> borders)
    {
        this.borders = borders.ToList();
        LoadBorderTextureAssets();
    }

    public void AddBorder(LabelText label_text)
    {
        List<LabelText> list = new List<LabelText>() { label_text };
        this.AddBorder(list);
    }

    public void AddBorder(IEnumerable<LabelText> label_texts)
    {
        Border border = new(label_texts);
        borders.Add(border);
    }

    public void AddBorder(IEnumerable<Border> borders)
    {
        foreach(Border border in borders)
            AddBorder(border);
    }

    public void AddBorder(Border border)
    {
        borders.Add(border);
    }

    public void Reset()
    {
        borders.Clear();
    }

    public Vector2 BorderSize()
    {
        Vector2 max = Vector2.Zero;
        foreach (Border border in borders)
        {
            max.X = Math.Max(max.X, border.TotalWidth);
            max.Y = Math.Max(max.Y, border.TotalHeight);
        }
        return max;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 offset)
    {
        Vector2 total_offset = offset;
        float width = BorderSize().X;
        for (int i = 0; i < borders.Count; i++)
        {
            if (i == 0)
            {
                borders[i].TopBorder = BorderType.Top;
            }
            if (i == borders.Count - 1)
            {
                borders[i].BottomBorder = BorderType.Bottom;
            }
            borders[i].FixedWidth = width;
            borders[i].Draw(spriteBatch, total_offset);
            // border draws from top left, so shift offset to the bottom of the border
            // this will then be the top of the next border
            total_offset.Y += borders[i].BottomRight.Y;
        }
    }

    protected void LoadBorderTextureAssets()
    {
        if (BorderTextures != null)
            return;

        Texture2D menuTextures = ModEntry.Instance.Helper.GameContent.Load<Texture2D>("Maps/MenuTiles");

        Color[] fullAssetColors = new Color[menuTextures.Width * menuTextures.Height];
        menuTextures.GetData(fullAssetColors);

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

        BorderTextures = new();
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
            BorderTextures.Add(assetColors[i].name, new Texture2D(Game1.graphics.GraphicsDevice, 64, 64));
            BorderTextures[assetColors[i].name].SetData(assetColors[i].Colors);
        }
    }

    public static void DrawMenuTexture(SpriteBatch spriteBatch, Texture2D texture, Vector2 pos, Vector2? scale = null, float? priority = null)
    {
        spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.Zero, scale ?? Vector2.One, SpriteEffects.None, priority ?? 0f);
    }
}


/// <summary>
/// Record struct to use when loading assets from spritesheet
/// </summary>
/// <param name="tileX"></param>
/// <param name="tileY"></param>
/// <param name="name"></param>
public record struct AssetColors(int tileX, int tileY, string name)
{
    public Color[] Colors { get; set; } = new Color[64 * 64];

    public int sheetBaseIndex => tileX * 64 + tileY * 64 * 256;
}