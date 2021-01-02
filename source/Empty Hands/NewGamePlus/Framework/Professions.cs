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
using StardewValley;

namespace NewGamePlus.Framework
{
    internal class Professions
    {
        /*********
        ** Accessors
        *********/
        // professions breakdown
        public static List<int> Farming = new List<int> { Farmer.rancher, Farmer.tiller, Farmer.butcher, Farmer.shepherd, Farmer.artisan, Farmer.agriculturist };
        public static List<int> Fishing = new List<int> { Farmer.fisher, Farmer.trapper, Farmer.angler, Farmer.pirate, Farmer.baitmaster, Farmer.mariner };
        public static List<int> Foraging = new List<int> { Farmer.forester, Farmer.gatherer, Farmer.lumberjack, Farmer.tapper, Farmer.botanist, Farmer.tracker };
        public static List<int> Mining = new List<int> { Farmer.miner, Farmer.geologist, Farmer.blacksmith, Farmer.burrower, Farmer.excavator, Farmer.gemologist };
        public static List<int> Combat = new List<int> { Farmer.fighter, Farmer.scout, Farmer.brute, Farmer.defender, Farmer.acrobat, Farmer.desperado };
        public static Dictionary<int, int[]> Hierarchy = new Dictionary<int, int[]>
        {
            [Farmer.rancher] = new[] { Farmer.farmingSkill, -1 },
            [Farmer.tiller] = new[] { Farmer.farmingSkill, -1 },
            [Farmer.butcher] = new[] { Farmer.farmingSkill, Farmer.rancher },
            [Farmer.shepherd] = new[] { Farmer.farmingSkill, Farmer.rancher },
            [Farmer.artisan] = new[] { Farmer.farmingSkill, Farmer.tiller },
            [Farmer.agriculturist] = new[] { Farmer.farmingSkill, Farmer.tiller },

            [Farmer.fisher] = new[] { Farmer.fishingSkill, -1 },
            [Farmer.trapper] = new[] { Farmer.fishingSkill, -1 },
            [Farmer.angler] = new[] { Farmer.fishingSkill, Farmer.fisher },
            [Farmer.pirate] = new[] { Farmer.fishingSkill, Farmer.fisher },
            [Farmer.baitmaster] = new[] { Farmer.fishingSkill, Farmer.trapper },
            [Farmer.mariner] = new[] { Farmer.fishingSkill, Farmer.trapper },

            [Farmer.forester] = new[] { Farmer.foragingSkill, -1 },
            [Farmer.gatherer] = new[] { Farmer.foragingSkill, -1 },
            [Farmer.lumberjack] = new[] { Farmer.foragingSkill, Farmer.forester },
            [Farmer.tapper] = new[] { Farmer.foragingSkill, Farmer.forester },
            [Farmer.botanist] = new[] { Farmer.foragingSkill, Farmer.gatherer },
            [Farmer.tracker] = new[] { Farmer.foragingSkill, Farmer.gatherer },

            [Farmer.miner] = new[] { Farmer.miningSkill, -1 },
            [Farmer.geologist] = new[] { Farmer.miningSkill, -1 },
            [Farmer.blacksmith] = new[] { Farmer.miningSkill, Farmer.miner },
            [Farmer.burrower] = new[] { Farmer.miningSkill, Farmer.miner },
            [Farmer.excavator] = new[] { Farmer.miningSkill, Farmer.geologist },
            [Farmer.gemologist] = new[] { Farmer.miningSkill, Farmer.geologist },

            [Farmer.fighter] = new[] { Farmer.combatSkill, -1 },
            [Farmer.scout] = new[] { Farmer.combatSkill, -1 },
            [Farmer.brute] = new[] { Farmer.combatSkill, Farmer.fighter },
            [Farmer.defender] = new[] { Farmer.combatSkill, Farmer.fighter },
            [Farmer.acrobat] = new[] { Farmer.combatSkill, Farmer.scout },
            [Farmer.desperado] = new[] { Farmer.combatSkill, Farmer.scout },
        };


        /*********
        ** Public methods
        *********/
        public static int GetSkillForProfession(int profession)
        {
            if (!Hierarchy.ContainsKey(profession)) return -1;
            return Hierarchy[profession][0];
        }

        public static bool HasAllProfessionsForSkill(Farmer farmer, int skill)
        {
            List<int> list;
            switch (skill)
            {
                case Farmer.farmingSkill:
                    list = Farming;
                    break;
                case Farmer.fishingSkill:
                    list = Fishing;
                    break;
                case Farmer.foragingSkill:
                    list = Foraging;
                    break;
                case Farmer.miningSkill:
                    list = Mining;
                    break;
                case Farmer.combatSkill:
                    list = Combat;
                    break;
                default:
                    return false;
            }
            foreach (int profession in list)
            {
                if (!farmer.professions.Contains(profession))
                    return false;
            }
            return true;
        }
    }
}
