/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;
using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

using xTile.Tiles;

using XLocation = xTile.Dimensions.Location;
using XRectangle = xTile.Dimensions.Rectangle;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extensions on GameLocation.
/// </summary>
public static class GameLocationExtensions
{
    /// <summary>
    /// Should this location be considered dangerous?
    /// Always safe: Farm, town, IslandWest.
    /// Always dangerous: Volcano, MineShaft.
    /// In-between: everywhere else.
    /// </summary>
    /// <param name="location">Location to check.</param>
    /// <returns>Whether the location should be considered dangerous.</returns>
    public static bool IsDangerousLocation(this GameLocation location)
        => !location.IsFarm && !location.IsGreenhouse && location is not (SlimeHutch or Town or IslandWest)
            && (location is MineShaft or VolcanoDungeon or BugLand || location.characters.Any((character) => character is Monster));

    /// <summary>
    /// Returns true if there's a festival at a location and the player can't actually warp there yet.
    /// </summary>
    /// <param name="location">Location to check.</param>
    /// <param name="monitor">Logger.</param>
    /// <param name="alertPlayer">Whether or not to show a notification.</param>
    /// <returns>True if there's a festival at this location and it's before the start time, false otherwise.</returns>
    public static bool IsBeforeFestivalAtLocation(this GameLocation location, IMonitor monitor, bool alertPlayer = false)
    {
        Guard.IsNotNull(monitor);
        Guard.IsNotNull(location);

        try
        {
            if (Game1.weatherIcon == 1)
            {
                Dictionary<string, string>? festivalData;
                try
                {
                    festivalData = Game1.temporaryContent.Load<Dictionary<string, string>>($@"Data\Festivals\{Game1.currentSeason}{Game1.dayOfMonth}");
                }
                catch (Exception ex)
                {
                    monitor.Log($"No festival file found for today....did someone screw with the time?\n\n{ex}", LogLevel.Warn);
                    return false;
                }
                if (festivalData.TryGetValue("conditions", out string? val))
                {
                    string[]? splits = val.Split('/');
                    if (splits.Length >= 2)
                    {
                        if (!location.Name.Equals(splits[0], StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                        if (int.TryParse(splits[1].GetNthChunk(' ', 0), out int startTime) && Game1.timeOfDay < startTime)
                        {
                            if (alertPlayer)
                            {
                                Game1.drawObjectDialogue(Game1.content.LoadString(@"Strings\StringsFromCSFiles:Game1.cs.2973"));
                            }
                            return true;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            monitor.Log($"Mod failed while trying to find festival days....\n\n{ex}", LogLevel.Error);
        }
        return false;
    }

    /// <summary>
    /// Gets the hoedirt at a specific tile, in a pot or on the ground.
    /// </summary>
    /// <param name="location">Location.</param>
    /// <param name="tile">Tile.</param>
    /// <returns>Hoedirt if found.</returns>
    public static HoeDirt? GetHoeDirtAtTile(this GameLocation location, Vector2 tile)
    {
        Guard.IsNotNull(location);

        if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature? terrain)
            && terrain is HoeDirt dirt)
        {
            return dirt;
        }
        else if (location.Objects.TryGetValue(tile, out SObject obj)
            && obj is IndoorPot pot)
        {
            return pot.hoeDirt.Value;
        }
        return null;
    }

    /// <summary>
    /// Whether or not a tile is covered by a Front or AlwaysFront tile at this location.
    /// </summary>
    /// <param name="loc">GameLocation.</param>
    /// <param name="tileLocation">Tile.</param>
    /// <param name="viewport">Viewport.</param>
    /// <returns>True if covered, false otherwise.</returns>
    public static bool IsTileViewable(this GameLocation? loc, XLocation tileLocation, XRectangle viewport)
    {
        if (loc is null)
        {
            return false;
        }

        return (loc.map.GetLayer("Front")?.PickTile(new XLocation(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size)
            ?? loc.map.GetLayer("AlwaysFront")?.PickTile(new XLocation(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size)) is null;
    }
}