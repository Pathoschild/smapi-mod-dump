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
using System.Linq;
using System.Text;
using Sprache;
using TehPers.CoreMod.Api.ContentPacks;
using TehPers.CoreMod.Api.ContentPacks.Tokens;
using TehPers.CoreMod.Api.Extensions;

namespace TehPers.CoreMod.ContentPacks.Tokens.Parsing {
    internal class TokenParser {
        private readonly TokenRegistry _tokenRegistry;

        public TokenParser(TokenRegistry tokenRegistry) {
            this._tokenRegistry = tokenRegistry;
        }

        public IEnumerable<IContentPackValueProvider<string>> ParseRawValue(string rawInput) {
            Parser<IEnumerable<IContentPackValueProvider<string>>> parser = this.TokenizedString();
            return parser.Parse(rawInput);
        }

        public IContentPackValueProvider<string> ParseToken(string tokenString) {
            return TokenParser.Token().Select(result => {
                if (!this._tokenRegistry.TryGetToken(result.name, out IToken token)) {
                    throw new ParseException($"Unknown token '{result.name}'");
                }

                return new TokenWithArguments(token, result.arguments);
            }).Parse(tokenString);
        }

        private Parser<IEnumerable<IContentPackValueProvider<string>>> TokenizedString() {
            return from partsWithTokens in TokenParser.LiteralThenTokenNonEndPart().Many().Select(this.CreateParts)
                   from trailingLiteral in TokenParser.LiteralEndPart()
                   let trailingLiteralPart = string.IsNullOrEmpty(trailingLiteral) ? null : new ConstantTokenizedStringPart(trailingLiteral)
                   select partsWithTokens.Concat(trailingLiteralPart?.Yield() ?? Enumerable.Empty<IContentPackValueProvider<string>>());
        }

        private IEnumerable<IContentPackValueProvider<string>> CreateParts(IEnumerable<(string literal, string tokenName, string[] arguments)> parsedParts) {
            foreach ((string literal, string tokenName, string[] arguments) in parsedParts) {
                // Literal part
                if (!string.IsNullOrEmpty(literal)) {
                    yield return new ConstantTokenizedStringPart(literal);
                }

                if (!this._tokenRegistry.TryGetToken(tokenName, out IToken token)) {
                    throw new ParseException($"Unknown token '{tokenName}'");
                }

                yield return new TokenWithArguments(token, arguments);
            }
        }

        private static Parser<(string literal, string tokenName, string[] arguments)> LiteralThenTokenNonEndPart() {
            return from literal in TokenParser.LiteralNonEndPart()
                   from token in TokenParser.ContainedToken()
                   select (literal, token.name, token.arguments);
        }

        private static Parser<string> LiteralNonEndPart() {
            return TokenParser.EscapedOpenToken()
                .Or(TokenParser.EscapedCloseToken())
                .Or(from brace in Parse.Char('{')
                    from nextChar in Parse.CharExcept('{')
                    select $"{brace}{nextChar}")
                .Or(Parse.String("{").End())
                .Or(Parse.CharExcept('{').Select(c => c.ToString()))
                .Many()
                .Select(parts => parts.Aggregate(new StringBuilder(), (builder, part) => builder.Append(part)).ToString());
        }

        private static Parser<string> LiteralEndPart() {
            Parser<string> anyEscape = TokenParser.EscapedOpenToken().Or(TokenParser.EscapedCloseToken());
            return anyEscape
                .Or(Parse.AnyChar.Select(c => c.ToString()))
                .Many()
                .Select(parts => parts.Aggregate(new StringBuilder(), (builder, part) => builder.Append(part)).ToString());
        }

        private static Parser<T> Fail<T>(string reason, IEnumerable<string> expectations) {
            return input => Result.Failure<T>(input, reason, expectations);
        }

        private static Parser<(string name, string[] arguments)> ContainedToken() {
            return TokenParser.Token().Contained(TokenParser.OpenToken(), TokenParser.CloseToken());
        }

        private static Parser<(string name, string[] arguments)> Token() {
            return TokenParser.TokenWithArgContents().Or(TokenParser.SimpleTokenContents()).Contained(TokenParser.OpenToken(), TokenParser.CloseToken());
        }

        private static Parser<string> OpenToken() {
            return Parse.String("{{").Text();
        }

        private static Parser<string> CloseToken() {
            return Parse.String("}}").Text();
        }

        private static Parser<string> EscapedOpenToken() {
            return TokenParser.OpenToken().Then(escape => TokenParser.OpenToken());
        }

        private static Parser<string> EscapedCloseToken() {
            return TokenParser.CloseToken().Then(escape => TokenParser.CloseToken());
        }

        private static Parser<string> Name() {
            return from firstChar in Parse.Letter
                   from rest in Parse.LetterOrDigit.Many()
                   select new string(rest.Prepend(firstChar).ToArray());
        }

        private static Parser<string> SimpleArgument() {
            return Parse.CharExcept(",}").Many().Text();
        }

        private static Parser<string> QuotedArgument() {
            return from openQuote in TokenParser.OpenQuote()
                   from contents in TokenParser.EscapedOpenQuote().Or(TokenParser.EscapedCloseQuote()).Or(TokenParser.NonQuoteArgumentCharacter()).Many().Text()
                   from closeQuote in TokenParser.ClosedQuote()
                   select contents;
        }

        private static Parser<string> Argument() {
            return TokenParser.QuotedArgument().Token().Or(TokenParser.SimpleArgument().Select(t => t.Trim()));
        }

        private static Parser<IEnumerable<string>> ArgumentList() {
            return from first in TokenParser.Argument()
                   from rest in Parse.Char(',').Token().Then(_ => TokenParser.Argument()).Many()
                   select rest.Prepend(first);
        }

        private static Parser<(string name, string[] arguments)> SimpleTokenContents() {
            return TokenParser.Name().Token().Select(name => (name, new string[0]));
        }

        private static Parser<(string name, string[] arguments)> TokenWithArgContents() {
            return from name in TokenParser.Name().Token()
                   from delimiter in Parse.Char(':')
                   from argumentList in TokenParser.ArgumentList()
                   select (name, argumentList.ToArray());
        }

        private static Parser<char> OpenQuote() {
            return Parse.Char('<');
        }

        private static Parser<char> ClosedQuote() {
            return Parse.Char('>');
        }

        private static Parser<char> EscapedOpenQuote() {
            return TokenParser.OpenQuote().Then(escape => TokenParser.OpenQuote());
        }

        private static Parser<char> EscapedCloseQuote() {
            return TokenParser.ClosedQuote().Then(escape => TokenParser.ClosedQuote());
        }

        private static Parser<char> NonQuoteArgumentCharacter() {
            return Parse.AnyChar.Except(TokenParser.OpenQuote().Or(TokenParser.ClosedQuote()));
        }
    }
}
