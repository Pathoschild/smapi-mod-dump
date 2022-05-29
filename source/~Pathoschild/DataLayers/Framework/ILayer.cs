/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace Pathoschild.Stardew.DataLayers.Framework
{
    /// <summary>Provides metadata to display in the overlay.</summary>
    internal interface ILayer
    {
        /*********
        ** Accessors
        *********/
        /// <summary>A unique identifier for the layer.</summary>
        string Id { get; }

        /// <summary>The layer display name.</summary>
        string Name { get; }

        /// <summary>The number of ticks between each update.</summary>
        int UpdateTickRate { get; }

        /// <summary>Whether to update the layer when the set of visible tiles changes.</summary>
        bool UpdateWhenVisibleTilesChange { get; }

        /// <summary>The legend entries to display.</summary>
        LegendEntry[] Legend { get; }

        /// <summary>The keys which activate the layer.</summary>
        KeybindList ShortcutKey { get; }

        /// <summary>Whether to always show the tile grid.</summary>
        bool AlwaysShowGrid { get; }


        /*********
        ** Methods
        *********/
        /// <summary>Get the updated data layer tiles.</summary>
        /// <param name="location">The current location.</param>
        /// <param name="visibleArea">The tile area currently visible on the screen.</param>
        /// <param name="visibleTiles">The tile positions currently visible on the screen.</param>
        /// <param name="cursorTile">The tile position under the cursor.</param>
        TileGroup[] Update(GameLocation location, in Rectangle visibleArea, in Vector2[] visibleTiles, in Vector2 cursorTile);
    }
}
