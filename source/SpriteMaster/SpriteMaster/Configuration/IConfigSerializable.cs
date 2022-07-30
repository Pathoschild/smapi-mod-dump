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

namespace SpriteMaster.Configuration;

internal interface IConfigSerializable {
	bool TrySerialize([NotNullWhen(true)] out string serialized);
	bool TryDeserialize(string serialized, [MaybeNullWhen(true)] out object? deserialized);
}

internal interface IConfigSerializable<T> : IConfigSerializable {
	bool TryDeserialize(string serialized, [MaybeNullWhen(true)] out T? deserialized);

	bool IConfigSerializable.TryDeserialize(string serialized, [MaybeNullWhen(true)] out object? deserialized) =>
		TryDeserialize(serialized, out deserialized);
}
