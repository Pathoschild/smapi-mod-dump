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
using Microsoft.Xna.Framework.Content;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections
{
    public class SpecialOrderInjections
    {
        private static readonly string[] _ignoredSpecialOrders =
        {
            // Add ignored orders here
        };

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static ContentManager _englishContentManager;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _englishContentManager =
                new ContentManager(Game1.game1.Content.ServiceProvider, Game1.game1.Content.RootDirectory);
        }

        // public static bool IsSpecialOrdersBoardUnlocked()
        public static bool IsSpecialOrdersBoardUnlocked_UnlockBasedOnApItem_Prefix(ref bool __result)
        {
            try
            {
                __result = _archipelago.HasReceivedItem(UnlockManager.SPECIAL_ORDER_BOARD_AP_NAME);
                return false; // don't run original logic;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(IsSpecialOrdersBoardUnlocked_UnlockBasedOnApItem_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic;
            }
        }

        // public static SpecialOrder GetSpecialOrder(string key, int? generation_seed)
        public static void GetSpecialOrder_ArchipelagoReward_Postfix(string key, int? generation_seed, ref SpecialOrder __result)
        {
            try
            {
                var specialOrderName = __result.GetName();
                if (_ignoredSpecialOrders.Contains(specialOrderName))
                {
                    return;
                }

                // Remove vanilla rewards if the player has not received the check.
                // We will keep vanilla rewards for repeated orders
                if (_locationChecker.IsLocationMissingAndExists(specialOrderName))
                {
                    __result.rewards.Clear();
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetSpecialOrder_ArchipelagoReward_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        // public void CheckCompletion()
        public static void CheckCompletion_ArchipelagoReward_Postfix(SpecialOrder __instance)
        {
            try
            {
                if (__instance.questState.Value != SpecialOrder.QuestState.Complete)
                {
                    return;
                }

                var specialOrderName = __instance.GetName();
                if (_ignoredSpecialOrders.Contains(specialOrderName))
                {
                    return;
                }

                _locationChecker.AddCheckedLocation(specialOrderName);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckCompletion_ArchipelagoReward_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}