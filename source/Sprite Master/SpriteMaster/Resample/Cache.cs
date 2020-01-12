using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpriteMaster.Resample {
	internal static class Cache {
		private static readonly string TextureCacheName = "TextureCache";
		private static readonly string JunctionCacheName = $"{TextureCacheName}_Current";
		private static readonly string AssemblyVersion = typeof(Upscaler).Assembly.GetName().Version.ToString();
		private static readonly string CacheName = $"{TextureCacheName}_{AssemblyVersion}";
		private static readonly string LocalDataPath = Path.Combine(Config.LocalRoot, CacheName);
		private static readonly string DumpPath = Path.Combine(LocalDataPath, "dump");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GetPath (params string[] path) {
			return Path.Combine(LocalDataPath, Path.Combine(path));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GetDumpPath (params string[] path) {
			return Path.Combine(DumpPath, Path.Combine(path));
		}

		private class CacheHeader {
			public string Assembly = AssemblyVersion;
			public ulong ConfigHash = SerializeConfig.Hash();
			public int RefScale;
			public Vector2I Size;
			public TextureFormat? Format;
			public Vector2B Wrapped;
			public Vector2I Padding;
			public Vector2I BlockPadding;
			public ulong DataHash;
			public uint DataLength;

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

			internal void Write (BinaryWriter writer) {
				foreach (var field in typeof(CacheHeader).GetFields()) {
					writer.Write(field.GetValue(this));
				}
			}

			internal void Validate (string path) {
				if (Assembly != AssemblyVersion) {
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

		private static ConcurrentDictionary<string, SaveState> SavingMap = new ConcurrentDictionary<string, SaveState>();

		internal static bool Fetch (
			string path,
			out int refScale,
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

			if (Config.Cache.Enabled && File.Exists(path)) {
				int retries = Config.Cache.LockRetries;

				while (retries-- > 0) {
					if (SavingMap.TryGetValue(path, out var state) && state != SaveState.Saved) {
						Thread.Sleep(Config.Cache.LockSleepMS);
						continue;
					}

					// https://stackoverflow.com/questions/1304/how-to-check-for-file-lock
					bool WasLocked (in IOException ex) {
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
							var dataLength = header.DataLength;
							var dataHash = header.DataHash;

							var remainingSize = reader.BaseStream.Length - reader.BaseStream.Position;
							if (remainingSize < header.DataLength) {
								throw new EndOfStreamException("Cache File is corrupted");
							}

							data = new byte[dataLength];

							foreach (int i in 0..data.Length) {
								data[i] = reader.ReadByte();
							}

							if (data.HashXX() != dataHash) {
								throw new IOException("Cache File is corrupted");
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
								Thread.Sleep(Config.Cache.LockSleepMS);
								break;
						}
					}
				}
			}
			return false;
		}

		internal static bool Save (
			string path,
			int refScale,
			Vector2I size,
			TextureFormat format,
			Vector2B wrapped,
			Vector2I padding,
			Vector2I blockPadding,
			byte[] data
		) {
			if (Config.Cache.Enabled) {
				if (!SavingMap.TryAdd(path, SaveState.Saving)) {
					return false;
				}

				ThreadPool.QueueUserWorkItem((object dataItem) => {
					var data = (byte[])dataItem;
					try {
						using (var writer = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))) {
							new CacheHeader() {
								RefScale = refScale,
								Size = size,
								Format = format,
								Wrapped = wrapped,
								Padding = padding,
								BlockPadding = blockPadding,
								DataLength = (uint)data.Length,
								DataHash = data.HashXX()
							}.Write(writer);

							foreach (var v in data) {
								writer.Write(v);
							}
						}
						SavingMap.TryUpdate(path, SaveState.Saved, SaveState.Saving);
					}
					catch {
						try { File.Delete(path); } catch { }
						SavingMap.TryRemove(path, out var _);
					}
				}, data);
			}
			return true;
		}

		enum LinkType : int {
			File = 0,
			Directory = 1
		}

		[DllImport("kernel32.dll")]
		static extern bool CreateSymbolicLink (string Link, string Target, LinkType Type);

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
							if (Config.Cache.Purge || (endPath != CacheName && endPath != JunctionCacheName)) {
								// If it doesn't match, it's outdated and should be deleted.
								Directory.Delete(directory, true);
							}
						}
						catch { /* Ignore failures */ }
					}
				}
			}
			catch { /* Ignore failures */ }

			if (Config.Cache.Enabled) {
				// Mark the directory as compressed because this is very space wasteful and we are currently not performing compression.
				// https://stackoverflow.com/questions/624125/compress-a-folder-using-ntfs-compression-in-net
				try {
					// Create the directory path
					Directory.CreateDirectory(LocalDataPath);

					if (Runtime.IsWindows) {
						var dir = new DirectoryInfo(LocalDataPath);
						if ((dir.Attributes & FileAttributes.Compressed) == 0) {
							var objectPath = $"Win32_Directory.Name='{dir.FullName.Replace("\\", @"\\").TrimEnd('\\')}'";
							using (var obj = new ManagementObject(objectPath)) {
								using (obj.InvokeMethod("Compress", null, null)) {
									// I don't really care about the return value, 
									// if we enabled it great but it can also be done manually
									// if really needed
								}
							}
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
