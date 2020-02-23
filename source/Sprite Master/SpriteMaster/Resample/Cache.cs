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
		internal static string GetPath (params string[] path) {
			return Path.Combine(LocalDataPath, Path.Combine(path));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GetDumpPath (params string[] path) {
			return Path.Combine(DumpPath, Path.Combine(path));
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private class CacheHeader {
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

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static CacheHeader Read (BinaryReader reader) {
				var newHeader = new CacheHeader();

				foreach (var field in typeof(CacheHeader).GetFields()) {
					field.SetValue(
						newHeader,
						reader.Read(field.FieldType)
					);
				}

				return newHeader;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal void Write (BinaryWriter writer) {
				foreach (var field in typeof(CacheHeader).GetFields()) {
					writer.Write(field.GetValue(this));
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

		private static readonly ConcurrentDictionary<string, SaveState> SavingMap = Config.FileCache.Enabled ? new ConcurrentDictionary<string, SaveState>() : null;

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
								throw new IOException("Cache File is corrupted");
							}

							if (dataLength == uncompressedDataLength) {
								data = rawData;
							}
							else {
								data = Compression.Decompress(rawData, Config.FileCache.Compress);
							}
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
				try {
					using (var writer = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))) {

						var algorithm = (SystemCompression && !Config.FileCache.ForceCompress) ? Compression.Algorithm.None : Config.FileCache.Compress;

						var compressedData = Compression.Compress(data, algorithm);

						if (compressedData.Length >= data.Length) {
							compressedData = data;
						}

						new CacheHeader() {
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
					}
					SavingMap.TryUpdate(path, SaveState.Saved, SaveState.Saving);
				}
				catch {
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
		private static bool IsSymbolic (string path) {
			var pathInfo = new FileInfo(path);
			return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
		}

		static Cache () {
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
				// Mark the directory as compressed because this is very space wasteful and we are currently not performing compression.
				// https://stackoverflow.com/questions/624125/compress-a-folder-using-ntfs-compression-in-net
				try {
					// Create the directory path
					Directory.CreateDirectory(LocalDataPath);
				}
				catch (Exception ex) {
					ex.PrintWarning();
				}
				try {
					if (Runtime.IsWindows) {
						SystemCompression = NTFS.CompressDirectory(LocalDataPath);
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
