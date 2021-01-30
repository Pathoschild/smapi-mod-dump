/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using Bookcase.Events;
using StardewValley;
using System;
using System.Reflection;

namespace Bookcase.Patches {

    public class FishCaughtInfoPatch : IGamePatch {

        public Type TargetType => typeof(StardewValley.Tools.FishingRod);

        public MethodBase TargetMethod => TargetType.GetMethod("pullFishFromWater");

        public static void Prefix(Farmer ___lastUser, int whichFish, int fishSize, int fishQuality, int fishDifficulty, bool treasureCaught, bool wasPerfect = false) {

            FarmerCaughtFishEvent caughtEvent = new FarmerCaughtFishEvent(___lastUser, whichFish, fishSize, fishQuality, fishDifficulty, treasureCaught, wasPerfect);
            BookcaseEvents.FishCaughtInfo.Post(caughtEvent);
        }
    }
}