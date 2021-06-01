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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.CompilerServices;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Xoshiro.PRNG32;
using Xoshiro.PRNG64;

namespace EvenBetterRNG
    {

    internal class RogueRandomDetails
        {
        internal DateTime HappenedAt;
        internal string AssemblyQualifiedName;
        internal string AssemblyCodeBase;
        internal RogueRandomDetails(Random rand) {
            HappenedAt = DateTime.Now;
            Type T = rand.GetType();
            AssemblyQualifiedName = T.AssemblyQualifiedName;
            AssemblyCodeBase = T.Assembly.CodeBase;
            }
        }

    internal class EvenBetterRNG
        {
        /// <summary>
        /// The XoShiRo-family PRNG class which will be instantiated into a new RNG
        /// </summary>
        public static Type PRNG_Class { get; private set; }
        /// <summary>
        /// The actual Random Number Generator, instantiated from PRNG_Class
        /// </summary>
        public static Random RandGen { get; private set; }

        // https://stackoverflow.com/a/56190306/149900
        const double ONE_TENTH_POS = 0.100000001490116;
        const double ONE_TENTH_NEG = -0.100000001490116;
        // I'm not a Tau-zealot, but in this case, we use TAU constant to not always perform
        // double-multiplication during Box-Muller transform.
        const double TAU = 2.0 * Math.PI;

        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly IManifest ModManifest;

        public static ModConfig Config { get; private set; }
        private static bool BlockOverwritingTemporarily = false;

        /* Internal States */
        private readonly StringBuilder sb = new StringBuilder();
        private readonly List<RogueRandomDetails> RogueRandoms = new List<RogueRandomDetails>();

        public EvenBetterRNG(IModHelper helper, IMonitor monitor, IManifest manifest) {
            Helper = helper;
            Monitor = monitor;
            ModManifest = manifest;

            EvenBetterRNGAPI.Monitor = Monitor;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (Environment.Is64BitProcess) {
                Monitor.Log("Process detected as 64-bit, using XoShiRo256**");
                PRNG_Class = typeof(XoShiRo256starstar);
                }
            else {
                Monitor.Log("Process detected as 32-bit, using XoShiRo128**");
                PRNG_Class = typeof(XoShiRo128starstar);
                }

            Config = Helper.ReadConfig<ModConfig>();
            LogOriginalRNG();
            InstallRNG();
            stopwatch.Stop();
            Monitor.Log($"Initialization takes {stopwatch.ElapsedMilliseconds}ms to complete");
            }

        private void LogOriginalRNG() {
            Monitor.Log("Original Game1.random is:");
            Type rT = Game1.random.GetType();
            Monitor.Log($"  {rT.AssemblyQualifiedName}");
            Monitor.Log($"  from {rT.Assembly.CodeBase}");
            }

        private void InstallRNG() {
            BlockOverwritingTemporarily = true;
            var prng_name = PRNG_Class.Name.Replace("starstar", "**");
            if (Config.RNGSeed == 0) {
                Monitor.Log($"{prng_name} seed is lib default", LogLevel.Debug);
                RandGen = (Random)Activator.CreateInstance(EvenBetterRNG.PRNG_Class);
                }
            else {
                Monitor.Log($"{prng_name} seed is {Config.RNGSeed}", LogLevel.Debug);
                RandGen = (Random)Activator.CreateInstance(EvenBetterRNG.PRNG_Class, (long)Config.RNGSeed);
                }
            Game1.random = RandGen;

            SanityCheck();
            BlockOverwritingTemporarily = false;
            }

        internal void SanityCheck() {
            sb.Clear()
              .Append("Sanity Test: Five random numbers: ");
            for (int i = 0; i < 5; i++) {
                sb.Append(RandGen.Next().ToString())
                  .Append(" ");
                }
            this.Monitor.Log(sb.ToString(), LogLevel.Debug);
            }

        /**************************************************
         * EVENT HANDLERS
         **************************************************/

        internal void OnDayStarted(object sender, DayStartedEventArgs args) {
            if (Config.OverrideDailyLuck) {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Monitor.Log("Re-randomizing Luck on day start", LogLevel.Trace);
                RandomizeDailyLuck();
                stopwatch.Stop();
                Monitor.Log($"Rerandomization takes {stopwatch.ElapsedMilliseconds}ms to complete");
                }
            }

        internal void OnButtonsChanged(object sender, ButtonsChangedEventArgs e) {
            if (Config.ReloadHotkey.JustPressed()) {
                Monitor.Log("Detected hotkey pressed");
                Config = Helper.ReadConfig<ModConfig>();
                Monitor.Log("Configuration reloaded", LogLevel.Info);
                if (Config.ReseedOnReload) {
                    Monitor.Log("ReseedOnReload specified; reseeding");
                    InstallRNG();
                    }
                }
            if (Config.ExperimentalChangeTodaysLuckHotkey.JustPressed()) {
                Monitor.Log("EXPERIMENTAL: Changing today's luck", LogLevel.Info);
                RandomizeDailyLuck();
                }
            }

        internal void OnGameLaunched(object sender, GameLaunchedEventArgs args) {
            RegisterConfigMenu();
            }

        internal void OnUpdateTicking(object sender, UpdateTickingEventArgs args) {
            OverwriteRogueRandomIfNeeded();
            }

        internal void OnOneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs args) {
            // Skip OverwriteRogueRandom if we already do it per tick
            if (Config.ExperimentalForcePerTick) return;
            OverwriteRogueRandomIfNeeded();
            }

        internal void OnTimeChanged(object sender, TimeChangedEventArgs args) {
            ReportRogueRandomsIfAny();
            }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OverwriteRogueRandomIfNeeded() {
            if (BlockOverwritingTemporarily) return;
            if (Game1.random == RandGen) return;
            RogueRandoms.Add(new RogueRandomDetails(Game1.random));
            Game1.random = RandGen;
            }

        private void ReportRogueRandomsIfAny(bool clear = true) {
            if (RogueRandoms.Count == 0) return;
            Monitor.Log("Since last TimeChanged, the following rogue randoms were encountered:");
            string patt = "HH:mm:ss.fff";
            LogLevel lvl = clear ? LogLevel.Trace : LogLevel.Info;
            foreach (var rr in RogueRandoms) {
                Monitor.Log($"  On {rr.HappenedAt.ToString(patt)}:", lvl);
                Monitor.Log($"    {rr.AssemblyQualifiedName}", lvl);
                Monitor.Log($"    {rr.AssemblyCodeBase}", lvl);
                }
            if (clear) RogueRandoms.Clear();
            }

        private void RegisterConfigMenu() {
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api == null) {
                Monitor.Log("GenericModConfigMenu not installed, skipping menu registry.");
                return;
                }
            Config = Helper.ReadConfig<ModConfig>();

            var _reseed_onsave = false;

            void commit(bool reseed) {
                Monitor.Log("Writing new Config");
                Helper.WriteConfig<ModConfig>(Config);
                if (reseed) {
                    Monitor.Log("Immediate reseeding was requested");
                    InstallRNG();
                    }
                _reseed_onsave = false; // Not live; will show after closing the configmenu and reopening
                }

            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => commit(_reseed_onsave));

            api.SetDefaultIngameOptinValue(ModManifest, true);
            // From this point on, because Optin is true, all options will be changeable in-game

            api.RegisterLabel(ModManifest, "Basic Settings", "");

            api.RegisterParagraph(ModManifest, "These settings are LIVE. As soon as you Save, they will be effective.");

            api.RegisterSimpleOption(ModManifest, "Override daily luck",
                "If enabled, then recalculate daily luck using the new, better RNG on every DayStart. Changing this will not affect today's Luck",
                () => Config.OverrideDailyLuck,
                (bool val) => Config.OverrideDailyLuck = val
                );

            api.RegisterSimpleOption(ModManifest, "Use Gaussian dist. for luck",
                "Instead of Uniform distribution, use Gaussian/Normal/BellCurve distribution for calculating luck. " +
                "Enabling this means that most of the time your luck will be neutral, sometimes lucky/unlucky, and rarely very lucky/unlucky." +
                "Kind of like Real Life :) ... this setting will be ignored if Override Daily Luck is disabled.",
                () => Config.GaussianLuck,
                (bool val) => Config.GaussianLuck = val
                );

            api.RegisterSimpleOption(ModManifest, "Reload Hotkey", "Hotkey to reload config in-game.",
                () => Config.ReloadHotkey,
                (KeybindList val) => Config.ReloadHotkey = val
                );

            api.RegisterLabel(ModManifest, "Advanced Settings", "Do NOT change any settings down here if you don't understand what they're for!");

            api.RegisterSimpleOption(ModManifest, "RNG Seed", "If 0, use a randomized seed. If NOT 0, use that as the PRNG seed. Do NOT change if you don't understand!",
                () => Config.RNGSeed,
                (int val) => Config.RNGSeed = val
                );

            api.RegisterSimpleOption(ModManifest, "Immediate re-seed on save", "Immediately apply new RNG Seed on save. Do NOT enable this if you're not sure!",
                () => _reseed_onsave,
                (bool val) => _reseed_onsave = val
                );
            api.RegisterParagraph(ModManifest, "Note: If 'Immediate re-seed' not enabled, you can force re-seed by pressing the Hotkey, provided the next option is enabled.");

            api.RegisterSimpleOption(ModManifest, "Reseed PRNG on reload", "",
                () => Config.ReseedOnReload,
                (bool val) => Config.ReseedOnReload = val
                );
            api.RegisterParagraph(ModManifest, "Note: If this is disabled, then pressing Reload Hotkey won't re-seed the RNG.");

            api.RegisterSimpleOption(ModManifest, "Gaussian Standard Deviation",
                "If Gaussian dist. for luck is enabled, this sets the Gaussian distribution's Standard Deviation. " +
                "If you're not sure of the effect, please don't change this value.",
                () => Config.GaussianLuckStdDev,
                (float val) => Config.GaussianLuckStdDev = val
                );
            }

        internal void ExecCli(string command, string[] args) {
            switch (command) {
                case "rng_get5":
                    SanityCheck();
                    return;
                case "rng_seed":
                    Config.RNGSeed = args.Length > 0 ? int.Parse(args[0]) : 0;
                    InstallRNG();
                    return;
                case "rng_rogues":
                    ReportRogueRandomsIfAny(clear: false);
                    return;
                default:
                    Monitor.Log($"Command not recognized: {command} {args}");
                    return;
                }
            }

        /**************************************************
         * Mathemagic here
         **************************************************/

        private double GaussianLuck() {
            // Box-Muller transform
            // See: https://stackoverflow.com/a/218600/149900
            double u1 = 1.0 - RandGen.NextDouble(); // uniform(0,1] random doubles. subtraction necessary to prevent getting u1=0.0, which will blow up Math.Log
            double u2 = RandGen.NextDouble();       // no such problem for u2. [0,1) is perfectly okay for Math.Sin
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(TAU * u2); // gaussian distrib around 0
            double randNormal = (double)Config.GaussianLuckStdDev * randStdNormal;      // stretch horizontally to get wanted curve
            double newLuckValue = randNormal / 1000.0;
            Monitor.Log($"Gaussian NewLuck: {newLuckValue}", LogLevel.Trace);
            return newLuckValue;
            }

        private double UniformLuck() {
            double newLuckValue = RandGen.Next(-100, 101) / 1000.0;  // The range is [-100, 101) (inclusive left, exclusive right)
            Monitor.Log($"Uniform NewLuck: {newLuckValue}", LogLevel.Trace);
            return newLuckValue;
            }

        private void RandomizeDailyLuck() {
            Monitor.Log($"Original luck: {Game1.player.team.sharedDailyLuck.Value}", LogLevel.Trace);
            double newLuckValue = Config.GaussianLuck ? GaussianLuck() : UniformLuck();
            // Clamp the values, this is due to we calculating using double but the game seems to want float (single) instead
            Game1.player.team.sharedDailyLuck.Value = Math.Min(ONE_TENTH_POS, Math.Max(ONE_TENTH_NEG, newLuckValue));
            }

        }
    }
