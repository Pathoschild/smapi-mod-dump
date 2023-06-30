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
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float TreasuresChance { get; internal set; } = 0.25f;
	[JsonProperty] public float TreasuresChanceWithEnchantment { get; internal set; } = 0.5f;
}

internal sealed class TreasuresAffix : BaseSeasonAffix, ISeasonAffix
{
	private const int ArtifactTroveID = 275;

	private static bool IsHarmonySetup = false;

	private static string ShortID => "Treasures";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description", new { Chance = $"{(int)(Mod.Config.TreasuresChance * 100):0.##}%" });
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(176, 176, 16, 16));

	public TreasuresAffix() : base(ShortID, "positive") { }

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 0;

	public void OnRegister()
		=> Apply(Mod.Harmony);

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);
		helper.AddNumberOption($"{I18nPrefix}.config.chance", () => Mod.Config.TreasuresChance, min: 0.01f, max: 1f, interval: 0.01f, value => $"{(int)(value * 100):0.##}%");
		helper.AddNumberOption($"{I18nPrefix}.config.chanceWithEnchantment", () => Mod.Config.TreasuresChanceWithEnchantment, min: 0.01f, max: 1f, interval: 0.01f, value => $"{(int)(value * 100):0.##}%");
	}

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatchVirtual(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(GameLocation), nameof(GameLocation.digUpArtifactSpot)),
			transpiler: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_digUpArtifactSpot_Transpiler)))
		);
	}

	private static IEnumerable<CodeInstruction> GameLocation_digUpArtifactSpot_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
	{
		try
		{
			return new SequenceBlockMatcher<CodeInstruction>(instructions)
				.PointerMatcher(SequenceMatcherRelativeElement.First)
				.CreateLabel(il, out var label)
				.Insert(
					SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.JustInsertion,

					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldarg_1),
					new CodeInstruction(OpCodes.Ldarg_2),
					new CodeInstruction(OpCodes.Ldarg_3),
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TreasuresAffix), nameof(GameLocation_digUpArtifactSpot_Transpiler_DropArtifactTroveOrContinue))),
					new CodeInstruction(OpCodes.Brtrue, label),
					new CodeInstruction(OpCodes.Ret)
				)
				.AllElements();
		}
		catch (Exception ex)
		{
			Mod.Monitor.Log($"Could not patch method {originalMethod} - {Mod.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
			return instructions;
		}
	}

	public static bool GameLocation_digUpArtifactSpot_Transpiler_DropArtifactTroveOrContinue(GameLocation location, int x, int y, Farmer? player)
	{
		if (!Mod.IsAffixActive(a => a is TreasuresAffix))
			return true;

		bool archaeologyEnchant = player is not null && player.CurrentTool is not null && player.CurrentTool.hasEnchantmentOfType<ArchaeologistEnchantment>();
		float chance = archaeologyEnchant ? Mod.Config.TreasuresChanceWithEnchantment : Mod.Config.TreasuresChance;
		Random random = new(x * 2000 + y + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed);
		if (random.NextDouble() > chance)
			return true;

		Game1.createObjectDebris(ArtifactTroveID, x, y, (player ?? Game1.player).UniqueMultiplayerID, location);
		return false;
	}
}