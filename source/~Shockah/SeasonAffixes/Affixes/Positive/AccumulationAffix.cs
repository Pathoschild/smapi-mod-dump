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
using Netcode;
using Newtonsoft.Json;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float AccumulationChance { get; internal set; } = 0.3f;
}

internal sealed class AccumulationAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;

	private static readonly Lazy<Func<Tree, NetBool>> TreeDestroyGetter = new(() => AccessTools.Field(typeof(Tree), "destroy").EmitInstanceGetter<Tree, NetBool>());
	private static readonly Lazy<Func<Tree, NetBool>> TreeFallingGetter = new(() => AccessTools.Field(typeof(Tree), "falling").EmitInstanceGetter<Tree, NetBool>());
	private static readonly Lazy<Func<Tree, NetLong>> TreeLastPlayerToHitGetter = new(() => AccessTools.Field(typeof(Tree), "lastPlayerToHit").EmitInstanceGetter<Tree, NetLong>());

	private static string ShortID => "Accumulation";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description", new { Chance = $"{(int)(Mod.Config.AccumulationChance * 100):0.##}%" });
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(64, 480, 16, 16));

	public AccumulationAffix() : base(ShortID, "positive") { }

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 0;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.WoodcuttingAspect, VanillaSkill.TappingAspect };

	public void OnRegister()
		=> Apply(Mod.Harmony);

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);
		helper.AddNumberOption($"{I18nPrefix}.config.chance", () => Mod.Config.AccumulationChance, min: 0.01f, max: 1f, interval: 0.01f, value => $"{(int)(value * 100):0.##}%");
	}

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(Tree), nameof(Tree.tickUpdate)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Tree_tickUpdate_Prefix))),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Tree_tickUpdate_Postfix)))
		);
	}

	private static void Tree_tickUpdate_Prefix(Tree __instance, ref bool __state)
	{
		__state = TreeFallingGetter.Value(__instance);
	}

	private static void Tree_tickUpdate_Postfix(Tree __instance, Vector2 tileLocation, GameLocation location, ref bool __state)
	{
		if (TreeDestroyGetter.Value(__instance).Value)
			return;
		if (TreeFallingGetter.Value(__instance) || !__state)
			return;
		if (!Mod.IsAffixActive(a => a is AccumulationAffix))
			return;

		var playerToDropFor = Game1.getFarmer(TreeLastPlayerToHitGetter.Value(__instance).Value) ?? GameExt.GetHostPlayer();
		if (Game1.player != playerToDropFor)
			return;
		if (Game1.random.NextDouble() >= Mod.Config.AccumulationChance)
			return;

		SObject fakeTapper = new(Vector2.Zero, 105);
		__instance.UpdateTapperProduct(fakeTapper);
		if (fakeTapper.heldObject.Value is null)
			return;

		Game1.createItemDebris(fakeTapper.heldObject.Value, new Vector2(tileLocation.X + (__instance.shakeLeft.Value ? (-4) : 4), tileLocation.Y) * Game1.tileSize, -1, location, playerToDropFor.getStandingY() - 32);
	}
}