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
using CoreBoy.memory;

namespace CoreBoy.sound
{
    public class SoundMode3 : SoundModeBase
    {
        private static readonly int[] DmgWave =
        {
            0x84, 0x40, 0x43, 0xaa, 0x2d, 0x78, 0x92, 0x3c,
            0x60, 0x59, 0x59, 0xb0, 0x34, 0xb8, 0x2e, 0xda
        };

        private static readonly int[] CgbWave =
        {
            0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff,
            0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff
        };

        private readonly Ram WaveRam = new Ram(0xff30, 0x10);
        private int FreqDivider;
        private int LastOutput;
        private int I;
        private int TicksSinceRead = 65536;
        private int LastReadAddress;
        private int Buffer;
        private bool Triggered;

        public SoundMode3(bool gbc) : base(0xff1a, 256, gbc)
        {
            foreach (int v in gbc ? CgbWave : DmgWave)
            {
                this.WaveRam.SetByte(0xff30, v);
            }
        }

        public override bool Accepts(int address) => this.WaveRam.Accepts(address) || base.Accepts(address);

        public override int GetByte(int address)
        {
            if (!this.WaveRam.Accepts(address))
            {
                return base.GetByte(address);
            }

            if (!this.IsEnabled())
            {
                return this.WaveRam.GetByte(address);
            }

            if (this.WaveRam.Accepts(this.LastReadAddress) && (this.Gbc || this.TicksSinceRead < 2))
            {
                return this.WaveRam.GetByte(this.LastReadAddress);
            }

            return 0xff;
        }


        public override void SetByte(int address, int value)
        {
            if (!this.WaveRam.Accepts(address))
            {
                base.SetByte(address, value);
                return;
            }

            if (!this.IsEnabled())
            {
                this.WaveRam.SetByte(address, value);
            }
            else if (this.WaveRam.Accepts(this.LastReadAddress) && (this.Gbc || this.TicksSinceRead < 2))
            {
                this.WaveRam.SetByte(this.LastReadAddress, value);
            }
        }

        protected override void SetNr0(int value)
        {
            base.SetNr0(value);
            this.DacEnabled = (value & (1 << 7)) != 0;
            this.ChannelEnabled &= this.DacEnabled;
        }

        protected override void SetNr1(int value)
        {
            base.SetNr1(value);
            this.Length.SetLength(256 - value);
        }

        protected override void SetNr4(int value)
        {
            if (!this.Gbc && (value & (1 << 7)) != 0)
            {
                if (this.IsEnabled() && this.FreqDivider == 2)
                {
                    int pos = this.I / 2;
                    if (pos < 4)
                    {
                        this.WaveRam.SetByte(0xff30, this.WaveRam.GetByte(0xff30 + pos));
                    }
                    else
                    {
                        pos &= ~3;
                        for (int j = 0; j < 4; j++)
                        {
                            this.WaveRam.SetByte(0xff30 + j, this.WaveRam.GetByte(0xff30 + ((pos + j) % 0x10)));
                        }
                    }
                }
            }

            base.SetNr4(value);
        }

        public override void Start()
        {
            this.I = 0;
            this.Buffer = 0;
            if (this.Gbc)
            {
                this.Length.Reset();
            }

            this.Length.Start();
        }

        protected override void Trigger()
        {
            this.I = 0;
            this.FreqDivider = 6;
            this.Triggered = !this.Gbc;
            if (this.Gbc)
            {
                this.GetWaveEntry();
            }
        }

        public override int Tick()
        {
            this.TicksSinceRead++;
            if (!this.UpdateLength())
            {
                return 0;
            }

            if (!this.DacEnabled)
            {
                return 0;
            }

            if ((this.GetNr0() & (1 << 7)) == 0)
            {
                return 0;
            }

            this.FreqDivider--;

            if (this.FreqDivider == 0)
            {
                this.ResetFreqDivider();
                if (this.Triggered)
                {
                    this.LastOutput = (this.Buffer >> 4) & 0x0f;
                    this.Triggered = false;
                }
                else
                {
                    this.LastOutput = this.GetWaveEntry();
                }

                this.I = (this.I + 1) % 32;
            }

            return this.LastOutput;
        }

        private int GetVolume() => (this.GetNr2() >> 5) & 0b11;

        private int GetWaveEntry()
        {
            this.TicksSinceRead = 0;
            this.LastReadAddress = 0xff30 + (this.I / 2);
            this.Buffer = this.WaveRam.GetByte(this.LastReadAddress);

            int b = this.Buffer;
            if (this.I % 2 == 0)
            {
                b = (b >> 4) & 0x0f;
            }
            else
            {
                b &= 0x0f;
            }

            return this.GetVolume() switch
            {
                0 => 0,
                1 => b,
                2 => b >> 1,
                3 => b >> 2,
                _ => throw new InvalidOperationException("Illegal state")
            };
        }

        private void ResetFreqDivider() => this.FreqDivider = this.GetFrequency() * 2;
    }
}
