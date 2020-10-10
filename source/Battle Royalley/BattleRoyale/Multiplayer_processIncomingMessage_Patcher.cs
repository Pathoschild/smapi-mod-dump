/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using Netcode;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BattleRoyale
{
	class Multiplayer_processIncomingMessage_Patcher : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Multiplayer), "processIncomingMessage");

		public static bool Prefix(IncomingMessage msg, Multiplayer __instance)
		{
            if (Game1.IsServer && (msg == null || !Game1.otherFarmers.ContainsKey(msg.FarmerID)))
            {
                //They have been kicked off the server
                return false;
            }

            if (msg.Reader == null)
			{
				//Data broke
				Console.WriteLine("Received null data");
				return false;
			}

            if (msg != null && msg.Data != null && msg.SourceFarmer != null && msg.MessageType == NetworkUtility.uniqueMessageType)
			{
				return ProcessMessage(msg.Data, msg.SourceFarmer);
			}

			return true;
		}

		private static bool ProcessMessage(byte[] msgData, Farmer sourceFarmer)
		{
			var subMessageType = (NetworkUtility.MessageTypes)BitConverter.ToInt32(msgData, 0);
			Console.WriteLine($"Receiving special message, type = {subMessageType}. Message length = {msgData.Length}");

			switch (subMessageType)
			{
				case (NetworkUtility.MessageTypes.TAKE_DAMAGE):
					int damage = BitConverter.ToInt32(msgData, 4);

                    long? damagerID = null;
                    if (msgData.Length > 8)
                        damagerID = BitConverter.ToInt64(msgData, 8);

                    Console.WriteLine($"Taking {damage} damage from another player");

					ModEntry.BRGame.TakeDamage(damage, damagerID);
					return false;
				case (NetworkUtility.MessageTypes.SERVER_BROADCAST_GAME_START):
					if (!Game1.IsServer)
					{
						int numberOfPlayers = BitConverter.ToInt32(msgData, 4);
						int stormIndex = BitConverter.ToInt32(msgData, 8);
						bool isHostParticipating = BitConverter.ToBoolean(msgData, 12);
						
						ModEntry.BRGame.ClientStartGame(numberOfPlayers, isHostParticipating, stormIndex);
					}
					return false;
				case NetworkUtility.MessageTypes.WARP:
					int tileX = BitConverter.ToInt32(msgData, 4);
					int tileY = BitConverter.ToInt32(msgData, 8);

					StringBuilder sb = new StringBuilder();
					for (int i = 12; i < msgData.Length; i++)
						sb.Append((char)msgData[i]);

					string locationName = sb.ToString().Substring(1);

					Console.WriteLine($"I was told by the server to move to {locationName} @ ({tileX},{tileY})");

					//Face downwards & Warp
					Game1.player.FacingDirection = 2;
					Game1.player.warpFarmer(new TileLocation(locationName, tileX, tileY).CreateWarp());
					return false;
				case (NetworkUtility.MessageTypes.SERVER_BROADCAST_GAME_END):
					if (!Game1.IsServer)
					{
						ModEntry.BRGame.ClientEndGame();
					}
					return false;
				case (NetworkUtility.MessageTypes.RELAY_MESSAGE_TO_ANOTHER_PLAYER):
					if (Game1.IsServer)
					{
						long targetUniqueID = BitConverter.ToInt64(msgData, 4);
						
						byte[] messageData = new byte[msgData.Length - 12];
						Array.Copy(msgData, 12, messageData, 0, msgData.Length - 12);

						if (targetUniqueID == Game1.player.UniqueMultiplayerID)
						{
							Console.WriteLine("Received relay instruction addressed to myself");
							ProcessMessage(messageData, sourceFarmer);
							return false;
						}
						else
						{
							Console.WriteLine($"Relaying a message to {targetUniqueID}...");
							Game1.server.sendMessage(targetUniqueID, new OutgoingMessage(NetworkUtility.uniqueMessageType, Game1.player, (object)messageData));
							return false;
						}
					}
					else
					{
						Console.WriteLine("Accidentally received relay instruction as a client");
						return false;
					}
				case (NetworkUtility.MessageTypes.TELL_SERVER_I_DIED):
                    if (msgData.Length > 12)
                        ModEntry.BRGame.HandleDeath(sourceFarmer, BitConverter.ToInt64(msgData, 12));
                    else
                        ModEntry.BRGame.HandleDeath(sourceFarmer);
					return false;
				case NetworkUtility.MessageTypes.SERVER_BROADCAST_CHAT_MESSAGE:
                    string message = Encoding.UTF8.GetString(msgData.Skip(4).ToArray()).Substring(1);
                    string colorName = "white";
                    try
                    {
                        colorName = message.Substring(1, message.Substring(1).IndexOf(']'));
                        Console.WriteLine($"color = {colorName}");
                    }
                    catch (Exception) { }

                    var color = StardewValley.Menus.ChatMessage.getColorFromName(colorName);
                    if (color == Microsoft.Xna.Framework.Color.White)
                        color = Microsoft.Xna.Framework.Color.Gold;

                    Game1.chatBox.addMessage(message, color);
					return false;
				case (NetworkUtility.MessageTypes.TELL_PLAYER_HIT_SHAKE_TIMER):
					int howLongMilliseconds = BitConverter.ToInt32(msgData, 4);
					HitShaker.SetHitShakeTimer(sourceFarmer.UniqueMultiplayerID, howLongMilliseconds);
					return false;
				case NetworkUtility.MessageTypes.BROADCAST_ALIVE_COUNT:
					int howManyPlayersAlive = BitConverter.ToInt32(msgData, 4);

					if (ModEntry.BRGame.OverlayUI != null)
						ModEntry.BRGame.OverlayUI.AlivePlayers = howManyPlayersAlive;
					return false;
				case NetworkUtility.MessageTypes.SEND_PHASE_DATA:
					try
					{
						var b = new BinaryFormatter();

						var phases = (List<Phase>[])b.Deserialize(new MemoryStream(msgData.Skip(4).ToArray()));

						Console.WriteLine($"Received phases data from server, info: Length={phases.Length}");

						Storm.Phases = phases;
					}catch(Exception)
					{
						Console.WriteLine("Unable to process phases data. Kicking to prevent glitches...");
                        long id = sourceFarmer.UniqueMultiplayerID;
                        NetworkUtility.SendChatMessageToPlayerWithoutMod(id, "Unable to process phases data. Kicking to prevent glitches...");
                        Game1.server.sendMessage(id, new OutgoingMessage((byte)19, id, new object[0]));
                        Game1.server.playerDisconnected(id);
                        Game1.otherFarmers.Remove(id);
                    }
					return false;
				case NetworkUtility.MessageTypes.KICK_PLAYER:
					StringBuilder sb2 = new StringBuilder();
					for (int i = 4; i < msgData.Length; i++)
						sb2.Append((char)msgData[i]);

					string kickMsg = sb2.ToString().Substring(1);
					Console.WriteLine($"Kicked. Reason: \"{kickMsg}\"");

					Game1.client.disconnect();

					System.Threading.Tasks.Task.Factory.StartNew(() =>
					{
						System.Threading.Thread.Sleep(300);
						Game1.activeClickableMenu = new StardewValley.Menus.DialogueBox($"Kicked. Reason: \"{kickMsg}\"");
					});
					
					return false;
				case NetworkUtility.MessageTypes.SEND_MY_VERSION_TO_SERVER:
					if (Game1.IsServer)
					{
						int major = BitConverter.ToInt32(msgData, 4);
						int minor = BitConverter.ToInt32(msgData, 8);
                        byte[] sha = msgData.Skip(12).ToArray();

						Console.WriteLine($"Received version from client {sourceFarmer.Name}/{sourceFarmer.UniqueMultiplayerID}: {major}.{minor}");
						new AutoKicker().AcknowledgeClientVersion(sourceFarmer.UniqueMultiplayerID, major, minor, sha);

						if (ModEntry.BRGame.IsGameInProgress)
						{
							System.Threading.Tasks.Task.Factory.StartNew(() =>
							{
								System.Threading.Thread.Sleep(1000);
								Game1.server.sendMessage(sourceFarmer.UniqueMultiplayerID,
									new OutgoingMessage(NetworkUtility.uniqueMessageType, Game1.player, (int)NetworkUtility.MessageTypes.TELL_NEW_PLAYER_TO_SPECTATE));
							});
							
						}
					}
					return false;
				case NetworkUtility.MessageTypes.TELL_NEW_PLAYER_TO_SPECTATE:
					if (!Game1.IsServer)
					{
						System.Threading.Tasks.Task.Factory.StartNew(() =>
						{
							System.Threading.Thread.Sleep(1000);

							if (!ModEntry.BRGame.alivePlayers.Contains(Game1.player.UniqueMultiplayerID))
							{
								var oldLocation = Game1.player.currentLocation;
								var oldPosition = new xTile.Dimensions.Location(
											(int)Game1.player.Position.X - Game1.viewport.Width / 2,
											(int)Game1.player.Position.Y - Game1.viewport.Height / 2);

								SpectatorMode.EnterSpectatorMode(oldLocation, oldPosition);
							}
						});

						
					}
					return false;
				case NetworkUtility.MessageTypes.GIVE_HAT:
					int hatID = 10;//Chicken hat
					Game1.player?.addItemToInventory(new StardewValley.Objects.Hat(hatID));
					return false;
				default:
					return true;
			}
		}

		
	}
}
