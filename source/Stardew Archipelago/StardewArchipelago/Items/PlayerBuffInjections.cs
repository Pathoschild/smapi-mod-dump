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
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Menus;
using StardewValley.Tools;

namespace StardewArchipelago.Items
{
    public class PlayerBuffInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;

        private static int _numberOfSpeedBonuses = 0;
        private static int _numberOfLuckBonuses = 0;
        private static int _numberOfDamageBonuses = 0;
        private static int _numberOfDefenseBonuses = 0;
        private static int _numberOfImmunityBonuses = 0;
        // private static int _numberOfHealthBonuses = 0; // Handled as an unlock
        private static int _numberOfEnergyBonuses = 0;
        private static int _numberOfBiteRateBonuses = 0;
        private static int _numberOfFishTrapBonuses = 0;
        private static int _numberOfFishingBarBonuses = 0;
        // private static int _numberOfQualityBonuses = 0; // I might implement this someday
        // private static int _numberOfGlowBonuses = 0; // I might implement this someday

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
        }

        public static void CheckForApBuffs()
        {
            _numberOfSpeedBonuses = _archipelago.GetReceivedItemCount(APItem.MOVEMENT_SPEED_BONUS);
            _numberOfLuckBonuses = _archipelago.GetReceivedItemCount(APItem.LUCK_BONUS);
            _numberOfDamageBonuses = _archipelago.GetReceivedItemCount(APItem.DAMAGE_BONUS);
            _numberOfDefenseBonuses = _archipelago.GetReceivedItemCount(APItem.DEFENSE_BONUS);
            _numberOfImmunityBonuses = _archipelago.GetReceivedItemCount(APItem.IMMUNITY_BONUS);
            _numberOfEnergyBonuses = _archipelago.GetReceivedItemCount(APItem.ENERGY_BONUS);
            _numberOfBiteRateBonuses = _archipelago.GetReceivedItemCount(APItem.BITE_RATE_BONUS);
            _numberOfFishTrapBonuses = _archipelago.GetReceivedItemCount(APItem.FISH_TRAP_BONUS);
            _numberOfFishingBarBonuses = _archipelago.GetReceivedItemCount(APItem.FISHING_BAR_SIZE_BONUS);
            // _numberOfQualityBonuses = _archipelago.GetReceivedItemCount(APItem.QUALITY_BONUS);
            // _numberOfGlowBonuses = _archipelago.GetReceivedItemCount(APItem.GLOW_BONUS);
        }

        public static void GetMovementSpeed_AddApBuffs_Postfix(Farmer __instance, ref float __result)
        {
            try
            {
                if (Game1.eventUp && Game1.CurrentEvent is { isFestival: false })
                {
                    return;
                }

                var baseCoefficient = 1.0f;
                var configValue = ModEntry.Instance.Config.BonusPerMovementSpeed;
                var valuePerMovementSpeed = 0.05f * configValue;
                var totalCoefficient = baseCoefficient + (valuePerMovementSpeed * _numberOfSpeedBonuses);

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

        // public float AttackMultiplier => this.GetValues().AttackMultiplier.Value;
        public static void GetAttackMultiplier_AddApBuffs_Postfix(BuffManager __instance, ref float __result)
        {
            try
            {
                var baseCoefficient = 1.0f;
                var totalCoefficient = baseCoefficient + (0.1f * _numberOfDamageBonuses);

                __result *= totalCoefficient;
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetAttackMultiplier_AddApBuffs_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public int Defense => (int) this.GetValues().Defense.Value;
        public static void GetDefense_AddApBuffs_Postfix(BuffManager __instance, ref int __result)
        {
            try
            {
                var bonusDefense = (1 * _numberOfDefenseBonuses);
                __result += bonusDefense;
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetDefense_AddApBuffs_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public int Immunity => (int) this.GetValues().Immunity.Value;
        public static void GetImmunity_AddApBuffs_Postfix(BuffManager __instance, ref int __result)
        {
            try
            {
                var bonusImmunity = (1 * _numberOfImmunityBonuses);
                __result += bonusImmunity;
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetImmunity_AddApBuffs_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public int MaxStamina => (int)this.GetValues().MaxStamina.Value;
        public static void GetMaxStamina_AddApBuffs_Postfix(BuffManager __instance, ref int __result)
        {
            try
            {
                var bonusStamina = (12 * _numberOfEnergyBonuses);
                __result += bonusStamina;
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetMaxStamina_AddApBuffs_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // private float calculateTimeUntilFishingBite(Vector2 bobberTile, bool isFirstCast, Farmer who)
        public static void CalculateTimeUntilFishingBite_AddApBuffs_Postfix(FishingRod __instance, Vector2 bobberTile, bool isFirstCast, Farmer who, ref float __result)
        {
            try
            {
                var biteTimeReduction = (1000 * _numberOfBiteRateBonuses);
                __result = Math.Max(300f, __result - biteTimeReduction);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CalculateTimeUntilFishingBite_AddApBuffs_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public BobberBar(string whichFish, float fishSize, bool treasure, List<string> bobbers, string setFlagOnCatch, bool isBossFish, string baitID = "", bool goldenTreasure = false)
        public static void BobberBarConstructor_AddApBuffs_Postfix(BobberBar __instance, string whichFish, float fishSize, bool treasure, 
            List<string> bobbers, string setFlagOnCatch, bool isBossFish, string baitID, bool goldenTreasure)
        {
            try
            {
                AddTrapBuff(__instance);
                AddFishingBarBuff(__instance);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(BobberBarConstructor_AddApBuffs_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void AddTrapBuff(BobberBar bar)
        {
            var trapMultiplier = (float)Math.Pow(0.8f, _numberOfFishTrapBonuses);
            bar.distanceFromCatchPenaltyModifier *= trapMultiplier;
        }

        private static void AddFishingBarBuff(BobberBar bar)
        {
            var fishingBarSizeIncrease = (10 * _numberOfFishingBarBonuses);
            bar.bobberBarHeight += fishingBarSizeIncrease;
            bar.bobberBarPos = (568 - bar.bobberBarHeight);
        }
    }
}
