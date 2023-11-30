/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using CoreBoy.memory.cart.battery;

namespace CoreBoy.memory.cart.type
{
    public class Mbc2 : IAddressSpace
    {
        private readonly int[] _cartridge;
        private readonly int[] _ram;
        private readonly IBattery _battery;
        private int _selectedRomBank = 1;
        private bool _ramWriteEnabled;

        public Mbc2(int[] cartridge, CartridgeType type, IBattery battery, int romBanks)
        {
            this._cartridge = cartridge;
            this._ram = new int[0x0200];
            for (var i = 0; i < this._ram.Length; i++)
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
                if ((address & 0x0100) == 0)
                {
                    this._ramWriteEnabled = (value & 0b1010) != 0;
                    if (!this._ramWriteEnabled)
                    {
                        this._battery.SaveRam(this._ram);
                    }
                }
            }
            else if (address >= 0x2000 && address < 0x4000)
            {
                if ((address & 0x0100) != 0)
                {
                    this._selectedRomBank = value & 0b00001111;
                }
            }
            else if (address >= 0xa000 && address < 0xc000 && this._ramWriteEnabled)
            {
                var ramAddress = GetRamAddress(address);
                if (ramAddress < this._ram.Length)
                {
                    this._ram[ramAddress] = value & 0x0f;
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

            if (address >= 0xa000 && address < 0xb000)
            {
                var ramAddress = GetRamAddress(address);
                if (ramAddress < this._ram.Length)
                {
                    return this._ram[ramAddress];
                }

                return 0xff;
            }

            return 0xff;
        }

        private int GetRomByte(int bank, int address)
        {
            var cartOffset = (bank * 0x4000) + address;
            if (cartOffset < this._cartridge.Length)
            {
                return this._cartridge[cartOffset];
            }

            return 0xff;
        }

        private static int GetRamAddress(int address) => address - 0xa000;
    }
}