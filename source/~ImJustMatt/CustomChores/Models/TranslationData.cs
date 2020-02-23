using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StardewModdingAPI;

namespace LeFauxMatt.CustomChores.Models
{
    public class TranslationData
    {
        public string Key { get; }
        protected internal Translation Translation { get; }
        protected internal IDictionary<string, string> Selectors { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly IList<string> _tokens = new List<string>();

        public TranslationData(Translation translation)
        {
            Translation = translation ?? throw new ArgumentNullException(nameof(translation));

            var parts = translation.Key.Split('.');

            // get key
            Key = parts.First().Trim();

            // get selectors
            foreach (var part in parts.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;
                if (!part.Contains('='))
                    continue;

                var subParts = part.Split('=');
                if (subParts.Length != 2)
                    continue;

                if (!Selectors.ContainsKey(subParts[0].Trim()))
                    Selectors.Add(subParts[0].Trim(), subParts[1].Trim());
            }

            // get tokens
            var reg = new Regex(@"\{\{\w+\}\}", RegexOptions.IgnoreCase);
            char[] charsToTrim = {'{', '}'};
            foreach (Match match in reg.Matches(translation.ToString()))
            {
                if (!_tokens.Contains(match.Value.Trim(charsToTrim)))
                    _tokens.Add(match.Value.Trim(charsToTrim));
            }
        }

        public bool HasSelector(string selector)
        {
            return Selectors.ContainsKey(selector);
        }

        public bool HasToken(string token)
        {
            return _tokens.Contains(token);
        }

        public bool Filter(IDictionary<string, Func<string>> tokens)
        {
            foreach (var selector in Selectors)
            {
                var tokenValue = tokens.TryGetValue(selector.Key, out var tokenFn) ? tokenFn.Invoke() : null;
                if (tokenValue is null || !selector.Value.Equals(tokenValue, StringComparison.CurrentCultureIgnoreCase))
                    return false;
            }

            return true;
        }

        public string Tokens(IDictionary<string, Func<string>> tokens)
        {
            var tokenValues = new Dictionary<string, string>();
            foreach (var tokenKey in _tokens)
            {
                var tokenValue = tokens.TryGetValue(tokenKey, out var tokenFn) ? tokenFn() : null;
                if (tokenValue != null)
                    tokenValues.Add(tokenKey, tokenValue);
            }

            return Translation.Tokens(tokenValues).ToString();
        }
    }
}
