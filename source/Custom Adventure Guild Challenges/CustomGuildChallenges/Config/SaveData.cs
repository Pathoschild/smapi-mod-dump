/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/defenthenation/StardewMod-CustomSlayerChallenges
**
*************************************************/

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
