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
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Items
{
    public class PlayerBuffInjections
    {
        private const string MOVEMENT_SPEED_AP_LOCATION = "Movement Speed Bonus";
        private const string LUCK_AP_LOCATION = "Luck Bonus";

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;

        private static int _numberOfSpeedBonuses = 0;
        private static int _numberOfLuckBonuses = 0;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
        }

        public static void CheckForApBuffs()
        {
            _numberOfSpeedBonuses = _archipelago.GetReceivedItemCount(MOVEMENT_SPEED_AP_LOCATION);
            _numberOfLuckBonuses = _archipelago.GetReceivedItemCount(LUCK_AP_LOCATION);
        }

        public static void GetMovementSpeed_AddApBuffs_Postfix(Farmer __instance, ref float __result)
        {
            try
            {
                var baseCoefficient = 1.0f;
                var totalCoefficient = baseCoefficient + (0.25f * _numberOfSpeedBonuses);

                __result *= totalCoefficient;
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetMovementSpeed_AddApBuffs_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static void DailyLuck_AddApBuffs_Postfix(Farmer __instance, ref double __result)
        {
            try
            {
                var totalBonus = 0.025f * _numberOfLuckBonuses;

                __result += totalBonus;
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(DailyLuck_AddApBuffs_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
