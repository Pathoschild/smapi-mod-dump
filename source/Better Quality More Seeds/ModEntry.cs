using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace SB_BQMS
{

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        SerializableDictionary<StardewValley.Object, allSeedMakerValueContainer> allSeedMakers;
        StardewValley.Object previousHeldItem;

        GameLocation previousLocation;
        bool isInitialized;

        /*********
        ** Public methods
        *********/
        /// <summary>Initialise the mod.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += initializeMod;
            SaveEvents.AfterReturnToTitle += ResetMod;
            GameEvents.UpdateTick += ModUpdate;
            allSeedMakers = new SerializableDictionary<StardewValley.Object, allSeedMakerValueContainer>();
            isInitialized = false;
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
            isInitialized = true;
        }

        private void ResetMod(object sender, EventArgs e)
        {
            isInitialized = false;
        }

        private void ModUpdate(object sender, EventArgs e)
        {
            if (isInitialized)
            {
                if ( (Game1.player.currentLocation.name != null && previousLocation.name != Game1.player.currentLocation.name ) )
                {
                    allSeedMakers.Clear();
                    foreach (KeyValuePair<Vector2, StardewValley.Object> allObjects in Game1.player.currentLocation.objects)
                    {
                        if (allObjects.Value.name.Equals("Seed Maker"))
                        {
                            allSeedMakers.Add(allObjects.Value, new allSeedMakerValueContainer(null, allObjects.Value.heldObject != null? true : false));
                        }
                    }
                    previousLocation = Game1.player.currentLocation;
                }
                List<StardewValley.Object> seedMakers = allSeedMakers.Keys.ToList();
                foreach (StardewValley.Object seedMaker in seedMakers)
                {
                    if(seedMaker.heldObject != null && allSeedMakers[seedMaker].hasBeenChecked == false && allSeedMakers[seedMaker].droppedObject == null)
                    {
                        allSeedMakers[seedMaker].droppedObject = previousHeldItem;
                        seedMaker.heldObject.addToStack(allSeedMakers[seedMaker].droppedObject.quality == 4 ? allSeedMakers[seedMaker].droppedObject.quality - 1 : allSeedMakers[seedMaker].droppedObject.quality);
                        allSeedMakers[seedMaker].hasBeenChecked = true;
                    }
                    if(seedMaker.heldObject == null && allSeedMakers[seedMaker].hasBeenChecked == true)
                    {
                        allSeedMakers[seedMaker].droppedObject = null;
                        allSeedMakers[seedMaker].hasBeenChecked = false;
                    }
                }
                previousHeldItem = Game1.player.ActiveObject;
            }
        }
    }
}