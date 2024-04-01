/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

global using MMConfig = MusicMaster.Configuration.Config;
using LinqFasterer;
using StardewModdingAPI;
using System;
using System.IO;
// ReSharper disable MemberHidesStaticFromOuterClass

namespace MusicMaster.Configuration;

internal static class Config {
	internal static readonly string ModuleName =
		typeof(MMConfig).Namespace?.Split('.').ElementAtOrDefaultF(0) ?? "MusicMaster";

	[Attributes.Ignore] internal static string Path { get; private set; } = null!;

	[Attributes.Ignore] internal static MemoryStream? DefaultConfig = null;

	internal static void SetPath(string path) => Path = path;

	internal delegate void OnConfigChangedDelegate();

	[Attributes.Ignore]
	internal static event OnConfigChangedDelegate? ConfigChanged;

	internal static void OnConfigChanged() {
		ConfigChanged?.Invoke();
	}

	internal const bool IgnoreConfig = false ||
#if DEBUG
		true;
#else
		false;
#endif
	internal const bool SkipIntro = IgnoreConfig;

	private enum BuildType {
		Alpha,
		Beta,
		Candidate,
		Final
	}

	[Attributes.Ignore]
	private static string GenerateAssemblyVersionString(
		int major, int minor, int revision, int build, BuildType type = BuildType.Final, int release = 0
	) {
		switch (type) {
			case BuildType.Alpha:
				break;
			case BuildType.Beta:
				release += 100;
				break;
			case BuildType.Candidate:
				release += 200;
				break;
			case BuildType.Final:
				release += 300;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type.ToString());
		}

		return $"{major}.{minor}.{revision}.{build + release}";
	}

	[Attributes.GMCMHidden] internal static string ConfigVersion = "";

	[Attributes.Ignore]
	internal static string ClearConfigBefore = GenerateAssemblyVersionString(0, 15, 0, 0, BuildType.Beta, 10);

	[Attributes.Ignore] internal static bool ForcedDisable = false;

	[Attributes.Ignore] internal static bool ToggledEnable = true;

	internal const bool Enabled = true;

	[Attributes.Ignore]
#pragma warning disable CS0618 // Type or member is obsolete
	internal static bool IsUnconditionallyEnabled => !ForcedDisable && Enabled;

	internal static bool IsEnabled => ToggledEnable && IsUnconditionallyEnabled;
#pragma warning restore CS0618 // Type or member is obsolete

	internal enum Configuration {
		Debug,
		Development,
		Release
	}

	internal const Configuration BuildConfiguration =
#if DEVELOPMENT
			Configuration.Development;
#elif DEBUG
			Configuration.Debug;
#else
		Configuration.Release;
#endif

	internal const bool IsDebug = BuildConfiguration == Configuration.Debug;
	internal const bool IsDevelopment = BuildConfiguration == Configuration.Development;
	internal const bool IsRelease = BuildConfiguration == Configuration.Release;

	[Attributes.Ignore]
	internal static readonly string LocalRootDefault = System.IO.Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
		"StardewValley",
		"Mods",
		ModuleName
	);

	[Attributes.Advanced]
	internal static class Debug {
		internal static class Logging {
			internal static LogLevel LogLevel = LogLevel.Trace;
		}
	}
}
