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
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Newtonsoft.Json;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float ThunderChance { get; internal set; } = 2f;
}

internal sealed class ThunderAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;

	private static string ShortID => "Thunder";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description", new { Chance = $"{Mod.Config.ThunderChance:0.##}x" });
	public TextureRectangle Icon => new(Game1.mouseCursors, new(413, 346, 13, 13));

	public ThunderAffix() : base(ShortID, "neutral") { }

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> Mod.Helper.ModRegistry.IsLoaded("Shockah.SafeLightning") ? 0 : 1;

	public double GetProbabilityWeight(OrdinalSeason season)
	{
		if (Mod.Config.ChoicePeriod == AffixSetChoicePeriod.Day)
			return 0;
		return season.Season switch
		{
			Season.Spring or Season.Fall => 0.5,
			Season.Summer => 1,
			Season.Winter => 0,
			_ => throw new ArgumentException($"{nameof(Season)} has an invalid value."),
		};
	}

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.CropsAspect, VanillaSkill.FlowersAspect, VanillaSkill.FishingAspect };

	public void OnRegister()
		=> Apply(Mod.Harmony);

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);
		helper.AddNumberOption($"{I18nPrefix}.config.chance", () => Mod.Config.ThunderChance, min: 0.25f, max: 4f, interval: 0.05f, value => $"{value:0.##}x");
	}

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		var game1Type = typeof(Game1);
		var newDayAfterFadeEnumeratorType = game1Type.GetNestedTypes(BindingFlags.NonPublic)
			.FirstOrDefault(t => t.Name.StartsWith("<_newDayAfterFade>") && AccessTools.Method(t, "MoveNext") is not null);

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(newDayAfterFadeEnumeratorType, "MoveNext"),
			transpiler: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Game1_newDayAfterFade_MoveNext_Transpiler)))
		);
	}

	private static IEnumerable<CodeInstruction> Game1_newDayAfterFade_MoveNext_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
	{
		try
		{
			var newLabel = il.DefineLabel();

			return new SequenceBlockMatcher<CodeInstruction>(instructions)
				.Find(
					ILMatches.Instruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Game1), nameof(Game1.random))),
					ILMatches.Call("NextDouble"),
					ILMatches.Instruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Game1), nameof(Game1.chanceToRainTomorrow))),
					ILMatches.BgeUn
				)
				.PointerMatcher(SequenceMatcherRelativeElement.First)
				.ExtractLabels(out var labels)
				.Insert(
					SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.JustInsertion,
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThunderAffix), nameof(Game1_newDayAfterFade_MoveNext_Transpiler_ModifyChanceToRainTomorrow))).WithLabels(labels)
				)
				.AllElements();
		}
		catch (Exception ex)
		{
			Mod.Monitor.Log($"Could not patch method {originalMethod} - {Mod.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
			return instructions;
		}
	}

	public static void Game1_newDayAfterFade_MoveNext_Transpiler_ModifyChanceToRainTomorrow()
	{
		if (!Mod.IsAffixActive(a => a is ThunderAffix))
			return;
		Game1.chanceToRainTomorrow *= Mod.Config.ThunderChance;
	}
}