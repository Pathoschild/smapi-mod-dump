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

namespace Shockah.SeasonAffixes;

internal sealed class DroughtAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;

	private static string ShortID => "Drought";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.mouseCursors, new(413, 333, 13, 13));

	public DroughtAffix() : base(ShortID, "negative") { }

	public int GetPositivity(OrdinalSeason season)
		=> 0;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.CropsAspect, VanillaSkill.FlowersAspect, VanillaSkill.FishingAspect };

	public double GetProbabilityWeight(OrdinalSeason season)
	{
		if (Mod.Config.ChoicePeriod == AffixSetChoicePeriod.Day)
			return 0;
		if (season.Season == Season.Winter)
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
			original: () => AccessTools.Method(typeof(Game1), nameof(Game1.getWeatherModificationsForDate)),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Game1_getWeatherModificationsForDate_Postfix)))
		);
	}

	private static void Game1_getWeatherModificationsForDate_Postfix(ref int __result)
	{
		if (!Mod.IsAffixActive(a => a is DroughtAffix))
			return;
		if (__result is Game1.weather_rain or Game1.weather_lightning)
			__result = Game1.weather_sunny;
	}
}