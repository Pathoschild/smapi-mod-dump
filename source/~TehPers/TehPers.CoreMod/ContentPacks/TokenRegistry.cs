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
using TehPers.CoreMod.Api.ContentPacks.Tokens;

namespace TehPers.CoreMod.ContentPacks {
    internal class TokenRegistry {
        private readonly Dictionary<string, IToken> _tokens = new Dictionary<string, IToken>(StringComparer.OrdinalIgnoreCase);

        public TokenRegistry() { }

        public void Add(string key, IToken token) {
            this._tokens.Add(key, token);
        }

        public bool TryGetToken(string key, out IToken token) {
            return this._tokens.TryGetValue(key, out token);
        }
    }
}