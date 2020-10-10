/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

namespace FarmHouseRedone
{
    class Pet_warpToFarmHouse_Patch
    {
        public static void Postfix(Farmer who, Pet __instance)
        {
            if (__instance.isSleepingOnFarmerBed && __instance.currentLocation is FarmHouse)
            {
                __instance.position.Value = (StardewValley.Utility.PointToVector2(FarmHouseStates.getBedSpot(StardewValley.Utility.getHomeOfFarmer(who))) + new Microsoft.Xna.Framework.Vector2(-1f, 0f)) * 64f;
            }
        }
    }
}
