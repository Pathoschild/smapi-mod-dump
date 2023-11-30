/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;

namespace CoreBoy.cpu
{
    public class InterruptManager : IAddressSpace
    {
        private bool _ime;
        private readonly bool _gbc;
        private int _interruptFlag = 0xe1;
        private int _interruptEnabled;
        private int _pendingEnableInterrupts = -1;
        private int _pendingDisableInterrupts = -1;

        public InterruptManager(bool gbc)
        {
            this._gbc = gbc;
        }

        public void EnableInterrupts(bool withDelay)
        {
            this._pendingDisableInterrupts = -1;
            if (withDelay)
            {
                if (this._pendingEnableInterrupts == -1)
                {
                    this._pendingEnableInterrupts = 1;
                }
            }
            else
            {
                this._pendingEnableInterrupts = -1;
                this._ime = true;
            }
        }

        public void DisableInterrupts(bool withDelay)
        {
            this._pendingEnableInterrupts = -1;
            if (withDelay && this._gbc)
            {
                if (this._pendingDisableInterrupts == -1)
                {
                    this._pendingDisableInterrupts = 1;
                }
            }
            else
            {
                this._pendingDisableInterrupts = -1;
                this._ime = false;
            }
        }

        public void RequestInterrupt(InterruptType type) => this._interruptFlag |= 1 << type.Ordinal;
        public void ClearInterrupt(InterruptType type) => this._interruptFlag &= ~(1 << type.Ordinal);

        public void OnInstructionFinished()
        {
            if (this._pendingEnableInterrupts != -1)
            {
                if (this._pendingEnableInterrupts-- == 0)
                {
                    this.EnableInterrupts(false);
                }
            }

            if (this._pendingDisableInterrupts != -1)
            {
                if (this._pendingDisableInterrupts-- == 0)
                {
                    this.DisableInterrupts(false);
                }
            }
        }

        public bool IsIme() => this._ime;
        public bool IsInterruptRequested() => (this._interruptFlag & this._interruptEnabled) != 0;
        public bool IsHaltBug() => (this._interruptFlag & this._interruptEnabled & 0x1f) != 0 && !this._ime;
        public bool Accepts(int address) => address == 0xff0f || address == 0xffff;

        public void SetByte(int address, int value)
        {
            switch (address)
            {
                case 0xff0f:
                    this._interruptFlag = value | 0xe0;
                    break;

                case 0xffff:
                    this._interruptEnabled = value;
                    break;
            }
        }

        public int GetByte(int address)
        {
            switch (address)
            {
                case 0xff0f:
                    return this._interruptFlag;

                case 0xffff:
                    return this._interruptEnabled;

                default:
                    return 0xff;
            }
        }

        public class InterruptType
        {
            public static InterruptType VBlank = new InterruptType(0x0040, 0);
            public static InterruptType Lcdc = new InterruptType(0x0048, 1);
            public static InterruptType Timer = new InterruptType(0x0050, 2);
            public static InterruptType Serial = new InterruptType(0x0058, 3);
            public static InterruptType P1013 = new InterruptType(0x0060, 4);

            public int Ordinal { get; }

            public int Handler { get; }

            private InterruptType(int handler, int ordinal)
            {
                this.Ordinal = ordinal;
                this.Handler = handler;
            }

            public static IEnumerable<InterruptType> Values()
            {
                yield return VBlank;
                yield return Lcdc;
                yield return Timer;
                yield return Serial;
                yield return P1013;
            }
        }
    }
}