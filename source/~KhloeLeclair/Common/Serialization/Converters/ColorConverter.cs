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

using Microsoft.Xna.Framework;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Leclair.Stardew.Common.Serialization.Converters;

public class ColorConverter : JsonConverter {
	public override bool CanConvert(Type objectType) {
		return
			typeof(Color?).IsAssignableFrom(objectType) ||
			typeof(Color).IsAssignableFrom(objectType);
	}

	public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
		string path = reader.Path;
		Color? result;
		switch (reader.TokenType) {
			case JsonToken.Null:
				result = null;
				break;
			case JsonToken.String:
				result = ReadString(JToken.Load(reader).Value<string>());
				break;
			case JsonToken.StartObject:
				result = ReadObject(JObject.Load(reader), path);
				break;
			default:
				throw new JsonReaderException($"Can't parse Color? from {reader.TokenType} node (path: {reader.Path}).");
		}

		if (typeof(Color?).IsAssignableFrom(objectType))
			return result;

		return result ?? Color.Transparent;
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
		if (value is not Color color) { 
			writer.WriteNull();
			return;
		}

		var jo = new JObject {
			{"R", color.R },
			{"G", color.G },
			{"B", color.B },
			{"A", color.A }
		};

		jo.WriteTo(writer);
	}

	private static Color? ReadString(string? value) {
		return CommonHelper.ParseColor(value);
	}

	private static Color? ReadObject(JObject? obj, string path) {
		if (obj is null)
			return null;

		try {
			if (!obj.TryGetValueIgnoreCase("R", out int R) ||
				!obj.TryGetValueIgnoreCase("G", out int G) ||
				!obj.TryGetValueIgnoreCase("B", out int B)
			)
				return null;

			if (obj.TryGetValueIgnoreCase("A", out int A))
				return new Color(R, G, B, A);

			return new Color(R, G, B);

		} catch(Exception ex) {
			throw new JsonReaderException($"Can't parse Color? from JSON object node (path: {path}).", ex);
		}
	}
}
