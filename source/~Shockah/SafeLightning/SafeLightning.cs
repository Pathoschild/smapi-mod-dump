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
using Netcode;
using Shockah.CommonModCode;
using Shockah.CommonModCode.GMCM;
using Shockah.CommonModCode.IL;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Shockah.SafeLightning
{
	public class SafeLightning: Mod
	{
		private enum StrikeTargetType { LightningRod, Tile, FruitTree }

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

		private static IEnumerable<CodeInstruction> Utility_performLightningUpdate_Transpiler(IEnumerable<CodeInstruction> enumerableInstructions, ILGenerator il)
		{
			var instructions = enumerableInstructions.ToList();

			// IL to find:
			// IL_023d: isinst StardewValley.TerrainFeatures.FruitTree
			// IL_0242: brtrue IL_036e
			var worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.opcode == OpCodes.Isinst && Equals(i.operand, typeof(FruitTree)),
				i => i.IsBrtrue()
			});
			if (worker is null)
			{
				Instance.Monitor.Log($"Could not patch methods - Safe Lightning probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}

			worker.Postfix(new[]
			{
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SafeLightning), nameof(Utility_performLightningUpdate_Transpiler_ShouldContinueWithTile))),
				new CodeInstruction(OpCodes.Brfalse, worker[1].operand)
			});

			// IL to find:
			// IL_0375: isinst StardewValley.TerrainFeatures.FruitTree
			// IL_037a: brfalse.s IL_03df
			worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.opcode == OpCodes.Isinst && Equals(i.operand, typeof(FruitTree)),
				i => i.IsBrfalse()
			}, startIndex: worker.EndIndex);
			if (worker is null)
			{
				Instance.Monitor.Log($"Could not patch methods - Safe Lightning probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}

			worker.Postfix(new[]
			{
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SafeLightning), nameof(Utility_performLightningUpdate_Transpiler_ShouldContinueWithFruitTree))),
				new CodeInstruction(OpCodes.Brfalse, worker[1].operand)
			});

			int supposedToFind = 3;
			int nextStartIndex = 0;
			while (true)
			{
				// IL to find (example, there are 3 occurences, we're finding all 3):
				// IL_01ba: ldloc.2 / ldloc.11
				// IL_01bb: callvirt instance void class Netcode.AbstractNetEvent1`1<class StardewValley.Farm/LightningStrikeEvent>::Fire(!0)
				worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
				{
					i => i.IsLdloc(),
					i => i.opcode == OpCodes.Callvirt && Equals(i.operand, AccessTools.Method(typeof(AbstractNetEvent1<Farm.LightningStrikeEvent>), nameof(AbstractNetEvent1<Farm.LightningStrikeEvent>.Fire)))
				}, startIndex: nextStartIndex);
				if (worker is null)
				{
					if (supposedToFind < 0)
					{
						Instance.Monitor.Log($"Found more matching IL than expected. Safe Lightning may behave incorrectly.", LogLevel.Warn);
					}
					else if (supposedToFind > 0)
					{
						Instance.Monitor.Log($"Could not patch methods - Safe Lightning probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
						return instructions;
					}
					break;
				}
				supposedToFind--;

				worker.Insert(1, new[]
				{
					new CodeInstruction(OpCodes.Dup),
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SafeLightning), nameof(Utility_performLightningUpdate_Transpiler_ModifyStrikeEvent)))
				});
				if (worker[4].opcode == OpCodes.Ret && nextStartIndex == 0) // originally at 2, +2 instructions we added
				{
					worker.Insert(1, new[]
					{
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SafeLightning), nameof(Utility_performLightningUpdate_Transpiler_WillStrikeLightningRod)))
					});
				}

				nextStartIndex = worker.EndIndex;
			}

			return instructions;
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
