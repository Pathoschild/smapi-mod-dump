namespace CustomGuildChallenges
{
    // Data for the mod - Info is loaded from config and CollectedReward is loaded from save config
    public class SlayerChallenge
    {
        public ChallengeInfo Info { get; set; }
        public bool CollectedReward { get; set; }

        public SlayerChallenge()
        {
        }
    }   
}
