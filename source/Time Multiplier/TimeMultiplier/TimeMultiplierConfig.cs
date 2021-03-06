/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DefenTheNation/StardewMod-TimeMultiplier
**
*************************************************/

namespace TimeMultiplier
{
    public class TimeMultiplierConfig
    {
        public bool Enabled { get; set; }
        public float TimeMultiplier { get; set; }

        public TimeMultiplierConfig()
        {
            Enabled = false;
            TimeMultiplier = 1.00f;
        }
    }
}
