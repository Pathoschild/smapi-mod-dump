/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/PolyamorySweetLove
**
*************************************************/

using System;

namespace PolyamorySweetLove
{
    [Flags]
    public enum FarmHousePositions : byte
    {
        None = 0,
        Bed = 1,
        Kitchen = 2,
        Patio = 4,
        Porch = 8,
        Room = 0x10,
        Porch2 = 0x20,
        Random1 = 0x40,
        Random2 = 0x80,
        All = byte.MaxValue
    }
}
