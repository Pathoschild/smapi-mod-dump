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
using SpriteMaster.Hashing;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml;

namespace SpriteMaster.Harmonize.Patches.TMX;

internal static class TMXParser {
	#region By Path

	private static readonly ConcurrentDictionary<string, (DateTime Modified, object Result)> ParseFileCache = new();

	[Harmonize(
		"TMXTile.TMXParser",
		"Parse",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last
	)]
	public static bool ParsePre(object __instance, ref object __result, string path, ref DateTime __state) {
		if (!ParseFileCache.TryGetValue(path, out var cachedResult)) {
			return true;
		}

		var currentTime = File.GetLastWriteTimeUtc(path);
		__state = currentTime;
		if (cachedResult.Modified != currentTime) {
			return true;
		}

		__result = cachedResult.Result;
		return false;

	}

	[Harmonize(
		"TMXTile.TMXParser",
		"Parse",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.Last
	)]
	public static void ParsePost(object __instance, object __result, string path, DateTime __state) {
		var currentTime = __state;

		ParseFileCache.AddOrUpdate(path, _ => (currentTime, __result), (_, _) => (currentTime, __result));
	}

	#endregion

	#region By XML Reader

	private static readonly ConcurrentDictionary<ulong, object> ParseXmlCache = new();

	[Harmonize(
		"TMXTile.TMXParser",
		"Parse",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last
	)]
	public static bool ParsePre(object __instance, ref object __result, XmlReader reader, ref ulong __state) {
		var document = new XmlDocument();
		document.Load(reader);

		var xmlHash = document.OuterXml.GetSafeHash64();

		__state = xmlHash;

		if (!ParseXmlCache.TryGetValue(xmlHash, out var cachedResult)) {
			return true;
		}

		__result = cachedResult;
		return false;
	}

	[Harmonize(
		"TMXTile.TMXParser",
		"Parse",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.Last
	)]
	public static void ParsePost(object __instance, object __result, XmlReader reader, ulong __state) {
		ParseXmlCache.TryAdd(__state, __result);
	}

	#endregion

	#region By Stream

	private static readonly ConcurrentDictionary<ulong, object> ParseStreamCache = new();

	[Harmonize(
		"TMXTile.TMXParser",
		"Parse",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last
	)]
	public static bool ParsePre(object __instance, ref object __result, ref Stream stream, string path, ref ulong __state) {
		long currentPosition = stream.Position;
		
		using var copyStream = new MemoryStream();
		stream.CopyTo(copyStream);

		var dataHash = copyStream.Hash();

		__state = dataHash;

		if (stream.CanSeek) {
			stream.Seek(currentPosition, SeekOrigin.Begin);
		}
		else {
			stream = copyStream;
		}

		if (!ParseStreamCache.TryGetValue(dataHash, out var cachedResult)) {
			return true;
		}

		__result = cachedResult;
		return false;
	}

	[Harmonize(
		"TMXTile.TMXParser",
		"Parse",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.Last
	)]
	public static void ParsePost(object __instance, object __result, Stream stream, string path, ulong __state) {
		ParseStreamCache.TryAdd(__state, __result);
	}

	#endregion
}
