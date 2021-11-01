/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley;
using StardewValley.Network;
using System.Collections.Generic;

namespace BattleRoyale.Utils
{
    internal class NetworkUtils
    {
        public const byte uniqueMessageType = 39; //Some random number that the game doesn't use (Check Multiplayer.processIncomingMessage)

        public enum MessageTypes
        {
            KICK_PLAYER,
            SEND_MY_VERSION_TO_SERVER,
            TAKE_DAMAGE,
            SERVER_BROADCAST_ROUND_START,
            SERVER_BROADCAST_ROUND_END,
            ANNOUNCE_CLIENT_DEATH,
            SEND_DEATH_ANIMATION,
            TELL_PLAYER_HIT_SHAKE_TIMER, // invincibility frames
            SERVER_BROADCAST_CHAT_MESSAGE,
            BROADCAST_ALIVE_COUNT,
            WARP,
            SEND_STORM_PHASE_DATA,
            SEND_STORM_LOCATION_DATA,
            SYNCHRONIZE_TIME,
            TOGGLE_SPECTATE,
            ON_JOIN,
            RETURN_TO_LOBBY,
            PERFORM_EMOTE,
            LEADERBOARD_DATA_SYNC,
            BROADCAST_TEAM
        }

        public static void SendDamageToPlayer(Farmer who, DamageSource source, int damage, long? damagerID = null, string monster = "")
        {
            List<object> data = new List<object>()
            {
                source, damage
            };
            if (damagerID.HasValue)
                data.Add(damagerID.Value);
            if (monster.Length != 0)
                data.Add(monster);

            NetworkMessage.Send(
                MessageTypes.TAKE_DAMAGE,
                NetworkMessageDestination.SPECIFIC_PEER,
                data,
                who.UniqueMultiplayerID
            );
        }

        public static void BroadcastRoundStart(int numberOfPlayers, int stormIndex, SpecialRoundType? specialRound)
        {
            if (!Game1.IsServer)
                return;

            List<object> data = new List<object>()
            {
                numberOfPlayers, stormIndex
            };
            if (specialRound != null)
                data.Add(specialRound);

            NetworkMessage.Send(
                MessageTypes.SERVER_BROADCAST_ROUND_START,
                NetworkMessageDestination.ALL,
                data
            );
        }

        public static void BroadcastRoundEnd(long? winnerId)
        {
            List<object> data = new List<object>();
            if (winnerId != null)
                data.Add(winnerId);

            NetworkMessage.Send(
                MessageTypes.SERVER_BROADCAST_ROUND_END,
                NetworkMessageDestination.ALL,
                data
            );
        }

        public static void SendChatMessageToAllPlayers(string message)
        {
            NetworkMessage.Send(
                MessageTypes.SERVER_BROADCAST_CHAT_MESSAGE,
                NetworkMessageDestination.ALL,
                new List<object>() { message }
            );
        }

        public static void SendChatMessageToPlayerWithoutMod(long playerID, string message)
        {
            Game1.server?.sendMessage(playerID, new OutgoingMessage(10, Game1.player, LocalizedContentManager.LanguageCode.en, message));
        }

        public static void WarpFarmer(Farmer target, TileLocation targetLocation)
        {
            NetworkMessage.Send(
                MessageTypes.WARP,
                NetworkMessageDestination.SPECIFIC_PEER,
                new List<object>() { targetLocation.locationName, targetLocation.tileX, targetLocation.tileY },
                target.UniqueMultiplayerID
            );
        }

        public static void KickPlayer(Farmer target, string reason)
        {
            NetworkMessage.Send(
                MessageTypes.KICK_PLAYER,
                NetworkMessageDestination.SPECIFIC_PEER,
                new List<object>() { reason },
                target.UniqueMultiplayerID
            );
        }
        public static void SynchronizeTimeData()
        {
            NetworkMessage.Send(
                MessageTypes.SYNCHRONIZE_TIME,
                NetworkMessageDestination.ALL_OTHERS,
                new List<object>() { Game1.currentSeason, Game1.timeOfDay }
            );
        }

        public static void AnnounceClientDeath(DamageSource source, string monster, long? killerId)
        {
            List<object> data = new List<object>() { (int)source, monster };
            if (killerId.HasValue)
                data.Add(killerId);

            NetworkMessage.Send(
                MessageTypes.ANNOUNCE_CLIENT_DEATH,
                NetworkMessageDestination.HOST,
                data
            );
        }
    }
}
