/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Models.Appearances.Generic.Random;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FashionSense.Framework.Models.Appearances.Generic
{
    public class Condition
    {
        public enum Comparison
        {
            EqualTo,
            GreaterThan,
            LessThan
        }

        public enum Type
        {
            Unknown,
            MovementSpeed,
            MovementDuration,
            MovementSpeedLogical,
            MovementDurationLogical,
            IsElapsedTimeMultipleOf,
            DidPreviousFrameDisplay,
            RidingHorse,
            InventoryItemCount,
            IsInventoryFull,
            IsDarkOut,
            IsRaining,
            IsSnowing,
            IsWalking,
            IsRunning,
            IsEating,
            IsDrinking,
            IsCasting,
            IsFishing,
            IsNibbling,
            IsReeling,
            IsPullingFishOutOfWater,
            ToolChargeLevel,
            IsUsingHeavyTool,
            IsUsingMilkPail,
            IsUsingShears,
            IsUsingPan,
            IsUsingScythe,
            IsWatering,
            IsUsingMeleeWeapon,
            IsUsingSlingshot,
            IsUsingDagger,
            IsHarvesting,
            IsInMines,
            IsOutdoors,
            HealthLevel,
            StaminaLevel,
            IsSitting,
            IsCarrying,
            IsSwimming,
            IsInBathingSuit,
            IsSick,
            IsPassingOut,
            CurrentFarmerFrame,
            RandomChance,
            GameStateQuery
        }

        public Type Name { get; set; }
        public string GroupName { get; set; }
        public object Value { get; set; }
        public Comparison Operator { get; set; } = Comparison.EqualTo;
        public bool Inverse { get; set; }
        public bool Independent { get; set; }

        private object ParsedValue { get; set; }
        private object Cache { get; set; }

        internal bool IsValid(bool booleanValue)
        {
            return IsValid(booleanValue, GetParsedValue<bool>(recalculateValue: false));
        }

        internal bool IsValid(bool booleanValue, bool comparisonValue)
        {
            var passed = booleanValue == comparisonValue;
            if (Inverse)
            {
                passed = !passed;
            }

            return passed;
        }

        internal bool IsValid(long numericalValue)
        {
            var passed = false;
            var comparisonValue = GetParsedValue<long>(recalculateValue: false);
            switch (Operator)
            {
                case Comparison.EqualTo:
                    passed = numericalValue == comparisonValue;
                    break;
                case Comparison.GreaterThan:
                    passed = numericalValue > comparisonValue;
                    break;
                case Comparison.LessThan:
                    passed = numericalValue < comparisonValue;
                    break;
            }
            if (Inverse)
            {
                passed = !passed;
            }

            return passed;
        }

        internal bool IsValid(double numericalValue)
        {
            var passed = false;
            var comparisonValue = GetParsedValue<double>(recalculateValue: false);
            switch (Operator)
            {
                case Comparison.EqualTo:
                    passed = numericalValue == comparisonValue;
                    break;
                case Comparison.GreaterThan:
                    passed = numericalValue > comparisonValue;
                    break;
                case Comparison.LessThan:
                    passed = numericalValue < comparisonValue;
                    break;
            }
            if (Inverse)
            {
                passed = !passed;
            }

            return passed;
        }

        internal T GetParsedValue<T>(bool recalculateValue = false)
        {
            if (ParsedValue is null || recalculateValue)
            {
                if (Value is JObject modelContext)
                {
                    if (modelContext["RandomRange"] != null)
                    {
                        var randomRange = JsonConvert.DeserializeObject<RandomRange>(modelContext["RandomRange"].ToString());

                        ParsedValue = (T)Convert.ChangeType(Game1.random.Next(randomRange.Min, randomRange.Max), typeof(T));
                    }
                    else if (modelContext["RandomValue"] != null)
                    {
                        var randomValue = JsonConvert.DeserializeObject<List<object>>(modelContext["RandomValue"].ToString());
                        ParsedValue = (T)Convert.ChangeType(randomValue[Game1.random.Next(randomValue.Count)], typeof(T));
                    }
                }
                else
                {
                    ParsedValue = Value;
                }
            }

            return (T)ParsedValue;
        }

        internal T GetCache<T>()
        {
            if (Cache is null)
            {
                return default;
            }

            return (T)Cache;
        }

        internal void SetCache(object value)
        {
            Cache = value;
        }
    }
}