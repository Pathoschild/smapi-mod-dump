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
    public class GbcRam : IAddressSpace
    {
        private readonly int[] _ram = new int[7 * 0x1000];
        private int _svbk;

        public bool Accepts(int address) => address == 0xff70 || (address >= 0xd000 && address < 0xe000);

        public void SetByte(int address, int value)
        {
            if (address == 0xff70)
            {
                _svbk = value;
            }
            else
            {
                _ram[Translate(address)] = value;
            }
        }

        public int GetByte(int address) => address == 0xff70 ? _svbk : _ram[Translate(address)];

        private int Translate(int address)
        {
            var ramBank = _svbk & 0x7;
            if (ramBank == 0)
            {
                ramBank = 1;
            }

            var result = address - 0xd000 + (ramBank - 1) * 0x1000;
            if (result < 0 || result >= _ram.Length)
            {
                throw new ArgumentException();
            }

            return result;
        }
    }
}