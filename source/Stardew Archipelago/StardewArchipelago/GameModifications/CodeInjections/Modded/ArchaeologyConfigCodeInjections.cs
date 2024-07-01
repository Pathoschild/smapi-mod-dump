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
    internal class ArchaeologyConfigCodeInjections
    {
        private const double ARTIFACT_SPOT_MULTIPLIER = 1.6; // Default Value: 10
        private const double PANNING_MULTIPLIER = 1.5; // Default Value: 20
        private const double DIGGING_MULTIPLIER = 1.6; // Default Value: 5
        private const double WATER_SHIFTER_MULTIPLIER = 1.8; // Default Value: 2

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
        }

        // public int ExperienceFromArtifactSpots { get; set; } = 10;
        public static void ExperienceFromArtifactSpots_APMultiplier_Postfix(object __instance, ref int __result)
        {
            try
            {
                __result = (int)(__result * ARTIFACT_SPOT_MULTIPLIER);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ExperienceFromArtifactSpots_APMultiplier_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public int ExperienceFromPanSpots { get; set; } = 20;
        public static void ExperienceFromPanSpots_APMultiplier_Postfix(object __instance, ref int __result)
        {
            try
            {
                __result = (int)(__result * PANNING_MULTIPLIER);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ExperienceFromPanSpots_APMultiplier_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public int ExperienceFromMinesDigging { get; set; } = 5;
        public static void ExperienceFromMinesDigging_APMultiplier_Postfix(object __instance, ref int __result)
        {
            try
            {
                __result = (int)(__result * DIGGING_MULTIPLIER);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ExperienceFromMinesDigging_APMultiplier_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public int ExperienceFromWaterShifter { get; set; } = 2;
        public static void ExperienceFromWaterShifter_APMultiplier_Postfix(object __instance, ref int __result)
        {
            try
            {
                __result = (int)(__result * WATER_SHIFTER_MULTIPLIER);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ExperienceFromWaterShifter_APMultiplier_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
