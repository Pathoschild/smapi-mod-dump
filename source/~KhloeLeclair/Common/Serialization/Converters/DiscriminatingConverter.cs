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
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Leclair.Stardew.Common.Serialization.Converters;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class DiscriminatedType : Attribute {

	public string Key { get; }

	public DiscriminatedType(string key) {
		Key = key;
	}

}


public class DiscriminatingConverter<T> : JsonConverter where T : class {

	public readonly string Key;
	public readonly string? WildcardKey;
	private readonly Dictionary<string, Type> Types;

	public DiscriminatingConverter(string key, Dictionary<string, Type> types, string? wildcardKey = null) {
		Key = key;
		WildcardKey = wildcardKey;
		Types = types;
	}

	public void PopulateTypes(Assembly[]? assemblies = null) {
		var types =
			from a in (assemblies ?? [typeof(T).Assembly])
			from t in a.GetTypes()
			where typeof(T).IsAssignableFrom(t)
			let attrs = t.GetCustomAttributes<DiscriminatedType>(false).ToArray()
			where attrs != null && attrs.Length > 0
			select new { Type = t, Attributes = attrs };

		foreach (var entry in types) {
			foreach (var attr in entry.Attributes)
				Types.TryAdd(attr.Key, entry.Type);
		}
	}

	public override bool CanConvert(Type objectType) {
		return typeof(T).IsAssignableFrom(objectType);
	}

	public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {

		JObject obj = JObject.Load(reader);

		string? type = (string?) obj[Key];
		if (string.IsNullOrEmpty(type))
			throw new JsonReaderException($"Cannot read \"{Key}\" from {reader.TokenType} node (path: {reader.Path}).");

		if (!Types.TryGetValue(type, out Type? value) && (WildcardKey == null || !Types.TryGetValue(WildcardKey, out value)))
			throw new JsonReaderException($"Invalid type \"{type}\" read from \"{Key}\" of {reader.TokenType} node (path: {reader.Path}).");

		object? result = Activator.CreateInstance(value)
			?? throw new JsonReaderException($"Unable to create new instance of type {value.FullName ?? value.Name} (type: \"{type}\", path: {reader.Path}).");

		serializer.Populate(obj.CreateReader(), result);
		return result;
	}

	public override bool CanWrite => false;

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
		throw new NotImplementedException();
	}

}
