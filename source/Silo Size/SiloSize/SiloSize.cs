/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-silo-size/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

using PatcherHelper;
using Harmony;
using System.Reflection.Emit;
using StardewModdingAPI.Events;
using System.Reflection;
using ContentPatcher;

namespace SiloSize
    {
    class SiloSize
        {
        const int ORIG_SILO_SIZE = 240;

        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly IManifest Manifest;
        private readonly ModConfig Config;

        private readonly int Capacity;

        public SiloSize(Mod mod) {
            Helper = mod.Helper;
            Monitor = mod.Monitor;
            Manifest = mod.ModManifest;
            Config = Helper.ReadConfig<ModConfig>();

            Capacity = Math.Max(1, Config.SiloSize);
            try {
                SiloSizePatcher.Initialize(mod, Capacity);
                SiloSizePatcher.Execute();
                SiloSizePatcher.Cleanup();
                string comparo = Capacity > ORIG_SILO_SIZE ? "bigger" : Capacity < ORIG_SILO_SIZE ? "smaller" : "same-sized";
                Monitor.Log($"All patching successful. Enjoy your {comparo} silos!", LogLevel.Info);
                }
            catch (Exception ex) {
                Monitor.Log($"Patching failed. Technical details:\n{ex}", LogLevel.Error);
                }
            }

        public void OnDayStarted(object sender, DayStartedEventArgs args) {
            if (!Config.MorningReport) return;
            var silos = Utility.numSilos();
            if (silos < 1) return;
            var farm = Game1.getFarm();
            string stored_hay = Helper.Translation.Get("stored_hay");
            var msg = $"{stored_hay}: {farm.piecesOfHay}/{silos * Capacity}";
            Game1.addHUDMessage(new HUDMessage(msg, ""));
            Monitor.Log($"HUD Message: {msg}");
            }

        public void OnGameLaunched(object sender, GameLaunchedEventArgs args) {
            RegisterCPToken();
            }

        public void RegisterCPToken() {
            var api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (api is null) {
                Monitor.Log("Content Patcher not installed, skipping CP Token registration");
                return;
                }
            api.RegisterToken(Manifest, "silosize", () => new string[] { Config.SiloSize.ToString() });
            Monitor.Log("Sucessfully registered CP Token 'silosize'");
            }
        }

    }
