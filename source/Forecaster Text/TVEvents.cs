/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-Forecaster
**
*************************************************/

/*
 * This software is licensed under the MIT License
 * https://github.com/GStefanowich/SDV-Forecaster
 *
 * Copyright (c) 2019 Gregory Stefanowich
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Linq;
using ForecasterText.Objects.Messages;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;

namespace ForecasterText {
    internal class TVEvents {
        private readonly ModEntry Mod;
        private readonly ForecasterConfigManager ConfigManager;
        
        private ITranslationHelper Translations => this.Mod.Helper.Translation;
        
        private ForecasterConfig Config => this.ConfigManager.ModConfig;
        private ForecasterConfig MultiplayerConfig => this.Config is {UseSameForOthers: true} ? this.Config : this.ConfigManager.MultiplayerConfig;
        
        public TVEvents(
            ModEntry mod,
            ForecasterConfigManager config
        ) {
            this.Mod = mod;
            this.ConfigManager = config;
        }
        
        public void OnDayStart(object? sender, DayStartedEventArgs e) {
            if (!Context.IsWorldReady) {
                this.Mod.Monitor.Log("World is not ready yet", LogLevel.Error);
                return;
            }
            
            // Send messages for each event
            this.SendChatMessages();
        }
        public void OnFarmerJoin(object? sender, Farmer farmer) {
            // Send messages for each event
            this.SendChatMessages(farmer);
        }
        
        #region Show messages in chat
        
        private void SendChatMessages(Farmer? peer = null) {
            ISourceMessage[] messages = {
                new WeatherMessage.PelicanTown(),
                new WeatherMessage.GingerIsland(),
                new SpiritMoodMessage(),
                new QueenOfSauceMessage(this.Mod),
                new BirthdaysMessage()
            };
            
            foreach (ISourceMessage message in messages) {
                if (peer is null)
                    this.SendChatMessage(message);
                else this.SendChatMessageToPeer(peer, message);
            }
        }
        private void SendChatMessage(ISourceMessage? message) {
            // Ignore if trying to send a disabled message
            if (message is null)
                return;
            
            // Write the message to ourselves
            if (message.Write(Game1.player, this.Translations, this.Config) is string written)
                Game1.chatBox.addInfoMessage(written);
            
            // If the mod should be shared with non-mod players
            if (this.Config.SendToOthers) {
                foreach (Farmer farmer in Game1.getOnlineFarmers().Where(farmer => !this.Mod.PlayerHasMod(farmer)))
                    this.SendChatMessageToPeer(farmer, message);
            }
        }
        private void SendChatMessageToPeer(Farmer farmer, ISourceMessage message) {
            ISourceMessage writeOut = (message as MessageSource)?.Message ?? message;
            if (writeOut.Write(farmer, this.Translations, this.MultiplayerConfig) is not string written)
                return;
            string name;
            string text;
            
            if (message is MessageSource source) {
                name = source.T9N;
                text = written;
            } else if (written.Split(':', 2) is {Length: 2} split) {
                name = split[0].TrimEnd();
                text = split[1].TrimStart();
            } else return;
            
            OutgoingMessage outgoing = new(
                15,
                farmer,
                (object)"ChatMessageFormat",
                (object)new[] {
                    name,
                    text
                }
            );
            
            if (Game1.IsServer && Game1.server is IGameServer server) {
                server.sendMessage(
                    outgoing.FarmerID,
                    15,
                    Game1.player,
                    (object)"ChatMessageFormat",
                    (object)new[] {
                        name,
                        text
                    }
                );
            } else if (Game1.IsClient)
                Game1.client.sendMessage(outgoing);
        }
        
        #endregion
    }
}