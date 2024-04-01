/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster.Types;

internal sealed class DoubleBuffer<T> {
	private const uint StartingIndex = 0U;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static uint GetIndex(uint index) => index & 1U;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private T GetBuffer(uint index) => (GetIndex(index) == 0U) ? Buffer0 : Buffer1;

	// Regarding bounds checking on x86 and x64
	// https://stackoverflow.com/questions/16713076/array-bounds-check-efficiency-in-net-4-and-above
	private readonly T Buffer0;
	private readonly T Buffer1;
	private readonly T[] UnorderedBothArray;

	private uint CurrentBufferIndex = StartingIndex;

	internal uint CurrentBuffer => GetIndex(CurrentBufferIndex);

	internal uint NextBuffer => GetIndex(CurrentBufferIndex + 1U);

	internal T Current => GetBuffer(CurrentBufferIndex);

	internal T Next => GetBuffer(CurrentBufferIndex + 1U);
	internal T this[int index] => GetBuffer((uint)index);
	internal T this[uint index] => GetBuffer(index);

	internal ReadOnlySpan<T> UnorderedBoth => UnorderedBothArray;

	internal (T, T) Both {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get {
			var currentIndex = CurrentBufferIndex;
			return (
				GetBuffer(currentIndex),
				GetBuffer(currentIndex + 1U)
			);
		}
	}



	internal (T, T) All => Both;
	internal DoubleBuffer(in T element0, in T element1) {
		Buffer0 = element0;
		Buffer1 = element1;
		UnorderedBothArray = new[] { element0, element1 };
		Thread.MemoryBarrier();
	}

	internal DoubleBuffer(T[] elements) : this(
		elements[0],
		elements[1]
	) { }

	internal DoubleBuffer(params object[] parameters) : this(
		// We do, indeed, want to create two separate instances.
#pragma warning disable CS0618 // Type or member is obsolete
		ReflectionExt.CreateInstance<T>(parameters) ?? throw new NullReferenceException(nameof(parameters)),
		ReflectionExt.CreateInstance<T>(parameters) ?? throw new NullReferenceException(nameof(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
	) { }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void Swap() => Interlocked.Increment(ref CurrentBufferIndex);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator T(in DoubleBuffer<T> buffer) => buffer.Current;
}
