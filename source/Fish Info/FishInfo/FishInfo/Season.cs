/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/FishInfo
**
*************************************************/

using System;

namespace FishInfo
{
    [Flags]
    internal enum Season
    {
        None = 0,
        Spring = 1 << 0,
        Summer = 1 << 1,
        Fall = 1 << 2,
        Winter = 1 << 3,

        All_seasons = Spring | Summer | Fall | Winter
    }
}
