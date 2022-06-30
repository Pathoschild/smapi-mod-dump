/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalEffects
{

    public class Player
    {
        public static int GetFriendshipPoints(string NPC)
        {
            var f2 = Game1.player;
            return f2.friendshipData.ContainsKey(NPC) ? f2.friendshipData[NPC].Points : 0;
        }

        public static void SetFriendshipPoints(string NPC, int points)
        {
            var f2 = Game1.player;
            if (f2.friendshipData.ContainsKey(NPC))
                f2.friendshipData[NPC].Points = points;
            else
                f2.friendshipData[NPC] = new Friendship(points);
        }

        //scaled, minimum possible to maximum possible, 0f to 1f.
        public static double GetLuckFactorFloat()
        {
            return Game1.random.NextDouble() + Game1.player.DailyLuck;
        }
    }
}

