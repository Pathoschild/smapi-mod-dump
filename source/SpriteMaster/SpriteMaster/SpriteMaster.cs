/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using HarmonyLib;
using LinqFasterer;
using SpriteMaster.Caching;
using SpriteMaster.Extensions;
using SpriteMaster.Harmonize;
using SpriteMaster.Harmonize.Patches.Game;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster;

public sealed class SpriteMaster : Mod {
	internal static SpriteMaster Self { get; private set; } = default!;

	private static readonly bool DotNet = (Runtime.Framework != Runtime.FrameworkType.Mono);
	private readonly Thread? MemoryPressureThread = null;
	private readonly Thread? GarbageCollectThread = null;
	private readonly object? CollectLock = DotNet ? new() : null;
	internal bool IsGameLoaded { get; private set; } = false;
	internal static string? AssemblyPath { get; private set; }

	internal static readonly string ChangeList = typeof(SpriteMaster).Assembly.GetCustomAttribute<ChangeListAttribute>()?.Value ?? "local";
	internal static readonly string BuildComputerName = typeof(SpriteMaster).Assembly.GetCustomAttribute<BuildComputerNameAttribute>()?.Value ?? "unknown";
	internal static readonly string FullVersion = typeof(SpriteMaster).Assembly.GetCustomAttribute<FullVersionAttribute>()?.Value ?? Config.CurrentVersion;

	internal static void DumpAllStats() {
		var currentProcess = Process.GetCurrentProcess();
		var workingSet = currentProcess.WorkingSet64;
		var virtualMem = currentProcess.VirtualMemorySize64;
		var gcAllocated = GC.GetTotalMemory(false);

		var lines = new List<string> {
			"SpriteMaster Stats Dump:",
			"\tVM:",
			$"\t\tProcess Working Set    : {workingSet.AsDataSize()}",
			$"\t\tProcess Virtual Memory : {virtualMem.AsDataSize()}:",
			$"\t\tGC Allocated Memory    : {gcAllocated.AsDataSize()}:",
			"",
		};

		lines.Add("\tSuspended Sprite Cache Stats:");
		lines.AddRange(SuspendedSpriteCache.DumpStats().SelectF(s => $"\t{s}"));
		lines.Add("");

		ManagedTexture2D.DumpStats(lines);

		foreach (var line in lines) {
			Debug.Info(line);
		}
	}

	private void MemoryPressureLoop() {
		for (; ; ) {
			if (DrawState.TriggerGC && DrawState.TriggerGC.Wait()) {
				continue;
			}

			lock (CollectLock!) {
				try {
					using var _ = new MemoryFailPoint(Config.Garbage.RequiredFreeMemory);
				}
				catch (InsufficientMemoryException) {
					Debug.Warning($"Less than {(Config.Garbage.RequiredFreeMemory * 1024 * 1024).AsDataSize(decimals: 0)} available for block allocation, forcing full garbage collection");
					ResidentCache.Purge();
					SuspendedSpriteCache.Purge();
					DrawState.TriggerGC.Set(true);
					Thread.Sleep(10000);
				}
			}
			Thread.Sleep(512);
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
				lock (CollectLock!) {
					if (DrawState.TriggerGC && DrawState.TriggerGC.Wait()) {
						continue;
					}

					ResidentCache.Purge();
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
		Contracts.AssertNull(Self);
		Self = this;

		Garbage.EnterNonInteractive();

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

	private static string GetVersionStringHeader() => $"SpriteMaster {FullVersion} build {Config.AssemblyVersionObj.Revision} ({Config.BuildConfiguration}, {ChangeList}, {BuildComputerName})";

	private static void ConsoleTriggerGC() {
		lock (Self.CollectLock!) {
			Garbage.Collect(compact: true, blocking: true, background: false);
			DrawState.TriggerGC.Set(true);
		}
	}

	private static void ConsoleTriggerPurge() {
		lock (Self.CollectLock!) {
			Garbage.Collect(compact: true, blocking: true, background: false);
			ResidentCache.Purge();
			Garbage.Collect(compact: true, blocking: true, background: false);
			DrawState.TriggerGC.Set(true);
		}
	}

	private static readonly Dictionary<string, (Action Action, string Description)> ConsoleCommandMap = new() {
		{ "help", (() => ConsoleHelp(null), "Prints this command guide") }, 
		{ "all-stats", (DumpAllStats, "Dump Statistics") },
		{ "memory", (Debug.DumpMemory, "Dump Memory") },
		{ "gc", (ConsoleTriggerGC, "Trigger full GC") },
		{ "purge", (ConsoleTriggerPurge, "Trigger Purge") }
	};

	private static void ConsoleHelp(string? unknownCommand = null) {
		var output = new StringBuilder();
		output.AppendLine();
		output.AppendLine(GetVersionStringHeader());
		if (unknownCommand is not null) {
			output.AppendLine($"Unknown Command: '{unknownCommand}'");
		}
		output.AppendLine("Help Command Guide");
		output.AppendLine();

		int maxKeyLength = ConsoleCommandMap.Keys.Select(k => k.Length).Max();

		foreach (var kv in ConsoleCommandMap) {
			output.AppendLine($"{kv.Key.PadRight(maxKeyLength)} : {kv.Value.Description}");
		}

		Debug.Message(output.ToString());
	}

	private void ConsoleCommand(string command, string[] arguments) {
		var argumentQueue = new Queue<string>(arguments);

		if (argumentQueue.Count == 0) {
			ConsoleHelp();
			return;
		}

		var subCommand = argumentQueue.Dequeue().ToLowerInvariant();
		if (ConsoleCommandMap.TryGetValue(subCommand, out var commandPair)) {
			commandPair.Action();
		}
		else {
			ConsoleHelp(subCommand);
			return;
		}
	}

	public override void Entry(IModHelper help) {
		Debug.Message(GetVersionStringHeader());

		AssemblyPath = help.DirectoryPath;

		ConfigureHarmony();

		var ConfigPath = Path.Combine(help.DirectoryPath, ConfigName);

		using (var tempStream = new MemoryStream()) {
			SerializeConfig.Save(tempStream);

			if (!Config.IgnoreConfig) {
				SerializeConfig.Load(ConfigPath);
			}

			if (IsVersionOutdated(Config.ConfigVersion)) {
				Debug.Warning($"config.toml is out of date ({Config.ConfigVersion} < {Config.ClearConfigBefore}), rewriting it.");

				SerializeConfig.Load(tempStream, retain: true);
				Config.ConfigVersion = Config.CurrentVersion;
			}
		}

		// handle sliced textures. At some point I will add struct support.
		foreach (var slicedTexture in Config.Resample.SlicedTextures) {
			//@"LooseSprites\Cursors::0,640:2000,256"
			var elements = slicedTexture.Split("::", 2);
			var texture = elements[0];
			var bounds = Bounds.Empty;
			if (elements.Length > 1) {
				try {
					var boundElements = elements[1].Split(":");
					var offsetElements = (boundElements.ElementAtOrDefaultF(0) ?? "0,0").Split(",", 2);
					var extentElements = (boundElements.ElementAtOrDefaultF(1) ?? "0,0").Split(",", 2);

					var offset = new Vector2I(int.Parse(offsetElements[0]), int.Parse(offsetElements[1]));
					var extent = new Vector2I(int.Parse(extentElements[0]), int.Parse(extentElements[1]));

					bounds = new Bounds(offset, extent);
				}
				catch {
					Debug.Error($"Invalid SlicedTexture Bounds: '{elements[1]}'");
				}

				Config.Resample.SlicedTexturesS.Add(new(texture, bounds));
			}
		}

		// Compile blacklist patterns
		Config.Resample.BlacklistPatterns.Capacity = Config.Resample.Blacklist.Count;
		foreach (var pattern in Config.Resample.Blacklist) {
			if (!pattern.StartsWith('@')) {
				Config.Resample.BlacklistPatterns.Add(new($"^{Regex.Escape(pattern)}.*", RegexOptions.Compiled));
			}
			else {
				var reducedPattern = pattern.Substring(1);
				Config.Resample.BlacklistPatterns.Add(new(reducedPattern, RegexOptions.Compiled));
			}
		}

		if (Config.ShowIntroMessage && !Config.SkipIntro) {
			help.Events.GameLoop.GameLaunched += (_, _) => {
				Game1.drawLetterMessage("Welcome to SpriteMaster!\nSpriteMaster must resample sprites as it sees them and thus some lag will likely be apparent at the start of the game, upon entering new areas, and when new sprites are seen.\n\nPlease be patient and do not take this as an indication that your computer is incapable of running SpriteMaster.\n\nEnjoy!".Replace("\n", "^"));
			};
			Config.ShowIntroMessage = false;
		}

		SerializeConfig.Save(ConfigPath);

		help.Events.Input.ButtonPressed += OnButtonPressed;

		help.ConsoleCommands.Add("spritemaster", "SpriteMaster Commands", ConsoleCommand);
		try {
			// Try to add 'sm' as a shortcut for my own sanity.
			help.ConsoleCommands.Add("sm", "SpriteMaster Commands", ConsoleCommand);
		}
		catch { }

		help.Events.GameLoop.DayStarted += OnDayStarted;
		// GC after major events
		help.Events.GameLoop.SaveLoaded += (_, _) => {
			ForceGarbageCollect();
			Garbage.EnterInteractive();
		};
		help.Events.GameLoop.DayEnding += (_, _) => ForceGarbageCollect();
		help.Events.GameLoop.ReturnedToTitle += (_, _) => OnTitle();
		help.Events.GameLoop.SaveCreated += (_, _) => ForceGarbageCollect();
		help.Events.GameLoop.GameLaunched += (_, _) => OnGameLaunched();
		help.Events.GameLoop.SaveCreating += (_, _) => OnSaveStart();
		help.Events.GameLoop.Saving += (_, _) => OnSaveStart();
		help.Events.GameLoop.SaveCreated += (_, _) => OnSaveFinish();
		help.Events.GameLoop.Saved += (_, _) => OnSaveFinish();
		help.Events.Display.WindowResized += (_, args) => OnWindowResized(args);
		help.Events.Player.Warped += (_, _) => {
			ForceGarbageCollect();
		};
		help.Events.Specialized.LoadStageChanged += (_, args) => {
			switch (args.NewStage) {
				case LoadStage.SaveLoadedBasicInfo:
				case LoadStage.SaveLoadedLocations:
				case LoadStage.Preloaded:
				case LoadStage.ReturningToTitle:
					Garbage.EnterNonInteractive();
					break;
			}
		};
		help.Events.Display.MenuChanged += OnMenuChanged;

		MemoryPressureThread?.Start();
		GarbageCollectThread?.Start();

		static void SetSystemTarget(XNA.Graphics.RenderTarget2D? target) {
			if (target is null) {
				return;
			}
			target.Meta().IsSystemRenderTarget = true;
		}

		SetSystemTarget(Game1.lightmap);
		SetSystemTarget(Game1.game1.screen);
		SetSystemTarget(Game1.game1.uiScreen);

		// TODO : Iterate deeply with reflection over 'StardewValley' namespace to find any Texture2D objects sitting around

		// Tell SMAPI to flush all assets loaded so that SM can precache already-loaded assets
		//bool invalidated = help.Content.InvalidateCache<XNA.Graphics.Texture>();

		/*
		var light = Game1.cauldronLight;
		//Game1
		//FarmerRenderer
		//MovieTheater
		//CraftingRecipe
		//Flooring
		//HoeDirt
		//Furniture
		//Tool
		//FruitTree
		//Bush
		//titleMenu
		try {
			var texturesToCache = new List<XNA.Graphics.Texture2D>();
			var resourcesLockField = typeof(XNA.Graphics.GraphicsDevice).GetField("_resourcesLock", BindingFlags.NonPublic | BindingFlags.Instance);
			var resourcesField = typeof(XNA.Graphics.GraphicsDevice).GetField("_resources", BindingFlags.NonPublic | BindingFlags.Instance);
			var resourcesLock = resourcesLockField.GetValue(DrawState.Device);
			var resources = resourcesField.GetValue<IEnumerable<WeakReference>>(DrawState.Device);

			lock (resourcesLock) {
				foreach (var resource in resources) {
					if (resource.Target is XNA.Graphics.Texture2D texture) {
						texturesToCache.Add(texture);
					}
				}
			}

			texturesToCache = texturesToCache;
		}
		catch { }

		try {
			var texturesToCache = new List<XNA.Graphics.Texture2D>();
			var assetsField = typeof(XNA.Content.ContentManager).GetField("disposableAssets", BindingFlags.NonPublic | BindingFlags.Instance);
			var cmField = typeof(XNA.Content.ContentManager).GetField("ContentManagers", BindingFlags.NonPublic | BindingFlags.Static);
			var contentManagers = cmField.GetValue<IEnumerable<WeakReference>>(null);
			foreach (var weakRef in contentManagers) {
				if (weakRef.Target is XNA.Content.ContentManager cm) {
					var assets = assetsField.GetValue<IEnumerable<IDisposable>>(cm);
					foreach (var asset in assets) {
						if (asset is XNA.Graphics.Texture2D texture) {
							texturesToCache.Add(texture);
						}
					}
				}
			}

			texturesToCache = texturesToCache;
		}
		catch { }
		*/
		System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(FileCache).TypeHandle);
		WatchDog.WatchDog.Initialize();
		ClickCrash.Initialize();
	}

	private static class ModUID {
		internal const string DynamicGameAssets = "spacechase0.DynamicGameAssets";
		internal const string ContentPatcher = "Pathoschild.ContentPatcher";
		internal const string ContentPatcherAnimations = "spacechase0.ContentPatcherAnimations";
	}

	private void OnWindowResized(WindowResizedEventArgs args) {
		if (args.NewSize == args.OldSize) {
			return;
		}
		Snow.OnWindowResized(args.NewSize);
	}

	private void OnMenuChanged(object? _, MenuChangedEventArgs args) {
		//_ = _;
	}

	private void OnSaveStart() {
		Garbage.EnterNonInteractive();
	}

	private void OnSaveFinish() {
		Garbage.EnterInteractive();
	}

	private void OnTitle() {
		ForceGarbageCollect();
		Garbage.EnterInteractive();
	}

	internal void OnFirstDraw() {
		Garbage.EnterInteractive();
	}

	private const string UnderTestingMessage = "which is still in testing under SpriteMaster - results may vary";
	private static readonly (string UID, string Name, string Message)[] WarnFrameworks = new (string UID, string Name, string Message)[] {
		(ModUID.ContentPatcherAnimations, "Content Patcher Animations", UnderTestingMessage),
		(ModUID.DynamicGameAssets, "Dynamic Game Assets", UnderTestingMessage),
	};

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	private void CheckMods() {
		var frameworkedMods = new Dictionary<string, List<IModInfo>>();

		foreach (var mod in Helper.ModRegistry.GetAll()) {
			if (mod is null) {
				continue;
			}
			var manifest = mod.Manifest;

			if (manifest is null) {
				continue;
			}

			foreach (var framework in WarnFrameworks) {
				if (manifest.Dependencies.AnyF(d => d.UniqueID == framework.UID) || manifest.ContentPackFor?.UniqueID == framework.UID) {
					if (!frameworkedMods.TryGetValue(framework.UID, out var list)) {
						list = new();
						frameworkedMods.Add(framework.UID, list);
					}
					list.Add(mod);
					break;
				}
			}
		}

		foreach (var modsPair in frameworkedMods) {
			if (modsPair.Value.Count == 0) {
				continue;
			}

			var framework = WarnFrameworks.FirstF(framework => framework.UID == modsPair.Key);

			var sb = new StringBuilder();
			sb.AppendLine($"The following mods have a dependency on {framework.Name} ({framework.UID}), {framework.Message}:");

			foreach (var mod in modsPair.Value) {
				sb.AppendLine($"\t{mod.Manifest.Name} ({mod.Manifest.UniqueID})");
			}

			Debug.Warning(sb.ToString());
		}
	}

	private void OnGameLaunched() {
		using var modCheckTask = Task.Run(CheckMods);

		Debug.Trace("Waiting for FileCache to initialize...");
		FileCache.Initialized.Wait();

		modCheckTask.Wait();

		ForceGarbageCollect();
		ManagedSpriteInstance.ClearTimers();

		IsGameLoaded = true;
	}

	// SMAPI/CP won't do this, so we do. Purge the cached textures for the previous season on a season change.
	private static void OnDayStarted(object? _, DayStartedEventArgs _1) {
		Harmonize.Patches.Game.Snow.PopulateDebrisWeatherArray();

		// Do a full GC at the start of each day
		Garbage.Collect(compact: true, blocking: true, background: false);

		var season = SDate.Now().Season;
		if (!season.EqualsInvariantInsensitive(CurrentSeason)) {
			CurrentSeason = season;
			SpriteMap.SeasonPurge(season.ToLowerInvariant());

			// And again after purge
			Garbage.Collect(compact: true, blocking: true, background: false);
		}
	}

	private static void ForceGarbageCollect() {
		Garbage.Collect(compact: true, blocking: true, background: false);
	}

	private static void ForceGarbageCollectConcurrent() {
		Garbage.Collect(compact: false, blocking: false, background: false);
	}

	private void ConfigureHarmony() {
		var instance = new Harmony(ModManifest.UniqueID);
		instance.ApplyPatches();
	}

	private static void OnButtonPressed(object? _, ButtonPressedEventArgs args) {
		if (args.Button == Config.ToggleButton) {
			Config.Enabled = !Config.Enabled;
		}
	}
}
