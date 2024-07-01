/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Modded;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{

    public static class SkillInjections
    {
        private const int MAX_XP_PER_SKILL = 15000;
        private const string _skillLocationName = "Level {0} {1}";

        private static readonly Dictionary<Skill, string> _skillToModName = new()
        {
            { Skill.Magic, ModNames.MAGIC }, { Skill.Binning, ModNames.BINNING }, { Skill.Cooking, ModNames.COOKING },
            { Skill.Archaeology, ModNames.ARCHAEOLOGY }, { Skill.Socializing, ModNames.SOCIALIZING },
        };

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static Dictionary<Skill, double> _archipelagoExperience = new();

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago,
            LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            AddMissingSkillsToDictionary();
        }

        public static Dictionary<int, int> GetArchipelagoExperience()
        {
            return _archipelagoExperience.ToDictionary(x => (int)x.Key, x => (int)Math.Round(x.Value));
        }

        public static List<string> GetArchipelagoExperienceForPrinting()
        {
            var pattern = "{0}: Level {1} ({2}/{3} to next level)";
            var printedSkills = new List<string>();

            foreach (var (skill, experience) in _archipelagoExperience)
            {
                var skillName = skill.ToString();
                var currentExperience = (int)Math.Round(experience);
                var currentLevel = GetLevel(currentExperience);
                if (currentLevel >= 10)
                {
                    printedSkills.Add($"{skillName}: Max!");
                    continue;
                }

                var neededExperience = GetExperienceNeeded(currentLevel + 1);
                printedSkills.Add(string.Format(pattern, skillName, currentLevel, currentExperience, neededExperience));
            }

            return printedSkills;
        }

        public static void SetArchipelagoExperience(Dictionary<int, int> values)
        {
            if (values == null)
            {
                ResetSkillExperience();
                return;
            }

            _archipelagoExperience = values.ToDictionary(x => (Skill)x.Key, x => (double)x.Value);
        }

        // public virtual void gainExperience(int which, int howMuch)
        public static bool GainExperience_NormalExperience_Prefix(Farmer __instance, int which, int howMuch)
        {
            try
            {
                var skill = (Skill)which;
                var enabledSkills = GetEnabledSkills();

                if (!enabledSkills.Contains(skill) || howMuch <= 0 || !__instance.IsLocalPlayer)
                {
                    return true; // run original logic
                }

                var experienceAmount = GetMultipliedExperience(howMuch);
                if (_archipelagoExperience[skill] >= MAX_XP_PER_SKILL)
                {
                    var currentMasteryLevel = MasteryTrackerMenu.getCurrentMasteryLevel();
                    Game1.stats.Increment("MasteryExp", Math.Max(1, (int)Math.Round(experienceAmount / 2)));
                    if (MasteryTrackerMenu.getCurrentMasteryLevel() > currentMasteryLevel)
                    {
                        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:Mastery_newlevel"));
                        Game1.playSound("newArtifact");
                    }
                }

                var oldExperienceLevel = _archipelagoExperience[skill];
                var newExperienceLevel = _archipelagoExperience[skill] + experienceAmount;
                _archipelagoExperience[skill] = newExperienceLevel;
                var oldLevel = GetLevel(oldExperienceLevel);
                var newLevel = GetLevel(newExperienceLevel);
                if (newLevel <= oldLevel)
                {
                    return false; // don't run original logic
                }

                switch (skill)
                {
                    case Skill.Farming:
                        __instance.farmingLevel.Value = newLevel;
                        break;
                    case Skill.Fishing:
                        __instance.fishingLevel.Value = newLevel;
                        break;
                    case Skill.Foraging:
                        __instance.foragingLevel.Value = newLevel;
                        break;
                    case Skill.Mining:
                        __instance.miningLevel.Value = newLevel;
                        break;
                    case Skill.Combat:
                        __instance.combatLevel.Value = newLevel;
                        break;
                    case Skill.Luck:
                        __instance.luckLevel.Value = newLevel;
                        break;
                }

                for (var levelUp = oldLevel + 1; levelUp <= newLevel; ++levelUp)
                {
                    __instance.newLevels.Add(new Point(which, levelUp));
                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:NewIdeas"));
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GainExperience_NormalExperience_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        [HarmonyPriority(Priority.VeryHigh)]
        public static bool GainExperience_ArchipelagoExperience_Prefix(Farmer __instance, int which, int howMuch)
        {
            try
            {
                var skill = (Skill)which;
                var enabledSkills = GetEnabledSkills();

                if (!enabledSkills.Contains(skill) || howMuch <= 0 || !__instance.IsLocalPlayer)
                {
                    return true; // run original logic
                }

                AddApExperienceAndCheckLocations(skill, howMuch);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GainExperience_ArchipelagoExperience_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        //public class Skills
        //public static void AddExperience(Farmer farmer, string skillName, int amt)
        public static bool AddExperience_ArchipelagoModExperience_Prefix(Farmer farmer, string skillName, int amt)
        {
            try
            {
                var skillActualName = skillName.Split('.').Last().Replace("Skill", "");
                var skill = Enum.Parse<Skill>(skillActualName);

                if (!_archipelago.SlotData.Mods.HasMod(_skillToModName[skill]))
                {
                    return true; // run original logic
                }

                AddApExperienceAndCheckLocations(skill, amt);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AddExperience_ArchipelagoModExperience_Prefix)}:\n{ex}",
                    LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void AddApExperienceAndCheckLocations(Skill skill, int amount)
        {
            var apSkillName = skill.ToString();
            var experienceAmount = GetMultipliedExperience(amount);
            if (_archipelagoExperience[skill] >= MAX_XP_PER_SKILL && _archipelago.GetReceivedItemCount($"{apSkillName} Level") >= 10)
            {
                var currentMasteryLevel = MasteryTrackerMenu.getCurrentMasteryLevel();
                Game1.stats.Increment("MasteryExp", Math.Max(1, (int)Math.Round(experienceAmount / 2)));
                if (MasteryTrackerMenu.getCurrentMasteryLevel() > currentMasteryLevel)
                {
                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:Mastery_newlevel"));
                    Game1.playSound("newArtifact");
                }
            }

            var newExperienceLevel = _archipelagoExperience[skill] + experienceAmount;
            _archipelagoExperience[skill] = newExperienceLevel;
            var newLevel = GetLevel(_archipelagoExperience[skill]);
            for (var i = 1; i <= newLevel; i++)
            {
                var checkedLocation = string.Format(_skillLocationName, i, apSkillName);
                _locationChecker.AddCheckedLocation(checkedLocation);
            }
        }

        private static double GetMultipliedExperience(int amount)
        {
            return amount * _archipelago.SlotData.ExperienceMultiplier;
        }

        private static int GetLevel(double experienceAmount)
        {
            return GetLevel((int)Math.Floor(experienceAmount));
        }

        public static int GetLevel(int experienceAmount)
        {
            return experienceAmount switch
            {
                < 100 => 0,
                < 380 => 1,
                < 770 => 2,
                < 1300 => 3,
                < 2150 => 4,
                < 3300 => 5,
                < 4800 => 6,
                < 6900 => 7,
                < 10000 => 8,
                < MAX_XP_PER_SKILL => 9,
                _ => 10,
            };
        }

        private static int GetExperienceNeeded(int level)
        {
            return level switch
            {
                1 => 100,
                2 => 380,
                3 => 770,
                4 => 1300,
                5 => 2150,
                6 => 3300,
                7 => 4800,
                8 => 6900,
                9 => 10000,
                10 => MAX_XP_PER_SKILL,
                _ => 0,
            };
        }

        public static void AddMissingSkillsToDictionary()
        {
            foreach (var skill in GetEnabledSkills())
            {
                if (_archipelagoExperience.ContainsKey(skill))
                {
                    continue;
                }

                _archipelagoExperience.Add(skill, 0);
            }
        }

        public static void ResetSkillExperience()
        {
            _archipelagoExperience = new Dictionary<Skill, double>();
        }

        private static IEnumerable<Skill> VanillaSkills()
        {
            yield return Skill.Farming;
            yield return Skill.Fishing;
            yield return Skill.Foraging;
            yield return Skill.Mining;
            yield return Skill.Combat;
        }

        public static List<Skill> GetEnabledSkills()
        {
            var skills = new List<Skill>();
            skills.AddRange(VanillaSkills());

            if (_archipelago.SlotData.Mods.HasMod(ModNames.LUCK))
            {
                skills.Add(Skill.Luck);
            }

            if (_archipelago.SlotData.Mods.HasMod(ModNames.BINNING))
            {
                skills.Add(Skill.Binning);
            }

            if (_archipelago.SlotData.Mods.HasMod(ModNames.MAGIC))
            {
                skills.Add(Skill.Magic);
            }

            if (_archipelago.SlotData.Mods.HasMod(ModNames.ARCHAEOLOGY))
            {
                skills.Add(Skill.Archaeology);
            }

            if (_archipelago.SlotData.Mods.HasMod(ModNames.COOKING))
            {
                skills.Add(Skill.Cooking);
            }

            if (_archipelago.SlotData.Mods.HasMod(ModNames.SOCIALIZING))
            {
                skills.Add(Skill.Socializing);
            }

            return skills;
        }
    }

    public enum Skill
    {
        Farming = 0,
        Fishing = 1,
        Foraging = 2,
        Mining = 3,
        Combat = 4,
        Luck = 5,
        Magic = 6,
        Socializing = 7,
        Archaeology = 8,
        Binning = 9,
        Cooking = 10,
    }
}
