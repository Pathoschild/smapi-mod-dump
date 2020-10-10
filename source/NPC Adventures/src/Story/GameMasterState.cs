/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Story
{
    public class GameMasterState
    {
        public Dictionary<long, PlayerState> EligiblePlayers { get; set; } = new Dictionary<long, PlayerState>();

        public PlayerState GetPlayerState(Farmer player)
        {
            if (!this.EligiblePlayers.ContainsKey(player.UniqueMultiplayerID)) {
                this.EligiblePlayers.Add(player.UniqueMultiplayerID, new PlayerState());
            }

            return this.EligiblePlayers[player.UniqueMultiplayerID];
        }

        public PlayerState GetPlayerState()
        {
            return this.GetPlayerState(Game1.player);
        }

        public class PlayerState
        {
            public bool isEligible = false;
            public HashSet<string> recruited = new HashSet<string>();
            public HashSet<int> completedQuests = new HashSet<int>(); 
        }
    }
}
