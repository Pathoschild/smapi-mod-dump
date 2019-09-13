using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using System.Collections.Generic;
using StardewValley.Objects;
using StardewValley.Buildings;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace FirstMod
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.DayStarted += this.OnNewDay;

        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.");
        }

        private void OnNewDay(object sender, DayStartedEventArgs e)
        {
            Item diamond = new StardewValley.Object(72, 1);
            int numDiamonds = CountOwnedItems(diamond);
            String formatedMsg = String.Format("You have {0} diamonds", numDiamonds);
            HUDMessage msg = new HUDMessage(formatedMsg);
            Game1.addHUDMessage(msg);
        }

        private IEnumerable<Item> GetAllOwnedItems()
        {
            List<Item> items = new List<Item>();

            items.AddRange(Game1.player.Items);
            
            //all locations
            foreach(GameLocation location in GetLocations())
            {
                //for every map object
                foreach(SObject item in location.objects.Values)
                {
                    //if its a chest
                    if(item is Chest chest)
                    {
                        if (chest.playerChest.Value)
                        {
                            items.Add(chest);
                            items.AddRange(chest.items);
                        }

                        //auto grabber
                        else if(item.ParentSheetIndex == 165 && item.heldObject.Value is Chest grabberChest)
                        {
                            items.Add(item);
                            items.AddRange(grabberChest.items);
                        }

                        //cask
                        else if(item is Cask)
                        {
                            items.Add(item);
                            items.Add(item.heldObject.Value); // cask contents can be retrieved anytime
                        }

                        //craftable
                        else if (item.bigCraftable.Value)
                        {
                            items.Add(item);
                            if(item.MinutesUntilReady == 0)
                            {
                                items.Add(item.heldObject.Value);
                            }
                        }

                        //anything else
                        else if (!item.IsSpawnedObject)
                        {
                            items.Add(item);
                            items.Add(item.heldObject.Value);
                        }
                    }
                }

                //furniture
                if(location is DecoratableLocation decoratableLocation)
                {
                    foreach(Furniture furniture in decoratableLocation.furniture)
                    {
                        items.Add(furniture);
                        items.Add(furniture.heldObject.Value);
                    }
                }

                //building output
                if(location is Farm farm)
                {
                    foreach(var building in farm.buildings)
                    {
                        if(building is Mill mill)
                        {
                            items.AddRange(mill.output.Value.items);
                        }
                        else if(building is JunimoHut hut)
                        {
                            items.AddRange(hut.output.Value.items);
                        }
                    }
                }
            }

            return items.Where(p => p != null);
        }

        private IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations
                .Concat(from location in Game1.locations.OfType<BuildableGameLocation>()
                        from building in location.buildings
                        where building.indoors.Value != null
                        select building.indoors.Value
                );
        }

        /// <summary>Count how many of an item the player owns.</summary>
        /// <param name="item">The item to count.</param>
        public int CountOwnedItems(Item item)
        {
            return (
                from worldItem in this.GetAllOwnedItems()
                where this.AreEquivalent(worldItem, item)
                let canStack = worldItem.canStackWith(worldItem)
                select canStack ? Math.Max(1, worldItem.Stack) : 1
            ).Sum();
        }

        /// <summary>Get whether two items are the same type (ignoring flavour text like 'blueberry wine' vs 'cranberry wine').</summary>
        /// <param name="a">The first item to compare.</param>
        /// <param name="b">The second item to compare.</param>
        private bool AreEquivalent(Item a, Item b)
        {
            return
                // same generic item type
                a.GetType() == b.GetType()
                && a.Category == b.Category
                && a.ParentSheetIndex == b.ParentSheetIndex

                // same discriminators
                //&& a.GetSpriteType() == b.GetSpriteType()
                && (a as Boots)?.indexInTileSheet == (b as Boots)?.indexInTileSheet
                && (a as BreakableContainer)?.Type == (b as BreakableContainer)?.Type
                && (a as Fence)?.isGate == (b as Fence)?.isGate
                && (a as Fence)?.whichType == (b as Fence)?.whichType
                && (a as Hat)?.which == (b as Hat)?.which
                && (a as MeleeWeapon)?.type == (b as MeleeWeapon)?.type
                && (a as Ring)?.indexInTileSheet == (b as Ring)?.indexInTileSheet
                && (a as Tool)?.InitialParentTileIndex == (b as Tool)?.InitialParentTileIndex
                && (a as Tool)?.CurrentParentTileIndex == (b as Tool)?.CurrentParentTileIndex;
        }
    }

}