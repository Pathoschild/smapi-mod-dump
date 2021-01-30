/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/ManaBar
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using static ManaBar.Mod;
using System.IO;
using SpaceShared;

namespace ManaBar
{
    public static class Extensions
    {
        private static void dataCheck( Farmer player )
        {
            if (!Data.players.ContainsKey(player.UniqueMultiplayerID))
                Data.players.Add(player.UniqueMultiplayerID, new MultiplayerSaveData.PlayerData());
        }

        public static int getCurrentMana(this Farmer player)
        {
            dataCheck(player);
            return Data.players[ player.UniqueMultiplayerID ].mana;
        }

        public static void addMana(this Farmer player, int amt)
        {
            dataCheck(player);
            Data.players[player.UniqueMultiplayerID].mana = Math.Max(0, Math.Min(player.getCurrentMana() + amt, player.getMaxMana()));
            if (player == Game1.player)
                Data.syncMineMini();
        }

        public static int getMaxMana(this Farmer player)
        {
            dataCheck(player);
            return Data.players[player.UniqueMultiplayerID].manaCap;
        }

        public static void setMaxMana(this Farmer player, int newCap )
        {
            dataCheck(player);
            Data.players[player.UniqueMultiplayerID].manaCap = newCap;
            if (player == Game1.player)
                Data.syncMineMini();
        }
    }
}
