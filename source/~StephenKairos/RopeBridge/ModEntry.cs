/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StephenKairos/Teban100-StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using Teban100.Common;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;
using SObject = StardewValley.Object;

namespace RopeBridge
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            CommonHelper.RemoveObsoleteFiles(this, "AutoGate.pdb");

            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.World.NpcListChanged += this.OnNpcListChanged;
            helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <inheritdoc cref="IPlayerEvents.Warped"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                this.FixLadders();
        }

        /// <inheritdoc cref="IPlayerEvents.InventoryChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            bool usedStaircase =
                e.IsLocalPlayer
                && Game1.currentLocation is MineShaft
                && (
                    e.Removed.Any(this.IsStaircase)
                    || e.QuantityChanged.Any(p => p.NewSize < p.OldSize && this.IsStaircase(p.Item))
                );

            if (usedStaircase)
                this.FixLadders();
        }

        /// <inheritdoc cref="IWorldEvents.ObjectListChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (e.IsCurrentLocation)
                this.FixLadders();
        }

        /// <inheritdoc cref="IWorldEvents.NpcListChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnNpcListChanged(object sender, NpcListChangedEventArgs e)
        {
            if (e.IsCurrentLocation && e.Removed.Any())
                this.FixLadders();
        }

        /// <summary>Detect any ladders in the current location and mark them passable.</summary>
        private void FixLadders()
        {
            if (Game1.currentLocation is MineShaft)
            {
                Layer layer = Game1.currentLocation.map.GetLayer("Buildings");
                if (layer == null)
                    return;

                for (int x = 0; x < layer.LayerWidth; x++)
                {
                    for (int y = 0; y < layer.LayerHeight; y++)
                    {
                        Tile tile = layer.Tiles[x, y];
                        if (tile?.TileIndex == 173)
                            tile.TileIndexProperties.Add(new KeyValuePair<string, PropertyValue>("Passable", "T"));
                    }
                }
            }
        }

        /// <summary>Get whether an item is a staircase.</summary>
        /// <param name="item">The item to check.</param>
        private bool IsStaircase(Item item)
        {
            return
                item is SObject obj
                && obj.bigCraftable.Value
                && obj.ParentSheetIndex == 71;
        }
    }
}
