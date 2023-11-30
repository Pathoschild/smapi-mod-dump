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

namespace CoreBoy.memory
{
    public class UndocumentedGbcRegisters : IAddressSpace
    {
        private readonly Ram _ram = new Ram(0xff72, 6);
        private int _xff6C;

        public UndocumentedGbcRegisters()
        {
            this._xff6C = 0xfe;
            this._ram.SetByte(0xff74, 0xff);
            this._ram.SetByte(0xff75, 0x8f);
        }

        public bool Accepts(int address) => address == 0xff6c || this._ram.Accepts(address);

        public void SetByte(int address, int value)
        {
            switch (address)
            {
                case 0xff6c:
                    this._xff6C = 0xfe | (value & 1);
                    break;

                case 0xff72:
                case 0xff73:
                case 0xff74:
                    this._ram.SetByte(address, value);
                    break;

                case 0xff75:
                    this._ram.SetByte(address, 0x8f | (value & 0b01110000));
                    break;
            }
        }

        public int GetByte(int address)
        {
            if (address == 0xff6c)
            {
                return this._xff6C;
            }

            if (!this._ram.Accepts(address))
            {
                throw new ArgumentException();
            }

            return this._ram.GetByte(address);
        }
    }
}