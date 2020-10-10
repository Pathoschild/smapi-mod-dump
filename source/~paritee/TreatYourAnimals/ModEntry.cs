/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Network;
using System.Collections.Generic;
using TreatYourAnimals.Framework;
using xTile.Dimensions;

namespace TreatYourAnimals
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private CharacterTreatedData CharacterTreatedData;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Load config
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // Events
            this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            // TODO: Change cursor to gift on hover with a valid object to give to animal - use same rectangle
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Refresh the tracked character treats each day
            this.SetDailyTreatsData(new CharacterTreatedData());
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
            {
                return;
            }

            // We only care about action buttons
            if (!e.Button.IsActionButton())
            {
                return;
            }

            // We only care if the player is holding an object ...
            if (Game1.player.ActiveObject == null)
            {
                return;
            }

            // ... and that object is sort-of edible
            if (Game1.player.ActiveObject.Edibility <= CharacterTreat.INEDIBLE_THRESHOLD)
            {
                return;
            }

            //Vector2 index = new Vector2((float)((Game1.getOldMouseX() + Game1.viewport.X) / 64), (float)((Game1.getOldMouseY() + Game1.viewport.Y) / 64));
            Location tileLocation = new Location((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y);
            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64);

            bool intersected = false;

            if (Game1.player.currentLocation is AnimalHouse)
            {
                AnimalHouse animalHouse = Game1.player.currentLocation as AnimalHouse;

                intersected = this.AttemptToGiveTreatToFarmAnimals(animalHouse.animals, rectangle);
            }
            else if (Game1.player.currentLocation is Farm)
            {
                Farm farm = Game1.player.currentLocation as Farm;

                intersected = this.AttemptToGiveTreatToFarmAnimals(farm.animals, rectangle);
            }

            if (!intersected)
            {
                intersected = this.AttemptToGiveTreatToHorsesAndPets(rectangle);
            }

            // Always suppress the button if we intersected as an attempt to treat
            // Blocks weird behaviour of mounting if you meant to treat a horse that was already treated
            if (intersected)
            {
                this.Helper.Input.Suppress(e.Button);
            }
        }

        private bool AttemptToGiveTreatToFarmAnimals(NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> animals, Microsoft.Xna.Framework.Rectangle rectangle)
        {
            foreach (KeyValuePair<long, FarmAnimal> pair in animals.Pairs)
            {
                if (pair.Value.GetBoundingBox().Intersects(rectangle))
                {
                    this.AttemptToGiveTreatToFarmAnimal(pair.Value);
                    
                    // Intersects always return true
                    return true;
                }
            }

            return false;
        }

        private bool AttemptToGiveTreatToFarmAnimal(FarmAnimal farmAnimal)
        {
            string type = farmAnimal.GetType().ToString();
            string id = farmAnimal.myID.ToString();

            FarmAnimalTreat treatHandler = new FarmAnimalTreat(farmAnimal, this.Config);

            // Refuse a poisonous treat
            if (treatHandler.IsPoisonous(Game1.player.ActiveObject))
            {
                treatHandler.RefuseTreat(true);

                return false;
            }

            // Can only give a treat once per day
            if (this.GivenTreatToday(type, id))
            {
                treatHandler.RefuseTreat(false);

                return false;
            }

            treatHandler.GiveTreat();

            this.TrackGivenTreat(type, id);

            return true;
        }

        private bool AttemptToGiveTreatToHorsesAndPets(Microsoft.Xna.Framework.Rectangle rectangle)
        {
            foreach (NPC character in Game1.player.currentLocation.characters)
            {
                // We only care about Horses and Pets
                if (!(character is Horse) && !(character is Pet))
                    continue;

                // We only care if they're intersecting the animal
                if (character.GetBoundingBox().Intersects(rectangle))
                {
                    if (character is Horse)
                    {
                        // Check if horse has a name
                        if (character.Name.Length <= 0)
                        {
                            // We don't want to stop the naming prompt even with an intercept
                            return false;
                        }

                        this.AttemptToGiveTreatToHorse(character as Horse);
                    }
                    else
                    {
                        this.AttemptToGiveTreatToPet(character as Pet);
                    }

                    // Intersects always return true
                    return true;
                }
            }

            return false;
        }

        private bool AttemptToGiveTreatToHorse(Horse horse)
        {
            string type = horse.GetType().ToString();
            string id = horse.id.ToString();

            HorseTreat treatHandler = new HorseTreat(horse, this.Config);

            // Refuse a poisonous treat
            if (treatHandler.IsPoisonous(Game1.player.ActiveObject))
            {
                treatHandler.RefuseTreat(true);

                return false;
            }

            // Can only give a treat once per day
            if (this.GivenTreatToday(type, id))
            {
                treatHandler.RefuseTreat(false);

                return false;
            }

            treatHandler.GiveTreat();

            this.TrackGivenTreat(type, id);

            return true;
        }

        private bool AttemptToGiveTreatToPet(Pet pet)
        {
            string type = pet.GetType().ToString();
            string id = pet.id.ToString();

            PetTreat treatHandler = new PetTreat(pet, this.Config);

            // Refuse a poisonous treat
            if (treatHandler.IsPoisonous(Game1.player.ActiveObject))
            {
                treatHandler.RefuseTreat(true);

                return false;
            }

            // Can only give a treat once per day
            if (this.GivenTreatToday(type, id))
            {
                treatHandler.RefuseTreat(false);

                return false;
            }

            treatHandler.GiveTreat();

            this.TrackGivenTreat(type, id);

            return true;
        }

        private bool GivenTreatToday(string type, string id)
        {
            CharacterTreatedData model = this.GetDailyTreatsData();

            // Check if the entry already was treated
            return model.Characters.Contains(model.FormatEntry(type, id));
        }

        private void TrackGivenTreat(string type, string id)
        {
            CharacterTreatedData model = this.GetDailyTreatsData();
            string entry = model.FormatEntry(type, id);

            model.Characters.Add(entry);

            this.SetDailyTreatsData(model);
        }

        private CharacterTreatedData GetDailyTreatsData()
        {
            return this.CharacterTreatedData;
        }

        private void SetDailyTreatsData(CharacterTreatedData model)
        {
            this.CharacterTreatedData = model;
        }
    }
}
