using System;
using System.Reflection;
using Bookcase.Events;
using StardewValley;

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