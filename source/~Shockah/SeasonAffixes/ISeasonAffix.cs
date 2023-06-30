/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.Kokoro.UI;
using StardewModdingAPI;
using System.Collections.Generic;

#nullable enable

namespace Shockah.SeasonAffixes;

public interface ISeasonAffix
{
	string UniqueID { get; }
	string LocalizedName { get; }
	string LocalizedDescription { get; }
	TextureRectangle Icon { get; }

	void OnRegister() { }
	void OnUnregister() { }

	void OnActivate(AffixActivationContext context) { }
	void OnDeactivate(AffixActivationContext context) { }

	void SetupConfig(IManifest manifest) { }
	void OnSaveConfig() { }

	int GetPositivity(OrdinalSeason season);
	int GetNegativity(OrdinalSeason season);

	IReadOnlySet<string> Tags
		=> new HashSet<string>();

	double GetProbabilityWeight(OrdinalSeason season)
		=> 1;
}