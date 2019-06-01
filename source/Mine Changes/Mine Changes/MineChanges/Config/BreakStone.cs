using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace Mine_Changes.MineChanges.Config
{
    public class BreakStone
    {
        public int stoneIndex = -1000;
        public string stoneName = "";
        public List<OreChances> oreChances = new List<OreChances>();

        public int getStoneIndex()
        {
            if (stoneIndex <= -1000)
            {
                if (stoneName == "")
                {
                    return -1;
                }
                stoneIndex = Mod.getObjectIDFromAsset(stoneName);
                if (stoneIndex <= -1000)
                    return -1;
            }
            return stoneIndex;
        }
    }

    public class OreChances
    {
        public int oreIndex = -1000;
        public String oreName = "";
        public double oreChance = 0.0f;
        public int minOre = 1;
        public int maxOre = 1;
        public List<GroupOfChanges> prioritizedListOfChanges = new List<GroupOfChanges>();
        public List<ProfessionChanges> professionChanges = new List<ProfessionChanges>();
        public List<SkillChanges> skillChanges = new List<SkillChanges>();
        public List<TimeChanges> timeChanges = new List<TimeChanges>();
        public List<WeatherChanges> weatherChanges = new List<WeatherChanges>();

        public int getOreIndex()
        {
            if(oreIndex <= -1000)
            {
                if(oreName == "")
                {
                    return -1;
                }
                oreIndex = Mod.getObjectIDFromAsset(oreName);
                if (oreIndex <= -1000)
                    return -1;
            }
            if(oreIndex < 0)
            {
                //TODO: add category support
            }
            return oreIndex;
        }

        public double applyChances()
        {
            return applyChances(Game1.player);
        }

        public double applyChances(Farmer who)
        {
            double chance = this.oreChance;

            if (prioritizedListOfChanges != null && prioritizedListOfChanges.Count > 0)
            {
                foreach (GroupOfChanges gr in prioritizedListOfChanges)
                {
                    gr.applyChances(who, ref chance);
                }
            }

            if (professionChanges != null && professionChanges.Count > 0)
            {
                foreach (ProfessionChanges pr in professionChanges)
                {
                    pr.applyChances(who, ref chance);
                }
            }
            if (skillChanges != null && skillChanges.Count > 0)
            {
                foreach (SkillChanges pr in skillChanges)
                {
                    pr.applyChances(who, ref chance);
                }
            }
            if (timeChanges != null && timeChanges.Count > 0)
            {
                foreach (TimeChanges pr in timeChanges)
                {
                    pr.applyChances(who, ref chance);
                }
            }
            if (weatherChanges != null && weatherChanges.Count > 0)
            {
                foreach (WeatherChanges pr in weatherChanges)
                {
                    pr.applyChances(who, ref chance);
                }
            }

            return chance;
        }

        public int getCount()
        {
            return getCount(Game1.random, Game1.player);
        }

        public int getCount(Random r, Farmer who)
        {
            double min = minOre;
            double max = maxOre;

            if (prioritizedListOfChanges != null && prioritizedListOfChanges.Count > 0)
            {
                foreach (GroupOfChanges gr in prioritizedListOfChanges)
                {
                    gr.applyCount(who, ref min, ref max);
                }
            }

            if (professionChanges != null && professionChanges.Count > 0)
            {
                foreach (ProfessionChanges pr in professionChanges)
                {
                    pr.applyCount(who, ref min, ref max);
                }
            }
            if (skillChanges != null && skillChanges.Count > 0)
            {
                foreach (SkillChanges pr in skillChanges)
                {
                    pr.applyCount(who, ref min, ref max);
                }
            }
            if (timeChanges != null && timeChanges.Count > 0)
            {
                foreach (TimeChanges pr in timeChanges)
                {
                    pr.applyCount(who, ref min, ref max);
                }
            }
            if (weatherChanges != null && weatherChanges.Count > 0)
            {
                foreach (WeatherChanges pr in weatherChanges)
                {
                    pr.applyCount(who, ref min, ref max);
                }
            }

            if (min > max)
            {
                min = max;
            }

            return (int)Math.Round(r.NextDouble() * (max - min) + min);
        }
    }

    public class GroupOfChanges
    {
        public List<ProfessionChanges> professionChanges = new List<ProfessionChanges>();
        public List<SkillChanges> skillChanges = new List<SkillChanges>();
        public List<TimeChanges> timeChanges = new List<TimeChanges>();
        public List<WeatherChanges> weatherChanges = new List<WeatherChanges>();

        public double applyChances(Farmer who, ref double chance)
        {
            if (professionChanges != null && professionChanges.Count > 0)
            {
                foreach (ProfessionChanges pr in professionChanges)
                {
                    pr.applyChances(who, ref chance);
                }
            }
            if (skillChanges != null && skillChanges.Count > 0)
            {
                foreach (SkillChanges pr in skillChanges)
                {
                    pr.applyChances(who, ref chance);
                }
            }
            if (timeChanges != null && timeChanges.Count > 0)
            {
                foreach (TimeChanges pr in timeChanges)
                {
                    pr.applyChances(who, ref chance);
                }
            }
            if (weatherChanges != null && weatherChanges.Count > 0)
            {
                foreach (WeatherChanges pr in weatherChanges)
                {
                    pr.applyChances(who, ref chance);
                }
            }

            return chance;
        }

        public void applyCount(Farmer who, ref double min, ref double max)
        {

            if (professionChanges != null && professionChanges.Count > 0)
            {
                foreach (ProfessionChanges pr in professionChanges)
                {
                    pr.applyCount(who, ref min, ref max);
                }
            }
            if (skillChanges != null && skillChanges.Count > 0)
            {
                foreach (SkillChanges pr in skillChanges)
                {
                    pr.applyCount(who, ref min, ref max);
                }
            }
            if (timeChanges != null && timeChanges.Count > 0)
            {
                foreach (TimeChanges pr in timeChanges)
                {
                    pr.applyCount(who, ref min, ref max);
                }
            }
            if (weatherChanges != null && weatherChanges.Count > 0)
            {
                foreach (WeatherChanges pr in weatherChanges)
                {
                    pr.applyCount(who, ref min, ref max);
                }
            }
        }
    }
    public class ProfessionChanges
    {
        public int profession = -1;
        public bool affectsCount = false;
        public double professionChange = 0.0f;
        public string professionOperation = "*";

        public void applyChances(Farmer who, ref double seedChance)
        {
            if (profession <= -1)
                return;

            double ans = seedChance;

            if (!affectsCount && who.professions.Contains(profession))
            {
                switch (professionOperation)
                {
                    case "+":
                        ans += professionChange;
                        break;
                    case "-":
                        ans -= professionChange;
                        break;
                    case "*":
                        ans *= professionChange;
                        break;
                    case "/":
                        ans /= professionChange;
                        break;
                    default:
                        break;
                }
            }
            seedChance = ans;
            return;
        }

        public void applyCount(Farmer who, ref double minSeed, ref double maxSeed)
        {
            if (profession <= -1)
                return;

            double min = minSeed;
            double max = maxSeed;

            if (affectsCount && who.professions.Contains(profession))
            {
                switch (professionOperation)
                {
                    case "+":
                        max += professionChange;
                        min += professionChange;
                        break;
                    case "-":
                        max -= professionChange;
                        min -= professionChange;
                        break;
                    case "*":
                        max *= professionChange;
                        min *= professionChange;
                        break;
                    case "/":
                        max /= professionChange;
                        min /= professionChange;
                        break;
                    case "min+":
                        min += professionChange;
                        break;
                    case "min-":
                        min -= professionChange;
                        break;
                    case "min*":
                        min *= professionChange;
                        break;
                    case "min/":
                        min /= professionChange;
                        break;
                    case "max+":
                        max += professionChange;
                        break;
                    case "max-":
                        max -= professionChange;
                        break;
                    case "max*":
                        max *= professionChange;
                        break;
                    case "max/":
                        max /= professionChange;
                        break;
                    default:
                        break;
                }
            }
            minSeed = min;
            maxSeed = max;
            return;
        }
    }

    public class SkillChanges
    {
        public string skill = "";
        public int minSkillLevel = -1;
        public int maxSkillLevel = -1;
        public bool affectsCount = false;
        public double change = 0.0f;
        public string operation = "*";

        public int getFarmerSkillLevel(Farmer who)
        {
            return Mod.getSkillLevel(who, skill);
        }

        public void applyChances(Farmer who, ref double seedChance)
        {
            int farmerSkillLevel = 0;
            if (skill != "")
            {
                farmerSkillLevel = getFarmerSkillLevel(who);
                if (farmerSkillLevel <= -1)
                    return;
            }

            double ans = seedChance;
            bool anySkillLevel = minSkillLevel <= -1 && maxSkillLevel <= -1;
            if (!affectsCount && (anySkillLevel ||
                ((minSkillLevel <= farmerSkillLevel) &&
                (maxSkillLevel >= farmerSkillLevel || maxSkillLevel <= -1))))
            {
                switch (operation)
                {
                    case "+":
                        ans += change;
                        break;
                    case "-":
                        ans -= change;
                        break;
                    case "*":
                        ans *= change;
                        break;
                    case "/":
                        ans /= change;
                        break;
                    default:
                        break;
                }
            }
            seedChance = ans;
            return;
        }

        public void applyCount(Farmer who, ref double minSeed, ref double maxSeed)
        {
            int farmerSkillLevel = 0;
            if (skill != "")
            {
                farmerSkillLevel = getFarmerSkillLevel(who);
                if (farmerSkillLevel <= -1)
                    return;
            }

            double min = minSeed;
            double max = maxSeed;

            bool anySkillLevel = minSkillLevel <= -1 && maxSkillLevel <= -1;
            if (!affectsCount && (anySkillLevel ||
                ((minSkillLevel <= farmerSkillLevel) &&
                (maxSkillLevel >= farmerSkillLevel || maxSkillLevel <= -1))))
            {
                switch (operation)
                {
                    case "+":
                        max += change;
                        min += change;
                        break;
                    case "-":
                        max -= change;
                        min -= change;
                        break;
                    case "*":
                        max *= change;
                        min *= change;
                        break;
                    case "/":
                        max /= change;
                        min /= change;
                        break;
                    case "min+":
                        min += change;
                        break;
                    case "min-":
                        min -= change;
                        break;
                    case "min*":
                        min *= change;
                        break;
                    case "min/":
                        min /= change;
                        break;
                    case "max+":
                        max += change;
                        break;
                    case "max-":
                        max -= change;
                        break;
                    case "max*":
                        max *= change;
                        break;
                    case "max/":
                        max /= change;
                        break;
                    default:
                        break;
                }
            }
            minSeed = min;
            maxSeed = max;
            return;
        }
    }

    public class TimeChanges
    {
        public int minMinute = -1;
        public int maxMinute = -1;
        public int minDay = -1;
        public int maxDay = -1;
        public int minYear = -1;
        public int maxYear = -1;
        public bool affectsCount = false;
        public double change = 0.0f;
        public string operation = "*";

        public void applyChances(Farmer who, ref double seedChance)
        {
            int currentYear = Game1.year;
            int currentDay = (Game1.dayOfMonth-1) + (Game1.IsSpring ? 0 : Game1.IsSummer ? 28: Game1.IsFall ? 56 : 84);
            int currentMinute = Game1.timeOfDay;

            bool ignoreYear = minYear == -1 && maxYear == -1;
            bool ignoreDay = minDay == -1 && maxDay == -1;
            bool ignoreMinute = minMinute == -1 && maxMinute == -1;

            if (ignoreYear || (currentYear >= minYear && (maxYear == -1 || currentYear <= maxYear)))
            {
                if (ignoreDay || (currentDay >= minDay && (maxDay == -1 || currentDay <= maxDay)))
                {
                    if (ignoreMinute || (currentMinute >= minMinute && (maxMinute == -1 || currentMinute <= maxDay)))
                    {
                        double ans = seedChance;

                        if (!affectsCount)
                        {
                            switch (operation)
                            {
                                case "+":
                                    ans += change;
                                    break;
                                case "-":
                                    ans -= change;
                                    break;
                                case "*":
                                    ans *= change;
                                    break;
                                case "/":
                                    ans /= change;
                                    break;
                                default:
                                    break;
                            }
                        }
                        seedChance = ans;
                        return;
                    }
                }
            }

        }

        public void applyCount(Farmer who, ref double minSeed, ref double maxSeed)
        {
            int currentYear = Game1.year;
            int currentDay = Game1.dayOfMonth + (Game1.IsSpring ? 0 : Game1.IsSummer ? 28 : Game1.IsFall ? 56 : 84);
            int currentMinute = Game1.timeOfDay;

            bool ignoreYear = minYear == -1 && maxYear == -1;
            bool ignoreDay = minDay == -1 && maxDay == -1;
            bool ignoreMinute = minMinute == -1 && maxMinute == -1;

            if (ignoreYear || (currentYear >= minYear && (maxYear == -1 || currentYear <= maxYear)))
            {
                if (ignoreDay || (currentDay >= minDay && (maxDay == -1 || currentDay <= maxDay)))
                {
                    if (ignoreMinute || (currentMinute >= minMinute && (maxMinute == -1 || currentMinute <= maxDay)))
                    {
                        double min = minSeed;
                        double max = maxSeed;

                        if (affectsCount)
                        {
                            switch (operation)
                            {
                                case "+":
                                    max += change;
                                    min += change;
                                    break;
                                case "-":
                                    max -= change;
                                    min -= change;
                                    break;
                                case "*":
                                    max *= change;
                                    min *= change;
                                    break;
                                case "/":
                                    max /= change;
                                    min /= change;
                                    break;
                                case "min+":
                                    min += change;
                                    break;
                                case "min-":
                                    min -= change;
                                    break;
                                case "min*":
                                    min *= change;
                                    break;
                                case "min/":
                                    min /= change;
                                    break;
                                case "max+":
                                    max += change;
                                    break;
                                case "max-":
                                    max -= change;
                                    break;
                                case "max*":
                                    max *= change;
                                    break;
                                case "max/":
                                    max /= change;
                                    break;
                                default:
                                    break;
                            }
                        }
                        minSeed = min;
                        maxSeed = max;
                    }
                }
            }
            return;
        }
    }

    public class WeatherChanges
    {
        public string weather = "";
        public bool tomorrow = false;
        public bool affectsCount = false;
        public double change = 0.0f;
        public string operation = "*";

        public bool isTargetWeather()
        {
            switch (weather)
            {
                case "":
                    return true;
                case "0":
                case "wedding":
                case "Wedding":
                    return Game1.weatherIcon == 0;
                case "1":
                case "festival":
                case "Festival":
                    return Game1.weatherIcon == 1;
                case "2":
                case "sunny":
                case "Sunny":
                    return Game1.weatherIcon == 2;
                case "3":
                case "spring_windy":
                case "Spring_windy":
                case "spring_Windy":
                case "Spring_Windy":
                case "springWindy":
                case "SpringWindy":
                    return Game1.weatherIcon == 3;
                case "4":
                case "rain":
                case "Rain":
                case "Raining":
                case "raining":
                    return Game1.weatherIcon == 4;
                case "5":
                case "storm":
                case "Storm":
                case "stormy":
                case "Stormy":
                case "lightning":
                case "Lightning":
                    return Game1.weatherIcon == 5;
                case "6":
                case "fall_windy":
                case "Fall_windy":
                case "fall_Windy":
                case "Fall_Windy":
                case "fallWindy":
                case "FallWindy":
                    return Game1.weatherIcon == 6;
                case "7":
                case "Snow":
                case "snow":
                case "Snowy":
                case "snowy":
                case "Snowing":
                case "snowing":
                    return Game1.weatherIcon == 7;
                case "windy":
                case "Windy":
                    return Game1.weatherIcon == 3 || Game1.weatherIcon == 6;
                default: return false;
            }
        }

        public bool isTomorowWeather()
        {
            switch (weather)
            {
                case "":
                    return true;
                case "6":
                case "wedding":
                case "Wedding":
                    return Game1.weatherForTomorrow == 6;
                case "4":
                case "festival":
                case "Festival":
                    return Game1.weatherForTomorrow == 4;
                case "0":
                case "sunny":
                case "Sunny":
                    return Game1.weatherForTomorrow == 0;
                case "spring_windy":
                case "Spring_windy":
                case "spring_Windy":
                case "Spring_Windy":
                case "springWindy":
                case "SpringWindy":
                    return Game1.weatherForTomorrow == 2 && Game1.IsSpring;
                case "1":
                case "rain":
                case "Rain":
                case "Raining":
                case "raining":
                    return Game1.weatherForTomorrow == 1;
                case "3":
                case "storm":
                case "Storm":
                case "stormy":
                case "Stormy":
                case "lightning":
                case "Lightning":
                    return Game1.weatherForTomorrow == 3;
                case "fall_windy":
                case "Fall_windy":
                case "fall_Windy":
                case "Fall_Windy":
                case "fallWindy":
                case "FallWindy":
                    return Game1.weatherForTomorrow == 2 && Game1.IsFall;
                case "5":
                case "Snow":
                case "snow":
                case "Snowy":
                case "snowy":
                case "Snowing":
                case "snowing":
                    return Game1.weatherForTomorrow == 5 || (Game1.weatherForTomorrow == 2 && Game1.IsWinter);
                case "2":
                case "windy":
                case "Windy":
                    return Game1.weatherForTomorrow == 2;
                default: return false;
            }
        }

        public void applyChances(Farmer who, ref double seedChance)
        {
            if (!affectsCount && ((tomorrow && isTomorowWeather()) || (!tomorrow && isTargetWeather())))
            {
                double ans = seedChance;
                switch (operation)
                {
                    case "+":
                        ans += change;
                        break;
                    case "-":
                        ans -= change;
                        break;
                    case "*":
                        ans *= change;
                        break;
                    case "/":
                        ans /= change;
                        break;
                    default:
                        break;
                }
                seedChance = ans;
            }
            return;
        }

        public void applyCount(Farmer who, ref double minSeed, ref double maxSeed)
        {
            if (affectsCount && ((tomorrow && isTomorowWeather()) || (!tomorrow && isTargetWeather())))
            {
                double min = minSeed;
                double max = maxSeed;

                switch (operation)
                {
                    case "+":
                        max += change;
                        min += change;
                        break;
                    case "-":
                        max -= change;
                        min -= change;
                        break;
                    case "*":
                        max *= change;
                        min *= change;
                        break;
                    case "/":
                        max /= change;
                        min /= change;
                        break;
                    case "min+":
                        min += change;
                        break;
                    case "min-":
                        min -= change;
                        break;
                    case "min*":
                        min *= change;
                        break;
                    case "min/":
                        min /= change;
                        break;
                    case "max+":
                        max += change;
                        break;
                    case "max-":
                        max -= change;
                        break;
                    case "max*":
                        max *= change;
                        break;
                    case "max/":
                        max /= change;
                        break;
                    default:
                        break;
                }
                minSeed = min;
                maxSeed = max;
            }
            return;
        }
    }
}
