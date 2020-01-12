using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using TeximpNet.Compression;

namespace SpriteMaster {
	static class Config {
		internal sealed class CommentAttribute : Attribute {
			public readonly string Message;

			public CommentAttribute (string message) {
				Message = message;
			}
		}

		internal sealed class ConfigIgnoreAttribute : Attribute { }

		internal static readonly string ModuleName = typeof(Config).Namespace;

		[ConfigIgnore]
		internal static readonly Version CurrentVersionObj = typeof(Config).Assembly.GetName().Version;
		[ConfigIgnore]
		internal static readonly string CurrentVersion = CurrentVersionObj.ToString(3);

		internal static string ConfigVersion = "";
		[ConfigIgnore]
		internal static string ClearConfigBefore = "0.10.3";

		internal static bool Enabled = true;
		internal static SButton ToggleButton = SButton.F11;

		internal const int MaxSamplers = 16;
		[ConfigIgnore]
		internal static bool RendererSupportsAsyncOps = true;
		[ConfigIgnore]
		internal static int ClampDimension = BaseMaxTextureDimension; // this is adjustable by the system itself. The user shouldn't be able to touch it.
		[Comment("The preferred maximum texture edge length, if allowed by the hardware")]
		internal const int AbsoluteMaxTextureDimension = 16384;
		internal const int BaseMaxTextureDimension = 4096;
		internal static int PreferredMaxTextureDimension = 8192;
		internal static int RequiredFreeMemory = 64;
		internal static double RequiredFreeMemoryHysterisis = 1.5;
		internal const bool ClampInvalidBounds = true;
		internal const bool IgnoreUnknownTextures = false;
		internal static bool GarbageCollectAccountUnownedTextures = true;
		internal static bool GarbageCollectAccountOwnedTexture = true;
		internal static bool LeakPreventTexture = true;
		internal static bool LeakPreventAll = true;
		internal static bool DiscardDuplicates = false;
		internal static int DiscardDuplicatesFrameDelay = 2;
		internal static List<string> DiscardDuplicatesBlacklist = new List<string>() {
			"LooseSprites/Cursors",
			"Minigames/TitleButtons"
		};

		internal enum Configuration {
			Debug,
			Release
		}

#if DEBUG
		internal const Configuration BuildConfiguration = Configuration.Debug;
#else
		internal const Configuration BuildConfiguration = Configuration.Release;
#endif

		internal const bool IsDebug = BuildConfiguration == Configuration.Debug;
		internal const bool IsRelease = BuildConfiguration == Configuration.Release;

		internal static readonly string LocalRoot = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"StardewValley",
			"Mods",
			ModuleName
		);

		internal static class Debug {
			internal static class Logging {
				internal static bool LogInfo = true;
				internal static bool LogWarnings = true;
				internal static bool LogErrors = true;
				internal const bool OwnLogFile = true;
				internal const bool UseSMAPI = true;
			}

			internal static class Sprite {
				internal const bool DumpReference = false;
				internal const bool DumpResample = false;
			}
		}

		internal static class DrawState {
			internal static bool SetLinear = true;
			internal static bool EnableMSAA = true;
			internal static bool DisableDepthBuffer = false;
			internal static SurfaceFormat BackbufferFormat = SurfaceFormat.Color;
		}

		internal static class Resample {
			internal const bool Smoothing = true;
			internal const bool Scale = Smoothing;
			internal const Upscaler.Scaler Scaler = Upscaler.Scaler.xBRZ;
			internal static bool EnableDynamicScale = true;
			internal static bool TrimWater = true;
			internal static float ScaleBias = 0.1f;
			internal static int MaxScale = 6;
			internal static int MinimumTextureDimensions = 4;
			internal static bool EnableWrappedAddressing = true;
			internal static bool UseBlockCompression = true;
			internal static CompressionQuality BlockCompressionQuality = CompressionQuality.Highest;
			internal static int BlockHardAlphaDeviationThreshold = 7;
			internal static List<string> Blacklist = new List<string>() {
				"LooseSprites/Lighting/"
			};
			internal static class Padding {
				internal static bool Enabled = true;
				internal static int MinimumSizeTexels = 4;
				internal static bool IgnoreUnknown = false;
				internal static List<string> StrictList = new List<string>() {
					"LooseSprites/Cursors"
				};
				internal static List<string> Whitelist = new List<string>() {
					"LooseSprites/font_bold",
					"Characters/Farmer/hairstyles",
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
				internal static List<string> Blacklist = new List<string>() {
				};
			}
		}

		internal static class WrapDetection {
			internal const bool Enabled = true;
			internal static float edgeThreshold = 0.2f;
			internal static byte alphaThreshold = 1;
		}

		internal static class AsyncScaling {
			internal static bool Enabled = true;
			internal static bool EnabledForUnknownTextures = false;
			internal const bool CanFetchAndLoadSameFrame = false;
			internal const int MaxLoadsPerFrame = 1;
			internal static long MinimumSizeTexels = 0;
			internal static long ScalingBudgetPerFrameTexels = 16 * 256 * 256;
		}

		internal static class MemoryCache {
			internal static bool Enabled = true;
			internal static bool AlwaysFlush = false;
			internal enum Algorithm {
				None = 0,
				COMPRESS = 1,
				LZ = 2,
				LZMA = 3
			}
			internal static Algorithm Type = Algorithm.COMPRESS;
			internal static bool Async = true;
		}

		internal static class Cache {
			internal const bool Purge = false;
			internal static bool Enabled = true;
			internal const int LockRetries = 32;
			internal const int LockSleepMS = 32;
		}
	}
}
