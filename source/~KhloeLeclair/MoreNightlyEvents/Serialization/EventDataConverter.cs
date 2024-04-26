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

using Leclair.Stardew.Common.Serialization.Converters;
using Leclair.Stardew.Common.Types;
using Leclair.Stardew.MoreNightlyEvents.Models;

using Newtonsoft.Json;

namespace Leclair.Stardew.MoreNightlyEvents.Serialization;

public class EventDataConverter : JsonConverter {

	private static readonly CaseInsensitiveDictionary<Type> Types = new();

	private static readonly DiscriminatingConverter<BaseEventData> Converter;

	static EventDataConverter() {
		Converter = new("Type", Types);
		Converter.PopulateTypes();
	}

	public static bool RegisterType(string key, Type type) {
		if (!type.IsAssignableFrom(typeof(BaseEventData)))
			throw new InvalidCastException($"{type} is not a subclass of {typeof(BaseEventData)}");

		return Types.TryAdd(key, type);
	}

	public override bool CanConvert(Type objectType) {
		return Converter.CanConvert(objectType);
	}

	public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
		return Converter.ReadJson(reader, objectType, existingValue, serializer);
	}

	public override bool CanWrite => Converter.CanWrite;

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
		Converter.WriteJson(writer, value, serializer);
	}

}
