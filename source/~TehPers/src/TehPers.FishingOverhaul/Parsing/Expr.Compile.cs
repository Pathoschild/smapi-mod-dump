/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/


// @generated

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace TehPers.FishingOverhaul.Parsing
{
	partial record Expr<T> {
		public bool TryCompile([MaybeNullWhen(false)] out Expression<Func<T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<T>>(body);
            return true;
        }

		public bool TryCompile(string arg0, [MaybeNullWhen(false)] out Expression<Func<double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, T>>(body, arg0Param);
            return true;
        }

		public bool TryCompile(string arg0, string arg1, [MaybeNullWhen(false)] out Expression<Func<double, double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            var arg1Param = Expression.Parameter(typeof(double), arg1);
            variables.Add(arg1, arg1Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, double, T>>(body, arg0Param, arg1Param);
            return true;
        }

		public bool TryCompile(string arg0, string arg1, string arg2, [MaybeNullWhen(false)] out Expression<Func<double, double, double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            var arg1Param = Expression.Parameter(typeof(double), arg1);
            variables.Add(arg1, arg1Param);
            var arg2Param = Expression.Parameter(typeof(double), arg2);
            variables.Add(arg2, arg2Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, double, double, T>>(body, arg0Param, arg1Param, arg2Param);
            return true;
        }

		public bool TryCompile(string arg0, string arg1, string arg2, string arg3, [MaybeNullWhen(false)] out Expression<Func<double, double, double, double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            var arg1Param = Expression.Parameter(typeof(double), arg1);
            variables.Add(arg1, arg1Param);
            var arg2Param = Expression.Parameter(typeof(double), arg2);
            variables.Add(arg2, arg2Param);
            var arg3Param = Expression.Parameter(typeof(double), arg3);
            variables.Add(arg3, arg3Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, double, double, double, T>>(body, arg0Param, arg1Param, arg2Param, arg3Param);
            return true;
        }

		public bool TryCompile(string arg0, string arg1, string arg2, string arg3, string arg4, [MaybeNullWhen(false)] out Expression<Func<double, double, double, double, double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            var arg1Param = Expression.Parameter(typeof(double), arg1);
            variables.Add(arg1, arg1Param);
            var arg2Param = Expression.Parameter(typeof(double), arg2);
            variables.Add(arg2, arg2Param);
            var arg3Param = Expression.Parameter(typeof(double), arg3);
            variables.Add(arg3, arg3Param);
            var arg4Param = Expression.Parameter(typeof(double), arg4);
            variables.Add(arg4, arg4Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, double, double, double, double, T>>(body, arg0Param, arg1Param, arg2Param, arg3Param, arg4Param);
            return true;
        }

		public bool TryCompile(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, [MaybeNullWhen(false)] out Expression<Func<double, double, double, double, double, double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            var arg1Param = Expression.Parameter(typeof(double), arg1);
            variables.Add(arg1, arg1Param);
            var arg2Param = Expression.Parameter(typeof(double), arg2);
            variables.Add(arg2, arg2Param);
            var arg3Param = Expression.Parameter(typeof(double), arg3);
            variables.Add(arg3, arg3Param);
            var arg4Param = Expression.Parameter(typeof(double), arg4);
            variables.Add(arg4, arg4Param);
            var arg5Param = Expression.Parameter(typeof(double), arg5);
            variables.Add(arg5, arg5Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, double, double, double, double, double, T>>(body, arg0Param, arg1Param, arg2Param, arg3Param, arg4Param, arg5Param);
            return true;
        }

		public bool TryCompile(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, [MaybeNullWhen(false)] out Expression<Func<double, double, double, double, double, double, double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            var arg1Param = Expression.Parameter(typeof(double), arg1);
            variables.Add(arg1, arg1Param);
            var arg2Param = Expression.Parameter(typeof(double), arg2);
            variables.Add(arg2, arg2Param);
            var arg3Param = Expression.Parameter(typeof(double), arg3);
            variables.Add(arg3, arg3Param);
            var arg4Param = Expression.Parameter(typeof(double), arg4);
            variables.Add(arg4, arg4Param);
            var arg5Param = Expression.Parameter(typeof(double), arg5);
            variables.Add(arg5, arg5Param);
            var arg6Param = Expression.Parameter(typeof(double), arg6);
            variables.Add(arg6, arg6Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, double, double, double, double, double, double, T>>(body, arg0Param, arg1Param, arg2Param, arg3Param, arg4Param, arg5Param, arg6Param);
            return true;
        }

		public bool TryCompile(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, [MaybeNullWhen(false)] out Expression<Func<double, double, double, double, double, double, double, double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            var arg1Param = Expression.Parameter(typeof(double), arg1);
            variables.Add(arg1, arg1Param);
            var arg2Param = Expression.Parameter(typeof(double), arg2);
            variables.Add(arg2, arg2Param);
            var arg3Param = Expression.Parameter(typeof(double), arg3);
            variables.Add(arg3, arg3Param);
            var arg4Param = Expression.Parameter(typeof(double), arg4);
            variables.Add(arg4, arg4Param);
            var arg5Param = Expression.Parameter(typeof(double), arg5);
            variables.Add(arg5, arg5Param);
            var arg6Param = Expression.Parameter(typeof(double), arg6);
            variables.Add(arg6, arg6Param);
            var arg7Param = Expression.Parameter(typeof(double), arg7);
            variables.Add(arg7, arg7Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, double, double, double, double, double, double, double, T>>(body, arg0Param, arg1Param, arg2Param, arg3Param, arg4Param, arg5Param, arg6Param, arg7Param);
            return true;
        }

		public bool TryCompile(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, [MaybeNullWhen(false)] out Expression<Func<double, double, double, double, double, double, double, double, double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            var arg1Param = Expression.Parameter(typeof(double), arg1);
            variables.Add(arg1, arg1Param);
            var arg2Param = Expression.Parameter(typeof(double), arg2);
            variables.Add(arg2, arg2Param);
            var arg3Param = Expression.Parameter(typeof(double), arg3);
            variables.Add(arg3, arg3Param);
            var arg4Param = Expression.Parameter(typeof(double), arg4);
            variables.Add(arg4, arg4Param);
            var arg5Param = Expression.Parameter(typeof(double), arg5);
            variables.Add(arg5, arg5Param);
            var arg6Param = Expression.Parameter(typeof(double), arg6);
            variables.Add(arg6, arg6Param);
            var arg7Param = Expression.Parameter(typeof(double), arg7);
            variables.Add(arg7, arg7Param);
            var arg8Param = Expression.Parameter(typeof(double), arg8);
            variables.Add(arg8, arg8Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, double, double, double, double, double, double, double, double, T>>(body, arg0Param, arg1Param, arg2Param, arg3Param, arg4Param, arg5Param, arg6Param, arg7Param, arg8Param);
            return true;
        }

		public bool TryCompile(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, [MaybeNullWhen(false)] out Expression<Func<double, double, double, double, double, double, double, double, double, double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            var arg1Param = Expression.Parameter(typeof(double), arg1);
            variables.Add(arg1, arg1Param);
            var arg2Param = Expression.Parameter(typeof(double), arg2);
            variables.Add(arg2, arg2Param);
            var arg3Param = Expression.Parameter(typeof(double), arg3);
            variables.Add(arg3, arg3Param);
            var arg4Param = Expression.Parameter(typeof(double), arg4);
            variables.Add(arg4, arg4Param);
            var arg5Param = Expression.Parameter(typeof(double), arg5);
            variables.Add(arg5, arg5Param);
            var arg6Param = Expression.Parameter(typeof(double), arg6);
            variables.Add(arg6, arg6Param);
            var arg7Param = Expression.Parameter(typeof(double), arg7);
            variables.Add(arg7, arg7Param);
            var arg8Param = Expression.Parameter(typeof(double), arg8);
            variables.Add(arg8, arg8Param);
            var arg9Param = Expression.Parameter(typeof(double), arg9);
            variables.Add(arg9, arg9Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, double, double, double, double, double, double, double, double, double, T>>(body, arg0Param, arg1Param, arg2Param, arg3Param, arg4Param, arg5Param, arg6Param, arg7Param, arg8Param, arg9Param);
            return true;
        }

		public bool TryCompile(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, [MaybeNullWhen(false)] out Expression<Func<double, double, double, double, double, double, double, double, double, double, double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            var arg1Param = Expression.Parameter(typeof(double), arg1);
            variables.Add(arg1, arg1Param);
            var arg2Param = Expression.Parameter(typeof(double), arg2);
            variables.Add(arg2, arg2Param);
            var arg3Param = Expression.Parameter(typeof(double), arg3);
            variables.Add(arg3, arg3Param);
            var arg4Param = Expression.Parameter(typeof(double), arg4);
            variables.Add(arg4, arg4Param);
            var arg5Param = Expression.Parameter(typeof(double), arg5);
            variables.Add(arg5, arg5Param);
            var arg6Param = Expression.Parameter(typeof(double), arg6);
            variables.Add(arg6, arg6Param);
            var arg7Param = Expression.Parameter(typeof(double), arg7);
            variables.Add(arg7, arg7Param);
            var arg8Param = Expression.Parameter(typeof(double), arg8);
            variables.Add(arg8, arg8Param);
            var arg9Param = Expression.Parameter(typeof(double), arg9);
            variables.Add(arg9, arg9Param);
            var arg10Param = Expression.Parameter(typeof(double), arg10);
            variables.Add(arg10, arg10Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, double, double, double, double, double, double, double, double, double, double, T>>(body, arg0Param, arg1Param, arg2Param, arg3Param, arg4Param, arg5Param, arg6Param, arg7Param, arg8Param, arg9Param, arg10Param);
            return true;
        }

		public bool TryCompile(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, string arg11, [MaybeNullWhen(false)] out Expression<Func<double, double, double, double, double, double, double, double, double, double, double, double, T>> result, [MaybeNullWhen(true)] out HashSet<string> missingVariables)
        {
            var variables = new Dictionary<string, ParameterExpression>();
            var arg0Param = Expression.Parameter(typeof(double), arg0);
            variables.Add(arg0, arg0Param);
            var arg1Param = Expression.Parameter(typeof(double), arg1);
            variables.Add(arg1, arg1Param);
            var arg2Param = Expression.Parameter(typeof(double), arg2);
            variables.Add(arg2, arg2Param);
            var arg3Param = Expression.Parameter(typeof(double), arg3);
            variables.Add(arg3, arg3Param);
            var arg4Param = Expression.Parameter(typeof(double), arg4);
            variables.Add(arg4, arg4Param);
            var arg5Param = Expression.Parameter(typeof(double), arg5);
            variables.Add(arg5, arg5Param);
            var arg6Param = Expression.Parameter(typeof(double), arg6);
            variables.Add(arg6, arg6Param);
            var arg7Param = Expression.Parameter(typeof(double), arg7);
            variables.Add(arg7, arg7Param);
            var arg8Param = Expression.Parameter(typeof(double), arg8);
            variables.Add(arg8, arg8Param);
            var arg9Param = Expression.Parameter(typeof(double), arg9);
            variables.Add(arg9, arg9Param);
            var arg10Param = Expression.Parameter(typeof(double), arg10);
            variables.Add(arg10, arg10Param);
            var arg11Param = Expression.Parameter(typeof(double), arg11);
            variables.Add(arg11, arg11Param);
            missingVariables = new();
            if (!this.TryCompile(variables, missingVariables, out var body))
            {
                result = default;
                return false;
            }

            result = Expression.Lambda<Func<double, double, double, double, double, double, double, double, double, double, double, double, T>>(body, arg0Param, arg1Param, arg2Param, arg3Param, arg4Param, arg5Param, arg6Param, arg7Param, arg8Param, arg9Param, arg10Param, arg11Param);
            return true;
        }

	}
}
