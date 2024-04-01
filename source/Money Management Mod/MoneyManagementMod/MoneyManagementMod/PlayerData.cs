/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tbonetomtom/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManagementMod
{
    public class PlayerData
    {
        public int PreviousMoney { get; set; }
        public int CurrentMoney { get; set; }
        public int TransferAmount { get; set; }
        public bool CanTax { get; set; }
        public bool DrawHUD { get; set; }
        public PlayerData()
        {
            PreviousMoney = -101;
            CurrentMoney = 0;
            TransferAmount = 10;
            CanTax = false;
            DrawHUD = true;
        }
    }
}
