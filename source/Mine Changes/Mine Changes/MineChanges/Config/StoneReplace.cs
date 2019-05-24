using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace Mine_Changes.MineChanges.Config
{
    public class StoneReplace
    {
        public static List<int> allStones = new List<int> {
            2,4,6,8,10,12,14, //gemtones
            32,34,36,38,40,42, //regular stones 1
            44,46,//purple and mystic stone
            48,50,52,54,56,58,//regular stones 2
            75,76,77, //geode stones
            290, //iron
            668,670, //rock stones
            751,//copper
            760,762,// dark stones
            764,765 //gold, iridium
        };
        public static List<int> regularStones = new List<int> {
            32,34,36,38,40,42, //regular stones 1
            48,50,52,54,56,58,//regular stones 2
            668,670, //rock stones
            760,762// dark stones
        };


        public int stoneIndex = -1000;
        public string stoneName = "";
        public List<int> stonesToReplace = new List<int>();
        public List<int> mineAreas = new List<int>();
        public List<int> mineLevels = new List<int>();
        public bool replaceAll = false;
        public double chance = 0.0f;
        public int durability = 3;
        public List<ProfessionChanges> professionChanges = new List<ProfessionChanges>();


        public StardewValley.Object tryAndReplaceObject(StardewValley.Object source, MineShaft locale, Vector2 tile)
        {
            Random r = (Random)(typeof(MineShaft).GetField("mineRandom", AccessTools.all).GetValue(locale));
            if(stonesToReplace == null || stonesToReplace.Count == 0)
            {
                stonesToReplace = regularStones;
            }
            if (validLevel(locale, locale.mineLevel))
            {
                if (replaceAll || stonesToReplace.Contains(source.ParentSheetIndex))
                {
                    double realChance = chance;
                    foreach (Farmer who in locale.farmers)
                    {
                        realChance = applyProfessionChances(who, chance);
                    }
                    if (r.NextDouble() < realChance)
                    {
                        int idx = getStoneIndex();
                        if (idx >= 0)
                        {
                            return new StardewValley.Object(tile, idx, "Stone", true, false, false, false)
                            {
                                MinutesUntilReady = durability
                            };
                        }
                    }
                }
            }

            return source;
        }

        public bool validLevel(MineShaft locale, int level)
        {
            if (mineLevels != null && mineLevels.Count > 0 && mineLevels.Contains(level))
            {
                return true;
            }
            return (mineAreas == null || mineAreas.Count == 0 || mineAreas.Contains(locale.getMineArea()));
        }

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
