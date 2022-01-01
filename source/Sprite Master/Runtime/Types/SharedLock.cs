/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace SpriteMaster;

sealed class SharedLock : CriticalFinalizerObject, IDisposable {
	private ReaderWriterLock Lock = new();

	internal struct SharedCookie : IDisposable {
		private ReaderWriterLock Lock;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal SharedCookie(ReaderWriterLock rwlock, int timeout) {
			Lock = null;
			rwlock.AcquireReaderLock(timeout);
			Lock = rwlock;
		}

		internal readonly bool IsDisposed => Lock is null;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public void Dispose() {
			if (Lock == null) {
				return;
			}

			Lock.ReleaseReaderLock();

			Lock = null;
		}
	}
	internal struct ExclusiveCookie : IDisposable {
		private ReaderWriterLock Lock;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal ExclusiveCookie(ReaderWriterLock rwlock, int timeout) {
			Lock = null;
			rwlock.AcquireWriterLock(timeout);
			Lock = rwlock;
		}

		internal readonly bool IsDisposed => Lock is null;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public void Dispose() {
			if (Lock == null) {
				return;
			}

			Lock.ReleaseWriterLock();

			Lock = null;
		}
	}

	internal struct PromotedCookie : IDisposable {
		private ReaderWriterLock Lock;
		private LockCookie Cookie;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal PromotedCookie(ReaderWriterLock rwlock, int timeout) {
			Lock = null;
			this.Cookie = rwlock.UpgradeToWriterLock(timeout);
			Lock = rwlock;
		}

		internal readonly bool IsDisposed => Lock is null;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public void Dispose() {
			if (Lock == null) {
				return;
			}

			Lock.DowngradeFromWriterLock(ref Cookie);

			Lock = null;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	~SharedLock() {
		Dispose();
		Lock = null;
	}

	internal bool IsLocked => Lock.IsReaderLockHeld || Lock.IsWriterLockHeld;

	internal bool IsSharedLock => Lock.IsReaderLockHeld;

	internal bool IsExclusiveLock => Lock.IsWriterLockHeld;

	internal bool IsDisposed => Lock == null;

	internal SharedCookie Shared => new(Lock, -1);

	internal SharedCookie? TryShared {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			try {
				return new(Lock, 0);
			}
			catch { // TODO : catch only specific exceptions
				return null;
			}
		}
	}

	internal ExclusiveCookie Exclusive => new(Lock, -1);

	internal ExclusiveCookie? TryExclusive {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			try {
				return new(Lock, 0);
			}
			catch { // TODO : catch only specific exceptions
				return null;
			}
		}
	}

	internal PromotedCookie Promote => new(Lock, -1);

	internal PromotedCookie? TryPromote {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			//Contract.Assert(!IsExclusiveLock && IsSharedLock);
			try {
				return new(Lock, 0);
			}
			catch {
				return null;
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public void Dispose() {
		if (Lock is null) {
			return;
		}

		if (Lock.IsWriterLockHeld) {
			Lock.ReleaseWriterLock();
		}
		else if (Lock.IsReaderLockHeld) {
			Lock.ReleaseReaderLock();
		}
	}
}
