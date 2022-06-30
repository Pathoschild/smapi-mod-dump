/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using xTile;
using xTile.Dimensions;
using xTile.Layers;

namespace Pathoschild.Stardew.Common
{
    /// <summary>Provides extension methods for <see cref="Map"/>.</summary>
    internal static class MapExtensions
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get the map's size in tiles.</summary>
        /// <param name="map">The map instance.</param>
        public static Size GetSizeInTiles(this Map map)
        {
            int width = 1;
            int height = 1;

            foreach (Layer layer in map.Layers)
            {
                width = Math.Max(width, layer.LayerWidth);
                height = Math.Max(height, layer.LayerHeight);
            }

            return new Size(width, height);
        }
    }
}
