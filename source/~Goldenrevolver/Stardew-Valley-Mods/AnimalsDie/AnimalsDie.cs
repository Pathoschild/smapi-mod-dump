/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace AnimalsDie
{
    using StardewModdingAPI;
    using StardewValley;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public enum AnimalType
    {
        Sheep,
        Cow,
        Goat,
        Pig,
        Ostrich,
        Chicken,
        Duck,
        Rabbit,
        Dinosaur,
        Other
    }

    public interface IAnimalsNeedWaterAPI
    {
        List<string> GetCoopsWithWateredTrough();

        List<string> GetBarnsWithWateredTrough();

        List<long> GetAnimalsLeftThirstyYesterday();

        List<long> GetFullAnimals();

        bool WasAnimalLeftThirstyYesterday(FarmAnimal animal);

        bool IsAnimalFull(FarmAnimal animal);
    }

    //// planned features: healing item, display messages

    public class AnimalsDie : Mod
    {
        public readonly List<string> Messages = new List<string>();

        public readonly List<Tuple<FarmAnimal, string>> AnimalsToKill = new List<Tuple<FarmAnimal, string>>();

        public readonly List<FarmAnimal> SickAnimals = new List<FarmAnimal>();

        public FarmAnimal WildAnimalVictim { get; set; }

        public IAnimalsNeedWaterAPI WaterMod { get; set; }

        /// <summary>
        /// Gets or sets the current config file
        /// </summary>
        public AnimalsDieConfig Config { get; set; }

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<AnimalsDieConfig>();

            AnimalsDieConfig.VerifyConfigValues(Config, this);

            Helper.Events.GameLoop.GameLaunched += delegate { SetupWaterMod(); AnimalsDieConfig.SetUpModConfigMenu(Config, this, WaterMod != null); };

            Helper.Events.GameLoop.DayStarted += delegate { OnDayStarted(); };

            Helper.Events.GameLoop.SaveLoaded += delegate { ResetVariables(); };
            Helper.Events.GameLoop.ReturnedToTitle += delegate { ResetVariables(); };
            Helper.Events.GameLoop.UpdateTicked += delegate { TryToSendMessage(); };

            Patcher.PatchAll(this);
        }

        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }

        public void VerboseLog(object o)
        {
            if (this.Monitor.IsVerbose)
            {
                Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
            }
        }

        public int CalculateIllness(FarmAnimal animal, byte actualFullness, bool gotWater, bool wasLeftOutLastNight)
        {
            int addIllness = 0;
            StringBuilder potentialLog = new StringBuilder();

            // was trapped outdoors overnight
            if (wasLeftOutLastNight)
            {
                addIllness++;
                potentialLog.Append("leftOutside ");
                if (WasColdOutside())
                {
                    addIllness++;
                    potentialLog.Append("alsoCold ");
                }
            }
            else
            {
                // not trapped outside: if it's cold weather
                if (WasColdOutside())
                {
                    // and the door was left open
                    if (animal.home.animalDoorOpen)
                    {
                        // if it's winter it's too cold regardless of if there is a heater (no heater even grants one more point)
                        if (IsWinter())
                        {
                            addIllness++;
                            potentialLog.Append("openDoorWinter ");
                        }

                        // if it's just cold then a heater is enough to prevent damage
                        if (!HasHeater(animal))
                        {
                            addIllness++;
                            potentialLog.Append("openDoorNoHeater ");
                        }
                    }
                    else if (IsWinter() && !HasHeater(animal))
                    {
                        addIllness++;
                        potentialLog.Append("WinterNoHeater ");
                    }
                }
            }

            // an animal died to a wild animal attack
            if (WildAnimalVictim != null)
            {
                addIllness++;
                potentialLog.Append("wildAnimalAttackHappened ");
            }

            // animal was not fed (also works if it was outside and found nothing to eat)
            // if this fails there will be a water check, so a maximum of one illness point if not fed and no water
            if (actualFullness < 30 && Config.DeathByStarvation)
            {
                addIllness++;
                potentialLog.Append("notFed ");
            }
            else if (WaterMod != null && Config.DeathByDehydrationWithAnimalsNeedWaterMod && !gotWater)
            {
                addIllness++;
                potentialLog.Append("noWater ");
            }

            string moddata;
            animal.modData.TryGetValue($"{ModManifest.UniqueID}/illness", out moddata);

            int illness = addIllness;

            if (!string.IsNullOrEmpty(moddata))
            {
                illness += int.Parse(moddata);
            }

            // taking care of your animal always reduces illness
            if (illness > 0 && (addIllness == 0 || animal.wasPet.Value))
            {
                addIllness--;
                illness--;
                potentialLog.Append("healed");
            }

            if (!string.IsNullOrEmpty(potentialLog.ToString()))
            {
                VerboseLog($"{animal.name} illness change, total: {illness}, new: {addIllness}, reasons: {potentialLog}");
            }

            animal.modData[$"{ModManifest.UniqueID}/illness"] = illness.ToString();

            return illness;
        }

        public int CalculateDehydration(FarmAnimal animal, bool gotWater)
        {
            string moddata;
            animal.modData.TryGetValue($"{ModManifest.UniqueID}/dehydration", out moddata);

            int dehydration = 0;

            if (!string.IsNullOrEmpty(moddata))
            {
                dehydration = int.Parse(moddata);
            }

            if (!gotWater)
            {
                dehydration++;
                VerboseLog($"{animal.name} didn't get water, dehydration: {dehydration}");
            }
            else
            {
                dehydration = 0;
            }

            animal.modData[$"{ModManifest.UniqueID}/dehydration"] = dehydration.ToString();

            return dehydration;
        }

        public int CalculateStarvation(FarmAnimal animal, byte actualFullness)
        {
            string moddata;
            animal.modData.TryGetValue($"{ModManifest.UniqueID}/starvation", out moddata);

            int starvation = 0;

            if (!string.IsNullOrEmpty(moddata))
            {
                starvation = int.Parse(moddata);
            }

            if (actualFullness < 30)
            {
                starvation++;
                VerboseLog($"{animal.name} didn't get fed, fullness: {actualFullness}, starvation: {starvation}");
            }
            else
            {
                starvation = 0;
            }

            animal.modData[$"{ModManifest.UniqueID}/starvation"] = starvation.ToString();

            return starvation;
        }

        public bool ShouldDieOfOldAge(FarmAnimal animal)
        {
            int age = animal.GetDaysOwned();

            var ages = GetMinAndMaxAnimalAgeInYears(animal);

            // convert years to days
            int minAge = ages.Item1 * 28 * 4;
            int maxAge = ages.Item2 * 28 * 4;

            if (age >= minAge)
            {
                double ran = Game1.random.NextDouble();

                double mappedValue = Map(age, minAge, maxAge, 0, 1);

                if (ran < mappedValue)
                {
                    return true;
                }
            }

            return false;
        }

        private void PostMessage(object o)
        {
            Messages.Add(o == null ? "null" : o.ToString());
        }

        private bool IsWinter()
        {
            return Game1.currentSeason.Equals("winter");
        }

        private bool WasColdOutside()
        {
            return Game1.wasRainingYesterday || IsWinter(); ////Game1.isRaining || Game1.isSnowing || Game1.isLightning
        }

        private bool HasHeater(FarmAnimal animal)
        {
            return animal.home.indoors.Value.numberOfObjectsWithName("Heater") > 0;
        }

        private double Map(double from, double fromMin, double fromMax, double toMin, double toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        private Tuple<int, int> GetMinAndMaxAnimalAgeInYears(FarmAnimal animal)
        {
            int barnPlaceHolderAge = 10;
            int coopPlaceHolderAge = 5;

            AnimalType type = GetAnimalType(animal);

            switch (type)
            {
                case AnimalType.Sheep:
                    return new Tuple<int, int>(Config.MinAgeSheep, Config.MaxAgeSheep);

                case AnimalType.Cow:
                    return new Tuple<int, int>(Config.MinAgeCow, Config.MaxAgeCow);

                case AnimalType.Goat:
                    return new Tuple<int, int>(Config.MinAgeGoat, Config.MaxAgeGoat);

                case AnimalType.Pig:
                    return new Tuple<int, int>(Config.MinAgeSheep, Config.MaxAgeSheep);

                case AnimalType.Ostrich:
                    return new Tuple<int, int>(Config.MinAgeOstrich, Config.MaxAgeOstrich);

                case AnimalType.Chicken:
                    return new Tuple<int, int>(Config.MinAgeChicken, Config.MaxAgeChicken);

                case AnimalType.Duck:
                    return new Tuple<int, int>(Config.MinAgeDuck, Config.MaxAgeDuck);

                case AnimalType.Rabbit:
                    return new Tuple<int, int>(Config.MinAgeRabbit, Config.MaxAgeRabbit);

                case AnimalType.Dinosaur:
                    return new Tuple<int, int>(Config.MinAgeDinosaur, Config.MaxAgeDinosaur);

                default:
                    return animal.isCoopDweller() ? new Tuple<int, int>(coopPlaceHolderAge, coopPlaceHolderAge) : new Tuple<int, int>(barnPlaceHolderAge, barnPlaceHolderAge);
            }
        }

        private AnimalType GetAnimalType(FarmAnimal animal)
        {
            var animalTypes = Enum.GetNames(typeof(AnimalType));

            foreach (var animalType in animalTypes)
            {
                if (animal.type.Contains(animalType))
                {
                    // can't fail because it would not be in Enum.GetNames otherwise
                    return (AnimalType)Enum.Parse(typeof(AnimalType), animalType);
                }
            }

            return AnimalType.Other;
        }

        private void CalculateDeathMessage(FarmAnimal animal, string cause)
        {
            // animal.age is the value that doesn't increase if you haven't fed the animal
            int age = animal.GetDaysOwned() + 1;
            string causeString = Helper.Translation.Get($"Cause.{cause}");

            string ageString;

            if (age >= 28 * 4)
            {
                int yearCount = (age / (28 * 4)) + 1;
                ageString = Helper.Translation.Get(yearCount == 1 ? "Age.year" : "Age.years", new { yearCount });

                int restAge = age - (yearCount * 28 * 4);
                if (restAge >= 28)
                {
                    int monthCount = (restAge / 28) + 1;

                    if (yearCount == 1)
                    {
                        ageString = Helper.Translation.Get(monthCount == 1 ? "Age.yearAndMonth" : "Age.yearAndMonths", new { yearCount, monthCount });
                    }
                    else
                    {
                        ageString = Helper.Translation.Get(monthCount == 1 ? "Age.yearsAndMonth" : "Age.yearsAndMonths", new { yearCount, monthCount });
                    }
                }
            }
            else
            {
                if (age >= 28)
                {
                    int monthCount = (age / 28) + 1;
                    ageString = Helper.Translation.Get(monthCount == 1 ? "Age.month" : "Age.months", new { monthCount });
                }
                else
                {
                    ageString = Helper.Translation.Get(age == 1 ? "Age.day" : "Age.days", new { dayCount = age });
                }
            }

            string happiness;

            if (animal.happiness < 30)
            {
                happiness = Helper.Translation.Get("Happiness.sad");
            }
            else if (animal.happiness < 200)
            {
                happiness = Helper.Translation.Get("Happiness.fine");
            }
            else
            {
                happiness = Helper.Translation.Get("Happiness.happy");
            }

            double hearts = animal.friendshipTowardFarmer < 1000 ? animal.friendshipTowardFarmer / 200.0 : 5;

            double withHalfHearts = ((int)(hearts * 2.0)) / 2.0;
            string loveString = Helper.Translation.Get(withHalfHearts == 1 ? "Love.heart" : "Love.hearts", new { heartCount = withHalfHearts });

            // if the locale is the default locale, keep the english animal type
            string animalType = string.IsNullOrWhiteSpace(Helper.Translation.Locale) ? animal.type.Value.ToLower() : animal.displayType;

            string message = Helper.Translation.Get("AnimalDeathMessage", new { animalType, animalName = animal.name, cause = causeString, entireAgeText = ageString, happinessText = happiness, lovestring = loveString });

            PostMessage(message);
        }

        /// <summary>
        /// This is the way the game deletes animals when you sell them
        /// </summary>
        /// <param name="animal"></param>
        /// <param name="cause"></param>
        private void KillAnimal(FarmAnimal animal, string cause)
        {
            VerboseLog($"Killed {animal.name} due to {cause}");

            // right before this Utility.fixAllAnimals gets called, so if it's still not fixed then... it truly doesn't have a home and I don't need to remove it
            if (animal.home != null)
            {
                (animal.home.indoors.Value as AnimalHouse).animalsThatLiveHere.Remove(animal.myID);
                (animal.home.indoors.Value as AnimalHouse).animals.Remove(animal.myID);
            }

            Game1.getFarm().animals.Remove(animal.myID);

            animal.health.Value = -1;

            if (animal.foundGrass != null && FarmAnimal.reservedGrass.Contains(animal.foundGrass))
            {
                FarmAnimal.reservedGrass.Remove(animal.foundGrass);
            }

            CalculateDeathMessage(animal, cause);
        }

        private void ResetVariables()
        {
            WildAnimalVictim = null;
            Messages.Clear();
            AnimalsToKill.Clear();
            SickAnimals.Clear();
        }

        private void SetupWaterMod()
        {
            // the null check is somewhere else
            WaterMod = Helper.ModRegistry.GetApi<IAnimalsNeedWaterAPI>("GZhynko.AnimalsNeedWater");
        }

        private void OnDayStarted()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            CheckHomeStatus();
            KillAnimals();
            DisplayIllMessage();
        }

        private void CheckHomeStatus()
        {
            foreach (var animal in Game1.getFarm().getAllFarmAnimals())
            {
                if (animal.home == null)
                {
                    Utility.fixAllAnimals();
                    DebugLog("Fixed at least one animal from the base game animal home bug");
                    break;
                }
            }
        }

        private void KillAnimals()
        {
            if (WildAnimalVictim != null)
            {
                CalculateDeathMessage(WildAnimalVictim, "wildAnimalAttack");
                WildAnimalVictim = null;
            }

            foreach (var item in AnimalsToKill)
            {
                if (item.Item1 != null)
                {
                    KillAnimal(item.Item1, item.Item2);
                }
            }

            AnimalsToKill.Clear();
        }

        private void DisplayIllMessage()
        {
            switch (SickAnimals.Count)
            {
                case 0:
                    return;

                case 1:
                    Game1.showGlobalMessage(Helper.Translation.Get("SickAnimalMessage.oneAnimal", new { firstAnimalName = SickAnimals[0].name }));
                    break;

                case 2:
                    Game1.showGlobalMessage(Helper.Translation.Get("SickAnimalMessage.twoAnimals", new { firstAnimalName = SickAnimals[0].name, secondAnimalName = SickAnimals[1].name, }));
                    break;

                case 3:

                    Game1.showGlobalMessage(Helper.Translation.Get("SickAnimalMessage.threeAnimals", new { firstAnimalName = SickAnimals[0].name, secondAnimalName = SickAnimals[1].name, thirdAnimalName = SickAnimals[2].name }));
                    break;

                case 4:

                    Game1.showGlobalMessage(Helper.Translation.Get("SickAnimalMessage.fourAnimals", new { firstAnimalName = SickAnimals[0].name, secondAnimalName = SickAnimals[1].name, thirdAnimalName = SickAnimals[2].name, sickAnimalCount = SickAnimals.Count - 3 }));
                    break;

                default:
                    Game1.showGlobalMessage(Helper.Translation.Get("SickAnimalMessage.morethanfourAnimals", new { firstAnimalName = SickAnimals[0].name, secondAnimalName = SickAnimals[1].name, thirdAnimalName = SickAnimals[2].name, sickAnimalCount = SickAnimals.Count - 3 }));
                    break;
            }

            SickAnimals.Clear();
        }

        private void TryToSendMessage()
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer)
            {
                return;
            }

            if (Messages.Count > 0 && Game1.activeClickableMenu == null)
            {
                Game1.drawObjectDialogue(Messages[0]);
                Messages.RemoveAt(0);
            }
        }
    }
}