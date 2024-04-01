/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.Automate;

using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

/// <summary>Constructs machines, containers, or connectors which can be added to a machine group.</summary>
public interface IAutomationFactory
{
    /*********
     ** Accessors
     *********/
    /// <summary>Get a machine, container, or connector instance for a given object.</summary>
    /// <param name="obj">The in-game object.</param>
    /// <param name="location">The location to check.</param>
    /// <param name="tile">The tile position to check.</param>
    /// <returns>Returns an instance or <c>null</c>.</returns>
    IAutomatable? GetFor(SObject obj, GameLocation location, in Vector2 tile);

    /// <summary>Get a machine, container, or connector instance for a given terrain feature.</summary>
    /// <param name="feature">The terrain feature.</param>
    /// <param name="location">The location to check.</param>
    /// <param name="tile">The tile position to check.</param>
    /// <returns>Returns an instance or <c>null</c>.</returns>
    IAutomatable? GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile);

    /// <summary>Get a machine, container, or connector instance for a given building.</summary>
    /// <param name="building">The building.</param>
    /// <param name="location">The location to check.</param>
    /// <param name="tile">The tile position to check.</param>
    /// <returns>Returns an instance or <c>null</c>.</returns>
    IAutomatable? GetFor(Building building, GameLocation location, in Vector2 tile);

    /// <summary>Get a machine, container, or connector instance for a given tile position.</summary>
    /// <param name="location">The location to check.</param>
    /// <param name="tile">The tile position to check.</param>
    /// <returns>Returns an instance or <c>null</c>.</returns>
    IAutomatable? GetForTile(GameLocation location, in Vector2 tile);
}