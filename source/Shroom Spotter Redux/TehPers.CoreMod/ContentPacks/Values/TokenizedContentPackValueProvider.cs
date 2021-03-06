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
using System.Globalization;
using System.Linq;
using TehPers.CoreMod.Api.ContentPacks;
using TehPers.CoreMod.Api.ContentPacks.Tokens;
using TehPers.CoreMod.ContentPacks.Tokens.Parsing;

namespace TehPers.CoreMod.ContentPacks.Values {
    internal class TokenizedContentPackValueProvider<T> : IContentPackValueProvider<T> {
        private readonly IContentPackValueProvider<string>[] _parts;

        public TokenizedContentPackValueProvider(string rawString, TokenParser tokenParser) {
            this._parts = tokenParser.ParseRawValue(rawString).ToArray();
        }

        public T GetValue(ITokenHelper helper) {
            // No parts, return the default value
            if (!this._parts.Any()) {
                return default;
            }

            // Concatenate each part's result into a single string
            string resultStr = string.Concat(this._parts.Select(p => p.GetValue(helper)));

            // Try to convert it to the given value type
            if (Convert.ChangeType(resultStr, typeof(T), CultureInfo.InvariantCulture) is T result) {
                return result;
            }

            // Conversion failed
            throw new InvalidOperationException($"Conversion to {typeof(T).Name} failed. Source: {resultStr}");
        }

        public bool IsValidInContext(IContext context) {
            return this._parts.All(p => p.IsValidInContext(context));
        }
    }
}