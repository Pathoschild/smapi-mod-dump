/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpriteMaster.Resample {
	internal static class Cache {
		private static readonly string TextureCacheName = "TextureCache";
		private static readonly string JunctionCacheName = $"{TextureCacheName}_Current";
		private static readonly Version AssemblyVersion = typeof(Upscaler).Assembly.GetName().Version;
		private static readonly Version RuntimeVersion = typeof(Runtime).Assembly.GetName().Version;
		private static readonly ulong AssemblyHash = AssemblyVersion.GetHashCode().Fuse(RuntimeVersion.GetHashCode()).Unsigned();
		private static readonly string CacheName = $"{TextureCacheName}_{AssemblyVersion}";
		private static readonly string LocalDataPath = Path.Combine(Config.LocalRoot, CacheName);
		private static readonly string DumpPath = Path.Combine(LocalDataPath, "dump");

		private static readonly bool SystemCompression = false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GetPath (params string[] path) => Path.Combine(LocalDataPath, Path.Combine(path));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GetDumpPath (params string[] path) => Path.Combine(DumpPath, Path.Combine(path));

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private class CacheHeader {
			public Compression.Algorithm Algorithm;
			public ulong Assembly = AssemblyHash;
			public ulong ConfigHash = SerializeConfig.GetWideHashCode();
			public uint RefScale;
			public Vector2I Size;
			public TextureFormat? Format;
			public Vector2B Wrapped;
			public Vector2I Padding;
			public Vector2I BlockPadding;
			public ulong DataHash;
			public uint UncompressedDataLength;
			public uint DataLength;

			private delegate void ReadValue (object obj, BinaryReader stream);
			private delegate void WriteValue (object obj, BinaryWriter stream);
			private static readonly uint HeaderSize;
			private static readonly ReadValue[] ReadValues;
			private static readonly WriteValue[] WriteValues;

			// Cache the serialization/deserialization commands so we don't have to do reflection every time.
			static CacheHeader () {
				uint headerSize = 0;

				var fields = typeof(CacheHeader).GetFields();
				ReadValues = new ReadValue[fields.Length];
				WriteValues = new WriteValue[fields.Length];
				uint i = 0;
				foreach (var field in fields) {
					headerSize += field.FieldType.GetSerializedSize();
					var reader = field.FieldType.GetReader();
					var writer = field.FieldType.GetWriter();

					ReadValues[i] = (obj, stream) => field.SetValue(obj, reader(stream));
					WriteValues[i] = (obj, stream) => writer(stream, field.GetValue(obj));
					++i;
				}

				HeaderSize = headerSize;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static CacheHeader Read (BinaryReader reader) {
				var newHeader = new CacheHeader();

				foreach (var setField in ReadValues) {
					setField(newHeader, reader);
				}

				return newHeader;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal void Write (BinaryWriter writer) {
				foreach (var writeField in WriteValues) {
					writeField(this, writer);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal void Validate (string path) {
				if (Assembly != AssemblyHash) {
					throw new IOException($"Texture Cache File out of date '{path}'");
				}

				if (!Format.HasValue) {
					throw new InvalidDataException("Illegal compression format in cached texture");
				}
			}
		}

		enum SaveState {
			Saving = 0,
			Saved = 1
		}

		private sealed class Profiler {
			private readonly object Lock = new object();
			private ulong FetchCount = 0L;
			private ulong SumFetchTime = 0L;
			private ulong StoreCount = 0L;
			private ulong SumStoreTime = 0L;

			internal ulong MeanFetchTime { get {
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

			internal Profiler() { }

			internal ulong AddFetchTime(ulong time) {
				lock(Lock) {
					++FetchCount;
					SumFetchTime += time;
					return SumFetchTime / FetchCount;
				}
			}

			internal ulong AddStoreTime(ulong time) {
				lock(Lock) {
					++StoreCount;
					SumStoreTime += time;
					return SumStoreTime / StoreCount;
				}
			}
		}
		private static readonly Profiler CacheProfiler = (Config.FileCache.Profile && Config.FileCache.Enabled) ? new Profiler() : null;

		private static readonly ConcurrentDictionary<string, SaveState> SavingMap = Config.FileCache.Enabled ? new() : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool Fetch (
			string path,
			out uint refScale,
			out Vector2I size,
			out TextureFormat format,
			out Vector2B wrapped,
			out Vector2I padding,
			out Vector2I blockPadding,
			out byte[] data
		) {
			refScale = 0;
			size = Vector2I.Zero;
			format = TextureFormat.Color;
			wrapped = Vector2B.False;
			padding = Vector2I.Zero;
			blockPadding = Vector2I.Zero;
			data = null;

			if (Config.FileCache.Enabled && File.Exists(path)) {
				int retries = Config.FileCache.LockRetries;

				while (retries-- > 0) {
					if (SavingMap.TryGetValue(path, out var state) && state != SaveState.Saved) {
						Thread.Sleep(Config.FileCache.LockSleepMS);
						continue;
					}

					// https://stackoverflow.com/questions/1304/how-to-check-for-file-lock
					static bool WasLocked (IOException ex) {
						var errorCode = Marshal.GetHRForException(ex) & ((1 << 16) - 1);
						return errorCode == 32 || errorCode == 33;
					}

					try {
						long start_time = Config.FileCache.Profile ? DateTime.Now.Ticks : 0L;

						using (var reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))) {
							var header = CacheHeader.Read(reader);
							header.Validate(path);

							refScale = header.RefScale;
							size = header.Size;
							format = header.Format.Value;
							wrapped = header.Wrapped;
							padding = header.Padding;
							blockPadding = header.BlockPadding;
							var uncompressedDataLength = header.UncompressedDataLength;
							var dataLength = header.DataLength;
							var dataHash = header.DataHash;

							var rawData = reader.ReadBytes((int)dataLength);

							if (rawData.HashXX() != dataHash) {
								throw new IOException($"Cache File '{path}' is corrupted");
							}

							data = Compression.Decompress(rawData, (int)uncompressedDataLength, header.Algorithm);
						}

						if (Config.FileCache.Profile) {
							var mean_ticks = CacheProfiler.AddFetchTime((ulong)(DateTime.Now.Ticks - start_time));
							Debug.InfoLn($"Mean Time Per Fetch: {(double)mean_ticks / TimeSpan.TicksPerMillisecond} ms");
						}
						
						return true;
					}
					catch (Exception ex) {
						switch (ex) {
							case FileNotFoundException _:
							case EndOfStreamException _:
							case IOException iox when !WasLocked(iox):
							default:
								ex.PrintWarning();
								try { File.Delete(path); } catch { }
								return false;
							case IOException iox when WasLocked(iox):
								Debug.TraceLn($"File was locked when trying to load cache file '{path}': {ex.Message} [{retries} retries]");
								Thread.Sleep(Config.FileCache.LockSleepMS);
								break;
						}
					}
				}
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool Save (
			string path,
			uint refScale,
			Vector2I size,
			TextureFormat format,
			Vector2B wrapped,
			Vector2I padding,
			Vector2I blockPadding,
			byte[] data
		) {
			if (!Config.FileCache.Enabled) {
				return true;
			}
			if (!SavingMap.TryAdd(path, SaveState.Saving)) {
				return false;
			}

			ThreadQueue.Queue((data) => {
				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
				using var _ = new AsyncTracker($"File Cache Write {path}");
				bool failure = false;
				try {
					long start_time = Config.FileCache.Profile ? DateTime.Now.Ticks : 0L;

					using (var writer = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))) {
						if (!writer.BaseStream.CanWrite) {
							failure = true;
							return;
						}
						var algorithm = (SystemCompression && !Config.FileCache.ForceCompress) ? Compression.Algorithm.None : Config.FileCache.Compress;

						var compressedData = Compression.Compress(data, algorithm);

						if (compressedData.Length >= data.Length) {
							compressedData = data;
							algorithm = Compression.Algorithm.None;
						}

						new CacheHeader() {
							Algorithm = algorithm,
							RefScale = refScale,
							Size = size,
							Format = format,
							Wrapped = wrapped,
							Padding = padding,
							BlockPadding = blockPadding,
							UncompressedDataLength = (uint)data.Length,
							DataLength = (uint)compressedData.Length,
							DataHash = compressedData.HashXX()
						}.Write(writer);

						writer.Write(compressedData);

						if (Config.FileCache.Profile) {
							writer.Flush();
						}
					}
					SavingMap.TryUpdate(path, SaveState.Saved, SaveState.Saving);

					if (Config.FileCache.Profile) {
						var mean_ticks = CacheProfiler.AddStoreTime((ulong)(DateTime.Now.Ticks - start_time));
						Debug.InfoLn($"Mean Time Per Store: {(double)mean_ticks / TimeSpan.TicksPerMillisecond} ms");
					}
				}
				catch (IOException ex) {
					Debug.TraceLn($"Failed to write texture cache file '{path}': {ex.Message}");
					failure = true;
				}
				catch (Exception ex) {
					Debug.WarningLn($"Internal Error writing texture cache file '{path}': {ex.Message}");
					failure = true;
				}
				if (failure) {
					try { File.Delete(path); } catch { }
					SavingMap.TryRemove(path, out var _);
				}
			}, data);
			return true;
		}

		enum LinkType : int {
			File = 0,
			Directory = 1
		}

		[DllImport("kernel32.dll")]
		static extern bool CreateSymbolicLink (string Link, string Target, LinkType Type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsSymbolic (string path) => new FileInfo(path).Attributes.HasFlag(FileAttributes.ReparsePoint);

		static Cache () {
			Config.FileCache.Compress = Compression.GetPreferredAlgorithm(Config.FileCache.Compress);

			// Delete any old caches.
			try {
				foreach (var root in new [] { Config.LocalRoot }) {
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
							if (Config.FileCache.Purge || (endPath != CacheName && endPath != JunctionCacheName)) {
								// If it doesn't match, it's outdated and should be deleted.
								Directory.Delete(directory, true);
							}
						}
						catch { /* Ignore failures */ }
					}
				}
			}
			catch { /* Ignore failures */ }

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
							SystemCompression = NTFS.CompressDirectory(LocalDataPath);
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
					Link: Path.Combine(Config.LocalRoot, JunctionCacheName),
					Target: Path.Combine(LocalDataPath),
					Type: LinkType.Directory
				);
			}
			catch { /* Ignore failure */ }
		}
	}
}
