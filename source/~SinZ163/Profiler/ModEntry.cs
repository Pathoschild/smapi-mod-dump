/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler
{
    public class ModEntry : Mod
    {
        internal ModConfig Config { get; private set; }
        public Stopwatch timer { get; private set; }

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.timer = new Stopwatch();
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoadedFast;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStartedFast;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunchedFast;
            helper.Events.Specialized.LoadStageChanged += Specialized_LoadStageChangedFast;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoadedSlow;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStartedSlow;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunchedSlow;
            helper.Events.Specialized.LoadStageChanged += Specialized_LoadStageChangedSlow;

            ManagedEventPatches.Initialize(Monitor, this.Config, new HarmonyLib.Harmony(this.ModManifest.UniqueID));
            this.timer.Restart();
        }

        [EventPriority((EventPriority)Int32.MaxValue)]
        private void Specialized_LoadStageChangedFast(object sender, StardewModdingAPI.Events.LoadStageChangedEventArgs e)
        {
            Monitor.Log($"[{timer.Elapsed.TotalMilliseconds:N}][Fast] LoadStageChanged {e.OldStage} -> {e.NewStage}", LogLevel.Info);
        }

        [EventPriority((EventPriority)Int32.MaxValue)]
        private void GameLoop_GameLaunchedFast(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            Monitor.Log($"[{timer.Elapsed.TotalMilliseconds:N}][Fast] Game Launched", LogLevel.Info);
        }

        [EventPriority((EventPriority)Int32.MaxValue)]
        private void GameLoop_DayStartedFast(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            Monitor.Log($"[{timer.Elapsed.TotalMilliseconds:N}][Fast] Day Started", LogLevel.Info);
        }

        [EventPriority((EventPriority)Int32.MaxValue)]
        private void GameLoop_SaveLoadedFast(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            Monitor.Log($"[{timer.Elapsed.TotalMilliseconds:N}][Fast] Save Loaded", LogLevel.Info);
        }

        [EventPriority((EventPriority)Int32.MinValue)]
        private void Specialized_LoadStageChangedSlow(object sender, StardewModdingAPI.Events.LoadStageChangedEventArgs e)
        {
            Monitor.Log($"[{timer.Elapsed.TotalMilliseconds:N}][Slow] LoadStageChanged {e.OldStage} -> {e.NewStage}", LogLevel.Info);
        }

        [EventPriority((EventPriority)Int32.MinValue)]
        private void GameLoop_GameLaunchedSlow(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            Monitor.Log($"[{timer.Elapsed.TotalMilliseconds:N}][Slow] Game Launched", LogLevel.Info);
        }

        [EventPriority((EventPriority)Int32.MinValue)]
        private void GameLoop_DayStartedSlow(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            Monitor.Log($"[{timer.Elapsed.TotalMilliseconds:N}][Slow] Day Started", LogLevel.Info);
        }

        [EventPriority((EventPriority)Int32.MinValue)]
        private void GameLoop_SaveLoadedSlow(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            Monitor.Log($"[{timer.Elapsed.TotalMilliseconds:N}][Slow] Save Loaded", LogLevel.Info);
        }
    }
}
