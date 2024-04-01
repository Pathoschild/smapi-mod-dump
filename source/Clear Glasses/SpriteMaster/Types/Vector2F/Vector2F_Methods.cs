/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Runtime.CompilerServices;

using NumericsVector2 = System.Numerics.Vector2;

namespace SpriteMaster.Types;

internal partial struct Vector2F {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2F Min() => new(MathF.Min(X, Y));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2F Max() => new(MathF.Max(X, Y));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2F Min(Vector2F v) => NumericsVector2.Min(this, v);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2F Max(Vector2F v) => NumericsVector2.Max(this, v);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2F Clamp(Vector2F min, Vector2F max) => NumericsVector2.Clamp(this, min, max);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2F Min(float v) => NumericsVector2.Min(this, new NumericsVector2(v));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2F Max(float v) => NumericsVector2.Max(this, new NumericsVector2(v));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal readonly Vector2F Clamp(float min, float max) => NumericsVector2.Clamp(this, new(min), new(max));

	internal readonly Vector2F Abs => NumericsVector2.Abs(this);
}