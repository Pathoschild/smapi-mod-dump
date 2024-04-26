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

namespace Leclair.Stardew.Common.Types;

public class Cache<T, K> {

	private readonly Func<K, T> Getter;
	private readonly Func<K> KeyGetter;

#nullable disable
	private T LastValue;
	private K LastKey;
#nullable enable
	public bool Valid { get; private set; }

	public Cache(Func<K> keyGetter, Func<K, T> getter) {
		Getter = getter;
		KeyGetter = keyGetter;
	}

	public Cache(Func<K, T> getter, Func<K> keyGetter) {
		Getter = getter;
		KeyGetter = keyGetter;
	}

	public void Invalidate() {
		Valid = false;
	}

	public T Value {
		get {
			K key = KeyGetter();
			if (!Valid || (key == null ? LastKey != null : !key.Equals(LastKey))) {
				LastKey = key;
				LastValue = Getter(key);
				Valid = true;
			}

			return LastValue;
		}
	}
}
