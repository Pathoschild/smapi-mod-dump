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
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Goals;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer
{
    public static class MonsterSlayerInjections
    {
        private const string MONSTER_ERADICATION_AP_PREFIX = "Monster Eradication: ";

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static MonsterKillList _killList;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _killList = new MonsterKillList(_archipelago);
        }

        // private void gil()
        public static bool Gil_NoMonsterSlayerRewards_Prefix(AdventureGuild __instance)
        {
            try
            {
                // private NPC Gil
                var gilField = _modHelper.Reflection.GetField<NPC>(__instance, "Gil");
                Game1.drawDialogue(gilField.GetValue(), Game1.content.LoadString("Characters\\Dialogue\\Gil:Snoring"));
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Gil_NoMonsterSlayerRewards_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public static bool areAllMonsterSlayerQuestsComplete()
        public static bool AreAllMonsterSlayerQuestsComplete_ExcludeGingerIsland_Prefix(ref bool __result)
        {
            try
            {
                if (!_archipelago.SlotData.ExcludeGingerIsland)
                {
                    return true; // run original logic
                }

                var num1 = Game1.stats.getMonstersKilled("Green Slime") + Game1.stats.getMonstersKilled("Frost Jelly") + Game1.stats.getMonstersKilled("Sludge") + Game1.stats.getMonstersKilled("Tiger Slime");
                var num2 = Game1.stats.getMonstersKilled("Shadow Guy") + Game1.stats.getMonstersKilled("Shadow Shaman") + Game1.stats.getMonstersKilled("Shadow Brute") + Game1.stats.getMonstersKilled("Shadow Sniper");
                var num3 = Game1.stats.getMonstersKilled("Skeleton") + Game1.stats.getMonstersKilled("Skeleton Mage");
                var num4 = Game1.stats.getMonstersKilled("Rock Crab") + Game1.stats.getMonstersKilled("Lava Crab") + Game1.stats.getMonstersKilled("Iridium Crab");
                var num5 = Game1.stats.getMonstersKilled("Grub") + Game1.stats.getMonstersKilled("Fly") + Game1.stats.getMonstersKilled("Bug");
                var num6 = Game1.stats.getMonstersKilled("Bat") + Game1.stats.getMonstersKilled("Frost Bat") + Game1.stats.getMonstersKilled("Lava Bat") + Game1.stats.getMonstersKilled("Iridium Bat");
                var num7 = Game1.stats.getMonstersKilled("Duggy") + Game1.stats.getMonstersKilled("Magma Duggy");
                Game1.stats.getMonstersKilled("Metal Head");
                Game1.stats.getMonstersKilled("Stone Golem");
                var monstersKilled1 = Game1.stats.getMonstersKilled("Dust Spirit");
                var monstersKilled2 = Game1.stats.getMonstersKilled("Mummy");
                var monstersKilled3 = Game1.stats.getMonstersKilled("Pepper Rex");
                var num8 = Game1.stats.getMonstersKilled("Serpent") + Game1.stats.getMonstersKilled("Royal Serpent");
                // var num9 = Game1.stats.getMonstersKilled("Magma Sprite") + Game1.stats.getMonstersKilled("Magma Sparker"); // None of these guys on exclude island
                __result = num1 >= 1000 && num2 >= 150 && num3 >= 50 && num5 >= 125 && num6 >= 200 && num7 >= 30 && monstersKilled1 >= 500 && num4 >= 60 && monstersKilled2 >= 100 && monstersKilled3 >= 50 && num8 >= 250;// && num9 >= 150;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AreAllMonsterSlayerQuestsComplete_ExcludeGingerIsland_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public void showMonsterKillList()
        public static bool ShowMonsterKillList_CustomListFromAP_Prefix(AdventureGuild __instance)
        {
            try
            {
                if (!Game1.player.mailReceived.Contains("checkedMonsterBoard"))
                {
                    Game1.player.mailReceived.Add("checkedMonsterBoard");
                }

                var killListContent = _killList.GetKillListLetterContent();
                Game1.drawLetterMessage(killListContent);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ShowMonsterKillList_CustomListFromAP_Prefix)}:\n{ex}",
                    LogLevel.Error);
                return true; // run original logic
            }
        }

        // public void monsterKilled(string name)
        public static void MonsterKilled_SendMonstersanityCheck_Postfix(Stats __instance, string name)
        {
            try
            {
                var category = GetCategory(name);
                switch (_archipelago.SlotData.Monstersanity)
                {
                    case Monstersanity.None:
                        return;
                    case Monstersanity.OnePerCategory:
                        CheckLocation(category);
                        return;
                    case Monstersanity.OnePerMonster:
                        CheckLocation(name);
                        return;
                    case Monstersanity.Goals:
                    case Monstersanity.ShortGoals:
                    case Monstersanity.VeryShortGoals:
                        CheckLocationIfEnoughMonstersInCategory(category);
                        return;
                    case Monstersanity.ProgressiveGoals:
                        CheckLocationIfEnoughMonstersInProgressiveCategory(category);
                        return;
                    case Monstersanity.SplitGoals:
                        CheckLocationIfEnoughMonsters(name);
                        return;
                    default:
                        return;
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MonsterKilled_SendMonstersanityCheck_Postfix)}:\n{ex}",
                    LogLevel.Error);
                return;
            }
        }

        // public void monsterKilled(string name)
        public static void MonsterKilled_CheckGoalCompletion_Postfix(Stats __instance, string name)
        {
            try
            {
                GoalCodeInjection.CheckProtectorOfTheValleyGoalCompletion();
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MonsterKilled_CheckGoalCompletion_Postfix)}:\n{ex}",
                    LogLevel.Error);
                return;
            }
        }

        private static void CheckLocationIfEnoughMonstersInCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category) || !_killList.MonsterGoals.ContainsKey(category))
            {
                return;
            }

            var amountNeeded = _killList.MonsterGoals[category];
            if (_killList.GetMonstersKilledInCategory(category) >= amountNeeded)
            {
                CheckLocation(category);
            }
        }

        private static void CheckLocationIfEnoughMonsters(string monster)
        {
            if (string.IsNullOrWhiteSpace(monster) || !_killList.MonsterGoals.ContainsKey(monster))
            {
                return;
            }

            var amountNeeded = _killList.MonsterGoals[monster];
            if (_killList.GetMonstersKilled(monster) >= amountNeeded)
            {
                CheckLocation(monster);
            }
        }

        private static void CheckLocationIfEnoughMonstersInProgressiveCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category) || !_killList.MonsterGoals.ContainsKey(category))
            {
                return;
            }

            var lastAmountNeeded = _killList.MonsterGoals[category];
            var progressiveStep = lastAmountNeeded / 5;
            var monstersKilled = _killList.GetMonstersKilledInCategory(category);
            for (var i = progressiveStep; i <= lastAmountNeeded; i += progressiveStep)
            {
                if (monstersKilled < i)
                {
                    return;
                }

                var progressiveCategoryName = (i == lastAmountNeeded) ? category : $"{i} {category}";
                CheckLocation(progressiveCategoryName);
            }
        }

        private static string GetCategory(string name)
        {
            foreach (var (category, monsters) in _killList.MonstersByCategory)
            {
                if (monsters.Contains(name))
                {
                    return category;
                }
            }

            _monitor.Log($"Could not find a monster slayer category for monster {name}");
            return "";
        }

        private static void CheckLocation(string goalName)
        {
            if (string.IsNullOrEmpty(goalName))
            {
                return;
            }

            goalName = goalName.Replace("Dust Spirit", "Dust Sprite");


            var apLocation = $"{MONSTER_ERADICATION_AP_PREFIX}{goalName}";
            if (_archipelago.GetLocationId(apLocation) > -1)
            {
                _locationChecker.AddCheckedLocation(apLocation);
            }
            else
            {
                _monitor.Log($"Tried to check a monster slayer goal, but it doesn't exist! [{apLocation}]", LogLevel.Info);
            }
        }
    }
}
