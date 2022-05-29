/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Hashing;
using SpriteMaster.Resample;
using SpriteMaster.Tasking;
using SpriteMaster.Types;
using SpriteMaster.Types.Spans;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster.Caching;

[SuppressUnmanagedCodeSecurity]
internal static class FileCache {
	private const string TextureCacheName = "TextureCache";
	private const string JunctionCacheName = $"{TextureCacheName}_Current";
	private static readonly Version AssemblyVersion = typeof(Resampler).Assembly.GetName().Version!;
	private static readonly Version RuntimeVersion = typeof(Runtime).Assembly.GetName().Version!;
	private static readonly ulong AssemblyHash = AssemblyVersion.GetSafeHash().Fuse(RuntimeVersion.GetSafeHash()).Unsigned();
	private static readonly string CacheName = $"{TextureCacheName}_{AssemblyVersion}";
	private static readonly string LocalDataPath = Path.Combine(Config.LocalRoot, CacheName);
	private static readonly string DumpPath = Path.Combine(LocalDataPath, "dump");

	private static bool SystemCompression = false;

	private static readonly ThreadedTaskScheduler TaskScheduler = new(threadNameFunction: index => $"FileCache IO Thread #{index}", threadPriority: ThreadPriority.BelowNormal);
	private static readonly TaskFactory TaskFactory = new(TaskScheduler);
	internal static readonly ManualCondition Initialized = new(false);
	internal static readonly ReaderWriterLockSlim CurrentTaskLock = new(LockRecursionPolicy.SupportsRecursion);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string GetPath(params string[] path) => Path.Combine(LocalDataPath, Path.Combine(path));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string GetDumpPath(params string[] path) => Path.Combine(DumpPath, Path.Combine(path)).Replace('=', '_');

	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
	private struct CacheHeader {
		private readonly ulong Assembly = AssemblyHash;
		private readonly ulong ConfigHash = Serialize.ConfigHash;
		internal ulong DataHash = default;
		internal Vector2I Size = default;
		internal PaddingQuad Padding = default;
		internal Vector2I BlockPadding = default;
		internal TextureFormat Format = default;
		[MarshalAs(UnmanagedType.U4)]
		internal Compression.Algorithm Algorithm = default;
		internal uint Scale = default;
		internal uint UncompressedDataLength = default;
		internal uint DataLength = default;
		internal Scaler Scaler = Scaler.None;
		internal Vector2B Wrapped = default;
		internal bool Gradient = false;

		public CacheHeader() { }

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static CacheHeader Read(BinaryReader reader) {
			CacheHeader newHeader = new();
			var newHeaderSpan = MemoryMarshal.CreateSpan(ref newHeader, 1).Cast<CacheHeader, byte>();
			var readBytes = reader.Read(newHeaderSpan);
			if (readBytes != newHeaderSpan.Length) {
				throw new IOException("Cache File is corrupted");
			}

			return newHeader;
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal void Write(BinaryWriter writer) {
			var headerSpan = MemoryMarshal.CreateSpan(ref this, 1).Cast<CacheHeader, byte>();

			writer.Write(headerSpan);
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal void Validate(string path) {
			if (Assembly != AssemblyHash) {
				throw new IOException($"Texture Cache File out of date '{path}'");
			}

			if (Format == TextureFormat.None) {
				throw new InvalidDataException($"Illegal compression format in cached texture '{path}'");
			}
		}
	}

	private enum SaveState {
		Saving = 0,
		Saved = 1
	}

	private sealed class Profiler {
		private readonly object Lock = new();
		private ulong FetchCount = 0UL;
		private ulong SumFetchTime = 0UL;
		private ulong StoreCount = 0UL;
		private ulong SumStoreTime = 0UL;

		internal ulong MeanFetchTime {
			get {
				lock (Lock) {
					return SumFetchTime / FetchCount;
				}
			}
		}

		internal ulong MeanStoreTime {
			get {
				lock (Lock) {
					return SumStoreTime / StoreCount;
				}
			}
		}

		internal ulong AddFetchTime(ulong time) {
			lock (Lock) {
				++FetchCount;
				SumFetchTime += time;
				return SumFetchTime / FetchCount;
			}
		}

		internal ulong AddStoreTime(ulong time) {
			lock (Lock) {
				++StoreCount;
				SumStoreTime += time;
				return SumStoreTime / StoreCount;
			}
		}
	}
	private static readonly Profiler CacheProfiler = Config.FileCache.Profile && Config.FileCache.Enabled ? new() : null!;

	private static readonly ConcurrentDictionary<string, SaveState> SavingMap = Config.FileCache.Enabled ? new() : null!;

	internal static bool Fetch(
		string path,
		out uint scale,
		out Vector2I size,
		out TextureFormat format,
		out Vector2B wrapped,
		out PaddingQuad padding,
		out Vector2I blockPadding,
		out IScalerInfo? scalerInfo,
		out bool gradient,
		out ReadOnlySpan<byte> data
	) {
		scale = 0;
		size = Vector2I.Zero;
		format = TextureFormat.Color;
		wrapped = Vector2B.False;
		padding = PaddingQuad.Zero;
		blockPadding = Vector2I.Zero;
		scalerInfo = null;
		gradient = false;
		data = null;

		try {
			CurrentTaskLock.EnterReadLock();

			if (!Config.FileCache.Enabled || !File.Exists(path)) {
				return false;
			}

			int retries = Config.FileCache.LockRetries;

			while (retries-- > 0) {
				if (SavingMap.TryGetValue(path, out var state) && state != SaveState.Saved) {
					Thread.Sleep(Config.FileCache.LockSleepMS);
					continue;
				}

				// https://stackoverflow.com/questions/1304/how-to-check-for-file-lock
				static bool WasLocked(IOException ex) {
					var errorCode = Marshal.GetHRForException(ex) & (1 << 16) - 1;
					return errorCode is (32 or 33);
				}

				try {
					long startTime = Config.FileCache.Profile ? DateTime.Now.Ticks : 0L;

					Scaler scaler;
					using (var reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))) {
						var header = CacheHeader.Read(reader);
						header.Validate(path);

						scale = header.Scale;
						size = header.Size;
						format = header.Format;
						wrapped = header.Wrapped;
						padding = header.Padding;
						blockPadding = header.BlockPadding;
						scaler = header.Scaler;
						gradient = header.Gradient;
						var uncompressedDataLength = header.UncompressedDataLength;
						var dataLength = header.DataLength;
						var dataHash = header.DataHash;

						var rawData = reader.ReadBytes((int)dataLength);

						if (rawData.Hash() != dataHash) {
							throw new IOException($"Cache File '{path}' is corrupted");
						}

						data = rawData.Decompress((int)uncompressedDataLength, header.Algorithm);
					}

					if (Config.FileCache.Profile) {
						var meanTicks = CacheProfiler.AddFetchTime((ulong)(DateTime.Now.Ticks - startTime));
						Debug.Info($"Mean Time Per Fetch: {(double)meanTicks / TimeSpan.TicksPerMillisecond} ms");
					}

					scalerInfo = Resample.Scalers.IScaler.GetScalerInfo(scaler);

					return true;
				}
				catch (Exception ex) {
					switch (ex) {
						case IOException iox when !WasLocked(iox):
						default:
							ex.PrintWarning();
							try { File.Delete(path); }
							catch {
								// ignored
							}

							return false;
						case IOException iox when WasLocked(iox):
							Debug.Trace($"File was locked when trying to load cache file '{path}': {ex} - {ex.Message} [{retries} retries]");
							Thread.Sleep(Config.FileCache.LockSleepMS);
							break;
					}
				}
			}
			return false;
		}
		finally {
			CurrentTaskLock.ExitReadLock();
		}
	}

	internal static bool Save(
		string path,
		uint scale,
		Vector2I size,
		TextureFormat format,
		Vector2B wrapped,
		PaddingQuad padding,
		Vector2I blockPadding,
		IScalerInfo? scalerInfo,
		bool gradient,
		ReadOnlyPinnedSpan<byte> data
	) {
		if (!Config.FileCache.Enabled) {
			return true;
		}
		CurrentTaskLock.EnterReadLock();
		try {
			if (!SavingMap.TryAdd(path, SaveState.Saving)) {
				return false;
			}

			TaskFactory.StartNew(obj => {
				CurrentTaskLock.EnterReadLock();
				try {
					var data = ((ReadOnlyPinnedSpan<byte>.FixedSpan)obj!).AsSpan;
					bool failure = false;
					try {
						long startTime = Config.FileCache.Profile ? DateTime.Now.Ticks : 0L;

						using (var writer = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))) {
							if (!writer.BaseStream.CanWrite) {
								return;
							}
							var algorithm = SystemCompression && !Config.FileCache.ForceCompress ? Compression.Algorithm.None : Config.FileCache.Compress;

							ReadOnlySpan<byte> compressedData = data.Compress(algorithm);

							if (compressedData.Length >= data.Length) {
								compressedData = data;
								algorithm = Compression.Algorithm.None;
							}

							new CacheHeader() {
								Algorithm = algorithm,
								Scale = scale,
								Size = size,
								Format = format,
								Wrapped = wrapped,
								Padding = padding,
								BlockPadding = blockPadding,
								UncompressedDataLength = (uint)data.Length,
								DataLength = (uint)compressedData.Length,
								DataHash = compressedData.Hash(),
								Scaler = scalerInfo?.Scaler ?? Scaler.None,
								Gradient = gradient
							}.Write(writer);

							writer.Write(compressedData);

							if (Config.FileCache.Profile) {
								writer.Flush();
							}
						}
						SavingMap.TryUpdate(path, SaveState.Saved, SaveState.Saving);

						if (Config.FileCache.Profile) {
							var meanTicks = CacheProfiler.AddStoreTime((ulong)(DateTime.Now.Ticks - startTime));
							Debug.Info($"Mean Time Per Store: {(double)meanTicks / TimeSpan.TicksPerMillisecond} ms");
						}
					}
					catch (IOException ex) {
						Debug.Trace($"Failed to write texture cache file '{path}': {ex.Message}");
						failure = true;
					}
					catch (Exception ex) {
						Debug.Warning($"Internal Error writing texture cache file '{path}': {ex} - {ex.Message}");
						failure = true;
					}
					if (failure) {
						try { File.Delete(path); }
						catch {
							// ignored
						}

						SavingMap.TryRemove(path, out _);
					}
				}
				finally {
					CurrentTaskLock.ExitReadLock();
				}
			}, data.Fixed);
			return true;
		}
		finally {
			CurrentTaskLock.ExitReadLock();
		}
	}

	internal static void Purge(bool reset = false) {
		var configState = Config.FileCache.Enabled;
		Config.FileCache.Enabled = false;
		CurrentTaskLock.EnterWriteLock();
		try {
			// TODO : This isn't perfect as there's a race condition for when tasks are _just_ started.

			SavingMap.Clear();
			DeleteCaches(null);
			BuildCache();

		}
		finally {
			CurrentTaskLock.ExitWriteLock();
			Config.FileCache.Enabled = configState;
		}
	}

	private enum LinkType {
		File = 0,
		Directory = 1
	}

	[DllImport("kernel32.dll")]
	private static extern bool CreateSymbolicLink(string link, string target, LinkType type);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool IsSymbolic(string path) => new FileInfo(path).Attributes.HasFlag(FileAttributes.ReparsePoint);

	private static void DeleteCaches(string? retain) {
		try {
			foreach (var root in new[] { Config.LocalRoot }) {
				var directories = Directory.EnumerateDirectories(root);
				foreach (var directory in directories) {
					try {
						if (!Directory.Exists(directory)) {
							continue;
						}
						if (IsSymbolic(directory)) {
							continue;
						}
						var endPath = Path.GetFileName(directory);
						if (Config.FileCache.Purge || (endPath != retain && endPath != JunctionCacheName)) {
							// If it doesn't match, it's outdated and should be deleted.
							Directory.Delete(directory, true);
						}
					}
					catch { /* Ignore failures */ }
				}
			}
		}
		catch { /* Ignore failures */ }
	}

	private static void BuildCache() {
		if (Config.FileCache.Enabled) {
			try {
				// Create the directory path
				Directory.CreateDirectory(LocalDataPath);
			}
			catch (Exception ex) {
				ex.PrintWarning();
			}
			try {
				if (Runtime.IsWindows) {
					// Use System compression if it is preferred and no other compression algorithm is supported for some reason.
					// https://stackoverflow.com/questions/624125/compress-a-folder-using-ntfs-compression-in-net
					if (Config.FileCache.PreferSystemCompression || (int)Config.FileCache.Compress <= (int)Compression.Algorithm.Deflate) {
						SystemCompression = DirectoryExt.CompressDirectory(LocalDataPath);
					}
				}
			}
			catch (Exception ex) {
				ex.PrintWarning();
			}
		}

		try {
			Directory.CreateDirectory(LocalDataPath);
			if (Config.Debug.Sprite.DumpReference || Config.Debug.Sprite.DumpResample) {
				Directory.CreateDirectory(DumpPath);
			}
		}
		catch (Exception ex) {
			ex.PrintWarning();
		}

		// Set up a symbolic link to aid in debugging.
		try {
			Directory.Delete(Path.Combine(Config.LocalRoot, JunctionCacheName), false);
		}
		catch { /* Ignore failure */ }
		try {
			CreateSymbolicLink(
				link: Path.Combine(Config.LocalRoot, JunctionCacheName),
				target: Path.Combine(LocalDataPath),
				type: LinkType.Directory
			);
		}
		catch { /* Ignore failure */ }
	}

	static FileCache() {
		try {
			Config.FileCache.Compress = Compression.GetPreferredAlgorithm(Config.FileCache.Compress);

			// Delete any old caches.
			DeleteCaches(CacheName);

			BuildCache();
		}
		finally {
			Initialized.Set();
		}
	}
}
