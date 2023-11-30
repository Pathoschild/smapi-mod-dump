/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using CoreBoy.cpu;

namespace CoreBoy.timer
{
    public class Timer : IAddressSpace
    {
        private readonly SpeedMode SpeedMode;
        private readonly InterruptManager InterruptManager;
        private static readonly int[] FreqToBit = { 9, 3, 5, 7 };

        private int Div;
        private int Tac;
        private int Tma;
        private int Tima;
        private bool PreviousBit;
        private bool Overflow;
        private int TicksSinceOverflow;

        public Timer(InterruptManager interruptManager, SpeedMode speedMode)
        {
            this.SpeedMode = speedMode;
            this.InterruptManager = interruptManager;
        }

        public void Tick()
        {
            this.UpdateDiv((this.Div + 1) & 0xffff);
            if (!this.Overflow)
            {
                return;
            }

            this.TicksSinceOverflow++;
            if (this.TicksSinceOverflow == 4)
            {
                this.InterruptManager.RequestInterrupt(InterruptManager.InterruptType.Timer);
            }

            if (this.TicksSinceOverflow == 5)
            {
                this.Tima = this.Tma;
            }

            if (this.TicksSinceOverflow == 6)
            {
                this.Tima = this.Tma;
                this.Overflow = false;
                this.TicksSinceOverflow = 0;
            }
        }

        private void IncTima()
        {
            this.Tima++;
            this.Tima %= 0x100;
            if (this.Tima == 0)
            {
                this.Overflow = true;
                this.TicksSinceOverflow = 0;
            }
        }

        private void UpdateDiv(int newDiv)
        {
            this.Div = newDiv;
            int bitPos = FreqToBit[this.Tac & 0b11];
            bitPos <<= this.SpeedMode.GetSpeedMode() - 1;
            bool bit = (this.Div & (1 << bitPos)) != 0;
            bit &= (this.Tac & (1 << 2)) != 0;
            if (!bit && this.PreviousBit)
            {
                this.IncTima();
            }

            this.PreviousBit = bit;
        }

        public bool Accepts(int address) => address >= 0xff04 && address <= 0xff07;

        public void SetByte(int address, int value)
        {
            switch (address)
            {
                case 0xff04:
                    this.UpdateDiv(0);
                    break;

                case 0xff05:
                    if (this.TicksSinceOverflow < 5)
                    {
                        this.Tima = value;
                        this.Overflow = false;
                        this.TicksSinceOverflow = 0;
                    }

                    break;

                case 0xff06:
                    this.Tma = value;
                    break;

                case 0xff07:
                    this.Tac = value;
                    break;
            }
        }

        public int GetByte(int address)
        {
            return address switch
            {
                0xff04 => this.Div >> 8,
                0xff05 => this.Tima,
                0xff06 => this.Tma,
                0xff07 => this.Tac | 0b11111000,
                _ => throw new ArgumentException("Invalid byte", nameof(GetByte))
            };
        }
    }
}
