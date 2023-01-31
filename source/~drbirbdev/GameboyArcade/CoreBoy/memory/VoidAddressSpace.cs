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
    public class VoidAddressSpace : IAddressSpace
    {
        public bool Accepts(int address) => true;

        public void SetByte(int address, int value)
        {
            if (address < 0 || address > 0xffff)
            {
                throw new ArgumentException("Invalid address: " + Integer.ToHexString(address));
            }

            //LOG.debug("Writing value {} to void address {}", Integer.toHexString(value), int.ToHexString(address));
        }

        public int GetByte(int address)
        {
            if (address < 0 || address > 0xffff)
            {
                throw new ArgumentException("Invalid address: " + Integer.ToHexString(address));
            }

            //LOG.debug("Reading value from void address {}", Integer.toHexString(address));
            return 0xff;
        }
    }
}