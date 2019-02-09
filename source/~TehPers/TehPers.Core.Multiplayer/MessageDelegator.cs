using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Harmony;
using StardewValley.Network;
using TehPers.Core.Helpers.Static;
using Game1 = StardewValley.Game1;

namespace TehPers.Core.Multiplayer {
    internal static class MessageDelegator {
        // MTN uses packets 30, 31, and 50, PyTK uses 99, and spacecore uses 234 (list from https://github.com/spacechase0/SpaceCore_SDV/blob/master/Overrides/Multiplayer.cs)
        // Deep Woods uses 220
        public const byte MESSAGE_TYPE = 240;

        private static readonly HarmonyInstance _harmony = HarmonyInstance.Create("TehPers.Core.Multiplayer");
        private static bool _patched = false;

        private static readonly Dictionary<string, MessageHandler> _channels = new Dictionary<string, MessageHandler>();

        public static void PatchIfNeeded() {
            if (MessageDelegator._patched) {
                return;
            }

            MessageDelegator._patched = true;

            // Add a prefix to catch all messages being sent between players
            MethodInfo target = typeof(StardewValley.Multiplayer).GetMethod(nameof(StardewValley.Multiplayer.processIncomingMessage), BindingFlags.Public | BindingFlags.Instance);
            MethodInfo prefix = typeof(MessageDelegator).GetMethod(nameof(MessageDelegator.ProcessIncomingMessagePrefix), BindingFlags.NonPublic | BindingFlags.Static);
            MessageDelegator._harmony.Patch(target, new HarmonyMethod(prefix));
        }

        // ReSharper disable once InconsistentNaming
        private static bool ProcessIncomingMessagePrefix(StardewValley.Multiplayer __instance, IncomingMessage msg) {
            // Make sure it's a message being sent by this API
            if (msg.MessageType != MessageDelegator.MESSAGE_TYPE) {
                return true;
            }

            // Call the handler for this message
            msg.Reader.BaseStream.Position = 0;
            string channel = msg.Reader.ReadString();
            if (MessageDelegator._channels.TryGetValue(channel, out MessageHandler handler)) {
                handler.OnMessageRecevied(msg.SourceFarmer, msg.Reader);
            }

            // Make sure all players get the message
            if (Game1.IsServer) {
                foreach (long peerId in Game1.otherFarmers.Keys) {
                    if (peerId != msg.FarmerID) {
                        Game1.server.sendMessage(peerId, MessageDelegator.MESSAGE_TYPE, msg.SourceFarmer, msg.Data);
                    }
                }
            }

            return false;
        }

        public static void SendMessage(string channel, MessageWriter messageWriter) {
            // Write the message to a byte array
            byte[] buffer;
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                writer.Write(channel);
                messageWriter(writer);
                buffer = stream.ToArray();
            }

            // Send the message
            if (Game1.IsServer) {
                // Send it to all connect clients
                foreach (long peerId in Game1.otherFarmers.Keys) {
                    Game1.server.sendMessage(peerId, MessageDelegator.MESSAGE_TYPE, Game1.player, buffer);
                }
            } else {
                // Send it to the server
                Game1.client?.sendMessage(MessageDelegator.MESSAGE_TYPE, buffer);
            }
        }

        public static bool RegisterMessageHandler(MessageHandler handler) {
            return MessageDelegator._channels.GetOrAdd(handler.Channel, () => handler) == handler;
        }

        public static bool UnregisterMessageHandler(MessageHandler handler) {
            return MessageDelegator._channels.Remove(handler.Channel);
        }
    }
}