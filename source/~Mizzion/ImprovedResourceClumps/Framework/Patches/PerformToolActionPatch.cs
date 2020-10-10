/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImprovedResourceClumps.Framework.Configs;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace ImprovedResourceClumps.Framework.Patches
{
    /*
        public const int stumpIndex = 600;
        public const int hollowLogIndex = 602;
        public const int meteoriteIndex = 622;
        public const int boulderIndex = 672;
        public const int mineRock1Index = 752;
        public const int mineRock2Index = 754;
        public const int mineRock3Index = 756;
        public const int mineRock4Index = 758;
     */
    internal class PerformToolActionPatch
    {
        public static IMonitor monitor;
        public static IModHelper helper;
        public static IrcConfig config;

        public PerformToolActionPatch(IMonitor _monitor, IModHelper _helper, IrcConfig _config)
        {
            monitor = _monitor;
            helper = _helper;
            config = _config;
        }

        public static bool performToolAction(ref ResourceClump __instance, ref Tool t, ref int damage,
            ref Vector2 tileLocation, ref GameLocation location)
        {
            ResourceClump rc = __instance;

            //Start checking ParentSheetIndexes for the ResourceClumps
            if (rc.health.Value > 0.0)
                return true;

            if (rc.parentSheetIndex.Value == 600)
            {

                
            }

            //Everything else failed. The original Method runs.
            return true;
        }
    }
}
