/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster.Types;

internal partial struct Vector2I {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2I Min() => new(Math.Min(X, Y));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2I Max() => new(Math.Max(X, Y));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2I Min(Vector2I v) {
		if (UseSIMD && Extensions.Simd.Support.Sse41) {
			var vec0 = AsVec128;
			var vec1 = v.AsVec128;
			var res = Sse41.Min(vec0, vec1);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			Math.Min(X, v.X),
			Math.Min(Y, v.Y)
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2I Max(Vector2I v) {
		if (UseSIMD && Extensions.Simd.Support.Sse41) {
			var vec0 = AsVec128;
			var vec1 = v.AsVec128;
			var res = Sse41.Max(vec0, vec1);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			Math.Max(X, v.X),
			Math.Max(Y, v.Y)
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2I Clamp(Vector2I min, Vector2I max) {
		if (UseSIMD && Extensions.Simd.Support.Sse41) {
			var vec = AsVec128;
			var minVec = min.AsVec128;
			var maxVec = max.AsVec128;
			var resMin = Sse41.Max(vec, minVec);
			var res = Sse41.Min(resMin, maxVec);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			X.Clamp(min.X, max.X),
			Y.Clamp(min.Y, max.Y)
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2I Min(int v) {
		if (UseSIMD && Extensions.Simd.Support.Sse41) {
			var vec0 = AsVec128;
			var vec1 = Vector128.Create(v);
			var res = Sse41.Min(vec0, vec1);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			Math.Min(X, v),
			Math.Min(Y, v)
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2I Max(int v) {
		if (UseSIMD && Extensions.Simd.Support.Sse41) {
			var vec0 = AsVec128;
			var vec1 = Vector128.Create(v);
			var res = Sse41.Max(vec0, vec1);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			Math.Max(X, v),
			Math.Max(Y, v)
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2I Clamp(int min, int max) {
		if (UseSIMD && Extensions.Simd.Support.Sse41) {
			var vec = AsVec128;
			var minVec = Vector128.Create(min);
			var maxVec = Vector128.Create(max);
			var resMin = Sse41.Max(vec, minVec);
			var res = Sse41.Min(resMin, maxVec);
			return new(res.AsUInt64().ToScalar());
		}
		return new(
			X.Clamp(min, max),
			Y.Clamp(min, max)
		);
	}

	internal readonly Vector2I Abs {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get {
			if (UseSIMD && Extensions.Simd.Support.Ssse3) {
				var vec = AsVec128;
				var res = Ssse3.Abs(vec);
				return new(res.AsUInt64().ToScalar());
			}
			return (Math.Abs(X), Math.Abs(Y));
		}
	}
}
