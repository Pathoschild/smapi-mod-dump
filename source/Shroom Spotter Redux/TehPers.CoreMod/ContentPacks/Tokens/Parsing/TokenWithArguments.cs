/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using TehPers.CoreMod.Api.ContentPacks;
using TehPers.CoreMod.Api.ContentPacks.Tokens;

namespace TehPers.CoreMod.ContentPacks.Tokens.Parsing {
    internal class TokenWithArguments : IContentPackValueProvider<string> {
        private readonly IToken _token;
        private readonly string[] _arguments;

        public TokenWithArguments(IToken token, string[] arguments) {
            this._token = token;
            this._arguments = arguments;
        }

        public bool IsValidInContext(IContext context) {
            return this._token.IsValidInContext(context);
        }

        public string GetValue(ITokenHelper helper) {
            return this._token.GetValue(helper, this._arguments);
        }
    }
}