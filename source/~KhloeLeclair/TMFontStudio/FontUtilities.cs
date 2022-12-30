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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Leclair.Stardew.ThemeManagerFontStudio.Models;

using Microsoft.CodeAnalysis;

using SpriteFontPlus;

using StardewValley;

using StbTrueTypeSharp;

namespace Leclair.Stardew.ThemeManagerFontStudio;

public record struct NameEntry(
	PlatformId PlatformId,
	ushort EncodingId,
	int LanguageId,
	NameId NameId,
	string Value
);

public static class FontUtilities {

	public unsafe static int GetOffsetForIndex(byte[] data, int index) {
		if (data is null || data.Length == 0)
			return 0;

		fixed (byte* ptr = data) {
			return StbTrueType.stbtt_GetFontOffsetForIndex(ptr, index);
		}
	}

	public unsafe static StbTrueType.stbtt_fontinfo GetFontInfo(byte[] data, int offset) {
		if (data is null || data.Length == 0)
			throw new ArgumentNullException(nameof(data));
		if (offset < 0) throw new ArgumentOutOfRangeException(nameof(data));

		var info = new StbTrueType.stbtt_fontinfo();
		fixed (byte* ptr = data) {
			if (StbTrueType.stbtt_InitFont(info, ptr, offset) == 0)
				throw new Exception("Failed to initialize font.");
		}

		return info;
	}

	public unsafe static StbTrueType.stbtt_fontinfo GetFontInfo(IFontData data) {
		StbTrueType.stbtt_fontinfo? info = data.Info;
		if (info is null) {
			info = GetFontInfo(data.Data!, data.DataOffset);
			//data.Info = info;
		}

		return info;
	}

	public unsafe static TtfFontBakerResult BakeFont(IFontData data, float fontPixelHeight, int bitmapWidth, int bitmapHeight, IEnumerable<CharacterRange> characterRanges) {
		if (data is null || data.Data is null || data.Data.Length == 0)
			throw new ArgumentNullException(nameof(data));

		if (fontPixelHeight <= 0) throw new ArgumentOutOfRangeException(nameof(fontPixelHeight));
		if (bitmapWidth <= 0) throw new ArgumentOutOfRangeException(nameof(bitmapWidth));
		if (bitmapHeight <= 0) throw new ArgumentOutOfRangeException(nameof(bitmapHeight));
		if (!characterRanges.Any()) throw new ArgumentNullException(nameof(characterRanges));

		byte[] pixels;
		var glyphs = new Dictionary<int, GlyphInfo>();

		StbTrueType.stbtt_fontinfo info = GetFontInfo(data);

		float scaleFactor = StbTrueType.stbtt_ScaleForPixelHeight(info, fontPixelHeight);

		int ascent, descent, lineGap;
		StbTrueType.stbtt_GetFontVMetrics(info, &ascent, &descent, &lineGap);

		pixels = new byte[bitmapWidth * bitmapHeight];
		StbTrueType.stbtt_pack_context pc = new StbTrueType.stbtt_pack_context();

		fixed (byte* pixelsPtr = pixels) {

			StbTrueType.stbtt_PackBegin(pc, pixelsPtr, bitmapWidth, bitmapHeight, bitmapWidth, 1, null);

			foreach(var range in characterRanges) {
				if (range.Start > range.End)
					continue;

				var cd = GC.AllocateUninitializedArray<StbTrueType.stbtt_packedchar>(range.End - range.Start + 1);
				for(int i = 0; i < cd.Length; i++)
					cd[i] = new StbTrueType.stbtt_packedchar();

				fixed(StbTrueType.stbtt_packedchar* cdPtr = cd) {
					StbTrueType.stbtt_PackFontRange(pc, info.data, data.DataIndex, fontPixelHeight, range.Start, range.End - range.Start + 1, cdPtr);
				}

				for(int i = 0; i < cd.Length; i++) {
					float yOffset = cd[i].yoff;
					yOffset += ascent * scaleFactor;

					glyphs[(char) (i + range.Start)] = new GlyphInfo {
						X = cd[i].x0,
						Y = cd[i].y0,
						Width = cd[i].x1 - cd[i].x0,
						Height = cd[i].y1 - cd[i].y0,
						XOffset = (int) cd[i].xoff,
						YOffset = (int) Math.Round(yOffset),
						XAdvance = (int) Math.Round(cd[i].xadvance)
					};
				}
			}

			StbTrueType.stbtt_PackEnd(pc);
		}

		return new TtfFontBakerResult(glyphs, fontPixelHeight, pixels, bitmapWidth, bitmapHeight);
	}

	public unsafe static int[]? GetTTCOffsets(byte[]? ttcData) {
		if (ttcData is null || ttcData.Length < 16)
			return null;

		fixed(byte* data = ttcData) {
			int count = StbTrueType.stbtt_GetNumberOfFonts(data);
			if (count <= 0) return null;

			int[] result = GC.AllocateUninitializedArray<int>(count);
			for(int i = 0; i < count; i++) {
				result[i] = StbTrueType.stbtt_GetFontOffsetForIndex(data, i);
			}

			return result;
		}
	}

	public static List<NameEntry>? ReadFontNames(IFontData fontData) {
		if (fontData.Names is not null)
			return fontData.Names;

		var info = GetFontInfo(fontData);
		if (info is null)
			return null;

		var names = ReadFontNames(info);
		fontData.Names = names;
		return names;
	}

	public unsafe static List<NameEntry>? ReadFontNames(byte[] ttf, int offset) {
		if (ttf == null || ttf.Length == 0)
			throw new ArgumentNullException(nameof(ttf));

		StbTrueType.stbtt_fontinfo info = new StbTrueType.stbtt_fontinfo();
		fixed (byte* ptr = ttf) {
			if (StbTrueType.stbtt_InitFont(info, ptr, offset) == 0)
				throw new Exception("Failed to read font.");
		}

		return ReadFontNames(info);
	}

	public unsafe static List<NameEntry>? ReadFontNames(StbTrueType.stbtt_fontinfo info) {
		byte* data = info.data;
		uint tableOffset = StbTrueType.stbtt__find_table(data, (uint) info.fontstart, "name");
		if (tableOffset == 0)
			return null;

		int entries = StbTrueType.ttUSHORT(data + tableOffset + 2);
		int storageOffset = (int) (tableOffset + StbTrueType.ttUSHORT(data + tableOffset + 4));

		List<NameEntry>? result = null;

		for(int i = 0; i < entries; i++) {
			uint entryOffset = (uint) (tableOffset + 6 + 12 * i);

			ushort platformId = StbTrueType.ttUSHORT(data + entryOffset);
			ushort encodingId = StbTrueType.ttUSHORT(data + entryOffset + 2);
			ushort languageId = StbTrueType.ttUSHORT(data + entryOffset + 4);
			ushort nameId = StbTrueType.ttUSHORT(data + entryOffset + 6);
			ushort strLength = StbTrueType.ttUSHORT(data + entryOffset + 8);
			ushort strOffset = StbTrueType.ttUSHORT(data + entryOffset + 10);

			if (strLength <= 0)
				continue;

			switch((NameId) nameId) {
				case NameId.FamilyName:
				case NameId.SubfamilyName:
				case NameId.TypographicFamilyName:
				case NameId.TypographicSubfamilyName:
				case NameId.Description:
				case NameId.UniqueId:
				case NameId.SampleText:
					break;
				default:
					continue;
			}

			string decoded;

			try {
				decoded = UnicodeEncoding.BigEndianUnicode.GetString(data + storageOffset + strOffset, strLength);
			} catch {
				continue;
			}

			result ??= new();

			result.Add(new((PlatformId) platformId, encodingId, languageId, (NameId) nameId, decoded));
		}

		return result;
	}

	[return: NotNullIfNotNull("names")]
	public static IEnumerable<PlatformId>? GetPlatformIds(List<NameEntry>? names) {
		return names?.Select(x => x.PlatformId).Distinct();
	}

	[return: NotNullIfNotNull("names")]
	public static IEnumerable<int>? GetLanguageIds(List<NameEntry>? names, PlatformId? platform = null) {
		return names?.Where(x => platform is null || x.PlatformId == platform).Select(x => x.LanguageId).Distinct();
	}

	public static string? GetFamilyName(List<NameEntry>? names) {
		// First, try to get a Microsoft result using the current culture.
		string? result;

		if (CultureInfo.CurrentUICulture != CultureInfo.InvariantCulture) {
			result = GetMatchingEntry(names, nameId: NameId.TypographicFamilyName, platform: PlatformId.Microsoft, languageId: CultureInfo.CurrentUICulture.LCID);
			if (result is not null)
				return result;

			result = GetMatchingEntry(names, nameId: NameId.FamilyName, platform: PlatformId.Microsoft, languageId: CultureInfo.CurrentUICulture.LCID);
			if (result is not null)
				return result;
		}

		// Well, we tried. Just get anything then.
		result = GetMatchingEntry(names, nameId: NameId.TypographicFamilyName);
		if (result is not null)
			return result;

		result = GetMatchingEntry(names, nameId: NameId.FamilyName);
		if (result is not null)
			return result;

		return null;
	}

	public static string? GetSubfamilyName(List<NameEntry>? names) {
		// First, try to get a Microsoft result using the current culture.
		string? result;

		if (CultureInfo.CurrentUICulture != CultureInfo.InvariantCulture) {
			result = GetMatchingEntry(names, nameId: NameId.TypographicSubfamilyName, platform: PlatformId.Microsoft, languageId: CultureInfo.CurrentUICulture.LCID);
			if (result is not null)
				return result;

			result = GetMatchingEntry(names, nameId: NameId.SubfamilyName, platform: PlatformId.Microsoft, languageId: CultureInfo.CurrentUICulture.LCID);
			if (result is not null)
				return result;
		}

		// Well, we tried. Just get anything then.
		result = GetMatchingEntry(names, nameId: NameId.TypographicSubfamilyName);
		if (result is not null)
			return result;

		result = GetMatchingEntry(names, nameId: NameId.SubfamilyName);
		if (result is not null)
			return result;

		return null;
	}

	public static string? GetMatchingEntry(List<NameEntry>? names, NameId nameId, PlatformId? platform = null, ushort? encodingId = null, int? languageId = null) {
		if (names is not null)
			foreach(var entry in names) {
				if (platform != null && platform != entry.PlatformId)
					continue;
				if (encodingId != null && encodingId != entry.EncodingId)
					continue;
				if (languageId != null && languageId != entry.LanguageId)
					continue;

				if (entry.NameId == nameId)
					return entry.Value;
			}

		return null;
	}

}
