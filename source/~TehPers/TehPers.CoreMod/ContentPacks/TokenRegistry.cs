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