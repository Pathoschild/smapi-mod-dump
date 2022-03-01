/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.Threading;

namespace SpriteMaster.Types;

sealed class Condition : IDisposable {
	private volatile int State = 0;
	private AutoResetEvent Event = new(false);

	internal Condition(bool initialState = false) => State = initialState.ToInt();

	public static implicit operator bool(Condition condition) => condition.State.ToBool();

	// This isn't quite thread-safe, but the granularity of this in our codebase is really loose to begin with. It doesn't need to be entirely thread-safe.
	internal bool Wait() {
		Event.WaitOne();
		return State.ToBool();
	}

	internal void Set(bool state = true) {
		State = state.ToInt();
		Event.Set();
	}

	// This clears the state without triggering the event.
	internal void Clear() => State = 0;

	internal bool GetAndClear() => Interlocked.Exchange(ref State, 0).ToBool();

	~Condition() => Dispose();

	public void Dispose() {
		Event?.Dispose();
		Event = null!;

		GC.SuppressFinalize(this);
	}
}
