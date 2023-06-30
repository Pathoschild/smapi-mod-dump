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
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

internal sealed class OvergrowthAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;
	private static readonly int SpawnDelay = 250;

	private static string ShortID => "Overgrowth";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(288, 256, 16, 16));

	public OvergrowthAffix() : base(ShortID, "positive") { }

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 0;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.CropsAspect, VanillaSkill.FlowersAspect, VanillaSkill.GatheringAspect };

	public void OnRegister()
		=> Apply(Mod.Harmony);

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(Crop), nameof(Crop.harvest)),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Crop_harvest_Postfix)))
		);
	}

	private static void Crop_harvest_Postfix(int xTile, int yTile, HoeDirt soil, ref bool __result)
	{
		if (!__result)
			return;
		if (!Mod.IsAffixActive(a => a is OvergrowthAffix))
			return;

		var itemToSpawn = GetItemToSpawn(soil.currentLocation);
		if (itemToSpawn is null)
			return;

		var location = soil.currentLocation;
		Vector2 point = new(xTile, yTile);
		Poof(location, point);
		DelayedAction.functionAfterDelay(() =>
		{
			location.removeEverythingExceptCharactersFromThisTile(xTile, yTile);
			location.dropObject(new SObject(point, itemToSpawn.Value, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: true), point * 64, Game1.viewport, initialPlacement: true);
		}, SpawnDelay);
	}

	private static int? GetItemToSpawn(GameLocation location)
	{
		var season = (Season)Utility.getSeasonNumber(location.GetSeasonForLocation());
		WeightedRandom<int> weighted = new();
		PopulateForage(weighted, location.Name, season);

		if (weighted.Items.Count == 0)
		{
			PopulateForage(weighted, "BusStop", season);
			PopulateForage(weighted, "Forest", season);
			PopulateForage(weighted, "Town", season);
			PopulateForage(weighted, "Mountain", season);
			PopulateForage(weighted, "Backwoods", season);
			PopulateForage(weighted, "Railroad", season);
		}
		return weighted.Items.Count == 0 ? null : weighted.Next(Game1.random);
	}

	private static void PopulateForage(WeightedRandom<int> weighted, string locationName, Season season)
	{
		var data = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
		if (!data.TryGetValue(locationName, out var rawData))
			return;

		string rawSeasonData = rawData.Split('/')[(int)season];
		if (rawSeasonData == "-1")
			return;

		string[] split = rawSeasonData.Split(" ");
		for (int i = 0; i < split.Length; i += 2)
			weighted.Add(new(double.Parse(split[i + 1]), int.Parse(split[i])));
	}

	private static void Poof(GameLocation location, Vector2 point)
	{
		var sprite = new TemporaryAnimatedSprite(
			textureName: Game1.mouseCursorsName,
			sourceRect: new Rectangle(464, 1792, 16, 16),
			animationInterval: 120f,
			animationLength: 5,
			numberOfLoops: 0,
			position: point * Game1.tileSize,
			flicker: false,
			flipped: Game1.random.NextBool(),
			layerDepth: 1f,
			alphaFade: 0.01f,
			color: Color.White,
			scale: Game1.pixelZoom,
			scaleChange: 0.01f,
			rotation: 0f,
			rotationChange: 0f
		)
		{
			light = true
		};
		GameExt.Multiplayer.broadcastSprites(location, sprite);
	}
}