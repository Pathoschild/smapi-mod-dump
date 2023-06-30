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

internal static class NetMessage
{
	public record QueueOvernightAffixChoice;

	public record UpdateAffixChoiceMenuConfig(
		OrdinalSeason Season,
		bool Incremental,
		List<HashSet<string>> Choices,
		int RerollsLeft = 0
	);

	public record UpdateActiveAffixes(
		HashSet<string> Affixes,
		AffixActivationContext Context
	);

	public record ConfirmAffixSetChoice(
		HashSet<string>? Affixes
	);

	public record AffixSetChoice(
		HashSet<string> Affixes
	);

	public record RerollChoice;
}