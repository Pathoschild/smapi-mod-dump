/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Types;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using Leclair.Stardew.Almanac.Models;

namespace Leclair.Stardew.Almanac.Managers;

internal class BookManager : BaseManager {

	public static readonly string AssetPath = PathUtilities.NormalizeAssetName("Mods/leclair.almanac/Books");

	#region Internals

	private CaseInsensitiveDictionary<Book>? RawBooks = null;
	private Dictionary<NamespaceId, Book>? ProcessedBooks = null;

	#endregion

	#region Constructor

	public BookManager(ModEntry mod) : base(mod) { }

	#endregion

	#region Event Handlers

	[Subscriber]
	private void OnAssetRequested(object sender, AssetRequestedEventArgs e) {
		if (!e.Name.IsEquivalentTo(AssetPath))
			return;

		e.LoadFrom(GetRawBooks, AssetLoadPriority.Low);
	}

	[Subscriber]
	private void OnAssetInvalidated(object sender, AssetsInvalidatedEventArgs e) {
		foreach (var name in e.Names) {
			if (name.IsEquivalentTo(AssetPath)) {
				ProcessedBooks = null;
				return;
			}
		}
	}

	#endregion

	#region Invalidation

	public void Invalidate() {
		RawBooks = null;
		ProcessedBooks = null;
		Mod.Helper.GameContent.InvalidateCache(AssetPath);
	}

	#endregion

	#region Data Access

	private Dictionary<string, Book> GetRawBooks() {
		if (RawBooks == null)
			DiscoverBooks();

		return RawBooks!;
	}

	public IEnumerable<Book> GetAllBooks() {
		return GetBooks().Values;
	}

	public IDictionary<NamespaceId, Book> GetBooks() {
		if (ProcessedBooks == null)
			ProcessBooks();

		return ProcessedBooks!;
	}

	public bool TryGetBook(string id, out Book? book) {
		return TryGetBook(new NamespaceId(id), out book);
	}

	public bool TryGetBook(NamespaceId id, out Book? book) {
		return GetBooks().TryGetValue(id, out book);
	}

	#endregion

	#region Raw Loading

	private void DiscoverBooks() {
		RawBooks = new CaseInsensitiveDictionary<Book>();

		// Make a list of all our content packs.
		var owned = Mod.Helper.ContentPacks.GetOwned().Select(cp => cp.Manifest.UniqueID);

		// Now scan every single mod.
		var mods = Mod.Helper.ModRegistry.GetAll();
		if (mods != null)
			foreach (var mod in mods) {
				if (mod == null)
					continue;

				DiscoverBooks(mod, owned.Contains(mod.Manifest.UniqueID));
			}

		Log($"Discovered {RawBooks.Count} books in data files.", LogLevel.Debug);
	}

	private void DiscoverBooks(IModInfo mod, bool owned) {
		string? path = null;

		// For our own Content Packs, books are located in Books/ rather than AlmanacBooks/
		if (owned)
			path = "Books";

		// For other mods, we need to detect an "Almanac:Books" key in their manifest before
		// we're willing to assume they have books.
		else if (mod.Manifest.ExtraFields != null && mod.Manifest.ExtraFields.TryGetValue("Almanac:Books", out object? value)) { 

			// For a boolean value, use the default path.
			if (value is bool bv) {
				if (!bv)
					return;

				path = "AlmanacBooks";

			// For a string value, use that path instead.
			} else if (value is string str)
				path = str;
		}

		// No path? No books. Leave!
		if (string.IsNullOrEmpty(path))
			return;

		// Alright. Is this a ContentPack?
		IContentPack? pack = null;
		string root;

		if (mod.IsContentPack && mod.GetType().GetProperty("ContentPack", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(mod) is IContentPack cp) { 
			pack = cp;
			root = cp.DirectoryPath;

		} else if (mod.GetType().GetProperty("DirectoryPath", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(mod) is string str) {
			root = str;

		} else {
			Log($"Unable to locate root directory of mod {mod.Manifest.UniqueID}", LogLevel.Warn);
			return;
		}

		string booksRoot = Path.Join(root, PathUtilities.NormalizePath(path));
		if (!Directory.Exists(booksRoot)) {
			Log($"Book subdirectory does not exist for mod {mod.Manifest.UniqueID}", owned ? LogLevel.Trace : LogLevel.Warn);
			return;
		}

		foreach (string bookDir in Directory.EnumerateDirectories(booksRoot)) {
			string id = Path.GetRelativePath(booksRoot, bookDir);
			string bookFile = Path.Join(bookDir, "book.json");
			if (!File.Exists(bookFile)) {
				Log($"Book subdirectory \"{id}\" has no book.json file.", LogLevel.Warn);
				continue;
			}

			// At this point, we definitely have a book file. Make sure we have
			// an IContentPack for reading its data.
			if (pack == null)
				pack = Mod.Helper.ContentPacks.CreateTemporary(
					directoryPath: root,
					id: $"{Mod.ModManifest.UniqueID}.books.{mod.Manifest.UniqueID}",
					name: mod.Manifest.Name,
					description: mod.Manifest.Description,
					author: mod.Manifest.Author,
					version: mod.Manifest.Version
				);

			string localBookFile = Path.Join(path, id, "book.json");

			Log($"Loading book \"{id}\" from {bookFile}", LogLevel.Trace);

			Book? book;
			try {
				book = pack.ReadJsonFile<Book>(localBookFile);
				if (book is null)
					throw new ArgumentNullException("book");

			} catch (Exception ex) {
				Log($"Error reading book file for \"{id}\" from {mod.Manifest.UniqueID}", LogLevel.Error, ex);
				continue;
			}

			// Set the book's ID based on its providing mod and folder, if an ID wasn't set.
			if (book.Id == null)
				book.Id = new NamespaceId(mod.Manifest.UniqueID, id);

			// Store the book.
			string bid = book.Id.ToString();
			if (RawBooks!.ContainsKey(bid))
				Log($"Found duplicate book for key \"{bid}\" from {mod.Manifest.UniqueID}", LogLevel.Warn);
			else
				RawBooks!.Add(bid, book);

			// Try to discover entries.
			foreach (string chapDir in Directory.EnumerateDirectories(bookDir)) {
				string cid = Path.GetRelativePath(bookDir, chapDir);
				string cfolder = cid;
				string chapFile = Path.Join(path, id, cfolder, "chapter.json");

				Chapter? chapter;

				if (pack.HasFile(chapFile)) {
					Log($"Loading chapter \"{bid}/{cid}\" from {chapFile}", LogLevel.Trace);

					try {
						chapter = pack.ReadJsonFile<Chapter>(chapFile);
					} catch (Exception ex) {
						Log($"Error reading chapter file for \"{bid}/{cid}\" from {mod.Manifest.UniqueID}", LogLevel.Error, ex);
						chapter = null;
					}

					if (chapter != null) {
						if (chapter.Id != null)
							cid = chapter.Id;
						else
							chapter.Id = cid;

						if (book.Chapters.ContainsKey(cid))
							Log($"Found duplicate chapter data for \"{bid}/{cid}\" from {mod.Manifest.UniqueID}", LogLevel.Warn);
						else
							book.Chapters.Add(cid, chapter);
					}
				}

				if (!book.Chapters.TryGetValue(cid, out chapter) || chapter == null) {
					Log($"Ignoring entries from unknown chapter \"{bid}/{cid}\". Are you missing a \"chapter.json\" file?", LogLevel.Warn);
					continue;
				}

				foreach(string file in Directory.EnumerateFiles(chapDir)) {
					if (!Path.GetExtension(file).Equals(".json"))
						continue;

					string eid = Path.GetFileNameWithoutExtension(file);
					if (eid.Equals("chapter") || eid.StartsWith('.'))
						continue;

					Log($"Loading entry \"{bid}/{cid}/{eid}\" from {file}", LogLevel.Trace);
					string entryFile = Path.Join(path, id, cfolder, $"{eid}.json");

					Entry? entry;
					try {
						entry = pack.ReadJsonFile<Entry>(entryFile);
						if (entry is null)
							throw new ArgumentNullException("entry");

					} catch (Exception ex) {
						Log($"Error reading entry file for \"{bid}/{cid}/{eid}\" from {mod.Manifest.UniqueID}", LogLevel.Error, ex);
						continue;
					}

					if (entry.Id != null)
						eid = entry.Id;
					else
						entry.Id = eid;

					if (chapter.Entries.ContainsKey(eid))
						Log($"Found duplicate entry data for \"{bid}/{cid}/{eid}\" from {mod.Manifest.UniqueID}", LogLevel.Warn);
					else
						chapter.Entries.Add(eid, entry);
				}
			}
		}
	}

	#endregion

	#region Processing

	private void ProcessBooks() {
		ProcessedBooks = new Dictionary<NamespaceId, Book>();
		var Extensions = new Dictionary<NamespaceId, List<Book>>();

		var rawBooks = Mod.Helper.GameContent.Load<CaseInsensitiveDictionary<Book>>(AssetPath);

		// Collect all the extensions first.
		foreach(var book in rawBooks) {
			if (book.Value.Extends != null) {
				if (Extensions.TryGetValue(book.Value.Extends, out var list))
					list.Add(book.Value);
				else
					Extensions.Add(book.Value.Extends, new List<Book> { book.Value });
			}
		}

		// Now, go over each individual book.
		foreach (var book in rawBooks) {
			// Skip extension books.
			if (book.Value.Extends != null)
				continue;

			// Process the book. If it fails, skip it.
			if (!ProcessBook(book.Key, book.Value, Extensions))
				continue;

			NamespaceId bookId = book.Value.Id!;
			if (!ProcessedBooks.ContainsKey(bookId))
				ProcessedBooks.Add(bookId, book.Value);
			else
				Log($"Attempted to register book with ID {bookId} but a book with that ID already exists. (raw key: {book.Key})", LogLevel.Warn);
		}
	}

	private bool ProcessBook(string key, Book book, Dictionary<NamespaceId, List<Book>> extensionMap) {
		// Before we do anything else, ensure this book has its ID set.
		if (book.Id == null)
			try {
				book.Id = new NamespaceId(key);
			} catch (Exception ex) {
				Log($"Unable to parse ID for book: {key}", LogLevel.Warn, ex);
				return false;
			}

		// Process the included content first.
		foreach(var pair in book.Chapters) {
			Chapter chp = pair.Value;
			if (chp.Id == null)
				chp.Id = pair.Key;
			else if (chp.Id != pair.Key)
				Log($"Chapter ID does not match key for {book.Id}/{chp.Id} (key: {pair.Key}). This is probably okay, but it's strange.", LogLevel.Debug);

			// Build the chapter's qualified ID.
			chp.QualifiedID = new NamespaceId(book.Id.Domain, $"{book.Id.Path}/{chp.Id}");

			foreach(var ep in chp.Entries) {
				Entry entry = ep.Value;
				if (entry.Id == null)
					entry.Id = ep.Key;
				else if (entry.Id != ep.Key)
					Log($"Entry ID does not match key for {chp.QualifiedID}/{entry.Id} (key: {ep.Key}). This is probably okay, but it's strange.", LogLevel.Debug);

				// Build the entry's qualified ID.
				entry.QualifiedId = new NamespaceId(chp.QualifiedID.Domain, $"{chp.QualifiedID.Path}/{entry.Id}");
			}
		}

		// Apply extension content.
		extensionMap.TryGetValue(book.Id, out List<Book>? extensions);
		if (extensions != null && book.AllowExtension) {

			// TODO: Merge extensions into the book.

		} else if (extensions != null) {
			Log($"Extensions present for book {book.Id}, but book does not allow extensions. Ignoring.", LogLevel.Warn);
		}

		// Disable entries, chapters, and books that have no content.
		bool had_content = false;
		foreach(var pair in book.Chapters) {
			Chapter chp = pair.Value;
			if (!chp.Enable)
				continue;

			bool chp_content = false;
			foreach(var ep in chp.Entries) {
				Entry entry = ep.Value;
				if (!entry.Enable)
					continue;

				if (entry.Pages == null || entry.Pages.Length == 0)
					entry.Enable = false;

				if (entry.Enable)
					chp_content = true;
			}

			if (!chp_content)
				chp.Enable = false;

			if (chp.Enable)
				had_content = true;
		}

		// If a book has no content, skip it completely.
		if (!had_content)
			return false;

		return true;
	}

	#endregion

}
