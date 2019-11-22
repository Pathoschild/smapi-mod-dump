using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelationshipTooltips
{
    /// <summary>
    /// Represents a list of all gifts given, and to which NPC.
    /// </summary>
    public class GiftSaveInfo
    {
        /// <summary>
        /// Gifts that have been recorded as given
        /// </summary>
        public List<NPCGift> giftsMade;
        /// <summary>
        /// Stores the gift giving event described by the NPC receiving and the gift given.
        /// </summary>
        /// <param name="npc">The NPC receiving the gift</param>
        /// <param name="gift">The Gift being given</param>
        public void AddGift(StardewValley.NPC npc, StardewValley.Item gift)
        {
            NPCGift equivalent = new NPCGift(npc.name, gift.Name);
            if (!giftsMade.Contains(equivalent))
                giftsMade.Add(equivalent);
        }
        public GiftSaveInfo()
        {
            giftsMade = new List<NPCGift>();
        }
        /// <summary>
        /// Checks if the player has given the gift - not retroactive - requires mod to be active to track
        /// </summary>
        /// <param name="npcName">The NPC being given the gift. (NPC.name)</param>
        /// <param name="giftName">The gift's name (Item.Name)</param>
        /// <returns></returns>
        public bool PlayerHasGifted(string npcName, string giftName)
        {
            return giftsMade.FirstOrDefault(e => e.NPC == npcName && e.Gift == giftName) != null;
        }
        /// <summary>
        /// A given gift
        /// </summary>
        public class NPCGift
        {
            /// <summary>
            /// NPC Name
            /// </summary>
            public string NPC;
            /// <summary>
            /// Gift Item Name
            /// </summary>
            public string Gift;

            public NPCGift(string npc, string gift)
            {
                NPC = npc;
                Gift = gift;
            }
        }
    }
}
