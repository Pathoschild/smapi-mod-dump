/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

global using SMConfig = SpriteMaster.Config;

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Resample;
using SpriteMaster.Types;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text.RegularExpressions;
using TeximpNet.Compression;

namespace SpriteMaster;

using SMResample = Resample;

static class Config {
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	internal sealed class CommentAttribute : Attribute {
		internal readonly string Message;

		internal CommentAttribute(string message) {
			Message = message;
		}
	}

	internal sealed class ConfigIgnoreAttribute : Attribute { }
	internal sealed class ConfigRetainAttribute : Attribute { }

	internal sealed class ConfigOldNameAttribute : Attribute {
		internal readonly string Name;

		internal ConfigOldNameAttribute(string name) {
			Name = name;
		}
	}

	internal static readonly string ModuleName = typeof(Config).Namespace ?? "SpriteMaster";

	internal const bool IgnoreConfig = false ||
#if DEBUG
		true;
#else
		false;
#endif
	internal const bool SkipIntro = IgnoreConfig;

	[ConfigIgnore]
	internal static readonly string CurrentVersion = typeof(Config).Assembly.GetCustomAttribute<FullVersionAttribute>()?.Value?.Split('-', 2)?.ElementAtOrDefault(0) ??
		throw new BadImageFormatException($"Could not extract version from assembly {typeof(Config).Assembly.FullName ?? typeof(Config).Assembly.ToString()}");

	[ConfigIgnore]
	internal static readonly Version AssemblyVersionObj = typeof(Config).Assembly.GetName().Version ??
		throw new BadImageFormatException($"Could not extract version from assembly {typeof(Config).Assembly.FullName ?? typeof(Config).Assembly.ToString()}");
	[ConfigIgnore]
	internal static readonly string AssemblyVersion = AssemblyVersionObj.ToString();

	private enum BuildType {
		Alpha,
		Beta,
		Candidate,
		Final
	}

	[ConfigIgnore]
	private static string GenerateAssemblyVersionString(int major, int minor, int revision, int build, BuildType type = BuildType.Final, int release = 0) {
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

	internal static string ConfigVersion = "";
	[ConfigIgnore]
	internal static string ClearConfigBefore = GenerateAssemblyVersionString(0, 13, 0, 0, BuildType.Final, 0);

	[ConfigIgnore]
	internal static bool ForcedDisable = false;
	[Comment("Should SpriteMaster be enabled?")]
	internal static bool Enabled = true;
	internal static bool IsEnabled => !ForcedDisable && Enabled;
	[Comment("Button to toggle SpriteMaster")]
	internal static SButton ToggleButton = SButton.F11;

	[ConfigIgnore]
	internal static int ClampDimension = BaseMaxTextureDimension; // this is adjustable by the system itself. The user shouldn't be able to touch it.
	[Comment("The preferred maximum texture edge length, if allowed by the hardware")]
	internal const int AbsoluteMaxTextureDimension = 16384;
	internal const int BaseMaxTextureDimension = 4096;
	internal static int PreferredMaxTextureDimension = 16384;
	internal const bool ClampInvalidBounds = true;
	internal const bool IgnoreUnknownTextures = false;

	[ConfigRetain]
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

	[ConfigIgnore]
	internal static readonly string LocalRootDefault = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
		"StardewValley",
		"Mods",
		ModuleName
	);
	internal static string LocalRoot => (DataStoreOverride.Length == 0) ? LocalRootDefault : DataStoreOverride;
	[Comment("If the data cache is preferred to be elsewhere, it can be set here")]
	internal static string DataStoreOverride = "";

	internal static class WatchDog {
		[Comment("Should the watchdog be enabled?")]
		internal static bool Enabled = false;
		[Comment("What should the default sleep interval be (in milliseconds)?")]
		internal static int DefaultSleepInterval = 5_000;
		[Comment("What should the short sleep interval be (in milliseconds)?")]
		internal static int ShortSleepInterval = 500;
		[Comment("What should the interrupt interval be (in milliseconds)?")]
		internal static int InterruptInterval = 10_000;
	}

	internal static class Garbage {
		[Comment("Should unowned textures be marked in the garbage collector's statistics?")]
		internal static bool CollectAccountUnownedTextures = false;
		[Comment("Should owned textures be marked in the garbage collector's statistics?")]
		internal static bool CollectAccountOwnedTextures = false;
		[Comment("The amount of free memory required by SM after which it triggers recovery operations")]
		internal static int RequiredFreeMemory = 128;
		[Comment("Hysterisis applied to RequiredFreeMemory")]
		internal static double RequiredFreeMemoryHysterisis = 1.5;
		[Comment("Should sprites containing season names be purged on a seasonal basis?")]
		internal static bool SeasonalPurge = true;
		[Comment("What runtime garbage collection latency mode should be set?")]
		internal static GCLatencyMode LatencyMode = GCLatencyMode.SustainedLowLatency;
	}

	internal static class Debug {
		internal static class Logging {
			internal static LogLevel LogLevel = LogLevel.Trace;
			internal const bool OwnLogFile = true;
		}

		internal static class Sprite {
			internal const bool DumpReference = !IsRelease;
			internal const bool DumpResample = !IsRelease;
		}
	}

	internal static class DrawState {
		[Comment("Enable linear sampling for sprites")]
		internal static bool SetLinear = true;
		[Comment("How many MSAA samples should be used?")]
		internal static int MSAASamples = 0;
		[Comment("Disable the depth buffer (unused in this game)")]
		internal static bool DisableDepthBuffer = false;
		[Comment("The default backbuffer format to request")]
		internal static SurfaceFormat BackbufferFormat = SurfaceFormat.Color;
		[Comment("The default HDR backbuffer format to request")]
		internal static SurfaceFormat BackbufferHDRFormat = SurfaceFormat.Rgba64;
		[Comment("Should the system HDR settings be honored?")]
		internal static bool HonorHDRSettings = true;
	}

	internal static class Performance {
		[Comment("Perform a Generation 0 and 1 garbage collection pass every N ticks (if <= 0, disabled)")]
		internal static int TransientGCTickCount = 150;
	}

	internal readonly record struct TextureRef(string Texture, Bounds Bounds);

	internal static class Resample {
		[Comment("Should resampling be enabled?")]
		internal static bool Enabled = true;
		[Comment("Should texture rescaling be enabled?")]
		internal static bool Scale = Enabled;
		[Comment("What scaling algorithm should be used by default?")]
		internal const Resampler.Scaler Scaler = Resampler.Scaler.xBRZ;
		[Comment("What scaling algorithm should be used for gradient sprites?")]
		internal const Resampler.Scaler ScalerGradient = Resampler.Scaler.None;
		[Comment("Should dynamic scaling be used (scaling based upon apparent sprite size)")]
		internal const bool EnableDynamicScale = true;
		[Comment("Should we assume that input sprites are gamma corrected?")]
		internal static bool AssumeGammaCorrected = true;
		[Comment("Should the scale factor of water be adjusted to account for water sprites being unusual?")]
		internal static bool TrimWater = true;
		[Comment("Maximum scale factor of sprites (dependant on chosen scaler)")]
		internal static uint MaxScale = SMResample.Scalers.IScaler.Current.MaxScale;
		[Comment("Minimum edge length of a sprite to be considered for resampling")]
		internal static int MinimumTextureDimensions = 1;
		[Comment("Should wrapped addressing be enabled for sprite resampling (when analysis suggests it)?")]
		internal static bool EnableWrappedAddressing = false;
		[Comment("Should resampling be stalled if it is determined that it will cause hitches?")]
		internal static bool UseFrametimeStalling = true;
		[Comment("Should color enhancement/rebalancing be performed?")]
		internal static bool UseColorEnhancement = true;
		[Comment("Should transparent pixels be premultiplied to prevent a 'halo' effect?")]
		internal static bool PremultiplyAlpha = true;
		[Comment("Low pass value that should be filtered when reversing premultiplied alpha.")]
		internal static int PremultiplicationLowPass = 1024;
		[Comment("Use redmean algorithm for perceptual color comparisons?")]
		internal static bool UseRedmean = false;
		[Comment("What textures are drawn in 'slices' and thus should be special-cased to be resampled as one texture?")]
		internal static List<string> SlicedTextures = new() {
			@"LooseSprites\Cursors::0,2000:640,256",
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
		[ConfigIgnore]
		internal static TextureRef[] SlicedTexturesS = Array.Empty<TextureRef>();
		internal static class BlockMultipleAnalysis {
			[Comment("Should sprites be analyzed to see if they are block multiples?")]
			internal static bool Enabled = true;
			[Comment("What threshold should be used for block multiple analysis?")]
			internal static int EqualityThreshold = 1;
			[Comment("How many blocks can be different for the test to still pass?")]
			internal static int MaxInequality = 1;
		}

		[Comment("What textures or spritesheets use 4xblock sizes?")]
		internal static List<string> TwoXTextures = new() {
			@"Maps\WoodBuildings" // is _almost_ TwoX.
		};
		[Comment("What textures or spritesheets use 4xblock sizes?")]
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
		internal static class Analysis {
			[Comment("Max color difference to not consider a sprite to be a gradient?")]
			internal static int MaxGradientColorDifference = 38;
			[Comment("Minimum different shades required (per channel) for a sprite to be a gradient?")]
			internal static int MinimumGradientShades = 2;
			[Comment("Use redmean algorithm for perceptual color comparisons?")]
			internal static bool UseRedmean = true;
		}
		[ConfigIgnore]
		internal static class Deposterization {
			[Comment("Should deposterization prepass be performed?")]
			internal const bool PreEnabled = false; // disabled as the system needs more work
			[Comment("Should deposterization postpass be performed?")]
			internal const bool PostEnabled = false; // disabled as the system needs more work
			[Comment("Deposterization Color Threshold")]
			internal static int Threshold = 32;
			[Comment("Deposterization Block Size")]
			internal static int BlockSize = 1;
			[Comment("Default number of passes")]
			internal static int Passes = 2;
			[Comment("Use perceptual color for color comparisons?")]
			internal static bool UsePerceptualColor = true;
			[Comment("Use redmean algorithm for perceptual color comparisons?")]
			internal static bool UseRedmean = false;
		}
		internal static readonly List<SurfaceFormat> SupportedFormats = new() {
			SurfaceFormat.Color,
			SurfaceFormat.Dxt5,
			SurfaceFormat.Dxt5SRgb,
			SurfaceFormat.Dxt1,
			SurfaceFormat.Dxt1SRgb,
			SurfaceFormat.Dxt1a,
		};

		[Comment("Experimental resample-based recolor support")]
		internal static class Recolor {
			[Comment("Should (experimental) resample-based recoloring be enabled?")]
			internal static bool Enabled = false;
			internal static double RScalar = 0.897642;
			internal static double GScalar = 0.998476;
			internal static double BScalar = 1.18365;
		}

		internal static class BlockCompression {
			[Comment("Should block compression of sprites be enabled?")]
			internal static bool Enabled = DevEnabled && (!Runtime.IsMacintosh || MacSupported) && true; // I cannot build a proper libnvtt for OSX presently.
			[ConfigIgnore]
			private const bool MacSupported = false;
			private const bool DevEnabled = true;
			[Comment("What quality level should be used?")]
			internal static CompressionQuality Quality = CompressionQuality.Highest;
			[Comment("What alpha deviation threshold should be applied to determine if a sprite's transparency is smooth or mask-like (determines between bc2 and bc3)?")]
			internal static int HardAlphaDeviationThreshold = 7;
		}
		[Comment("What spritesheets will absolutely not be resampled or processed?")]
		internal static List<string> Blacklist = new() {
			@"LooseSprites\Lighting\",
			@"@^Maps\\.+Mist",
			@"@^Maps\\.+mist",
			@"@^Maps\\.+Shadow",
			@"@^Maps\\.+Shadows",
			@"@^Maps\\.+Fog",
			@"@^Maps\\.+FogBackground",
		};
		[ConfigIgnore]
		internal static Regex[] BlacklistPatterns = new Regex[0];
		[Comment("What spritesheets will absolutely not be treated as gradients?")]
		internal static List<string> GradientBlacklist = new() {
			@"TerrainFeatures\hoeDirt"
		};
		[ConfigIgnore]
		internal static Regex[] GradientBlacklistPatterns = new Regex[0];
		internal static class Padding {
			[Comment("Should padding be applied to sprites to allow resampling to extend beyond the natural sprite boundaries?")]
			internal static bool Enabled = DevEnabled && true;
			private const bool DevEnabled = true;
			[Comment("What is the minimum edge size of a sprite for padding to be enabled?")]
			internal static int MinimumSizeTexels = 4;
			[Comment("Should unknown (unnamed) sprites be ignored by the padding system?")]
			internal static bool IgnoreUnknown = false;
			[Comment("Should solid edges be padded?")]
			internal static bool PadSolidEdges = false;

			[Comment("What spritesheets should not be padded?")]
			internal static List<string> BlackList = new() {
				@"LooseSprites\Cursors::256,308:50,34", // UI borders
			};
			[ConfigIgnore]
			internal static TextureRef[] BlackListS = Array.Empty<TextureRef>();

			[Comment("What spritesheets should have a stricter edge-detection algorithm applied?")]
			internal static List<string> StrictList = new() {
				@"LooseSprites\Cursors"
			};
			[Comment("What spritesheets should always be padded?")]
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
		internal static class xBRZ {
			[Comment("The weight provided to luminance as opposed to chrominance when performing color comparisons")]
			internal static double LuminanceWeight = 1.0;
			[Comment("The tolerance for colors to be considered equal - [0, 256)")]
			internal static uint EqualColorTolerance = 20;
			[Comment("The threshold for a corner-direction to be considered 'dominant'")]
			internal static double DominantDirectionThreshold = 4.4;
			[Comment("The threshold for a corner-direction to be considered 'steep'")]
			internal static double SteepDirectionThreshold = 2.2;
			[Comment("Bias towards kernel center applied to corner-direction calculations")]
			internal static double CenterDirectionBias = 3.0;
			[Comment("Should gradient block copies be used? (Note: Very Broken)")]
			internal static bool UseGradientBlockCopy = false;
		}
	}

	internal static class WrapDetection {
		[Comment("Should edge-wrap analysis be enabled?")]
		internal const bool Enabled = true;
		[Comment("What is the threshold percentage of alpha values to be used to determine if it is a wrapping edge?")]
		internal static float edgeThreshold = 0.2f;
		[Comment("What is the minimum alpha value assumed to be opaque?")]
		internal static byte alphaThreshold = 1;
	}

	internal static class AsyncScaling {
		internal const bool Enabled = true;
		[Comment("Should asynchronous scaling be enabled for unknown textures?")]
		internal static bool EnabledForUnknownTextures = true;
		[Comment("Should synchronous stores always be used?")]
		internal static bool ForceSynchronousStores = !Runtime.Capabilities.AsyncStores;
		[Comment("Should synchronous stores be throttled?")]
		internal static bool ThrottledSynchronousLoads = true;
		[Comment("Should we fetch and load texture data within the same frame?")]
		internal static bool CanFetchAndLoadSameFrame = true;
		[Comment("What is the minimum number of texels in a sprite to be considered for asynchronous scaling?")]
		internal static long MinimumSizeTexels = 0;
	}

	internal static class MemoryCache {
		[Comment("Should the memory cache be enabled?")]
		internal static bool Enabled = DevEnabled && true;
		private const bool DevEnabled = true;
		[Comment("Should memory cache elements always be flushed upon update?")]
		internal static bool AlwaysFlush = false;
	}

	internal static class SuspendedCache {
		[Comment("Should the suspended sprite cache be enabled?")]
		internal static bool Enabled = true;
		[Comment("What is the maximum size (in bytes) to store in suspended sprite cache?")]
		internal static long MaxCacheSize = 0x1000_0000L;
		[Comment("What is the maximum number of sprites to store in suspended sprite cache?")]
		internal static long MaxCacheCount = 2_000L;
	}

	internal static class SMAPI {
		[Comment("Should the experimental SMAPI texture cache patch be enabled?")]
		internal static bool TextureCacheEnabled = true;
		[Comment("Should the experimental SMAPI texture cache have high memory usage enabled?")]
		[Comment("Unrecommended: This results in the game's texture being retained (and thus loaded faster) but doesn't suspend the resampled sprite instances.")]
		internal static bool TextureCacheHighMemoryEnabled = false;
		[Comment("Should the ApplyPatch method be patched?")]
		internal static bool ApplyPatchEnabled = true;
		[Comment("Should ApplyPatch pin temporary memory?")]
		internal static bool ApplyPatchPinMemory = false;
		[Comment("Should 'GetData' be patched to use SM caches?")]
		internal static bool ApplyGetDataPatch = true;
	}

	internal static class Extras {
		[Comment("Should the game have 'fast quitting' enabled?")]
		internal static bool FastQuit = false;
		[Comment("Should line drawing be smoothed?")]
		internal static bool SmoothLines = true;
		[Comment("Should Harmony patches have inlining re-enabled?")]
		internal static bool HarmonyInlining = false;
		[Comment("Should the game's 'parseMasterSchedule' method be fixed and optimized?")]
		internal static bool FixMasterSchedule = true;
		[Comment("Should NPC Warp Points code be optimized?")]
		internal static bool OptimizeWarpPoints = true;
		[Comment("Should NPCs take true shortest paths?")]
		internal static bool TrueShortestPath = false;
		[Comment("Allow NPCs onto the farm?")]
		internal static bool AllowNPCsOnFarm = false;
		[Comment("Should the default batch sort be replaced with a stable sort?")]
		internal static bool StableSort = true;
		[Comment("Should the game be prevented from going 'unresponsive' during loads?")]
		internal static bool PreventUnresponsive = true;
		[Comment("Should the engine's deferred thread task runner be optimized?")]
		internal static bool OptimizeEngineTaskRunner = true;
		internal static class Snow {
			[Comment("Should custom snowfall be used during snowstorms?")]
			internal static bool Enabled = true;
			[Comment("Minimum Snow Density")]
			internal static int MinimumDensity = 1024;
			[Comment("Maximum Snow Density")]
			internal static int MaximumDensity = 3072;
			[Comment("Maximum Snow Rotation Speed")]
			internal static double MaximumRotationSpeed = 1.0 / 60.0;
			[Comment("Maximum Snow Scale")]
			internal static float MaximumScale = 3.0f;
			[Comment("Puffersnow Chance")]
			internal static float PuffersnowChance = -1.0f;
		}
		internal static class ModPatches {
			[Comment("Patch CustomNPCFixes in order to improve load times?")]
			internal static bool PatchCustomNPCFixes = false;
			[Comment("Disable PyTK mitigation for SpriteMaster?")]
			internal static bool DisablePyTKMitigation = true;
		}
	}

	internal static class FileCache {
		internal const bool Purge = false;
		[Comment("Should the file cache be enabled?")]
		internal static bool Enabled = DevEnabled && true;
		private const bool DevEnabled = true;
		internal const int LockRetries = 32;
		internal const int LockSleepMS = 32;
		[Comment("What compression algorithm should be used?")]
		internal static Compression.Algorithm Compress = Compression.BestAlgorithm;
		[Comment("Should files be compressed regardless of if it would be beneficial?")]
		internal static bool ForceCompress = false;
		[Comment("Should system compression (such as NTFS compression) be preferred?")]
		internal static bool PreferSystemCompression = false;
		internal const bool Profile = false;
	}
}
