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
using StardewValley.Events;
using System;
using System.Reflection;

namespace Bookcase.Patches {

    public class PatchPickPersonalFarmEvent : IGamePatch {

        public Type TargetType => typeof(StardewValley.Utility);

        public MethodBase TargetMethod => TargetType.GetMethod("pickPersonalFarmEvent");

        public static void Postfix(ref FarmEvent __result) {

            SelectFarmEvent selectEvent = new SelectFarmEvent(__result);
            __result = BookcaseEvents.SelectPersonalEvent.Post(selectEvent) ? null : selectEvent.SelectedEvent;
        }
    }
}