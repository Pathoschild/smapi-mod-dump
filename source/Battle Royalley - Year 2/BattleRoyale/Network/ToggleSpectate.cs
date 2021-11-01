/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using BattleRoyale.Utils;
using StardewValley;
using System;
using System.Collections.Generic;

namespace BattleRoyale.Network
{
    class ToggleSpectate : NetworkMessage
    {
        public ToggleSpectate()
        {
            MessageType = NetworkUtils.MessageTypes.TOGGLE_SPECTATE;
        }

        public override void Receive(Farmer source, List<object> data)
        {
            Round round = ModEntry.BRGame.GetActiveRound();

            long playerId = Convert.ToInt64(data[0]);
            bool status = Convert.ToBoolean(data[1]);

            Farmer player = Game1.getFarmer(playerId);

            if (playerId == Game1.player.UniqueMultiplayerID)
                ModEntry.BRGame.isSpectating = status;

            if (round != null && round.AlivePlayers.Contains(player) && Game1.player.UniqueMultiplayerID == playerId)
                FarmerUtils.TakeDamage(Game1.player, DamageSource.WORLD, 1000);

            if (status)
                ModEntry.BRGame.spectatingPlayers.Add(playerId);
            else
            {
                ModEntry.BRGame.spectatingPlayers.Remove(playerId);
                if (round != null && round.AlivePlayers.Count <= 1 && !ModEntry.BRGame.waitingForNextRoundToStart && Game1.IsServer)
                    round.HandleWin(null, null);
            }
        }
    }
}
