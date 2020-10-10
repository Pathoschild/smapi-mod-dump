/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

// Copyright (c) 2019 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using Netcode;
using StardewModdingAPI;
using StardewValley;

using System.Collections.Generic;

namespace LevelingAdjustment
{
    public class ModEntry : Mod
    {
        public const int SKILL_COUNT = 5; //without luck
        public const int FARMING_SKILL = 0;
        public const int MINING_SKILL = 3;
        public const int FISHING_SKILL = 1;
        public const int FORAGING_SKILL = 2;
        public const int LUCK_SKILL = 5;
        public const int COMBAT_SKILL = 4;

        private int[] oldExperiencePoints;
        private int[] oldLevels;
        private LevelingConfig conf;

        private List<ExpAnimation> expAnimations = new List<ExpAnimation>();

        public override void Entry(IModHelper helper)
        {
            conf = Helper.ReadConfig<LevelingConfig>();

            if (conf.combatExperienceFactor < 0 || conf.farmingExperienceFactor < 0 || conf.fishingExperienceFactor < 0 || conf.foragingExperienceFactor < 0 || conf.miningExperienceFactor < 0 || conf.generalExperienceFactor < 0)
            {
                Monitor.Log("ExperienceFactors in config.json must be at least 0", LogLevel.Error);
                Monitor.Log("Deactivating mod", LogLevel.Error);
                return;
            }

            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

            Helper.Events.Display.RenderedWorld += Display_RenderedWorld;


            /*
            Helper.ConsoleCommands.Add("setexp", "", HandleSetExp);
            Helper.ConsoleCommands.Add("resetlevels", "", (arg, args) =>
            {
                for (int skill = 0; skill < SKILL_COUNT; skill++)
                {
                    int exp = Game1.player.experiencePoints[skill];
                    SetLevel(skill, exp);
                }
            });
            */
        }

        void HandleSetExp(string arg1, string[] arg2)
        {
            if (arg2.Length == 2)
            {
                int skill = System.Convert.ToInt32(arg2[0]);
                int exp = System.Convert.ToInt32(arg2[1]);
                Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
                Game1.player.experiencePoints[skill] = exp;
                SetOldExpArray();
                Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            }
        }


        void Display_RenderedWorld(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            if (Game1.paused)
                return;

            expAnimations.RemoveAll(anim => anim.Expired());
            expAnimations.ForEach(anim =>
            {
                anim.update(Game1.currentGameTime);
                anim.Draw(e.SpriteBatch);
            });
        }


        void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            conf = Helper.ReadConfig<LevelingConfig>();
            SetOldExpArray();
        }

        private void SetOldExpArray()
        {
            oldExperiencePoints = new int[SKILL_COUNT];
            var exp = Game1.player.experiencePoints.Fields.ToArray();
            for (int i = 0; i < SKILL_COUNT; i++)
                oldExperiencePoints[i] = exp[i];

            oldLevels = new int[SKILL_COUNT];
            oldLevels[FARMING_SKILL] = Game1.player.farmingLevel;
            oldLevels[FISHING_SKILL] = Game1.player.fishingLevel;
            oldLevels[FORAGING_SKILL] = Game1.player.foragingLevel;
            oldLevels[MINING_SKILL] = Game1.player.miningLevel;
            oldLevels[COMBAT_SKILL] = Game1.player.combatLevel;
        }

        void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            bool expchanged = false;

            var gameexp = Game1.player.experiencePoints.Fields.ToArray();
            for (int skill = 0; skill < SKILL_COUNT; skill++)
            {
                int gameexpi = gameexp[skill];
                int diff = gameexpi - oldExperiencePoints[skill];
                if (diff > 0)
                {
                    expchanged = true;

                    Monitor.Log(SkillName(skill) + " exp is " + oldExperiencePoints[skill], LogLevel.Trace);
                    Monitor.Log(SkillName(skill) + " exp increased by " + diff + " (game)", LogLevel.Trace);

                    int modexpi = (int)System.Math.Ceiling(oldExperiencePoints[skill] + diff * conf.generalExperienceFactor * ExperienceFactor(skill));
                    int moddiff = modexpi - oldExperiencePoints[skill];
                    int newgain = moddiff - diff;
                    Monitor.Log(SkillName(skill) + " exp increased by " + moddiff + " (with factor " + conf.generalExperienceFactor + "*" + ExperienceFactor(skill), LogLevel.Trace);

                    if (newgain > 0) //mod increases exp gain
                    {
                        //handles level ups
                        Game1.player.gainExperience(skill, newgain);
                    }
                    else if (newgain < 0) //mod reduces exp gain
                    {
                        //check if level was increased but modexpi would decrease the level again
                        //in this case do nothing since otherwise the player might get notified multiple times about the new level
                        if (GetLevelFromExp(modexpi) == GetSkillLevelValue(skill))
                        {
                            Game1.player.experiencePoints[skill] = modexpi;
                        }
                    }

                    Monitor.Log(SkillName(skill) + " exp is now " + Game1.player.experiencePoints[skill], LogLevel.Trace);

                    if (conf.expNotification)
                    {
                        expAnimations.Add(new ExpAnimation(moddiff, skill));
                    }
                }
            }

            if (conf.levelNotification)
            {
                if (Game1.player.farmingLevel != oldLevels[FARMING_SKILL])
                {
                    expAnimations.Clear();
                    expAnimations.Add(new ExpAnimation(FARMING_SKILL));
                }

                if (Game1.player.fishingLevel != oldLevels[FISHING_SKILL])
                {
                    expAnimations.Clear();
                    expAnimations.Add(new ExpAnimation(FISHING_SKILL));
                }

                if (Game1.player.miningLevel != oldLevels[MINING_SKILL])
                {
                    expAnimations.Clear();
                    expAnimations.Add(new ExpAnimation(MINING_SKILL));
                }

                if (Game1.player.foragingLevel != oldLevels[FORAGING_SKILL])
                {
                    expAnimations.Clear();
                    expAnimations.Add(new ExpAnimation(FORAGING_SKILL));
                }

                if (Game1.player.combatLevel != oldLevels[COMBAT_SKILL])
                {
                    expAnimations.Clear();
                    expAnimations.Add(new ExpAnimation(COMBAT_SKILL));
                }
            }

            if (expchanged)
                SetOldExpArray();
        }

        private void SetLevel(int skill, int exp)
        {
            int level = GetLevelFromExp(exp);
            switch (skill)
            {
                case FARMING_SKILL:
                    Game1.player.FarmingLevel = level;
                    return;
                case MINING_SKILL:
                    Game1.player.MiningLevel = level;
                    return;
                case FISHING_SKILL:
                    Game1.player.FishingLevel = level;
                    return;
                case FORAGING_SKILL:
                    Game1.player.ForagingLevel = level;
                    return;
                case COMBAT_SKILL:
                    Game1.player.CombatLevel = level;
                    return;
                default:
                    Monitor.Log($"SetLevel({skill})", LogLevel.Error);
                    return;
            }
        }

        /// <summary>
        /// Returns the value of the player level for the given skill number
        /// </summary>
        private int GetSkillLevelValue(int skill)
        {
            switch (skill)
            {
                //lower-case fields should be used (farming instead of Farming) 
                case FARMING_SKILL:
                    return Game1.player.farmingLevel;
                case MINING_SKILL:
                    return Game1.player.miningLevel;
                case FISHING_SKILL:
                    return Game1.player.fishingLevel;
                case FORAGING_SKILL:
                    return Game1.player.foragingLevel;
                case COMBAT_SKILL:
                    return Game1.player.combatLevel;
                default:
                    Monitor.Log($"GetSkillLevelValue({skill})", LogLevel.Error);
                    return 0;
            }
        }

        /// <summary>
        /// Returns the expected level for the given exp value
        /// </summary>
        private int GetLevelFromExp(int exp)
        {
            if (0 <= exp && exp < 100)
            {
                return 0;
            }
            if (100 <= exp && exp < 380)
            {
                return 1;
            }
            if (380 <= exp && exp < 770)
            {
                return 2;
            }
            if (770 <= exp && exp < 1300)
            {
                return 3;
            }
            if (1300 <= exp && exp < 2150)
            {
                return 4;
            }
            if (2150 <= exp && exp < 3300)
            {
                return 5;
            }
            if (3300 <= exp && exp < 4800)
            {
                return 6;
            }
            if (4800 <= exp && exp < 6900)
            {
                return 7;
            }
            if (6900 <= exp && exp < 10000)
            {
                return 8;
            }
            if (10000 <= exp && exp < 15000)
            {
                return 9;
            }
            if (15000 <= exp)
            {
                return 10;
            }
            Monitor.Log($"GetLevelFromExp({exp})", LogLevel.Error);
            return -1;
        }
    

    private double ExperienceFactor(int skill)
        {
            switch (skill)
            {
                case FARMING_SKILL:
                    return conf.farmingExperienceFactor;
                case MINING_SKILL:
                    return conf.miningExperienceFactor;
                case FISHING_SKILL:
                    return conf.fishingExperienceFactor;
                case FORAGING_SKILL:
                    return conf.foragingExperienceFactor;
                case COMBAT_SKILL:
                    return conf.combatExperienceFactor;
                default:
                    Monitor.Log($"ExperienceFactor({skill})", LogLevel.Error);
                    return 1;
            }
        }

        private string SkillName(int i) {
            switch (i)
            {
                case FARMING_SKILL:
                    return "farmingLevel";
                case MINING_SKILL:
                    return "miningLevel";
                case FISHING_SKILL:
                    return "fishingLevel";
                case FORAGING_SKILL:
                    return "foragingLevel";
                case LUCK_SKILL:
                    return "luckLevel";
                case COMBAT_SKILL:
                    return "combatLevel";
                default:
                    return "unknownLevel";
            }
        }
    }
}
