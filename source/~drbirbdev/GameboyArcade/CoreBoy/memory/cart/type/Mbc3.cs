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
using CoreBoy.memory.cart.battery;
using CoreBoy.memory.cart.rtc;

namespace CoreBoy.memory.cart.type
{
    public class Mbc3 : IAddressSpace
    {
        private readonly int[] _cartridge;
        private readonly int[] _ram;
        private readonly RealTimeClock _clock;
        private readonly IBattery _battery;
        private int _selectedRamBank;
        private int _selectedRomBank = 1;
        private bool _ramWriteEnabled;
        private int _latchClockReg = 0xff;
        private bool _clockLatched;

        public Mbc3(int[] cartridge, CartridgeType type, IBattery battery, int romBanks, int ramBanks)
        {
            this._cartridge = cartridge;
            this._ram = new int[0x2000 * Math.Max(ramBanks, 1)];
            for (var i = 0; i < this._ram.Length; i++)
            {
                this._ram[i] = 0xff;
            }

            this._clock = new RealTimeClock(Clock.SystemClock);
            this._battery = battery;

            var clockData = new long[12];
            battery.LoadRamWithClock(this._ram, clockData);
            this._clock.Deserialize(clockData);
        }


        public bool Accepts(int address)
        {
            return (address >= 0x0000 && address < 0x8000) ||
                   (address >= 0xa000 && address < 0xc000);
        }


        public void SetByte(int address, int value)
        {
            if (address >= 0x0000 && address < 0x2000)
            {
                this._ramWriteEnabled = (value & 0b1010) != 0;
                if (!this._ramWriteEnabled)
                {
                    this._battery.SaveRamWithClock(this._ram, this._clock.Serialize());
                }
            }
            else if (address >= 0x2000 && address < 0x4000)
            {
                var bank = value & 0b01111111;
                this.SelectRomBank(bank);
            }
            else if (address >= 0x4000 && address < 0x6000)
            {
                this._selectedRamBank = value;
            }
            else if (address >= 0x6000 && address < 0x8000)
            {
                if (value == 0x01 && this._latchClockReg == 0x00)
                {
                    if (this._clockLatched)
                    {
                        this._clock.Unlatch();
                        this._clockLatched = false;
                    }
                    else
                    {
                        this._clock.Latch();
                        this._clockLatched = true;
                    }
                }

                this._latchClockReg = value;
            }
            else if (address >= 0xa000 && address < 0xc000 && this._ramWriteEnabled && this._selectedRamBank < 4)
            {
                var ramAddress = this.GetRamAddress(address);
                if (ramAddress < this._ram.Length)
                {
                    this._ram[ramAddress] = value;
                }
            }
            else if (address >= 0xa000 && address < 0xc000 && this._ramWriteEnabled && this._selectedRamBank >= 4)
            {
                this.SetTimer(value);
            }
        }

        private void SelectRomBank(int bank)
        {
            if (bank == 0)
            {
                bank = 1;
            }

            this._selectedRomBank = bank;
        }


        public int GetByte(int address)
        {
            if (address >= 0x0000 && address < 0x4000)
            {
                return this.GetRomByte(0, address);
            }
            else if (address >= 0x4000 && address < 0x8000)
            {
                return this.GetRomByte(this._selectedRomBank, address - 0x4000);
            }
            else if (address >= 0xa000 && address < 0xc000 && this._selectedRamBank < 4)
            {
                var ramAddress = this.GetRamAddress(address);
                if (ramAddress < this._ram.Length)
                {
                    return this._ram[ramAddress];
                }
                else
                {
                    return 0xff;
                }
            }
            else if (address >= 0xa000 && address < 0xc000 && this._selectedRamBank >= 4)
            {
                return this.GetTimer();
            }
            else
            {
                throw new ArgumentException(Integer.ToHexString(address));
            }
        }

        private int GetRomByte(int bank, int address)
        {
            var cartOffset = (bank * 0x4000) + address;
            if (cartOffset < this._cartridge.Length)
            {
                return this._cartridge[cartOffset];
            }
            else
            {
                return 0xff;
            }
        }

        private int GetRamAddress(int address)
        {
            return (this._selectedRamBank * 0x2000) + (address - 0xa000);
        }

        private int GetTimer()
        {
            switch (this._selectedRamBank)
            {
                case 0x08:
                    return this._clock.GetSeconds();

                case 0x09:
                    return this._clock.GetMinutes();

                case 0x0a:
                    return this._clock.GetHours();

                case 0x0b:
                    return this._clock.GetDayCounter() & 0xff;

                case 0x0c:
                    var result = (this._clock.GetDayCounter() & 0x100) >> 8;
                    result |= this._clock.IsHalt() ? (1 << 6) : 0;
                    result |= this._clock.IsCounterOverflow() ? (1 << 7) : 0;
                    return result;
            }

            return 0xff;
        }

        private void SetTimer(int value)
        {
            var dayCounter = this._clock.GetDayCounter();
            switch (this._selectedRamBank)
            {
                case 0x08:
                    this._clock.SetSeconds(value);
                    break;

                case 0x09:
                    this._clock.SetMinutes(value);
                    break;

                case 0x0a:
                    this._clock.SetHours(value);
                    break;

                case 0x0b:
                    this._clock.SetDayCounter((dayCounter & 0x100) | (value & 0xff));
                    break;

                case 0x0c:
                    this._clock.SetDayCounter((dayCounter & 0xff) | ((value & 1) << 8));
                    this._clock.SetHalt((value & (1 << 6)) != 0);
                    if ((value & (1 << 7)) == 0)
                    {
                        this._clock.ClearCounterOverflow();
                    }

                    break;
            }
        }
    }
}