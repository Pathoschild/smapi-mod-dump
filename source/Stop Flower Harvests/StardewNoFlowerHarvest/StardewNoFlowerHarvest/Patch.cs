/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seichen/Stardew-Stop-Flower-Harvests
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley.GameData.Crops;
using StardewValley;

namespace StopFlowerHarvests
{
    internal class Patch
    {
        public static HarvestMethod Postfix_GetHarvestMethod(HarvestMethod __result, Crop __instance)
        {
            if (__instance.GetData().HarvestItemId.Equals("595"))
            {
                return HarvestMethod.Scythe;
            }
            return __result;
        }
    }
}
