/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace TehPers.FishingOverhaul.Parsing
{
    internal record UnaryExpr(UnaryOperator Operator, Expr<double> Inner) : Expr<double>
    {
        public override Expr<double> Optimized()
        {
            var inner = this.Inner.Optimized();
            return inner switch
            {
                // Constant folding
                // -<number> => <-number>
                ConstantExpr<double>(var value) when this.Operator == UnaryOperator.Negate =>
                    new ConstantExpr<double>(-value),

                // Double negative
                // -(-<expr>) => <expr>
                UnaryExpr(UnaryOperator.Negate, UnaryExpr(UnaryOperator.Negate, var x)) => x,

                // Fallback
                _ => new UnaryExpr(this.Operator, inner),
            };
        }

        public override bool TryEvaluate(
            IDictionary<string, double> variables,
            HashSet<string> missingVariables,
            out double result
        )
        {
            if (!this.Inner.TryEvaluate(variables, missingVariables, out var inner))
            {
                result = default;
                return false;
            }

            result = this.Operator switch
            {
                UnaryOperator.Negate => -inner,
                _ => throw new InvalidOperationException($"Unknown operator {this.Operator}"),
            };
            return true;
        }

        public override bool TryCompile(
            Dictionary<string, ParameterExpression> variables,
            HashSet<string> missingVariables,
            [MaybeNullWhen(false)] out Expression result
        )
        {
            if (!this.Inner.TryCompile(variables, missingVariables, out var inner))
            {
                result = default;
                return false;
            }

            result = this.Operator switch
            {
                UnaryOperator.Negate => Expression.Negate(inner),
                _ => throw new InvalidOperationException($"Unknown operator {this.Operator}"),
            };
            return true;
        }

        public override string ToString()
        {
            return this.Operator switch
            {
                UnaryOperator.Negate when this.Inner is ConstantExpr<double> or IdentExpr =>
                    $"-{this.Inner}",
                UnaryOperator.Negate => $"-({this.Inner})",
                _ => base.ToString()
            };
        }
    }
}
