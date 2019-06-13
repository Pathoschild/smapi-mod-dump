using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Objects;
using StardewValley.Network;

namespace SB_BQMS
{

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        Dictionary<StardewValley.Object, AllSeedMakerValueContainer> allSeedMakers;
        Dictionary<StardewValley.Object, AllChestsValueContainer> allChests;

        StardewValley.Object previousHeldItem;

        GameLocation previousLocation;
        bool isInitialized;
        bool hasAutomate;
        /*********
        ** Public methods
        *********/
        /// <summary>Initialise the mod.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            if (Helper.ModRegistry.IsLoaded("Pathoschild.Automate")) hasAutomate = true; else hasAutomate = false;

            Helper.Events.GameLoop.SaveLoaded += initializeMod;
            Helper.Events.GameLoop.ReturnedToTitle += resetMod;
            Helper.Events.GameLoop.UpdateTicked += ModUpdate;
            Helper.Events.World.ObjectListChanged += populateSeedmakers;
            allSeedMakers = new Dictionary<StardewValley.Object, AllSeedMakerValueContainer>();
            if (hasAutomate) allChests = new Dictionary<StardewValley.Object, AllChestsValueContainer>();
            isInitialized = false;
        }

        //  For Automate  We now need to get all seed makers available.
        //  So making this a method on its own
        private void populateSeedmakersBase()
        {
            allSeedMakers.Clear();
            if (hasAutomate) allChests.Clear();

            foreach (GameLocation location in Game1.locations)
            {
                OverlaidDictionary allObjects = location.objects;
                    foreach (StardewValley.Object ob in allObjects.Values)
                {
                    if (ob.name.Equals("Seed Maker"))
                    {
                        if (!allSeedMakers.ContainsKey(ob))
                            allSeedMakers.Add(ob, new AllSeedMakerValueContainer(null, location, ob != null ? true : false));
                    }
                    if (hasAutomate)
                    {
                        if ((ob is Chest) && (ob as Chest).playerChest)
                        {
                            if (!allChests.ContainsKey(ob))
                                allChests.Add(ob, new AllChestsValueContainer(null, location, false));
                        }
                    }
                }

            }
            previousLocation = Game1.player.currentLocation;
        }

        private void populateSeedmakers(object sender, ObjectListChangedEventArgs e)
        {
            populateSeedmakersBase();
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Initializes the Mod. Adds all Seed Makers in the Game
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void initializeMod(object sender, EventArgs e)
        {
            previousLocation = Game1.player.currentLocation;
            //TODO: Add the allSeedMakers population code here.
            //TODO: add a LocationObjectsChanged method to check if the allSeedMakers variable needs to be updated
            //Note: The Method has been designed to do this since alpha! LOL
            populateSeedmakersBase();
            isInitialized = true;
        }

        private void resetMod(object sender, EventArgs e)
        {
            isInitialized = false;
        }

        private StardewValley.Object checkChests(StardewValley.Object seedmaker)
        {
            OverlaidDictionary allObjectsinLocation = allSeedMakers[seedmaker].location.objects;
            OverlaidDictionary.KeysCollection keyCollection = allObjectsinLocation.Keys;
            foreach (Vector2 objectKey in keyCollection)
            {
                if (allObjectsinLocation[objectKey] is Chest && Vector2.Distance(objectKey, seedmaker.TileLocation) <= 1.0)
                {
                    Chest thisChest = (allObjectsinLocation[objectKey] as Chest);
                    Netcode.NetObjectList<Item> currentItems = thisChest.items;
                    for (int index = 0; index < currentItems.Count; index++)
                    {
                        if (currentItems[index] == null || currentItems[index].ParentSheetIndex == 433) continue;
                        {
                            Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Crops");
                            foreach (KeyValuePair<int, string> keyValuePair in dictionary)
                            {
                                if (Convert.ToInt32(keyValuePair.Value.Split('/')[3]) == currentItems[index].ParentSheetIndex)
                                {
                                    allChests[thisChest].previousItem = currentItems[index];
                                    return currentItems[index] as StardewValley.Object;
                                }
                            }
                        }
                    }
                    if (allChests[thisChest].previousItem != null)
                    {
                        Item returnItem = allChests[thisChest].previousItem;
                        allChests[thisChest].previousItem = null;
                        return returnItem as StardewValley.Object;
                    }
                }
            }
            return null;
        }

        private void ModUpdate(object sender, EventArgs e)
        {
            if (isInitialized)
            {
                List<StardewValley.Object> seedMakers = allSeedMakers.Keys.ToList();
                foreach (StardewValley.Object seedMaker in seedMakers)
                {
                    if (seedMaker.heldObject == null && allSeedMakers[seedMaker].hasBeenChecked == true)
                    {
                        allSeedMakers[seedMaker].droppedObject = null;
                        allSeedMakers[seedMaker].hasBeenChecked = false;
                    }
                    if (seedMaker.heldObject != null && allSeedMakers[seedMaker].hasBeenChecked == false && allSeedMakers[seedMaker].droppedObject == null)
                    {
                        if (hasAutomate)
                        {
                            // This should invoke checkChests, which will scan the seed maker for adjacent chests,
                            // Then checks if those chests inventory decreased.
                            StardewValley.Object droppedChestObject = checkChests(seedMaker);
                            if (droppedChestObject != null)
                            {
                                allSeedMakers[seedMaker].droppedObject = droppedChestObject;
                                seedMaker.addToStack(allSeedMakers[seedMaker].droppedObject.Quality == 4 ? allSeedMakers[seedMaker].droppedObject.Quality - 1 : allSeedMakers[seedMaker].droppedObject.Quality);
                                allSeedMakers[seedMaker].hasBeenChecked = true;
                                continue;
                            }
                        }
                        //  Checks if the Farmer is in the same location as the seed maker
                        //  This will save up cpu time for the ones that are not on location
                        if (previousHeldItem != null && Game1.player.currentLocation == allSeedMakers[seedMaker].location)
                        {
                            allSeedMakers[seedMaker].droppedObject = previousHeldItem;
                            seedMaker.addToStack(allSeedMakers[seedMaker].droppedObject.Quality == 4 ? allSeedMakers[seedMaker].droppedObject.Quality - 1 : allSeedMakers[seedMaker].droppedObject.Quality);
                            allSeedMakers[seedMaker].hasBeenChecked = true;
                            continue;
                        }
                    }
                }
                previousHeldItem = Game1.player.ActiveObject;
            }
        }
    }
}