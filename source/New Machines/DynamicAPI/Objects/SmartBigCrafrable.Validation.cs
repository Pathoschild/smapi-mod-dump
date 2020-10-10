/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.Diagnostics;
using EnsureThat;
using Igorious.StardewValley.DynamicAPI.Utils;
using StardewValley;
using XColor = Microsoft.Xna.Framework.Color;

namespace Igorious.StardewValley.DynamicAPI.Objects
{
    public static class ValidationExtensions
    {
        [DebuggerStepThrough]
        public static Param<T> Must<T>(this Param<T> param, Func<T, bool> predicate, string message)
        {
            if (!Ensure.IsActive || predicate(param.Value)) return param;
            throw ExceptionFactory.CreateForParamValidation(param, string.Format(message, param.Value));
        }

        [DebuggerStepThrough]
        public static Param<T> IsNullOr<T>(this Param<T> param, Func<Param<T>, Param<T>> orAction)
        {
            if (!Ensure.IsActive || param.Value == null) return param;
            return orAction(param);
        }

        [DebuggerStepThrough]
        public static Param<string> IsNotWhiteSpace(this Param<string> param)
        {
            if (!Ensure.IsActive || param.Value == null || !string.IsNullOrWhiteSpace(param.Value)) return param;
            throw ExceptionFactory.CreateForParamValidation(param, $"String '{param.Value}' must be not empty!");
        }
            
        [DebuggerStepThrough]
        public static Param<T?> IsGt<T>(this Param<T?> param, T limit) where T : struct, IComparable<T>
        {
            if (!Ensure.IsActive || param.Value != null && param.Value.Value.CompareTo(limit) > 0) return param;
            throw ExceptionFactory.CreateForParamValidation(param, string.Format(ExceptionMessages.EnsureExtensions_IsNotGt, param.Value, limit));
        }

        [DebuggerStepThrough]
        public static Param<T?> IsGte<T>(this Param<T?> param, T limit) where T : struct, IComparable<T>
        {
            if (!Ensure.IsActive || param.Value != null && param.Value.Value.CompareTo(limit) >= 0) return param;
            throw ExceptionFactory.CreateForParamValidation(param, string.Format(ExceptionMessages.EnsureExtensions_IsNotGte, param.Value, limit));
        }
    }

    partial class SmartBigCrafrableBase
    {
        private static class PutItemValidator
        {
            [Conditional("DEBUG")]
            public static void Validate(int itemID, int count, int itemQuality, string overridedName, int? overridedPrice, XColor? color)
            {
                try
                {
                    Ensure.That(itemID, nameof(itemID)).Must(BeRegistered, "Item {0} must be registered!");
                    Ensure.That(count, nameof(count)).IsGt(0);
                    Ensure.That(itemQuality, nameof(itemQuality)).IsInRange(0, 3);
                    Ensure.That(overridedName, nameof(overridedName)).IsNullOr(x => x.IsNotWhiteSpace());
                    Ensure.That(overridedPrice, nameof(overridedPrice)).IsNullOr(x => x.IsGte(0));
                }
                catch (Exception e)
                {
                    Log.Info($"Error during PutItem({nameof(itemID)}: {itemID}, {nameof(count)}: {count}, {nameof(itemQuality)}: {itemQuality}, {nameof(overridedName)}: {overridedName}, {nameof(overridedPrice)}: {overridedPrice})");                   
                    Log.Error(e.ToString());
                    throw;
                }
            }

            private static bool BeRegistered(int itemID)
            {
                return Game1.objectInformation.ContainsKey(itemID);
            }
        }
    }
}