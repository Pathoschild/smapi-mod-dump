using System;
using Microsoft.Xna.Framework.Input;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Events {
    public class EventArgsKeyRepeated : EventArgs {
        public Keys RepeatedKey { get; }
        public char? Character { get; }

        public EventArgsKeyRepeated(Keys repeatedKey) {
            this.RepeatedKey = repeatedKey;
            this.Character = repeatedKey.ToChar();
        }
    }
}