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

namespace ChessBoard
{
    public class SaveData
    {
        public Dictionary<string, Session> Sessions = new Dictionary<string, Session>();
        public List<GameResult> LastSessions = new List<GameResult>();
        public Dictionary<string, LeaderBoardEntry> LeaderBoard = new Dictionary<string, LeaderBoardEntry>();
    }
}
