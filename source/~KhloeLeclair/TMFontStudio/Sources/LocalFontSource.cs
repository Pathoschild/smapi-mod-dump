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
using System.IO;
using System.Threading.Tasks;

using Leclair.Stardew.ThemeManagerFontStudio.Models;

using StbTrueTypeSharp;

namespace Leclair.Stardew.ThemeManagerFontStudio.Sources;

public class LocalFontSource : IFontSource {

	public async Task<IFontData?> GetFont(string uniqueId) {
		int index = 0;
		int idx = uniqueId.IndexOf("::");
		if (idx != -1) {
			index = int.Parse(uniqueId[(idx + 2)..]);
			uniqueId = uniqueId[..idx];
		}

		if (!File.Exists(uniqueId))
			return null;

		byte[]? data;

		try {
			data = await File.ReadAllBytesAsync(uniqueId);
		} catch {
			return null;
		}

		if (data is null || data.Length == 0)
			return null;

		return LoadFont(uniqueId, data, index, null);
	}

	public Task<IFontData?> LoadFont(IFontData data) {
		// Our fonts should always be loaded. Ignore this weirdness.
		return Task.FromResult<IFontData?>(null);
	}

	private LocalFontData? LoadFont(string uniqueId, byte[] data, int index, int? offset = null) {
		List<NameEntry>? names;

		if (offset is null)
			try {
				offset = FontUtilities.GetOffsetForIndex(data, index);
			} catch {
				return null;
			}

		StbTrueType.stbtt_fontinfo? info;

		try {
			info = FontUtilities.GetFontInfo(data, offset.Value);
			names = FontUtilities.ReadFontNames(info);
		} catch {
			names = null;
		}

		if (names is null)
			return null;

		string family = FontUtilities.GetFamilyName(names) ?? Path.GetFileNameWithoutExtension(uniqueId);
		string subfamily = FontUtilities.GetSubfamilyName(names) ?? "Regular";

		return new LocalFontData(
			uniqueId: offset > 0 ? $"{uniqueId}::{index}" : uniqueId,
			familyName: family,
			subfamilyName: subfamily,
			data: data,
			dataIndex: index,
			dataOffset: offset.Value,
			names: names
		);
	}

	public async IAsyncEnumerable<IFontData> GetAllFonts(IProgress<int> progress) {

		// TODO: Different platforms than Windows.

		string[]? folders = new string[] {
			Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
			Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "Fonts")
		};

		foreach(string folder in folders) {
			string path = Environment.ExpandEnvironmentVariables(folder);
			if (!Directory.Exists(path))
				continue;

			if (Directory.Exists(path))
				foreach (string file in Directory.EnumerateFiles(path)) {
					string? extension = Path.GetExtension(file);
					if (extension == ".fon")
						continue;

					byte[]? data;
					try {
						data = await File.ReadAllBytesAsync(file);
					} catch { data = null; }

					if (data is null || data.Length == 0)
						continue;

					int[]? offsets;
					try {
						offsets = FontUtilities.GetTTCOffsets(data);
					} catch {
						continue;
					}

					if (offsets is not null)
						for(int i = 0; i < offsets.Length; i++) {
							int offset = offsets[i];
							if (offset != -1) {
								LocalFontData? font = LoadFont(file, data, i, offset);
								if (font is not null)
									yield return font;
							}
						}
				}
		}
	}

}

public class LocalFontData : IFontData {

	public string Source => "local";

	public string UniqueId { get; init; }

	public string FamilyName { get; init; }

	public string SubfamilyName { get; init; }

	public bool IsLoaded => true;

	public StbTrueType.stbtt_fontinfo? Info { get; set; }

	public List<NameEntry>? Names { get; set; }

	public byte[]? Data { get; init; }

	public int DataOffset { get; init; }

	public int DataIndex { get; init; }

	public LocalFontData(string uniqueId, string familyName, string subfamilyName, byte[]? data, int dataIndex, int dataOffset, List<NameEntry>? names) {
		UniqueId = uniqueId;
		FamilyName = familyName;
		SubfamilyName = subfamilyName;
		Data = data;
		DataIndex = dataIndex;
		DataOffset = dataOffset;
		Names = names;
	}

}
