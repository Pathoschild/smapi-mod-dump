/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmods-artifact-fix-redux/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;

using PatcherHelper;

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ArtifactSystemFixed_Redux
    {
    class ASFRedux
        {
        internal static IModHelper Helper;
        internal static ModConfig Config;

        public ASFRedux(Mod mod) {
            Helper = mod.Helper;
            Config = Helper.ReadConfig<ModConfig>();

            PerformPatching();

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            }

        ~ASFRedux() {
            Log.Error("ASFRedux got garbage-collected! This is not supposed to happen!!");
            }

        public void OnGameLaunched(object sender, GameLaunchedEventArgs args) {
            var ebrng_api = Helper.ModRegistry.GetApi<EvenBetterRNG.IEvenBetterRNGAPI>("pepoluan.EvenBetterRNG");
            if (ebrng_api is null) {
                Log.Debug("'EvenBetterRNG' mod not loaded; using Game1.random as is.");
                return;
                }
            try {
                Patches.RNG = ebrng_api.GetMainRandom();
                Log.Debug("Using high-quality PRNG provided by the 'EvenBetterRNG' mod. You're awesome!");
                }
            catch (Exception) {
                Log.Warn("Failure invoking EBRNG.GetMainRandom() even though 'EvenBetterRNG' mod is loaded. Probably need to update it?");
                Log.Warn("Reverting to using Game1.random as is.");
                }
            }

        internal void PerformPatching() {
            HarmonyPatcherHelper harp = new(ASFR_Mod.Instance);

            harp.TryPatching(
                typeof(StardewValley.GameLocation),
                nameof(StardewValley.GameLocation.digUpArtifactSpot),
                prefix: Patches.Get(nameof(Patches.GameLocation_digUpArtifactSpot_prefix)),
                postfix: Patches.Get(nameof(Patches.GameLocation_digUpArtifactSpot_postfix))
                );

            harp.TryPatching(
                typeof(StardewValley.Utility),
                nameof(StardewValley.Utility.getTreasureFromGeode),
                prefix: Patches.Get(nameof(Patches.Utility_getTreasureFromGeode_prefix)),
                postfix: Patches.Get(nameof(Patches.Utility_getTreasureFromGeode_postfix))
                );

            Log.Debug("All patching succesful. Mod is now active.");
            }

        }
    }
