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

namespace CoreBoy.sound
{
    public abstract class SoundModeBase : IAddressSpace
    {
        protected bool DacEnabled;
        protected bool ChannelEnabled;
        protected readonly int Offset;
        protected readonly bool Gbc;
        protected LengthCounter Length;

        protected int Nr0, Nr1, Nr2, Nr3, Nr4;

        protected virtual int GetNr0() => this.Nr0;
        protected virtual int GetNr1() => this.Nr1;
        protected virtual int GetNr2() => this.Nr2;
        protected virtual int GetNr3() => this.Nr3;
        protected virtual int GetNr4() => this.Nr4;

        protected virtual void SetNr0(int value) => this.Nr0 = value;
        protected virtual void SetNr1(int value) => this.Nr1 = value;
        protected virtual void SetNr2(int value) => this.Nr2 = value;
        protected virtual void SetNr3(int value) => this.Nr3 = value;

        protected SoundModeBase(int offset, int length, bool gbc)
        {
            this.Offset = offset;
            this.Length = new LengthCounter(length);
            this.Gbc = gbc;
        }

        public abstract int Tick();
        protected abstract void Trigger();

        public bool IsEnabled() => this.ChannelEnabled && this.DacEnabled;
        public virtual bool Accepts(int address) => address >= this.Offset && address < this.Offset + 5;

        public virtual void SetByte(int address, int value)
        {
            var offset = address - this.Offset;

            switch (offset)
            {
                case 0:
                    this.SetNr0(value);
                    break;

                case 1:
                    this.SetNr1(value);
                    break;

                case 2:
                    this.SetNr2(value);
                    break;

                case 3:
                    this.SetNr3(value);
                    break;

                case 4:
                    this.SetNr4(value);
                    break;
            }
        }

        public virtual int GetByte(int address)
        {
            var offset = address - this.Offset;

            return offset switch
            {
                0 => this.GetNr0(),
                1 => this.GetNr1(),
                2 => this.GetNr2(),
                3 => this.GetNr3(),
                4 => this.GetNr4(),
                _ => throw new ArgumentException("Illegal address for sound mode: " + Integer.ToHexString(address))
            };
        }


        protected virtual void SetNr4(int value)
        {
            this.Nr4 = value;
            this.Length.SetNr4(value);
            if ((value & (1 << 7)) != 0)
            {
                this.ChannelEnabled = this.DacEnabled;
                this.Trigger();
            }
        }

        protected virtual int GetFrequency()
        {
            return 2048 - (this.GetNr3() | ((this.GetNr4() & 0b111) << 8));
        }

        public abstract void Start();

        public void Stop() => this.ChannelEnabled = false;

        protected bool UpdateLength()
        {
            this.Length.Tick();
            if (!this.Length.Enabled)
            {
                return this.ChannelEnabled;
            }

            if (this.ChannelEnabled && this.Length.Length == 0)
            {
                this.ChannelEnabled = false;
            }

            return this.ChannelEnabled;
        }
    }
}