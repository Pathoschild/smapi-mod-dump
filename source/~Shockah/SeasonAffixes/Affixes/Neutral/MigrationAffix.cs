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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Shockah.SeasonAffixes;

internal sealed class MigrationAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;
	private static readonly List<WeakReference<GameLocation>> LocationsDuringGetFish = new();

	private static string ShortID => "Migration";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(32, 464, 16, 16));

	public MigrationAffix() : base(ShortID, "neutral") { }

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.FishingAspect };
	
	public void OnRegister()
		=> Apply(Mod.Harmony);

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
			transpiler: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_getFish_Transpiler)))
		);

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(Beach), nameof(Beach.getFish)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocationSubclass_getFish_Prefix)), priority: Priority.First),
			finalizer: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocationSubclass_getFish_Finalizer)), priority: Priority.Last)
		);

		harmony.TryPatchVirtual(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsUsingMagicBait)),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_IsUsingMagicBait_Postfix)))
		);
	}

	private static IEnumerable<CodeInstruction> GameLocation_getFish_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
	{
		// TODO: Shrike improvements: finding a matching stloc/ldloc to a given stloc/ldloc

		bool foundInstruction = false;
		int toSkip = 0;

		foreach (var instruction in instructions)
		{
			if (!foundInstruction && instruction.opcode == OpCodes.Ldloc_S && ((instruction.operand is int intValue && intValue == 4) || (instruction.operand is sbyte sbyteValue && sbyteValue == 4) || (instruction.operand is LocalBuilder builder && builder.LocalIndex == 4)))
			{
				foundInstruction = true;
				toSkip = 1;

				yield return new CodeInstruction(OpCodes.Ldloc_S, (sbyte)4);
				yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MigrationAffix), nameof(GameLocation_getFish_Transpiler_ShouldCountAsMagicBait)));
			}

			if (toSkip > 0)
				toSkip--;
			else
				yield return instruction;
		}

		if (!foundInstruction || toSkip != 0)
			Mod.Monitor.Log($"Could not patch method {originalMethod} - {Mod.ModManifest.Name} probably won't work.\nReason: could not find instruction sequence.", LogLevel.Error);
	}

	public static bool GameLocation_getFish_Transpiler_ShouldCountAsMagicBait(bool isUsingMagicBait)
		=> isUsingMagicBait || Mod.IsAffixActive(a => a is MigrationAffix);

	private static void GameLocationSubclass_getFish_Prefix(GameLocation __instance)
	{
		LocationsDuringGetFish.Add(new(__instance));
	}

	private static void GameLocationSubclass_getFish_Finalizer(GameLocation __instance)
	{
		int? index = LocationsDuringGetFish.FirstIndex(r => r.TryGetTarget(out var location) && location == __instance);
		if (index is not null)
			LocationsDuringGetFish.RemoveAt(index.Value);
	}

	private static void GameLocation_IsUsingMagicBait_Postfix(GameLocation __instance, ref bool __result)
	{
		if (!Mod.IsAffixActive(a => a is MigrationAffix))
			return;
		if (!LocationsDuringGetFish.Any(r => r.TryGetTarget(out var location) && location == __instance))
			return;
		__result = true;
	}
}