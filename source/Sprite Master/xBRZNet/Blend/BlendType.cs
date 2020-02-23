using System.Runtime.CompilerServices;

namespace SpriteMaster.xBRZ.Blend {
	internal enum BlendType : byte {
		// These blend types must fit into 2 bits.
		None = 0, //do not blend
		Normal = 1, //a normal indication to blend
		Dominant = 2 //a strong indication to blend
	}

	internal static class BlendTypeExtension {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static byte Byte (this BlendType type) {
			return unchecked((byte)type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static BlendType BlendType (this byte value) {
			return unchecked((BlendType)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static BlendType BlendType (this int value) {
			return unchecked((BlendType)value);
		}
	}
}
