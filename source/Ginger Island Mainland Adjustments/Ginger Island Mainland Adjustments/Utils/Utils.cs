/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using System.Globalization;
using Microsoft.Xna.Framework;

namespace GingerIslandMainlandAdjustments.Utils;

/// <summary>
/// Utility methods.
/// </summary>
internal static class Utils
{
    /// <summary>
    /// Yields all tiles around a specific tile.
    /// </summary>
    /// <param name="tile">Vector2 location of tile.</param>
    /// <param name="radius">A radius to search in.</param>
    /// <returns>All tiles within radius.</returns>
    /// <remarks>This actually returns a square, not a circle.</remarks>
    internal static IEnumerable<Point> YieldSurroundingTiles(Vector2 tile, int radius = 1)
    {
        int x = (int)tile.X;
        int y = (int)tile.Y;
        for (int xdiff = -radius; xdiff <= radius; xdiff++)
        {
            for (int ydiff = -radius; ydiff <= radius; ydiff++)
            {
                Globals.ModMonitor.Log($"{x + xdiff} {y + ydiff}");
                yield return new Point(x + xdiff, y + ydiff);
            }
        }
    }

    /// <summary>
    /// Yields an iterator over all tiles on a location.
    /// </summary>
    /// <param name="location">Location to check.</param>
    /// <returns>IEnumerable of all tiles.</returns>
    internal static IEnumerable<Vector2> YieldAllTiles(GameLocation location)
    {
        for (int x = 0; x < location.Map.Layers[0].LayerWidth; x++)
        {
            for (int y = 0; y < location.Map.Layers[0].LayerHeight; y++)
            {
                yield return new Vector2(x, y);
            }
        }
    }

    /// <summary>
    /// Sort strings, taking into account CultureInfo of currently selected language.
    /// </summary>
    /// <param name="enumerable">IEnumerable of strings to sort.</param>
    /// <returns>A sorted list of strings.</returns>
    [Pure]
    internal static List<string> ContextSort(IEnumerable<string> enumerable)
    {
        LocalizedContentManager contextManager = Game1.content;
        string langcode = contextManager.LanguageCodeString(contextManager.GetCurrentLanguage());
        List<string> outputlist = enumerable.ToList();
        outputlist.Sort(StringComparer.Create(new CultureInfo(langcode), true));
        return outputlist;
    }
}