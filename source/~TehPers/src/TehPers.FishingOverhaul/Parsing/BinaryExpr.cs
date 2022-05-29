/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace TehPers.FishingOverhaul.Parsing
{
    /// <summary>
    /// A binary operation.
    /// </summary>
    /// <param name="Operator">The operator to apply.</param>
    /// <param name="Left">The expression on the left side of the operator.</param>
    /// <param name="Right">The expression on the right side of the operator.</param>
    internal record BinaryExpr(
        BinaryOperator Operator,
        Expr<double> Left,
        Expr<double> Right
    ) : Expr<double>
    {
        public override Expr<double> Optimized()
        {
            // Optimize inner
            var left = this.Left.Optimized();
            var right = this.Right.Optimized();
            var op = this.Operator;

            return (op, left, right) switch
            {
                // ** Pre-optimization **

                // Subtraction -> addition
                // l - r:const => l + -r
                (BinaryOperator.Subtract, var l, ConstantExpr<double>(var r)) => new BinaryExpr(
                    BinaryOperator.Add,
                    l,
                    new ConstantExpr<double>(-r)
                ).Optimized(),

                // Division -> multiplication
                // l / r:const => l * (1 / r)
                (BinaryOperator.Divide, var l, ConstantExpr<double>(var r)) => new BinaryExpr(
                    BinaryOperator.Multiply,
                    l,
                    new ConstantExpr<double>(1 / r)
                ).Optimized(),

                // Put numbers before expressions
                // l:!const <+*> r:const => r <+*> l
                (BinaryOperator.Add or BinaryOperator.Multiply, var l and not ConstantExpr<double>,
                    ConstantExpr<double> r) => new BinaryExpr(op, r, l).Optimized(),

                // ** Basic properties **

                // Identity
                // l + 0 => l
                (BinaryOperator.Add, var l, ConstantExpr<double>(0)) => l,
                // 0 + r => r
                (BinaryOperator.Add, ConstantExpr<double>(0), var r) => r,
                // l * 1 => l
                (BinaryOperator.Multiply, var l, ConstantExpr<double>(1)) => l,
                // 1 * r => r
                (BinaryOperator.Multiply, ConstantExpr<double>(1), var r) => r,
                // l ^ 1 => l
                (BinaryOperator.Power, var l, ConstantExpr<double>(1)) => l,

                // TODO: Zero - can't enforce conditions at compile time
                // (BinaryOperator.Multiply, Number(0), _) => new Number(0), // when r is finite
                // (BinaryOperator.Multiply, _, Number(0)) => new Number(0), // when l is finite
                // (BinaryOperator.Divide, Number(0), not Number(0)) => new Number(0),
                // (BinaryOperator.Power, _, Number(0)) => new Number(1), // except 0^0
                // (BinaryOperator.Power, Number(0), _) => new Number(0), // except 0^0

                // 0 - r => -r
                (BinaryOperator.Subtract, ConstantExpr<double>(0), var r) => new UnaryExpr(
                    UnaryOperator.Negate,
                    r
                ).Optimized(),

                // Addition -> multiplication
                // x + x => 2 * x
                (BinaryOperator.Add, var l, var r) when l == r => new BinaryExpr(
                    BinaryOperator.Multiply,
                    new ConstantExpr<double>(2),
                    l
                ).Optimized(),
                // x + (rl:const * x) => (rl + 1) * x
                (BinaryOperator.Add, var l, BinaryExpr(BinaryOperator.Multiply,
                    ConstantExpr<double>(var rl), var rr)) when l == rr => new BinaryExpr(
                        BinaryOperator.Multiply,
                        new ConstantExpr<double>(rl + 1),
                        l
                    ).Optimized(),
                // (ll:const * x) + x => (ll + 1) * x
                (BinaryOperator.Add, BinaryExpr(BinaryOperator.Multiply,
                    ConstantExpr<double>(var ll), var lr), var r) when lr == r => new BinaryExpr(
                        BinaryOperator.Multiply,
                        new ConstantExpr<double>(ll + 1),
                        lr
                    ).Optimized(),

                // Subtraction -> multiplication
                // x - x => 0
                (BinaryOperator.Subtract, var l, var r) when l == r => new ConstantExpr<double>(0),
                // x - (rl:const * x) => (1 - rl) * x
                (BinaryOperator.Subtract, var l, BinaryExpr(BinaryOperator.Multiply,
                    ConstantExpr<double>(var rl), var rr)) when l == rr => new BinaryExpr(
                        BinaryOperator.Multiply,
                        new ConstantExpr<double>(1 - rl),
                        rr
                    ).Optimized(),
                // (ll:const * x) - x => (ll - 1) * x
                (BinaryOperator.Subtract, BinaryExpr(BinaryOperator.Multiply,
                    ConstantExpr<double>(var ll), var lr), var r) when lr == r => new BinaryExpr(
                        BinaryOperator.Multiply,
                        new ConstantExpr<double>(ll - 1),
                        lr
                    ).Optimized(),

                // Multiplication -> powers
                // x * x => x ^ 2
                (BinaryOperator.Multiply, var l, var r) when l == r => new BinaryExpr(
                    BinaryOperator.Power,
                    l,
                    new ConstantExpr<double>(2)
                ).Optimized(),
                // x * (x ^ rr) => x ^ (rr + 1)
                (BinaryOperator.Multiply, var l, BinaryExpr(BinaryOperator.Power, var rl, var rr))
                    when l == rl => new BinaryExpr(
                        BinaryOperator.Power,
                        l,
                        new BinaryExpr(BinaryOperator.Add, rr, new ConstantExpr<double>(1))
                    ).Optimized(),
                // (x ^ lr) * x => x ^ (lr + 1)
                (BinaryOperator.Multiply, BinaryExpr(BinaryOperator.Power, var ll, var lr), var r)
                    when ll == r => new BinaryExpr(
                        BinaryOperator.Power,
                        ll,
                        new BinaryExpr(BinaryOperator.Add, lr, new ConstantExpr<double>(1))
                    ).Optimized(),
                // (x ^ lr) * (x ^ rr) => x ^ (lr + rr)
                (BinaryOperator.Multiply, BinaryExpr(BinaryOperator.Power, var ll, var lr),
                    BinaryExpr(BinaryOperator.Power, var rl, var rr)) when ll == rl =>
                    new BinaryExpr(
                        BinaryOperator.Power,
                        ll,
                        new BinaryExpr(BinaryOperator.Add, lr, rr)
                    ).Optimized(),

                // Flattening fraction towers
                // (ll / lr) / r => ll / (lr * r)
                (BinaryOperator.Divide, BinaryExpr(BinaryOperator.Divide, var ll, var lr), var r) =>
                    new BinaryExpr(
                        BinaryOperator.Divide,
                        ll,
                        new BinaryExpr(BinaryOperator.Multiply, lr, r)
                    ).Optimized(),
                // l / (rl / rr) => (l * rr) / rl
                (BinaryOperator.Divide, var l, BinaryExpr(BinaryOperator.Divide, var rl, var rr)) =>
                    new BinaryExpr(
                        BinaryOperator.Divide,
                        new BinaryExpr(BinaryOperator.Multiply, l, rr),
                        rl
                    ).Optimized(),

                // Flatten power towers
                // (ll ^ lr) ^ r => ll ^ (lr * r)
                (BinaryOperator.Power, BinaryExpr(BinaryOperator.Power, var ll, var lr), var r) =>
                    new BinaryExpr(
                        BinaryOperator.Power,
                        ll,
                        new BinaryExpr(BinaryOperator.Multiply, lr, r)
                    ).Optimized(),
                // l ^ (rl ^ rr) => l ^ (rl * rr)
                (BinaryOperator.Power, var l, BinaryExpr(BinaryOperator.Power, var rl, var rr)) =>
                    new BinaryExpr(
                        BinaryOperator.Power,
                        l,
                        new BinaryExpr(BinaryOperator.Multiply, rl, rr)
                    ).Optimized(),

                // TODO: x / x => 1 (except 0 / 0)
                // (BinaryOperator.Divide, var l, var r) when l == r => new ConstantExpr<double>(1),

                // Negative distributive property
                // -l <+-> -r => -(l <+-> r)
                (BinaryOperator.Add or BinaryOperator.Subtract,
                    UnaryExpr(UnaryOperator.Negate, var l), UnaryExpr(UnaryOperator.Negate, var r))
                    => new UnaryExpr(UnaryOperator.Negate, new BinaryExpr(op, l, r)).Optimized(),

                // Distributive property
                // (x * lr) <+-> (x * rr) => x * (lr <+-> rr)
                (BinaryOperator.Add or BinaryOperator.Subtract or BinaryOperator.Divide,
                    BinaryExpr(BinaryOperator.Multiply, var ll, var lr), BinaryExpr(BinaryOperator
                        .Multiply, var rl, var rr)) when ll == rl => new BinaryExpr(
                        BinaryOperator.Multiply,
                        ll,
                        new BinaryExpr(op, lr, rr)
                    ).Optimized(),
                // (x * lr) <+-> (rl * x) => x * (lr <+-> rl)
                (BinaryOperator.Add or BinaryOperator.Subtract or BinaryOperator.Divide,
                    BinaryExpr(BinaryOperator.Multiply, var ll, var lr), BinaryExpr(BinaryOperator
                        .Multiply, var rl, var rr)) when ll == rr => new BinaryExpr(
                        BinaryOperator.Multiply,
                        ll,
                        new BinaryExpr(op, lr, rl)
                    ).Optimized(),
                // (ll * x) <+-> (x * rr) => x * (ll <+-> rr)
                (BinaryOperator.Add or BinaryOperator.Subtract or BinaryOperator.Divide,
                    BinaryExpr(BinaryOperator.Multiply, var ll, var lr), BinaryExpr(BinaryOperator
                        .Multiply, var rl, var rr)) when lr == rl => new BinaryExpr(
                        BinaryOperator.Multiply,
                        lr,
                        new BinaryExpr(op, ll, rr)
                    ).Optimized(),
                // (ll * x) <+-> (rl * x) => x * (ll <+-> rl)
                (BinaryOperator.Add or BinaryOperator.Subtract or BinaryOperator.Divide,
                    BinaryExpr(BinaryOperator.Multiply, var ll, var lr), BinaryExpr(BinaryOperator
                        .Multiply, var rl, var rr)) when lr == rr => new BinaryExpr(
                        BinaryOperator.Multiply,
                        lr,
                        new BinaryExpr(op, ll, rl)
                    ).Optimized(),

                // TODO if needed: Addition/subtraction of fractions
                // <number> <+-> (<number> / <expr>) -> (<number> <+-> <expr>) / <expr>

                // Multiplication of fractions
                // l * (rl / rr) => (l * rl) / rr
                (BinaryOperator.Multiply, var l, BinaryExpr(BinaryOperator.Divide, var rl, var rr))
                    => new BinaryExpr(
                        BinaryOperator.Divide,
                        new BinaryExpr(BinaryOperator.Multiply, l, rl),
                        rr
                    ).Optimized(),
                // (ll / lr) * r => (ll * r) / lr
                (BinaryOperator.Multiply, BinaryExpr(BinaryOperator.Divide, var ll, var lr), var r)
                    => new BinaryExpr(
                        BinaryOperator.Divide,
                        new BinaryExpr(BinaryOperator.Multiply, ll, r),
                        lr
                    ).Optimized(),

                // **Constant folding**

                // Simple constant expressions
                // l:const <op> r:const => <result>
                (BinaryOperator.Add, ConstantExpr<double>(var l), ConstantExpr<double>(var r)) =>
                    new ConstantExpr<double>(l + r),
                (BinaryOperator.Multiply, ConstantExpr<double>(var l), ConstantExpr<double>(var r))
                    => new ConstantExpr<double>(l * r),
                (BinaryOperator.Modulo, ConstantExpr<double>(var l), ConstantExpr<double>(var r)) =>
                    new ConstantExpr<double>(l % r),
                (BinaryOperator.Power, ConstantExpr<double>(var l), ConstantExpr<double>(var r)) =>
                    new ConstantExpr<double>(Math.Pow(l, r)),

                // Associative expressions
                // l:const <+*> (rl:const <+*> <expr>) => (l <+*> rl) <+*> <expr>
                (BinaryOperator.Add, ConstantExpr<double>(var l), BinaryExpr(BinaryOperator.Add,
                    ConstantExpr<double>(var rl), var rr)) => new BinaryExpr(
                        BinaryOperator.Add,
                        new ConstantExpr<double>(l + rl),
                        rr
                    ).Optimized(),
                (BinaryOperator.Multiply, ConstantExpr<double>(var l), BinaryExpr(
                    BinaryOperator.Multiply, ConstantExpr<double>(var rl), var rr)) =>
                    new BinaryExpr(BinaryOperator.Multiply, new ConstantExpr<double>(l * rl), rr)
                        .Optimized(),

                // Inverse associative expressions
                // l:const - (rl:const + rr) => (l - rl) - rr
                (BinaryOperator.Subtract, ConstantExpr<double>(var l), BinaryExpr(BinaryOperator.Add
                    , ConstantExpr<double>(var rl), var rr)) => new BinaryExpr(
                        BinaryOperator.Subtract,
                        new ConstantExpr<double>(l - rl),
                        rr
                    ).Optimized(),
                // TODO if needed: (ll:const + rl) - rr:const => rl - (ll + rr)

                // l:const - (rl:const - rr) => (l - rl) + rr
                (BinaryOperator.Subtract, ConstantExpr<double>(var l), BinaryExpr(
                    BinaryOperator.Subtract, ConstantExpr<double>(var rl), var rr)) =>
                    new BinaryExpr(BinaryOperator.Add, new ConstantExpr<double>(l - rl), rr)
                        .Optimized(),

                // Fallback
                _ => new BinaryExpr(this.Operator, left, right),
            };
        }

        public override bool TryEvaluate(
            IDictionary<string, double> variables,
            HashSet<string> missingVariables,
            out double result
        )
        {
            if (!this.Left.TryEvaluate(variables, missingVariables, out var left)
                || !this.Right.TryEvaluate(variables, missingVariables, out var right))
            {
                result = default;
                return false;
            }

            result = this.Operator switch
            {
                BinaryOperator.Add => left + right,
                BinaryOperator.Subtract => left - right,
                BinaryOperator.Multiply => left * right,
                BinaryOperator.Divide => left / right,
                BinaryOperator.Modulo => left % right,
                BinaryOperator.Power => Math.Pow(left, right),
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
            if (!this.Left.TryCompile(variables, missingVariables, out var left)
                || !this.Right.TryCompile(variables, missingVariables, out var right))
            {
                result = default;
                return false;
            }

            result = this.Operator switch
            {
                BinaryOperator.Add => Expression.Add(left, right),
                BinaryOperator.Subtract => Expression.Subtract(left, right),
                BinaryOperator.Multiply => Expression.Multiply(left, right),
                BinaryOperator.Divide => Expression.Divide(left, right),
                BinaryOperator.Modulo => Expression.Modulo(left, right),
                BinaryOperator.Power => Expression.Call(
                    null,
                    AccessTools.Method(
                        typeof(Math),
                        nameof(Math.Pow),
                        new[] {typeof(double), typeof(double)}
                    ),
                    left,
                    right
                ),
                _ => throw new InvalidOperationException($"Unknown operator {this.Operator}"),
            };
            return true;
        }

        public override string ToString()
        {
            return this.Operator switch
            {
                BinaryOperator.Add => $"({this.Left} + {this.Right})",
                BinaryOperator.Subtract => $"({this.Left} - {this.Right})",
                BinaryOperator.Multiply => $"({this.Left} * {this.Right})",
                BinaryOperator.Divide => $"({this.Left} / {this.Right})",
                BinaryOperator.Modulo => $"({this.Left} % {this.Right})",
                BinaryOperator.Power => $"({this.Left} ^ {this.Right})",
                _ => base.ToString(),
            };
        }
    }
}
