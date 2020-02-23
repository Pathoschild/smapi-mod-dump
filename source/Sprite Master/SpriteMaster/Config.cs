using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
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

		internal const bool IgnoreConfig = false;
		internal const bool SkipIntro = IgnoreConfig;

		[ConfigIgnore]
		internal static readonly Version CurrentVersionObj = typeof(Config).Assembly.GetName().Version;
		[ConfigIgnore]
		internal static readonly string CurrentVersion = CurrentVersionObj.ToString(3);

		internal static string ConfigVersion = "";
		[ConfigIgnore]
		internal static string ClearConfigBefore = "0.11.0";

		internal static bool Enabled = true;
		internal static SButton ToggleButton = SButton.F11;

		internal const int MaxSamplers = 16;
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
		internal static bool LeakPreventTexture = false;
		internal static bool LeakPreventAll = false;
		internal static bool ShowIntroMessage = true;

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
			}

			internal static class Sprite {
				internal const bool DumpReference = false;
				internal const bool DumpResample = false;
			}
		}

		internal static class DrawState {
			internal static bool SetLinear = true;
			internal static bool EnableMSAA = false;
			internal static bool DisableDepthBuffer = false;
			internal static SurfaceFormat BackbufferFormat = SurfaceFormat.Color;
		}

		internal static class Resample {
			internal static bool Smoothing = true;
			internal static bool Scale = Smoothing;
			internal const Upscaler.Scaler Scaler = Upscaler.Scaler.xBRZ;
			internal const bool EnableDynamicScale = true;
			internal static bool TrimWater = true;
			internal static float ScaleBias = 0.1f;
			internal static uint MaxScale = 6;
			internal static int MinimumTextureDimensions = 4;
			internal static bool EnableWrappedAddressing = true;
			internal static readonly List<SurfaceFormat> SupportedFormats = new List<SurfaceFormat>() {
				SurfaceFormat.Color,
				SurfaceFormat.Dxt5,
				SurfaceFormat.Dxt3,
				SurfaceFormat.Dxt1,
				SurfaceFormatExt.HasSurfaceFormat("Dxt1a") ? SurfaceFormatExt.GetSurfaceFormat("Dxt1a") : SurfaceFormat.Color
			};
			internal static class BlockCompression {
				internal static bool Enabled = DevEnabled && (!Runtime.IsMacintosh || MacSupported) && true; // I cannot build a proper libnvtt for OSX presently.
				[ConfigIgnore]
				private const bool MacSupported = false;
				private const bool DevEnabled = true;
				internal static bool Synchronized = false;
				internal static CompressionQuality Quality = CompressionQuality.Highest;
				internal static int HardAlphaDeviationThreshold = 7;
			}
			internal static List<string> Blacklist = new List<string>() {
				"LooseSprites/Lighting/"
			};
			internal static class Padding {
				internal static bool Enabled = DevEnabled && true;
				private const bool DevEnabled = true;
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
			internal const bool Enabled = true;
			internal static bool EnabledForUnknownTextures = false;
			internal static bool ForceSynchronousLoads = Runtime.IsUnix;
			internal static bool ThrottledSynchronousLoads = true;
			internal static bool CanFetchAndLoadSameFrame = true;
			internal static int MaxLoadsPerFrame = int.MaxValue;
			internal static long MinimumSizeTexels = 0;
		}

		internal static class MemoryCache {
			internal static bool Enabled = DevEnabled && true;
			private const bool DevEnabled = true;
			internal static bool AlwaysFlush = false;
			internal static Compression.Algorithm Compress = Compression.Algorithm.LZ;
			internal static bool Async = true;
		}

		internal static class FileCache {
			internal const bool Purge = false;
			internal static bool Enabled = DevEnabled && true;
			private const bool DevEnabled = true;
			internal const int LockRetries = 32;
			internal const int LockSleepMS = 32;
			internal static Compression.Algorithm Compress = Compression.Algorithm.LZ;
			internal static bool ForceCompress = false;
		}
	}
}
