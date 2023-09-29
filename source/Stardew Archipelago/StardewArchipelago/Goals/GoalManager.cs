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
using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Locations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace StardewArchipelago.Goals
{
    public class GoalManager
    {
        private static IMonitor _monitor;
        private IModHelper _modHelper;
        private Harmony _harmony;
        private ArchipelagoClient _archipelago;
        private LocationChecker _locationChecker;

        public GoalManager(IMonitor monitor, IModHelper modHelper, Harmony harmony, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _harmony = harmony;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            GoalCodeInjection.Initialize(_monitor, _modHelper, _archipelago, _locationChecker);
        }

        public void CheckGoalCompletion()
        {
            switch (_archipelago.SlotData.Goal)
            {
                case Goal.CommunityCenter:
                    GoalCodeInjection.CheckCommunityCenterGoalCompletion();
                    return;
                case Goal.GrandpaEvaluation:
                    GoalCodeInjection.CheckGrandpaEvaluationGoalCompletion();
                    return;
                case Goal.BottomOfMines:
                    GoalCodeInjection.CheckBottomOfTheMinesGoalCompletion();
                    return;
                case Goal.CrypticNote:
                    GoalCodeInjection.CheckCrypticNoteGoalCompletion();
                    return;
                case Goal.MasterAngler:
                    GoalCodeInjection.CheckMasterAnglerGoalCompletion();
                    return;
                case Goal.CompleteCollection:
                    GoalCodeInjection.CheckCompleteCollectionGoalCompletion();
                    return;
                case Goal.FullHouse:
                    GoalCodeInjection.CheckFullHouseGoalCompletion();
                    return;
                case Goal.GreatestWalnutHunter:
                    GoalCodeInjection.CheckWalnutHunterGoalCompletion();
                    return;
                case Goal.ProtectorOfTheValley:
                    GoalCodeInjection.CheckProtectorOfTheValleyGoalCompletion();
                    return;
                case Goal.FullShipment:
                    GoalCodeInjection.CheckFullShipmentGoalCompletion();
                    return;
                case Goal.Perfection:
                    GoalCodeInjection.CheckPerfectionGoalCompletion();
                    return;
                default:
                    throw new ArgumentOutOfRangeException($"Goal [{_archipelago.SlotData.Goal}] is not supported in this version of the mod.");
            }
        }

        public void InjectGoalMethods()
        {
            switch (_archipelago.SlotData.Goal)
            {
                case Goal.CommunityCenter:
                    InjectCommunityCenterGoalMethods();
                    return;
                case Goal.GrandpaEvaluation:
                    return;
                case Goal.BottomOfMines:
                    InjectBottomOfTheMinesGoalMethods();
                    return;
                case Goal.CrypticNote:
                    // Gets tested on quest completion
                    return;
                case Goal.MasterAngler:
                    // Gets tested on fish caught
                    return;
                case Goal.CompleteCollection:
                    // Gets tested on donation
                    return;
                case Goal.FullHouse:
                    return;
                case Goal.GreatestWalnutHunter:
                    InjectWalnutHunterGoalMethods();
                    return;
                case Goal.ProtectorOfTheValley:
                    // Gets tested when slaying monsters
                    return;
                case Goal.FullShipment:
                    // Gets tested when Shipping an item
                    return;
                case Goal.Perfection:
                    InjectPerfectionGoalMethods();
                    return;
                default:
                    throw new ArgumentOutOfRangeException($"Goal [{_archipelago.SlotData.Goal}] is not supported in this version of the mod.");
            }
        }

        private void InjectCommunityCenterGoalMethods()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(CommunityCenter), "doAreaCompleteReward"),
                postfix: new HarmonyMethod(typeof(GoalCodeInjection), nameof(GoalCodeInjection.DoAreaCompleteReward_CommunityCenterGoal_PostFix))
            );
        }

        private void InjectBottomOfTheMinesGoalMethods()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.enterMine)),
                postfix: new HarmonyMethod(typeof(GoalCodeInjection), nameof(GoalCodeInjection.EnterMine_Level120Goal_PostFix))
            );
        }

        private void InjectWalnutHunterGoalMethods()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.foundWalnut)),
                postfix: new HarmonyMethod(typeof(GoalCodeInjection), nameof(GoalCodeInjection.FounddWalnut_WalnutHunterGoal_Postfix))
            );
        }

        private void InjectPerfectionGoalMethods()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.percentGameComplete)),
                postfix: new HarmonyMethod(typeof(GoalCodeInjection), nameof(GoalCodeInjection.PercentGameComplete_PerfectionGoal_Postfix))
            );
        }
    }
}
