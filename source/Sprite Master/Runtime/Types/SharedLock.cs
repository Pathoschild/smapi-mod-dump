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
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public SharedCookie (ReaderWriterLock rwlock, int timeout) {
				Lock = null;
				rwlock.AcquireReaderLock(timeout);
				Lock = rwlock;
			}

			public bool IsDisposed {
				[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get { return Lock == null; }
			}

			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExclusiveCookie (ReaderWriterLock rwlock, int timeout) {
				Lock = null;
				rwlock.AcquireWriterLock(timeout);
				Lock = rwlock;
			}

			public bool IsDisposed {
				[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get { return Lock == null; }
			}

			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public PromotedCookie (ReaderWriterLock rwlock, int timeout) {
				Lock = null;
				this.Cookie = rwlock.UpgradeToWriterLock(timeout);
				Lock = rwlock;
			}

			public bool IsDisposed {
				[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get { return Lock == null; }
			}

			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose () {
				if (Lock == null) {
					return;
				}

				Lock.DowngradeFromWriterLock(ref Cookie);

				Lock = null;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		~SharedLock () {
			Dispose();
			Lock = null;
		}

		public bool IsLocked {
			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return Lock.IsReaderLockHeld || Lock.IsWriterLockHeld; }
		}

		public bool IsSharedLock {
			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return Lock.IsReaderLockHeld; }
		}

		public bool IsExclusiveLock {
			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return Lock.IsWriterLockHeld; }
		}

		public bool IsDisposed {
			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return Lock == null; }
		}

		public SharedCookie Shared {
			[SecuritySafeCritical]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return new SharedCookie(Lock, -1);
			}
		}

		public SharedCookie? TryShared {
			[SecuritySafeCritical]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return new ExclusiveCookie(Lock, -1);
			}
		}

		public ExclusiveCookie? TryExclusive {
			[SecuritySafeCritical]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				//Contract.Assert(!IsExclusiveLock && IsSharedLock);
				return new PromotedCookie(Lock, -1);
			}
		}

		public PromotedCookie? TryPromote {
			[SecuritySafeCritical]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
