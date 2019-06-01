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
        public bool onlyFirstFarmer = true;
        public List<GroupOfChanges> prioritizedListOfChanges = new List<GroupOfChanges>();
        public List<ProfessionChanges> professionChanges = new List<ProfessionChanges>();
        public List<SkillChanges> skillChanges = new List<SkillChanges>();
        public List<TimeChanges> timeChanges = new List<TimeChanges>();
        public List<WeatherChanges> weatherChanges = new List<WeatherChanges>();

        public StardewValley.Object tryAndReplaceObject(StardewValley.Object source, MineShaft locale, int level)
        {
            Random r = (Random)(typeof(MineShaft).GetField("mineRandom", AccessTools.all).GetValue(locale));

            if (validLevel(locale, level))
            {
                if (replaceAll || itemsToReplace.Contains(source.ParentSheetIndex))
                {
                    double realChance = chance;
                    if (!onlyFirstFarmer)
                    {
                        realChance = applyChances(locale);
                    }
                    else
                    {
                        realChance = applyChances();
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
            if (mineLevels != null && mineLevels.Count > 0 && mineLevels.Contains(level))
            {
                return true;
            }
            return (mineAreas == null || mineAreas.Count == 0 || mineAreas.Contains(locale.getMineArea()));
        }

        public double applyChances(GameLocation loc)
        {
            double ans = this.chance;
            foreach (Farmer who in loc.farmers)
            {
               ans = applyChances(who, ref ans);
            }
            return ans;
        }

        public double applyChances()
        {
            double ans = this.chance;
            return applyChances(Game1.player, ref ans);
        }

        public double applyChances(Farmer who, ref double chance)
        {
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

    }    
}

