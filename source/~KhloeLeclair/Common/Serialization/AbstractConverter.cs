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

using Leclair.Stardew.Common.Types;

using Newtonsoft.Json;

namespace Leclair.Stardew.Common.Serialization;

public class AbstractConverter<TReal, TAbstract> : JsonConverter where TReal : TAbstract, new() {

	public override bool CanConvert(Type objectType) {
		return typeof(TAbstract).IsAssignableFrom(objectType);
	}

	public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
		return serializer.Deserialize<TReal>(reader);
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
		throw new NotImplementedException("should never be used to write");
	}

	public override bool CanWrite => false;

}


public class AbstractListConverter<TReal, TAbstract> : JsonConverter where TReal : TAbstract, new() {

	public virtual IList<TAbstract> Create(int capacity) {
		return new List<TAbstract>(capacity);
	}

	public override bool CanConvert(Type objectType) {
		return typeof(List<TAbstract>).IsAssignableFrom(objectType);
	}

	public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
		var temp = serializer.Deserialize<TReal[]?>(reader);
		if (temp is null)
			return null;

		IList<TAbstract> result = Create(temp.Length);
		foreach (var entry in temp)
			result.Add(entry);

		return result;
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
		throw new NotImplementedException("should never be used to write");
	}

	public override bool CanWrite => false;

}


public class AbstractValueListConverter<TReal, TAbstract> : AbstractListConverter<TReal, TAbstract> where TReal : TAbstract, new() {

	public override IList<TAbstract> Create(int capacity) {
		return new ValueEqualityList<TAbstract>(capacity);
	}

}
