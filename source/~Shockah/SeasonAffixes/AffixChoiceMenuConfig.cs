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

internal record AffixChoiceMenuConfig(
	OrdinalSeason Season,
	bool Incremental,
	IReadOnlyList<IReadOnlySet<ISeasonAffix>> Choices,
	int RerollsLeft
)
{
	public AffixChoiceMenuConfig WithChoices(IReadOnlyList<IReadOnlySet<ISeasonAffix>> choices)
		=> new(Season, Incremental, choices, RerollsLeft);

	public AffixChoiceMenuConfig WithRerollsLeft(int rerollsLeft)
		=> new(Season, Incremental, Choices, rerollsLeft);
};