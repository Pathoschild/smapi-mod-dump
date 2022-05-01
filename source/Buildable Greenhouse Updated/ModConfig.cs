/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/BuildableGreenhouse
**
*************************************************/

using System.Collections.Generic;
using Object = StardewValley.Object;

namespace BuildableGreenhouse
{
    internal class ModConfig
    {
        public int BuildPrice { get; set; } = 100000;

        public Dictionary<int, int> BuildMaterals { get; set; } = new()
        {
            [Object.stone] = 500,
            [709 /* Hardwood */] = 100,
            [Object.iridiumBar] = 5
        };
    }
}