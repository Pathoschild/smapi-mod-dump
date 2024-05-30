/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;

using Leclair.Stardew.Common.Serialization.Converters;

using Newtonsoft.Json;

namespace Leclair.Stardew.Common.Types;

[JsonConverter(typeof(NamespaceIdConverter))]
public class NamespaceId : IEquatable<NamespaceId?> {

	public string Domain { get; }
	public string Path { get; }

	#region Constructors

	public NamespaceId(string id) {
		if (string.IsNullOrEmpty(id))
			throw new ArgumentNullException(nameof(id));

		int idx = id.IndexOf(':');
		if (idx == -1)
			throw new ArgumentOutOfRangeException(nameof(id), "No domain present in id.");

		Domain = id[..idx].ToLowerInvariant();
		Path = id[(idx + 1)..].ToLowerInvariant();
	}

	public NamespaceId(string domain, string path, bool domainOptional = false) {
		if (string.IsNullOrEmpty(domain))
			throw new ArgumentNullException(nameof(domain));
		if (string.IsNullOrEmpty(path))
			throw new ArgumentNullException(nameof(path));

		int idx;
		if (domainOptional && (idx = path.IndexOf(':')) != -1) {
			Domain = path[..idx].ToLowerInvariant();
			Path = path[(idx + 1)..].ToLowerInvariant();

		} else {
			Domain = domain.ToLowerInvariant();
			Path = path.ToLowerInvariant();
		}
	}

	public NamespaceId(NamespaceId other, string path, bool domainOptional = false) : this(other.Domain, path, domainOptional) { }

	#endregion

	#region Casting

	public override string ToString() {
		return $"{Domain}:{Path}";
	}

	#endregion

	#region Equality

	public bool MatchesDomain(string domain) {
		return Domain.Equals(domain, StringComparison.OrdinalIgnoreCase);
	}

	public bool MatchesDomain(NamespaceId other) {
		return MatchesDomain(other.Domain);
	}

	public bool MatchesPath(string path) {
		return Path.Equals(path, StringComparison.OrdinalIgnoreCase);
	}

	public bool MatchesPath(NamespaceId other) {
		return MatchesPath(other.Path);
	}

	public override bool Equals(object? obj) {
		return Equals(obj as NamespaceId);
	}

	public bool Equals(NamespaceId? other) {
		return other != null && MatchesDomain(other) && MatchesPath(other);
	}

	public override int GetHashCode() {
		return HashCode.Combine(Domain, Path);
	}

	public static bool operator ==(NamespaceId? left, NamespaceId? right) {
		return EqualityComparer<NamespaceId>.Default.Equals(left, right);
	}

	public static bool operator !=(NamespaceId? left, NamespaceId? right) {
		return !(left == right);
	}

	#endregion

}
