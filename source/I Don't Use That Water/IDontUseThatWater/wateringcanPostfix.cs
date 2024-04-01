/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/iDontUseThatWater
**
*************************************************/

using iDontUseThatWater;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeawaterIsUnusable
{
    internal class WateringcanPostfix
    {
        private static readonly IMonitor monitor;

        public static void IsProperForWaterSource(int tileX, int tileY, ref bool __result)
        {
            try
            {
                if (Game1.currentLocation.doesTileHavePropertyNoNull(tileX, tileY, "WaterSource", "Back").ToUpper() == "F")
                {
                    __result = false;
                }
                return;
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed in {nameof(IsProperForWaterSource)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
