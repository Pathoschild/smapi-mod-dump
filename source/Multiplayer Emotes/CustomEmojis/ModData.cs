
using CustomEmojis.Framework.Utilities;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using StardewModdingAPI;
using System.Linq;
using CustomEmojis.Framework.Constants;
using System.Diagnostics;

namespace CustomEmojis {

	public class ModData {

		[JsonIgnore] // Unused variable
		public bool DataChanged { get; set; } = false;

		[JsonIgnore]
		public IModHelper ModHelper { get; set; }
		[JsonIgnore]
		public bool FilesChanged { get; set; } = false;
		[JsonIgnore]
		public string[] FileExtensionsFilter { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".gif" };
		public List<string> WatchedPaths { get; set; } = new List<string>();
		public Dictionary<string, string> FilesChecksums { get; set; } = new Dictionary<string, string>();

		public ModData() {
		}

		public ModData(IModHelper modHelper, string[] fileExtensionsFilter) {
			ModHelper = modHelper;
			FileExtensionsFilter = fileExtensionsFilter;
		}

		public bool ShouldSaveData() {
			return FilesChanged || DataChanged;
		}

		public bool ShouldGenerateTexture() {
			return FilesChanged || !File.Exists(Path.Combine(ModHelper.DirectoryPath, Sprites.CustomEmojis.AssetName));
		}

		public void ShouldGenerateTexture(bool generate) {
			FilesChanged = generate;
		}

		public bool Checksum() {
#if DEBUG
			Stopwatch sw = new Stopwatch();
			ModEntry.ModLogger.Log("[Checksum] Timer started.");
			sw.Start();
#endif
			if(!FilesChanged) {
				foreach(string path in WatchedPaths) {

					string absolutePath = Path.Combine(ModHelper.DirectoryPath, path);

					if(Directory.Exists(absolutePath)) {

						Dictionary<string, string> currentFilesChecksum = ModUtilities.GetFolderFilesHash(absolutePath, SearchOption.AllDirectories, FileExtensionsFilter);

						if(!EqualDictionaries(FilesChecksums, currentFilesChecksum)) {
							FilesChecksums = currentFilesChecksum;
							FilesChanged = true;
						}

					}

				}
			}
#if DEBUG
			sw.Stop();
			ModEntry.ModLogger.Log("[Checksum] Timer stoped.", $"Elapsed Time: {sw.Elapsed}");
#endif
			return FilesChanged;
		}

		private bool EqualDictionaries<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2) {

			// Check equal ammount of elements
			if(dict1.Count != dict2.Count) {
				return false;
			}

			foreach(KeyValuePair<TKey, TValue> pair in dict1) {

				if(dict2.TryGetValue(pair.Key, out TValue value)) {
					// Requires value to be equal
					if(!EqualityComparer<TValue>.Default.Equals(value, pair.Value)) {
						return false;
					}
				} else {
					// Requires key to be present
					return false;
				}

			}
			return true;
		}

	}

}
