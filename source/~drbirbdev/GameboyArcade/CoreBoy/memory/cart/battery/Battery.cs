/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

namespace CoreBoy.memory.cart.battery
{
    public interface IBattery
    {
        void LoadRam(int[] ram);

        void SaveRam(int[] ram);

        void LoadRamWithClock(int[] ram, long[] clockData);

        void SaveRamWithClock(int[] ram, long[] clockData);

    }
}