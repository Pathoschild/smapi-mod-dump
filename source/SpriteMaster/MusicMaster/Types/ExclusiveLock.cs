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
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MusicMaster;

internal readonly ref struct TryLock {
	private readonly object? Lock;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal TryLock(object lockObj) {
		Lock = !Monitor.TryEnter(lockObj) ? null : lockObj;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal TryLock(object lockObj, TimeSpan timeout) {
		Lock = !Monitor.TryEnter(lockObj, timeout) ? null : lockObj;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal TryLock(object lockObj, int millisecondsTimeout) {
		Lock = !Monitor.TryEnter(lockObj, millisecondsTimeout) ? null : lockObj;
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator bool(in TryLock tryLock) => tryLock.Lock is not null;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public void Dispose() {
		if (Lock is null) {
			return;
		}

		Monitor.Exit(Lock);
		Unsafe.AsRef(Lock) = null!;
	}
}
