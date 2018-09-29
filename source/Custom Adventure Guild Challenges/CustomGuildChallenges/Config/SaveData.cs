using System.Collections.Generic;

namespace CustomGuildChallenges
{
    // Save data json file - different for each character
    public class SaveData
    {
        public List<ChallengeSave> Challenges { get; set; }

        public SaveData()
        {
            Challenges = new List<ChallengeSave>();
        }
    }
}
