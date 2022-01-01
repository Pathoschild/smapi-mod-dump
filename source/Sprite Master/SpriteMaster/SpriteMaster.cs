/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

global using XNA = Microsoft.Xna.Framework;
global using DrawingPoint = System.Drawing.Point;
global using DrawingRectangle = System.Drawing.Rectangle;
global using DrawingSize = System.Drawing.Size;
global using XTilePoint = xTile.Dimensions.Location;
global using XTileRectangle = xTile.Dimensions.Rectangle;
global using XTileSize = xTile.Dimensions.Size;
using HarmonyLib;
using SpriteMaster.Extensions;
using SpriteMaster.Harmonize;
using SpriteMaster.Metadata;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: CompilationRelaxations(CompilationRelaxations.NoStringInterning)]
[module: SkipLocalsInit]

namespace SpriteMaster;
public sealed class SpriteMaster : Mod {
	internal static SpriteMaster Self { get; private set; } = default;

	private static readonly bool DotNet = (Runtime.Framework != Runtime.FrameworkType.Mono);
	private readonly Thread MemoryPressureThread = null;
	private readonly Thread GarbageCollectThread = null;
	private readonly object CollectLock = DotNet ? new() : null;
	internal static string AssemblyPath { get; private set; }

	internal static void DumpStats() {
		var currentProcess = Process.GetCurrentProcess();
		var workingSet = currentProcess.WorkingSet64;
		var vmem = currentProcess.VirtualMemorySize64;
		var gc_allocated = GC.GetTotalMemory(false);

		var lines = new List<string> {
			"SpriteMaster Stats Dump:",
			"\tVM:",
			$"\t\tProcess Working Set    : {workingSet.AsDataSize()}",
			$"\t\tProcess Virtual Memory : {vmem.AsDataSize()}:",
			$"\t\tGC Allocated Memory    : {gc_allocated.AsDataSize()}:",
			"",
		};

		ManagedTexture2D.DumpStats(lines);

		foreach (var line in lines) {
			Debug.InfoLn(line);
		}
	}

	private void MemoryPressureLoop() {
		for (; ; ) {
			if (DrawState.TriggerGC && DrawState.TriggerGC.Wait()) {
				continue;
			}

			lock (CollectLock) {
				try {
					using var _ = new MemoryFailPoint(Config.Garbage.RequiredFreeMemory);
					Thread.Sleep(128);
				}
				catch (InsufficientMemoryException) {
					Debug.WarningLn($"Less than {(Config.Garbage.RequiredFreeMemory * 1024 * 1024).AsDataSize(decimals: 0)} available for block allocation, forcing full garbage collection");
					MTexture2D.PurgeDataCache();
					DrawState.TriggerGC.Set(true);
					Thread.Sleep(10000);
				}
			}
		}
	}

	private void GarbageCheckLoop() {
		try {
			for (; ; ) {
				GC.RegisterForFullGCNotification(10, 10);
				GC.WaitForFullGCApproach();
				if (Garbage.ManualCollection) {
					Thread.Sleep(128);
					continue;
				}
				lock (CollectLock) {
					if (DrawState.TriggerGC && DrawState.TriggerGC.Wait()) {
						continue;
					}

					MTexture2D.PurgeDataCache();
					DrawState.TriggerGC.Set(true);
					// TODO : Do other cleanup attempts here.
				}
			}
		}
		catch {

		}
	}

	private const string ConfigName = "config.toml";

	private static volatile string CurrentSeason = "";

	public SpriteMaster() {
		Contract.AssertNull(Self);
		Self = this;

		if (DotNet) {
			MemoryPressureThread = new Thread(MemoryPressureLoop) {
				Name = "Memory Pressure Thread",
				Priority = ThreadPriority.BelowNormal,
				IsBackground = true
			};

			GarbageCollectThread = new Thread(GarbageCheckLoop) {
				Name = "Garbage Collection Thread",
				Priority = ThreadPriority.BelowNormal,
				IsBackground = true
			};
		}
	}

	private bool IsVersionOutdated(string configVersion) {
		string referenceVersion = Config.ClearConfigBefore;

		var configStrArray = configVersion.Split('.').BeList();
		var referenceStrArray = referenceVersion.Split('.').BeList();

		try {
			while (configStrArray.Count > referenceStrArray.Count) {
				referenceStrArray.Add("0");
			}
			while (referenceStrArray.Count > configStrArray.Count) {
				configStrArray.Add("0");
			}

			foreach (int i in 0.RangeTo(configStrArray.Count)) {
				if (configStrArray[i].IsEmpty()) {
					return true;
				}

				var configElement = int.Parse(configStrArray[i]);
				var referenceElement = int.Parse(referenceStrArray[i]);

				if (configElement < referenceElement) {
					return true;
				}
			}
		}
		catch {
			return true;
		}
		return false;
	}

	public override void Entry(IModHelper help) {
		AssemblyPath = help.DirectoryPath;

		var ConfigPath = Path.Combine(help.DirectoryPath, ConfigName);

		using (var tempStream = new MemoryStream()) {
			SerializeConfig.Save(tempStream);

			if (!Config.IgnoreConfig) {
				SerializeConfig.Load(ConfigPath);
			}

			if (IsVersionOutdated(Config.ConfigVersion)) {
				Debug.WarningLn("config.toml is out of date, rewriting it.");

				SerializeConfig.Load(tempStream, retain: true);
				Config.ConfigVersion = Config.CurrentVersion;
			}
		}

		if (Config.ShowIntroMessage && !Config.SkipIntro) {
			help.Events.GameLoop.GameLaunched += (_, _) => {
				Game1.drawLetterMessage("Welcome to SpriteMaster!\nSpriteMaster must resample sprites as it sees them and thus some lag will likely be apparent at the start of the game, upon entering new areas, and when new sprites are seen.\n\nPlease be patient and do not take this as an indication that your computer is incapable of running SpriteMaster.\n\nEnjoy!".Replace("\n", "^"));
			};
			Config.ShowIntroMessage = false;
		}

		SerializeConfig.Save(ConfigPath);

		ConfigureHarmony();
		help.Events.Input.ButtonPressed += OnButtonPressed;

		help.ConsoleCommands.Add("spritemaster_stats", "Dump SpriteMaster Statistics", (_, _) => { DumpStats(); });
		help.ConsoleCommands.Add("spritemaster_memory", "Dump SpriteMaster Memory", (_, _) => { Debug.DumpMemory(); });
		help.ConsoleCommands.Add("spritemaster_gc", "Trigger Spritemaster GC", (_, _) => {
			lock (CollectLock) {
				Garbage.Collect(compact: true, blocking: true, background: false);
				DrawState.TriggerGC.Set(true);
			}
		});
		help.ConsoleCommands.Add("spritemaster_purge", "Trigger Spritemaster Purge", (_, _) => {
			lock (CollectLock) {
				Garbage.Collect(compact: true, blocking: true, background: false);
				MTexture2D.PurgeDataCache();
				Garbage.Collect(compact: true, blocking: true, background: false);
				DrawState.TriggerGC.Set(true);
			}
		});


		help.Events.GameLoop.DayStarted += OnDayStarted;
		// GC after major events
		help.Events.GameLoop.SaveLoaded += (_, _) => ForceGarbageCollect();
		help.Events.GameLoop.DayEnding += (_, _) => ForceGarbageCollect();
		help.Events.GameLoop.GameLaunched += (_, _) => ForceGarbageCollect();
		help.Events.GameLoop.ReturnedToTitle += (_, _) => ForceGarbageCollect();
		help.Events.GameLoop.SaveCreated += (_, _) => ForceGarbageCollect();

		MemoryPressureThread?.Start();
		GarbageCollectThread?.Start();
	}

	// SMAPI/CP won't do this, so we do. Purge the cached textures for the previous season on a season change.
	private static void OnDayStarted(object _, DayStartedEventArgs _1) {
		// Do a full GC at the start of each day
		Garbage.Collect(compact: true, blocking: true, background: false);

		var season = SDate.Now().Season.ToLower();
		if (season != CurrentSeason) {
			CurrentSeason = season;
			ScaledTexture.SpriteMap.SeasonPurge(season);
		}

		// And again after purge
		Garbage.Collect(compact: true, blocking: true, background: false);
	}

	private static void ForceGarbageCollect() {
		Garbage.Collect(compact: true, blocking: true, background: false);
	}

	private void ConfigureHarmony() {
		var instance = new Harmony($"DigitalCarbide.${Config.ModuleName}");
		instance.ApplyPatches();
	}

	private static void OnButtonPressed(object sender, ButtonPressedEventArgs args) {
		if (args.Button == Config.ToggleButton) {
			Config.Enabled = !Config.Enabled;
		}
	}
}
