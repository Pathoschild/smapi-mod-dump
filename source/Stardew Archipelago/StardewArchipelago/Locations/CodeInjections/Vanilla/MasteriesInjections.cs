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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class MasteriesInjections
    {
        private const string MASTERY = "Mastery";

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago,
            LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public virtual bool performAction(string[] action, Farmer who, Location tileLocation)
        public static bool PerformAction_MasteryCaveInteractions_Prefix(GameLocation __instance, string[] action, Farmer who, Location tileLocation)
        {
            try
            {
                if (__instance.ShouldIgnoreAction(action, who, tileLocation) ||
                    !who.IsLocalPlayer ||
                    !ArgUtility.TryGet(action, 0, out var actionKey, out var error) ||
                    !actionKey.StartsWith(MASTERY))
                {
                    return true; // run original logic
                }

                switch (actionKey)
                {
                    case "MasteryRoom":
                        PerformActionMasteryRoom();
                        return false; // don't run original logic
                    case "MasteryCave_Pedestal":
                        PerformActionMasteryCavePedestal();
                        return false; // don't run original logic
                    case "MasteryCave_Farming":
                        PerformActionMasteryCaveSkill(Skill.Farming);
                        return false; // don't run original logic
                    case "MasteryCave_Foraging":
                        PerformActionMasteryCaveSkill(Skill.Foraging);
                        return false; // don't run original logic
                    case "MasteryCave_Fishing":
                        PerformActionMasteryCaveSkill(Skill.Fishing);
                        return false; // don't run original logic
                    case "MasteryCave_Mining":
                        PerformActionMasteryCaveSkill(Skill.Mining);
                        return false; // don't run original logic
                    case "MasteryCave_Combat":
                        PerformActionMasteryCaveSkill(Skill.Combat);
                        return false; // don't run original logic
                    default:
                        return true; // run original logic
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformAction_MasteryCaveInteractions_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void PerformActionMasteryRoom()
        {
            const string masteryOfTheFiveWays = "Mastery Of The Five Ways";
            if (_archipelago.HasReceivedItem(masteryOfTheFiveWays))
            {
                Game1.playSound("doorClose");
                Game1.warpFarmer("MasteryCave", 7, 11, 0);
                return;
            }

            const string hintMessage = "Only a master of the five ways may enter.";
            Game1.drawObjectDialogue(hintMessage);
        }

        private static void PerformActionMasteryCavePedestal()
        {
            Game1.activeClickableMenu = new MasteryTrackerMenu();
        }

        private static void PerformActionMasteryCaveSkill(Skill skill)
        {
            if (_locationChecker.IsLocationMissing($"{skill} {MASTERY}"))
            {
                Game1.activeClickableMenu = new MasteryTrackerMenu((int)skill);
            }
        }

        // private void claimReward()
        public static bool ClaimReward_SendMasteryCheck_Prefix(MasteryTrackerMenu __instance)
        {
            try
            {
                // private int which;
                var whichField = _helper.Reflection.GetField<int>(__instance, "which");
                var skillNumber = whichField.GetValue();
                var skill = (Skill)skillNumber;

                _locationChecker.AddCheckedLocation($"{skill} {MASTERY}");

                Game1.stats.Increment("masteryLevelsSpent");
                Game1.currentLocation.removeTemporarySpritesWithID(8765 + skillNumber);
                MasteryTrackerMenu.addSkillFlairPlaque(skillNumber);
                var currentMasteryXp = (int)Game1.stats.Get("MasteryExp");
                if (MasteryTrackerMenu.getCurrentMasteryLevel() - (int)Game1.stats.Get("masteryLevelsSpent") <= 0)
                {
                    Game1.currentLocation.removeTemporarySpritesWithID(8765);
                    Game1.currentLocation.removeTemporarySpritesWithID(8766);
                    Game1.currentLocation.removeTemporarySpritesWithID(8767);
                    Game1.currentLocation.removeTemporarySpritesWithID(8768);
                    Game1.currentLocation.removeTemporarySpritesWithID(8769);
                }
                if (!MasteryTrackerMenu.hasCompletedAllMasteryPlaques())
                {
                    return false; // don't run original logic
                }
                DelayedAction.functionAfterDelay(() => MasteryTrackerMenu.addSpiritCandles(), 500);
                Game1.player.freezePause = 2000;
                DelayedAction.functionAfterDelay(() => Game1.changeMusicTrack("grandpas_theme"), 2000);
                DelayedAction.functionAfterDelay(() =>
                {
                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:MasteryCompleteToast"));
                    Game1.playSound("newArtifact");
                }, 4000);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ClaimReward_SendMasteryCheck_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public static bool hasCompletedAllMasteryPlaques()
        public static bool HasCompletedAllMasteryPlaques_RelyOnSentChecks_Prefix(ref bool __result)
        {
            try
            {
                var skills = new[] { Skill.Farming, Skill.Foraging, Skill.Fishing, Skill.Mining, Skill.Combat };
                __result = skills.All(skill => _locationChecker.IsLocationChecked($"{skill} {MASTERY}"));
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(HasCompletedAllMasteryPlaques_RelyOnSentChecks_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
