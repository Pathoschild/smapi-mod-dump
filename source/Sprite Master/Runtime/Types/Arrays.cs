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
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace SpriteMaster.Types {
	public static class Arrays {
		internal static class EmptyArrayStatic<T> {
			[ImmutableObject(true)]
			internal static readonly T[] Value = new T[0];
		}

		[Pure, MethodImpl(Runtime.MethodImpl.Optimize)]
		[ImmutableObject(true)]
		public static T[] Empty<T> () => EmptyArrayStatic<T>.Value;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static T[] Singleton<T> (T value) => new T[] { value };

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static T[] Of<T> (params T[] values) => values;

		private sealed class WrappedUnmanagedMemoryStream<T> : UnmanagedMemoryStream {
			private readonly GCHandle Handle;
			private volatile bool IsDisposed = false;

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			private unsafe WrappedUnmanagedMemoryStream (GCHandle handle, int offset, int size, FileAccess access) :
				base(
					(byte*)(handle.AddrOfPinnedObject() + (Marshal.SizeOf(typeof(T)) * offset)),
					size * Marshal.SizeOf(typeof(T)),
					size * Marshal.SizeOf(typeof(T)),
					access
				) {
				Handle = handle;
			}

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public static unsafe WrappedUnmanagedMemoryStream<T> Get (T[] data, int offset, int size, FileAccess access) {
				var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
				try {
					return new WrappedUnmanagedMemoryStream<T>(handle, offset, size, access);
				}
				catch {
					handle.Free();
					throw;
				}
			}

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			~WrappedUnmanagedMemoryStream () {
				Dispose(true);
			}

			[SecuritySafeCritical]
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			protected override void Dispose (bool disposing) {
				if (!IsDisposed) {
					Handle.Free();
					IsDisposed = true;
				}
			}
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static unsafe UnmanagedMemoryStream Stream<T> (this T[] data) where T : struct {
			return WrappedUnmanagedMemoryStream<T>.Get(data, 0, data.Length, FileAccess.ReadWrite);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static UnmanagedMemoryStream Stream<T> (this T[] data, int offset = 0, int length = -1, FileAccess access = FileAccess.ReadWrite) {
			if (length == -1) {
				length = data.Length - offset;
			}
			return WrappedUnmanagedMemoryStream<T>.Get(data, offset, length, access);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static MemoryStream Stream (this byte[] data) {
			return new MemoryStream(data, 0, data.Length, true, true);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static MemoryStream Stream (this byte[] data, int offset = 0, int length = -1, FileAccess access = FileAccess.ReadWrite) {
			if (length == -1) {
				length = data.Length - offset;
			}
			return new MemoryStream(data, offset, length, (access != FileAccess.Read), true);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static FixedSpan<U> CastAs<T, U> (this T[] data) where T : unmanaged where U : unmanaged => new FixedSpan<T>(data).As<U>();

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static T[] Reverse<T> (this T[] array) {
			//Contract.AssertNotNull(array);
			Array.Reverse(array);
			return array;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static T[] Reversed<T> (this T[] array) {
			//Contract.AssertNotNull(array);
			var result = (T[])array.Clone();
			Array.Reverse(result);
			return result;
		}
	}

	public static class Arrays<T> {
		[ImmutableObject(true)]
		public static readonly T[] Empty = Arrays.Empty<T>();

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static T[] Singleton (T value) => Arrays.Singleton<T>(value);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static T[] Of (params T[] values) => Arrays.Of<T>(values);
	}
}
