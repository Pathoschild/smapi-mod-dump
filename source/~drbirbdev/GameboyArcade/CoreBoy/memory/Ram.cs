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
    public class Ram : IAddressSpace
    {
        private readonly int[] _space;
        private readonly int _length;
        private readonly int _offset;

        public Ram(int offset, int length)
        {
            this._space = new int[length];
            this._length = length;
            this._offset = offset;
        }

        public bool Accepts(int address) => address >= this._offset && address < this._offset + this._length;
        public void SetByte(int address, int value) => this._space[address - this._offset] = value;

        public int GetByte(int address)
        {
            var index = address - this._offset;
            if (index < 0 || index >= this._space.Length)
            {
                throw new IndexOutOfRangeException("Address: " + address);
            }

            return this._space[index];
        }
    }
}