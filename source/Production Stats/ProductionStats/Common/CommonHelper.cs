/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

// derived from code by Jesse Plamondon-Willard under MIT license: https://github.com/Pathoschild/StardewMods

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;

namespace ProductionStats.Common;

internal class CommonHelper
{
    /// <summary>
    ///     A blank pixel which can be colorized and stretched to 
    ///     draw geometric shapes.
    /// </summary>
    public static Texture2D Pixel => _lazyPixel.Value;

    /// <summary>
    ///     A blank pixel which can be colorized and stretched to 
    ///     draw geometric shapes.
    /// </summary>
    private static readonly Lazy<Texture2D> _lazyPixel = new(() =>
    {
        Texture2D pixel = new(Game1.graphics.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        return pixel;
    });

    /// <summary>Get the dimensions of a space character.</summary>
    /// <param name="font">The font to measure.</param>
    public static float GetSpaceWidth(SpriteFont font)
    {
        return font.MeasureString("A B").X - font.MeasureString("AB").X;
    }

    /// <summary>Get all game locations.</summary>
    /// <param name="includeTempLevels">Whether to include temporary mine/dungeon locations.</param>
    public static IEnumerable<GameLocation> GetLocations(bool includeTempLevels = false)
    {
        var locations = Game1.locations
            .Concat(
                from location in Game1.locations
                from indoors in location.GetInstancedBuildingInteriors()
                select indoors
            );

        if (includeTempLevels)
        {
            locations = locations.Concat(MineShaft.activeMines).Concat(VolcanoDungeon.activeLevels);
        }

        return locations;
    }
}