/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StardewModdingAPI;
using TehPers.CoreMod.Api.ContentLoading;

namespace TehPers.CoreMod.ContentLoading {
    internal class CoreTranslation : ICoreTranslation {
        private static readonly Regex _tokenRegex = new Regex(@"\{\{\s*(?<token>.*?)\s*\}\}");

        private readonly string _value;

        public string Key { get; }
        public bool Found { get; }

        public CoreTranslation(Translation translation) : this(translation.Key, translation.HasValue() ? translation.ToString() : null, translation.HasValue()) { }
        public CoreTranslation(string key) : this(key, null, false) { }
        public CoreTranslation(string key, string value) : this(key, value, true) { }
        private CoreTranslation(string key, string value, bool found) {
            this.Key = key;
            this._value = value;
            this.Found = found;
        }

        public ICoreTranslation Assert() {
            if (!this.Found) {
                throw new KeyNotFoundException("No translation was found for the key '{}'");
            }

            return new CoreTranslation(this.Key, this._value, this.Found);
        }

        public ICoreTranslation WithFormatValues(params object[] values) {
            return new CoreTranslation(this.Key, this.Found ? string.Format(this._value, values) : this._value, this.Found);
        }

        public ICoreTranslation WithTokens(object tokens) {
            // If the translation wasn't found, don't replace any tokens (don't make reflection calls)
            if (!this.Found) {
                return new CoreTranslation(this.Key, this._value, this.Found);
            }

            // Get the type of the object passed in
            Type type = tokens.GetType();

            // Pair each property with its getter
            IEnumerable<(string name, Func<object, object> getter)> properties = type.GetProperties().Select(property => (property.Name, (Func<object, object>) property.GetValue));

            // Pair each field with its getter
            IEnumerable<(string name, Func<object, object> getter)> fields = type.GetFields().Select(field => (field.Name, (Func<object, object>) field.GetValue));

            // Create a dictionary to fetch token values
            IDictionary<string, Lazy<string>> tokenDictionary = properties.Concat(fields).ToDictionary(item => item.name, item => new Lazy<string>(() => item.getter(tokens).ToString()), StringComparer.OrdinalIgnoreCase);

            // Call another overload using a token converter
            return this.WithTokens(token => tokenDictionary.TryGetValue(token, out Lazy<string> valueFactory) ? valueFactory.Value : $"{{{token}}}");
        }

        public ICoreTranslation WithTokens(IDictionary<string, string> tokens) {
            // If the translation wasn't found, don't replace any tokens (don't create a new dictionary)
            if (!this.Found) {
                return new CoreTranslation(this.Key, this._value, this.Found);
            }

            // Convert the dictionary to a case-insensitive one
            tokens = new Dictionary<string, string>(tokens, StringComparer.OrdinalIgnoreCase);

            // Call another overload using a token converter
            return this.WithTokens(token => tokens.TryGetValue(token, out string replaced) ? replaced : $"{{{token}}}");
        }

        public ICoreTranslation WithTokens(Func<string, string> tokenConverter) {
            // If the translation wasn't found, don't replace any tokens
            if (!this.Found) {
                return new CoreTranslation(this.Key, this._value, this.Found);
            }

            string replaced = CoreTranslation._tokenRegex.Replace(this._value, match => tokenConverter(match.Groups["token"].Value));
            return new CoreTranslation(this.Key, replaced, this.Found);
        }

        public ICoreTranslation WithDefault(string @default) {
            return new CoreTranslation(this.Key, this._value ?? @default, this.Found);
        }

        public override string ToString() {
            return this._value ?? this.Key;
        }
    }
}