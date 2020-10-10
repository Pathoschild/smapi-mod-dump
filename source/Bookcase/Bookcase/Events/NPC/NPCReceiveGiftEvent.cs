/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookcase.Events {
    /// <summary>
    /// Event fired when an NPC is given a gift.
    /// </summary>
    public class NPCReceiveGiftEvent : Event {
        /// <summary>
        /// The NPC being given the gift. Immutable.
        /// </summary>
        public NPC Target { get; }

        /// <summary>
        /// The Gift being given.
        /// </summary>
        public StardewValley.Object Gift { get; set; }

        /// <summary>
        /// The player/farmer giving the gift.
        /// </summary>
        public Farmer Giver { get; set; }

        /// <summary>
        /// If this gift will count against the daily/weekly gift limit
        /// </summary>
        public bool UpdateGiftLimitInfo { get; set; }

        /// <summary>
        /// Flat multiplier to add to friendship gained. (untested)
        /// </summary>
        public float FriendshipChangeMultiplier { get; set; }

        /// <summary>
        /// If an appropriate text/emote response should be shown. Default true.
        /// </summary>
        public bool ShowResponse { get; set; }

        public NPCReceiveGiftEvent(NPC instance, StardewValley.Object o, Farmer giver, bool updateGiftLimitInfo, float friendshipChangeMultiplier, bool showResponse) {
            Target = instance;
            Gift = o;
            Giver = giver;
            UpdateGiftLimitInfo = updateGiftLimitInfo;
            FriendshipChangeMultiplier = friendshipChangeMultiplier;
            ShowResponse = showResponse;
        }

        public override bool CanCancel() {
            return true;
        }
    }
}
