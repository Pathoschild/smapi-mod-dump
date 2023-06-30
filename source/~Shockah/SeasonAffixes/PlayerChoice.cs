/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace Shockah.SeasonAffixes;

internal abstract class PlayerChoice
{
	public sealed class Choice : PlayerChoice
	{
		public IReadOnlySet<ISeasonAffix> Affixes { get; init; }

		public Choice(IReadOnlySet<ISeasonAffix> affixes)
		{
			this.Affixes = affixes;
		}

		public override bool Equals(object? obj)
			=> obj is Choice other && Affixes.SetEquals(other.Affixes);

		public override int GetHashCode()
		{
			int hash = 0;
			foreach (var affix in Affixes)
				hash ^= affix.GetHashCode();
			return hash;
		}
	}

	public sealed class Reroll : PlayerChoice
	{
		public static Reroll Instance { get; private set; } = new();

		private Reroll()
		{
		}
	}

	public sealed class Invalid : PlayerChoice
	{
		public static Invalid Instance { get; private set; } = new();

		private Invalid()
		{
		}
	}

	private PlayerChoice() { }
}