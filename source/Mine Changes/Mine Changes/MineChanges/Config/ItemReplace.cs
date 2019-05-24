using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using StardewValley;
using StardewValley.Locations;

namespace Mine_Changes.MineChanges.Config
{
    public class ItemReplace
    {
        public int itemIndex = -1000;
        public string itemName = "";
        public List<int> itemsToReplace = new List<int>();
        public List<int> mineAreas = new List<int>();
        public List<int> mineLevels = new List<int>();
        //public List<LevelFormula> mineLevelFormulas = new List<LevelFormula>(); // TODO: allow level selection by formula, like level%5 == 0
        public bool replaceAll = false;
        public double chance = 0.0f;
        public List<ProfessionChanges> professionChanges = new List<ProfessionChanges>();


        public StardewValley.Object tryAndReplaceObject(StardewValley.Object source, MineShaft locale, int level)
        {
            Random r = (Random)(typeof(MineShaft).GetField("mineRandom", AccessTools.all).GetValue(locale));

            if (validLevel(locale, level))
            {
                if (replaceAll || itemsToReplace.Contains(source.ParentSheetIndex))
                {
                    double realChance = chance;
                    foreach (Farmer who in locale.farmers)
                    {
                        realChance = applyProfessionChances(who, chance);
                    }
                    if (r.NextDouble() < realChance)
                    {
                        int idx = getItemIndex();
                        if (idx >= 0)
                        {
                            return new StardewValley.Object(idx, 1, false, -1, 0)
                            {
                                IsSpawnedObject = true
                            };
                        }
                    }
                }
            }

            return source;
        }

        public int getItemIndex()
        {
            if (itemIndex <= -1000)
            {
                if (itemName == "")
                {
                    return -1;
                }
                itemIndex = Mod.getObjectIDFromAsset(itemName);
                if (itemIndex <= -1000)
                    return -1;
            }
            
            return itemIndex;
        }

        public bool validLevel(MineShaft locale, int level)
        {
            if(mineLevels != null && mineLevels.Count > 0 && mineLevels.Contains(level))
            {
                return true;
            }
            return (mineAreas == null || mineAreas.Count == 0 || mineAreas.Contains(locale.getMineArea()));
        }

        public double applyProfessionChances(Farmer who, double chance)
        {
            double ans = chance;
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
    }
    
}

