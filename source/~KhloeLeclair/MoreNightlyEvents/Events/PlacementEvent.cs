/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.Common;
using Leclair.Stardew.MoreNightlyEvents.Models;
using Leclair.Stardew.MoreNightlyEvents.Patches;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.Internal;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.MoreNightlyEvents.Events;

public class PlacementEvent : BaseFarmEvent<PlacementEventData> { 

	private int timer;

	private bool playedSound;
	private bool showedMessage;
	private bool finished;

	private GameLocation? targetMap;
	private Vector2[]? targetLocations;
	private PlacementItemData? targetData;
	private Point? targetSize;

	public PlacementEvent(string key, PlacementEventData? data = null) : base(key, data) {

	}

	public static bool IsTileOpenBesidesTerrainFeatures(GameLocation location, Vector2 tile) {
		Rectangle bounds = new Rectangle((int)tile.X * 64, (int)tile.Y * 64, 64, 64);

		foreach(var building in location.buildings) { 
			if (building.intersects(bounds))
				return false;
		}

		foreach(var clump in location.resourceClumps) {
			if (clump.getBoundingBox().Intersects(bounds))
				return false;
		}

		foreach(var animal in location.animals.Values) {
			if (animal.GetBoundingBox().Intersects(bounds))
				return false;
		}

		foreach(var feature in location.terrainFeatures.Values) {
			if (feature.getBoundingBox().Intersects(bounds))
				return false;
		}

		foreach(var feature in location.largeTerrainFeatures) {
			if (feature.getBoundingBox().Intersects(bounds))
				return false;
		}

		if (location.objects.ContainsKey(tile))
			return false;

		return location.isTilePassable(tile.ToLocation(), Game1.viewport);
	}

	public List<Vector2> CalculatePossiblePositions(GameLocation location, IEnumerable<Rectangle>? areas) {
		List<Vector2> result = new();
		var layer = location.Map.Layers[0];
		if (layer is null)
			return result;

		if (areas is null)
			result.AddRange(Vector2.Zero.IterArea(0, layer.LayerWidth, 0, layer.LayerHeight));
		else
			foreach(var area in areas) {
				foreach(var point in area.GetPoints()) {
					if (point.X >= 0 && point.X < layer.LayerWidth && point.Y >= 0 && point.Y < layer.LayerHeight)
						result.Add(point.ToVector2());
				}
			}

		return result;
	}

	public Vector2? GetTarget(GameLocation location, int width, int height, List<Vector2>? validPositions, HashSet<Vector2> invalid_places, int attempts = 10, Random? rnd = null, bool strict = false, Func<Vector2, bool>? extraValidCheck = null) {
		rnd ??= Game1.random;

		var layer = location.Map.Layers[0];
		if (layer is null)
			return null;

		while(attempts > 0) {
			int x;
			int y;
			if (validPositions is not null) {
				Vector2 pos = rnd.ChooseFrom(validPositions);
				x = (int) pos.X;
				y = (int) pos.Y;
			} else {
				x = rnd.Next(0, layer.LayerWidth);
				y = rnd.Next(0, layer.LayerHeight);
			}

			attempts--;

			bool valid = true;

			for(int i=0; i < width; i++) {
				for(int j = 0; j < height; j++) {
					Vector2 v = new(x + i, y + j);

					if (invalid_places.Contains(v) || (validPositions is not null && ! validPositions.Contains(v))) {
						valid = false;
						break;
					}

					// Loose placing requirements.
					if (! IsTileOpenBesidesTerrainFeatures(location, v) ||
						location.doesTileHaveProperty(x+i, y+j, "Water", "Back") != null
					) {
						valid = false;
						break;
					}

					// Stricter placing requirements.
					if (strict && (
						location.IsNoSpawnTile(v) ||
						location.doesEitherTileOrTileIndexPropertyEqual(x+i, y+j, "Spawnable", "Back", "F") ||
						! location.CanItemBePlacedHere(v) ||
						location.isBehindBush(v)
					)) {
						valid = false;
						break;
					}

					// Extra checks
					if (extraValidCheck != null && ! extraValidCheck(v)) {
						valid = false;
						break;
					}
				}

				if (!valid)
					break;
			}

			if (valid)
				return new Vector2(x, y);
		}

		return null;
	}

	#region FarmEvent

	public override bool setUp() {
		if (! LoadData())
			return true;

		Random rnd = Utility.CreateDaySaveRandom();

		var loc = GetLocation();
		if (loc is null || Data?.Output is null)
			return true;

		// What are we placing?
		targetData = null;
		GameStateQueryContext ctx = new(loc, null, null, null, rnd);
		foreach(var thing in Data.Output) {
			if (string.IsNullOrEmpty(thing.Condition) || GameStateQuery.CheckConditions(thing.Condition, ctx)) {
				targetData = thing;
				break;
			}
		}

		// If we didn't get a thing, we can't continue
		// so just stop now.
		if (targetData is null)
			return true;

		// How many things are we spawning?
		int min = Math.Max(1, targetData.MinStack);
		int max = Math.Max(min, targetData.MaxStack);

		int toSpawn = rnd.Next(min, max);

		// We need to figure out
		// 1. how big is the thing we're spawning
		// 2. can we place it anywhere
		bool strictPlacement = true;
		Func<Vector2, bool>? extraValidity = null;

		int x;
		int y;

		switch(targetData.Type) {
			case PlaceableType.Building:
				// We need this to be a buildable location.
				if (!loc.IsBuildableLocation())
					return true;

				x = 1;
				y = 1;

				var buildings = DataLoader.Buildings(Game1.content);
				if (targetData.RandomItemId != null && targetData.RandomItemId.Count > 0) {
					foreach(string id in targetData.RandomItemId)
						if (!string.IsNullOrEmpty(id) && buildings.TryGetValue(id, out var buildingData)) {
							x = Math.Max(x, buildingData.Size.X);
							y = Math.Max(y, buildingData.Size.Y);
						}
				} else if (!string.IsNullOrEmpty(targetData.ItemId) && buildings.TryGetValue(targetData.ItemId, out var buildingData)) {
					x = buildingData.Size.X;
					y = buildingData.Size.Y;

				} else {
					// We couldn't find it. Abort.
					return true;
				}

				targetSize = new(x, y);
				extraValidity = pos => loc.isBuildable(pos, true);
				break;

			case PlaceableType.ResourceClump:
				// We need a clump Id to process this.
				if (targetData.ClumpId < 0)
					return true;

				strictPlacement = targetData.ClumpStrictPlacement;
				targetSize = new(
					targetData.ClumpWidth,
					targetData.ClumpHeight
				);
				break;

			case PlaceableType.GiantCrop:
				// We need to check every single giant crop we have an ID for,
				// and use the biggest size.
				x = 1;
				y = 1;

				var crops = DataLoader.GiantCrops(Game1.content);
				if (targetData.RandomItemId != null && targetData.RandomItemId.Count > 0) {
					foreach (string id in targetData.RandomItemId)
						if (! string.IsNullOrEmpty(id) && crops.TryGetValue(id, out var cropData)) {
							x = Math.Max(x, cropData.TileSize.X);
							y = Math.Max(y, cropData.TileSize.Y);
						}

				} else if (! string.IsNullOrEmpty(targetData.ItemId) && crops.TryGetValue(targetData.ItemId, out var cropData)) {
					x = cropData.TileSize.X;
					y = cropData.TileSize.Y;

				} else {
					// We couldn't find it. Abort.
					return true;
				}

				targetSize = new(x, y);
				break;

			case PlaceableType.WildTree:
			case PlaceableType.FruitTree:
				// Trees are only 1x1, but they need a 3x3 to grow so
				// let's use the 3x3 size.
				targetSize = new(3, 3);
				break;

			case PlaceableType.Crop:
				// These are just 1x1.
				targetSize = new(1, 1);
				// ... but they require a HoeDirt, which requires diggable.
				extraValidity = pos =>
					loc.doesTileHaveProperty((int) pos.X, (int) pos.Y, "Diggable", "Back") != null &&
					loc.CanItemBePlacedHere(pos, itemIsPassable: true, CollisionMask.All, CollisionMask.None);
				break;

			case PlaceableType.Item:
			default:
				// These are just 1x1.
				targetSize = new(1, 1);
				break;
		}


		// Figure out where we're putting them.
		HashSet<Vector2> invalidPlaces = new();
		List<Vector2> locations = new();

		List<Vector2>? validPositions = targetData.SpawnAreas != null && targetData.SpawnAreas.Count > 0
			? CalculatePossiblePositions(loc, targetData.SpawnAreas)
			: null;

		// Nothing valid? Abort.
		if (validPositions != null && validPositions.Count == 0)
			return true;

		for(int i = 0; i < toSpawn; i++) {
			var pos = GetTarget(loc, targetSize.Value.X, targetSize.Value.Y, validPositions, invalidPlaces, attempts: 50, rnd: rnd, strict: strictPlacement, extraValidity);
			if (pos is not null) {
				locations.Add(pos.Value);
				for (x = 0; x < targetSize.Value.X; x++)
					for (y = 0; y < targetSize.Value.Y; y++)
						invalidPlaces.Add(new(pos.Value.X + x, pos.Value.Y + y));
			}
		}

		// If we don't have any targets, we can't spawn anything, so
		// we give up.
		targetLocations = locations.Count > 0 ? locations.ToArray() : null;
		if (targetLocations is null)
			return true;

		// Require a minimum amount.
		if (targetData.RequireMinimumSpots && targetLocations.Length < targetData.MinStack)
			return true;

		// But if we got this far, it's go time.
		targetMap = loc;

		Game1.freezeControls = true;
		return false;
	}

	public override void InterruptEvent() {
		finished = true;
	}

	public override bool tickUpdate(GameTime time) {
		timer += time.ElapsedGameTime.Milliseconds;
		if (timer > 1500f && !playedSound) {
			playedSound = true;
			if (! string.IsNullOrEmpty(Data?.SoundName) )
				Game1.playSound(Data.SoundName);
		}

		if (timer > (Data?.MessageDelay ?? 7000) && ! showedMessage) {
			showedMessage = true;
			if (Data?.Message == null)
				finished = true;
			else {
				Game1.pauseThenMessage(10, Translate(Data?.Message, Game1.player));
				Game1.afterDialogues = delegate {
					finished = true;
				};
			}
		}
		if (finished) {
			Game1.freezeControls = false;
			return true;
		}
		return false;
	}

	public override void draw(SpriteBatch b) {
		b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height), Color.Black);

		if (!showedMessage)
			b.Draw(Game1.mouseCursors_1_6,
				new Vector2(12f, Game1.viewport.Height - 12 - 76),
				new Rectangle(
					256 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0 / 100.0) * 19,
					413, 19, 19),
				Color.White,
				0f,
				Vector2.Zero,
				4f,
				SpriteEffects.None,
				1f
			);

	}

	private static bool IsTreeIdValid(string id, bool isFruitTree) {
		if (string.IsNullOrEmpty(id))
			return false;

		return isFruitTree
			? DataLoader.FruitTrees(Game1.content).ContainsKey(id)
			: DataLoader.WildTrees(Game1.content).ContainsKey(id);
	}

	public override void makeChangesToLocation() {
		if (!Game1.IsMasterGame || targetLocations is null || targetMap is null || targetData is null || targetSize is null)
			return;

		var mod = ModEntry.Instance;

		Random rnd = Utility.CreateDaySaveRandom();
		ItemQueryContext ctx = new(targetMap, null, rnd);

		string id;
		int growthStage;
		Item? targetItem = null;
		int offsetX;
		int offsetY;

		// Okay, it's go time.
		foreach(var pos in targetLocations) {
			switch(targetData.Type) {
				case PlaceableType.Building:
					// Pick a suitable Id.
					id = targetData.ItemId;
					if (targetData.RandomItemId is not null && targetData.RandomItemId.Count > 0)
						id = rnd.ChooseFrom(targetData.RandomItemId);

					string? skin = targetData.SkinId;
					if (targetData.RandomSkinId is not null && targetData.RandomSkinId.Count > 0)
						skin = rnd.ChooseFrom(targetData.RandomSkinId);

					var buildings = DataLoader.Buildings(Game1.content);
					if (string.IsNullOrEmpty(id) || !buildings.TryGetValue(id, out var buildingData)) {
						mod.Log($"Error loading building '{id}' for event: unable to find building data", StardewModdingAPI.LogLevel.Error);
						continue;
					}

					// Validate the skin.
					if (!string.IsNullOrEmpty(skin) && (buildingData.Skins is null || !buildingData.Skins.Where(s => s.Id == skin).Any())) {
						mod.Log($"Error loading building '{id}' for event: no such skin '{skin}'", StardewModdingAPI.LogLevel.Error);
						continue;
					}

					// Calculate the offset for this crop within our area.
					offsetX = (targetSize.Value.X - buildingData.Size.X) / 2;
					offsetY = (targetSize.Value.Y - buildingData.Size.Y) / 2;
					Vector2 finalPos = new(pos.X + offsetX, pos.Y + offsetY);

					Building bld = Building.CreateInstanceFromId(id, finalPos);
					if (bld is null) {
						mod.Log($"Error loading building '{id}' for event: unable to create instance", StardewModdingAPI.LogLevel.Error);
						continue;
					}

					bld.skinId.Value = skin;

					if (!targetMap.buildStructure(bld, finalPos, Game1.MasterPlayer, true)) {
						mod.Log($"Error placing building '{id}' for event: buildStructure failed", StardewModdingAPI.LogLevel.Error);
						continue;
					}

					bld.FinishConstruction();

					// Animals?
					if (targetData.Animals != null && targetData.Animals.Count > 0 && buildingData.ValidOccupantTypes != null && bld.GetIndoors() is AnimalHouse ahouse) {
						var animals = DataLoader.FarmAnimals(Game1.content);
						var validAnimals = animals
							.Where(pair => pair.Value.House != null && buildingData.ValidOccupantTypes.Contains(pair.Value.House))
							.Select(pair => pair.Key)
							.ToHashSet();

						foreach(var entry in targetData.Animals) {
							if (ahouse.isFull())
								break;

							// Pick a suitable Id.
							string? aid = entry.AnimalId;
							if (entry.RandomAnimalId is not null && entry.RandomAnimalId.Count > 0)
								aid = rnd.ChooseFrom(entry.RandomAnimalId);

							if (string.IsNullOrEmpty(aid) || ! validAnimals.Contains(aid) || ! animals.TryGetValue(aid, out var animalData)) {
								mod.Log($"Error spawning animal in building '{id}' for event: animal id '{aid}' is not valid", StardewModdingAPI.LogLevel.Error);
								continue;
							}

							string? askin = entry.SkinId;
							if (entry.RandomSkinId is not null && entry.RandomSkinId.Count > 0)
								askin = rnd.ChooseFrom(entry.RandomSkinId);

							if (! string.IsNullOrEmpty(askin) && (animalData.Skins is null || ! animalData.Skins.Where(s => s.Id == askin).Any())) {
								mod.Log($"Error spawning animal '{aid}' in building '{id}' for event: no such skin '{askin}'", StardewModdingAPI.LogLevel.Error);
								continue;
							}

							FarmAnimal animal = new FarmAnimal(aid, ModEntry.Instance.Helper.Multiplayer.GetNewID(), Game1.MasterPlayer.UniqueMultiplayerID);
							animal.skinID.Value = askin;

							string? aname = entry.Name;
							if (entry.RandomName is not null && entry.RandomName.Count > 0)
								aname = rnd.ChooseFrom(entry.RandomName);

							if (!string.IsNullOrEmpty(aname))
								animal.Name = Translate(aname, rnd: rnd);

							int friendship = entry.MinFriendship;
							if (entry.MaxFriendship > entry.MinFriendship)
								friendship = rnd.Next(friendship, entry.MaxFriendship);

							int happiness = entry.MinHappiness;
							if (entry.MaxHappiness > entry.MinHappiness)
								happiness = rnd.Next(happiness, entry.MaxHappiness);

							animal.friendshipTowardFarmer.Value = Math.Clamp(friendship, 0, 1000);
							animal.happiness.Value = Math.Clamp(happiness, 0, 255);

							// TODO: Support ages.
							animal.growFully();
							ahouse.adoptAnimal(animal);
						}
					}

					break;

				case PlaceableType.ResourceClump:
					var clump = new ResourceClump(
						targetData.ClumpId,
						targetData.ClumpWidth,
						targetData.ClumpHeight,
						pos,
						targetData.ClumpHealth,
						targetData.ClumpTexture
					);

					// Copy over any ModData from this spawning condition.
					if (targetData.ModData is not null)
						foreach (var entry in targetData.ModData)
							clump.modData.TryAdd(entry.Key, entry.Value);

					// Remove any features under the clump.
					for(int x = 0; x < targetSize.Value.X; x++) {
						for(int y = 0; y < targetSize.Value.Y; y++) {
							Vector2 target = new(pos.X + x, pos.Y + y);
							targetMap.terrainFeatures.Remove(target);
						}
					}

					// Add to the map.
					targetMap.resourceClumps.Add(clump);
					break;

				case PlaceableType.GiantCrop:
					// Pick a suitable Id.
					id = targetData.ItemId;
					if (targetData.RandomItemId is not null && targetData.RandomItemId.Count > 0)
						id = rnd.ChooseFrom(targetData.RandomItemId);

					var giantCrops = DataLoader.GiantCrops(Game1.content);
					if (string.IsNullOrEmpty(id) || ! giantCrops.TryGetValue(id, out var giantCropData)) {
						mod.Log($"Error loading giant crop '{id}' for event: unable to find giant crop data", StardewModdingAPI.LogLevel.Error);
						continue;
					}

					// Calculate the offset for this crop within our area.
					offsetX = (targetSize.Value.X - giantCropData.TileSize.X) / 2;
					offsetY = (targetSize.Value.Y - giantCropData.TileSize.Y) / 2;

					// Create the new crop.
					var crop = new GiantCrop(id, new Vector2(pos.X + offsetX, pos.Y + offsetY));

					// Copy over any ModData from this spawning condition.
					if (targetData.ModData is not null)
						foreach (var entry in targetData.ModData)
							crop.modData.TryAdd(entry.Key, entry.Value);

					// Add to the map.
					targetMap.resourceClumps.Add(crop);
					break;

				case PlaceableType.Crop:
					// Pick a suitable Id.
					id = targetData.ItemId;
					if (targetData.RandomItemId is not null && targetData.RandomItemId.Count > 0)
						id = rnd.ChooseFrom(targetData.RandomItemId);

					var crops = DataLoader.Crops(Game1.content);
					if (string.IsNullOrEmpty(id) || !crops.TryGetValue(id, out var cropData)) {
						mod.Log($"Error loading crop '{id}' for event: unable to find crop data", StardewModdingAPI.LogLevel.Error);
						continue;
					}

					if (targetData.IgnoreSeasons == IgnoreSeasonsMode.Never && ! targetMap.SeedsIgnoreSeasonsHere() && !(cropData.Seasons?.Contains(targetMap.GetSeason()) ?? false)) {
						mod.Log($"Unable to place crop '{id}' for event: crop is not in season and IgnoreSeasons is set to Never.", StardewModdingAPI.LogLevel.Warn);
						continue;
					}

					// We need to spawn a HoeDirt and add our crop.
					HoeDirt dirt = new HoeDirt();
					dirt.crop = new Crop(id, (int) pos.X, (int) pos.Y, targetMap);

					// TODO: Fertilizers?

					// Add the dirt now so we can do neighbor stuff.
					targetMap.terrainFeatures.Add(pos, dirt);

					// Apply speed stuff using the main farmer.
					dirt.applySpeedIncreases(Game1.MasterPlayer);

					// Now, do the paddy check.
					dirt.nearWaterForPaddy.Value = -1;
					if (dirt.hasPaddyCrop() && dirt.paddyWaterCheck()) {
						dirt.state.Value = 1;
						dirt.updateNeighbors();
					}

					// Now, see about growing the crop.
					switch (targetData.IgnoreSeasons) {
						case IgnoreSeasonsMode.Always:
							dirt.crop.modData.TryAdd(ModEntry.IGNORE_SEASON_DATA, "true");
							break;
						case IgnoreSeasonsMode.DuringSpawn:
							TreeCrop_Patches.IsSpawning = true;
							break;
					}

					growthStage = Math.Clamp(targetData.GrowthStage, -1, 4);

					if (growthStage != 0 && dirt.crop.IsInSeason(targetMap)) {
						if (growthStage == -1)
							dirt.crop.growCompletely();
						else {
							int attempts = 100;
							while (attempts-- > 0 && !dirt.crop.fullyGrown.Value && dirt.crop.currentPhase.Value < growthStage)
								dirt.crop.newDay(1);
						}
					}

					TreeCrop_Patches.IsSpawning = false;

					// Copy over any ModData from this spawning condition.
					if (targetData.ModData is not null)
						foreach (var entry in targetData.ModData)
							dirt.crop.modData.TryAdd(entry.Key, entry.Value);

					break;

				case PlaceableType.FruitTree:
				case PlaceableType.WildTree:
					// Pick a suitable Id.
					id = targetData.ItemId;
					if (targetData.RandomItemId is not null && targetData.RandomItemId.Count > 0)
						id = rnd.ChooseFrom(targetData.RandomItemId);

					bool fruitTree = targetData.Type == PlaceableType.FruitTree;

					if (!IsTreeIdValid(id, fruitTree)) { 
						mod.Log($"Error loading tree '{id}' for event: unable to find tree data", StardewModdingAPI.LogLevel.Error);
						continue;
					}

					growthStage = targetData.GrowthStage == -1
						? 4
						: Math.Clamp(targetData.GrowthStage, 0, 4);

					TerrainFeature tree = fruitTree
						? new FruitTree(id, growthStage)
						: new Tree(id, growthStage);

					switch (targetData.IgnoreSeasons) {
						case IgnoreSeasonsMode.Always:
							tree.modData.TryAdd(ModEntry.IGNORE_SEASON_DATA, "true");
							break;
						case IgnoreSeasonsMode.DuringSpawn:
							TreeCrop_Patches.IsSpawning = true;
							break;
					}

					// Perform up to 100 attempts to add fruit, if the tree is in season.
					if (tree is FruitTree ft && ft.IsInSeasonHere()) {
						int attempts = 100;
						int fruit = Math.Clamp(targetData.InitialFruit, 0, 3);

						while (attempts-- > 0 && ft.fruit.Count < fruit)
							ft.TryAddFruit();
					}

					TreeCrop_Patches.IsSpawning = false;

					// Copy over any ModData from this spawning condition.
					if (targetData.ModData is not null)
						foreach(var entry in targetData.ModData)
							tree.modData.TryAdd(entry.Key, entry.Value);

					// Add at the center of the 3x3 we checked was clear.
					targetMap.terrainFeatures.TryAdd(new Vector2(pos.X + 1, pos.Y + 1), tree);
					break;

				case PlaceableType.Item:
					Item item = ItemQueryResolver.TryResolveRandomItem(targetData, ctx, logError: (query, error) => {
						mod.Log($"Error parsing item query '{query}' for event: {error}", StardewModdingAPI.LogLevel.Error);
					});

					if (item is null)
						continue;

					if (item is not SObject sobj) {
						mod.Log($"Ignoring invalid item query '{targetData}' for placement: the resulting item isn't a placeable item.", StardewModdingAPI.LogLevel.Error);
						continue;
					}

					// TODO: Figure out a good way to just use placementAction
					// for better compatibility. We'll need harmony patches to
					// suppress error messages and sound effects, though.

					if (sobj.HasContextTag("sign_item"))
						sobj = new Sign(pos, sobj.ItemId);

					else if (sobj.HasContextTag("torch_item"))
						sobj = new Torch(sobj.ItemId, bigCraftable: true);

					else
						switch (sobj.QualifiedItemId) {
							case "(BC)62":
								sobj = new IndoorPot(pos);
								break;

							case "(BC)130":
							case "(BC)232":
								sobj = new Chest(playerChest: true, pos, sobj.ItemId) {
									Name = sobj.Name,
									shakeTimer = 50
								};
								break;

							case "(BC)BigChest":
							case "(BC)BigStoneChest":
								sobj = new Chest(playerChest: true, pos, sobj.ItemId) {
									shakeTimer = 50,
									SpecialChestType = Chest.SpecialChestTypes.BigChest
								};
								break;

							case "(BC)163":
								sobj = new Cask(pos);
								break;

							case "(BC)165":
								// Autograbber
								sobj.heldObject.Value = new Chest();
								break;

							case "(BC)208":
								sobj = new Workbench(pos);
								break;

							case "(BC)209":
								sobj = new MiniJukebox(pos);
								break;

							case "(BC)211":
								sobj = new WoodChipper(pos);
								break;

							case "(BC)214":
								sobj = new Phone(pos);
								break;

							case "(BC)216":
								Chest fridge = new Chest("216", pos, 217, 2) {
									shakeTimer = 50,
								};
								fridge.fridge.Value = true;
								sobj = fridge;
								break;

							case "(BC)248":
								sobj = new Chest(playerChest: true, pos, sobj.ItemId) {
									Name = sobj.Name,
									shakeTimer = 50,
									SpecialChestType = Chest.SpecialChestTypes.MiniShippingBin
								};
								break;

							case "(BC)256":
								sobj = new Chest(playerChest: true, pos, sobj.ItemId) {
									Name = sobj.Name,
									shakeTimer = 50,
									SpecialChestType = Chest.SpecialChestTypes.JunimoChest
								};
								break;

							case "(BC)275":
								Chest loader = new Chest(playerChest: true, pos, sobj.ItemId) {
									Name = sobj.Name,
									shakeTimer = 50,
									SpecialChestType = Chest.SpecialChestTypes.AutoLoader
								};
								loader.lidFrameCount.Value = 2;
								sobj = loader;
								break;
						}

					if (targetData.Contents != null && targetData.Contents.Count > 0) {
						List<Item> items = new();
						foreach(var entry in targetData.Contents) {
							Item i = ItemQueryResolver.TryResolveRandomItem(entry, ctx, logError: (query, error) => {
								mod.Log($"Error parsing item query '{query}' for event: {error}", StardewModdingAPI.LogLevel.Error);
							});
							if (i is not null)
								items.Add(i);
						}

						if (items.Count > 0) {
							if (sobj is StorageFurniture sorg) {
								foreach (var i in items)
									sorg.AddItem(i);
							} else if (sobj is Chest chest)
								chest.Items.AddRange(items);
						}
					}

					if (sobj is Furniture furn) {
						furn.TileLocation = pos;
						targetMap.furniture.Add(furn);

					} else if (sobj.bigCraftable.Value) {
						targetMap.Objects.TryAdd(pos, sobj);

						if (sobj is MiniJukebox jbox)
							jbox.RegisterToLocation();

					} else {
						sobj.IsSpawnedObject = true;
						targetMap.dropObject(sobj, pos * 64f, Game1.viewport, initialPlacement: true);
					}

					// Copy over any ModData from this spawning condition.
					if (targetData.ModData is not null)
						foreach (var entry in targetData.ModData)
							sobj.modData.TryAdd(entry.Key, entry.Value);

					if (targetItem is null)
						targetItem = sobj;

					break;
			}
		}

		// Finally, do our stuff.
		PerformSideEffects(targetMap, Game1.player, targetItem);

	}

	#endregion

}
