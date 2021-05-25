/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-tapper-report
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace TapperReport
{
    class ModConfig
    {
        // Used to access Monitor's "Log" method
        // Access is set to "internal" to prevent this property 'leaking' to SMAPI
        internal static IMonitor Monitor;

        private static KeybindList _kbd_hot = KeybindList.Parse("F9");
        public KeybindList HotKey
        {
            get
            {
                return _kbd_hot;
            }
            set
            {
                if (_kbd_hot == value) return;
                _kbd_hot = value;
                Monitor.Log($"Keyboard hotkey changed to {value}");
            }
        }

        private static KeybindList _pad_hot = KeybindList.Parse("DPadLeft");
        public KeybindList GamePadHotKey
        {
            get
            {
                return _pad_hot;
            }
            set
            {
                if (_pad_hot == value) return;
                _pad_hot = value;
                Monitor.Log($"Gamepad hotkey changed to {value}");
            }
        }

        public string Separator { get; set; } = " | ";
        public bool CheckNewFinishedContinually { get; set; } = true;
        public int CheckEvery { get; set; } = 6;
    }

    class ModConfigMenu
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly IManifest ModManifest;
        private readonly TapperTracker Tracker;

        private ModConfig config;

        internal ModConfigMenu(IModHelper helper, IMonitor monitor, IManifest manifest, TapperTracker tracker)
        {
            Helper = helper;
            Monitor = monitor;
            ModManifest = manifest;
            Tracker = tracker;
        }

        internal void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api == null)
            {
                Monitor.Log("GenericModConfigMenu not installed, skipping menu registry.");
                return;
            }
            config = Helper.ReadConfig<ModConfig>();
            api.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => CommitConfig());

            api.SetDefaultIngameOptinValue(ModManifest, true);
            // From this point on, because Optin is true, all options will be changeable in-game

            api.RegisterLabel(ModManifest, "Tapper Report", "Settings for Tapper Report mod by pepoluan");
            api.RegisterSimpleOption(ModManifest, "Keyboard Hotkey", "", 
                () => config.HotKey, 
                (KeybindList val) => config.HotKey = val
                );
            api.RegisterSimpleOption(ModManifest, "Gamepad Hotkey", "Ignore if you don't have a gamepad",
                () => config.GamePadHotKey, 
                (KeybindList val) => config.GamePadHotKey = val
                );
            api.RegisterSimpleOption(ModManifest, "Tapper Type Separator", "",
                () => config.Separator,
                (string val) => config.Separator = val
                );
            api.RegisterSimpleOption(ModManifest, "Periodic Check for finished tapper", "",
                () => config.CheckNewFinishedContinually,
                (bool val) => config.CheckNewFinishedContinually = val
                );
            api.RegisterSimpleOption(ModManifest, "Check every Nx10 game minutes", "Will be ignored if Periodic Check is disabled",
                () => config.CheckEvery,
                (int val) => config.CheckEvery = val
                );
        }

        private void CommitConfig()
        {
            Helper.WriteConfig<ModConfig>(config);
            Tracker.UpdateConfig();
        }

    }
}
