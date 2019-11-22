using System;
using System.Reflection;
using StardewValley;
using Bookcase.Events;

namespace Bookcase.Patches {

    class PatchDayUpdate : IGamePatch {

        public Type TargetType => typeof(Farmer);

        public MethodBase TargetMethod => TargetType.GetMethod("dayupdate");

        public static void Prefix(Farmer __instance) {

            BookcaseEvents.FarmerStartDayEventPre.Post(new FarmerStartDayEvent(__instance));
        }

        public static void Postfix(Farmer __instance) {

            BookcaseEvents.FarmerStartDayEventPost.Post(new FarmerStartDayEvent(__instance));
        }
    }
}