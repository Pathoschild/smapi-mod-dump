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

using Leclair.Stardew.GiantCropTweaks.Models;


using StardewModdingAPI;

namespace Leclair.Stardew.GiantCropTweaks;

public class ModApi : IGiantCropTweaks {

	internal readonly ModEntry Mod;
	internal readonly IManifest Other;

	public ModApi(ModEntry mod, IManifest other) {
		Mod = mod;
		Other = other;
	}

	public IExtraGiantCropData CreateNew() {
		return new ExtraGiantCropData();
	}

	public IEnumerable<KeyValuePair<string, IExtraGiantCropData>> GetData() {
		Mod.LoadCropData();
		foreach (var pair in Mod.CropData)
			yield return new(pair.Key, pair.Value);
	}

	public IExtraGiantCropDataEditor GetEditor(IAssetData assetData) {
		throw new NotImplementedException();
	}

	public bool TryGetData(string key, [NotNullWhen(true)] out IExtraGiantCropData? data) {
		Mod.LoadCropData();
		if (Mod.CropData.TryGetValue(key, out var result)) {
			data = result;
			return true;
		}

		data = null;
		return false;
	}
}


public class DataEditorValues : ICollection<IExtraGiantCropData> {

	internal readonly IDictionary<string, ExtraGiantCropData> Data;

	public DataEditorValues(IDictionary<string, ExtraGiantCropData> source) {
		Data = source;
	}

	public int Count => Data.Count;

	public bool IsReadOnly => false;

	public void Add(IExtraGiantCropData item) {
		if (item is not ExtraGiantCropData gcd)
			throw new ArgumentException("cannot use own types, must use API provided type");
		if (string.IsNullOrEmpty(gcd.Id))
			throw new ArgumentNullException(nameof(gcd.Id));
		Data.Add(gcd.Id, gcd);
	}

	public void Clear() {
		Data.Clear();
	}

	public bool Contains(IExtraGiantCropData item) {
		if (item is not ExtraGiantCropData gcd)
			return false;
		return Data.Values.Contains(gcd);
	}

	public void CopyTo(IExtraGiantCropData[] array, int arrayIndex) {
		throw new NotImplementedException();
	}

	public IEnumerator<IExtraGiantCropData> GetEnumerator() {
		foreach(var gcd in Data.Values)
			yield return gcd;
	}

	public bool Remove(IExtraGiantCropData item) {
		return item is ExtraGiantCropData gcd && Data.Values.Remove(gcd);
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}


public class DataEditor : IExtraGiantCropDataEditor {

	internal readonly IDictionary<string, ExtraGiantCropData> Data;
	internal readonly DataEditorValues ValueGetter;

	public DataEditor(IAssetData data) {
		Data = data.AsDictionary<string, ExtraGiantCropData>().Data;
		ValueGetter = new(Data);
	}

	public IExtraGiantCropData this[string key] {
		get => Data[key];
		set {
			if (value is not ExtraGiantCropData gcd)
				throw new ArgumentException("cannot use own types, must use API provided type");
			Data[key] = gcd;
		}
	}

	public ICollection<string> Keys => Data.Keys;

	public ICollection<IExtraGiantCropData> Values => ValueGetter;

	public int Count => Data.Count;

	public bool IsReadOnly => false;

	public void Add(string key, IExtraGiantCropData value) {
		if (value is not ExtraGiantCropData gcd)
			throw new ArgumentException("cannot use own types, must use API provided type");
		Data.Add(key, gcd);
	}

	public void Add(KeyValuePair<string, IExtraGiantCropData> item) {
		if (item.Value is not ExtraGiantCropData gcd)
			throw new ArgumentException("cannot use own types, must use API provided type");
		Data.Add(item.Key, gcd);
	}

	public void Clear() {
		Data.Clear();
	}

	public bool Contains(KeyValuePair<string, IExtraGiantCropData> item) {
		return Data.TryGetValue(item.Key, out var gcd) && gcd == item.Value;
	}

	public bool ContainsKey(string key) {
		return Data.ContainsKey(key);
	}

	public void CopyTo(KeyValuePair<string, IExtraGiantCropData>[] array, int arrayIndex) {
		throw new NotImplementedException();
	}

	public IEnumerator<KeyValuePair<string, IExtraGiantCropData>> GetEnumerator() {
		foreach (var pair in Data)
			yield return new(pair.Key, pair.Value);
	}

	public IExtraGiantCropData GetOrCreate(string key) {
		if (Data.TryGetValue(key, out var gcd))
			return gcd;

		gcd = new ExtraGiantCropData() {
			Id = key
		};
		Data[key] = gcd;
		return gcd;
	}

	public bool Remove(string key) {
		return Data.Remove(key);
	}

	public bool Remove(KeyValuePair<string, IExtraGiantCropData> item) {
		return Data.TryGetValue(item.Key, out var gcd) && gcd == item.Value && Data.Remove(item.Key);
	}

	public bool TryGetValue(string key, [MaybeNullWhen(false)] out IExtraGiantCropData value) {
		if (Data.TryGetValue(key, out var gcd)) {
			value = gcd;
			return true;
		}

		value = null;
		return false;
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
