using System;
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
			public SharedCookie (ReaderWriterLock rwlock) {
				this.Lock = rwlock;
				this.Lock.AcquireReaderLock(-1);
			}

			public bool IsDisposed {
				[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
				get { return Lock == null; }
			}

			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			public void Dispose () {
				if (Lock == null) {
					return;
				}

				Contract.Assert(Lock.IsReaderLockHeld && !Lock.IsWriterLockHeld);
				if (Lock.IsReaderLockHeld) {
					Lock.ReleaseReaderLock();
				}
				Lock = null;
			}
		}
		public struct ExclusiveCookie : IDisposable {
			private ReaderWriterLock Lock;

			[SecuritySafeCritical]
			public ExclusiveCookie (ReaderWriterLock rwlock) {
				this.Lock = rwlock;
				this.Lock.AcquireWriterLock(-1);
			}

			public bool IsDisposed {
				[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
				get { return Lock == null; }
			}

			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			public void Dispose () {
				if (Lock == null) {
					return;
				}

				Contract.Assert(!Lock.IsReaderLockHeld && Lock.IsWriterLockHeld);
				if (Lock.IsWriterLockHeld) {
					Lock.ReleaseWriterLock();
				}
				Lock = null;
			}
		}

		public struct PromotedCookie : IDisposable {
			private ReaderWriterLock Lock;
			private LockCookie Cookie;

			[SecuritySafeCritical]
			public PromotedCookie (ReaderWriterLock rwlock) {
				this.Lock = rwlock;
				this.Cookie = this.Lock.UpgradeToWriterLock(-1);
			}

			public bool IsDisposed {
				[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
				get { return Lock == null; }
			}

			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			public void Dispose () {
				if (Lock == null) {
					return;
				}

				Contract.AssertTrue(Lock.IsWriterLockHeld);
				if (Lock.IsWriterLockHeld) {
					Lock.DowngradeFromWriterLock(ref Cookie);
				}
				Lock = null;
			}
		}

		~SharedLock () {
			Dispose();
			Lock = null;
		}

		public bool IsLocked {
			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get { return Lock.IsReaderLockHeld || Lock.IsWriterLockHeld; }
		}

		public bool IsSharedLock {
			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get { return Lock.IsReaderLockHeld; }
		}

		public bool IsExclusiveLock {
			[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get { return Lock.IsWriterLockHeld; }
		}

		public bool IsDisposed {
			get { return Lock == null; }
		}

		public SharedCookie Shared {
			[SecuritySafeCritical]
			get {
				Contract.Assert(!IsLocked);
				return new SharedCookie(Lock);
			}
		}

		public ExclusiveCookie Exclusive {
			[SecuritySafeCritical]
			get {
				Contract.Assert(!IsLocked);
				return new ExclusiveCookie(Lock);
			}
		}

		public PromotedCookie Promote {
			[SecuritySafeCritical]
			get {
				Contract.Assert(!IsExclusiveLock && IsSharedLock);
				return new PromotedCookie(Lock);
			}
		}

		[SecuritySafeCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
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
