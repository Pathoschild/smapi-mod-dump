/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

using System;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.DataLayers.Framework;
using StardewValley;

namespace Pathoschild.Stardew.DataLayers.Layers
{
    /// <summary>A data layer which just shows the tile grid.</summary>
    internal class GridLayer : BaseLayer
    {
        /*********
        ** Fields
        *********/
        /// <summary>A cached empty tile group list.</summary>
        private readonly TileGroup[] NoGroups = Array.Empty<TileGroup>();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="config">The data layer settings.</param>
        public GridLayer(LayerConfig config)
            : base(I18n.Grid_Name(), config)
        {
            this.Legend = Array.Empty<LegendEntry>();
            this.AlwaysShowGrid = true;
        }

        /// <summary>Get the updated data layer tiles.</summary>
        /// <param name="location">The current location.</param>
        /// <param name="visibleArea">The tile area currently visible on the screen.</param>
        /// <param name="visibleTiles">The tile positions currently visible on the screen.</param>
        /// <param name="cursorTile">The tile position under the cursor.</param>
        public override TileGroup[] Update(GameLocation location, in Rectangle visibleArea, in Vector2[] visibleTiles, in Vector2 cursorTile)
        {
            return this.NoGroups;
        }
    }
}
