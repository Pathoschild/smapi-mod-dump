/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Resample.Decoder;

internal static class BlockDecoderCommon {
	[DebuggerDisplay("[{X}, {Y}}")]
	[StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong), Size = sizeof(ulong))]
	internal ref struct Vector2U {
		internal uint X;
		internal uint Y;

		internal uint Width {
			[MethodImpl(Runtime.MethodImpl.Inline)]
			readonly get => X;
			[MethodImpl(Runtime.MethodImpl.Inline)]
			set => X = value;
		}
		internal uint Height {
			[MethodImpl(Runtime.MethodImpl.Inline)]
			readonly get => Y;
			[MethodImpl(Runtime.MethodImpl.Inline)]
			set => Y = value;
		}

		internal uint Area => X * Y;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal Vector2U(uint x, uint y) {
			X = x;
			Y = y;
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal Vector2U(Vector2I vec) : this((uint)vec.X, (uint)vec.Y) { }

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal Vector2U((uint X, uint Y) vec) : this(vec.X, vec.Y) { }

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public static implicit operator Vector2U(Vector2I vec) => new(vec);
		[MethodImpl(Runtime.MethodImpl.Inline)]
		public static implicit operator Vector2U((uint X, uint Y) vec) => new(vec);
		[MethodImpl(Runtime.MethodImpl.Inline)]
		public static implicit operator Vector2I(Vector2U vec) => new((int)vec.X, (int)vec.Y);

		public override readonly string ToString() => $"{{{X}, {Y}}}";
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsBlockMultiple(int value) => (value & 3) == 0;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsBlockMultiple(uint value) => (value & 3) == 0;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsBlockMultiple(Vector2I value) => IsBlockMultiple(value.X | value.Y);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool IsBlockMultiple(Vector2U value) => IsBlockMultiple(value.X | value.Y);
}
