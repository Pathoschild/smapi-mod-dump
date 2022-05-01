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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.Common.Serialization.Converters;

public class NamespaceIdConverter : JsonConverter {

	public override bool CanConvert(Type objectType) {
		return typeof(NamespaceId).IsAssignableFrom(objectType);
	}

	public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
		string path = reader.Path;
		switch (reader.TokenType) {
			case JsonToken.Null:
				return null;
			case JsonToken.String:
				return ReadString(JToken.Load(reader).Value<string>(), path);
			case JsonToken.StartObject:
				return ReadObject(JObject.Load(reader), path);
			default:
				throw new JsonReaderException($"Cannot parse NamespaceId from {reader.TokenType} node (path: {path}).");
		}
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
		throw new NotImplementedException();
	}

	private static NamespaceId? ReadString(string? value, string path) {
		if (string.IsNullOrEmpty(value))
			throw new JsonReaderException($"Cannot parse NamespaceId from string node (path: {path}).");

		return new NamespaceId(value);
	}

	private static NamespaceId? ReadObject(JObject obj, string tokenPath) {
		try {
			string? domain = obj.ValueIgnoreCase<string>("Domain");
			string? path = obj.ValueIgnoreCase<string>("Path");

			if (string.IsNullOrEmpty(domain))
				throw new ArgumentNullException(nameof(domain));

			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException(nameof(path));

			return new NamespaceId(domain, path);

		} catch (Exception ex) {
			throw new JsonReaderException($"Cannot parse NamespaceId from object node (path: {tokenPath}).", ex);
		}
	}

}
