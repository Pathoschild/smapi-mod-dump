/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/MoreConversationTopics
**
*************************************************/

namespace MoreConversationTopics
{
    public class ModConfig
    {
        // Durations are set to 7 days by default, except joja_Greenhouse and joja_Complete match vanilla
        public int EngagementDuration { get; set; } = 3;
        public int WeddingDuration { get; set; } = 7;
        public int BirthDuration { get; set; } = 7;
        public int DivorceDuration { get; set; } = 7;
        public int LuauDuration { get; set; } = 7;
        public int JojaGreenhouseDuration { get; set; } = 3;
        public int JojaCompletionDuration { get; set; } = 3;
        public int JojaLightningDuration { get; set; } = 7;
        public int WillyBoatRepairDuration { get; set; } = 7;
        public int LeoArrivalDuration { get; set; } = 7;
        public int UFOLandedDuration { get; set; } = 7;
        public int MeteoriteLandedDuration { get; set; } = 7;
        public int OwlStatueDuration { get; set; } = 7;
        public int RailroadEarthquakeDuration { get; set; } = 7;
        public int WitchVisitDuration { get; set; } = 7;
        public int FairyVisitDuration { get; set; } = 7;
        public int IslandResortDuration { get; set; } = 7;
        public int SelfishShrineDuration { get; set; } = 7;
    }
}
