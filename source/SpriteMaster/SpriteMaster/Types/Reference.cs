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

namespace SpriteMaster.Types;

sealed class Reference<T> : IEquatable<Reference<T>>, IEquatable<T?> where T : struct {
	internal T? Value = null;

	internal bool HasValue => Value is not null;

	internal Reference() { }

	internal Reference(T? value) => Value = value;

	internal Reference(in T value) => Value = value;

	public static implicit operator T?(in Reference<T> reference) => reference.Value;
	public static implicit operator Reference<T>(in T? value) => new(value);

	public bool Equals(Reference<T>? other) => Value.Equals(other?.Value);

	public bool Equals(T? other) => Value.Equals(other);

	public override int GetHashCode() => Value.GetHashCode();

	public static bool operator ==(Reference<T> a, Reference<T> b) => a.Equals(b);
	public static bool operator ==(Reference<T> a, in T? b) => a.Equals(b);
	public static bool operator ==(in T? a, Reference<T> b) => b.Equals(a);

	public static bool operator !=(Reference<T> a, Reference<T> b) => !a.Equals(b);
	public static bool operator !=(Reference<T> a, in T? b) => !a.Equals(b);
	public static bool operator !=(in T? a, Reference<T> b) => !b.Equals(a);

	public override bool Equals(object? obj) {
		switch (obj) {
			case Reference<T> reference:
				return this.Equals(reference);
			case T value:
				return this.Equals(Value);
			case null:
				return !Value.HasValue;
			default:
				return false;
		}
	}
}
