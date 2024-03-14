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

    public enum Cause
    {
        starvation,
        dehydration,
        oldAge,
        illness,
        wildAnimalAttack
    }

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

    //// planned features: healing item, display mood messages

    public class AnimalsDie : Mod
    {
        public readonly List<string> Messages = new();

        public readonly List<Tuple<FarmAnimal, string>> AnimalsToKill = new();

        public readonly List<FarmAnimal> SickAnimals = new();

        public readonly List<FarmAnimal> CheckedToday = new();

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

            Helper.Events.GameLoop.Saving += delegate { Serialize(); };
            Helper.Events.GameLoop.SaveLoaded += delegate { ResetVariables(); Deserialize(); };
            Helper.Events.GameLoop.ReturnedToTitle += delegate { ResetVariables(); AnimalsToKill.Clear(); };

            Helper.Events.GameLoop.DayStarted += delegate { OnDayStarted(); };
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

        public int CalculateIllness(FarmAnimal animal, int actualFullness, bool gotWater, bool wasLeftOutLastNight)
        {
            int addIllness = 0;
            StringBuilder potentialLog = new();

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
                    if (animal.home?.animalDoorOpen.Value == true)
                    {
                        // if it's winter it's too cold regardless of if there is a heater (no heater even grants one more point)
                        if (Game1.IsWinter)
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
                    else if (Game1.IsWinter && !HasHeater(animal))
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

            animal.modData.TryGetValue($"{ModManifest.UniqueID}/illness", out string moddata);

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
                VerboseLog($"{animal.Name} illness change, total: {illness}, new: {addIllness}, reasons: {potentialLog}");
            }

            animal.modData[$"{ModManifest.UniqueID}/illness"] = illness.ToString();

            return illness;
        }

        public int CalculateDehydration(FarmAnimal animal, bool gotWater)
        {
            animal.modData.TryGetValue($"{ModManifest.UniqueID}/dehydration", out string moddata);

            int dehydration = 0;

            if (!string.IsNullOrEmpty(moddata))
            {
                dehydration = int.Parse(moddata);
            }

            if (!gotWater)
            {
                dehydration++;
                VerboseLog($"{animal.Name} didn't get water, dehydration: {dehydration}");
            }
            else
            {
                dehydration = 0;
            }

            animal.modData[$"{ModManifest.UniqueID}/dehydration"] = dehydration.ToString();

            return dehydration;
        }

        public int CalculateStarvation(FarmAnimal animal, int actualFullness)
        {
            animal.modData.TryGetValue($"{ModManifest.UniqueID}/starvation", out string moddata);

            int starvation = 0;

            if (!string.IsNullOrEmpty(moddata))
            {
                starvation = int.Parse(moddata);
            }

            if (actualFullness < 30)
            {
                starvation++;
                VerboseLog($"{animal.Name} didn't get fed, fullness: {actualFullness}, starvation: {starvation}");
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

        private static bool WasColdOutside()
        {
            return Game1.wasRainingYesterday || Game1.IsWinter; ////Game1.isRaining || Game1.isSnowing || Game1.isLightning
        }

        private static bool HasHeater(FarmAnimal animal)
        {
            return animal.home?.GetIndoors()?.numberOfObjectsWithName("Heater") > 0;
        }

        private static double Map(double from, double fromMin, double fromMax, double toMin, double toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        private static AnimalType GetAnimalType(FarmAnimal animal)
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

        private void PostMessage(object o)
        {
            Messages.Add(o == null ? "null" : o.ToString());
        }

        private Tuple<int, int> GetMinAndMaxAnimalAgeInYears(FarmAnimal animal)
        {
            int barnPlaceHolderAge = 10;
            int coopPlaceHolderAge = 5;

            AnimalType type = GetAnimalType(animal);

            return type switch
            {
                AnimalType.Sheep => new Tuple<int, int>(Config.MinAgeSheep, Config.MaxAgeSheep),
                AnimalType.Cow => new Tuple<int, int>(Config.MinAgeCow, Config.MaxAgeCow),
                AnimalType.Goat => new Tuple<int, int>(Config.MinAgeGoat, Config.MaxAgeGoat),
                AnimalType.Pig => new Tuple<int, int>(Config.MinAgeSheep, Config.MaxAgeSheep),
                AnimalType.Ostrich => new Tuple<int, int>(Config.MinAgeOstrich, Config.MaxAgeOstrich),
                AnimalType.Chicken => new Tuple<int, int>(Config.MinAgeChicken, Config.MaxAgeChicken),
                AnimalType.Duck => new Tuple<int, int>(Config.MinAgeDuck, Config.MaxAgeDuck),
                AnimalType.Rabbit => new Tuple<int, int>(Config.MinAgeRabbit, Config.MaxAgeRabbit),
                AnimalType.Dinosaur => new Tuple<int, int>(Config.MinAgeDinosaur, Config.MaxAgeDinosaur),
                _ => IsCoopDweller(animal) ? new Tuple<int, int>(coopPlaceHolderAge, coopPlaceHolderAge) : new Tuple<int, int>(barnPlaceHolderAge, barnPlaceHolderAge),
            };
        }

        private static bool IsCoopDweller(FarmAnimal animal)
        {
            return animal.GetAnimalData()?.House == "Coop";
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

            if (animal.happiness.Value < 30)
            {
                happiness = Helper.Translation.Get("Happiness.sad");
            }
            else if (animal.happiness.Value < 200)
            {
                happiness = Helper.Translation.Get("Happiness.fine");
            }
            else
            {
                happiness = Helper.Translation.Get("Happiness.happy");
            }

            double hearts = animal.friendshipTowardFarmer.Value < 1000 ? animal.friendshipTowardFarmer.Value / 200.0 : 5;

            double withHalfHearts = ((int)(hearts * 2.0)) / 2.0;
            string loveString = Helper.Translation.Get(withHalfHearts == 1 ? "Love.heart" : "Love.hearts", new { heartCount = withHalfHearts });

            // if the locale is the default locale, keep the english animal type
            string animalType = string.IsNullOrWhiteSpace(Helper.Translation.Locale) ? animal.type.Value.ToLower() : animal.displayType;

            string message = Helper.Translation.Get("AnimalDeathMessage", new { animalType, animalName = animal.Name, cause = causeString, entireAgeText = ageString, happinessText = happiness, lovestring = loveString });

            PostMessage(message);
        }

        /// <summary>
        /// This is the way the game deletes animals when you sell them
        /// </summary>
        /// <param name="animal"></param>
        /// <param name="cause"></param>
        private void KillAnimal(FarmAnimal animal, string cause)
        {
            // one more check in case this happens with a re-kill and the user changed configs inbetween
            if (Enum.TryParse(cause, out Cause c))
            {
                switch (c)
                {
                    case Cause.starvation:
                        if (!Config.DeathByStarvation)
                        {
                            return;
                        }
                        break;

                    case Cause.dehydration:
                        if (!Config.DeathByDehydrationWithAnimalsNeedWaterMod)
                        {
                            return;
                        }
                        break;

                    case Cause.oldAge:
                        if (!Config.DeathByOldAge)
                        {
                            return;
                        }
                        break;

                    case Cause.illness:
                        if (!Config.DeathByIllness)
                        {
                            return;
                        }
                        break;
                }
            }

            VerboseLog($"Killed {animal.Name} due to {cause}");

            // right before this Utility.fixAllAnimals gets called, so if it's still not fixed then... it truly doesn't have a home and I don't need to remove it
            if (animal.home?.GetIndoors() is AnimalHouse animalHouse)
            {
                animalHouse.animalsThatLiveHere.Remove(animal.myID.Value);
                animalHouse.animals.Remove(animal.myID.Value);
            }

            Utility.ForEachLocation(delegate (GameLocation location)
            {
                location.animals.Remove(animal.myID.Value);

                return true;
            });

            animal.health.Value = -1;

            if (animal.foundGrass != null && FarmAnimal.reservedGrass.Contains(animal.foundGrass))
            {
                FarmAnimal.reservedGrass.Remove(animal.foundGrass);
            }

            CalculateDeathMessage(animal, cause);
        }

        private void ResetVariables()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            WildAnimalVictim = null;
            Messages.Clear();
            SickAnimals.Clear();
            CheckedToday.Clear();
            // don't reset animals to kill here!
        }

        private void Serialize()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            string output = string.Empty;

            if (AnimalsToKill.Count > 0)
            {
                output += $"{AnimalsToKill[0].Item1.myID.Value},{AnimalsToKill[0].Item2}";
            }

            for (int i = 1; i < AnimalsToKill.Count; i++)
            {
                output += $",{AnimalsToKill[i].Item1.myID.Value},{AnimalsToKill[i].Item2}";
            }

            if (Game1.getFarm().modData.ContainsKey($"{ModManifest.UniqueID}/animalsToKill"))
            {
                Game1.getFarm().modData[$"{ModManifest.UniqueID}/animalsToKill"] = output;
            }
            else
            {
                Game1.getFarm().modData.Add($"{ModManifest.UniqueID}/animalsToKill", output);
            }
        }

        private void Deserialize()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            AnimalsToKill.Clear();

            try
            {
                Game1.getFarm().modData.TryGetValue($"{ModManifest.UniqueID}/animalsToKill", out string input);

                if (!string.IsNullOrEmpty(input))
                {
                    Game1.getFarm().modData[$"{ModManifest.UniqueID}/animalsToKill"] = string.Empty;
                    DebugLog(input);

                    var list = input.Split(',');

                    for (int i = 0; i < list.Length; i += 2)
                    {
                        var id = long.Parse(list[i]);

                        AnimalsToKill.Add(new Tuple<FarmAnimal, string>(Utility.getAnimal(id), list[i + 1]));
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog("Couldn't parse animals to re-kill. There may be animals that died on the last day you played, but got \"revived\" because I can't kill them again now", e);
            }
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

            CheckedToday.Clear();
            KillAnimals();
            DisplayIllMessage();
        }

        private void KillAnimals()
        {
            if (WildAnimalVictim != null)
            {
                CalculateDeathMessage(WildAnimalVictim, Cause.wildAnimalAttack.ToString());
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
            if (!Config.IllnessMessages)
            {
                SickAnimals.Clear();
                return;
            }

            switch (SickAnimals.Count)
            {
                case 0:
                    return;

                case 1:
                    Game1.showGlobalMessage(Helper.Translation.Get("SickAnimalMessage.oneAnimal", new { firstAnimalName = SickAnimals[0].Name }));
                    break;

                case 2:
                    Game1.showGlobalMessage(Helper.Translation.Get("SickAnimalMessage.twoAnimals", new { firstAnimalName = SickAnimals[0].Name, secondAnimalName = SickAnimals[1].Name, }));
                    break;

                case 3:

                    Game1.showGlobalMessage(Helper.Translation.Get("SickAnimalMessage.threeAnimals", new { firstAnimalName = SickAnimals[0].Name, secondAnimalName = SickAnimals[1].Name, thirdAnimalName = SickAnimals[2].Name }));
                    break;

                case 4:

                    Game1.showGlobalMessage(Helper.Translation.Get("SickAnimalMessage.fourAnimals", new { firstAnimalName = SickAnimals[0].Name, secondAnimalName = SickAnimals[1].Name, thirdAnimalName = SickAnimals[2].Name, sickAnimalCount = SickAnimals.Count - 3 }));
                    break;

                default:
                    Game1.showGlobalMessage(Helper.Translation.Get("SickAnimalMessage.morethanfourAnimals", new { firstAnimalName = SickAnimals[0].Name, secondAnimalName = SickAnimals[1].Name, thirdAnimalName = SickAnimals[2].Name, sickAnimalCount = SickAnimals.Count - 3 }));
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