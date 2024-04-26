/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace EscasModdingPlugins
{
    /// <summary>EMP's SMAPI API interface. Allows direct interaction with other SMAPI mods, e.g. access to EMP feature data.</summary>
    /// <remarks>Copy this interface into your mod and reference it to get EMP's API from SMAPI: Helper.ModRegistry.GetApi&lt;IEmpApi&gt;("Esca.EMP");</remarks>
    public interface IEmpApi
    {
        /// <summary>Gets overridden fish location settings for the provided location and tile, if any.</summary>
        /// <param name="location">The in-game location to check.</param>
        /// <param name="tile">The tile to check, e.g. the position of a player's fishing bobber.</param>
        /// <param name="useLocationName">Outputs the name of the location to use instead of the provided location (e.g. the key to use in the Data/Locations asset). Null if the original location will be used.</param>
        /// <param name="useTile">Outputs the tile to use instead of the provided tile. Null if the original tile will be used.</param>
        /// <param name="useCrabPotTypes">Outputs a list of fish types that can be caught by crab pots on this tile, e.g. ["Ocean", "Freshwater"]. Null if the original types will be used.</param>
        void GetFishLocationsData(GameLocation location, Vector2 tile, out string useLocationName, out Vector2? useTile, out List<string> useCrabPotTypes);

        /* Obsolete features below; avoid using these */

        /// <summary>Gets overridden fish location settings for the provided location and tile, if any.</summary>
        /// <param name="location">The in-game location to check.</param>
        /// <param name="tile">The tile to check, e.g. the position of a player's fishing bobber.</param>
        /// <param name="useLocationName">The name of the location to use instead of the provided location (e.g. the key to use in the Data/Locations asset). Null if the original location will be used.</param>
        /// <param name="useZone">The fishing zone to use. Null if the original zone will be used. "Zone" refers to the result of "GameLocation.getFishingLocation(Vector2)".</param>
        /// <param name="useOceanCrabPots">True if this tile should use ocean results for crab pots; false if it should use freshwater results. Null if the original results will be used.</param>
        [Obsolete("In EMP v1.2.3 and later, some of these parameters are disabled or implemented differently. Please use another overload of this method.", false)]
        void GetFishLocationsData(GameLocation location, Vector2 tile, out string useLocationName, out int? useZone, out bool? useOceanCrabPots);
    }
}
