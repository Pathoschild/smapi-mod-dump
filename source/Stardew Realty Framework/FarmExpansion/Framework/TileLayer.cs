/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/FarmExpansion
**
*************************************************/

namespace FarmExpansion.Framework
{
    /// <summary>A tile layer in tile sheets.</summary>
    internal enum TileLayer
    {
        /// <summary>Typically contains terrain, water, and basic features (like permanent paths).</summary>
        Back,

        /// <summary>Typically contains placeholders for buildings (like the farmhouse).</summary>
        Buildings,

        /// <summary>Typically contains flooring, paths, grass, and debris (like stones, weeds, and stumps) which can be removed by the player.</summary>
        Paths,

        /// <summary>Typically contains objects that are drawn on top of things behind them, like most trees.</summary>
        Front,

        /// <summary>Typically contains objects that are always drawn on top of other layers. This is typically used for foreground effects like foliage cover.</summary>
        AlwaysFront
    }
}
