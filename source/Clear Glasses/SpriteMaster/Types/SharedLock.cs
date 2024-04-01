/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpriteMaster;

using LockType = ReaderWriterLockSlim;

internal sealed class SharedLock : CriticalFinalizerObject, IDisposable {
	private LockType? Lock;

	[StructLayout(LayoutKind.Auto)]
	internal ref struct ReadCookie {
		private LockType? Lock = null;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private ReadCookie(LockType rwlock) => Lock = rwlock;

		[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
		internal static ReadCookie Create(LockType rwlock) {
			rwlock.EnterReadLock();
			return new(rwlock);
		}

		[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
		internal static ReadCookie TryCreate(LockType rwlock) => rwlock.TryEnterReadLock(0) ? new(rwlock) : new();

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public void Dispose() {
			if (Lock is null) {
				return;
			}

			Lock.ExitReadLock();
			Lock = null;
		}

		[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
		public static implicit operator bool(ReadCookie cookie) => cookie.Lock is not null;
	}
	[StructLayout(LayoutKind.Auto)]
	internal ref struct ExclusiveCookie {
		private LockType? Lock = null;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private ExclusiveCookie(LockType rwlock) => Lock = rwlock;

		[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
		internal static ExclusiveCookie Create(LockType rwlock) {
			rwlock.EnterWriteLock();
			return new(rwlock);
		}

		[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
		internal static ExclusiveCookie TryCreate(LockType rwlock) => rwlock.TryEnterWriteLock(0) ? new(rwlock) : new();

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public void Dispose() {
			if (Lock is null) {
				return;
			}

			Lock.ExitWriteLock();
			Lock = null;
		}

		[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
		public static implicit operator bool(ExclusiveCookie cookie) => cookie.Lock is not null;
	}

	[StructLayout(LayoutKind.Auto)]
	internal ref struct ReadWriteCookie {
		private LockType? Lock = null;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private ReadWriteCookie(LockType rwlock) {
			Lock = rwlock;
		}

		[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
		internal static ReadWriteCookie Create(LockType rwlock) {
			rwlock.EnterUpgradeableReadLock();
			return new(rwlock);
		}

		[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
		internal static ReadWriteCookie TryCreate(LockType rwlock) => rwlock.TryEnterUpgradeableReadLock(0) ? new(rwlock) : new();

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public void Dispose() {
			if (Lock is null) {
				return;
			}

			Lock.ExitUpgradeableReadLock();
			Lock = null;
		}

		[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
		public static implicit operator bool(ReadWriteCookie cookie) => cookie.Lock is not null;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal SharedLock(LockRecursionPolicy recursionPolicy = LockRecursionPolicy.NoRecursion) {
		Lock = new(recursionPolicy);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	~SharedLock() => Dispose();

	internal bool IsLocked => IsReadLock || IsWriteLock || IsReadWriteLock;
	internal bool IsReadLock => Lock?.IsReadLockHeld ?? false;
	internal bool IsWriteLock => Lock?.IsWriteLockHeld ?? false;
	internal bool IsReadWriteLock => Lock?.IsUpgradeableReadLockHeld ?? false;
	internal bool IsDisposed => Lock is null;

	internal ReadCookie Read => ReadCookie.Create(Lock!);
	internal ReadCookie TryRead => ReadCookie.TryCreate(Lock!);
	internal ExclusiveCookie Write => ExclusiveCookie.Create(Lock!);
	internal ExclusiveCookie TryWrite => ExclusiveCookie.TryCreate(Lock!);
	internal ReadWriteCookie ReadWrite => ReadWriteCookie.Create(Lock!);
	internal ReadWriteCookie TryReadWrite => ReadWriteCookie.TryCreate(Lock!);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public void Dispose() {
		if (Lock is null) {
			return;
		}

		if (IsReadWriteLock) {
			Lock.ExitUpgradeableReadLock();
		}
		if (IsWriteLock) {
			Lock.ExitWriteLock();
		}
		else if (IsReadLock) {
			Lock.ExitReadLock();
		}

		Lock = null;

		GC.SuppressFinalize(this);
	}
}
