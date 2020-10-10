/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

namespace TehPers.CoreMod.Api.ContentPacks.Tokens {
    public readonly struct TokenValue {
        private readonly object _value;

        public TokenValue(string value) : this((object) value) { }
        public TokenValue(int value) : this((object) value) { }
        public TokenValue(bool value) : this((object) value) { }
        public TokenValue(double value) : this((object) value) { }
        private TokenValue(object value) {
            this._value = value;
        }

        public bool TryGetString(out string value) => this.TryGet(out value);
        public bool TryGetInt32(out int value) => this.TryGet(out value);
        public bool TryGetBoolean(out bool value) => this.TryGet(out value);
        public bool TryGetDouble(out double value) {
            // Try to get the value as a double
            if (this.TryGet(out value)) {
                return true;
            }

            // Try to get the value as an int, then convert it to a double
            if (this.TryGet(out int intValue)) {
                value = intValue;
                return true;
            }

            return false;
        }

        private bool TryGet<T>(out T value) {
            if (this._value is T converted) {
                value = converted;
                return true;
            }

            value = default;
            return false;
        }

        public override string ToString() {
            return this._value.ToString();
        }
    }
}