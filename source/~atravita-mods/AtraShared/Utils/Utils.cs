/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;

namespace AtraShared.Utils;

/// <summary>
/// Utility methods.
/// </summary>
public static class Utils
{
    /// <summary>
    /// A Lazy that contains a single pixel, useful for drawing geometric shapes.
    /// </summary>
    /// <remarks>Taken from https://github.com/Pathoschild/StardewMods/blob/develop/Common/CommonHelper.cs . Much thanks.</remarks>
    private static readonly Lazy<Texture2D> LazyPixel = new(() =>
    {
        Texture2D pixel = new(Game1.graphics.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        return pixel;
    });

    /// <summary>
    /// Gets a Pixel that can be used for drawing arbitrary geometric shapes.
    /// </summary>
    public static Texture2D Pixel => LazyPixel.Value;

    /// <summary>
    /// Gets the configuration instance, or returns a default one.
    /// </summary>
    /// <typeparam name="T">Type of config.</typeparam>
    /// <param name="helper">Smapi's helper.</param>
    /// <param name="monitor">Logger.</param>
    /// <returns>Config.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T GetConfigOrDefault<T>(IModHelper helper, IMonitor monitor)
        where T : class, new()
    {
        try
        {
            return helper.ReadConfig<T>();
        }
        catch
        {
            monitor.Log(
                helper.Translation.Get("IllFormatedConfig")
                    .Default("Config file seems ill-formated, using default. Please use Generic Mod Config Menu to configure."),
                LogLevel.Warn);
            return new();
        }
    }

    /// <summary>
    /// Yields all tiles around a specific tile.
    /// </summary>
    /// <param name="tile">Vector2 location of tile.</param>
    /// <param name="radius">A radius to search in.</param>
    /// <returns>All tiles within radius.</returns>
    /// <remarks>This actually returns a square, not a circle.</remarks>
    public static IEnumerable<Point> YieldSurroundingTiles(Vector2 tile, int radius = 1)
    {
        int x = (int)tile.X;
        int y = (int)tile.Y;
        for (int xdiff = -radius; xdiff <= radius; xdiff++)
        {
            for (int ydiff = -radius; ydiff <= radius; ydiff++)
            {
                yield return new Point(x + xdiff, y + ydiff);
            }
        }
    }

    /// <summary>
    /// Yields an iterator over all tiles on a location.
    /// </summary>
    /// <param name="location">Location to check.</param>
    /// <returns>IEnumerable of all tiles.</returns>
    public static IEnumerable<Vector2> YieldAllTiles(GameLocation location)
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
    public static List<string> ContextSort(IEnumerable<string> enumerable)
    {
        List<string> outputlist = enumerable.ToList();
        outputlist.Sort(GetCurrentLanguageComparer(ignoreCase: true));
        return outputlist;
    }

    /// <summary>
    /// Returns a StringComparer for the current language the player is using.
    /// </summary>
    /// <param name="ignoreCase">Whether or not to ignore case.</param>
    /// <returns>A string comparer.</returns>
    public static StringComparer GetCurrentLanguageComparer(bool ignoreCase = false)
        => StringComparer.Create(Game1.content.CurrentCulture, ignoreCase);

    /// <summary>
    /// Gets all birthday NPCs.
    /// </summary>
    /// <param name="day">Current date.</param>
    /// <returns>IEnumerable of birthday npcs.</returns>
    public static IEnumerable<NPC> GetBirthdayNPCs(SDate day)
    {
        foreach (NPC npc in Utility.getAllCharacters())
        {
            if (npc.isBirthday(day.Season, day.Day))
            {
                yield return npc;
            }
        }
    }

#warning - fix in Stardew 1.6
    /// <summary>
    /// Gets the next day, given the specific season and day.
    /// </summary>
    /// <param name="season">Season as string.</param>
    /// <param name="day">Day as int.</param>
    /// <returns>ValueTuple containing the next day.</returns>
    public static (string season, int day) GetTomorrow(string season, int day)
    {
        if (day == 28)
        {
            return season switch
            {
                "spring" => ("summer", 1),
                "summer" => ("fall", 1),
                "fall" => ("winter", 1),
                "winter" => ("spring", 1),
                _ => ThrowHelper.ThrowArgumentException<ValueTuple<string, int>>($"Unexpected season {season}!"),
            };
        }
        else
        {
            return (season, day + 1);
        }
    }
}