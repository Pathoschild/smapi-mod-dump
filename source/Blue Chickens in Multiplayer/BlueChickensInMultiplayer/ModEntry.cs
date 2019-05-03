using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BlueChickensInMultiplayer
{
    public class ModEntry : Mod
    {
        // Persistent list of animals kept on the farm
        private readonly List<FarmAnimal> _animals = new List<FarmAnimal>();
        
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += Initialize;
            SaveEvents.AfterReturnToTitle += RemoveCheckFunc;
        }

        private void Initialize(object sender, EventArgs e)
        {
            // At start, add all animals on the farm to the persistent list
            foreach (var animal in Game1.getFarm().getAllFarmAnimals())
            {
                _animals.Add(animal);
            }

            // Tell the game to update the list every quarter second
            GameEvents.QuarterSecondTick += CheckAddedAnimal;
        }

        // Function to gather info on any animals that may have just been purchased/hatched
        private void CheckAddedAnimal(object sender, EventArgs e)
        {
            // Get a new list of all animals on the farm to compare with the old
            var animals = Game1.getFarm().getAllFarmAnimals();
            _animals.RemoveAll(a => !animals.Contains(a));

            // Loop in reverse order so elements can be removed from the list safely
            for (var i = animals.Count - 1; i >= 0; i--)
            {
                var animal = animals[i];

                // Check if the animal is already being tracked
                var existing = _animals.Find(a => a == animal);

                if (existing != null)
                {
                    // Animal already existed, remove it from the list
                    animals.RemoveAt(i);
                }
            }

            // These animals are newly created, now check if any are chickens
            foreach (var animal in animals)
            {
                // If animal is any kind of chicken other than Void Chicken
                if (animal.type.Contains("Chicken") && !animal.type.Equals("Void Chicken"))
                {
                    Monitor.Log($"New chicken detected: {animal.type} {animal.displayName}");

                    // Only do this if in multiplayer mode
                    if (Game1.IsMultiplayer)
                    {
                        // Try to determine if players have seen the event with Shane to unlock Blue Chickens
                        bool eventSeen = false;
                        // First check if this player has seen it
                        if (Game1.player.eventsSeen.Contains(3900074))
                        {
                            eventSeen = true;
                        }
                        // Now check if any others have
                        foreach (Farmer farmer in Game1.otherFarmers.Values)
                        {
                            if (farmer.eventsSeen.Contains(3900074))
                            {
                                eventSeen = true;
                            }
                        }

                        // Now, if any player has seen the event, randomly turn the new chicken into a blue chicken
                        if (eventSeen)
                        {
                            //Monitor.Log($"At least one player has seen event");
                            if (Game1.random.NextDouble() < 0.25)
                            {
                                animal.type.Set("Blue Chicken");
                                animal.reload(animal.home);
                                Monitor.Log($"Chicken turned blue");
                            }
                        }
                        /*else
                        {
                            Monitor.Log($"Event not seen, skipping check");
                        }*/
                    }
                }

                // Put it into the list after all changes are done
                _animals.Add(animal);
            }
        }

        // Function to get rid of the addition to the event, primarily for when returning to title
        private void RemoveCheckFunc(object sender, EventArgs e)
        {
            GameEvents.QuarterSecondTick -= CheckAddedAnimal;
        }
    }
}
