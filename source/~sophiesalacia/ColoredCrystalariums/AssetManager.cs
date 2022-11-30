/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace ColoredCrystalariums;

public class AssetManager
{
    public static Texture2D OverlayTexture { get; internal set; }
    public static Dictionary<int, string> ColorOverrides { get; internal set; }
    public static Dictionary<int, Color> CachedAverageColors { get; internal set; }

    internal static Color GetColorFor(Object item)
    {
        int id = item.ParentSheetIndex;

        // if not in asset or malformed value, get average color
        if (!ColorOverrides.ContainsKey(id) || ColorOverrides[id].Length != 6)
        {
            return GetAverageColorFor(item);
        }

        string packedColorString = ColorOverrides[id];

        packedColorString += "ff";
        bool parsed = uint.TryParse(packedColorString, NumberStyles.HexNumber, null, out uint packedColorResult);
        // if successfully parsed, pass back as color - otherwise, get average color for sprite
        return parsed ? new Color(packedColorResult) : GetAverageColorFor(item);
    }

    private static Color GetAverageColorFor(Object item)
    {
        if (CachedAverageColors.ContainsKey(item.ParentSheetIndex))
            return CachedAverageColors[item.ParentSheetIndex];

        Texture2D itemTexture = new(Game1.graphics.GraphicsDevice, 16, 16);
        Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, item.ParentSheetIndex, 16, 16);
        Color[] data = new Color[sourceRect.Width * sourceRect.Height];
        Game1.objectSpriteSheet.GetData(0, sourceRect, data, 0, sourceRect.Width * sourceRect.Height);
        itemTexture.SetData(data);

        int averageR = 0;
        int averageG = 0;
        int averageB = 0;
        int numPixels = 1;

        // iteratively calculate average to avoid possible overflow
        foreach (Color color in data)
        {
            if (color.A == 0)
                continue;

            averageR += (color.R - averageR) / numPixels;
            averageG += (color.G - averageG) / numPixels;
            averageB += (color.B - averageB) / numPixels;
            numPixels++;
        }

        Color averageColor = numPixels > 1 ? new Color(averageR, averageG, averageB) : new Color(255, 255, 255);

        CachedAverageColors[item.ParentSheetIndex] = averageColor;
        return CachedAverageColors[item.ParentSheetIndex];
    }

    public static void InitializeAssets(object sender, SaveLoadedEventArgs e)
    {
        OverlayTexture = Game1.content.Load<Texture2D>(Globals.CrystalariumOverlayPath);
        ColorOverrides = Game1.content.Load<Dictionary<int, string>>(Globals.ColorOverridesPath);
        CachedAverageColors = new Dictionary<int, Color>();
    }

    public static void LoadAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Globals.CrystalariumOverlayPath))
        {
            e.LoadFromModFile<Texture2D>("Assets/overlay.png", AssetLoadPriority.Medium);
        }

        else if (e.NameWithoutLocale.IsEquivalentTo(Globals.ColorOverridesPath))
        {
            e.LoadFrom(() => new Dictionary<int, string>(), AssetLoadPriority.Medium);
        }
    }

    public static void UpdateAssets(object sender, AssetReadyEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Globals.CrystalariumOverlayPath))
        {
            OverlayTexture = Game1.content.Load<Texture2D>(Globals.CrystalariumOverlayPath);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(Globals.ColorOverridesPath))
        {
            ColorOverrides = Game1.content.Load<Dictionary<int, string>>(Globals.ColorOverridesPath);
        }
    }
}
