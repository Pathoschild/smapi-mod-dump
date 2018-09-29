using System;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace Cobalt.Framework
{
    internal static class Items
    {
        internal static void init()
        {
            PlayerEvents.InventoryChanged += replaceModItems;
            LocationEvents.CurrentLocationChanged += watchCurrentLocation_1;
            GameEvents.OneSecondTick += periodicUpdate;
        }

        private static void periodicUpdate(object sender, EventArgs args)
        {
            if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is WateringCan wcan)
            {
                if (wcan.upgradeLevel == 5 && wcan.WaterLeft == wcan.waterCanMax)
                    wcan.waterCanMax = wcan.WaterLeft = 150;
            }

            foreach (var item in Game1.player.items)
            {
                if (item == null)
                    continue;

                if (item is Tool tool && tool.upgradeLevel >= 4)
                {
                    if (!Game1.player.knowsRecipe("Cobalt Bar"))
                        Game1.player.craftingRecipes.Add("Cobalt Bar", 0);
                }
                if (item is StardewValley.Object && item.parentSheetIndex == 645 && Game1.player.knowsRecipe("Cobalt Bar"))
                {
                    if (!Game1.player.knowsRecipe("Cobalt Sprinkler"))
                        Game1.player.craftingRecipes.Add("Cobalt Sprinkler", 0);
                }
            }
        }

        private static void replaceModItems(object sender, EventArgsInventoryChanged args)
        {
            for (int i = 0; i < args.Inventory.Count; ++i)
            {
                var item = args.Inventory[i];
                if (item == null)
                    continue;

                if (item.parentSheetIndex == CobaltSprinklerObject.INDEX && !(item is CobaltSprinklerObject))
                {
                    args.Inventory[i] = new CobaltSprinklerObject();
                }
                else continue;

                Log.trace("Replacing a vanilla instance of custom item with my own. (Inventory)");
                args.Inventory[i].Stack = item.Stack;
            }
        }

        private static void watchCurrentLocation_1(object sender, EventArgsCurrentLocationChanged args)
        {
            if (args.PriorLocation != null)
                args.PriorLocation.objects.CollectionChanged -= watchCurrentLocation_2;
            if (args.NewLocation != null)
                args.NewLocation.objects.CollectionChanged += watchCurrentLocation_2;
        }

        private static void watchCurrentLocation_2(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var key_ in args.NewItems)
                {
                    var key = (Vector2)key_;
                    var obj = Game1.currentLocation.objects[key];
                    if (obj.parentSheetIndex == CobaltSprinklerObject.INDEX && !(obj is CobaltSprinklerObject))
                    {
                        Game1.currentLocation.objects[key] = new CobaltSprinklerObject();
                    }
                    else continue;
                    Log.trace("Replacing a vanilla instance of custom item with my own. (World)");
                    Game1.currentLocation.objects[key].tileLocation = key;
                }
            }
        }
    }
}
