using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace Cobalt.Framework
{
    internal static class Items
    {
        internal static void init(IModEvents events)
        {
            events.Player.InventoryChanged += OnInventoryChanged;
            events.World.ObjectListChanged += OnObjectListChanged;
            events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnUpdateTicked(object sender, EventArgs e)
        {
            if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is WateringCan wcan)
            {
                if (wcan.UpgradeLevel == 5 && wcan.WaterLeft == wcan.waterCanMax)
                    wcan.waterCanMax = wcan.WaterLeft = 150;
            }

            foreach (var item in Game1.player.Items)
            {
                if (item == null)
                    continue;

                if (item is Tool tool && tool.UpgradeLevel >= 4)
                {
                    if (!Game1.player.knowsRecipe("Cobalt Bar"))
                        Game1.player.craftingRecipes.Add("Cobalt Bar", 0);
                }
                if (item is StardewValley.Object && item.ParentSheetIndex == 645 && Game1.player.knowsRecipe("Cobalt Bar"))
                {
                    if (!Game1.player.knowsRecipe("Cobalt Sprinkler"))
                        Game1.player.craftingRecipes.Add("Cobalt Sprinkler", 0);
                }
            }
        }

        /// <summary>Raised after items are added or removed to a player's inventory.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            // replace mod items
            if (e.IsLocalPlayer)
            {
                IList<Item> inventory = Game1.player.Items;

                for (int i = 0; i < inventory.Count; ++i)
                {
                    var item = inventory[i];
                    if (item == null)
                        continue;

                    if (item.ParentSheetIndex == CobaltSprinklerObject.INDEX && !(item is CobaltSprinklerObject))
                    {
                        inventory[i] = new CobaltSprinklerObject();
                    }
                    else continue;

                    Log.trace("Replacing a vanilla instance of custom item with my own. (Inventory)");
                    inventory[i].Stack = item.Stack;
                }
            }
        }

        /// <summary>Raised after objects are added or removed in a location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (e.Location == Game1.currentLocation)
            {
                foreach (KeyValuePair<Vector2, Object> pair in e.Added)
                {
                    var key = pair.Key;
                    var obj = pair.Value;
                    if (obj.ParentSheetIndex == CobaltSprinklerObject.INDEX && !(obj is CobaltSprinklerObject))
                        Game1.currentLocation.objects[key] = new CobaltSprinklerObject();
                    else continue;

                    Log.trace("Replacing a vanilla instance of custom item with my own. (World)");
                    Game1.currentLocation.objects[key].TileLocation = key;
                }
            }
        }
    }
}
