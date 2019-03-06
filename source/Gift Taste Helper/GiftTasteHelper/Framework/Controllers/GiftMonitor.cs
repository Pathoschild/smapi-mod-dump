using System.Collections.Generic;
using Netcode;
using StardewValley;
using StardewValley.Network;

namespace GiftTasteHelper.Framework
{
    internal class GiftMonitor : IGiftMonitor
    {
        public event GiftGivenDelegate GiftGiven;

        private StardewValley.Object ActiveObject => Game1.player.ActiveObject;
        private uint GiftsGiven => Game1.stats.GiftsGiven;

        // 0 = Friendship level
        // 1 = gifts given this week
        // 2 = has player talked to them today
        // 3 = has player given them a gift today
        // 4 = has proposed when friendship level too low?
        // 5 = ?
        private NetStringDictionary<Friendship, NetRef<Friendship>> Friendships => Game1.player.friendshipData;

        // Last known number of gifts given so we can check when the stat value changes.
        private uint PriorGiftsGiven;
        // Currently held gift.
        private StardewValley.Object HeldGift;
        // Last known state of which npc's have been given gifts. Must be reset when the day changes.
        private Dictionary<string, bool> GiftsGivenToday;

        public bool IsHoldingValidGift => this.HeldGift != null;

        /// <summary>Initializes the GiftMonitor.</summary>
        public void Load()
        {
            this.PriorGiftsGiven = this.GiftsGiven;
            this.HeldGift = null;
            RebuildGiftsGiven();
        }

        /// <summary>Resets the tracking of who has been given gifts today. Should be called after load.</summary>
        public void Reset()
        {
            RebuildGiftsGiven();
        }

        /// <summary>Sets the internally held item to what the player is currently holding. Should be called when right click it pressed.</summary>
        public void UpdateHeldGift()
        {
            if (this.ActiveObject != null && this.ActiveObject.canBeGivenAsGift())
            {
                Utils.DebugLog($"Set held item to {this.ActiveObject.Name}");
                this.HeldGift = this.ActiveObject;
            }
        }

        /// <summary>Checks if a gift has been given to an npc and invokes the GiftGiven event if so. Should be called on right mouse up.</summary>
        public void CheckGiftGiven()
        {
            if (this.HeldGift == null)
            {
                return;
            }

            // If the stat value changed then a gift was given.
            if (this.GiftsGiven != this.PriorGiftsGiven)
            {
                string npcGivenTo = null;
                foreach (var friendpair in this.Friendships.Pairs)
                {
                    if (!this.GiftsGivenToday.ContainsKey(friendpair.Key))
                    {
                        Utils.DebugLog($"GiftsGivenToday does not contain {friendpair.Key}; adding to list.");
                        this.GiftsGivenToday.Add(friendpair.Key, false);
                    }

                    // Find whose 'given today' state has changed.
                    bool givenToday = friendpair.Value.GiftsToday > 0;
                    if (this.GiftsGivenToday[friendpair.Key] != givenToday)
                    {
                        this.GiftsGivenToday[friendpair.Key] = true;
                        npcGivenTo = friendpair.Key;
                        break;
                    }
                }

                this.PriorGiftsGiven = this.GiftsGiven;
                var itemId = this.HeldGift.ParentSheetIndex;
                this.HeldGift = null;

                if (Utils.Ensure(npcGivenTo != null, "NPC given to is null!"))
                {
                    // Notify a gift was given.
                    GiftGiven(npcGivenTo, itemId);
                }
            }
        }

        private void RebuildGiftsGiven()
        {
            this.GiftsGivenToday = new Dictionary<string, bool>();
            foreach (var friendpair in this.Friendships.Pairs)
            {
                // Third element is whether a gift has been given today.
                GiftsGivenToday.Add(friendpair.Key, friendpair.Value.GiftsToday > 0);
            }
        }
    }
}
