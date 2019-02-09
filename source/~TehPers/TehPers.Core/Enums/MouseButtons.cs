using System;

namespace TehPers.Core.Enums {
    [Flags]
    public enum MouseButtons {
        LEFT = 0b1,
        RIGHT = 0b10,
        MIDDLE = 0b100
    }
}