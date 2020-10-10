/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/AnotherHungerMod
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherHungerMod
{
    internal class SaveData
    {
        public double Fullness { get; set; } = 100;
        public bool FedSpouseMeal = false;

        public void SyncToHost()
        {
            if (Context.IsMainPlayer)
                Mod.instance.Helper.Data.WriteSaveData($"spacechase0.AnotherHungerMod.{Game1.player.UniqueMultiplayerID}", Mod.Data);
            else
            {
                Mod.instance.Helper.Multiplayer.SendMessage(Mod.Data, Mod.MSG_HUNGERDATA, null, new long[] { Game1.MasterPlayer.UniqueMultiplayerID } );
            }
        }
    }
}
