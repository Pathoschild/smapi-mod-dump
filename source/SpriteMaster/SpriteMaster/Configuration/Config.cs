/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

global using SMConfig = SpriteMaster.Configuration.Config;
using LinqFasterer;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Resample;
using SpriteMaster.Types;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime;
using System.Text.RegularExpressions;

using Root = SpriteMaster;
// ReSharper disable MemberHidesStaticFromOuterClass

namespace SpriteMaster.Configuration;

internal static class Config {
	internal static readonly string ModuleName =
		typeof(SMConfig).Namespace?.Split('.').ElementAtOrDefaultF(0) ?? "SpriteMaster";

	[Attributes.Ignore] internal static string Path { get; private set; } = null!;

	[Attributes.Ignore] internal static MemoryStream? DefaultConfig = null;

	internal static void SetPath(string path) => Path = path;

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
	internal static string ClearConfigBefore = GenerateAssemblyVersionString(0, 15, 0, 0, BuildType.Beta, 4);

	[Attributes.Ignore] internal static bool ForcedDisable = false;

	[Attributes.Ignore] internal static bool ToggledEnable = true;

	[Attributes.Comment("Should SpriteMaster be enabled? Unsetting this will disable _all_ SpriteMaster functionality.")]
	[Attributes.MenuName("Enable SpriteMaster")]
	[Obsolete($"Use {nameof(IsEnabled)}")]
	internal static bool Enabled = true;

	[Attributes.Ignore]
#pragma warning disable CS0618 // Type or member is obsolete
	internal static bool IsUnconditionallyEnabled => !ForcedDisable && (Preview.Override.Instance?.Enabled ?? Enabled);

	internal static bool IsEnabled => ToggledEnable && IsUnconditionallyEnabled;
#pragma warning restore CS0618 // Type or member is obsolete

	[Attributes.Comment("Button to toggle SpriteMaster")]
	internal static SButton ToggleButton = SButton.F11;

	[Attributes.Ignore]
	internal static int
		ClampDimension =
			BaseMaxTextureDimension; // this is adjustable by the system itself. The user shouldn't be able to touch it.

	internal const int AbsoluteMaxTextureDimension = 16384;
	internal const int BaseMaxTextureDimension = 4096;

	[Attributes.Comment("The preferred maximum texture edge length, if allowed by the hardware")]
	[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
	[Attributes.LimitsInt(min: 1, max: AbsoluteMaxTextureDimension)]
	[Attributes.Advanced]
	internal static int PreferredMaxTextureDimension = 16384;

	internal const bool ClampInvalidBounds = true;

	[Attributes.Retain] [Attributes.GMCMHidden]
	internal static bool ShowIntroMessage = true;

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

	internal const bool DumpTextures = !IsRelease ||
#if DUMP_TEXTURES
		true
#else
		false
#endif
		;

[Attributes.Ignore]
	internal static readonly string LocalRootDefault = System.IO.Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
		"StardewValley",
		"Mods",
		ModuleName
	);
	internal static string LocalRoot => DataStoreOverride.Length == 0 ? LocalRootDefault : DataStoreOverride;
	[Attributes.Comment("If the data cache is preferred to be elsewhere, it can be set here")]
	[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushFileCache)]
	[Attributes.GMCMHidden]
	internal static string DataStoreOverride = "";

	[Attributes.GMCMHidden]
	[Attributes.Advanced]
	internal static class WatchDog {
		[Attributes.Comment("Should the watchdog be enabled?")]
		internal static bool Enabled = false;
		[Attributes.Comment("What should the default sleep interval be (in milliseconds)?")]
		internal static int DefaultSleepInterval = 5_000;
		[Attributes.Comment("What should the short sleep interval be (in milliseconds)?")]
		internal static int ShortSleepInterval = 500;
		[Attributes.Comment("What should the interrupt interval be (in milliseconds)?")]
		internal static int InterruptInterval = 10_000;
	}

	[Attributes.Advanced]
	internal static class Garbage {
		[Attributes.Comment("Should unowned textures be marked in the garbage collector's statistics?")]
		[Attributes.Advanced]
		internal static bool CollectAccountUnownedTextures = false;
		[Attributes.Comment("Should owned textures be marked in the garbage collector's statistics?")]
		[Attributes.Advanced]
		[Obsolete($"Use {nameof(ShouldCollectAccountOwnedTextures)}")]
		internal static bool? CollectAccountOwnedTextures = null;

#pragma warning disable CS0618
		internal static bool ShouldCollectAccountOwnedTextures = CollectAccountOwnedTextures ?? SystemInfo.Graphics.IsIntegrated;
#pragma warning restore CS0618

		[Attributes.Comment("The amount of free memory required by SM after which it triggers hard recovery operations")]
		[Attributes.LimitsInt(1L, int.MaxValue * (long)SizesExt.MiB)]
		[Attributes.Advanced]
		internal static long RequiredFreeMemoryHard = SizesExt.AsMiB(128L);
		[Attributes.Comment("The amount of free memory required by SM after which it triggers soft recovery operations")]
		[Attributes.LimitsInt(1L, int.MaxValue * (long)SizesExt.MiB)]
		[Attributes.Advanced]
		internal static long RequiredFreeMemorySoft = SizesExt.AsGiB(1L);
		[Attributes.Comment("Hysteresis applied to RequiredFreeMemory")]
		[Attributes.LimitsReal(1.01, 10.0)]
		[Attributes.Advanced]
		internal static double RequiredFreeMemoryHysteresis = 1.5;
		[Attributes.Comment("Should sprites containing season names be purged on a seasonal basis?")]
		internal static bool SeasonalPurge = true;
		[Attributes.Comment("What runtime garbage collection latency mode should be set?")]
		internal static GCLatencyMode LatencyMode = GCLatencyMode.SustainedLowLatency;
		[Attributes.Comment("Perform an ephemeral (Generation 0 and 1) garbage collection pass every N time periods (if <= 0, disabled)")]
		[Attributes.LimitsTimeSpan(0L, 12_000L * TimeSpan.TicksPerMillisecond)]
		internal static TimeSpan EphemeralCollectPeriod = TimeSpan.FromMilliseconds(6_000);
		[Attributes.Comment("What ephemeral collection pause period goal should be used")]
		[Attributes.LimitsTimeSpan(500L * TimeSpan.TicksPerMillisecond, 2000L * TimeSpan.TicksPerMillisecond)]
		internal static TimeSpan EphemeralCollectPauseGoal = TimeSpan.FromTicks(500L * TimeSpan.TicksPerMillisecond);
	}

	[Attributes.Advanced]
	internal static class Debug {
		internal static class Logging {
			internal static LogLevel LogLevel = LogLevel.Trace;
#if (!SHIPPING && !RELEASE) || LOG_MONITOR
			internal static bool SilenceOtherMods = true;
			internal static string[] SilencedMods = new[] {
				"Farm Type Manager",
				"Quest Framework",
				"AntiSocial NPCs",
				"SMAPI",
				"Json Assets",
				"Content Patcher",
				"Free Love",
				"Mail Framework Mod",
				"Shop Tile Framework",
				"Custom Companions",
				"Farmer Helper",
				"Wind Effects",
				"Multiple Spouse Dialogs"
			};
#endif
		}

		internal static class Sprite {
			internal const bool DumpReference = DumpTextures;
			internal const bool DumpResample = DumpTextures;
		}
	}

	[Attributes.Advanced]
	internal static class DrawState {
		[Attributes.Comment("Enable linear sampling for sprites")]
		[Obsolete($"Use {nameof(IsSetLinear)}")]
		internal static bool SetLinear = true;

		[Attributes.Ignore]
#pragma warning disable CS0618 // Type or member is obsolete
		internal static bool IsSetLinear => Preview.Override.Instance?.SetLinear ?? SetLinear;
#pragma warning restore CS0618 // Type or member is obsolete

		[Attributes.Comment("Enable linear sampling for sprites")]
		[Obsolete($"Use {nameof(IsSetLinearUnresampled)}")]
		internal static bool SetLinearUnresampled = false;

		[Attributes.Ignore]
#pragma warning disable CS0618 // Type or member is obsolete
		internal static bool IsSetLinearUnresampled => (Preview.Override.Instance?.SetLinearUnresampled ?? SetLinearUnresampled && Resample.IsEnabled);
#pragma warning restore CS0618 // Type or member is obsolete

		[Attributes.Comment("How many MSAA samples should be used?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.ResetDisplay | Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.LimitsInt(1, 16)]
		internal static int AntialiasingSamples = 1;
		[Attributes.Comment("Disable the depth buffer (unused in this game)")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.ResetDisplay)]
		[Attributes.Advanced]
		internal static bool DisableDepthBuffer = false;
		[Attributes.Comment("The default backbuffer format to request")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.ResetDisplay)]
		internal static SurfaceFormat BackbufferFormat = SurfaceFormat.Color;
		[Attributes.Comment("The default HDR backbuffer format to request")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.ResetDisplay)]
		internal static SurfaceFormat BackbufferHDRFormat = SurfaceFormat.Rgba64;
		[Attributes.Comment("Should the system HDR settings be honored?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.ResetDisplay)]
		internal static bool HonorHDRSettings = true;
	}

	internal readonly record struct TextureRef(string Texture, Bounds Bounds);

	internal static class Resample {
		[Attributes.Ignore]
		internal static bool ToggledEnable = true;

		[Attributes.Comment("Should resampling be enabled?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.MenuName("Enable Resampling")]
		[Obsolete($"Use {nameof(IsEnabled)}")]
		internal static bool Enabled = true;

		[Attributes.Ignore]
#pragma warning disable CS0618 // Type or member is obsolete
		internal static bool IsEnabled => Preview.Override.Instance?.ResampleEnabled ?? (Enabled && ToggledEnable);
#pragma warning restore CS0618 // Type or member is obsolete

		[Attributes.Comment("Should resampling be enabled for normal sprites?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		internal static bool EnabledSprites = true;
		[Attributes.Comment("Should resampling be enabled for regular text?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		internal static bool EnabledText = true;
		[Attributes.Comment("Should resampling be enabled for 'basic' text?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		internal static bool EnabledBasicText = true;
		[Attributes.Comment("Should the texture be scale-adjusted if its scaled dimensions are outside preferred dimensional limits?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.Advanced]
		internal static bool Scale = true;
		[Attributes.Comment("What scaling algorithm should be used by default?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		internal static Root.Resample.Scaler Scaler = Root.Resample.Scaler.xBRZ;
		[Attributes.Comment("What scaling algorithm should be used for gradient sprites?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		internal static Root.Resample.Scaler ScalerGradient = Root.Resample.Scaler.None;
		[Attributes.Comment("Should dynamic scaling be used (scaling based upon apparent sprite size)")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.Advanced]
		internal static bool EnableDynamicScale = true;

		[Attributes.Comment("Should excess transparent rows/colums be trimmed?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.Advanced]
		internal static bool TrimExcessTransparency = true;
		[Attributes.Comment("Should we assume that input sprites are gamma corrected?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.Advanced]
		internal static bool AssumeGammaCorrected = true;
		[Attributes.Comment("Maximum scale factor of sprites (clamped to chosen scaler)")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.LimitsInt(1, 6)]
		internal static int MaxScale = 6;
		[Attributes.Comment("Minimum edge length of a sprite to be considered for resampling")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.LimitsInt(1, AbsoluteMaxTextureDimension)]
		[Attributes.Advanced]
		internal static int MinimumTextureDimensions = 1;
		[Attributes.Comment("Should wrapped addressing be enabled for sprite resampling (when analysis suggests it)?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.Advanced]
		internal static bool EnableWrappedAddressing = false;
		[Attributes.Comment("Should resampling be stalled if it is determined that it will cause hitches?")]
		[Attributes.Advanced]
		internal static bool UseFrametimeStalling = true;
		[Attributes.Comment("Should color enhancement/rebalancing be performed?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.Advanced]
		internal static bool UseColorEnhancement = true;
		[Attributes.Comment("Should transparent pixels be premultiplied to prevent a 'halo' effect?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.Advanced]
		internal static bool PremultiplyAlpha = true;
		[Attributes.Comment("Low pass value that should be filtered when reversing premultiplied alpha.")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.LimitsInt(ushort.MinValue, ushort.MaxValue)]
		[Attributes.Advanced]
		internal static ushort PremultiplicationLowPass = 1023;
		[Attributes.Comment("Use redmean algorithm for perceptual color comparisons?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.Advanced]
		internal static bool UseRedmean = false;
		[Attributes.Comment("What textures are drawn in 'slices' and thus should be special-cased to be resampled as one texture?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.GMCMHidden]
		internal static List<string> SlicedTextures = new() {
			@"LooseSprites\Cursors::0,2000:640,256",
			@"Maps\Mines\volcano_dungeon::0,320:160,64",
			@"LooseSprites\Cloudy_Ocean_BG",
			@"LooseSprites\Cloudy_Ocean_BG_Night",
			@"LooseSprites\stardewPanorama",
			@"Maps\nightSceneMaru",
			@"Maps\nightSceneMaruTrees",
			@"Maps\sebastianMountainTiles",
			@"Maps\sebastianRideTiles",
			// SVE
			@"Tilesheets\GuntherExpedition2_Shadows",
			@"Tilesheets\Highlands_Fog",
			@"Tilesheets\Highlands_FogBackground",

		};
		[Attributes.Ignore]
		internal static TextureRef[] SlicedTexturesS = Array.Empty<TextureRef>();
		[Attributes.Advanced]
		internal static class BlockMultipleAnalysis {
			[Attributes.Comment("Should sprites be analyzed to see if they are block multiples?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal static bool Enabled = true;
			[Attributes.Comment("What threshold should be used for block multiple analysis?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsInt(0, 255)]
			internal static int EqualityThreshold = 1;
			[Attributes.Comment("How many blocks can be different for the test to still pass?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsInt(1, int.MaxValue)]
			internal static int MaxInequality = 1;
		}

		[Attributes.Comment("What textures or spritesheets use 4xblock sizes?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.GMCMHidden]
		internal static List<string> TwoXTextures = new() {
			@"Maps\WoodBuildings" // is _almost_ TwoX.
		};
		[Attributes.Comment("What textures or spritesheets use 4xblock sizes?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.GMCMHidden]
		internal static List<string> FourXTextures = new() {
			@"Characters\Monsters\Crow",
			@"Characters\femaleRival",
			@"Characters\maleRival",
			@"LooseSprites\Bat",
			@"LooseSprites\buildingPlacementTiles",
			@"LooseSprites\chatBox",
			@"LooseSprites\daybg",
			@"LooseSprites\DialogBoxGreen",
			@"LooseSprites\hoverBox",
			@"LooseSprites\nightbg",
			@"LooseSprites\robinAtWork",
			@"LooseSprites\skillTitles",
			@"LooseSprites\textBox",
			@"Maps\busPeople",
			@"Maps\cavedarker",
			@"Maps\FarmhouseTiles",
			@"Maps\GreenHouseInterior",
			@"Maps\MenuTiles",
			@"Maps\MenuTilesUncolored",
			@"Maps\spring_BusStop",
			@"Maps\TownIndoors",
			@"TerrainFeatures\BuffsIcons",
			@"TerrainFeatures\DiggableWall_basic",
			@"TerrainFeatures\DiggableWall_basic_dark",
			@"TerrainFeatures\DiggableWall_frost",
			@"TerrainFeatures\DiggableWall_frost_dark",
			@"TerrainFeatures\DiggableWall_lava",
			@"TerrainFeatures\DiggableWall_lava_dark",
			@"TerrainFeatures\Stalagmite",
			@"TerrainFeatures\Stalagmite_Frost",
			@"TerrainFeatures\Stalagmite_Lava",
			@"TileSheets\Fireball",
			@"TileSheets\rain",
			@"TileSheets\animations"
		};

		[Attributes.Advanced]
		internal static class Analysis {
			[Attributes.Comment("Max color difference to not consider a sprite to be a gradient?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsInt(0, 255)]
			internal static int MaxGradientColorDifference = 38;
			[Attributes.Comment("Minimum different shades required (per channel) for a sprite to be a gradient?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsInt(0, int.MaxValue)]
			internal static int MinimumGradientShades = 2;
			[Attributes.Comment("Maximum proportion of opaque texels for a sprite to be a gradient?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsReal(0.0, 1.0)]
			internal static double MaximumGradientOpaqueProportion = 0.95;
			[Attributes.Comment("Minimum proportion of opaque texels for a sprite to be premultiplied?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsReal(0.0, 1.0)]
			internal static double MinimumPremultipliedOpaqueProportion = 0.05;
		}

		[Attributes.Ignore]
		[Attributes.Advanced]
		internal static class Deposterization {
			[Attributes.Comment("Should deposterization prepass be performed?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal const bool PreEnabled = false; // disabled as the system needs more work
			[Attributes.Comment("Should deposterization postpass be performed?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal const bool PostEnabled = false; // disabled as the system needs more work
			[Attributes.Comment("Deposterization Color Threshold")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsInt(0, 65_535)]
			internal static int Threshold = 32;
			[Attributes.Comment("Deposterization Block Size")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsInt(1, int.MaxValue)]
			internal static int BlockSize = 1;
			[Attributes.Comment("Default number of passes")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsInt(1, int.MaxValue)]
			internal static int Passes = 2;
			[Attributes.Comment("Use perceptual color for color comparisons?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal static bool UsePerceptualColor = true;
			[Attributes.Comment("Use redmean algorithm for perceptual color comparisons?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal static bool UseRedmean = false;
		}
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.GMCMHidden]
		internal static readonly List<SurfaceFormat> SupportedFormats = new() {
			SurfaceFormat.Color,
			SurfaceFormat.Dxt5,
			SurfaceFormat.Dxt5SRgb,
			SurfaceFormat.Dxt1,
			SurfaceFormat.Dxt1SRgb,
			SurfaceFormat.Dxt1a,
		};

		[Attributes.Comment("Experimental resample-based recolor support")]
		[Attributes.Advanced]
		internal static class Recolor {
			[Attributes.Comment("Should (experimental) resample-based recoloring be enabled?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal static bool Enabled = false;
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsReal(0, 10.0)]
			internal static double RScalar = 0.897642;
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsReal(0, 10.0)]
			internal static double GScalar = 0.998476;
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsReal(0, 10.0)]
			internal static double BScalar = 1.18365;
		}

		[Attributes.Advanced]
		internal static class BlockCompression {
			[Attributes.Comment("Should block compression of sprites be enabled?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal static bool Enabled = DevEnabled && true;
			private const bool DevEnabled = true;
			[Attributes.Comment("What quality level should be used?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal static CompressionQuality Quality = CompressionQuality.High;
			[Attributes.Comment("What alpha deviation threshold should be applied to determine if a sprite's transparency is smooth or mask-like (determines between bc2 and bc3)?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsInt(0, int.MaxValue)]
			internal static int HardAlphaDeviationThreshold = 7;
		}
		[Attributes.Comment("What spritesheets will absolutely not be resampled or processed?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.GMCMHidden]
		internal static List<string> Blacklist = new() {
			@"LooseSprites\Lighting\",
			@"@^Maps\\.+Mist",
			@"@^Maps\\.+mist",
			@"@^Maps\\.+Shadow",
			@"@^Maps\\.+Shadows",
			@"@^Maps\\.+Fog",
			@"@^Maps\\.+FogBackground",
		};
		[Attributes.Ignore]
		internal static Regex[] BlacklistPatterns = Array.Empty<Regex>();
		[Attributes.Comment("What spritesheets will absolutely not be treated as gradients?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.GMCMHidden]
		internal static List<string> GradientBlacklist = new() {
			@"TerrainFeatures\hoeDirt"
		};
		[Attributes.Ignore]
		internal static Regex[] GradientBlacklistPatterns = Array.Empty<Regex>();

		[Attributes.Advanced]
		internal static class Padding {
			[Attributes.Comment("Should padding be applied to sprites to allow resampling to extend beyond the natural sprite boundaries?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal static bool Enabled = DevEnabled && true;
			private const bool DevEnabled = true;
			[Attributes.Comment("What is the minimum edge size of a sprite for padding to be enabled?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsInt(1, int.MaxValue)]
			internal static int MinimumSizeTexels = 4;
			[Attributes.Comment("Should unknown (unnamed) sprites be ignored by the padding system?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal static bool IgnoreUnknown = false;
			[Attributes.Comment("Should solid edges be padded?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal static bool PadSolidEdges = false;

			[Attributes.Comment("What spritesheets should not be padded?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.GMCMHidden]
			internal static List<string> BlackList = new() {
				@"LooseSprites\Cursors::256,308:50,34", // UI borders
			};
			[Attributes.Ignore]
			internal static TextureRef[] BlackListS = Array.Empty<TextureRef>();

			[Attributes.Comment("What spritesheets should have a stricter edge-detection algorithm applied?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.GMCMHidden]
			internal static List<string> StrictList = new() {
				@"LooseSprites\Cursors"
			};
			[Attributes.Comment("What spritesheets should always be padded?")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.GMCMHidden]
			internal static List<string> AlwaysList = new() {
				@"LooseSprites\font_bold",
				@"Characters\Farmer\hairstyles",
				@"Characters\Farmer\hairstyles2",
				@"Characters\Farmer\hats",
				@"Characters\Farmer\pants",
				@"Characters\Farmer\shirts",
				@"TileSheets\weapons",
				@"TileSheets\bushes",
				@"TerrainFeatures\grass",
				@"TileSheets\debris",
				@"TileSheets\animations",
				@"Maps\springobjects",
				@"Maps\summerobjects",
				@"Maps\winterobjects",
				@"Maps\fallobjects",
				@"Buildings\houses",
				@"TileSheets\furniture",
				@"TerrainFeatures\tree1_spring",
				@"TerrainFeatures\tree2_spring",
				@"TerrainFeatures\tree3_spring",
				@"TerrainFeatures\tree1_summer",
				@"TerrainFeatures\tree2_summer",
				@"TerrainFeatures\tree3_summer",
				@"TerrainFeatures\tree1_fall",
				@"TerrainFeatures\tree2_fall",
				@"TerrainFeatures\tree3_fall",
				@"TerrainFeatures\tree1_winter",
				@"TerrainFeatures\tree2_winter",
				@"TerrainFeatures\tree3_winter",
			};
		}

		[Attributes.Comment("Settings common to all scalers")]
		[Attributes.Advanced]
		internal static class Common {
			[Attributes.Comment("The tolerance for colors to be considered equal - [0, 256)")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsInt(0, 255)]
			internal static byte EqualColorTolerance = 20;
			[Attributes.Comment("The weight provided to luminance as opposed to chrominance when performing color comparisons")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsReal(0.0, 10.0)]
			internal static double LuminanceWeight = 1.0;
		}

		[Attributes.Advanced]
		internal static class xBRZ {
			[Attributes.Comment("The threshold for a corner-direction to be considered 'dominant'")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsReal(0.0, 10.0)]
			internal static double DominantDirectionThreshold = 4.4;
			[Attributes.Comment("The threshold for a corner-direction to be considered 'steep'")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsReal(0.0, 10.0)]
			internal static double SteepDirectionThreshold = 2.2;
			[Attributes.Comment("Bias towards kernel center applied to corner-direction calculations")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			[Attributes.LimitsReal(0.0, 10.0)]
			internal static double CenterDirectionBias = 3.0;
			[Attributes.Comment("Should gradient block copies be used? (Note: Very Broken)")]
			[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
			internal static bool UseGradientBlockCopy = false;
		}
	}

	[Attributes.Advanced]
	internal static class WrapDetection {
		[Attributes.Comment("Should edge-wrap analysis be enabled?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		internal const bool Enabled = true;
		[Attributes.Comment("What is the threshold percentage of alpha values to be used to determine if it is a wrapping edge?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		[Attributes.LimitsReal(0.0, 1.0)]
		internal static float EdgeThreshold = 0.2f;
		[Attributes.Comment("What is the minimum alpha value assumed to be opaque?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushAllInternalCaches)]
		internal static byte AlphaThreshold = 1;
	}

	[Attributes.Advanced]
	internal static class AsyncScaling {
		internal const bool Enabled = true;
		[Attributes.Comment("Should asynchronous scaling be enabled for unknown textures?")]
		internal static bool EnabledForUnknownTextures = true;
		[Attributes.Comment("Should synchronous stores always be used?")]
		internal static bool ForceSynchronousStores = !Runtime.Capabilities.AsyncStores;
		[Attributes.Comment("Should synchronous stores be throttled?")]
		internal static bool ThrottledSynchronousLoads = true;
		[Attributes.Comment("Should we fetch and load texture data within the same frame?")]
		internal static bool CanFetchAndLoadSameFrame = true;
		[Attributes.Comment("What is the minimum number of texels in a sprite to be considered for asynchronous scaling?")]
		[Attributes.LimitsInt(0, AbsoluteMaxTextureDimension * AbsoluteMaxTextureDimension)]
		internal static long MinimumSizeTexels = 0;
		[Attributes.Comment("Should the Synchronized Task Scheduler be flushed during warps?")]
		internal static bool FlushSynchronizedTasksOnWarp = true;
	}

	[Attributes.Advanced]
	internal static class ResidentCache {
		[Attributes.Comment("Should the resident cache be enabled?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushResidentCache)]
		internal static bool Enabled = DevEnabled && true;
		private const bool DevEnabled = true;
		[Attributes.Comment("Should memory cache elements always be flushed upon update?")]
		internal static bool AlwaysFlush = false;
		[Attributes.Comment("What is the maximum size of the resident cache?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushResidentCache)]
		[Attributes.LimitsInt(0, long.MaxValue)]
		internal static long MaxSize = SizesExt.AsGiB(2);
		[Attributes.Comment("The preferred compression algorithm for the resident cache")]
		internal static Compression.Algorithm Compress = Compression.BestAlgorithm;
	}

	[Attributes.Advanced]
	internal static class TextureFileCache {
		[Attributes.Comment("Should the texture memory cache be enabled?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushTextureFileCache)]
		internal static bool Enabled = DevEnabled && true;
		private const bool DevEnabled = true;
		[Attributes.Comment("What is the maximum size of the resident cache?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushTextureFileCache)]
		[Attributes.LimitsInt(0, long.MaxValue)]
		internal static long MaxSize = SizesExt.AsGiB(2);
	}

	[Attributes.Advanced]
	internal static class SuspendedCache {
		[Attributes.Comment("Should the suspended sprite cache be enabled?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushSuspendedSpriteCache)]
		internal static bool Enabled = true;
		[Attributes.Comment("What is the maximum size (in bytes) to store in suspended sprite cache?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushSuspendedSpriteCache)]
		[Attributes.LimitsInt(0, long.MaxValue)]
		internal static long MaxCacheSize = SizesExt.AsGiB(4);
	}

	[Attributes.Advanced]
	internal static class SMAPI {
		[Attributes.Comment("Should the ApplyPatch method be patched?")]
		internal static bool ApplyPatchEnabled = true;
		[Attributes.Comment("Should 'GetData' be patched to use SM caches?")]
		internal static bool ApplyGetDataPatch = true;
	}

	[Attributes.Advanced]
	internal static class Extras {
		[Attributes.Comment("Should the game have 'fast quitting' enabled?")]
		[Attributes.Broken]
		internal static bool FastQuit = false;

		[Attributes.Comment("Should line drawing be smoothed?")]
		internal static bool SmoothLines = true;

		[Attributes.Comment("Should shadowed text be stroked instead?")]
		[Attributes.Experimental]
		internal static bool StrokeShadowedText = false;

		[Attributes.Comment("Should the game's 'parseMasterSchedule' method be fixed and optimized?")]
		internal static bool FixMasterSchedule = true;

		[Attributes.Comment("Should the game's 'getSchedule' method be fixed and optimized?")]
		internal static bool FixGetSchedule = true;

		[Attributes.Advanced]
		internal static class Pathfinding {
			[Attributes.Comment("Should NPC Warp Points code be optimized?")]
			internal static bool OptimizeWarpPoints = true;

			[Attributes.Comment("Should gender-locked locations be honored?")]
			internal static bool HonorGenderLocking = true;
		}

		[Attributes.Comment("Should the default batch sort be replaced with a stable sort?")]
		internal static bool StableSort = true;

		[Attributes.Comment("Should the game be prevented from going 'unresponsive' during loads?")]
		internal static bool PreventUnresponsive = true;

		[Attributes.Comment("Should the engine's deferred thread task runner be optimized?")]
		internal static bool OptimizeEngineTaskRunner = true;

		[Attributes.Comment("Should dirt drawing optimizations be enabled?")]
		internal static bool EnableDirtDrawOptimizations = false;

		// ReSharper disable once InconsistentNaming
		internal static class OpenGL {
			[Attributes.Comment("Should low-level OpenGL optimizations be performed?")]
			internal static bool Enabled = true;

			[Attributes.Comment("Should Texture2D.SetData be optimized?")]
			internal static bool OptimizeTexture2DSetData = true;

			[Attributes.Comment("Should Texture2D.GetData be optimized?")]
			internal static bool OptimizeTexture2DGetData = true;

			[Attributes.Comment("Should DrawUserIndexedPrimitives be optimized?")]
			internal const bool OptimizeDrawUserIndexedPrimitives = true;

			[Attributes.Comment("Should glCopyTexture by used?")]
			internal static bool UseCopyTexture = true;

			[Attributes.Comment("Should glTexStorage be used?")]
			internal const bool UseTexStorage = true;
		}

		internal static class Snow {
			[Attributes.Ignore]
			internal static bool ToggledEnable => Resample.ToggledEnable;

			[Attributes.Ignore]
#pragma warning disable CS0618 // Type or member is obsolete
			internal static bool IsEnabled => Preview.Override.Instance?.ResampleEnabled ?? (Enabled && ToggledEnable);
#pragma warning restore CS0618 // Type or member is obsolete

			[Attributes.Comment("Should custom snowfall be used during snowstorms?")]
			[Obsolete($"Use {nameof(IsEnabled)}")]
			internal static bool Enabled = true;
			[Attributes.Comment("Minimum Snow Density")]
			[Attributes.LimitsInt(1, int.MaxValue)]
			internal static int MinimumDensity = 48;
			[Attributes.Comment("Maximum Snow Density")]
			[Attributes.LimitsInt(1, int.MaxValue)]
			internal static int MaximumDensity = 144;
			[Attributes.Comment("Maximum Snow Rotation Speed")]
			[Attributes.LimitsReal(0.0f, 1.0f)]
			internal static float MaximumRotationSpeed = 1.0f / 60.0f;
			[Attributes.Comment("Maximum Snow Scale")]
			[Attributes.LimitsReal(0.0001f, float.MaxValue)]
			internal static float MaximumScale = 3.0f;
			[Attributes.Comment("Puffersnow Chance")]
			[Attributes.LimitsReal(0.0f, 1.0f)]
			internal static float PuffersnowChance = 0.0f;
		}
		internal static class ModPatches {
			[Attributes.Comment("Patch CustomNPCFixes in order to improve load times?")]
			internal static bool PatchCustomNPCFixes = true;
			[Attributes.Comment("Disable unnecessary PyTK mitigation for SpriteMaster?")]
			[Attributes.MenuName("Disable PyTK Mitigation")]
			internal static bool DisablePyTKMitigation = true;
		}
	}

	[Attributes.Advanced]
	internal static class FileCache {
		internal const bool Purge = false;
		[Attributes.Comment("Should the file cache be enabled?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushFileCache)]
		internal static bool Enabled = DevEnabled && true;
		private const bool DevEnabled = true;
		internal const int LockRetries = 32;
		internal const int LockSleepMilliseconds = 32;
		[Attributes.Comment("What compression algorithm should be used?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushFileCache)]
		internal static Compression.Algorithm Compress = Compression.BestAlgorithm;
		[Attributes.Comment("Should files be compressed regardless of if it would be beneficial?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushFileCache)]
		internal static bool ForceCompress = false;
		[Attributes.Comment("Should system compression (such as NTFS compression) be preferred?")]
		[Attributes.OptionsAttribute(Attributes.OptionsAttribute.Flag.FlushFileCache)]
		internal static bool PreferSystemCompression = false;
		internal const bool Profile = false;
	}
}
