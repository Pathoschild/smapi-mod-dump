using System;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace Demiacle.ImprovedQualityOfLife {
    internal class QuickFish {

        /// <summary>
        /// This mod advances the time forward when waiting to catch a fish
        /// </summary>
        public QuickFish() {
            GameEvents.UpdateTick += speedTimeWhenFishing;
        }

        /// <summary>
        /// Advances the 10 minute clock tick based on the amount of time the fish is suppose to take before a fish bites
        /// </summary>
        private void speedTimeWhenFishing( object sender, EventArgs e ) {
            if( Game1.player.CurrentTool is FishingRod ) {

                var fishingRod = (FishingRod) Game1.player.CurrentTool;

                // Standard 10 minute mark is 7 seconds
                if( fishingRod.timeUntilFishingBite > 7000 && fishingRod.hit == false && fishingRod.isReeling == false && fishingRod.pullingOutOfWater == false && fishingRod.fishCaught == false ) {
                    fishingRod.timeUntilFishingBite -= 7000;
                    Game1.performTenMinuteClockUpdate();
                }
            }
        }

    }
}