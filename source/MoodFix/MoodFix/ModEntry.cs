/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/vaindil/sdv-moodfix
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace MoodFix
{
    public class ModEntry : Mod
    {
        private readonly List<AnimalWrapper> _animals = new List<AnimalWrapper>();
        private bool _hasProfession;

        public override void Entry(IModHelper helper)
        {
            TimeEvents.AfterDayStarted += CheckPlayerProfessions;
            SaveEvents.AfterLoad += Initialize;
            SaveEvents.AfterReturnToTitle += KillItWithFire;
        }

        private void Initialize(object sender, EventArgs e)
        {
            foreach (var animal in Game1.getFarm().getAllFarmAnimals())
            {
                _animals.Add(new AnimalWrapper(animal));
            }

            GameEvents.QuarterSecondTick += CheckAnimalHappiness;
        }

        private void CheckPlayerProfessions(object sender, EventArgs e)
        {
            _hasProfession = Game1.player.professions.Contains(2) || Game1.player.professions.Contains(3);
        }

        private void CheckAnimalHappiness(object sender, EventArgs e)
        {
            var animals = Game1.getFarm().getAllFarmAnimals();
            _animals.RemoveAll(a => !animals.Contains(a.Animal));

            // Loop in reverse order so elements can be removed from the list safely
            for (var i = animals.Count - 1; i >= 0; i--)
            {
                var animal = animals[i];

                // Check if the animal is already being tracked
                var existing = _animals.Find(a => a.Animal == animal);

                if (existing != null)
                {
                    // If the happiness didn't change then there's no reason to run the following calculations
                    if (existing.CurrentHappiness != animal.happiness)
                    {
                        // These are used for the following check to fix bug where animal happiness drops after 6pm
                        var happinessChange = existing.CurrentHappiness - animal.happiness;
                        var isAnimalIndoors = ((AnimalHouse)animal.home.indoors).animals.ContainsValue(animal);

                        // If the time is 6pm or later, the animal is indoors, and the happiness change was less than 10,
                        // this is the bug. Just revert the drop to correct the problem.
                        if (Game1.timeOfDay >= 1800 && isAnimalIndoors && happinessChange > 0 && happinessChange <= 10)
                        {
                            // Purposely commented out, this would be a bit too much spam
                            // Monitor.Log($"Fixing animal happiness: {animal.name}, from {animal.happiness} to {existing.CurrentHappiness}");

                            animal.happiness = (byte)existing.CurrentHappiness;
                        }
                        // Did the happiness change because the animal was petted, and did that cause an overflow?
                        else if (_hasProfession && existing.WasOverflown(animal.happiness))
                        {
                            animal.happiness = 255;
                            existing.CurrentHappiness = 255;
                            Monitor.Log($"Happiness overflow detected: {animal.type} {animal.displayName}, setting to 255");
                        }
                    }

                    // Animal is taken care of so remove it from the list
                    animals.RemoveAt(i);
                }
            }

            // These animals are new to the party (they weren't removed by the previous loop)
            foreach (var animal in animals)
            {
                _animals.Add(new AnimalWrapper(animal));
                Monitor.Log($"New animal detected: {animal.type} {animal.displayName}");
            }
        }

        private void KillItWithFire(object sender, EventArgs e)
        {
            GameEvents.QuarterSecondTick -= CheckAnimalHappiness;
        }
    }

    /// <summary>
    /// Wrapper around farm animals to track information
    /// </summary>
    internal class AnimalWrapper
    {
        public AnimalWrapper(FarmAnimal animal)
        {
            Animal = animal;
            CurrentHappiness = animal.happiness;
            HappinessChangeWhenPetted = (40 - animal.happinessDrain) * 2;
        }

        public bool WasOverflown(int newHappiness)
        {
            return newHappiness == CurrentHappiness + HappinessChangeWhenPetted - 256;
        }

        /// <summary>
        /// The animal's internal ID
        /// </summary>
        public FarmAnimal Animal { get; set; }

        /// <summary>
        /// The current happiness of the animal to compare against when the value changes
        /// </summary>
        public int CurrentHappiness { get; set; }

        /// <summary>
        /// The amount the animal's happiness will change when petted
        /// </summary>
        public int HappinessChangeWhenPetted { get; set; }
    }
}
