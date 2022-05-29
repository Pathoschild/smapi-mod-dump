/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/


using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace LoadMenuTweaks
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int MaxTrees { get; set; } = 20;
        public int TreeChancePercent { get; set; } = 5;
        public int TreeGrowthStage { get; set; } = 5;
        public int ObjectChancePercent { get; set; } = 2;
    }
}
