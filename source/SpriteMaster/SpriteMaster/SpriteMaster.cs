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
using SpriteMaster.Configuration;
using SpriteMaster.Configuration.GMCM;
using SpriteMaster.Experimental;
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpriteMaster;

public sealed class SpriteMaster : Mod {
	internal static SpriteMaster Self { get; private set; } = default!;

	internal readonly MemoryMonitor MemoryMonitor;
	internal static string? AssemblyPath { get; private set; }

	private static T? GetAssemblyAttribute<T>() where T : Attribute => typeof(SpriteMaster).Assembly.GetCustomAttribute<T>();

	internal static readonly string ChangeList = GetAssemblyAttribute<ChangeListAttribute>()?.Value ?? "local";
	internal static readonly string BuildComputerName = GetAssemblyAttribute<BuildComputerNameAttribute>()?.Value ?? "unknown";
	internal static readonly string FullVersion = GetAssemblyAttribute<FullVersionAttribute>()?.Value ?? Config.CurrentVersion;

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

	private const string ConfigName = "config.toml";

	public SpriteMaster() {
		Contracts.AssertNull(Self);
		Self = this;

		Garbage.EnterNonInteractive();

		MemoryMonitor = new MemoryMonitor();

		var assemblyPath = typeof(SpriteMaster).Assembly.Location;
		assemblyPath = Path.GetDirectoryName(assemblyPath);

		// Compress our own directory
		if (assemblyPath is not null) {
			DirectoryExt.CompressDirectory(assemblyPath, force: true);
		}
	}

	private bool IsVersionOutdated(string configVersion) {
		string referenceVersion = Config.ClearConfigBefore;

		var configStrArray = configVersion.Split('.');
		var referenceStrArray = referenceVersion.Split('.');

		try {
			int maxLen = Math.Max(configStrArray.Length, referenceStrArray.Length);
			for (int i = 0; i < maxLen; ++i) {
				if (configStrArray.Length <= i || configStrArray[i].IsEmpty()) {
					return true;
				}
				if (referenceStrArray.Length <= i || referenceStrArray[i].IsEmpty()) {
					return false;
				}

				var configElement = int.Parse(configStrArray[i]);
				var referenceElement = int.Parse(referenceStrArray[i]);

				if (configElement > referenceElement) {
					return false;
				}

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
		Self.MemoryMonitor.TriggerGC();
	}

	private static void ConsoleTriggerPurge() {
		Self.MemoryMonitor.TriggerPurge();
	}

	private static readonly Dictionary<string, (Action<string, Queue<string>> Action, string Description)> ConsoleCommandMap = new() {
		{ "help", ((_, _) => ConsoleHelp(null), "Prints this command guide") },
		{ "all-stats", ((_, _) => DumpAllStats(), "Dump Statistics") },
		{ "memory", ((_, _) => Debug.DumpMemory(), "Dump Memory") },
		{ "gc", ((_, _) => ConsoleTriggerGC(), "Trigger full GC") },
		{ "purge", ((_, _) => ConsoleTriggerPurge(), "Trigger Purge") }
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

		int maxKeyLength = ConsoleCommandMap.Keys.Max(k => k.Length);

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
			commandPair.Action(subCommand, argumentQueue);
		}
		else {
			ConsoleHelp(subCommand);
			return;
		}
	}

	private void InitConsoleCommands() {
		foreach (var type in typeof(SpriteMaster).Assembly.GetTypes()) {
			foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public)) {
				var command = method.GetCustomAttribute<CommandAttribute>();
				if (command is not null) {
					var parameters = method.GetParameters();
					if (parameters.Length != 2) {
						Debug.Error($"Console command '{command.Name}' for method '{method.GetFullName()}' does not have the expected number of parameters");
						continue;
					}
					if (parameters[0].ParameterType != typeof(string)) {
						Debug.Error($"Console command '{command.Name}' for method '{method.GetFullName()}' : parameter 0 type {parameters[0].ParameterType} is not {typeof(string)}");
						continue;
					}
					if (parameters[1].ParameterType != typeof(Queue<string>)) {
						Debug.Error($"Console command '{command.Name}' for method '{method.GetFullName()}' : parameter 1 type {parameters[1].ParameterType} is not {typeof(Queue<string>)}");
						continue;
					}

					if (ConsoleCommandMap.ContainsKey(command.Name)) {
						Debug.Error($"Console command is already registered: '{command.Name}'");
						continue;
					}

					ConsoleCommandMap.Add(command.Name, (method.CreateDelegate<Action<string, Queue<string>>>(), command.Description));
				}
			}
		}
	}

	public override void Entry(IModHelper help) {
		Debug.Message(GetVersionStringHeader());

		AssemblyPath = help.DirectoryPath;

		ConfigureHarmony();

		Configuration.Config.SetPath(Path.Combine(help.DirectoryPath, ConfigName));

		Config.DefaultConfig = new MemoryStream();
		Serialize.Save(Config.DefaultConfig, leaveOpen: true);

		if (!Config.IgnoreConfig) {
			Serialize.Load(Configuration.Config.Path);
		}

		if (IsVersionOutdated(Config.ConfigVersion)) {
			Debug.Info($"config.toml is out of date ({Config.ConfigVersion} < {Config.ClearConfigBefore}), rewriting it.");

			Serialize.Load(Config.DefaultConfig, retain: true);
			Config.DefaultConfig.Position = 0;
			Config.ConfigVersion = Config.CurrentVersion;
		}

		static Config.TextureRef[] ProcessTextureRefs(List<string> textureRefStrings) {
			// handle sliced textures. At some point I will add struct support.
			var result = new Config.TextureRef[textureRefStrings.Count];
			for (int i = 0; i < result.Length; ++i) {
				var slicedTexture = textureRefStrings[i];
				//@"LooseSprites\Cursors::0,640:2000,256"
				var elements = slicedTexture.Split("::", 2);
				var texture = elements[0]!;
				var bounds = Bounds.Empty;
				if (elements.Length > 1) {
					try {
						var boundElements = elements[1].Split(':');
						var offsetElements = (boundElements.ElementAtOrDefaultF(0) ?? "0,0").Split(',', 2);
						var extentElements = (boundElements.ElementAtOrDefaultF(1) ?? "0,0").Split(',', 2);

						var offset = new Vector2I(int.Parse(offsetElements[0]), int.Parse(offsetElements[1]));
						var extent = new Vector2I(int.Parse(extentElements[0]), int.Parse(extentElements[1]));

						bounds = new Bounds(offset, extent);
					}
					catch {
						Debug.Error($"Invalid SlicedTexture Bounds: '{elements[1]}'");
					}
				}
				result[i] = new(string.Intern(texture), bounds);
			}
			return result;
		}

		Config.Resample.SlicedTexturesS = ProcessTextureRefs(Config.Resample.SlicedTextures);
		Config.Resample.Padding.BlackListS = ProcessTextureRefs(Config.Resample.Padding.BlackList);

		// Compile blacklist patterns
		static Regex[] ProcessTexturePatterns(List<string> texturePatternStrings) {
			var result = new Regex[texturePatternStrings.Count];
			for (int i = 0; i < texturePatternStrings.Count; ++i) {
				var pattern = texturePatternStrings[i];
				if (!pattern.StartsWith('@')) {
					pattern = $"^{Regex.Escape(pattern)}.*";
				}
				else {
					pattern = pattern.Substring(1);
				}
				result[i] = new(pattern, RegexOptions.Compiled);
			}
			return result;
		}


		Config.Resample.BlacklistPatterns = ProcessTexturePatterns(Config.Resample.Blacklist);
		Config.Resample.GradientBlacklistPatterns = ProcessTexturePatterns(Config.Resample.GradientBlacklist);

		/*
		if (Config.ShowIntroMessage && !Config.SkipIntro) {
			help.Events.GameLoop.GameLaunched += (_, _) => {
				Game1.drawLetterMessage("Welcome to SpriteMaster!\nSpriteMaster must resample sprites as it sees them and thus some lag will likely be apparent at the start of the game, upon entering new areas, and when new sprites are seen.\n\nPlease be patient and do not take this as an indication that your computer is incapable of running SpriteMaster.\n\nEnjoy!".Replace("\n", "^"));
			};
			Config.ShowIntroMessage = false;
		}
		*/

		if (Config.ShowIntroMessage && !Config.SkipIntro) {
			help.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			Config.ShowIntroMessage = false;
		}

		help.Events.GameLoop.GameLaunched += (_, _) => Setup.Initialize(help);

		Serialize.Save(Configuration.Config.Path);

		help.Events.Input.ButtonPressed += OnButtonPressed;

		try {
			help.ConsoleCommands.Add("spritemaster", "SpriteMaster Commands", ConsoleCommand);
		}
		catch (Exception ex) {
			Debug.Warning("Could not register 'spritemaster' for console commands", ex);
		}
		try {
			// Try to add 'sm' as a shortcut for my own sanity.
			help.ConsoleCommands.Add("sm", "SpriteMaster Commands", ConsoleCommand);
		}
		catch (Exception ex) {
			Debug.Warning("Could not register 'sm' for console commands", ex);
		}

		InitConsoleCommands();

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
			ForceGarbageCollectConcurrent();
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

		MemoryMonitor.Start();

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

	private void OnUpdateTicked(object? sender, UpdateTickedEventArgs args) {
		if (!Config.ShowIntroMessage) {
			return;
		}

		if (Game1.ticks <= 1) {
			return;
		}

		Configuration.GMCM.Setup.ForceOpen();

		Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
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

			Debug.Info(sb.ToString());
		}
	}

	private readonly struct WaitWrapper : IDisposable {
		private readonly object Waiter;

		internal WaitWrapper(object waiter) => Waiter = waiter;

		public readonly void Dispose() {
			if (Waiter is IDisposable disposable) {
				disposable.Dispose();
			}
		}

		internal readonly void Wait() {
			switch (Waiter) {
				case Task task:
					task.Wait();
					break;
				case ManualCondition condition:
					condition.Wait();
					break;
				default:
					throw new InvalidOperationException(Waiter.GetType().Name);
			}
		}
	}

	private void OnGameLaunched() {
		var waiters = new WaitWrapper[] {
			new(Task.Run(CheckMods)),
			new(Task.Run(Inlining.Reenable)),
			new(FileCache.Initialized)
		};

		foreach (var waiter in waiters) {
			waiter.Wait();
			waiter.Dispose();
		}

		ForceGarbageCollect();
		ManagedSpriteInstance.ClearTimers();
	}

	// SMAPI/CP won't do this, so we do. Purge the cached textures for the previous season on a season change.
	private static void OnDayStarted(object? _, DayStartedEventArgs _1) {
		Harmonize.Patches.Game.Snow.PopulateDebrisWeatherArray();

		// Do a full GC at the start of each day
		Garbage.Collect(compact: true, blocking: true, background: false);

		var season = Game1.currentSeason;
		if (!season.EqualsInvariantInsensitive(GameState.CurrentSeason)) {
			GameState.CurrentSeason = season;
			SpriteMap.SeasonPurge(season.ToLowerInvariant());

			// And again after purge
			Garbage.Collect(compact: true, blocking: true, background: false);
		}
	}

	private static void ForceGarbageCollect() {
		Garbage.Collect(compact: true, blocking: true, background: false);
	}

	private static void ForceGarbageCollectConcurrent() {
		Garbage.Collect(compact: false, blocking: false, background: true);
	}

	private void ConfigureHarmony() {
		var instance = new Harmony(ModManifest.UniqueID);
		instance.ApplyPatches();
	}

	private static void OnButtonPressed(object? _, ButtonPressedEventArgs args) {
		if (args.Button == Config.ToggleButton) {
			Config.ToggledEnable = !Config.ToggledEnable;
		}
	}
}
