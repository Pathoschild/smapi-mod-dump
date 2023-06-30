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
using Microsoft.Xna.Framework;
using Shockah.Kokoro;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewValley;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

internal sealed class HurricaneAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;
	private static readonly WeakCounter<GameLocation> DayUpdateCallCounter = new();

	private static string ShortID => "Hurricane";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(368, 224, 16, 16));

	public HurricaneAffix() : base(ShortID, "negative") { }

	public int GetPositivity(OrdinalSeason season)
		=> 0;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

	public double GetProbabilityWeight(OrdinalSeason season)
		=> Mod.Config.ChoicePeriod == AffixSetChoicePeriod.Day ? 0 : 1;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.GatheringAspect };

	public void OnRegister()
		=> Apply(Mod.Harmony);

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatchVirtual(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(GameLocation), nameof(GameLocation.DayUpdate)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_DayUpdate_Prefix)), priority: Priority.First),
			finalizer: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_DayUpdate_Finalizer)), priority: Priority.Last)
		);
		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(GameLocation), nameof(GameLocation.dropObject), new Type[] { typeof(SObject), typeof(Vector2), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(Farmer) }),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_dropObject_Prefix)))
		);

		if (Mod.Helper.ModRegistry.IsLoaded("Esca.FarmTypeManager"))
		{
			harmony.TryPatch(
				monitor: Mod.Monitor,
				original: () => AccessTools.Method(AccessTools.Inner(AccessTools.TypeByName("FarmTypeManager.ModEntry, FarmTypeManager"), "Generation"), "ForageGeneration"),
				prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(FarmTypeManager_ModEntry_Generation_ForageGeneration_Prefix)))
			);
		}
	}

	private static void GameLocation_DayUpdate_Prefix(GameLocation __instance)
		=> DayUpdateCallCounter.Push(__instance);

	private static void GameLocation_DayUpdate_Finalizer(GameLocation __instance)
		=> DayUpdateCallCounter.Pop(__instance);

	private static bool GameLocation_dropObject_Prefix(GameLocation __instance, SObject obj, bool initialPlacement)
	{
		if (!initialPlacement)
			return true;
		if (!(obj.Category is SObject.FruitsCategory or SObject.VegetableCategory or SObject.GreensCategory or SObject.sellAtFishShopCategory or SObject.FishCategory))
			return true;
		if (DayUpdateCallCounter.Get(__instance) == 0)
			return true;
		if (!Mod.IsAffixActive(a => a is HurricaneAffix))
			return true;
		return false;
	}

	private static bool FarmTypeManager_ModEntry_Generation_ForageGeneration_Prefix()
	{
		return !Mod.IsAffixActive(a => a is HurricaneAffix);
	}
}