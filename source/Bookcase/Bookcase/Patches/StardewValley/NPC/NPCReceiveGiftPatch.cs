using Bookcase.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
