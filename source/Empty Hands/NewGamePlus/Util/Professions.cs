/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/quicksilverfox/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace NewGamePlus.Util
{
    class Professions
    {
        // professions breakdown
        public static List<int> Farmer = new List<int> { StardewValley.Farmer.rancher, StardewValley.Farmer.tiller, StardewValley.Farmer.butcher, StardewValley.Farmer.shepherd, StardewValley.Farmer.artisan, StardewValley.Farmer.agriculturist };
        public static List<int> Fishing = new List<int> { StardewValley.Farmer.fisher, StardewValley.Farmer.trapper, StardewValley.Farmer.angler, StardewValley.Farmer.pirate, StardewValley.Farmer.baitmaster, StardewValley.Farmer.mariner };
        public static List<int> Foraging = new List<int> { StardewValley.Farmer.forester, StardewValley.Farmer.gatherer, StardewValley.Farmer.lumberjack, StardewValley.Farmer.tapper, StardewValley.Farmer.botanist, StardewValley.Farmer.tracker };
        public static List<int> Mining = new List<int> { StardewValley.Farmer.miner, StardewValley.Farmer.geologist, StardewValley.Farmer.blacksmith, StardewValley.Farmer.burrower, StardewValley.Farmer.excavator, StardewValley.Farmer.gemologist };
        public static List<int> Combat = new List<int> { StardewValley.Farmer.fighter, StardewValley.Farmer.scout, StardewValley.Farmer.brute, StardewValley.Farmer.defender, StardewValley.Farmer.acrobat, StardewValley.Farmer.desperado };
        public static Dictionary<int, int[]> Hierarchy = new Dictionary<int, int[]>
        {
            [StardewValley.Farmer.rancher] = new int[] { StardewValley.Farmer.farmingSkill, - 1 },
            [StardewValley.Farmer.tiller] = new int[] { StardewValley.Farmer.farmingSkill, -1 },
            [StardewValley.Farmer.butcher] = new int[] { StardewValley.Farmer.farmingSkill, StardewValley.Farmer.rancher },
            [StardewValley.Farmer.shepherd] = new int[] { StardewValley.Farmer.farmingSkill, StardewValley.Farmer.rancher },
            [StardewValley.Farmer.artisan] = new int[] { StardewValley.Farmer.farmingSkill, StardewValley.Farmer.tiller },
            [StardewValley.Farmer.agriculturist] = new int[] { StardewValley.Farmer.farmingSkill, StardewValley.Farmer.tiller },

            [StardewValley.Farmer.fisher] = new int[] { StardewValley.Farmer.fishingSkill, -1 },
            [StardewValley.Farmer.trapper] = new int[] { StardewValley.Farmer.fishingSkill, -1 },
            [StardewValley.Farmer.angler] = new int[] { StardewValley.Farmer.fishingSkill, StardewValley.Farmer.fisher },
            [StardewValley.Farmer.pirate] = new int[] { StardewValley.Farmer.fishingSkill, StardewValley.Farmer.fisher },
            [StardewValley.Farmer.baitmaster] = new int[] { StardewValley.Farmer.fishingSkill, StardewValley.Farmer.trapper },
            [StardewValley.Farmer.mariner] = new int[] { StardewValley.Farmer.fishingSkill, StardewValley.Farmer.trapper },

            [StardewValley.Farmer.forester] = new int[] { StardewValley.Farmer.foragingSkill, -1 },
            [StardewValley.Farmer.gatherer] = new int[] { StardewValley.Farmer.foragingSkill, -1 },
            [StardewValley.Farmer.lumberjack] = new int[] { StardewValley.Farmer.foragingSkill, StardewValley.Farmer.forester },
            [StardewValley.Farmer.tapper] = new int[] { StardewValley.Farmer.foragingSkill, StardewValley.Farmer.forester },
            [StardewValley.Farmer.botanist] = new int[] { StardewValley.Farmer.foragingSkill, StardewValley.Farmer.gatherer },
            [StardewValley.Farmer.tracker] = new int[] { StardewValley.Farmer.foragingSkill, StardewValley.Farmer.gatherer },

            [StardewValley.Farmer.miner] = new int[] { StardewValley.Farmer.miningSkill, -1 },
            [StardewValley.Farmer.geologist] = new int[] { StardewValley.Farmer.miningSkill, -1 },
            [StardewValley.Farmer.blacksmith] = new int[] { StardewValley.Farmer.miningSkill, StardewValley.Farmer.miner },
            [StardewValley.Farmer.burrower] = new int[] { StardewValley.Farmer.miningSkill, StardewValley.Farmer.miner },
            [StardewValley.Farmer.excavator] = new int[] { StardewValley.Farmer.miningSkill, StardewValley.Farmer.geologist },
            [StardewValley.Farmer.gemologist] = new int[] { StardewValley.Farmer.miningSkill, StardewValley.Farmer.geologist },

            [StardewValley.Farmer.fighter] = new int[] { StardewValley.Farmer.combatSkill, -1 },
            [StardewValley.Farmer.scout] = new int[] { StardewValley.Farmer.combatSkill, -1 },
            [StardewValley.Farmer.brute] = new int[] { StardewValley.Farmer.combatSkill, StardewValley.Farmer.fighter },
            [StardewValley.Farmer.defender] = new int[] { StardewValley.Farmer.combatSkill, StardewValley.Farmer.fighter },
            [StardewValley.Farmer.acrobat] = new int[] { StardewValley.Farmer.combatSkill, StardewValley.Farmer.scout },
            [StardewValley.Farmer.desperado] = new int[] { StardewValley.Farmer.combatSkill, StardewValley.Farmer.scout },
        };

        public static int GetSkillForProfession(int profession)
        {
            if (!Hierarchy.ContainsKey(profession)) return -1;
            return Hierarchy[profession][0];
        }

        public static bool HasAllProfessionsForSkill(StardewValley.Farmer farmer, int skill)
        {
            List<int> list;
            switch(skill)
            {
                case StardewValley.Farmer.farmingSkill:
                    list = Farmer;
                    break;
                case StardewValley.Farmer.fishingSkill:
                    list = Fishing;
                    break;
                case StardewValley.Farmer.foragingSkill:
                    list = Foraging;
                    break;
                case StardewValley.Farmer.miningSkill:
                    list = Mining;
                    break;
                case StardewValley.Farmer.combatSkill:
                    list = Combat;
                    break;
                default:
                    return false;
            }
            foreach(int profession in list)
            {
                if (!farmer.professions.Contains(profession))
                    return false;
            }
            return true;
        }
    }
}
