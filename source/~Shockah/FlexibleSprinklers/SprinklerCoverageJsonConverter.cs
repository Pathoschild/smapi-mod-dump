/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shockah.Kokoro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.FlexibleSprinklers;

internal sealed class SprinklerCoverageJsonConverter : JsonConverter<ISet<IntPoint>>
{
	public override ISet<IntPoint>? ReadJson(JsonReader reader, Type objectType, ISet<IntPoint>? existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		JToken token = JToken.ReadFrom(reader);
		if (token is null)
			return null;

		// new format
		IList<string>? lineList = null;
		try
		{
			lineList = token.ToObject<IList<string>>();
		}
		catch
		{
		}
		if (lineList is not null)
		{
			HashSet<IntPoint> newPointSet = new();
			if (lineList.Count == 0)
				return newPointSet;

			if (lineList.Any(row => row.Length != lineList.Count))
				throw new ArgumentException($"Cannot deserialize a malformed grid.");

			for (int y = 0; y < lineList.Count; y++)
				for (int x = 0; x < lineList.Count; x++)
					if (lineList[y][x] != '.')
						newPointSet.Add(new(x - lineList.Count / 2, y - lineList.Count / 2));
			return newPointSet;
		}


		// old format
		ISet<IntPoint>? pointSet = null;
		try
		{
			pointSet = token.ToObject<ISet<IntPoint>>();
		}
		catch
		{
		}
		if (pointSet is not null)
			return pointSet;

		throw new ArgumentException("Could not deserialize.");
	}

	public override void WriteJson(JsonWriter writer, ISet<IntPoint>? value, JsonSerializer serializer)
	{
		if (value is null)
		{
			writer.WriteNull();
			return;
		}
		if (value.Count == 0)
		{
			writer.WriteStartArray();
			writer.WriteEndArray();
			return;
		}

		int max = value.Max(p => Math.Max(Math.Abs(p.X), Math.Abs(p.Y)));

		writer.WriteStartArray();
		for (int y = -max; y <= max; y++)
			writer.WriteValue(string.Join("", Enumerable.Range(-max, max * 2 + 1).Select(x => value.Contains(new(x, y)) ? "x" : ".")));
		writer.WriteEndArray();
	}
}