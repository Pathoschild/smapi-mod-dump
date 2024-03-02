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

namespace StardewArchipelago.GameModifications.CodeInjections.Modded
{
    internal class SocializingConfigCodeInjections
    {
        private const double TALKING_MULTIPLIER = 1.8;
        private const double GIFT_MULTIPLIER = 1.6;
        private const double EVENT_MULTIPLIER = 1.6;
        private const double QUEST_MULTIPLIER = 1.6;
        private const double LOVED_GIFT_MULTIPLIER = 1.2;
        private const double BIRTHDAY_GIFT_MULTIPLIER = 1.2;

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
        }

        // public int ExperienceFromTalking { get; set; } = 2;
        public static void ExperienceFromTalking_APMultiplier_Postfix(object __instance, ref int __result)
        {
            try
            {
                __result = (int)(__result * TALKING_MULTIPLIER);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ExperienceFromTalking_APMultiplier_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public int ExperienceFromGifts { get; set; } = 5;
        public static void ExperienceFromGifts_APMultiplier_Postfix(object __instance, ref int __result)
        {
            try
            {
                __result = (int)(__result * GIFT_MULTIPLIER);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ExperienceFromGifts_APMultiplier_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public int ExperienceFromEvents { get; set; } = 10;
        public static void ExperienceFromEvents_APMultiplier_Postfix(object __instance, ref int __result)
        {
            try
            {
                __result = (int)(__result * EVENT_MULTIPLIER);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ExperienceFromEvents_APMultiplier_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public int ExperienceFromQuests { get; set; } = 20;
        public static void ExperienceFromQuests_APMultiplier_Postfix(object __instance, ref int __result)
        {
            try
            {
                __result = (int)(__result * QUEST_MULTIPLIER);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ExperienceFromQuests_APMultiplier_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public float LovedGiftExpMultiplier { get; set; } = 2f;
        public static void LovedGiftExpMultiplier_APMultiplier_Postfix(object __instance, ref float __result)
        {
            try
            {
                __result = (float)(__result * LOVED_GIFT_MULTIPLIER);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(LovedGiftExpMultiplier_APMultiplier_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public float BirthdayGiftExpMultiplier { get; set; } = 2f;
        public static void BirthdayGiftExpMultiplier_APMultiplier_Postfix(object __instance, ref float __result)
        {
            try
            {
                __result = (float)(__result * BIRTHDAY_GIFT_MULTIPLIER);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(BirthdayGiftExpMultiplier_APMultiplier_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
