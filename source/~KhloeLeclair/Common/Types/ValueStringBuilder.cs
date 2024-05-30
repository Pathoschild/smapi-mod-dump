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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Leclair.Stardew.Common.Types;

internal ref struct ValueStringBuilder {

	private Span<char> _chars;
	private int _pos;

	public ValueStringBuilder(Span<char> initialBuffer) {
		_chars = initialBuffer;
		_pos = 0;
	}

	public int Length {
		get => _pos;
		set {
			Debug.Assert(value >= 0);
			Debug.Assert(value <= _chars.Length);
			_pos = value;
		}
	}

	public int Capacity => _chars.Length;

	public void Clear() {
		_pos = 0;
	}

	public void EnsureCapacity(int capacity) {
		Debug.Assert(capacity >= 0);

		if ((uint) capacity > (uint) _chars.Length)
			Grow(capacity - _pos);
	}

	public ref char this[int index] {
		get {
			Debug.Assert(index < _pos);
			return ref _chars[index];
		}
	}

	public override string ToString() {
		return _chars.Slice(0, _pos).ToString();
	}

	public ReadOnlySpan<char> AsSpan() => _chars.Slice(0, _pos);
	public ReadOnlySpan<char> AsSpan(int start) => _chars.Slice(start, _pos - start);
	public ReadOnlySpan<char> AsSpan(int start, int length) => _chars.Slice(start, length);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(char c) {
		if (_pos >= _chars.Length)
			Grow(1);

		_chars[_pos] = c;
		_pos++;
	}

	public void Append(ReadOnlySpan<char> value) {
		if (_pos > _chars.Length - value.Length)
			Grow(value.Length);

		value.CopyTo(_chars.Slice(_pos));
		_pos += value.Length;
	}

	//[MethodImpl(MethodImplOptions.NoInlining)]
	private void Grow(int additionalCapacityBeyondPos) {
		Debug.Assert(additionalCapacityBeyondPos > 0);
		Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

		// Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative
		char[] poolArray = new char[(int) Math.Max((uint) (_pos + additionalCapacityBeyondPos), (uint) _chars.Length * 2)];

		_chars.Slice(0, _pos).CopyTo(poolArray);
		_chars = poolArray;
	}

}
