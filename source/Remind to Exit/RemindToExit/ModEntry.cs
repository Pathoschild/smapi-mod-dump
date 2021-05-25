/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/projects/sdvmod-remind-to-exit/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;

namespace RemindToExit
    {
    public class ModEntry : Mod
        {
        private ModConfig Config;
        private bool RemindOnDayStart = false;
        private string ReminderBody = null;

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();
            Monitor.Log($"Activate/Deactivate HotKey is {Config.HotKeyActivate}", LogLevel.Info);
            Monitor.Log($"Custom Message Hotkey is {Config.HotKeyCustomMessage}", LogLevel.Info);
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs args) {
            RegisterConfigMenu();
            }

        private void RegisterConfigMenu(){
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api == null) {
                Monitor.Log("GenericModConfigMenu not installed, skipping menu registry.");
                return;
                }

            Config = Helper.ReadConfig<ModConfig>();
            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig<ModConfig>(Config));

            api.SetDefaultIngameOptinValue(ModManifest, true);
            // From this point on, because Optin is true, all options will be changeable in-game

            string desc;

            api.RegisterSimpleOption(ModManifest, "Enabled", "",
                () => Config.Enabled,
                (bool val) => Config.Enabled = val
                );

            api.RegisterLabel(ModManifest, "HotKeys", "");
            desc = Helper.Translation.Get("config.hotkey.desc");
            api.RegisterSimpleOption(ModManifest, "Activate/Deactivate", desc,
                () => Config.HotKeyActivate,
                (KeybindList val) => Config.HotKeyActivate = val
                );

            desc = Helper.Translation.Get("config.hotkeycustmsg.desc");
            api.RegisterSimpleOption(ModManifest, "Activate with CustomMessage", desc,
                () => Config.HotKeyCustomMessage,
                (KeybindList val) => Config.HotKeyCustomMessage = val
                );
            }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs args) {
            if (!Config.Enabled) return;
            if (Config.HotKeyCustomMessage.JustPressed()) {
                if (!Context.IsPlayerFree) return;
                ShowReminderMenu();
                }
            else if (Config.HotKeyActivate.JustPressed()) {
                if (RemindOnDayStart) {
                    Deactivate();
                    ReminderBody = null;
                    }
                else {
                    Activate();
                    }
                }
            }

        private void Activate() {
            RemindOnDayStart = true;
            string msg = Helper.Translation.Get("reminder.enabled");
            Game1.addHUDMessage(new HUDMessage(msg, ""));
            }

        private void Deactivate() {
            RemindOnDayStart = false;
            string msg = Helper.Translation.Get("reminder.disabled");
            Game1.addHUDMessage(new HUDMessage(msg, ""));
            }

        private void OnDayStarted(object sender, DayStartedEventArgs args) {
            if (!Config.Enabled) return;
            if (!RemindOnDayStart) {
                Monitor.Log("Reminder not active. Continuing", LogLevel.Info);
                return;
                }
            Monitor.Log("Reminder is active, popping up message", LogLevel.Info);
            RemindOnDayStart = false;
            string title = Helper.Translation.Get("reminder.title");
            string body = ReminderBody ?? Helper.Translation.Get("reminder.body-default");
            Game1.activeClickableMenu = new DialogueBox($"{title}^{body}");
            Monitor.Log($"Message popped with body:\n{body}");
            ReminderBody = null;
            }

        private void ShowReminderMenu() {
            var ReminderMenu = new ReminderMessageUI(Helper, ReminderBody, GetMenuButton(), OnReminderMenuClosing);
            Game1.activeClickableMenu = ReminderMenu;
            }

        private void OnReminderMenuClosing(string reason, string result) {
            switch (reason) {
                case "cancel":
                    Deactivate();
                    break;
                case "ok":
                    ReminderBody = result.Trim() == "" ? null : result;
                    Activate();
                    break;
                }
            Game1.exitActiveMenu();
            }

        /// <summary>
        /// Returns the button that is set to open the menu in current save.
        /// </summary>
        /// <returns>the button that opens menu as SButton</returns>
        public SButton GetMenuButton() {
            SButton? menuButton = null;
            if (Context.IsMainPlayer) {
                var saveFilePath = Path.Combine(Constants.CurrentSavePath, Constants.SaveFolderName);
                Monitor.Log($"Loading save file {saveFilePath}");
                var saveFile = XDocument.Load(saveFilePath);
                var query = from xml in saveFile.Descendants("menuButton")
                            select xml.Element("InputButton").Element("key").Value;
                string menuButtonString = query.First<string>();
                Monitor.Log($"menuButton = {menuButtonString}");
                menuButton = (SButton)Enum.Parse(typeof(SButton), menuButtonString);
                }
            return menuButton ?? SButton.E;
            }
        }
    }
