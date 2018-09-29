
using StardewValley;
using StardewValley.Network;
using Microsoft.Xna.Framework.Graphics;
using System;
using CustomEmojis.Framework.Events;
using CustomEmojis.Framework.Constants;
using CustomEmojis.Framework.Network;
using System.Collections.Generic;
using System.Linq;
using Netcode;
using System.IO;

namespace CustomEmojis.Framework.Extensions {

    public static class MultiplayerExtension {

        public static event EventHandler<ReceivedEmojiTextureEventArgs> OnReceiveEmojiTexture = delegate { };
        public static event EventHandler<ReceivedEmojiTextureRequestEventArgs> OnReceiveEmojiTextureRequest = delegate { };
        public static event EventHandler<ReceivedEmojiTextureDataEventArgs> OnReceiveEmojiTextureData = delegate { };

        public static event EventHandler<PlayerConnectedEventArgs> OnPlayerConnected = delegate { };
        public static event EventHandler<PlayerDisconnectedEventArgs> OnPlayerDisconnected = delegate { };

        public static event EventHandler<ChatMessageEventArgs> OnReceiveChatMessage = delegate { };
        public static event EventHandler<ChatMessageEventArgs> OnSendChatMessage = delegate { };

        public static void BroadcastEmojiTexture(this Multiplayer multiplayer, Texture2D texture, int numberEmojis) {

            if(Game1.IsMultiplayer) {

                object[] objArray = new object[3] {
                    Message.Action.BroadcastEmojiTexture.ToString(),
                    numberEmojis,
                    DataSerialization.Serialize(new TextureData(texture))
                };
                OutgoingMessage message = new OutgoingMessage(Message.TypeID, Game1.player, objArray);

                if(Game1.IsClient) {
                    Game1.client.sendMessage(message);
                } else if(Game1.IsClient) {
                    foreach(Farmer farmer in Game1.getAllFarmers()) {
                        if(farmer != Game1.player) {
                            Game1.server.sendMessage(farmer.UniqueMultiplayerID, message);
                        }
                    }
                }

            }

        }

        public static void ReceiveEmojiTextureBroadcast(this Multiplayer multiplayer, IncomingMessage msg) {
            if(Game1.IsMultiplayer && msg.Data.Length > 0) {
                ReceivedEmojiTextureEventArgs args = new ReceivedEmojiTextureEventArgs {
                    SourceFarmer = msg.SourceFarmer,
                    NumberEmojis = msg.Reader.ReadInt32(),
                    EmojiTexture = DataSerialization.Deserialize<TextureData>(msg.Reader.BaseStream).GetTexture()
                };
                OnReceiveEmojiTexture(null, args);
            }
        }

        public static void RequestEmojiTexture(this Multiplayer multiplayer) {
            if(Game1.IsMultiplayer) {
                foreach(Farmer farmer in Game1.getAllFarmers()) {
                    if(farmer.IsMainPlayer) {
                        multiplayer.RequestEmojiTexture(farmer);
                    }
                }
            }
        }

        public static void RequestEmojiTexture(this Multiplayer multiplayer, Farmer farmer) {
            multiplayer.RequestEmojiTexture(farmer.UniqueMultiplayerID);
        }

        public static void RequestEmojiTexture(this Multiplayer multiplayer, long peerId) {
            if(Game1.IsMultiplayer) {
                object[] objArray = new object[1] {
                    Message.Action.RequestEmojiTexture.ToString()
                };
                OutgoingMessage message = new OutgoingMessage(Message.TypeID, Game1.player, objArray);
                if(Game1.IsClient) {
                    Game1.client.sendMessage(message);
                } else if(Game1.IsServer) {
                    Game1.server.sendMessage(peerId, message);
                }
            }
        }

        public static void ReceiveEmojiTextureRequest(this Multiplayer multiplayer, IncomingMessage msg) {
            if(Game1.IsMultiplayer && msg.Data.Length > 0) {
                ReceivedEmojiTextureRequestEventArgs args = new ReceivedEmojiTextureRequestEventArgs {
                    SourceFarmer = msg.SourceFarmer
                };
                OnReceiveEmojiTextureRequest(null, args);
            }
        }

        #region TextureData List

        public static void SendEmojisTextureDataList(this Multiplayer multiplayer, Farmer farmer, IEnumerable<List<TextureData>> textureDataListEnumerable) {
            multiplayer.SendEmojisTextureDataList(farmer.UniqueMultiplayerID, textureDataListEnumerable.SelectMany(x => x).ToList());
        }

        public static void SendEmojisTextureDataList(this Multiplayer multiplayer, Farmer farmer, List<TextureData> textureDataList) {
            multiplayer.SendEmojisTextureDataList(farmer.UniqueMultiplayerID, textureDataList);
        }

        public static void SendEmojisTextureDataList(this Multiplayer multiplayer, long peerId, List<TextureData> textureDataList) {
            if(Game1.IsMultiplayer) {
                object[] objArray = new object[3] {
                    Message.Action.SendEmojisTextureDataList.ToString(),
                    peerId,
                    DataSerialization.Serialize(textureDataList)
                };

                OutgoingMessage message = new OutgoingMessage(Message.TypeID, Game1.player, objArray);
                if(Game1.IsClient) {
                    Game1.client.sendMessage(message);
                } else if(Game1.IsServer) {
                    Game1.server.sendMessage(peerId, message);
                }
            }
        }

        public static void ReceiveEmojisTextureDataList(this Multiplayer multiplayer, IncomingMessage msg) {

            if(Game1.IsMultiplayer && msg.Data.Length > 0) {

                long uniqueMultiplayerID = msg.Reader.ReadInt64();

                ReceivedEmojiTextureDataEventArgs args = new ReceivedEmojiTextureDataEventArgs {
                    SourceFarmer = msg.SourceFarmer,
                    TextureDataList = DataSerialization.Deserialize<List<TextureData>>(msg.Reader.BaseStream)
                };

                if(Game1.player.UniqueMultiplayerID == uniqueMultiplayerID) {
                    OnReceiveEmojiTextureData(null, args);
                } else if(Game1.IsServer) {
                    multiplayer.SendEmojisTextureDataList(uniqueMultiplayerID, args.TextureDataList);
                }

            }

        }

        #endregion

        public static void ResponseEmojiTexture(this Multiplayer multiplayer, Farmer farmer, Texture2D texture, int numberEmojis) {
            multiplayer.SendEmojiTexture(farmer.UniqueMultiplayerID, texture, numberEmojis);
        }

        public static void SendEmojiTexture(this Multiplayer multiplayer, long peerId, Texture2D texture, int numberEmojis) {
            if(Game1.IsMultiplayer) {
                object[] objArray = new object[3] {
                    Message.Action.SendEmojiTexture.ToString(),
                    numberEmojis,
                    DataSerialization.Serialize(new TextureData(texture))
                };

                OutgoingMessage message = new OutgoingMessage(Message.TypeID, Game1.player, objArray);
                if(Game1.IsClient) {
                    Game1.client.sendMessage(message);
                } else if(Game1.IsServer) {
                    Game1.server.sendMessage(peerId, message);
                }
            }
        }

        public static void ReceiveEmojiTexture(this Multiplayer multiplayer, IncomingMessage msg) {
            if(Game1.IsMultiplayer && msg.Data.Length > 0) {
                ReceivedEmojiTextureEventArgs args = new ReceivedEmojiTextureEventArgs {
                    SourceFarmer = msg.SourceFarmer,
                    NumberEmojis = msg.Reader.ReadInt32(),
                    EmojiTexture = DataSerialization.Deserialize<TextureData>(msg.Reader.BaseStream).GetTexture()
                };
                OnReceiveEmojiTexture(null, args);
            }
        }

        #region Vanilla MessageTypes

        //TODO: Not in use, remove if not needed
        public static void PlayerConnected(this Multiplayer multiplayer, IncomingMessage msg) {
            PlayerConnectedEventArgs args = new PlayerConnectedEventArgs {
                Player = multiplayer.readFarmer(msg.Reader).Value
            };
            ModEntry.ModMonitor.Log("PlayerConnected(this Multiplayer multiplayer, IncomingMessage msg)");
            OnPlayerConnected(null, args);
        }

        public static void PlayerConnected(this Multiplayer multiplayer, Farmer farmer) {
            PlayerConnectedEventArgs args = new PlayerConnectedEventArgs {
                Player = farmer
            };
            OnPlayerConnected(null, args);
        }

        //TODO: Not in use, remove if not needed
        public static void PlayerDisconnected(this Multiplayer multiplayer, IncomingMessage msg) {
            PlayerDisconnectedEventArgs args = new PlayerDisconnectedEventArgs {
                Player = msg.SourceFarmer
            };
            ModEntry.ModMonitor.Log("PlayerDisconnected(this Multiplayer multiplayer, IncomingMessage msg)");
            OnPlayerDisconnected(null, args);
        }

        public static void PlayerDisconnected(this Multiplayer multiplayer, Farmer farmer) {
            PlayerDisconnectedEventArgs args = new PlayerDisconnectedEventArgs {
                Player = farmer
            };
            OnPlayerDisconnected(null, args);
        }

        #endregion

        #region Chat Messages

        public static void ReceivedChatMessage(this Multiplayer multiplayer, Farmer farmer, LocalizedContentManager.LanguageCode language, string message) {
            ChatMessageEventArgs args = new ChatMessageEventArgs {
                SourcePlayer = farmer,
                Language = language,
                Message = message
            };
#if DEBUG
            ModEntry.ModLogger.LogTrace();
#endif
            OnReceiveChatMessage(null, args);
        }

        public static void SendedChatMessage(this Multiplayer multiplayer, Farmer farmer, LocalizedContentManager.LanguageCode language, string message) {
            ChatMessageEventArgs args = new ChatMessageEventArgs {
                SourcePlayer = farmer,
                Language = language,
                Message = message
            };
#if DEBUG
            ModEntry.ModLogger.LogTrace();
#endif
            OnSendChatMessage(null, args);
        }

        #endregion

    }

}
