/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/F1r3w477/TheseModsAintLoyal
**
*************************************************/

using System;

namespace MoodyPlanet
{
    public class ModConfig
    {
        public int seed { get; set; } = (int)(DateTime.Now.Ticks & 0x0000FFFF);
    }
}