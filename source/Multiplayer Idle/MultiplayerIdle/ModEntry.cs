using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace MultiplayerIdle {
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.RenderedHud += this.OnRenderedHud;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            //helper.Events.Multiplayer.PeerDisconnected += this.OnPeerDisconnected;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if (e.Button == SButton.P) {
                switchIdleMode();
                showMessage($"Idle Swiths to {this.Config.IdleMethod} Mode");
            }
            if (isIdle) {
                isIdle = false;
                if (Context.IsMainPlayer) {
                    if (!CheckIdle()) {
                        showIdle = false;
                        NotifyFarmersIdle(isIdle);
                    }
                } else {
                    NotifyHosterIdle(false);
                }
            }
            lastPressedTime = Game1.currentGameTime.TotalGameTime.Seconds;
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e) {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if (showIdle) {
                var b = Game1.spriteBatch;
                b.DrawString(Game1.dialogueFont, "zzZ", new Vector2(0.0f), Game1.textColor);
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
            //
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
            isIdle = false; showIdle = false;
            lastPressedTime = Game1.currentGameTime.TotalGameTime.Seconds;
            _peers = new Dictionary<long, bool>();
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if (Game1.currentGameTime.TotalGameTime.Seconds > lastPressedTime + this.Config.IdleCheckSeconds && !isIdle) {
                isIdle = true;
                if (Context.IsMainPlayer) {
                    if (CheckIdle()) {
                        showIdle = true;
                        NotifyFarmersIdle(showIdle);
                    }
                } else {
                    NotifyHosterIdle(true);
                }
            }
            if (CheckIdle()) {
                if (Context.IsMainPlayer) {
                    Game1.gameTimeInterval = 0;
                }
            }
        }

        private void OnPeerDisconnected(object sender, PeerDisconnectedEventArgs e) {
            if (Context.IsMainPlayer) {
                long peerPlayerID = e.Peer.PlayerID;
                if (!_peers.ContainsKey(peerPlayerID)) {
                    _peers.Add(peerPlayerID, false);
                } else {
                    _peers[peerPlayerID] = false;
                }
            }
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e) {
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "IdleMessage") {
                IdleMessage message = e.ReadAs<IdleMessage>();
                // handle message fields here
                if (CheckIdle() && !message.Idle) {
                    _peers[e.FromPlayerID] = message.Idle;
                    if (!CheckIdle()) {
                        showIdle = false;
                        NotifyFarmersIdle(showIdle);
                    }
                } else if (!CheckIdle() && message.Idle) {
                    _peers[e.FromPlayerID] = message.Idle;
                    if (CheckIdle()) {
                        showIdle = true;
                        NotifyFarmersIdle(showIdle);
                    }
                }
                _peers[e.FromPlayerID] = message.Idle;
            }
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "ShowIdleMessage" && !Context.IsMainPlayer) {
                ShowIdleMessage message = e.ReadAs<ShowIdleMessage>();
                showIdle = message.Idle;
            }
        }

        private bool CheckIdle() {
            if (Context.IsMainPlayer) {
                checkOnlineFarmer();
            }
            switch (this.Config.IdleMethod) {
                case "ALL":
                    return !_peers.ContainsValue(false) && isIdle;
                case "SINGLE":
                default:
                    return _peers.ContainsValue(true) || isIdle;
            }
        }

        private void checkOnlineFarmer() {
            if (Game1.getOnlineFarmers().Count < _peers.Count-1) {
                foreach (long peerdi in _peers.Keys) {
                    if(Game1.player.UniqueMultiplayerID != peerdi && this.Helper.Multiplayer.GetConnectedPlayer(peerdi) == null) {
                        _peers.Remove(peerdi);
                    }
                }
            }
            if (Game1.getOnlineFarmers().Count > _peers.Count-1) {
                foreach (Farmer farmer in Game1.getOnlineFarmers()) {
                    if (Game1.player.UniqueMultiplayerID != farmer.UniqueMultiplayerID && !_peers.ContainsKey(farmer.UniqueMultiplayerID)) {
                        _peers.Add(farmer.UniqueMultiplayerID, false);
                    }
                }
            }
        }

        private void NotifyHosterIdle(bool idle) {
            foreach (IMultiplayerPeer peer in this.Helper.Multiplayer.GetConnectedPlayers()) {
                if (peer.HasSmapi || peer.IsHost) {
                    IdleMessage message = new IdleMessage(idle);
                    this.Helper.Multiplayer.SendMessage(message, "IdleMessage", modIDs: new[] { this.ModManifest.UniqueID }, playerIDs: new[] { peer.PlayerID });
                }
            }
        }

        private void NotifyFarmersIdle(bool idle) {
            ShowIdleMessage message = new ShowIdleMessage(idle);
            this.Helper.Multiplayer.SendMessage(message, "ShowIdleMessage", modIDs: new[] { this.ModManifest.UniqueID });
        }

        private void switchIdleMode() {
            switch (this.Config.IdleMethod) {
                case "ALL":
                    this.Config.IdleMethod = "SINGLE";
                    break;
                case "SINGLE":
                    this.Config.IdleMethod = "ALL";
                    break;
                default:
                    this.Config.IdleMethod = "SINGLE";
                    break;
            }
            this.Helper.WriteConfig(this.Config);
        }

        private void showMessage(string str) {
            Game1.addHUDMessage(new HUDMessage(str, ""));
        }

        private Dictionary<long, bool> _peers;
        private bool showIdle;
        private bool isIdle;
        private int lastPressedTime;
        private ModConfig Config;
    }
}