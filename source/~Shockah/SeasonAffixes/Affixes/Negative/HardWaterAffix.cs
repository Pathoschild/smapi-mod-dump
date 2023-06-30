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
using Shockah.Kokoro;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

internal sealed class HardWaterAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;

	private static string ShortID => "HardWater";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(368, 384, 16, 16));

	public HardWaterAffix() : base(ShortID, "negative") { }

	public int GetPositivity(OrdinalSeason season)
		=> 0;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.CropsAspect, VanillaSkill.FlowersAspect };

	public double GetProbabilityWeight(OrdinalSeason season)
	{
		if (Mod.Config.ChoicePeriod == AffixSetChoicePeriod.Day)
			return 0;
		bool greenhouseUnlocked = Game1.getAllFarmers().Any(p => p.mailReceived.Contains("ccVault") || p.mailReceived.Contains("jojaVault"));
		bool gingerIslandUnlocked = Game1.getAllFarmers().Any(p => p.mailReceived.Contains("willyBackRoomInvitation"));
		bool isWinter = season.Season == Season.Winter;
		return isWinter && !greenhouseUnlocked && !gingerIslandUnlocked ? 0 : 1;
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
			original: () => AccessTools.Method(typeof(SObject), nameof(SObject.IsSprinkler)),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(SObject_IsSprinkler_Postfix)))
		);
	}

	private static void SObject_IsSprinkler_Postfix(ref bool __result)
	{
		if (!Mod.IsAffixActive(a => a is HardWaterAffix))
			return;
		__result = false;
	}
}