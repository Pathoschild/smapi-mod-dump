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

using Leclair.Stardew.CloudySkies.LayerData;
using Leclair.Stardew.Common.Serialization.Converters;
using Leclair.Stardew.Common.Types;

using Newtonsoft.Json;

namespace Leclair.Stardew.CloudySkies.Serialization;

public class LayerDataConverter : JsonConverter {

	private static readonly CaseInsensitiveDictionary<Type> Types = new();

	private static readonly DiscriminatingConverter<BaseLayerData> Converter;

	static LayerDataConverter() {
		Converter = new("Type", Types);
		Converter.PopulateTypes();
	}

	public static bool RegisterType(string key, Type type) {
		if (!type.IsAssignableFrom(typeof(BaseLayerData)))
			throw new InvalidCastException($"{type} is not a subclass of {typeof(BaseLayerData)}");

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
