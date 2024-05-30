/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_WIDGETS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace Leclair.Stardew.Common.UI;

[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct KMargins : IEquatable<KMargins> {

	private static KMargins emptyMargins = new(0, 0, 0 ,0);

	public static KMargins Empty => emptyMargins;

	[DataMember]
	public int Left;

	[DataMember]
	public int Top;

	[DataMember]
	public int Right;

	[DataMember]
	public int Bottom;

	public bool IsEmpty => Left == 0 && Top == 0 && Right == 0 && Bottom == 0;

	internal string DebugDisplayString => $"Left={Left}, Top={Top}, Right={Right}, Bottom={Bottom}";

	public KMargins(int left, int top, int right, int bottom) {
		Left = left;
		Top = top;
		Right = right;
		Bottom = bottom;
	}

	public static bool operator ==(KMargins a, KMargins b) {
		return a.Left == b.Left && a.Top == b.Top && a.Right == b.Right && a.Bottom == b.Bottom;
	}

	public static bool operator !=(KMargins a, KMargins b) {
		return a.Left != b.Left || a.Top != b.Top || a.Right != b.Right || a.Bottom != b.Bottom;
	}

	public static KMargins operator +(KMargins a) => a;
	public static KMargins operator +(KMargins a, int amount) => new(a.Left + amount, a.Top + amount, a.Right + amount, a.Bottom + amount);
	public static KMargins operator +(KMargins a, KMargins b) => new(a.Left + b.Left, a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom);

	public static KMargins operator -(KMargins a) => new(-a.Left, -a.Top, -a.Right, -a.Bottom);
	public static KMargins operator -(KMargins a, int amount) => new(a.Left - amount, a.Top - amount, a.Right - amount, a.Bottom - amount);
	public static KMargins operator -(KMargins a, KMargins b) => new(a.Left - b.Left, a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom);

	public static KMargins operator *(KMargins a, int factor) => new(a.Left * factor, a.Top * factor, a.Right * factor, a.Bottom * factor);
	public static KMargins operator *(KMargins a, KMargins b) => new(a.Left * b.Left, a.Top * b.Top, a.Right * b.Right, a.Bottom * b.Bottom);
	public static KMargins operator /(KMargins a, int factor) => new(a.Left / factor, a.Top / factor, a.Right / factor, a.Bottom / factor);
	public static KMargins operator /(KMargins a, KMargins b) => new(a.Left / b.Left, a.Top / b.Top, a.Right / b.Right, a.Bottom / b.Bottom);

	public static KMargins operator |(KMargins a, KMargins b) => new(Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));

	public override bool Equals(object? obj) {
		if (obj is KMargins margins)
			return this == margins;
		return false;
	}

	public bool Equals(KMargins other) {
		return this == other;
	}

	public override int GetHashCode() {
		return HashCode.Combine(Left, Top, Right, Bottom);
	}

	public override string ToString() {
		return $"{{Left:{Left} Top:{Top} Right:{Right} Bottom:{Bottom}}}";
	}

	public void Deconstruct(out int left, out int top, out int right, out int bottom) {
		left = Left;
		top = Top;
		right = Right;
		bottom = Bottom;
	}

}

#endif
