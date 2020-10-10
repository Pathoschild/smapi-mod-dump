/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/defenthenation/StardewMod-CustomSlayerChallenges
**
*************************************************/

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
