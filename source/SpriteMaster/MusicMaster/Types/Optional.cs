/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MusicMaster.Types;

[StructLayout(LayoutKind.Auto)]
internal ref struct Optional<T> {
	internal static Optional<T> Empty => new();

	private T? InnerValue = default;

	internal T Value {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get {
			if (!HasValue) {
				ThrowHelper.ThrowInvalidOperationException("No Value");
			}

			return InnerValue;
		}
		[MemberNotNull("Value", "InnerValue"), MethodImpl(MethodImplOptions.AggressiveInlining)]
		set {
			if (value is null) {
				ThrowHelper.ThrowArgumentNullException(nameof(value));
			}

			InnerValue = value;
			HasValue = true;
#pragma warning disable CS8774
		}
#pragma warning restore CS8774
	}

	[MemberNotNullWhen(true, "Value", "InnerValue")]
	internal bool HasValue { readonly get; private set; } = false;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Optional() {
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Optional(T value) {
		InnerValue = value;
		HasValue = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void Clear() {
		InnerValue = default;
		HasValue = false;
	}

	[MemberNotNull("Value", "InnerValue"), MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Optional<T>(T value) => new(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator T(Optional<T> value) => value.Value;
}
