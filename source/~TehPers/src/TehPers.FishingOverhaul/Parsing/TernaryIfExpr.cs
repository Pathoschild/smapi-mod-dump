/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace TehPers.FishingOverhaul.Parsing
{
    internal record TernaryIfExpr<T>(Expr<bool> Condition, Expr<T> True, Expr<T> False) : Expr<T>
    {
        public override bool TryEvaluate(
            IDictionary<string, double> variables,
            HashSet<string> missingVariables,
            [MaybeNullWhen(false)] out T result
        )
        {
            if (!this.Condition.TryEvaluate(variables, missingVariables, out var condition))
            {
                result = default;
                return false;
            }

            switch (condition)
            {
                case true when this.True.TryEvaluate(variables, missingVariables, out result):
                case false when this.False.TryEvaluate(variables, missingVariables, out result):
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        public override bool TryCompile(
            Dictionary<string, ParameterExpression> variables,
            HashSet<string> missingVariables,
            [MaybeNullWhen(false)] out Expression result
        )
        {
            if (!this.Condition.TryCompile(variables, missingVariables, out var condition)
                || !this.True.TryCompile(variables, missingVariables, out var trueBranch)
                || !this.False.TryCompile(variables, missingVariables, out var falseBranch))
            {
                result = default;
                return false;
            }

            result = Expression.Condition(condition, trueBranch, falseBranch);
            return true;
        }
    }
}
