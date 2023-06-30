/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.SeasonAffixes;

public record AffixSetEntry(
	int Positive = 0,
	int Negative = 0,
	double Weight = 0
)
{
	internal bool IsValid()
		=> Positive >= 0 && Negative >= 0 && (Positive + Negative) >= 0 && Weight > 0;
}