/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-even-better-rng/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using StardewModdingAPI;

namespace EvenBetterRNG
    {
    public class ModEntry : Mod
        {
        private EvenBetterRNG NewRNG;

        public override void Entry(IModHelper helper) {
            NewRNG = new EvenBetterRNG(helper, Monitor, ModManifest);

            helper.ConsoleCommands.Add("rng_get5", "Get 5 random numbers, used to test sanity", NewRNG.ExecCli);
            helper.ConsoleCommands.Add("rng_seed", "Replace current game's RNG with a new PRNG with specified seed. Syntax: rng_seed <int>", NewRNG.ExecCli);
            helper.ConsoleCommands.Add("rng_rogues", "List 'rogue' RNGs that attempted to hijack Even Better RNG", NewRNG.ExecCli);

            helper.Events.GameLoop.GameLaunched += NewRNG.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += NewRNG.OnDayStarted;
            helper.Events.GameLoop.OneSecondUpdateTicking += NewRNG.OnOneSecondUpdateTicking;
            helper.Events.GameLoop.TimeChanged += NewRNG.OnTimeChanged;
            helper.Events.Input.ButtonsChanged += NewRNG.OnButtonsChanged;
            
            if (EvenBetterRNG.Config.ExperimentalForcePerTick) {
                Monitor.Log("Experimental Force Per Tick enabled", LogLevel.Debug);
                helper.Events.GameLoop.UpdateTicking += NewRNG.OnUpdateTicking;
                }
            }

        public override object GetApi() {
            return new EvenBetterRNGAPI();
            }
        }
    }
