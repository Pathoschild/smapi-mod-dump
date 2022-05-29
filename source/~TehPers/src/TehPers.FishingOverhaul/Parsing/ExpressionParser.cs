/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Superpower;
using Superpower.Display;
using Superpower.Parsers;
using Superpower.Tokenizers;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.FishingOverhaul.Parsing
{
    internal class ExpressionParser
    {
        private static readonly Tokenizer<ExpressionToken> tokenizer =
            new TokenizerBuilder<ExpressionToken>().Ignore(Character.WhiteSpace)
                .Match(Numerics.Decimal, ExpressionToken.Number)
                .Match(Character.Letter.AtLeastOnce(), ExpressionToken.Ident)
                .Match(Character.EqualTo('+'), ExpressionToken.Plus)
                .Match(Character.EqualTo('-'), ExpressionToken.Minus)
                .Match(Character.EqualTo('*'), ExpressionToken.Asterisk)
                .Match(Character.EqualTo('/'), ExpressionToken.FSlash)
                .Match(Character.EqualTo('%'), ExpressionToken.Percent)
                .Match(Character.EqualTo('^'), ExpressionToken.Caret)
                .Match(Character.EqualTo('('), ExpressionToken.LParen)
                .Match(Character.EqualTo(')'), ExpressionToken.RParen)
                .Build();

        // <add> := PLUS
        private static readonly TokenListParser<ExpressionToken, BinaryOperator> add =
            Token.EqualTo(ExpressionToken.Plus).Value(BinaryOperator.Add);

        // <subtract> := MINUS
        private static readonly TokenListParser<ExpressionToken, BinaryOperator> subtract =
            Token.EqualTo(ExpressionToken.Minus).Value(BinaryOperator.Subtract);

        // <multiply> := ASTERISK
        private static readonly TokenListParser<ExpressionToken, BinaryOperator> multiply =
            Token.EqualTo(ExpressionToken.Asterisk).Value(BinaryOperator.Multiply);

        // <divide> := FSLASH
        private static readonly TokenListParser<ExpressionToken, BinaryOperator> divide =
            Token.EqualTo(ExpressionToken.FSlash).Value(BinaryOperator.Divide);

        // <modulo> := PERCENT
        private static readonly TokenListParser<ExpressionToken, BinaryOperator> modulo =
            Token.EqualTo(ExpressionToken.Percent).Value(BinaryOperator.Modulo);

        // <power> := CARET
        private static readonly TokenListParser<ExpressionToken, BinaryOperator> power =
            Token.EqualTo(ExpressionToken.Caret).Value(BinaryOperator.Power);

        // <number> := NUMBER
        private static readonly TokenListParser<ExpressionToken, Expr<double>> number =
            Token.EqualTo(ExpressionToken.Number)
                .Apply(Numerics.DecimalDouble)
                .Select(n => (Expr<double>)new ConstantExpr<double>(n))
                .Named("number");

        // <ident> := IDENT
        private static readonly TokenListParser<ExpressionToken, Expr<double>> ident =
            Token.EqualTo(ExpressionToken.Ident)
                .Apply(Character.Letter.AtLeastOnce())
                .Select(chars => (Expr<double>)new IdentExpr(new(chars)))
                .Named("identifier");

        // <group> := "(" <expr> ")"
        private static readonly TokenListParser<ExpressionToken, Expr<double>> group =
            Token.EqualTo(ExpressionToken.LParen)
                .IgnoreThen(Parse.Ref(() => ExpressionParser.expr!))
                .Then(expr => Token.EqualTo(ExpressionToken.RParen).Value(expr));

        // <factor> := <group> | <number> | <ident>
        private static readonly TokenListParser<ExpressionToken, Expr<double>> factor =
            ExpressionParser.group.Or(ExpressionParser.number).Or(ExpressionParser.ident);

        // <negative> := MINUS <unaryAssoc>
        private static readonly TokenListParser<ExpressionToken, Expr<double>> negative =
            Token.EqualTo(ExpressionToken.Minus)
                .IgnoreThen(Parse.Ref(() => ExpressionParser.unaryAssoc!))
                .Select(inner => (Expr<double>)new UnaryExpr(UnaryOperator.Negate, inner));

        // <unaryAssoc> := <negative> | <factor>
        private static readonly TokenListParser<ExpressionToken, Expr<double>> unaryAssoc =
            ExpressionParser.negative.Or(ExpressionParser.factor);

        // <exponentialAssoc> := <unaryAssoc> (<power> <unaryAssoc>)* [left-assoc]
        private static readonly TokenListParser<ExpressionToken, Expr<double>> exponentialAssoc =
            Parse.Chain(
                ExpressionParser.power,
                ExpressionParser.unaryAssoc,
                (op, l, r) => new BinaryExpr(op, l, r)
            );

        // <multiplicativeAssoc> := <exponentialAssoc> (<multiply | divide | modulo> <exponentialAssoc>)* [left-assoc]
        private static readonly TokenListParser<ExpressionToken, Expr<double>> multiplicativeAssoc =
            Parse.Chain(
                ExpressionParser.multiply.Or(ExpressionParser.divide).Or(ExpressionParser.modulo),
                ExpressionParser.exponentialAssoc,
                (op, l, r) => new BinaryExpr(op, l, r)
            );

        // <additiveAssoc> := <multiplicativeAssoc> (<add | subtract> <multiplicativeAssoc>)* [left-assoc]
        private static readonly TokenListParser<ExpressionToken, Expr<double>> additiveAssoc =
            Parse.Chain(
                ExpressionParser.add.Or(ExpressionParser.subtract),
                ExpressionParser.multiplicativeAssoc,
                (op, l, r) => new BinaryExpr(op, l, r)
            );

        // <expr> := <additiveAssoc>
        private static readonly TokenListParser<ExpressionToken, Expr<double>> expr =
            ExpressionParser.additiveAssoc;

        public static bool TryParse(
            string source,
            [MaybeNullWhen(false)] out Expr<double> result,
            [MaybeNullWhen(true)] out string error
        )
        {
            // Tokenize
            var tokenized = ExpressionParser.tokenizer.TryTokenize(source);
            if (!tokenized.HasValue)
            {
                result = default;
                error = tokenized.FormatErrorMessageFragment();
                return false;
            }

            // Parse
            var parsed = ExpressionParser.expr.AtEnd().TryParse(tokenized.Value);
            if (!parsed.HasValue)
            {
                result = default;
                error =
                    $"{parsed.FormatErrorMessageFragment()} (at position {parsed.ErrorPosition.Absolute})";
                return false;
            }

            // Optimize and return
            result = parsed.Value.Optimized();
            error = default;
            return true;
        }

        public enum ExpressionToken
        {
            [Token(Example = "+")]
            Plus,

            [Token(Example = "-")]
            Minus,

            [Token(Example = "*")]
            Asterisk,

            [Token(Example = "/")]
            FSlash,

            [Token(Example = "%")]
            Percent,

            [Token(Example = "^")]
            Caret,

            [Token(Example = "(")]
            LParen,

            [Token(Example = ")")]
            RParen,

            [Token(Description = "identifier", Example = "x")]
            Ident,

            [Token(Description = "number", Example = "24.5")]
            Number,
        }
    }
}
