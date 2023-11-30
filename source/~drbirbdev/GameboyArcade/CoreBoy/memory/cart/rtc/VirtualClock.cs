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
    public class VirtualClock : IClock
    {
        private DateTimeOffset _clock = DateTimeOffset.UtcNow;
        public long CurrentTimeMillis() => this._clock.ToUnixTimeMilliseconds();
        public void Forward(TimeSpan time) => this._clock += time;
    }
}