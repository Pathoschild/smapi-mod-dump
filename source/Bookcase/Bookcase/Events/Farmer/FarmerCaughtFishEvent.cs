using StardewValley;

namespace Bookcase.Events {

    /// <summary>
    /// Immutable information about the currently caught fish - intercepted from the Networking call by FishingRod.
    /// </summary>
    public class FarmerCaughtFishEvent : FarmerEvent {

        /// <summary>
        /// The fish's object id.
        /// </summary>
        public int FishID { get; private set; }

        /// <summary>
        /// The size of the fish in inches.
        /// </summary>
        public int FishSize { get; private set; }

        /// <summary>
        /// The quality of the fish.
        /// </summary>
        public int FishQuality { get; private set; }

        /// <summary>
        /// The difficulty of the fishing minigame,  0 if the item was caught without playing the minigame - see BobberBar.update(GameTime).
        /// </summary>
        public int FishDifficulty { get; private set; }

        /// <summary>
        /// If there was a treasure caught aswell.
        /// </summary>
        public bool TreasureCaught { get; private set; }

        /// <summary>
        /// If the fishing minigame was completed with a perfect score.
        /// </summary>
        public bool WasPerfect { get; private set; }

        internal FarmerCaughtFishEvent(Farmer lastUser, int fishID, int fishSize, int fishQuality, int fishDifficulty, bool treasureCaught, bool wasPerfect) : base(lastUser) {

            FishID = fishID;
            FishSize = fishSize;
            FishQuality = fishQuality;
            FishDifficulty = fishDifficulty;
            TreasureCaught = treasureCaught;
            WasPerfect = wasPerfect;
        }
    }
}