/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using TeximpNet.Compression;

namespace SpriteMaster;
static class Config {
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

	internal static readonly string ModuleName = typeof(Config).Namespace;

	internal const bool IgnoreConfig = true;
	internal const bool SkipIntro = IgnoreConfig;

	[ConfigIgnore]
	internal static readonly Version CurrentVersionObj = typeof(Config).Assembly.GetName().Version;
	[ConfigIgnore]
	internal static readonly string CurrentVersion = CurrentVersionObj.ToString(3);

	internal static string ConfigVersion = "";
	[ConfigIgnore]
	internal static string ClearConfigBefore = "0.13.0.0";

	[Comment("Should SpriteMaster be enabled?")]
	internal static bool Enabled = true;
	[Comment("Button to toggle SpriteMaster")]
	internal static SButton ToggleButton = SButton.F11;

	internal const int MaxSamplers = 16;
	[ConfigIgnore]
	internal static int ClampDimension = BaseMaxTextureDimension; // this is adjustable by the system itself. The user shouldn't be able to touch it.
	[Comment("The preferred maximum texture edge length, if allowed by the hardware")]
	internal const int AbsoluteMaxTextureDimension = 16384;
	internal const int BaseMaxTextureDimension = 4096;
	internal static int PreferredMaxTextureDimension = 8192;
	internal const bool ClampInvalidBounds = true;
	internal const bool IgnoreUnknownTextures = false;

	[ConfigRetain]
	internal static bool ShowIntroMessage = true;

	internal enum Configuration {
		Debug,
		Release
	}

	internal const Configuration BuildConfiguration =
#if DEBUG
			Configuration.Debug;
#else
			Configuration.Release;
#endif

	internal const bool IsDebug = BuildConfiguration == Configuration.Debug;
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

	internal static class Garbage {
		[Comment("Should unowned textures be marked in the garbage collector's statistics?")]
		internal static bool CollectAccountUnownedTextures = true;
		[Comment("Should owned textures be marked in the garbage collector's statistics?")]
		internal static bool CollectAccountOwnedTextures = true;
		[Comment("Should SM attempt to detect and prevent texture memory leaks?")]
		internal static bool LeakPreventTexture = false;
		[Comment("Should SM attempt to detect and prevent disposable object memory leaks?")]
		internal static bool LeakPreventAll = false;
		[Comment("The amount of free memory required by SM after which it triggers recovery operations")]
		internal static int RequiredFreeMemory = 64;
		[Comment("Hysterisis applied to RequiredFreeMemory")]
		internal static double RequiredFreeMemoryHysterisis = 1.5;
	}

	internal static class Debug {
		internal static class Logging {
			internal static bool LogInfo = true;
			internal static bool LogWarnings = true;
			internal static bool LogErrors = true;
			internal const bool OwnLogFile = true;
		}

		internal static class Sprite {
			internal const bool DumpReference = true;
			internal const bool DumpResample = true;
		}

		internal const bool MacOSTestMode = false;
	}

	internal static class DrawState {
		[Comment("Enable linear sampling for sprites")]
		internal static bool SetLinear = true;
		[Comment("Enable MSAA")]
		internal static bool EnableMSAA = false;
		[Comment("Disable the depth buffer (unused in this game)")]
		internal static bool DisableDepthBuffer = false;
		[Comment("The default backbuffer format to request")]
		internal static SurfaceFormat BackbufferFormat = SurfaceFormat.Color;
	}

	internal static class Resample {
		[Comment("Should resampling be enabled?")]
		internal static bool Enabled = true;
		[Comment("Should texture rescaling be enabled?")]
		internal static bool Scale = Enabled;
		[Comment("What scaling algorithm should be used?")]
		internal const Upscaler.Scaler Scaler = Upscaler.Scaler.xBRZ;
		[Comment("Should dynamic scaling be used (scaling based upon apparent sprite size)")]
		internal const bool EnableDynamicScale = true;
		[Comment("Should we assume that input sprites are gamma corrected?")]
		internal static bool AssumeGammaCorrected = true;
		[Comment("Should the scale factor of water be adjusted to account for water sprites being unusual?")]
		internal static bool TrimWater = true;
		[Comment("Positive bias applied to sprite scaling calculations")]
		internal static float ScaleBias = 0.1f;
		[Comment("Maximum scale factor of sprites")]
		internal static uint MaxScale = 6;
		[Comment("Minimum edge length of a sprite to be considered for resampling")]
		internal static int MinimumTextureDimensions = 4;
		[Comment("Should wrapped addressing be enabled for sprite resampling (when analysis suggests it)?")]
		internal static bool EnableWrappedAddressing = true;
		[Comment("Should resampling be stalled if it is determined that it will cause hitches?")]
		internal static bool UseFrametimeStalling = true;
		[Comment("Should color enhancement/rebalancing be performed?")]
		internal static bool UseColorEnhancement = true;
		[Comment("Should input textures be assumed to be using premultiplied alpha?")]
		internal static bool AssumePremultiplyAlpha = true;
		[Comment("Should transparent pixels be premultiplied to prevent a 'halo' effect?")]
		internal static bool PremultiplyAlpha = true;
		[ConfigIgnore]
		internal static class Deposterization {
			[Comment("Should deposterization be performed?")]
			internal const bool Enabled = false; // disabled as the system needs more work
			[Comment("Deposterization Color Threshold")]
			internal static int Threshold = 56;
			[Comment("Deposterization Block Size")]
			internal static int BlockSize = 2;
			[Comment("Default number of passes")]
			internal static int Passes = 3;
			[Comment("Use YCbCr for color comparisons?")]
			internal static bool UseYCbCr = true;
		}
		internal static readonly List<SurfaceFormat> SupportedFormats = new() {
			SurfaceFormat.Color,
			SurfaceFormat.Dxt5,
			SurfaceFormat.Dxt5SRgb,
			SurfaceFormat.Dxt3,
			SurfaceFormat.Dxt3SRgb,
			SurfaceFormat.Dxt1,
			SurfaceFormat.Dxt1SRgb,
			SurfaceFormat.Dxt1a,
		};
		internal static class BlockCompression {
			[Comment("Should block compression of sprites be enabled?")]
			internal static bool Enabled = DevEnabled && ((!Runtime.IsMacintosh && !Debug.MacOSTestMode) || MacSupported) && true; // I cannot build a proper libnvtt for OSX presently.
			[ConfigIgnore]
			private const bool MacSupported = false;
			private const bool DevEnabled = true;
			[Comment("Should block compression of sprites be synchronous?")]
			internal static bool Synchronized = false;
			[Comment("What quality level should be used?")]
			internal static CompressionQuality Quality = CompressionQuality.Highest;
			[Comment("What alpha deviation threshold should be applied to determine if a sprite's transparency is smooth or mask-like (determines between bc2 and bc3)?")]
			internal static int HardAlphaDeviationThreshold = 7;
		}
		[Comment("What spritesheets will absolutely not be resampled or processed?")]
		internal static List<string> Blacklist = new() {
			"LooseSprites/Lighting/",
			"LooseSprites/Cloudy_Ocean_BG",
			"LooseSprites/Cloudy_Ocean_BG_Night"
		};
		internal static class Padding {
			[Comment("Should padding be applied to sprites to allow resampling to extend beyond the natural sprite boundaries?")]
			internal static bool Enabled = DevEnabled && true;
			private const bool DevEnabled = true;
			[Comment("What is the minimum edge size of a sprite for padding to be enabled?")]
			internal static int MinimumSizeTexels = 4;
			[Comment("Should unknown (unnamed) sprites be ignored by the padding system?")]
			internal static bool IgnoreUnknown = false;

			[Comment("What spritesheets should have a stricter edge-detection algorithm applied?")]
			internal static List<string> StrictList = new() {
				"LooseSprites/Cursors"
			};
			[Comment("What spritesheets should always be padded?")]
			internal static List<string> Whitelist = new() {
				"LooseSprites/font_bold",
				"Characters/Farmer/hairstyles",
				"Characters/Farmer/hairstyles2",
				"Characters/Farmer/hats",
				"Characters/Farmer/pants",
				"Characters/Farmer/shirts",
				"TileSheets/weapons",
				"TileSheets/bushes",
				"TerrainFeatures/grass",
				"TileSheets/debris",
				"TileSheets/animations",
				"Maps/springobjects",
				"Maps/summerobjects",
				"Maps/winterobjects",
				"Maps/fallobjects",
				"Buildings/houses",
				"TileSheets/furniture",
				"TerrainFeatures/tree1_spring",
				"TerrainFeatures/tree2_spring",
				"TerrainFeatures/tree3_spring",
				"TerrainFeatures/tree1_summer",
				"TerrainFeatures/tree2_summer",
				"TerrainFeatures/tree3_summer",
				"TerrainFeatures/tree1_fall",
				"TerrainFeatures/tree2_fall",
				"TerrainFeatures/tree3_fall",
				"TerrainFeatures/tree1_winter",
				"TerrainFeatures/tree2_winter",
				"TerrainFeatures/tree3_winter",
			};

			[Comment("What spritesheets should never be padded?")]
			internal static List<string> Blacklist = new() {
				"LooseSprites/Cloudy_Ocean_BG",
				"LooseSprites/Cloudy_Ocean_BG_Night"
			};
		}
		internal static class xBRZ {
			[Comment("The weight provided to luminance as opposed to chrominance when performing color comparisons")]
			internal static double LuminanceWeight = 1.0;
			[Comment("The tolerance for colors to be considered equal - [0, 256)")]
			internal static double EqualColorTolerance = 30.0;
			[Comment("The threshold for a corner-direction to be considered 'dominant'")]
			internal static double DominantDirectionThreshold = 3.6;
			[Comment("The threshold for a corner-direction to be considered 'steep'")]
			internal static double SteepDirectionThreshold = 2.2;
			[Comment("Bias towards kernel center applied to corner-direction calculations")]
			internal static double CenterDirectionBias = 4.0;
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
		internal static bool EnabledForUnknownTextures = false;
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
		[Comment("Should memory compression algorithm should be used?")]
		internal static Compression.Algorithm Compress = (Runtime.Bits == 64) ? Compression.Algorithm.None : Compression.BestAlgorithm;
		[Comment("Should the memory cache be asynchronous?")]
		internal static bool Async = true;
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
