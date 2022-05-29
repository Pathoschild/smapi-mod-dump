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
    /// A parsed expression.
    /// </summary>
    internal abstract partial record Expr<T>
    {
        /// <summary>
        /// this expression.
        /// </summary>
        /// <returns>The optimized expression.</returns>
        public virtual Expr<T> Optimized()
        {
            return this;
        }

        /// <summary>
        /// Tries to evaluate this expression.
        /// </summary>
        /// <param name="variables">The values of variables within this expression.</param>
        /// <param name="missingVariables">The set of variables that had no value during evaluation.</param>
        /// <param name="result">The result of evaluating this expression.</param>
        /// <returns></returns>
        public abstract bool TryEvaluate(
            IDictionary<string, double> variables,
            HashSet<string> missingVariables,
            [MaybeNullWhen(false)] out T result
        );

        /// <summary>
        /// Tries to compile this expression.
        /// </summary>
        /// <param name="variables">The parameters that represent the variables within this expression.</param>
        /// <param name="missingVariables">The set of variables that had no value during evaluation.</param>
        /// <param name="result">The result of evaluating this expression.</param>
        /// <returns></returns>
        public abstract bool TryCompile(
            Dictionary<string, ParameterExpression> variables,
            HashSet<string> missingVariables,
            [MaybeNullWhen(false)] out Expression result
        );
    }
}
