/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pathoschild.Http.Client;

using Leclair.Stardew.ThemeManagerFontStudio.Models;
using StbTrueTypeSharp;
using Pathoschild.Http.Client.Internal;
using System.IO;
using System.Buffers.Text;
using System.Text.Unicode;

namespace Leclair.Stardew.ThemeManagerFontStudio.Sources;

public class GoogleFontSource : IFontSource {

	public static readonly Dictionary<string, string> VariantNames = new() {
		{ "100", "Thin" },
		{ "100italic", "Thin Italic" },
		{ "200", "ExtraLight" },
		{ "200italic", "ExtraLight Italic" },
		{ "300", "Light" },
		{ "300italic", "Light Italic" },
		{ "400", "Regular" },
		{ "400italic", "Italic" },
		{ "regular", "Regular" },
		{ "italic", "Italic" },
		{ "500", "Medium" },
		{ "500italic", "Medium Italic" },
		{ "600", "SemiBold" },
		{ "600italic", "SemiBold Italic" },
		{ "700", "Bold" },
		{ "700italic", "Bold Italic" },
		{ "800", "ExtraBold" },
		{ "800italic", "ExtraBold Italic" },
		{ "900", "Black" },
		{ "900italic", "Black Italic" }
	};

	private readonly ModEntry Mod;

	private string CachePath;

	private FluentClient Client;

	private Dictionary<string, WebFontEntry>? FontEntries;

	public GoogleFontSource(ModEntry mod) {
		Mod = mod;
		Client = new();

		CachePath = Path.Join(Path.GetTempPath(), "leclair.stardew.tmfontstudio", "GoogleCache");
		Directory.CreateDirectory(CachePath);

		Mod.Log($"[GoogleFontSource] Caching web resources to: {CachePath}", StardewModdingAPI.LogLevel.Debug);
	}

	private async Task LoadFontEntries() {
		/*lock(this) {
			if (FontEntries is not null)
				return;
		}*/

		var result = new Dictionary<string, WebFontEntry>(StringComparer.OrdinalIgnoreCase);

		WebFontResponse? response;

		try {
			response = await Client.GetAsync("https://www.googleapis.com/webfonts/v1/webfonts")
				.WithArgument("sort", "POPULARITY")
				.WithArgument("key", " AIzaSyA3kyWaHth4dfmiz5dT131fJh4-HHeeoI8")
				.As<WebFontResponse>();
		} catch {
			response = null;
		}

		if (response?.Items is not null)
			foreach(var entry in response.Items) {
				if (!string.IsNullOrWhiteSpace(entry.Family) && entry.Files is not null && entry.Files.Count > 0)
					result[entry.Family] = entry;
			}

		lock(this) {
			FontEntries = result;
		}
	}


	public async Task<IFontData?> GetFont(string uniqueId) {
		await LoadFontEntries();

		Dictionary<string, WebFontEntry>? entries;
		lock (this) {
			entries = FontEntries;
		}

		int idx = uniqueId.IndexOf("::");

		string? family;
		string? variant;

		if (idx == -1) {
			family = uniqueId;
			variant = "regular";
		} else {
			family = uniqueId[..idx];
			variant = uniqueId[(idx + 2)..];
		}

		if (entries is not null && entries.TryGetValue(family, out var value) && value.Files is not null && value.Files.TryGetValue(variant, out string? url))
			return new GoogleFontData(
				uniqueId: uniqueId,
				url: url,
				familyName: value.Family!,
				subfamilyName: variant
			);

		return null;
	}

	public string GetCacheName(string url, string family, string subfamily) {
		string b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
		char[] fileChars = $"{family} - {subfamily} - {b64}.ttf".ToCharArray();
		char[] invalid = Path.GetInvalidFileNameChars();

		for (int i = 0; i < fileChars.Length; i++) {
			char c = fileChars[i];
			if (invalid.Contains(c))
				fileChars[i] = '_';
		}

		return new string(fileChars);
	}

	public async Task<IFontData?> LoadFont(IFontData data) {
		if (data is not GoogleFontData gfd)
			return null;

		byte[]? bytes;
		int offset;
		List<NameEntry>? names;

		string file = GetCacheName(gfd.Url, gfd.FamilyName, gfd.SubfamilyName);
		string path = Path.Join(CachePath, file);
		bool cached = File.Exists(path);

		try {
			if (cached)
				bytes = await File.ReadAllBytesAsync(path);
			else
				bytes = await Client.GetAsync(gfd.Url).AsByteArray();

			offset = FontUtilities.GetOffsetForIndex(bytes, 0);
			names = FontUtilities.ReadFontNames(bytes, offset);
		} catch {
			return null;
		}

		if (offset == -1 || names is null)
			return null;

		if (!cached)
			try {
				await File.WriteAllBytesAsync(path, bytes);
			} catch {
				/* no-op */
			}

		string family = FontUtilities.GetFamilyName(names) ?? gfd.FamilyName;
		string subfamily = FontUtilities.GetSubfamilyName(names) ?? gfd.SubfamilyName;

		return new GoogleFontData(
			uniqueId: data.UniqueId,
			url: gfd.Url,
			familyName: family,
			subfamilyName: subfamily
		) {
			Data = bytes,
			DataIndex = 0,
			DataOffset = offset,
			IsLoaded = true
		};
	}


	public async IAsyncEnumerable<IFontData> GetAllFonts(IProgress<int> progress) {

		await LoadFontEntries();

		Dictionary<string, WebFontEntry>? entries;
		lock(this) {
			entries = FontEntries;
		}

		if (entries is null || entries.Count == 0)
			yield break;

		foreach(var entry in entries.Values) {
			if (string.IsNullOrEmpty(entry.Family) || entry.Files is null || entry.Files.Count == 0)
				continue;

			foreach(var file in entry.Files) {
				if (!VariantNames.TryGetValue(file.Key, out string? variant))
					variant = file.Key;

				string fname = GetCacheName(file.Value, entry.Family, variant);
				string path = Path.Join(CachePath, fname);
				bool cached = File.Exists(path);

				byte[]? bytes = null;
				int offset = 0;
				List<NameEntry>? names = null;

				if (cached) {
					try {
						bytes = await File.ReadAllBytesAsync(path);
						offset = FontUtilities.GetOffsetForIndex(bytes, 0);
						names = FontUtilities.ReadFontNames(bytes, offset);
					} catch {
						bytes = null;
						offset = 0;
						names = null;

						try {
							// Delete the bad cache entry.
							File.Delete(path);
						} catch {  /* no-op */ }
					}
				}

				yield return new GoogleFontData(
					uniqueId: $"{entry.Family}::{file.Key}",
					url: file.Value,
					familyName: entry.Family,
					subfamilyName: variant
				) {
					Data = bytes,
					DataIndex = 0,
					DataOffset = offset,
					IsLoaded = bytes is not null
				};
			}
		}
	}
}


public class GoogleFontData : IFontData {

	public string Source => "google";

	public string UniqueId { get; init; }

	public string Url { get; init; }

	public string FamilyName { get; init; }

	public string SubfamilyName { get; init; }

	public bool IsLoaded { get; init; }

	public StbTrueType.stbtt_fontinfo? Info { get; set; }

	public List<NameEntry>? Names { get; set; }

	public byte[]? Data { get; init; }

	public int DataOffset { get; init; }

	public int DataIndex { get; init; }

	public GoogleFontData(string uniqueId, string url, string familyName, string subfamilyName) {
		UniqueId= uniqueId;
		Url = url;
		FamilyName= familyName;
		SubfamilyName= subfamilyName;
	}

}


public class WebFontResponse {

	public string? Kind { get; set; }

	public WebFontEntry[]? Items { get; set; }

}

public class WebFontEntry {

	public string? Kind { get; set; }

	public string? Category { get; set; }
	public string? Family { get; set; }

	public string? LastModified { get; set; }
	public string? Version { get; set; }

	public string[]? Subsets { get; set; }
	public string[]? Variants { get; set; }

	public Dictionary<string, string>? Files { get; set; }

}
