using TehPers.CoreMod.Api.ContentPacks;
using TehPers.CoreMod.Api.ContentPacks.Tokens;

namespace TehPers.CoreMod.ContentPacks.Values {
    internal class StaticContentPackValueProvider<T> : IContentPackValueProvider<T> {
        private readonly T _value;

        public StaticContentPackValueProvider(T value) {
            this._value = value;
        }

        public T GetValue(ITokenHelper helper) {
            return this._value;
        }

        public bool IsValidInContext(IContext context) {
            return true;
        }
    }
}