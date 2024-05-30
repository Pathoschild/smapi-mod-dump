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

using Leclair.Stardew.Common.Events;

using StardewModdingAPI;

namespace Leclair.Stardew.Common.Types;

// NOTE: MAKE SURE TO UPDATE THIS IN API DOCS IF YOU EVER CHANGE IT

/// <summary>
/// An <c>IModAssetEditor</c> is a special type of <see cref="IDictionary"/>
/// that works with SMAPI's API proxying to allow you to edit another
/// mod's data assets from a C# mod.
///
/// Unlike a normal dictionary, this custom <see cref="IDictionary"/> will
/// potentially throw <see cref="ArgumentException"/> when adding/assigning
/// values if they do not match the internal types.
///
/// To get around that, you are expected to use <see cref="GetOrCreate(string)"/>
/// and <see cref="Create{TValue}"/> to make instances using the correct
/// internal types, which you can then modify as needed.
/// </summary>
/// <typeparam name="TModel">An interface describing the internal model.</typeparam>
public interface IModAssetEditor<TModel> : IDictionary<string, TModel> {

	/// <summary>
	/// Get the data entry with the given key. If one does not exist, create
	/// a new entry, add it to the dictionary, and return that.
	/// </summary>
	/// <param name="key">The key to get an entry for.</param>
	TModel GetOrCreate(string key);

	/// <summary>
	/// Creates an instance of the provided type. This should be used to create
	/// instances of <typeparamref name="TValue"/>, where <typeparamref name="TValue"/>
	/// is an interface existing within <typeparamref name="TModel"/>.
	///
	/// For example, if <typeparamref name="TModel"/> has a property referencing a
	/// <c>ISomeOtherModel</c> and you need to create an instance of that, then
	/// you'll need to call <c>Create</c> with <typeparamref name="TValue"/> set to
	/// <c>ISomeOtherModel</c>.
	/// </summary>
	TValue Create<TValue>();

}

#if PINTAIL
public class ModAssetEditorValues<MType, TModel, TAbstract> : ICollection<TAbstract> where MType : PintailModSubscriber where TModel : TAbstract, new() {
#else
public class ModAssetEditorValues<MType, TModel, TAbstract> : ICollection<TAbstract> where MType : Mod where TModel : TAbstract, new() {
#endif

	private readonly ModAssetEditor<MType, TModel, TAbstract> Owner;

	public ModAssetEditorValues(ModAssetEditor<MType, TModel, TAbstract> owner) {
		Owner = owner;
	}

	public int Count => Owner.Data.Count;

	public bool IsReadOnly => false;

	public void Add(TAbstract item) {
		if (Owner.GetIdFunc is null)
			throw new InvalidOperationException("this asset editor does not support adding items without keys");

		if (item is not TModel titem)
			throw new ArgumentException("provided item is not of internal type; make sure to use Create() or GetOrCreate()");

		string key = Owner.GetIdFunc(titem);
		if (string.IsNullOrEmpty(key))
			throw new ArgumentNullException("key");

		Owner.Data.Add(key, titem);
	}

	public void Clear() {
		Owner.Data.Clear();
	}

	public bool Contains(TAbstract item) {
		if (item is not TModel titem)
			return false;

		return Owner.Data.Values.Contains(titem);
	}

	public void CopyTo(TAbstract[] array, int arrayIndex) {
		throw new NotImplementedException();
	}

	public IEnumerator<TAbstract> GetEnumerator() {
		foreach (var item in Owner.Data.Values)
			yield return item;
	}

	public bool Remove(TAbstract item) {
		return item is TModel titem && Owner.Data.Values.Remove(titem);
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}

#if PINTAIL
public class ModAssetEditor<MType, TModel, TAbstract> : IModAssetEditor<TAbstract> where MType : PintailModSubscriber where TModel : TAbstract, new() {
#else
public class ModAssetEditor<MType, TModel, TAbstract> : IModAssetEditor<TAbstract> where MType : Mod where TModel : TAbstract, new() {
#endif

	internal readonly MType Mod;
	internal readonly IManifest Other;

	internal readonly IDictionary<string, TModel> Data;
	internal readonly ModAssetEditorValues<MType, TModel, TAbstract> ValueGetter;

	internal readonly Action<TAbstract, string>? SetIdFunc;
	internal readonly Func<TAbstract, string>? GetIdFunc;

	internal readonly Dictionary<Type, Func<object?>> CreatableTypes;

#if PINTAIL
	internal readonly Dictionary<Type, Func<object?>?> MappedTypes = new();
#endif

	public ModAssetEditor(
		MType mod,
		IManifest other,
		IAssetData data,
		Func<TAbstract, string>? getIdFunc = null,
		Action<TAbstract, string>? setIdFunc = null,
		IEnumerable<KeyValuePair<Type, Func<object?>>>? creatableTypes = null
	) {
		Mod = mod;
		Other = other;
		Data = data.AsDictionary<string, TModel>().Data;
		ValueGetter = new(this);
		GetIdFunc = getIdFunc;
		SetIdFunc = setIdFunc;

		CreatableTypes = new();
		if (creatableTypes != null)
			foreach (var entry in creatableTypes)
				CreatableTypes.Add(entry.Key, entry.Value);

		if (!CreatableTypes.ContainsKey(typeof(TModel)) && !CreatableTypes.ContainsKey(typeof(TAbstract)))
			CreatableTypes.Add(typeof(TModel), () => CreateModel()!);
	}

	private bool TryUnbox(TAbstract input, [NotNullWhen(true)] out TModel? unboxed) {
		if (input is TModel tvalue) {
			unboxed = tvalue;
			return true;
		}

#if PINTAIL
		if (Mod.TryUnproxy(input, out object? result) && result is TModel tunboxed) {
			unboxed = tunboxed;
			return true;
		}
#endif

		unboxed = default;
		return false;
	}


	protected virtual TModel CreateModel() {
		return new TModel();
	}


	public virtual TValue Create<TValue>() {
#if PINTAIL
		Type ttype = typeof(TValue);

		// Cache our mapping work, since this can have a performance hit.
		if (!MappedTypes.TryGetValue(ttype, out var wanted)) {
			// The first time, try to map an exact name.
			foreach (var entry in CreatableTypes) {
				if (entry.Key.Name == ttype.Name && Mod.CanProxy(entry.Key, Mod.ModManifest.UniqueID, ttype, Other.UniqueID)) {
					wanted = entry.Value;
					break;
				}
			}

			// But if that doesn't work, try mapping everything else till
			// something works.
			if (wanted == null)
				foreach (var entry in CreatableTypes) {
					// We reverse name equality to avoid logging the same
					// error twice if there is a mapping problem, and to
					// avoid doing duplicate work in that same situation.
					if (entry.Key.Name != ttype.Name && Mod.CanProxy(entry.Key, Mod.ModManifest.UniqueID, ttype, Other.UniqueID)) {
						wanted = entry.Value;
						break;
					}
				}

			// Whether or not we got something, store it so we don't need to
			// do this again.
			MappedTypes[ttype] = wanted;
		}

		// Create a new instance and proxy it.
		if (wanted != null) {
			object? instance = wanted();
			// If wanted() returns null, we are opting out of creating this
			// specific interface, so we'll fall back to InvalidCastException.
			if (instance != null && Mod.TryProxy(instance, Mod.ModManifest.UniqueID, ttype, Other.UniqueID, out object? proxied))
				return (TValue) proxied;
		}
#else
		// Without Pintail access we don't know a gosh darned thing.
		if (typeof(TValue) == typeof(TAbstract) || typeof(TValue) == typeof(TModel))
			return (TValue) (object) CreateModel()!;
#endif
		throw new InvalidCastException($"Create<TValue>() does not support type {typeof(TValue).FullName}");
	}


	public TAbstract this[string key] {
		get => Data[key];
		set {
			if (!TryUnbox(value, out var tvalue))
				throw new ArgumentException("provided item is not of internal type; make sure to use Create() or GetOrCreate()");

			SetIdFunc?.Invoke(tvalue, key);
			Data[key] = tvalue;
		}
	}

	public ICollection<string> Keys => Data.Keys;

	public ICollection<TAbstract> Values => ValueGetter;

	public int Count => Data.Count;

	public bool IsReadOnly => false;

	public void Add(string key, TAbstract value) {
		if (!TryUnbox(value, out var tvalue))
			throw new ArgumentException("provided item is not of internal type; make sure to use Create() or GetOrCreate()");

		SetIdFunc?.Invoke(tvalue, key);
		Data.Add(key, tvalue);
	}

	public void Add(KeyValuePair<string, TAbstract> item) {
		if (!TryUnbox(item.Value, out var tvalue))
			throw new ArgumentException("provided item is not of internal type; make sure to use Create() or GetOrCreate()");

		SetIdFunc?.Invoke(tvalue, item.Key);
		Data.Add(item.Key, tvalue);
	}

	public void Clear() {
		Data.Clear();
	}

	public bool Contains(KeyValuePair<string, TAbstract> item) {
		if (item is not TModel tvalue)
			return false;
		return Data.TryGetValue(item.Key, out var stored) && EqualityComparer<TModel>.Default.Equals(stored, tvalue);
	}

	public bool ContainsKey(string key) {
		return Data.ContainsKey(key);
	}

	public void CopyTo(KeyValuePair<string, TAbstract>[] array, int arrayIndex) {
		throw new NotImplementedException();
	}

	public IEnumerator<KeyValuePair<string, TAbstract>> GetEnumerator() {
		foreach (var pair in Data)
			yield return new KeyValuePair<string, TAbstract>(pair.Key, pair.Value);
	}

	public TAbstract GetOrCreate(string key) {
		if (Data.TryGetValue(key, out var tvalue))
			return tvalue;

		tvalue = new TModel();
		SetIdFunc?.Invoke(tvalue, key);
		Data[key] = tvalue;
		return tvalue;
	}

	public bool Remove(string key) {
		return Data.Remove(key);
	}

	public bool Remove(KeyValuePair<string, TAbstract> item) {
		if (!TryUnbox(item.Value, out var tvalue))
			return false;

		return Data.TryGetValue(item.Key, out var existing) && EqualityComparer<TModel>.Default.Equals(existing, tvalue) && Data.Remove(item.Key);
	}

	public bool TryGetValue(string key, [MaybeNullWhen(false)] out TAbstract value) {
		if (Data.TryGetValue(key, out var existing)) {
			value = existing;
			return true;
		}

		value = default;
		return false;
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
