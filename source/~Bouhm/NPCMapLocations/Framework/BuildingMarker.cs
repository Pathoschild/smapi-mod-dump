/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace NPCMapLocations.Framework
{
    /// <summary>The basic building info for a map marker.</summary>
    /// <param name="CommonName">The base name for the interior location without the unique suffix.</param>
    /// <param name="MapPosition">The marker's pixel position on the map.</param>
    internal record BuildingMarker(string CommonName, Vector2 MapPosition);
}
