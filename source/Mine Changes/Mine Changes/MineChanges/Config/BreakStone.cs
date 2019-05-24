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
        public List<ProfessionChanges> professionChanges;

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

        public double applyProfessionChances(Farmer who)
        {
            double ans = oreChance;
            if(professionChanges == null || professionChanges.Count == 0)
                return ans;

            foreach(ProfessionChanges p in professionChanges)
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

        public int applyProfessionCount(Random r, Farmer who)
        {
            double min = minOre;
            double max = maxOre;
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
