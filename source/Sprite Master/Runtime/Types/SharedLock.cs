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
using System.Security;
using System.Threading;

namespace SpriteMaster {
	[SecuritySafeCritical]
	public sealed class SharedLock : CriticalFinalizerObject, IDisposable {
		private ReaderWriterLock Lock = new ReaderWriterLock();

		public struct SharedCookie : IDisposable {
			private ReaderWriterLock Lock;

			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public SharedCookie (ReaderWriterLock rwlock, int timeout) {
				Lock = null;
				rwlock.AcquireReaderLock(timeout);
				Lock = rwlock;
			}

			public bool IsDisposed {
				[SecuritySafeCritical]
				[MethodImpl(Runtime.MethodImpl.Optimize)]
				get { return Lock == null; }
			}

			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public void Dispose () {
				if (Lock == null) {
					return;
				}

				Lock.ReleaseReaderLock();

				Lock = null;
			}
		}
		public struct ExclusiveCookie : IDisposable {
			private ReaderWriterLock Lock;

			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public ExclusiveCookie (ReaderWriterLock rwlock, int timeout) {
				Lock = null;
				rwlock.AcquireWriterLock(timeout);
				Lock = rwlock;
			}

			public bool IsDisposed {
				[SecuritySafeCritical]
				[MethodImpl(Runtime.MethodImpl.Optimize)]
				get { return Lock == null; }
			}

			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public void Dispose () {
				if (Lock == null) {
					return;
				}

				Lock.ReleaseWriterLock();

				Lock = null;
			}
		}

		public struct PromotedCookie : IDisposable {
			private ReaderWriterLock Lock;
			private LockCookie Cookie;

			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public PromotedCookie (ReaderWriterLock rwlock, int timeout) {
				Lock = null;
				this.Cookie = rwlock.UpgradeToWriterLock(timeout);
				Lock = rwlock;
			}

			public bool IsDisposed {
				[SecuritySafeCritical]
				[MethodImpl(Runtime.MethodImpl.Optimize)]
				get { return Lock == null; }
			}

			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public void Dispose () {
				if (Lock == null) {
					return;
				}

				Lock.DowngradeFromWriterLock(ref Cookie);

				Lock = null;
			}
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		~SharedLock () {
			Dispose();
			Lock = null;
		}

		public bool IsLocked {
			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get { return Lock.IsReaderLockHeld || Lock.IsWriterLockHeld; }
		}

		public bool IsSharedLock {
			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get { return Lock.IsReaderLockHeld; }
		}

		public bool IsExclusiveLock {
			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get { return Lock.IsWriterLockHeld; }
		}

		public bool IsDisposed {
			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get { return Lock == null; }
		}

		public SharedCookie Shared {
			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get {
				return new SharedCookie(Lock, -1);
			}
		}

		public SharedCookie? TryShared {
			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get {
				try {
					return new SharedCookie(Lock, 0);
				}
				catch {
					return null;
				}
			}
		}

		public ExclusiveCookie Exclusive {
			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get {
				return new ExclusiveCookie(Lock, -1);
			}
		}

		public ExclusiveCookie? TryExclusive {
			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get {
				try {
					return new ExclusiveCookie(Lock, 0);
				}
				catch {
					return null;
				}
			}
		}

		public PromotedCookie Promote {
			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get {
				//Contract.Assert(!IsExclusiveLock && IsSharedLock);
				return new PromotedCookie(Lock, -1);
			}
		}

		public PromotedCookie? TryPromote {
			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get {
				//Contract.Assert(!IsExclusiveLock && IsSharedLock);
				try {
					return new PromotedCookie(Lock, 0);
				}
				catch {
					return null;
				}
			}
		}

		[SecuritySafeCritical]
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public void Dispose () {
			if (Lock == null) {
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
}
