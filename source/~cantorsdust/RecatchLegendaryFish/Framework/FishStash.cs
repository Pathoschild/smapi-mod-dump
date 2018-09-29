using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace RecatchLegendaryFish.Framework
{
    /// <summary>Temporarily hides caught legendary fish from the game.</summary>
    internal class FishStash
    {
        /*********
        ** Properties
        *********/
        /// <summary>The fish IDs for legendary fish.</summary>
        private readonly int[] LegendaryFishIDs = { 159, 160, 163, 682, 775 };

        /// <summary>A backup of legendaries caught by the current player.</summary>
        private readonly IDictionary<int, int[]> Stash = new Dictionary<int, int[]>();

        /// <summary>Whether a stash has been started.</summary>
        private bool IsStarted;


        /*********
        ** Accessors
        *********/
        /// <summary>Whether the player's legendary fish have been stashed.</summary>
        public bool IsStashed => this.IsStarted || this.Stash.Any();


        /*********
        ** Public methods
        *********/
        /// <summary>Stash any caught legendaries so they're not visible to the game.</summary>
        public void Start()
        {
            if (this.IsStashed)
                throw new InvalidOperationException("A stash has already been started.");

            foreach (int fishID in this.LegendaryFishIDs)
            {
                if (Game1.player.fishCaught.TryGetValue(fishID, out int[] freshValues))
                {
                    this.Stash[fishID] = freshValues;
                    Game1.player.fishCaught.Remove(fishID);
                }
            }

            this.IsStarted = true;
        }

        /// <summary>Restore any caught legendaries so they're visible to the game, and trigger fishing achievements if needed.</summary>
        public void Restore()
        {
            if (!this.IsStashed)
                return;

            // restore fish caught
            foreach (int fishID in this.Stash.Keys)
            {
                int[] stashedData = this.Stash[fishID];

                // merge new stats
                if (Game1.player.fishCaught.TryGetValue(fishID, out int[] newValues))
                {
                    int newCount = newValues[0];
                    int newSize = newValues[1];

                    stashedData[0] += newCount;
                    stashedData[1] = Math.Max(stashedData[1], newSize);
                }

                // restore stats
                Game1.player.fishCaught[fishID] = stashedData;
            }

            // trigger achievement if needed
            Game1.stats.checkForFishingAchievements();

            // clear stache
            this.Clear();
        }

        /// <summary>Reset the stash if it has any data.</summary>
        public void Clear()
        {
            this.Stash.Clear();
            this.IsStarted = false;
        }
    }
}
