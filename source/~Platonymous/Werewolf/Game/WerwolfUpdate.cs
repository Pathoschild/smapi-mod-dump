/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace Werewolf.Game
{
    public class WerwolfUpdate : WerwolfMPMessage
    {
        public override string Type { get; set; } = "Update";
        public List<WerwolfClientPlayer> Players {get; set;} = new List<WerwolfClientPlayer>();
        public List<WerwolfClientPlayer> Winners { get; set; } = new List<WerwolfClientPlayer>();

        public string WinMessage { get; set; }

        public int Round { get; set; } = -1;

        public bool WolvesWon { get; set; }

        public WerwolfUpdate()
        {

        }

        public WerwolfUpdate(int round, long sendTo, long sendFrom, WerwolfGame game, List<WerwolfClientPlayer> players, List<WerwolfClientPlayer> winners, string winMessage, bool wolveswon) : base(sendTo, sendFrom, game)
        {
            WinMessage = winMessage;
            Players = players;
            Winners = winners ?? new List<WerwolfClientPlayer>();
            WinMessage = winMessage;
            WolvesWon = wolveswon;
            Round = round;
        }
    }
}
