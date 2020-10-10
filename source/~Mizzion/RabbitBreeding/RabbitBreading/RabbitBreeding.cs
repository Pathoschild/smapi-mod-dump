/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace RabbitBreading
{
    public class RabbitBreeding : Mod
    {
        private static readonly Random _random = new Random();
        private static ModConfig _config;
        private bool _debug;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>() ?? new ModConfig();
            _debug = true;
            if(_debug)
                helper.ConsoleCommands.Add("breeder_dev", "Command to be used while debugging.\n\nUsage: breeder_dev <value>\n- value: run or test.", this.DoDev);
            //Set up events
            helper.Events.GameLoop.Saved += this.Saved;
        }

        private void Saved(object sender, SavedEventArgs e)
        {
            Farm farm = Game1.getFarm();
            int amountOfBreeders = GetBreeders(farm);
            int maleRabbits = _config.GenderMatters ? GetMatureMales(farm) : amountOfBreeders;
            List<string>outty = new List<string>();

            foreach (FarmAnimal animal in farm.getAllFarmAnimals())
            {
                if (animal.type.Value == "Rabbit" && animal.age.Value >= animal.ageWhenMature.Value + 14 &&
                    (!animal.isMale() || _config.GenderMatters))
                {
                    if (_debug)
                        this.Monitor.Log("Passed the check, grabbing the home info now.", LogLevel.Alert);
                    Building home = animal.home;
                    int availableSpace = AvailableSpace(home);
                       /* (home.buildingType.Value == "Coop" ? 4 : home.buildingType.Value == "Big Coop" ? 8 : 12) -
                        home.currentOccupants.Value;*/
                    double babyChance = ChanceOfBabies(maleRabbits, animal);
                    int babyNum = NumberOfBabies(availableSpace, animal);
                    if (!(_random.NextDouble() <= babyChance) || availableSpace <= 0 ||
                        babyNum <= 0) continue;
                    for (int i = 0; i < babyNum; i++)
                    {
                        if (_debug)
                            this.Monitor.Log($"Trying to add baby number {i} now.", LogLevel.Alert);
                        this.AddBaby(home);
                        if (_debug)
                            this.Monitor.Log($"Baby number {i} should have been added.", LogLevel.Alert);
                    }
                    if (_debug)
                        this.Monitor.Log(
                            $"Name: {animal.Name} \n Happiness: {animal.happiness.Value} \n AvailableSpace: {availableSpace} \n ChanceOfBaby: {babyChance}");
                    outty.Add($"During the night, {animal.Name} gave birth to {babyNum} baby rabbits.");
                }
                else
                {
                    if (_debug)
                        this.Monitor.Log("The check failed.", LogLevel.Alert);
                }
            }
            if(outty.Count >= 1)
                Game1.multipleDialogues(outty.ToArray());
        }

        //Private methods that's used by the dev command
        private void DoDev(string command, string[] args)
        {
            if (args[0] == "run")
            {
                Farm farm = Game1.getFarm();

                foreach (FarmAnimal animal in farm.getAllFarmAnimals())
                {
                    if (animal.type.Value == "Rabbit")
                    {
                        animal.friendshipTowardFarmer.Value += 250;
                        this.Monitor.Log($"{animal.Name}'s friendship is now {animal.friendshipTowardFarmer.Value}.");
                    }
                }
            }
            if (args[0] == "test")
            {
                Farm farm = Game1.getFarm();

                foreach (FarmAnimal animal in farm.getAllFarmAnimals())
                {
                    if (animal.type.Value == "Rabbit")
                    {
                        Building home = animal.home;
                        int amountOfBreeders = GetBreeders(farm);
                        int maleRabbits = _config.GenderMatters ? GetMatureMales(farm) : amountOfBreeders;
                        int availableSpace = AvailableSpace(home);
                        double rnd = _random.NextDouble();
                        double babyChance = ChanceOfBabies(maleRabbits, animal);
                        int babyNumber = NumberOfBabies((int)availableSpace, animal);

                        this.Monitor.Log($"\n Animal Name: {animal.Name} \n Building Type: {home.buildingType.Value} \n IsMale: {animal.isMale()} \n AmtBreeders:{amountOfBreeders} \n Male Rabbits: {maleRabbits} \n Available Space: {availableSpace} \n Rnd: {rnd} \n BabyChance: {babyChance} \n BabyNumber: {babyNumber} \n MaxOccupants: {home.maxOccupants.Value}", LogLevel.Info);
                    }
                }
            }
            if (args[0] == "chance")
            {
                for(int i = 0; i < 99; i++)
                    Monitor.Log($"Random Chance: {_random.NextDouble()}.");
            }
        }
        //Private methods that's used by the Saved method
        private int AvailableSpace(Building building)
        {
            AnimalHouse animalHome = building.indoors.Value as AnimalHouse;
            var availableSpace = animalHome?.animalLimit.Value - animalHome?.animalsThatLiveHere.Count; 
            if (availableSpace == null) return 0;
            return (int) availableSpace;
        }
        private int NumberOfBabies(int availableSpace, FarmAnimal animal)
        {
            if (!_config.CanHaveMultiples)
                return 1;
            //Multiples allowed, so we move on.
            int min = _config.MinBabiesToAllow;//animal.happiness.Value / 80 - 1;
            int max = _config.MaxBabiesToAllow;//animal.happiness.Value / 16;
            int babies = _random.Next(min, max);
            return babies; //Math.Max(0, Math.Min(availableSpace, babies));
        }

        private void AddBaby(Building building)
        {
            try
            {
                FarmAnimal animal = new FarmAnimal("Rabbit", this.Helper.Multiplayer.GetNewID(),
                    Game1.player.UniqueMultiplayerID)
                {
                    home = building,
                    Name = "Rabbit",
                    homeLocation = {new Vector2(building.tileX.Value, building.tileY.Value)}
                };
                building.currentOccupants.Value += 1;

                var animalHouse = building.indoors.Value as AnimalHouse;
                animalHouse?.animals.Add(animal.myID.Value, animal);
                animalHouse?.animalsThatLiveHere.Add(animal.myID.Value);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"There was an error: \n{ex}");
            }
        }

        private int GetBreeders(Farm farm)
        {
            int totalRabbits = 0;
            int totalBreeders = 0;
            foreach (FarmAnimal animal in farm.getAllFarmAnimals())
            {
                if (animal.type.Value == "Rabbit")
                {
                    totalRabbits++;
                    if (animal.age.Value > animal.ageWhenMature.Value/* + 14*/)
                        totalBreeders++;
                }
            }
            if (_debug)
                this.Monitor.Log($"There are currently {totalRabbits} Rabbits and {totalBreeders} Breeders");
            return totalBreeders;
        }

        private static int GetMatureMales(Farm farm)
        {
            return farm.getAllFarmAnimals().Count(animal => animal.type.Value == "Rabbit" && animal.isMale() && !animal.isBaby());
        }

        private double ChanceOfBabies(int maleCount, FarmAnimal animal)
        {
            if (maleCount == 0)
                return 0.0f;
            double ageMatters = 0.0f;
            double happinessMatters = 0.0f;
            double friendshipMatters = 0.0f;
            double seasonMatters = 0.0f;
            double baseChance = 1.0f / _config.BaseChance - (double) animal.daysSinceLastFed.Value / 20;
            double maleCount1 = (double) maleCount / 100.0f;
            //Now we do the checks
            if (maleCount1 > 0.03f)
                maleCount1 = 0.03f;
            if (_config.AgeMatters)
                ageMatters = animal.age.Value / 420.0f;
            if (_config.AgeMatters && animal.age.Value > 112)
                ageMatters = 0.04f - animal.age.Value / 4200.0f;
            if (_config.AnimalHappinessMatters)
                happinessMatters = animal.happiness.Value / 4200.0f;
            if (_config.AnimalFriendshipMatters)
                friendshipMatters = animal.friendshipTowardFarmer.Value / 60000.0f;
            if (_config.SeasonMatters)
            {
                seasonMatters = Game1.currentSeason == "winter"
                    ? 0.01f
                    : Game1.currentSeason == "summer"
                        ? -0.02f
                        : 0.0f;
            }
            var total = baseChance + maleCount1 + ageMatters + happinessMatters + friendshipMatters + seasonMatters;
            if (_debug)
                this.Monitor.Log($"MatureMales: {maleCount}, Age: {animal.age.Value} | {ageMatters}, Happiness: {animal.happiness.Value} | {happinessMatters}, Friendship: {animal.friendshipTowardFarmer.Value} | {friendshipMatters}, Season: {seasonMatters}, BaseChance {baseChance}, Total {total}");
            return total;
        }
    }
}
