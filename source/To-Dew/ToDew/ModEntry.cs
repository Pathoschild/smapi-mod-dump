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
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ToDew {
    /// <summary>The configuration data model.</summary>
    public class ModConfig {
        public SButton hotkey = SButton.L;
        public KeybindList hotkeyList = new KeybindList();
        public SButton secondaryCloseButton = SButton.ControllerBack;
        public bool debug = false;
        public OverlayConfig overlay = new OverlayConfig();
    }
    /// <summary>The To-Dew mod.</summary>
    /// Encapsulates the game lifecycle events and orchestrates the UI (ToDoMenu) data
    /// model (ToDoList) bits.
    public class ModEntry : Mod {
        private readonly PerScreen<ToDoList> list = new PerScreen<ToDoList>();
        private readonly PerScreen<ToDoOverlay> overlay = new PerScreen<ToDoOverlay>();
        internal ModConfig config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            I18n.Init(helper.Translation);
            this.config = helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
            helper.Events.GameLoop.GameLaunched += onLaunched;
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
            helper.Events.GameLoop.Saving += onSaving;
            helper.Events.GameLoop.DayStarted += onDayStarted;
        }

        private void onLaunched(object sender, GameLaunchedEventArgs e) {
            // integrate with Generic Mod Config Menu, if installed
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api != null) {
                api.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
                api.RegisterSimpleOption(ModManifest, I18n.Config_Hotkey(), I18n.Config_Hotkey_Desc(), () => config.hotkey, (SButton val) => config.hotkey = val);
                api.RegisterSimpleOption(ModManifest, I18n.Config_SecondaryCloseButton(), I18n.Config_SecondaryCloseButton_Desc(), () => config.secondaryCloseButton, (SButton val) => config.secondaryCloseButton = val);
                api.RegisterSimpleOption(ModManifest, I18n.Config_Debug(), I18n.Config_Debug_Desc(), () => config.debug, (bool val) => config.debug = val);
                OverlayConfig.RegisterConfigMenuOptions(() => config.overlay, api, ModManifest);
            }

            // integrate with MobilePhone, if installed
            var phoneApi = Helper.ModRegistry.GetApi<IMobilePhoneApi>("aedenthorn.MobilePhone");
            if (phoneApi != null) {
                // This is a whole lot of trouble to be able to use something out of one of the built-in
                // tile sheets, since the mobile phone api doesn't support specifying a rectangle for
                // the sprite.
                Texture2D originalTexture = Game1.content.Load<Texture2D>("Maps\\TownIndoors");
                Rectangle sourceRectangle = new Rectangle(202, 1870, 48, 48);
                Texture2D cropTexture = new Texture2D(Game1.graphics.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
                Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
                originalTexture.GetData(0, sourceRectangle, data, 0, data.Length);
                cropTexture.SetData(data);

                phoneApi.AddApp(Helper.ModRegistry.ModID, "To-Dew", () => { Game1.activeClickableMenu = new ToDoMenu(this, this.list.Value); } , cropTexture);
            }
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e) {
            if (config.debug) {
                Monitor.Log($"Received mod message {e.Type} from {e.FromModID}", LogLevel.Debug);
            }
            if (e.FromModID.Equals(this.ModManifest.UniqueID)) {
                list.Value?.ReceiveModMessage(e);
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
            list.Value = new ToDoList(this);
            if (config.overlay.enabled) {
                overlay.Value = new ToDoOverlay(this, list.Value);
            }
        }

        private void onDayStarted(object sender, DayStartedEventArgs e) {
            list.Value?.RefreshVisibility();
        }

        private void onSaving(object sender, SavingEventArgs e) {
            list.Value?.PreSaveCleanup();
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e) {
            list.Value = null;
            overlay.Value?.Dispose();
            overlay.Value = null;
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e) {
            if (Context.IsWorldReady
                && Context.IsPlayerFree
                && list.Value != null
                && !this.list.Value.IncompatibleMultiplayerHost) {

                if (e.Pressed.Contains(this.config.hotkey) || this.config.hotkeyList.JustPressed()) {
                    if (Game1.activeClickableMenu != null)
                        Game1.exitActiveMenu();
                    Game1.activeClickableMenu = new ToDoMenu(this, this.list.Value);
                }
                if (e.Pressed.Contains(this.config.overlay.hotkey) || this.config.overlay.hotkeyList.JustPressed()) {
                    if (overlay.Value != null) {
                        overlay.Value.Dispose();
                        overlay.Value = null;
                    } else if (config.overlay.enabled) {
                        overlay.Value = new ToDoOverlay(this, list.Value);
                    }
                }
            }
        }
    }
    // See https://github.com/spacechase0/GenericModConfigMenu/blob/master/Api.cs for full API
    public interface GenericModConfigMenuAPI {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void RegisterLabel(IManifest mod, string labelName, string labelDesc);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<SButton> optionGet, Action<SButton> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet);
    }
    // See https://www.nexusmods.com/stardewvalley/articles/467
    public interface IMobilePhoneApi {
        bool AddApp(string id, string name, Action action, Texture2D icon);
    }
}
