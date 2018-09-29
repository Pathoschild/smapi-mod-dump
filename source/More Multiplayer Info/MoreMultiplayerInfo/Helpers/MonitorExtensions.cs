using System;
using StardewModdingAPI;
using StardewValley;

namespace MoreMultiplayerInfo.Helpers
{
    public static class MonitorExtensions
    {
        public static void BroadcastInfoMessage(this IModHelper helper, string message)
        {
            if (Context.IsMainPlayer)
            {
                helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().globalChatInfoMessage("ChatMessageFormat", new String[] { "PlayerReady", message });
            }

        }

        public static void SelfInfoMessage(this IModHelper helper, string message)
        {
            Game1.chatBox.addInfoMessage(message);
        }

        public static void BroadcastIfHost(this IModHelper helper, string message)
        {
            if (Game1.player.IsMainPlayer)
            {
                helper.BroadcastInfoMessage(message);
            }
            else
            {
                helper.SelfInfoMessage(message);
            }

        }
    }
}