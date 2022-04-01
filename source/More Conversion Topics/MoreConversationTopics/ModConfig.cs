/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/MoreConversationTopics
**
*************************************************/

using System;
namespace MoreConversationTopics
{
    public class ModConfig
    {
        public int WeddingDuration { get; set; }
        public int BirthDuration { get; set; }
        public int DivorceDuration { get; set; }
        public int LuauDuration { get; set; }
        public int JojaGreenhouseDuration { get; set; }
        public int JojaCompletionDuration { get; set; }
        public int JojaLightningDuration { get; set; }
        public int WillyBoatRepairDuration { get; set; }
        public int LeoArrivalDuration { get; set; }
        public int UFOLandedDuration { get; set; }
        public int MeteoriteLandedDuration { get; set; }
        public int OwlStatueDuration { get; set; }
        public int RailroadEarthquakeDuration { get; set; }
        public int WitchVisitDuration { get; set; }
        public int FairyVisitDuration { get; set; }
        public int IslandResortDuration { get; set; }

        public ModConfig()
        {
            // Durations are set to 7 days by default, except joja_Greenhouse and joja_Complete match vanilla
            this.WeddingDuration = 7;
            this.BirthDuration = 7;
            this.DivorceDuration = 7;
            this.LuauDuration = 7;
            this.JojaGreenhouseDuration = 3;
            this.JojaCompletionDuration = 4;
            this.JojaLightningDuration = 7;
            this.WillyBoatRepairDuration = 7;
            this.LeoArrivalDuration = 7;
            this.UFOLandedDuration = 7;
            this.MeteoriteLandedDuration = 7;
            this.OwlStatueDuration = 7;
            this.RailroadEarthquakeDuration = 7;
            this.WitchVisitDuration = 7;
            this.FairyVisitDuration = 7;
            this.IslandResortDuration = 7;
        }
    }
}
