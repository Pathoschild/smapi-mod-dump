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
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster.Types;

internal partial struct Vector2I {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator -(Vector2I value) => new(
		-value.X,
		-value.Y
	);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator ~(Vector2I value) => new(
		~value.X,
		~value.Y
	);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator +(Vector2I value) => value;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator >>(Vector2I value, int bits) {
		if (Sse2.IsSupported && UseSIMD) {
			var vec = value.AsVec128;
			var res = Sse2.ShiftRightArithmetic(vec, (byte)bits);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			value.X >> bits,
			value.Y >> bits
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator <<(Vector2I value, int bits) {
		if (Sse2.IsSupported && UseSIMD) {
			var vec = value.AsVec128;
			var res = Sse2.ShiftLeftLogical(vec, (byte)bits);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			value.X << bits,
			value.Y << bits
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator +(Vector2I lhs, Vector2I rhs) {
		if (Sse2.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var rVec = rhs.AsVec128;
			var res = Sse2.Add(lVec, rVec);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			lhs.X + rhs.X,
			lhs.Y + rhs.Y
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator -(Vector2I lhs, Vector2I rhs) {
		if (Sse2.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var rVec = rhs.AsVec128;
			var res = Sse2.Subtract(lVec, rVec);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			lhs.X - rhs.X,
			lhs.Y - rhs.Y
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator *(Vector2I lhs, Vector2I rhs) {
		if (Sse41.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var rVec = rhs.AsVec128;
			var res = Sse41.MultiplyLow(lVec, rVec);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			lhs.X * rhs.X,
			lhs.Y * rhs.Y
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator /(Vector2I lhs, Vector2I rhs) {
		return new(
			lhs.X / rhs.X,
			lhs.Y / rhs.Y
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator %(Vector2I lhs, Vector2I rhs) => new(
		lhs.X % rhs.X,
		lhs.Y % rhs.Y
	);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator &(Vector2I lhs, Vector2I rhs) {
		if (Sse2.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var rVec = rhs.AsVec128;
			var res = Sse2.And(lVec, rVec);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			lhs.X & rhs.X,
			lhs.Y & rhs.Y
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator |(Vector2I lhs, Vector2I rhs) {
		if (Sse2.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var rVec = rhs.AsVec128;
			var res = Sse2.Or(lVec, rVec);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			lhs.X | rhs.X,
			lhs.Y | rhs.Y
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator ^(Vector2I lhs, Vector2I rhs) {
		if (Sse2.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var rVec = rhs.AsVec128;
			var res = Sse2.Xor(lVec, rVec);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			lhs.X ^ rhs.X,
			lhs.Y ^ rhs.Y
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator +(Vector2I lhs, int rhs) {
		if (Sse2.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var scalar = Vector128.Create(rhs);
			var res = Sse2.Add(lVec, scalar);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			lhs.X + rhs,
			lhs.Y + rhs
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator -(Vector2I lhs, int rhs) {
		if (Sse2.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var scalar = Vector128.Create(rhs);
			var res = Sse2.Subtract(lVec, scalar);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			lhs.X - rhs,
			lhs.Y - rhs
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator *(Vector2I lhs, int rhs) {
		if (Sse41.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var scalar = Vector128.Create(rhs);
			var res = Sse41.MultiplyLow(lVec, scalar);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			lhs.X * rhs,
			lhs.Y * rhs
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator /(Vector2I lhs, int rhs) => new(
		lhs.X / rhs,
		lhs.Y / rhs
	);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator %(Vector2I lhs, int rhs) => new(
		lhs.X % rhs,
		lhs.Y % rhs
	);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator &(Vector2I lhs, int rhs) {
		if (Sse2.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var scalar = Vector128.Create(rhs);
			var res = Sse2.And(lVec, scalar);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			lhs.X & rhs,
			lhs.Y & rhs
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator |(Vector2I lhs, int rhs) {
		if (Sse2.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var scalar = Vector128.Create(rhs);
			var res = Sse2.Or(lVec, scalar);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			lhs.X | rhs,
			lhs.Y | rhs
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator ^(Vector2I lhs, int rhs) {
		if (Sse2.IsSupported && UseSIMD) {
			var lVec = lhs.AsVec128;
			var scalar = Vector128.Create(rhs);
			var res = Sse2.Xor(lVec, scalar);
			return new (res.AsUInt64().ToScalar());
		}
		return new (
			lhs.X ^ rhs,
			lhs.Y ^ rhs
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator +(Vector2I lhs, uint rhs) => lhs + (int)rhs;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator -(Vector2I lhs, uint rhs) => lhs - (int)rhs;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator *(Vector2I lhs, uint rhs) => lhs * (int)rhs;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator /(Vector2I lhs, uint rhs) => lhs / (int)rhs;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator %(Vector2I lhs, uint rhs) => lhs % (int)rhs;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator &(Vector2I lhs, uint rhs) => lhs & (int)rhs;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator |(Vector2I lhs, uint rhs) => lhs | (int)rhs;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static Vector2I operator ^(Vector2I lhs, uint rhs) => lhs ^ (int)rhs;
}
