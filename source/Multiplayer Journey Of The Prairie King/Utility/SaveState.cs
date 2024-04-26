/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/scayze/multiprairie
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MultiplayerPrairieKing.Utility.Serialization;

namespace MultiplayerPrairieKing.Utility
{

    public class PlayerSaveState
    {
        public long PlayerID { get; set; }

        public string PlayerName { get; set; }

        public int BulletDamage { get; set; }

        public int FireSpeedLevel { get; set; }

        public int AmmoLevel { get; set; }

        public bool SpreadPistol { get; set; }

        public int RunSpeedLevel { get; set; }

        public int HeldItem { get; set; }
    }

    public class SaveState
    {
        public List<PlayerSaveState> playerSaveStates { get; set; }

        public int Lives { get; set; }

        public int Coins { get; set; }

        public int Score { get; set; }

        public bool Died { get; set; }

        public int WhichRound { get; set; }

        public int WhichWave { get; set; }

        public int World { get; set; }

        public int WaveTimer { get; set; }

        public List<SVector2> MonsterChances { get; set; }
    }
}
