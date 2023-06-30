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
using System.Reflection;
using System.Reflection.Emit;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public int SilenceFriendshipGain { get; internal set; } = 0;
}

internal sealed class SilenceAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;

	private static string ShortID => "Silence";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.emoteSpriteSheet, new(32, 144, 16, 16));

	public SilenceAffix() : base(ShortID, "negative") { }

	public int GetPositivity(OrdinalSeason season)
		=> 0;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { new SpaceCoreSkill("drbirbdev.Socializing").UniqueID };

	public void OnRegister()
		=> Apply(Mod.Harmony);

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);
		helper.AddNumberOption($"{I18nPrefix}.config.friendshipGain", () => Mod.Config.SilenceFriendshipGain, min: 0, max: 250, interval: 10);
	}

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
			transpiler: new HarmonyMethod(AccessTools.Method(GetType(), nameof(NPC_checkAction_Transpiler)))
		);
	}

	private static IEnumerable<CodeInstruction> NPC_checkAction_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
	{
		try
		{
			var newLabel = il.DefineLabel();

			return new SequenceBlockMatcher<CodeInstruction>(instructions)
				.AsGuidAnchorable()
				.Find(
					ILMatches.Ldarg(1),
					ILMatches.Call("get_CanMove"),
					ILMatches.Brtrue.WithAutoAnchor(out Guid branchAnchor),
					ILMatches.LdcI4(0),
					ILMatches.Instruction(OpCodes.Ret)
				)
				.PointerMatcher(branchAnchor)
				.ExtractBranchTarget(out var branchTarget)
				.Replace(new CodeInstruction(OpCodes.Brtrue, newLabel))
				.Find(
					SequenceBlockMatcherFindOccurence.First, SequenceMatcherRelativeBounds.WholeSequence,
					new ElementMatch<CodeInstruction>($"{{instruction with label {branchTarget}}}", i => i.labels.Contains(branchTarget))
				)
				.PointerMatcher(SequenceMatcherRelativeElement.First)
				.Insert(
					SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.JustInsertion,

					new CodeInstruction(OpCodes.Ldarg_0).WithLabels(newLabel),
					new CodeInstruction(OpCodes.Ldarg_1),
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SilenceAffix), nameof(NPC_checkAction_Transpiler_SilenceOrContinue))),
					new CodeInstruction(OpCodes.Brtrue, branchTarget),
					new CodeInstruction(OpCodes.Ldc_I4_1),
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

	public static bool NPC_checkAction_Transpiler_SilenceOrContinue(NPC npc, Farmer player)
	{
		if (!Mod.IsAffixActive(a => a is SilenceAffix))
			return true;

		npc.grantConversationFriendship(player, Mod.Config.SilenceFriendshipGain);
		if (!player.isEmoting)
			player.doEmote(40);
		if (!npc.isEmoting)
			npc.doEmote(8);
		npc.shake(250);

		return false;
	}
}