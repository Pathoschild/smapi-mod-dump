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

namespace CoreBoy.memory.cart.type
{
    public class Mbc5 : IAddressSpace
    {
        private readonly int _ramBanks;
        private readonly int[] _cartridge;
        private readonly int[] _ram;
        private readonly IBattery _battery;
        private int _selectedRamBank;
        private int _selectedRomBank = 1;
        private bool _ramWriteEnabled;

        public Mbc5(int[] cartridge, CartridgeType type, IBattery battery, int romBanks, int ramBanks)
        {
            this._cartridge = cartridge;
            this._ramBanks = ramBanks;
            this._ram = new int[0x2000 * Math.Max(this._ramBanks, 1)];
            for (int i = 0; i < this._ram.Length; i++)
            {
                this._ram[i] = 0xff;
            }

            this._battery = battery;
            battery.LoadRam(this._ram);
        }

        public bool Accepts(int address) => (address >= 0x0000 && address < 0x8000) || (address >= 0xa000 && address < 0xc000);

        public void SetByte(int address, int value)
        {
            if (address >= 0x0000 && address < 0x2000)
            {
                this._ramWriteEnabled = (value & 0b1010) != 0;
                if (!this._ramWriteEnabled)
                {
                    this._battery.SaveRam(this._ram);
                }
            }
            else if (address >= 0x2000 && address < 0x3000)
            {
                this._selectedRomBank = (this._selectedRomBank & 0x100) | value;
            }
            else if (address >= 0x3000 && address < 0x4000)
            {
                this._selectedRomBank = (this._selectedRomBank & 0x0ff) | ((value & 1) << 8);
            }
            else if (address >= 0x4000 && address < 0x6000)
            {
                int bank = value & 0x0f;
                if (bank < this._ramBanks)
                {
                    this._selectedRamBank = bank;
                }
            }
            else if (address >= 0xa000 && address < 0xc000 && this._ramWriteEnabled)
            {
                int ramAddress = this.GetRamAddress(address);
                if (ramAddress < this._ram.Length)
                {
                    this._ram[ramAddress] = value;
                }
            }
        }

        public int GetByte(int address)
        {
            if (address >= 0x0000 && address < 0x4000)
            {
                return this.GetRomByte(0, address);
            }

            if (address >= 0x4000 && address < 0x8000)
            {
                return this.GetRomByte(this._selectedRomBank, address - 0x4000);
            }

            if (address >= 0xa000 && address < 0xc000)
            {
                int ramAddress = this.GetRamAddress(address);
                if (ramAddress < this._ram.Length)
                {
                    return this._ram[ramAddress];
                }

                return 0xff;
            }

            throw new ArgumentException(Integer.ToHexString(address));
        }

        private int GetRomByte(int bank, int address)
        {
            int cartOffset = (bank * 0x4000) + address;
            if (cartOffset < this._cartridge.Length)
            {
                return this._cartridge[cartOffset];
            }

            return 0xff;
        }

        private int GetRamAddress(int address) => (this._selectedRamBank * 0x2000) + (address - 0xa000);
    }
}