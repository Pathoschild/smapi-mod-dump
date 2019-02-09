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