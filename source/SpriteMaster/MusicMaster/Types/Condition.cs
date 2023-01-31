/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using MusicMaster.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MusicMaster.Types;

internal sealed class Condition : IDisposable {
	private volatile int State = 0;
	private AutoResetEvent? Event = new(false);

	internal Condition(bool initialState = false) => State = initialState.ToInt();

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator bool(Condition condition) => condition.State.ToBool();

	// This isn't quite thread-safe, but the granularity of this in our codebase is really loose to begin with. It doesn't need to be entirely thread-safe.
	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool Wait() {
		Event!.WaitOne();
		return State.ToBool();
	}

	// This isn't quite thread-safe, but the granularity of this in our codebase is really loose to begin with. It doesn't need to be entirely thread-safe.
	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool WaitClear() {
		var result = Wait();
		Clear();
		return result;
	}

	// This isn't quite thread-safe, but the granularity of this in our codebase is really loose to begin with. It doesn't need to be entirely thread-safe.
	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool WaitSet(bool state) {
		var result = Wait();
		State = state.ToInt();
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void Set(bool state = true) {
		State = state.ToInt();
		Event!.Set();
	}

	// This clears the state without triggering the event.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void Clear() => State = 0;

	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool GetAndClear() => Interlocked.Exchange(ref State, 0).ToBool();

	~Condition() => Dispose();

	public void Dispose() {
		Event?.Dispose();
		Event = null;

		GC.SuppressFinalize(this);
	}
}
