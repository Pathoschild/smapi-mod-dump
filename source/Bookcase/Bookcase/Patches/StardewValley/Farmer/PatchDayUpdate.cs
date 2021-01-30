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