/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models.Enums;

namespace Archery.Framework.Models.Generic
{
    public class Condition
    {
        public enum Type
        {
            Unknown,
            CurrentChargingPercentage,
            IsUsingSpecificArrow,
            IsLoaded,
            FrameDuration,
            GameStateQuery,
            IsFiring
        }

        public Type Name { get; set; }
        public object Value { get; set; }
        public Comparison Operator { get; set; } = Comparison.EqualTo;
        public bool Inverse { get; set; }
        public bool Independent { get; set; }

        internal bool IsValid(bool booleanValue)
        {
            return IsValid(booleanValue, GetParsedValue<bool>());
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

        internal bool IsValid(string stringValue)
        {
            var passed = stringValue == GetParsedValue<string>();
            if (Inverse)
            {
                passed = !passed;
            }

            return passed;
        }

        internal bool IsValid(long numericalValue)
        {
            var passed = false;
            var comparisonValue = GetParsedValue<long>();
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
                case Comparison.GreaterThanOrEqualTo:
                    passed = numericalValue >= comparisonValue;
                    break;
                case Comparison.LessThanOrEqualTo:
                    passed = numericalValue <= comparisonValue;
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
            var comparisonValue = GetParsedValue<double>();
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
                case Comparison.GreaterThanOrEqualTo:
                    passed = numericalValue >= comparisonValue;
                    break;
                case Comparison.LessThanOrEqualTo:
                    passed = numericalValue <= comparisonValue;
                    break;
            }
            if (Inverse)
            {
                passed = !passed;
            }

            return passed;
        }

        internal T GetParsedValue<T>()
        {
            return (T)Value;
        }
    }
}
