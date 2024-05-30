/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tocseoj/StardewValleyMods
**
*************************************************/

// namespace Tocseoj.Stardew.BigCropBonus;

// internal sealed class QualifiedItemId(string value)
// {
//   private string _value = value;
// 	public override string ToString() => _value;
// 	public override bool Equals(object? obj) => (obj is string str && _value == str) || (obj is QualifiedItemId itemId && _value == itemId._value);
// 	public override int GetHashCode() => _value.GetHashCode();

// 	public static implicit operator string(QualifiedItemId itemId) => itemId._value;
// 	public static implicit operator QualifiedItemId(string str) => new(str);

// 	public static bool operator ==(QualifiedItemId itemId, string str) => itemId._value == str;
// 	public static bool operator !=(QualifiedItemId itemId, string str) => itemId._value != str;

// 	public static bool operator ==(QualifiedItemId itemId, QualifiedItemId other) => itemId._value == other._value;
// 	public static bool operator !=(QualifiedItemId itemId, QualifiedItemId other) => itemId._value != other._value;

	// public static bool operator ==(QualifiedItemId itemId, UnqualifiedItemId other) => itemId._value == ItemRegistry.QualifiedItemId(other);
	// public static bool operator !=(QualifiedItemId itemId, UnqualifiedItemId other) => itemId._value != ItemRegistry.QualifiedItemId(other);
// }

// MultiplayerID
// UnqualifiedItemID
