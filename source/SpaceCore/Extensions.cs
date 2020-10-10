/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/SpaceCore_SDV
**
*************************************************/

using StardewValley;

namespace SpaceCore
{
    public static class Extensions
    {
        public static int countStardropsEaten(this Farmer player)
        {
            int count = 0;
            if (Game1.player.hasOrWillReceiveMail("CF_Fair"))
                ++count;
            if (Game1.player.hasOrWillReceiveMail("CF_Fish"))
                ++count;
            if (Game1.player.hasOrWillReceiveMail("CF_Mines"))
                ++count;
            if (Game1.player.hasOrWillReceiveMail("CF_Sewer"))
                ++count;
            if (Game1.player.hasOrWillReceiveMail("museumComplete"))
                ++count;
            if (Game1.player.hasOrWillReceiveMail("CF_Spouse"))
                ++count;
            if (Game1.player.hasOrWillReceiveMail("CF_Statue"))
                ++count;
            return count;
        }
    }
}
