/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

namespace CoreBoy.cpu
{
    public static class BitUtils
    {
        public static int GetMsb(int word) => word >> 8;
        public static int GetLsb(int word) => word & 0xff;
        public static int ToWord(int[] bytes) => ToWord(bytes[1], bytes[0]);
        public static int ToWord(int msb, int lsb) => (msb << 8) | lsb;
        public static bool GetBit(int byteValue, int position) => (byteValue & (1 << position)) != 0;
        public static int SetBit(int byteValue, int position, bool value) => value ? SetBit(byteValue, position) : ClearBit(byteValue, position);
        public static int SetBit(int byteValue, int position) => (byteValue | (1 << position)) & 0xff;
        public static int ClearBit(int byteValue, int position) => ~(1 << position) & byteValue & 0xff;
        public static int ToSigned(int byteValue) => (byteValue & (1 << 7)) == 0 ? byteValue : byteValue - 0x100;
    }
}