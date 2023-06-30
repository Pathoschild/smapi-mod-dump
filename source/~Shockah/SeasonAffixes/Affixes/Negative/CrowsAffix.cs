/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Kokoro;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

internal sealed class CrowsAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;

	private static string ShortID => "Crows";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.content.Load<Texture2D>(Critter.critterTexture), new(134, 46, 21, 17));

	public CrowsAffix() : base(ShortID, "negative") { }

	public int GetPositivity(OrdinalSeason season)
		=> 0;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.CropsAspect, VanillaSkill.FlowersAspect };

	public double GetProbabilityWeight(OrdinalSeason season)
	{
		if (Mod.Config.ChoicePeriod == AffixSetChoicePeriod.Day)
			return 0;
		if (!Mod.Config.WinterCrops && season.Season == Season.Winter)
			return 0;
		return 1;
	}

	public void OnRegister()
		=> Apply(Mod.Harmony);

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(SObject), nameof(SObject.IsScarecrow)),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(SObject_IsScarecrow_Postfix)))
		);
	}

	private static void SObject_IsScarecrow_Postfix(ref bool __result)
	{
		if (!Mod.IsAffixActive(a => a is CrowsAffix))
			return;
		__result = false;
	}
}