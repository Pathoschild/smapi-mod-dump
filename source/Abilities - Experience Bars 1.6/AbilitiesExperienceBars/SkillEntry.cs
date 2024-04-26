/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sandman534/Abilities-Experience-Bars
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;

namespace AbilitiesExperienceBars
{
    public class SkillEntry
    {
        // Skill ID
        public string skillID;

        // Skill Textures and Rectangles
        public Texture2D smallIcon;
        public Rectangle smallIconRectangle;
        public Texture2D bigIcon;
        public Rectangle bigIconRectangle;

        // Skill Colors
        public Color skillColor;
        public Color skillRestorationColor;
        public Color skillFinalColor = new(150, 175, 55);
        public Color skillGoldColor = new(150, 175, 55);

        // Experience and Level tracking
        public int currentEXP;
        public int previousEXP;
        public int currentLevel;
        public int previousLevel;

        // Animation Tracking
        public bool animateSkill;
        public bool expIncreasing;
        public bool expPopup;
        public bool actualExpGainedMessage;
        public int expGained;
        public byte expAlpha;
        public bool inIncrease;
        public bool inWait;
        public bool inDecrease;
        public int timeExpMessageLeft;

        // Experience Levels
        public List<int> expPerLevel;
        public bool levelExtended;
        public int maxLevel;

        // Mastery Test
        public bool isMastery;
        public bool isMastered;
        private bool isBasic;

        // API Interface
        private ISpaceCoreApi _spaceCoreAPI;

        public SkillEntry(ISpaceCoreApi spaceCoreAPI, string ID, int skillIndex, Texture2D skillIcon, Color skillColorCode, bool levelExtender)
        {
            // Set the skill ID
            skillID = ID;

            // Setup API
            _spaceCoreAPI = spaceCoreAPI;
            levelExtended = levelExtender;

            // Load Skill Icon
            if (skillIndex > 0)
                SetSkillIcon(skillIndex, skillIcon);
            else if (_spaceCoreAPI != null)
                SetCustomSkillIcon();
                
            // Load Colors
            skillColor = skillColorCode;
            skillRestorationColor = skillColorCode;

            // Check for a basic skill
            List<string> basicSkills = new() { "farming", "fishing", "foraging", "mining", "combat" };
            isBasic = basicSkills.Contains(skillID);

            // Mastery or regular skill
            if (skillID == "mastery")
                SetMaxLevel(5, true);
            else if (levelExtended && (isBasic || skillID == "luck"))
                SetMaxLevel(100, false);
            else
                SetMaxLevel(10, false);

            // Set Current Data
            SetSkillData(true);

            // Set Mastery Bool
            if (isBasic && currentLevel >= 10)
                isMastered = true;
        }

        public void SetSkillData(bool current)
        {
            // Stardew Valley Skills
            if (skillID == "farming")
                SetData(Game1.player.farmingLevel.Value, Game1.player.experiencePoints[0], current);
            else if (skillID == "fishing")
                SetData(Game1.player.fishingLevel.Value, Game1.player.experiencePoints[1], current);
            else if (skillID == "foraging")
                SetData(Game1.player.foragingLevel.Value, Game1.player.experiencePoints[2], current);
            else if (skillID == "mining")
                SetData(Game1.player.miningLevel.Value, Game1.player.experiencePoints[3], current);
            else if (skillID == "combat")
                SetData(Game1.player.combatLevel.Value, Game1.player.experiencePoints[4], current);
            else if (skillID == "luck")
                SetData(Game1.player.luckLevel.Value, Game1.player.experiencePoints[5], current);
            else if (skillID == "mastery")
                SetData((int)Game1.stats.Get("masteryLevelsSpent"), (int)Game1.stats.Get("MasteryExp"), current);
            else if (_spaceCoreAPI != null)
                SetData(_spaceCoreAPI.GetLevelForCustomSkill(Game1.player, skillID), _spaceCoreAPI.GetExperienceForCustomSkill(Game1.player, skillID), current);

            // Check if a basic skill has reached level 10 for mastery
            if (current && isBasic && !isMastered && currentLevel == 10)
                isMastered = true;
        }

        public Rectangle GetExperienceBar(Vector2 barPosition, Vector2 barSize, int scale)
        {
            float percentage = currentLevel >= maxLevel ? barSize.X : ((float)currentEXP - (float)expPerLevel[currentLevel]) / ((float)expPerLevel[currentLevel + 1] - (float)expPerLevel[currentLevel]) * barSize.X;
            Rectangle barRect = new((int)barPosition.X, (int)barPosition.Y, (int)percentage * scale, (int)barSize.Y * scale);
            return barRect;
        }

        public string GetExperienceText(int scale)
        {
            // Show accumulated experience
            string experienceText = $"{currentEXP} xp";
            if (currentEXP >= 1000000 && scale == 1)
                experienceText = $"{(currentEXP / 1000000D).ToString("0.#") + "M"} xp";

            // Abbreviate Next over 100k
            if (currentLevel < maxLevel)
            {
                // Current Experience Value
                int expGained = currentEXP - expPerLevel[currentLevel];
                string thisLevel = expGained.ToString();
                if (expGained >= 1000000)
                    thisLevel = (expGained / 1000000D).ToString("0.#") + "M";
                else if (expGained >= 100000 && scale >= 3)
                    thisLevel = (expGained / 1000D).ToString("0.#") + "K";
                else if (expGained >= 100000)
                    thisLevel = (expGained / 1000D).ToString("0") + "K";
                else if (expGained >= 10000 && scale <= 2)
                    thisLevel = (expGained / 1000D).ToString("0") + "K";

                // Next Experience Value
                int expToLevel = expPerLevel[currentLevel + 1] - expPerLevel[currentLevel];
                string nextLevel = expToLevel.ToString();
                if (expToLevel >= 1000000)
                    nextLevel = (expToLevel / 1000000D).ToString("0.#") + "M";
                else if (expToLevel >= 100000 && scale >= 3)
                    nextLevel = (expToLevel / 1000D).ToString("0.#") + "K";
                else if (expToLevel >= 100000)
                    nextLevel = (expToLevel / 1000D).ToString("0") + "K";
                else if (expToLevel >= 10000 && scale <= 2)
                    nextLevel = (expToLevel / 1000D).ToString("0") + "K";

                // Set Text
                experienceText = $"{thisLevel}/{nextLevel}";
            }

            return experienceText;
        }

        public void ExperienceAlpha(byte intensity)
        {
            if (inIncrease)
            {
                int virtualAlphaValue = expAlpha + intensity;
                if (virtualAlphaValue < 255)
                    expAlpha += intensity;
                else
                {
                    expAlpha = 255;
                    inIncrease = false;
                    inWait = true;
                }
            }
            else if (inWait)
            {
                if (timeExpMessageLeft > 0)
                    timeExpMessageLeft--;
                else
                {
                    inWait = false;
                    inDecrease = true;
                }
            }
            else if (inDecrease)
            {
                int virtualAlphaValue = expAlpha - intensity;
                if (virtualAlphaValue > 0)
                    expAlpha -= intensity;
                else
                {
                    expAlpha = 0;
                    inDecrease = false;
                    actualExpGainedMessage = false;
                }
            }
        }

        public float ExperienceTextScale(int scale)
        {
            return 0.1f + (scale * 0.2f);
        }

        public float LevelTextScale(int scale)
        {
            float baseScale;
            if (currentLevel >= 10)
                baseScale = 0f;
            else
                baseScale = 0.25f;

            return baseScale + (scale * 0.25f);
        }

        public bool GainLevel()
        {
            if (currentLevel == previousLevel) return false;

            // Set Level
            previousLevel = currentLevel;
            return true;
        }

        public void GainExperience()
        {
            if (currentEXP == previousEXP) return;

            // Set Experience Values
            expGained = currentEXP - previousEXP;
            previousEXP = currentEXP;

            // Set Experience Values
            inIncrease = true;
            actualExpGainedMessage = true;
            timeExpMessageLeft = 3 * 60;
            expAlpha = 0;

            // Set Experience Bools
            expPopup = true;
            expIncreasing = true;
            animateSkill = true;

        }

        public bool DisplaySkill(bool enabled)
        {
            return enabled && currentLevel >= maxLevel ? false : true;
        }

        private void SetData(int level, int exp, bool current)
        {
            if (current)
            {
                //currentLevel = iLevel;
                currentLevel = SetLevelFromExperience(exp);
                currentEXP = exp;
            }
            else
            {
                //previousLevel = iLevel;
                previousLevel = SetLevelFromExperience(exp);
                previousEXP = exp;
            }
        }

        private int SetLevelFromExperience(int experience)
        {
            int level = maxLevel;
            for (int i = 1; i < expPerLevel.Count; i++)
            {
                if (expPerLevel[i] > experience)
                {
                    level = i - 1;
                    break;
                }
            }

            return level;
        }

        private void SetCustomSkillIcon()
        {
            smallIcon = _spaceCoreAPI.GetSkillPageIconForCustomSkill(skillID);
            smallIconRectangle = new(0, 0, 10, 10);
            bigIcon = _spaceCoreAPI.GetSkillIconForCustomSkill(skillID);
            bigIconRectangle = new(0, 0, 16, 16);
        }

        private void SetSkillIcon(int skillIndex, Texture2D skillIcon)
        {
            // Change the Y postion based on the skill index
            int xPosition = 10 * ((skillIndex % 6 > 0 ? skillIndex % 6 : 6) - 1);
            int yPosition = 64 + (10 * (skillIndex % 6 > 0 ? skillIndex / 6 : (skillIndex / 6) - 1));
            smallIcon = skillIcon;
            smallIconRectangle = new Rectangle(xPosition, yPosition, 10, 10);

            // Change the Y postion based on the skill index
            xPosition = 16 * ((skillIndex % 6 > 0 ? skillIndex % 6 : 6) - 1);
            yPosition = 16 * (skillIndex % 6 > 0 ? skillIndex / 6 : (skillIndex / 6) - 1);
            bigIcon = skillIcon;
            bigIconRectangle = new Rectangle(xPosition, yPosition, 16, 16);
        }

        private void SetMaxLevel(int max, bool mastery)
        {
            isMastery = mastery;
            maxLevel = max;

            // Set default experience levels
            if (isMastery)
                expPerLevel = new() { 0, 10000, 25000, 45000, 70000, 100000 };
            else
                expPerLevel = new() { 0, 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };

            // If using level extended, set the curve
            if (levelExtended)
            {
                for (int i = 11; i <= maxLevel; i++)
                {
                    if (i < 45)
                        expPerLevel.Add(expPerLevel[i - 1] + 300 + (1000 * i));
                    else
                        expPerLevel.Add(expPerLevel[i - 1] + 300 + (int)Math.Round(i * i * i * 0.5));
                }
            }
        }
    }
}
