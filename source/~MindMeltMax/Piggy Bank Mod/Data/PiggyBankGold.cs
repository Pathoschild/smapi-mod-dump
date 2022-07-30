/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piggy_Bank_Mod.Data
{
    public class PiggyBankGold
    {
        public string Label { get; set; }
        public int Id { get; set; }
        public float StoredGold { get; set; } = 0;
        public Vector2 BankTile { get; set; }
        public string BankLocationName { get; set; }
        public long OwnerID { get; set; }

        public PiggyBankGold()
        {
        }

        public PiggyBankGold(string label, float gold, Vector2 bankTile, int id, long playerId, string locName)
        {
            Id = id;
            Label = label;
            StoredGold = gold;
            BankTile = bankTile;
            OwnerID = playerId;
            BankLocationName = locName;
        }
    }

    public class allGold
    {
        public List<PiggyBankGold> goldList { get; set; }

        public allGold() { }
        public allGold(List<PiggyBankGold> list) { goldList = list; }
    }
}
