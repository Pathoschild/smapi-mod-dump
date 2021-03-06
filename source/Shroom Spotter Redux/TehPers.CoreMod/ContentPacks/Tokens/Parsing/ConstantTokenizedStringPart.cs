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
    internal class ConstantTokenizedStringPart : IContentPackValueProvider<string> {
        private readonly string _value;

        public ConstantTokenizedStringPart(string value) {
            this._value = value;
        }

        public string GetValue(ITokenHelper helper) {
            return this._value;
        }

        public bool IsValidInContext(IContext context) {
            return true;
        }
    }
}