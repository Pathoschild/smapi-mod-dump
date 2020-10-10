/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessBoard
{
    public class LeaderBoardEntry
    {
        public string Name { get; set; } = "";
        public int Games { get; set; } = 1;
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;

        public LeaderBoardEntry(string name, bool won)
        {
            Name = name;
            Wins = won ? 1 : 0;
            Losses = won ? 0 : 1;
        }
    }
}
