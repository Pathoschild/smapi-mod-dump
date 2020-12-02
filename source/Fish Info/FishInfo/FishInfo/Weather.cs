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
    internal enum Weather
    {
        None = 0,
        Sun = 1 << 0,
        Rain = 1 << 1
    }
}
