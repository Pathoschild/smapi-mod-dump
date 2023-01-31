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
using JetBrains.Annotations;
using MusicMaster.Configuration;
using MusicMaster.Extensions;
using MusicMaster.Harmonize;
using MusicMaster.Types;
using StardewModdingAPI;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MusicMaster;

public sealed class MusicMaster : Mod {
	internal static Assembly Assembly => typeof(MusicMaster).Assembly;
	private const string UniqueId = "DigitalCarbide.MusicMaster";
	private static string ModDirectory => Self?.Helper?.DirectoryPath ?? Path.GetDirectoryName(Assembly.Location) ?? Assembly.Location;

	internal static MusicMaster Self { get; private set; } = default!;
	private const string ConfigName = "config.toml";

	private readonly Lazy<Harmony> HarmonyInstance = new(() => new(UniqueId));

	[UsedImplicitly]
	public MusicMaster() {
		Self.AssertNull();
		Self = this;

		_ = ThreadingExt.IsMainThread;

		DirectoryCleanup.Cleanup();

		Initialize();
	}

	private void InitializeConfig() {
		Config.SetPath(Path.Combine(ModDirectory, ConfigName));

		Config.DefaultConfig = new MemoryStream();
		Serialize.Save(Config.DefaultConfig, leaveOpen: true);

		if (!Config.IgnoreConfig) {
			Serialize.Load(Config.Path);
		}

		if (Versioning.IsOutdated(Config.ConfigVersion)) {
			Debug.Info($"config.toml is out of date ({Config.ConfigVersion} < {Config.ClearConfigBefore}), rewriting it.");

			Serialize.Load(Config.DefaultConfig, retain: true);
			Config.DefaultConfig.Position = 0;
			Config.ConfigVersion = Versioning.CurrentVersion;
		}
	}

	private bool TryAddConsoleCommand(string name, string documentation, Action<string, string[]> callback) {
		try {
			Helper.ConsoleCommands.Add(name, documentation, callback);
			return true;
		}
		catch (Exception ex)
		{
			Debug.Warning($"Could not register '{name}' for console commands", ex);
			return false;
		}
	}

	private void InitializeEvents() {
		var gameLoop = Helper.Events.GameLoop;

		gameLoop.DayEnding += (_, _) => ForceGarbageCollect();
		gameLoop.GameLaunched += (_, _) => OnGameLaunched();
	}

	private bool Initialized = false;
	private void Initialize() {
		if (Initialized) {
			ConfigureHarmony(early: false);
			return;
		}

		try {
			Debug.Message(Versioning.StringHeader);

			ConfigureHarmony(early: true);

			InitializeConfig();

			Initialized = true;
		}
		catch {
			// Swallow Exceptions
		}
	}

	[UsedImplicitly]
	public override void Entry(IModHelper help) {
#if !SHIPPING
		ModManifest.UniqueID.AssertEqual(UniqueId);
#endif

		Runtime.CorrectProcessorAffinity();

		Initialize();

		Serialize.Save(Config.Path);

		foreach (var prefix in new[] { "musicmaster", "mm" }) {
			_ = TryAddConsoleCommand(prefix, "MusicMaster Commands", ConsoleSupport.Invoke);
		}

		InitializeEvents();

		MusicManager.Initialize(this, help);
	}
	
	[StructLayout(LayoutKind.Auto)]
	private readonly struct WaitWrapper : IDisposable {
		private readonly object Waiter;

		internal WaitWrapper(object waiter) => Waiter = waiter;

		public void Dispose() {
			if (Waiter is IDisposable disposable) {
				disposable.Dispose();
			}
		}

		internal void Wait() {
			switch (Waiter) {
				case Task task:
					task.Wait();
					break;
				case ManualCondition condition:
					condition.Wait();
					break;
				default:
					ThrowHelper.ThrowInvalidOperationException(Waiter.GetType().Name);
					break;
			}
		}
	}

	private void OnGameLaunched() {
		var waiters = new WaitWrapper[] {
			new(Task.Run(Configuration.ConfigMenu.Setup.Initialize))
		};

		foreach (var waiter in waiters) {
			waiter.Wait();
			waiter.Dispose();
		}

		ForceGarbageCollect();
	}

	private static void ForceGarbageCollect() {
		Garbage.Collect(compact: true, blocking: true, background: false);
	}

	private static void ForceGarbageCollectConcurrent() {
		Garbage.Collect(compact: false, blocking: false, background: true);
	}

	private void ConfigureHarmony(bool early) {
		bool wasInitialized = HarmonyInstance.IsValueCreated;

		var instance = HarmonyInstance.Value;

		// If early initialization hadn't already occurred, do it now.
		if (!early && !wasInitialized) {
			instance.ApplyPatches(early: true);
		}

		instance.ApplyPatches(early);
	}
}
