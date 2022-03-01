/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[CLSCompliant(false)]
[DebuggerDisplay("[{X}, {Y}}")]
[StructLayout(LayoutKind.Explicit, Pack = Vector2I.Alignment, Size = Vector2I.ByteSize)]
unsafe partial struct Vector2I :
	CompositePrimitive<ulong>,
	ILongHash,
	ICloneable {
	internal const int ByteSize = sizeof(ulong);
	internal const int Alignment = sizeof(ulong);

	internal static readonly Vector2I MaxValue = (int.MaxValue, int.MaxValue);
	internal static readonly Vector2I MinValue = (int.MinValue, int.MinValue);

	internal static readonly Vector2I Zero = (0, 0);
	internal static readonly Vector2I One = (1, 1);
	internal static readonly Vector2I MinusOne = (-1, -1);
	internal static readonly Vector2I Empty = Zero;

	[FieldOffset(0)]
	private fixed int Value[2];

	[FieldOffset(0)]
	internal ulong Packed;

	[FieldOffset(0)]
	internal int X;
	[FieldOffset(sizeof(int))]
	internal int Y;

	internal int Width {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => X;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => X = value;
	}
	internal int Height {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Y;
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Y = value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot), DebuggerStepThrough, DebuggerHidden()]
	private static int CheckIndex(int index) {
#if DEBUG
		if (index < 0 || index >= 2) {
			throw new IndexOutOfRangeException(nameof(index));
		}
#endif
		return index;
	}

	internal int this[int index] {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		readonly get => Value[CheckIndex(index)];
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => Value[CheckIndex(index)] = value;
	}

	internal readonly int Area => X * Y;

	internal readonly bool IsEmpty => Packed == 0UL;
	internal readonly bool IsZero => Packed == 0UL;
	internal readonly int MinOf => Math.Min(X, Y);
	internal readonly int MaxOf => Math.Max(X, Y);

	internal readonly int Sum => X + Y;

	internal readonly int LengthSquared => X * X + Y * Y;
	internal readonly float Length => MathF.Sqrt(LengthSquared);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(ulong Packed) : this() => this.Packed = Packed;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I From(ulong Packed) => new(Packed: Packed);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(int X, int Y) : this() {
		this.X = X;
		this.Y = Y;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I From(int X, int Y) => new(X, Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(in (int X, int Y) vec) : this(vec.X, vec.Y) {}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I From(in (int X, int Y) vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(int Value) : this(Value, Value) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Vector2I From(int Value) => new(Value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(in XNA.Vector2 Vector, bool Round = true) : this(Round ? Vector.NearestInt() : Vector.TruncateInt()) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(in Vector2F Vector, bool Round = true) : this(Round ? Vector.NearestInt() : Vector.TruncateInt()) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(Vector2I vec) : this(vec.Packed) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(DrawingPoint v) : this(v.X, v.Y) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(XNA.Point v) : this(v.X, v.Y) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(XTilePoint v) : this(v.X, v.Y) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(DrawingSize v) : this(v.Width, v.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(XTileSize v) : this(v.Width, v.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(Microsoft.Xna.Framework.Graphics.Texture2D tex) : this(tex.Width, tex.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Vector2I(System.Drawing.Bitmap bmp) : this(bmp.Width, bmp.Height) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(in (int X, int Y) vec) => new (vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator (int X, int Y)(Vector2I vec) => (vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator DrawingPoint(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator XNA.Point(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator XTilePoint(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator DrawingSize(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator XTileSize(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator XNA.Vector2(Vector2I vec) => new(vec.X, vec.Y);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(DrawingPoint vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(XNA.Point vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(XTilePoint vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(DrawingSize vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Vector2I(XTileSize vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator Bounds(Vector2I vec) => new(vec);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly Vector2I Clone() => this;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	readonly object ICloneable.Clone() => this;

	public override readonly string ToString() => $"{{{X}, {Y}}}";
	public readonly string ToString(IFormatProvider? provider) => $"{{{X.ToString(provider)}, {Y.ToString(provider)}}}";

	// C# GetHashCode on all integer primitives, even longs, just returns it truncated to an int.
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly override int GetHashCode() => (int)Hashing.Combine(X.GetHashCode(), Y.GetHashCode());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	ulong ILongHash.GetLongHashCode() => ((uint)X.GetHashCode() << 32) | (uint)Y.GetHashCode();
}
