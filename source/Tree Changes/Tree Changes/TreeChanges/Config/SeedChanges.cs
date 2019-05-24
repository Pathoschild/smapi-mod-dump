using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace TreeChanges.TreeChanges.Config
{
public class SeedChanges
{
    public int seedIndex = -1000;
    public bool replaceOldSeed = false;
    public List<SeedChances> seedChances = new List<SeedChances>();
}
public class SeedChances
{
        public int seedIndex = -1000;
        public string seedName = "";
        public double seedChance = 0.0f;
        public int minSeed = 1;
        public int maxSeed = 1;
        public List<ProfessionChanges> professionChanges;

        public int getSeedIndex()
        {
            if (seedIndex <= -1000)
            {
                if (seedName == "")
                {
                    return -1;
                }
                seedIndex = Mod.getObjectIDFromAsset(seedName);
                if (seedIndex <= -1000)
                    return -1;
            }
            if (seedIndex < 0)
            {
                //TODO: add category support
            }
            return seedIndex;
        }

        public double applyProfessionChances(GameLocation locale)
        {
            if (locale == null || professionChanges == null || professionChanges.Count == 0)
                return seedChance;

            double chance = seedChance;
            foreach(Farmer who in locale.farmers)
            {
                chance = applyProfessionChances(who, chance);
            }
            return chance;
        }

        public double applyProfessionChances(Farmer who)
        {
            return applyProfessionChances(who, seedChance);
        }

        public double applyProfessionChances(Farmer who, double seedChance)
        {
            double ans = seedChance;
            if (professionChanges == null || professionChanges.Count == 0)
                return ans;

            foreach (ProfessionChanges p in professionChanges)
            {
                if (!p.affectsCount && who.professions.Contains(p.profession))
                {
                    switch (p.professionOperation)
                    {
                        case "+":
                            ans += p.professionChange;
                            break;
                        case "-":
                            ans -= p.professionChange;
                            break;
                        case "*":
                            ans *= p.professionChange;
                            break;
                        case "/":
                            ans /= p.professionChange;
                            break;
                        default:
                            break;
                    }
                }
            }

            return ans;
        }

        public int applyProfessionCount(Farmer who)
        {
            return applyProfessionCount(Game1.random, who, minSeed, maxSeed);
        }

        public int applyProfessionCount(Random r, Farmer who, double minSeed, double maxSeed)
        {
            double min = minSeed;
            double max = maxSeed;
            if (professionChanges == null || professionChanges.Count == 0)
            {

                if (max < min)
                    max = min;
                if (min > max)
                    min = max;

                return (int)Math.Round(r.NextDouble() * (max - min) + min);
            }

            foreach (ProfessionChanges p in professionChanges)
            {
                if (p.affectsCount && who.professions.Contains(p.profession))
                {
                    switch (p.professionOperation)
                    {
                        case "+":
                            max += p.professionChange;
                            min += p.professionChange;
                            break;
                        case "-":
                            max -= p.professionChange;
                            min -= p.professionChange;
                            break;
                        case "*":
                            max *= p.professionChange;
                            min *= p.professionChange;
                            break;
                        case "/":
                            max /= p.professionChange;
                            min /= p.professionChange;
                            break;
                        case "min+":
                            min += p.professionChange;
                            break;
                        case "min-":
                            min -= p.professionChange;
                            break;
                        case "min*":
                            min *= p.professionChange;
                            break;
                        case "min/":
                            min /= p.professionChange;
                            break;
                        case "max+":
                            max += p.professionChange;
                            break;
                        case "max-":
                            max -= p.professionChange;
                            break;
                        case "max*":
                            max *= p.professionChange;
                            break;
                        case "max/":
                            max /= p.professionChange;
                            break;
                        default:
                            break;
                    }
                }
            }
            if (max < min)
                max = min;
            if (min > max)
                min = max;
            return (int)Math.Round(r.NextDouble() * (max - min) + min);
        }
    }

    public class ProfessionChanges
    {
        public int profession = -1;
        public bool affectsCount = false;
        public double professionChange = 0.0f;
        public string professionOperation = "*";
    }
}

