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

namespace CoreBoy.memory.cart.rtc
{
    public class SystemClock : IClock
    {
        public long CurrentTimeMillis() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}