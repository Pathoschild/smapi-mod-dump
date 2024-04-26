/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace AutoForager.Extensions
{
	public static class SemanticVersionExtensions
	{
		public static bool IsEqualToOrNewerThan(this ISemanticVersion current, ISemanticVersion other)
		{
			return current.Equals(other)
				|| current.IsNewerThan(other);
		}

		public static bool IsEqualToOrNewerThan(this ISemanticVersion current, string other)
		{
			return current.IsEqualToOrNewerThan(new SemanticVersion(other));
		}
	}
}
