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
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers;

internal abstract class Config : IEquatable<Config> {
	internal readonly Vector2B Wrapped;
	internal readonly bool HasAlpha;
	internal readonly bool GammaCorrected;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected Config(
		Vector2B wrapped,
		bool hasAlpha,
		bool gammaCorrected
	) {
		Wrapped = wrapped;
		HasAlpha = hasAlpha;
		GammaCorrected = gammaCorrected;
	}

	public bool Equals(Config? other) {
		try {
			foreach (var field in typeof(Config).GetFields()) {
				var leftField = field.GetValue(this);
				var rightField = field.GetValue(other);
				// TODO possibly fall back on IComparable
				if (leftField is null) {
					return rightField is null;
				}
				if (!leftField.Equals(rightField)) {
					return false;
				}
			}
			return true;
		}
		catch {
			return false;
		}
	}

	public override bool Equals(object? obj) {
		if (obj is Config other) {
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode() {
		int hash = 0;
		foreach (var field in typeof(Config).GetFields()) {
			hash ^= field.GetValue(this).GetSafeHash();
		}
		return hash;
	}

	public static bool operator ==(in Config left, in Config right) {
		return left.Equals(right);
	}

	public static bool operator !=(in Config left, in Config right) {
		return !left.Equals(right);
	}
}
