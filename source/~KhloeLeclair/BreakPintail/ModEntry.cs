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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.BreakPintail;

public class ModEntry : Mod {

	public override void Entry(IModHelper helper) {
		
	}

	public override object? GetApi() {
		return new ModApi();
	}

}

public class ThingTwoImpl<TValue> : IThingTwo<TValue> {

	private readonly TValue? _Value;

	public ThingTwoImpl(TValue? value) {
		_Value = value;
	}

	public TValue? Value => _Value;

}

public class ThingOneImpl<TValue> : IThingOne<TValue> {

	private readonly Dictionary<string, TValue> _values = new();

	public void Add(string key, TValue value) {
		_values.Add(key, value);
	}

	public IReadOnlyDictionary<string, TValue> CalculatedValues => _values;

	public TValue this[string key] => _values[key];

	public IEnumerable<string> Keys => _values.Keys;

	public IEnumerable<TValue> Values => _values.Values;

	public int Count => _values.Count;

	public bool ContainsKey(string key) {
		return _values.ContainsKey(key);
	}

	public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() {
		return _values.GetEnumerator();
	}

	public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value) {
		return _values.TryGetValue(key, out value);
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return _values.GetEnumerator();
	}
}


public class ModApi : IBreakPintailApi {
	public IThingOne<IThingTwo<Texture2D>> GetThingGetter() {
		var result = new ThingOneImpl<IThingTwo<Texture2D>>();
		result.Add("test", new ThingTwoImpl<Texture2D>(Game1.staminaRect));
		return result;
	}
}
