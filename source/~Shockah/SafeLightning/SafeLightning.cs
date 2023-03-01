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
using Nanoray.Shrike.Harmony;
using Nanoray.Shrike;
using Netcode;
using Shockah.CommonModCode.GMCM;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro;

namespace Shockah.SafeLightning
{
	public class SafeLightning : Mod
	{
		private enum StrikeTargetType
		{
			LightningRod,
			Tile,
			FruitTree
		}

		private static SafeLightning Instance = null!;
		internal ModConfig Config { get; private set; } = null!;

		private StrikeTargetType? LastTargetType;
		private bool DidPreventLast;

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;

			var harmony = new Harmony(ModManifest.UniqueID);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(Utility), nameof(Utility.performLightningUpdate)),
				prefix: new HarmonyMethod(typeof(SafeLightning), nameof(Utility_performLightningUpdate_Prefix)),
				transpiler: new HarmonyMethod(typeof(SafeLightning), nameof(Utility_performLightningUpdate_Transpiler))
			);
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			SetupConfig();
		}

		private void SetupConfig()
		{
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;
			GMCMI18nHelper helper = new(api, ModManifest, Helper.Translation);

			api.Register(
				ModManifest,
				reset: () => Config = new(),
				save: () => Helper.WriteConfig(Config)
			);

			helper.AddBoolOption("config.safeTiles", () => Config.SafeTiles);
			helper.AddBoolOption("config.safeFruitTrees", () => Config.SafeFruitTrees);
			helper.AddEnumOption("config.bigLightningBehavior", () => Config.BigLightningBehavior);
		}

		private static void Utility_performLightningUpdate_Prefix()
		{
			Instance.LastTargetType = null;
			Instance.DidPreventLast = false;
		}

		private static IEnumerable<CodeInstruction> Utility_performLightningUpdate_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.AsAnchorable<CodeInstruction, Guid, Guid, SequencePointerMatcher<CodeInstruction>, SequenceBlockMatcher<CodeInstruction>>()

					.Find(
						ILMatches.Isinst<FruitTree>(),
						ILMatches.Brtrue.WithAutoAnchor(out Guid branchAnchor)
					)
					.AnchorBlock(out Guid findBlock)
					.MoveToPointerAnchor(branchAnchor)
					.ExtractBranchTarget(out Label branchTarget)
					.MoveToBlockAnchor(findBlock)
					.Insert(
						SequenceMatcherPastBoundsDirection.After, true,

						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SafeLightning), nameof(Utility_performLightningUpdate_Transpiler_ShouldContinueWithTile))),
						new CodeInstruction(OpCodes.Brfalse, branchTarget)
					)

					.Find(
						ILMatches.Isinst<FruitTree>(),
						ILMatches.Brfalse.WithAutoAnchor(out branchAnchor)
					)
					.AnchorBlock(out findBlock)
					.MoveToPointerAnchor(branchAnchor)
					.ExtractBranchTarget(out branchTarget)
					.MoveToBlockAnchor(findBlock)
					.Insert(
						SequenceMatcherPastBoundsDirection.After, true,

						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SafeLightning), nameof(Utility_performLightningUpdate_Transpiler_ShouldContinueWithFruitTree))),
						new CodeInstruction(OpCodes.Brfalse, branchTarget)
					)

					.ForEach(
						SequenceMatcherRelativeBounds.WholeSequence,
						new IElementMatch<CodeInstruction>[]
						{
							ILMatches.Ldloc<Farm.LightningStrikeEvent>(originalMethod.GetMethodBody()!.LocalVariables),
							ILMatches.Call(AccessTools.Method(typeof(AbstractNetEvent1<Farm.LightningStrikeEvent>), nameof(AbstractNetEvent1<Farm.LightningStrikeEvent>.Fire))),
							ElementMatch<CodeInstruction>.True
						},
						matcher =>
						{
							matcher = matcher
								.PointerMatcher(SequenceMatcherRelativeElement.First)
								.Advance()
								.Insert(
									SequenceMatcherPastBoundsDirection.Before, true,

									new CodeInstruction(OpCodes.Dup),
									new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SafeLightning), nameof(Utility_performLightningUpdate_Transpiler_ModifyStrikeEvent)))
								)
								.BlockMatcher(SequenceMatcherRelativeBounds.WholeSequence);

							if (ILMatches.Instruction(OpCodes.Ret).Matches(matcher.PointerMatcher(SequenceMatcherRelativeElement.Last).Element()))
							{
								matcher = matcher
									.PointerMatcher(SequenceMatcherRelativeElement.First)
									.Advance()
									.Insert(
										SequenceMatcherPastBoundsDirection.Before, true,
										new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SafeLightning), nameof(Utility_performLightningUpdate_Transpiler_WillStrikeLightningRod)))
									)
									.BlockMatcher(SequenceMatcherRelativeBounds.WholeSequence);
							}

							return matcher;
						},
						minExpectedOccurences: 3,
						maxExpectedOccurences: 3
					)

					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}

		public static bool Utility_performLightningUpdate_Transpiler_ShouldContinueWithTile()
		{
			Instance.LastTargetType = StrikeTargetType.Tile;
			bool shouldPrevent = Instance.Config.SafeTiles;
			Instance.DidPreventLast = shouldPrevent;
			return !shouldPrevent;
		}

		public static bool Utility_performLightningUpdate_Transpiler_ShouldContinueWithFruitTree()
		{
			Instance.LastTargetType = StrikeTargetType.FruitTree;
			bool shouldPrevent = Instance.Config.SafeFruitTrees;
			Instance.DidPreventLast = shouldPrevent;
			return !shouldPrevent;
		}

		public static void Utility_performLightningUpdate_Transpiler_WillStrikeLightningRod()
		{
			Instance.LastTargetType = StrikeTargetType.LightningRod;
		}

		public static void Utility_performLightningUpdate_Transpiler_ModifyStrikeEvent(Farm.LightningStrikeEvent @event)
		{
			switch (Instance.Config.BigLightningBehavior)
			{
				case BigLightningBehavior.Never:
					@event.smallFlash = true;
					@event.bigFlash = false;
					break;
				case BigLightningBehavior.WhenSupposedToStrike:
					break;
				case BigLightningBehavior.WhenActuallyStrikes:
					if (@event.bigFlash && (Instance.DidPreventLast || Instance.LastTargetType is null))
					{
						@event.smallFlash = true;
						@event.bigFlash = false;
					}
					break;
				case BigLightningBehavior.Always:
					@event.smallFlash = false;
					@event.bigFlash = true;
					break;
			}
		}
	}
}
