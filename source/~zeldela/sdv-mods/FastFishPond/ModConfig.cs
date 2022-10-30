/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zeldela/sdv-mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;

namespace FastFishPond
{
    internal class ModConfig
    {
        public int spawnTime { get; set; } = 1;
        public bool vanilla { get; set; } = true;
        public float multiplier { get; set; } = 1.5f;
    }
}