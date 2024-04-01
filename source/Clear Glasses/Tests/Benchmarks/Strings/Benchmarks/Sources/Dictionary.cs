/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using LinqFasterer;
using SpriteMaster;
using SpriteMaster.Extensions;
using System.IO.Compression;

namespace Benchmarks.Strings.Benchmarks.Sources;

public abstract class Dictionary : StringSource {
	private const int RandSeed = 0x13377113;

	private sealed class DictionaryReader : IDisposable {
		private readonly List<IDisposable> Disposables = new();
		internal readonly StreamReader Stream;

		internal DictionaryReader(string path) {
			var isZip = Path.GetExtension(path).EqualsInvariantInsensitive(".zip");

			try {
				var fileStream = File.OpenRead(path);
				Disposables.Add(fileStream);
				if (isZip) {
					ZipArchive zip = new(fileStream, ZipArchiveMode.Read, leaveOpen: false);
					Disposables.Add(zip);
					Disposables.Add(Stream = new(zip.Entries[0].Open()));
				}
				else {
					Disposables.Add(Stream = new(path));
				}
			}
			catch {
				foreach (var disposable in Disposables.ReverseF()) {
					disposable.Dispose();
				}
				Disposables.Clear();
				throw;
			}
		}

		public void Dispose() {
			foreach (var disposable in Disposables.ReverseF()) {
				disposable.Dispose();
			}
			Disposables.Clear();
		}
	}

	static Dictionary() {
		var random = new Random(RandSeed);

		var dictionary = Program.CurrentOptions?.Dictionary ?? Options.Default.Dictionary;

		string[] words;
		try {
			using var reader = new DictionaryReader(dictionary);
			words = reader.Stream.ReadToEnd().Replace('\r', '\n').Split('\n', StringSplitOptions.RemoveEmptyEntries);
		}
		catch (Exception ex) {
			ThrowHelper.ThrowException($"Failed to open dictionary file '{dictionary}'", ex);
			return;
		}

		AddSet(random, words);
	}
}
