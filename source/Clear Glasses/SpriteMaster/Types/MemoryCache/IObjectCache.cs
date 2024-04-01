/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SpriteMaster.Types.MemoryCache;

internal interface IObjectCache<TKey, TValue> :
	IDisposable, IAsyncDisposable, ICache
	where TKey : notnull where TValue : notnull {

	internal interface IValueGetter {
		internal TValue Invoke();
	}

	/// <summary>
	/// The name of the cache.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// The total byte-size of the cache.
	/// </summary>
	long TotalSize { get; }

	/// <summary>
	/// The entry count of the cache.
	/// </summary>
	[Pure]
	int Count { get; }

	[Pure, MustUseReturnValue]
	bool Contains(TKey key);

	/// <summary>
	/// Gets the value represented by the provided <paramref name="key"/>.
	/// <para>Returns an empty <see cref="Nullable{TValue}"/> if it did not exist.</para>
	/// </summary>
	[Pure, MustUseReturnValue]
	TValue? Get(TKey key);

	/// <summary>
	/// Tries to get the value represented by the provided <paramref name="key"/>.
	/// </summary>
	[Pure, MustUseReturnValue]
	bool TryGet(TKey key, [NotNullWhen(true)] out TValue? value);

	[MustUseReturnValue]
	bool TrySetDelegated<TValueGetter>(TKey key, TValueGetter valueGetter) where TValueGetter : struct, IValueGetter;

	[MustUseReturnValue]
	bool TrySet(TKey key, TValue value);

	/// <summary>
	/// Sets the value represented by the provided <paramref name="key"/>.
	/// <para>Returns <paramref name="value"/>.</para>
	/// </summary>
	[MustUseReturnValue]
	TValue Set(TKey key, TValue value);

	[MustUseReturnValue]
	TValue SetOrTouch(TKey key, TValue value);

	/// <summary>
	/// Updates the value represented by the provided <paramref name="key"/>.
	/// </summary>
	void SetFast(TKey key, TValue value);

	void SetOrTouchFast(TKey key, TValue value);

	/// <summary>
	/// Updates the value represented by the provided <paramref name="key"/>.
	/// <para>Returns the original represented value, or an empty <see cref="Nullable{TValue}"/> if it did not exist.</para>
	/// </summary>
	[MustUseReturnValue]
	TValue? Update(TKey key, TValue value);

	/// <summary>
	/// Removes the value represented by the provided <paramref name="key"/>.
	/// <para>Returns the represented value, or an empty <see cref="Nullable{TValue}"/> if it did not exist.</para>
	/// </summary>
	[MustUseReturnValue]
	TValue? Remove(TKey key);

	/// <summary>
	/// Removes the value represented by the provided <paramref name="key"/>.
	/// </summary>
	void RemoveFast(TKey key);

	/// <summary>
	/// Indicates to the cache that the value represented by this key may have been mutated.
	/// </summary>
	void Touch(TKey key);

	/// <summary>
	/// Trims the provided <paramref name="count">number</paramref> of elements from the cache.
	/// </summary>
	void Trim(int count);

	/// <summary>
	/// Trims the cache to the provided <paramref name="count">number</paramref> of elements.
	/// </summary>
	void TrimTo(int count);

	/// <summary>
	/// Removes all elements from the cache.
	/// </summary>
	void Clear();

	/// <summary>
	/// Removes all elements from the cache, and returns the number and size removed.
	/// </summary>
	[MustUseReturnValue]
	(ulong Count, ulong Size) ClearWithCount();
}
