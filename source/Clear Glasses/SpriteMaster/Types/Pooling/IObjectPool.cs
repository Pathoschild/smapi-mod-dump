/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types.Pooling;

internal interface IObjectPool<T> where T : class, new() {
	int Count { get; }
	long Allocated { get; }

	T Get();

	IPooledObject<T> GetSafe();

	void Return(T value);
}

internal interface ISealedObjectPool<T, TPool> : IObjectPool<T> where T : class, new() where TPool : ISealedObjectPool<T, TPool> {
	IPooledObject<T> IObjectPool<T>.GetSafe() => GetSafe();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal sealed PooledObject<T, TPool> GetSafe(Action<T>? clear = null) {
		return new(Get(), this, clear);
	}
}
