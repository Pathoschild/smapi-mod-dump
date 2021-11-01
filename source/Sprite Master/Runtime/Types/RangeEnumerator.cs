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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpriteMaster.Types {
	public sealed class RangeEnumerator :
		IEnumerator,
		IEnumerator<sbyte>,
		IEnumerator<short>,
		IEnumerator<int>,
		IEnumerator<long>,
		IEnumerator<byte>,
		IEnumerator<ushort>,
		IEnumerator<uint>,
		IEnumerator<ulong> {
		readonly long Start;
		readonly long End;
		private long _Current;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public RangeEnumerator(Index start, Index end) {
			Start = start.Value;
			End = end.Value;

			if (Start <= End) {
				_Current = Start - 1;
			}
			else {
				_Current = Start + 1;
			}
		}

		public object Current => _Current;
		sbyte IEnumerator<sbyte>.Current => (sbyte)_Current;
		short IEnumerator<short>.Current => (short)_Current;
		int IEnumerator<int>.Current => (int)_Current;
		long IEnumerator<long>.Current => _Current;
		byte IEnumerator<byte>.Current => (byte)_Current;
		ushort IEnumerator<ushort>.Current => (ushort)_Current;
		uint IEnumerator<uint>.Current => (uint)_Current;
		ulong IEnumerator<ulong>.Current => (ulong)_Current;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public void Dispose() {
			// I cannot fathom what we need to dispose of.
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public bool MoveNext() {
			if (Start <= End) {
				++_Current;
			}
			else {
				--_Current;
			}
			return _Current != End;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public void Reset() => _Current = Start;
	}
}
