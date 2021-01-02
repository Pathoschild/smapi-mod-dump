/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewToDew
**
*************************************************/

// Copyright 2020 Jamie Taylor
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ToDew {
    /// <summary>The configuration data model.</summary>
    public class ModConfig {
        public SButton hotkey = SButton.L;
        public bool debug = false;
    }
    /// <summary>The To-Dew mod.</summary>
    /// Encapsulates the game lifecycle events and orchestrates the UI (ToDoMenu) data
    /// model (ToDoList) bits.
    public class ModEntry : Mod {
        private ToDoList list;
        internal ModConfig config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
            helper.Events.GameLoop.GameLaunched += onLaunched;
            this.config = helper.ReadConfig<ModConfig>();
        }

        private void onLaunched(object sender, GameLaunchedEventArgs e) {
            // integrate with Generic Mod Config Menu, if installed
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api != null) {
                api.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
                api.RegisterSimpleOption(ModManifest, "Hotkey", "The key to bring up the to-do list", () => config.hotkey, (SButton val) => config.hotkey = val);
                api.RegisterSimpleOption(ModManifest, "Debug", "Enable debugging output in the log", () => config.debug, (bool val) => config.debug = val);
            }
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e) {
            if (config.debug) {
                Monitor.Log($"Received mod message {e.Type} from {e.FromModID}", LogLevel.Debug);
            }
            if (e.FromModID.Equals(this.ModManifest.UniqueID)) {
                list.ReceiveModMessage(e);
            }
        }

        private void OnPeerConnected(object sender, PeerConnectedEventArgs e) {
            if (config.debug) {
                this.Monitor.Log("OnPeerConnected", LogLevel.Debug);
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
            if (config.debug) {
                this.Monitor.Log("OnSaveLoaded", LogLevel.Debug);
                this.Monitor.Log($"My multiplayer ID: {Game1.player.UniqueMultiplayerID}", LogLevel.Debug);
            }
            list = new ToDoList(this);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
            if (Context.IsWorldReady
                && Context.IsPlayerFree
                && e.Button == this.config.hotkey
                && !this.list.IncompatibleMultiplayerHost) {
                if (Game1.activeClickableMenu != null)
                    Game1.exitActiveMenu();
                Game1.activeClickableMenu = new ToDoMenu(this.Monitor, this.list);
            }
        }
    }
    // See https://github.com/spacechase0/GenericModConfigMenu/blob/master/Api.cs for full API
    public interface GenericModConfigMenuAPI {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<SButton> optionGet, Action<SButton> optionSet);
    }
}
