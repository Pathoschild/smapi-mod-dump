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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using SObject = StardewValley.Object;

namespace AutoGate
{
    /// <summary>The mod entry class.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The gates in the current location.</summary>
        private readonly PerScreen<Dictionary<Vector2, Fence>> Gates = new(() => new());

        /// <summary>The last player tile position for which we checked for gates.</summary>
        private readonly PerScreen<Vector2> LastPlayerTile = new(() => new Vector2(-1));


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
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
                this.ResetGateList();
        }

        /// <inheritdoc cref="IWorldEvents.ObjectListChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            this.ResetGateList();
        }

        /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // skip if nothing to do
            if (!Context.IsWorldReady || !this.Gates.Value.Any())
                return;

            // skip if we already handled gates from this tile
            Vector2 playerTile = Game1.player.getTileLocation();
            if (playerTile == this.LastPlayerTile.Value)
                return;
            this.LastPlayerTile.Value = playerTile;

            // get gates that should be open
            // (We need to do this before applying changes, so we don't close a double-gate when one side is out of range.)
            HashSet<Vector2> shouldBeOpen = new();
            foreach (Vector2 tile in this.GetSearchTiles(playerTile))
            {
                if (!this.Gates.Value.ContainsKey(tile))
                    continue;

                shouldBeOpen.Add(tile);
                foreach (Vector2 connectedTile in Utility.getAdjacentTileLocations(tile))
                {
                    if (this.Gates.Value.ContainsKey(connectedTile))
                        shouldBeOpen.Add(connectedTile);
                }
            }

            // step 2: update gates
            foreach ((Vector2 tile, Fence gate) in this.Gates.Value)
            {
                bool open = shouldBeOpen.Contains(tile);
                int expectedPosition = open
                    ? Fence.gateOpenedPosition
                    : Fence.gateClosedPosition;

                if (gate.gatePosition.Value != expectedPosition)
                    gate.toggleGate(Game1.player, open: open);
            }
        }

        /// <summary>Reset the gate cache for the current location.</summary>
        private void ResetGateList()
        {
            this.Gates.Value.Clear();
            this.LastPlayerTile.Value = new Vector2(-1);

            if (Game1.currentLocation?.objects != null)
            {
                foreach ((Vector2 tile, SObject obj) in Game1.currentLocation.objects.Pairs)
                {
                    if (obj is Fence fence && fence.isGate.Value)
                        this.Gates.Value[tile] = fence;
                }
            }
        }

        /// <summary>Get all tiles to search for a gate.</summary>
        /// <param name="tile">The tile from which to search for gates.</param>
        private IEnumerable<Vector2> GetSearchTiles(Vector2 tile)
        {
            return Utility
                .getAdjacentTileLocationsArray(tile)
                .Concat(new[] { tile });
        }
    }
}
