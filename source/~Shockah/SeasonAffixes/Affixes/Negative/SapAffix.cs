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
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

internal sealed class SapAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;

	private static string ShortID => "Sap";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(320, 48, 16, 16));

	public SapAffix() : base(ShortID, "negative") { }

	public int GetPositivity(OrdinalSeason season)
		=> 0;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

	public double GetProbabilityWeight(OrdinalSeason season)
		=> Mod.Config.ChoicePeriod == AffixSetChoicePeriod.Day ? 0 : 1;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.TappingAspect };

	public void OnRegister()
		=> Apply(Mod.Harmony);

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(Tree), nameof(Tree.UpdateTapperProduct)),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Tree_UpdateTapperProduct_Postfix)))
		);
	}

	private static void Tree_UpdateTapperProduct_Postfix(SObject tapper_instance)
	{
		if (!Mod.IsAffixActive(a => a is SapAffix))
			return;

		float timeMultiplier = tapper_instance.ParentSheetIndex == 264 ? 0.5f : 1f;
		Random random = new((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + 73137);
		tapper_instance.heldObject.Value = new SObject(92, random.Next(3, 8));
		tapper_instance.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int)Math.Max(1.0, Math.Floor(1f * timeMultiplier)));
	}
}