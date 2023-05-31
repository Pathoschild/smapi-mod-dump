/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

namespace FishingTrawler.Framework.Utilities
{
    public static class ModDataKeys
    {
        // General
        internal const string REWARD_CHEST_DATA_KEY = "PeacefulEnd.FishingTrawler_RewardChest";
        internal const string HOISTED_FLAG_KEY = "PeacefulEnd.FishingTrawler_HoistedFlag";

        // Murphy related
        internal const string MURPHY_WAS_GREETED_TODAY_KEY = "PeacefulEnd.FishingTrawler_MurphyGreeted";
        internal const string MURPHY_SAILED_TODAY_KEY = "PeacefulEnd.FishingTrawler_MurphySailedToday";
        internal const string MURPHY_WAS_TRIP_SUCCESSFUL_KEY = "PeacefulEnd.FishingTrawler_MurphyTripSuccessful";
        internal const string MURPHY_FINISHED_TALKING_KEY = "PeacefulEnd.FishingTrawler_MurphyFinishedTalking";
        internal const string MURPHY_HAS_SEEN_FLAG_KEY = "PeacefulEnd.FishingTrawler_MurphyHasSeenFlag";
        internal const string MURPHY_ON_TRIP = "PeacefulEnd.FishingTrawler_MurphyOnTrip";
        internal const string MURPHY_DAY_TO_APPEAR = "PeacefulEnd.FishingTrawler_MurphyDayToAppear";
        internal const string MURPHY_DAY_TO_APPEAR_ISLAND = "PeacefulEnd.FishingTrawler_MurphyDayToAppearIsland";
        internal const string MURPHY_TRIPS_COMPLETED = "PeacefulEnd.FishingTrawler_MurphyTripsCompleted";

        // Item related
        internal const string BAILING_BUCKET_KEY = "PeacefulEnd.FishingTrawler_BailingBucket";
        internal const string BAILING_BUCKET_CONTAINS_WATER = "PeacefulEnd.FishingTrawler_BailingBucket.ContainsWater";
        internal const string BAILING_BUCKET_SCALE = "PeacefulEnd.FishingTrawler_BailingBucket.Scale";

        internal const string TRIDENT_TOOL_KEY = "PeacefulEnd.FishingTrawler_Trident";
        internal const string TRIDENT_TOOL_FISH_ID = "PeacefulEnd.FishingTrawler_Trident.FishId";
        internal const string TRIDENT_TOOL_FISH_SIZE = "PeacefulEnd.FishingTrawler_Trident.FishSize";
        internal const string TRIDENT_TOOL_FISH_QUALITY = "PeacefulEnd.FishingTrawler_Trident.FishQuality";
        internal const string TRIDENT_TOOL_FISH_COUNT = "PeacefulEnd.FishingTrawler_Trident.FishCount";

        // Rewards related
        internal const string ANCIENT_FLAG_KEY = "PeacefulEnd.FishingTrawler_AncientFlag";
        internal const string ANGLER_RING_KEY = "PeacefulEnd.FishingTrawler_AnglerRing";
        internal const string LOST_FISHING_CHARM_KEY = "PeacefulEnd.FishingTrawler_LostFishingCharm";
        internal const string SEABORNE_TACKLE_KEY = "PeacefulEnd.FishingTrawler_SeaborneTackle";

        internal const string HAS_FARMER_GOTTEN_ANGLER_RING = "PeacefulEnd.FishingTrawler_AnglerRing.HasReceived";
        internal const string HAS_FARMER_GOTTEN_LOST_FISHING_CHARM = "PeacefulEnd.FishingTrawler_LostFishingCharm.HasReceived";
        internal const string HAS_FARMER_GOTTEN_SEABORNE_TACKLE_GUIDING_NARWHAL = "PeacefulEnd.FishingTrawler_SeaborneTackle.GuidingNarwhal.HasReceived";
        internal const string HAS_FARMER_GOTTEN_SEABORNE_TACKLE_RUSTY_CAGE = "PeacefulEnd.FishingTrawler_SeaborneTackle.RustyCage.HasReceived";
        internal const string HAS_FARMER_GOTTEN_SEABORNE_TACKLE_ALLURING_JELLYFISH = "PeacefulEnd.FishingTrawler_SeaborneTackle.AlluringJellyfish.HasReceived";
        internal const string HAS_FARMER_GOTTEN_SEABORNE_TACKLE_WEIGHTED_TREASURE = "PeacefulEnd.FishingTrawler_SeaborneTackle.WeightedTreasure.HasReceived";
        internal const string HAS_FARMER_GOTTEN_SEABORNE_TACKLE_STARRY_BOBBER = "PeacefulEnd.FishingTrawler_SeaborneTackle.StarryBobber.HasReceived";
        internal const string HAS_FARMER_GOTTEN_TRIDENT_TOOL = "PeacefulEnd.FishingTrawler_Trident.HasReceived";

        // Resource related
        internal const string COAL_CLUMP_KEY = "PeacefulEnd.FishingTrawler_CoalClump";

        // Mail related
        internal const string MAIL_FLAG_MURPHY_WAS_INTRODUCED = "PeacefulEnd.FishingTrawler_WillyIntroducesMurphy";
        internal const string MAIL_FLAG_MURPHY_FOUND_GINGER_ISLAND = "PeacefulEnd.FishingTrawler_MurphyGingerIsland";

        // Location related
        internal const string TRAWLER_SURFACE_LOCATION_NAME = "Custom_FishingTrawler";
        internal const string TRAWLER_HULL_LOCATION_NAME = "Custom_TrawlerHull";
        internal const string TRAWLER_CABIN_LOCATION_NAME = "Custom_TrawlerCabin";
    }
}
