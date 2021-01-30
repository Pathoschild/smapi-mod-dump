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
    /// <summary>
    /// Patches NPC.receiveGift and runs when an NPC receives a gift from a player.
    /// </summary>
    public class NPCReceiveGiftPatch : IGamePatch {
        public Type TargetType => typeof(NPC);

        public MethodBase TargetMethod => TargetType.GetMethod("receiveGift");

        public static bool Prefix(NPC __instance, ref StardewValley.Object o, ref Farmer giver, ref bool updateGiftLimitInfo, ref float friendshipChangeMultiplier, ref bool showResponse) {
            NPCReceiveGiftEvent args = new NPCReceiveGiftEvent(__instance, o, giver, updateGiftLimitInfo, friendshipChangeMultiplier, showResponse);
            BookcaseEvents.NPCReceiveGiftPre.Post(args);
            o = args.Gift;
            giver = args.Giver;
            updateGiftLimitInfo = args.UpdateGiftLimitInfo;
            friendshipChangeMultiplier = args.FriendshipChangeMultiplier;
            showResponse = args.ShowResponse;
            return !args.IsCanceled();
        }
    }
}
