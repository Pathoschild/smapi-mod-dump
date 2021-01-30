/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

namespace AnimalsNeedWater
{
    /// <summary> The mod config class. More info here: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Config </summary>
    public class ModConfig
    {
        /// <summary>
        /// Whether to show "love" bubbles over animals inside the building when watered the trough.
        /// </summary>
        public bool ShowLoveBubblesOverAnimalsWhenWateredTrough { get; set; } = true;

        /// <summary>
        /// Whether to enable the watering system in Deluxe Coops and Deluxe Barns.
        /// </summary>
        public bool WateringSystemInDeluxeBuildings { get; set; } = true;

        /// <summary>
        /// Whether to replace coop's and big coop's textures when troughs inside them are empty.
        /// </summary>
        public bool ReplaceCoopTextureIfTroughIsEmpty { get; set; } = true;

        /// <summary>
        /// The amount of friendship points player gets for watering a trough.
        /// </summary>
        public int FriendshipPointsForWateredTrough { get; set; } = 15;

        /// <summary>
        /// The amount of friendship points player gets for watering a trough with animals inside the building.
        /// </summary>
        public int AdditionalFriendshipPointsForWateredTroughWithAnimalsInsideBuilding { get; set; } = 15;

        /// <summary>
        /// The amount of friendship points player loses for not watering a trough.
        /// </summary>
        public int NegativeFriendshipPointsForNotWateredTrough { get; set; } = 20;

        /// <summary>
        /// Whether animals can drink outside.
        /// </summary>
        public bool AnimalsCanDrinkOutside { get; set; } = true;

        /// <summary>
        /// Whether animals can only drink from lakes/rivers/seas etc. If set to false, animals will drink from any place you can refill your watering can at (well, troughs, water bodies etc.).
        /// </summary>
        public bool AnimalsCanOnlyDrinkFromWaterBodies { get; set; } = true;
        
        /// <summary>
        /// Whether troughs should have a cleaner texture.
        /// </summary>
        public bool CleanerTroughs { get; set; } = false;
    }
}
