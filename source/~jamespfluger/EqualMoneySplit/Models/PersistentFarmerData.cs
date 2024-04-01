/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-EqualMoneySplit
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace EqualMoneySplit.Models
{
    /// <summary>
    /// Contains information on the local Farmer's past member values
    /// </summary>
    public class PersistentFarmerData
    {
        /// <summary>
        /// Money the local Farmer had in the previos game tick
        /// </summary>
        public int PocketMoney { get; set; }

        /// <summary>
        /// Money the local Farmer had when the day began ending
        /// </summary>
        public int PersonalShippingBinsMoney { get; set; }

        /// <summary>
        /// Money the local Farmer will send to other connected Farmer's
        /// </summary>
        public int ShareToSend { get; set; }

        public void DumpData()
        {
            EqualMoneyMod.Logger.Log($"{Game1.player.displayName} [{Game1.player.UniqueMultiplayerID}]: pcket:{PocketMoney} w/ shipping:{PersonalShippingBinsMoney} and share:{ShareToSend}", StardewModdingAPI.LogLevel.Alert);
        }
    }
}
