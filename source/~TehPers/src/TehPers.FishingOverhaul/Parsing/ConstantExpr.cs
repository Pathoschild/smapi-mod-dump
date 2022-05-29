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
    /// <summary>
    /// A numeric expression.
    /// </summary>
    /// <param name="Value">The value of this expression.</param>
    internal record ConstantExpr<T>(T Value) : Expr<T>
    {
        public override bool TryEvaluate(
            IDictionary<string, double> variables,
            HashSet<string> missingVariables,
            [MaybeNullWhen(false)] out T result
        )
        {
            result = this.Value;
            return true;
        }

        public override bool TryCompile(
            Dictionary<string, ParameterExpression> variables,
            HashSet<string> missingVariables,
            [MaybeNullWhen(false)] out Expression result
        )
        {
            result = Expression.Constant(this.Value);
            return true;
        }

        public override string ToString()
        {
            return this.Value?.ToString() ?? "<null>";
        }
    }
}
