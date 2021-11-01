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
        private readonly PerScreen<Dictionary<Vector2, SObject>> Gates = new PerScreen<Dictionary<Vector2, Object>>(() => new SerializableDictionary<Vector2, SObject>());

        /// <summary>The last player tile position for which we checked for gates.</summary>
        private readonly PerScreen<Vector2> LastPlayerTile = new PerScreen<Vector2>(() => new Vector2(-1));


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                this.ResetGateList();
        }

        /// <summary>Raised after objects are added or removed in a location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            this.ResetGateList();
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
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

            // update all gates
            var adjacentTiles = new HashSet<Vector2>(this.GetSearchTiles(playerTile));
            foreach (var pair in this.Gates.Value)
            {
                Vector2 tile = pair.Key;
                SObject gate = pair.Value;

                if (gate.isPassable() != adjacentTiles.Contains(tile))
                    gate.checkForAction(Game1.player);
            }
        }

        /// <summary>Reset the gate cache for the current location.</summary>
        private void ResetGateList()
        {
            this.Gates.Value.Clear();
            this.LastPlayerTile.Value = new Vector2(-1);

            foreach (var pair in Game1.currentLocation.objects.Pairs)
            {
                Vector2 tile = pair.Key;
                SObject obj = pair.Value;

                if (obj.Name == "Gate")
                    this.Gates.Value[tile] = obj;
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
