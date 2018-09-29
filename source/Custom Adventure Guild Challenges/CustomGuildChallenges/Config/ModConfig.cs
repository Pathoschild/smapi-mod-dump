using System.Collections.Generic;

namespace CustomGuildChallenges
{
    // Global mod config
    public class ModConfig
    {
        public bool CustomChallengesEnabled { get; set; }
        public bool CountKillsOnFarm { get; set; }
        public bool DebugMonsterKills { get; set; }
        public string GilNoRewardDialogue { get; set; }
        public string GilSleepingDialogue { get; set; }
        public string GilSpecialGiftDialogue { get; set; }
        public List<ChallengeInfo> Challenges { get; set; }

        public ModConfig()
        {
            Challenges = new List<ChallengeInfo>();
        }
    }
}
