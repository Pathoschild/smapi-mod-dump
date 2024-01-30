/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-ModCollection
**
*************************************************/

using Netcode;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyLeprechaunBoots
{
    public static class BootsUtil
    {
        /// <summary>
        /// Checks to see if the Farmer is wearing the Leprechaun boots
        /// </summary>
        /// <returns></returns>
        public static bool IsWearingLeprechaunBoots()
        {
            NetRef<Boots> currentBootsRef = Game1.player.boots;

            // If the Farmer has no boots on, or they aren't the Leprechaun Boots their luck isn't increased
            if (currentBootsRef == null || currentBootsRef.Value == null || currentBootsRef.Value.indexInTileSheet.Value != 806)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
