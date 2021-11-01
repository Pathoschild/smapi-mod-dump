/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;
using SpriteMaster.xBRZ.Common;

namespace SpriteMaster.xBRZ.Blend {
	internal static class BlendInfo {
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static BlendType GetTopL (this byte b) { unchecked { return (b & 0x3).BlendType(); } }
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static BlendType GetTopR (this byte b) { unchecked { return ((byte)(b >> 2) & 0x3).BlendType(); } }
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static BlendType GetBottomR (this byte b) { unchecked { return ((byte)(b >> 4) & 0x3).BlendType(); } }
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static BlendType GetBottomL (this byte b) { unchecked { return ((byte)(b >> 6) & 0x3).BlendType(); } }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static byte SetTopL (this byte b, BlendType bt) { unchecked { return (byte)(b | bt.Byte()); } }
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static byte SetTopR (this byte b, BlendType bt) { unchecked { return (byte)(b | (bt.Byte() << 2)); } }
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static byte SetBottomR (this byte b, BlendType bt) { unchecked { return (byte)(b | (bt.Byte() << 4)); } }
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static byte SetBottomL (this byte b, BlendType bt) { unchecked { return (byte)(b | (bt.Byte() << 6)); } }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static bool BlendingNeeded(this byte b) { return b != 0; }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static byte Rotate (this byte b, RotationDegree rotDeg) {
			unchecked {
				var l = (byte)((byte)rotDeg << 1);
				var r = (byte)(8 - l);

				return unchecked((byte)(b << l | b >> r));
			}
		}
	}
}
