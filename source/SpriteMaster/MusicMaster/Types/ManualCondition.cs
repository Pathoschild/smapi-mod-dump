/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using MusicMaster.Extensions;
using System;
using System.Threading;

namespace MusicMaster.Types;

internal sealed class ManualCondition : IDisposable {
	private volatile int State = 0;
	private ManualResetEvent? Event = new(false);

	internal ManualCondition(bool initialState = false) => State = initialState.ToInt();

	public static implicit operator bool(ManualCondition condition) => condition.State.ToBool();

	// This isn't quite thread-safe, but the granularity of this in our codebase is really loose to begin with. It doesn't need to be entirely thread-safe.
	internal bool Wait() {
		Event!.WaitOne();
		return State.ToBool();
	}

	internal void Set(bool state = true) {
		State = state.ToInt();
		Event!.Set();
	}

	// This clears the state without triggering the event.
	internal void Clear() => State = 0;

	internal bool GetAndClear() => Interlocked.Exchange(ref State, 0).ToBool();

	~ManualCondition() => Dispose();

	public void Dispose() {
		Event?.Dispose();
		Event = null;

		GC.SuppressFinalize(this);
	}
}
