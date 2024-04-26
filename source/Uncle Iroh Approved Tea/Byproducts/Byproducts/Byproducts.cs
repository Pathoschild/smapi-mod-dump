/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slimerrain/stardew-mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using SpaceCore.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Triggers;
using xTile.Tiles;

namespace Byproducts
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SpaceEvents.OnItemEaten += ItemEvents_OnEat;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player eats.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void ItemEvents_OnEat(object sender, EventArgs e)
        {
            // if item being eaten isn't an object?
            if (Game1.player.itemToEat is not StardewValley.Object food) return;

            // Add byproducts from eaten oranges to the inventory, or drop it on the ground if full
            if (food.DisplayName.Equals("Orange"))
            {
                StardewValley.Object byproduct = new StardewValley.Object("slimerrain.uncleirohapprovedteacp_Orange_Peel", 1, false, 50, food.Quality);

                if (Game1.player.couldInventoryAcceptThisItem(byproduct)) { Game1.player.addItemToInventory(byproduct); }
                else { Game1.createItemDebris(byproduct, Game1.player.Position, -1); }
            }
        }
    }
}