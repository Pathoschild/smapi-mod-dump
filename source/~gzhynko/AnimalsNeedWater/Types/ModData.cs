/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;

namespace AnimalsNeedWater.Types
{
    /// <summary> Contains global variables and constants for the mod. </summary>
    public static class ModData
    {
        public static List<string> CoopsWithWateredTrough { get; set; } = new List<string>();
        public static List<string> BarnsWithWateredTrough { get; set; } = new List<string>();
        public static List<FarmAnimal> FullAnimals { get; set; } = new List<FarmAnimal>();
        public static int LoveEmote { get; } = 20;
    }
}
