/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using System.Text.RegularExpressions;

namespace xBRZ;

internal readonly record struct Argument(string Key, string? Value = null) {
	internal readonly bool IsCommand => Key[0] is '-' or '/';
	private static readonly Regex CommandPattern = new(@"^(?:--|-|/)(.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
	internal readonly string? Command => IsCommand ? CommandPattern.Match(Key).Groups.ElementAtOrDefaultF(1)?.Value : null;

	public override readonly string ToString() => Value is null ? Key : $"{Key}={Value}";
}
