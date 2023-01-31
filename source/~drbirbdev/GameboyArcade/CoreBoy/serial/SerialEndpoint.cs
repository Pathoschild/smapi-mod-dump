/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

namespace CoreBoy.serial
{
    public interface SerialEndpoint
    {
        int transfer(int outgoing);
    }


    public class NullSerialEndpoint : SerialEndpoint
    {
        public int transfer(int outgoing)
        {
            return 0xff;
        }
    }
}
