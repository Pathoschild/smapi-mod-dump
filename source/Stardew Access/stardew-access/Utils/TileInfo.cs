/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using stardew_access.Translation;
using static stardew_access.Utils.MachineUtils;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;

namespace stardew_access.Utils;

public class TileInfo
{
    private static readonly string[] trackable_machines;
    private static readonly Dictionary<int, string> ResourceClumpNameTranslationKeys = [];
    private static readonly Dictionary<int, (string category, string itemName)> ParentSheetIndexes = [];
    private static readonly Dictionary<string, Dictionary<(int, int), string>> BundleLocations = [];

    private static readonly Dictionary<Color, int> colorToSelectionMap = new()
    {
        { Color.Black, 0 },
        { new(85, 85, 255), 1 },
        { new(119, 191, 255), 2 },
        { new(0, 170, 170), 3 },
        { new(0, 234, 175), 4 },
        { new(0, 170, 0), 5 },
        { new(159, 236, 0), 6 },
        { new(255, 234, 18), 7 },
        { new(255, 167, 18), 8 },
        { new(255, 105, 18), 9 },
        { new(255, 0, 0), 10 },
        { new(135, 0, 35), 11 },
        { new(255, 173, 199), 12 },
        { new(255, 117, 195), 13 },
        { new(172, 0, 198), 14 },
        { new(143, 0, 255), 15 },
        { new(89, 11, 142), 16 },
        { new(64, 64, 64), 17 },
        { new(100, 100, 100), 18 },
        { new(200, 200, 200), 19 },
        { new(254, 254, 254), 20 },
        // Add more color mappings as needed
    };

    static TileInfo()
    {
        JsonLoader.TryLoadJsonArray("trackable_machines.json", out trackable_machines, subdir: "assets/TileData");
        JsonLoader.TryLoadJsonDictionary("resource_clump_name_translation_keys.json", out ResourceClumpNameTranslationKeys, subdir: "assets/TileData");
        JsonLoader.TryLoadNestedJson<int, (string, string)>(
            "ParentSheetIndexes.json",
            ProcessParentSheetIndex,
            ref ParentSheetIndexes!,
            2,
            subdir: "assets/TileData"
        );
        JsonLoader.TryLoadNestedJson<string, Dictionary<(int, int), string>>(
            "BundleLocations.json",
            ProcessBundleLocation,
            ref BundleLocations!,
            2,
            subdir: "assets/TileData"
        );

    }

    private static void ProcessParentSheetIndex(List<string> path, JToken token, ref Dictionary<int, (string, string)> result)
    {
        string category = path[0];
        string itemName = path[1];

        if (token.Type == JTokenType.Array)
        {
            foreach (JToken indexToken in token.Children())
            {
                if (int.TryParse(indexToken.ToString(), out int index))
                {
                    result[index] = (category, itemName);
                }
                else
                {
                    Log.Warn($"Invalid index format: '{indexToken}'. Expected an integer.");
                }
            }
        }
        else
        {
            Log.Warn($"Expected an array for parent sheet indexes, but found: {token.Type}.");
        }
    }

    private static void ProcessBundleLocation(List<string> path, JToken token, ref Dictionary<string, Dictionary<(int x, int y), string>> bundleLocations)
    {
        string locationName = path[0];
        string bundleName = path[1];

        if (token.Type == JTokenType.Array && token.Children().Count() == 2)
        {
            var elements = token.Children().ToArray();
            if (int.TryParse(elements[0].ToString(), out int x) && int.TryParse(elements[1].ToString(), out int y))
            {
                if (!bundleLocations.ContainsKey(locationName))
                {
                    bundleLocations[locationName] = [];
                }

                bundleLocations[locationName][(x, y)] = bundleName;
            }
            else
            {
                Log.Warn($"Invalid coordinate format for bundle location '{locationName}'. Expected integers.");
            }
        }
        else
        {
            Log.Warn($"Expected an array of two integers for coordinates, but found: {token.Type}.");
        }
    }

    ///<summary>Returns the name of the object at tile alongwith it's category's name</summary>
    public static (string? name, string? categoryName) GetNameWithCategoryNameAtTile(Vector2 tile, GameLocation? currentLocation)
    {
        (string? name, CATEGORY? category) = GetNameWithCategoryAtTile(tile, currentLocation);

        category ??= CATEGORY.Other;

        return (name, category.ToString());
    }

    ///<summary>Returns the name of the object at tile</summary>
    public static string? GetNameAtTile(Vector2 tile, GameLocation? currentLocation = null)
    {
        currentLocation ??= Game1.currentLocation;
        return GetNameWithCategoryAtTile(tile, currentLocation).name;
    }

    public static string GetNameAtTileWithBlockedOrEmptyIndication(Vector2 tile)
    {
        String? name = GetNameAtTile(tile);

        // Prepend the player's name if the viewing tile is occupied by the player itself
        if (CurrentPlayer.PositionX == (int)tile.X && CurrentPlayer.PositionY == (int)tile.Y)
        {
            name = $"{Game1.player.displayName}, {name}";
        }

        // Report if a tile is empty or blocked if there is nothing on it
        return name ?? Translator.Instance.Translate(
            IsCollidingAtTile(Game1.currentLocation, (int)tile.X, (int)tile.Y)
                ? "feature-tile_viewer-blocked_tile_name"
                : "feature-tile_viewer-empty_tile_name");
    }

    ///<summary>Returns the name of the object at tile alongwith it's category</summary>
    public static (string? name, CATEGORY? category) GetNameWithCategoryAtTile(Vector2 tile, GameLocation? currentLocation, bool lessInfo = false)
    {
        var (name, category) = GetTranslationKeyWithCategoryAtTile(tile, currentLocation, lessInfo);
        if (name == null)
            return (null, CATEGORY.Other);

        return (Translator.Instance.Translate(name, disableWarning: true), category);
    }

    public static (string? name, CATEGORY? category) GetTranslationKeyWithCategoryAtTile(Vector2 tile, GameLocation? currentLocation, bool lessInfo = false)
    {
        currentLocation ??= Game1.currentLocation;
        int x = (int)tile.X;
        int y = (int)tile.Y;

        var terrainFeature = currentLocation.terrainFeatures.FieldDict;

        if (currentLocation.isCharacterAtTile(tile) is NPC npc)
        {
            CATEGORY category = npc.IsVillager || npc.CanSocialize ? CATEGORY.Farmers : CATEGORY.NPCs;
            string npcName;
            if (npc is Horse horse)
            {
                if (string.IsNullOrEmpty(horse.displayName))
                {
                    npcName = Translator.Instance.Translate("npc_name-horse_with_no_name");
                }
                else
                {
                    npcName = horse.displayName;
                }
            }
            else
            {
                npcName = npc.displayName;
            }

            return (npcName, category);
        }

        string? farmAnimal = GetFarmAnimalAt(currentLocation, x, y);
        if (farmAnimal is not null)
        {
            return (farmAnimal, CATEGORY.Animals);
        }

        string? door = GetDoorAtTile(currentLocation, x, y);
        if (door != null)
        {
            return (door, CATEGORY.Doors);
        }

        (string name, CATEGORY category)? staticTile = MainClass.TileManager.GetNameAndCategoryAt((x, y), "user", currentLocation);
        staticTile ??= MainClass.TileManager.GetNameAndCategoryAt((x, y), "stardew-access", currentLocation);
        if (staticTile is { } static_tile)
        {
#if DEBUG
            Log.Verbose($"TileInfo: Got static tile {static_tile} from TileManager");
#endif
            return (static_tile.name, static_tile.category);
        }

        (string? name, CATEGORY? category) dynamicTile = DynamicTiles.GetDynamicTileAt(currentLocation, x, y, lessInfo);
        if (dynamicTile.name != null)
        {
            return (dynamicTile.name, dynamicTile.category);
        }

        if (currentLocation.isObjectAtTile(x, y))
        {
            (string? name, CATEGORY? category) = GetObjectAtTile(currentLocation, x, y, lessInfo);
            return (name, category);
        }

        if (currentLocation.isWaterTile(x, y) && !lessInfo && IsCollidingAtTile(currentLocation, x, y))
        {
            return ("tile-water-name", CATEGORY.Water);
        }

        string? resourceClump = GetResourceClumpAtTile(currentLocation, x, y, lessInfo);
        if (resourceClump != null)
        {
            return (resourceClump, CATEGORY.ResourceClumps);
        }

        LargeTerrainFeature? ltf = currentLocation.getLargeTerrainFeatureAt(x, y);
        (string? name, CATEGORY? category) ltfInfo = (TerrainUtils.GetTerrainFeatureInfoAndCategory(ltf, lessInfo));
        if (ltfInfo.name != null)
        {
            if (ltf is Tent tent && (int)tent.Tile.X == x && (int)tent.Tile.Y == y)
            {
                ltfInfo.name = Translator.Instance.Translate("terrain_util-tent_entrance");
                ltfInfo.category = CATEGORY.Interactables;
            }
            return ltfInfo;
        }

        if (terrainFeature.TryGetValue(tile, out var tf))
        {
            (string? name, CATEGORY? category) tfInfo = (TerrainUtils.GetTerrainFeatureInfoAndCategory(tf.Value, lessInfo));
            if (tfInfo.name != null)
            {
                return tfInfo;
            }
        }

        string? junimoBundle = GetJunimoBundleAt(currentLocation, x, y);
        if (junimoBundle != null)
        {
            return (junimoBundle, CATEGORY.Bundles);
        }

        // Track dropped items
        if (MainClass.Config.TrackDroppedItems)
        {
            try
            {
                foreach (var item in currentLocation.debris)
                {
                    if (item.Chunks.Count <= 0) continue;

                    int xPos = ((int)item.Chunks[0].position.Value.X / Game1.tileSize) + 1;
                    int yPos = ((int)item.Chunks[0].position.Value.Y / Game1.tileSize) + 1;
                    if (xPos != x || yPos != y) continue;

                    string name = item.item is null || string.IsNullOrWhiteSpace(item.item.DisplayName)
                        ? TokenParser.ParseText(ObjectUtils.GetObjectById(item.itemId.Value)?.DisplayName) ?? ""
                        : item.item.DisplayName;
                    int count = item.item is null ? item.Chunks.Count : item.item.Stack;

                    if (string.IsNullOrWhiteSpace(name)) continue;

                    return (Translator.Instance.Translate("item-dropped_item-info", new { item_count = count, item_name = name }), CATEGORY.DroppedItems);
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred while detecting dropped items:\n{e.StackTrace}");
            }
        }

        return (null, CATEGORY.Other);
    }

    /// <summary>
    /// Determines if there is a Junimo bundle at the specified tile coordinates in the provided GameLocation.
    /// </summary>
    /// <param name="currentLocation">The GameLocation instance to search for Junimo bundles.</param>
    /// <param name="x">The x-coordinate of the tile to check.</param>
    /// <param name="y">The y-coordinate of the tile to check.</param>
    /// <returns>The name of the Junimo bundle if one is found at the specified coordinates, otherwise null.</returns>
    public static string? GetJunimoBundleAt(GameLocation currentLocation, int x, int y)
    {
        string locationName = currentLocation.NameOrUniqueName;

        if (BundleLocations.TryGetValue(locationName, out Dictionary<(int, int), string>? bundleCoords))
        {
            if (bundleCoords.TryGetValue((x, y), out string? bundleName))
            {
                if (currentLocation is CommunityCenter communityCenter)
                {
                    if (communityCenter.shouldNoteAppearInArea(CommunityCenter.getAreaNumberFromName(bundleName)))
                    {
                        return Translator.Instance.Translate("tile-bundles-suffix", new { content = bundleName });
                    }
                }
                else if (currentLocation is AbandonedJojaMart)
                {
                    return Translator.Instance.Translate("tile-bundles-suffix", new { content = bundleName });
                }
            }
        }

        return null;  // No bundle was found
    }

    /// <summary>
    /// Determines if there is a collision at the specified tile coordinates in the provided GameLocation.
    /// </summary>
    /// <param name="currentLocation">The GameLocation instance to search for collisions.</param>
    /// <param name="x">The x-coordinate of the tile to check.</param>
    /// <param name="y">The y-coordinate of the tile to check.</param>
    /// <returns>True if a collision is detected at the specified tile coordinates, otherwise False.</returns>
    public static bool IsCollidingAtTile(GameLocation currentLocation, int x, int y, bool lessInfo = false)
    {
        // This function highly optimized over readability because `currentLocation.isCollidingPosition` takes ~30ms on the Farm map, more on larger maps I.E. Forest.
        // Return the result of the logical comparison directly, inlining operations
        // Check if the tile is NOT a warp point and if it collides with an object or terrain feature
        // OR if the tile has stumps in a Woods location
        return !DoorUtils.IsWarpAtTile((x, y), currentLocation) &&
               (currentLocation.isCollidingPosition(new Rectangle(x * 64 + 1, y * 64 + 1, 62, 62), Game1.viewport, true, 0, glider: false, Game1.player, pathfinding: true));
    }

    /// <summary>
    /// Gets the farm animal at the specified tile coordinates in the given location.
    /// </summary>
    /// <param name="location">The location where the farm animal might be found. Must be either a Farm or an AnimalHouse (coop, barn, etc).</param>
    /// <param name="x">The x-coordinate of the tile to check.</param>
    /// <param name="y">The y-coordinate of the tile to check.</param>
    /// <returns>
    /// A string containing the farm animal's name, type, and age if a farm animal is found at the specified tile;
    /// null if no farm animal is found or if the location is not a Farm or an AnimalHouse.
    /// </returns>
    public static string? GetFarmAnimalAt(GameLocation location, int x, int y)
    {
        Dictionary<(int x, int y), FarmAnimal>? animalsByCoordinate = AnimalUtils.GetAnimalsByLocation(location);

        if (animalsByCoordinate == null || !animalsByCoordinate.TryGetValue((x, y), out FarmAnimal? foundAnimal))
            return null;

        string name = foundAnimal.displayName;
        int age = (foundAnimal.GetDaysOwned() + 1) / 28 + 1;
        string type = foundAnimal.displayType;

        object? translationCategory = new
        {
            name,
            type,
            age
        };
        return Translator.Instance.Translate("npc-farm_animal_info", translationCategory);
    }

    /// <summary>
    /// Retrieves the name and category of the terrain feature at the given tile.
    /// </summary>
    /// <param name="terrain">A reference to the terrain feature to be checked.</param>
    /// <returns>A tuple containing the name and category of the terrain feature at the tile.</returns>
    public static (string? name, CATEGORY? category) GetTerrainFeatureAtTile(Netcode.NetRef<TerrainFeature> terrain)
    {
        // Get the terrain feature from the reference
        var terrainFeature = terrain.Get();
        return TerrainUtils.GetTerrainFeatureInfoAndCategory(terrainFeature,
                Game1.currentLocation is MineShaft && !MainClass.Config.ReadHoedDirtInMineShafts);
    }
    #region Objects

    private static int GetSelectionFromColor(Color c)
    {
        if (colorToSelectionMap.TryGetValue(c, out int selection))
        {
            return selection;
        }
        return -1; // Color not found
    }

    internal static string GetChestColorAndName(Chest chest)
    {
        int colorIndex = GetSelectionFromColor(chest.playerChoiceColor.Get());
        Log.Trace($"colorIndex is {colorIndex}");
        string chestColor = (!chest.playerChest.Value || colorIndex <= 0)
            ? ""
            : Translator.Instance.Translate("menu-item_grab-chest_colors",
                new { index = colorIndex, is_selected = 0 }, TranslationCategory.Menu);
        Log.Trace($"chestColor is {chestColor}");
        Log.Trace($"chest.displayName is {chest.DisplayName}");
        string displayName = chest.giftbox.Value
        ? (chest.giftboxIsStarterGift.Value ? "Starter Gift" : "Giftbox")
        : (chest.fridge.Value
        ? "Fridge"
        : (chest.playerChest.Value
        ? chest.DisplayName
        : chest.Name));
        return $"{chestColor} {displayName}";
    }

    /// <summary>
    /// Retrieves the name and category of an object at a specific tile in the game location.
    /// </summary>
    /// <param name="currentLocation">The current game location.</param>
    /// <param name="x">The X coordinate of the tile.</param>
    /// <param name="y">The Y coordinate of the tile.</param>
    /// <param name="lessInfo">An optional parameter to display less information, set to false by default.</param>
    /// <returns>A tuple containing the object's name and category.</returns>
    public static (string? name, CATEGORY category) GetObjectAtTile(GameLocation currentLocation, int x, int y, bool lessInfo = false)
    {
        (string? name, CATEGORY category) toReturn = (null, CATEGORY.Other);

        // Get the object at the specified tile
        StardewValley.Object obj = currentLocation.getObjectAtTile(x, y);
        if (obj == null) return toReturn;

        int index = obj.ParentSheetIndex;
        toReturn.name = obj.DisplayName;

        // TODO Update this to use the QualifiedItemIds instead
        // Get object names and categories based on index
        (string? name, CATEGORY category) correctNameAndCategory = GetCorrectNameAndCategoryFromIndex(index);

        // Check the object type and assign the appropriate name and category
        if (obj is Chest chest)
        {
            string displayColorAndName = GetChestColorAndName(chest);
            toReturn = (displayColorAndName, CATEGORY.Containers);
        }
        else if (obj.ItemId == "TextSign")
        {
            if (!string.IsNullOrWhiteSpace(obj.signText.Value))
            {
                toReturn.name = $"{toReturn.name}: {obj.signText.Value}";
            }
            toReturn.category = CATEGORY.Interactables;
        }
        else if (obj is Mannequin mannequin)
        {
            string itemsOnDisplay = string.Join(", ", new List<string>()
            {
                mannequin.hat.Value?.DisplayName ?? "",
                mannequin.shirt.Value?.DisplayName ?? "",
                mannequin.pants.Value?.DisplayName ?? "",
                mannequin.boots.Value?.DisplayName ?? ""
            }.Where(i => !string.IsNullOrWhiteSpace(i)));

            toReturn.name = $"{obj.DisplayName}" + (string.IsNullOrWhiteSpace(itemsOnDisplay) ? "" : $", {itemsOnDisplay}");
        }
        else if (obj is IndoorPot indoorPot)
        {
            string potContent = indoorPot.bush.Value != null
                ? TerrainUtils.GetBushInfoString(indoorPot.bush.Value)
                : TerrainUtils.GetDirtInfoString(indoorPot.hoeDirt.Value, true);
            toReturn.name = $"{obj.DisplayName}, {potContent}";
        }
        else if (obj is Sign sign && sign.displayItem.Value != null)
        {
            toReturn.name = $"{sign.DisplayName}, {sign.displayItem.Value.DisplayName}";
        }
        else if (obj is Furniture furniture)
        {
            if (lessInfo && (furniture.TileLocation.X != x || furniture.TileLocation.Y != y))
            {
                toReturn.category = CATEGORY.Other;
                toReturn.name = null;
            }
            else
            {
                toReturn.category = CATEGORY.Furniture;
            }
        }
        else if (obj is Torch torch)
        {
            if (obj.QualifiedItemId == "(BC)146")
            {
                toReturn.name = Translator.Instance.Translate("static_tile-mountain-linus_campfire", TranslationCategory.StaticTiles);
                toReturn.category = CATEGORY.Decor;
            }
            else if (obj.QualifiedItemId == "(BC)278")
            {
                toReturn.category = CATEGORY.Interactables;
            }
        }
        else if (obj.IsSprinkler() && obj.heldObject.Value != null) // Detect the upgrade attached to the sprinkler
        {
            string heldObjectName = obj.heldObject.Value.Name;
            if (MainClass.ModHelper is not null)
            {
                if (heldObjectName.Contains("pressure nozzle", StringComparison.OrdinalIgnoreCase))
                {
                    toReturn.name = Translator.Instance.Translate("tile-sprinkler-pressure_nozzle-prefix", new { content = toReturn.name });
                }
                else if (heldObjectName.Contains("enricher", StringComparison.OrdinalIgnoreCase))
                {
                    toReturn.name = Translator.Instance.Translate("tile-sprinkler-enricher-prefix", new { content = toReturn.name });
                }
                else
                {
                    toReturn.name = $"{obj.heldObject.Value.DisplayName} {toReturn.name}";
                }
            }
        }
        else if ((obj.Type == "Crafting" && obj.bigCraftable.Value) || obj.Name.Equals("crab pot", StringComparison.OrdinalIgnoreCase))
        {
            // TODO optimize this
            foreach (string machine in trackable_machines)
            {
                if (obj.Name.Contains(machine, StringComparison.OrdinalIgnoreCase))
                {
                    MachineState machineState = GetMachineState(obj);
                    if (obj.heldObject.Value is not null)
                    {
                        toReturn.name = $"{obj.DisplayName}, {InventoryUtils.GetItemDetails(obj.heldObject.Value)}";
                        toReturn.category = (machineState == MachineState.Busy) ? CATEGORY.Machines : CATEGORY.Ready;
                    }
                    else
                    {
                        toReturn.name = obj.DisplayName;
                        toReturn.category = CATEGORY.Machines;
                    }
                }
            }
        }
        else if (obj is Fence fence && fence.isGate.Value)
        {
            // The `gatePosition` only has two values, 0 (indicating gate is closed) and 88.
            toReturn.name = Translator.Instance.Translate("tile-fence_gate-suffix", new
            {
                is_open = fence.gatePosition.Value != 0 ? 1 : 0,
                less_info = lessInfo ? 1 : 0,
                toReturn.name // using inferred member name; silences IDE0037
            });
            toReturn.category = CATEGORY.Doors;
        }
        else if (correctNameAndCategory.name != null && !obj.ItemId.Contains("GreenRainWeeds"))
        {
            toReturn = correctNameAndCategory;
        }
        else if (obj.name.Equals("stone", StringComparison.OrdinalIgnoreCase))
            toReturn.category = CATEGORY.Debris;
        else if (obj.name.Equals("twig", StringComparison.OrdinalIgnoreCase))
            toReturn.category = CATEGORY.Debris;
        else if (obj.name.Contains("quartz", StringComparison.OrdinalIgnoreCase))
            toReturn.category = CATEGORY.MineItems;
        else if (obj.name.Contains("earth crystal", StringComparison.OrdinalIgnoreCase))
            toReturn.category = CATEGORY.MineItems;
        else if (obj.name.Contains("frozen tear", StringComparison.OrdinalIgnoreCase))
            toReturn.category = CATEGORY.MineItems;

        if (toReturn.category == CATEGORY.Machines || toReturn.category == CATEGORY.Ready || toReturn.category == CATEGORY.Pending) // Fix for `Harvestable table` and `Busy nodes`
        {
            MachineState machineState = GetMachineState(obj);
            if (machineState == MachineState.Ready)
                toReturn.name = Translator.Instance.Translate("tile-harvestable-prefix", new { content = toReturn.name });
            else if (machineState == MachineState.Busy)
                toReturn.name = Translator.Instance.Translate("tile-busy-prefix", new { content = toReturn.name });
        }

        if (MainClass.Config.ReadTileDebug)
        {
            if (!string.IsNullOrEmpty(obj.QualifiedItemId))
                toReturn.name = $"{toReturn.name} ({obj.QualifiedItemId})";
            Log.Trace($"Owner is {obj.owner}", true);
            Farmer farmerOwner = Game1.getFarmerMaybeOffline(obj.owner.Value);
            string ownerName;
            if (farmerOwner == null)
                ownerName = "";
            else if (farmerOwner.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
                ownerName = "you";
            else
                ownerName = farmerOwner.Name;
            if (!string.IsNullOrEmpty(ownerName))
                toReturn.name = $"{toReturn.name} owned by {ownerName}";
        }
        return toReturn;
    }

    /// <summary>
    /// Retrieves the correct name and category for an object based on its index and name.
    /// </summary>
    /// <param name="index">The object's index value.</param>
    /// <param name="objName">The object's name.</param>
    /// <returns>A tuple containing the object's correct name and category.</returns>
    private static (string? name, CATEGORY category) GetCorrectNameAndCategoryFromIndex(int index)
    {
        // Use the ParentSheetIndexes dictionary for fast lookups.
        if (ParentSheetIndexes.TryGetValue(index, out var info))
        {
            return (info.itemName, CATEGORY.FromString(info.category));
        }

        // If the index is not found in the ParentSheetIndexes dictionary, return the Others category.
        return (null, CATEGORY.Other);
    }
    #endregion  

    /// <summary>
    /// Gets the door information at the specified tile coordinates in the given location.
    /// </summary>
    /// <param name="currentLocation">The GameLocation where the door might be found.</param>
    /// <param name="x">The x-coordinate of the tile to check.</param>
    /// <param name="y">The y-coordinate of the tile to check.</param>
    /// <returns>A string containing the door information if a door is found at the specified tile; null if no door is found.</returns>
    public static string? GetDoorAtTile(GameLocation currentLocation, int x, int y, bool lessInfo = false, bool ignoreWarps = false)
    {
        var doors = ignoreWarps ? DoorUtils.GetDoors(Game1.currentLocation, lessInfo) : DoorUtils.GetAllDoors(Game1.currentLocation, lessInfo);
        if (doors.TryGetValue((x, y), out var doorName))
        {
            return doorName!;
        }
        return null;
    }

    /// <summary>
    /// Gets the resource clump information at the specified tile coordinates in the given location.
    /// </summary>
    /// <param name="currentLocation">The GameLocation where the resource clump might be found.</param>
    /// <param name="x">The x-coordinate of the tile to check.</param>
    /// <param name="y">The y-coordinate of the tile to check.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A string containing the resource clump information if a resource clump is found at the specified tile; null if no resource clump is found.</returns>
    public static string? GetResourceClumpAtTile(GameLocation currentLocation, int x, int y, bool lessInfo = false)
    {
        // Get the dictionary of resource clumps (this includes stumps in woods)
        Dictionary<(int x, int y), ResourceClump>? resourceClumpsByCoordinate = ResourceClumpUtils.GetResourceClumpsAtLocation(currentLocation);

        // Check if there's a resource clump at the given coordinates
        if (resourceClumpsByCoordinate?.TryGetValue((x, y), out ResourceClump? resourceClump) == true)
        {
            // Check if lessInfo condition is met
            if (!lessInfo || ((int)resourceClump.Tile.X == x && (int)resourceClump.Tile.Y == y))
            {
                // Return the name of the resource clump or "Unknown" if not available
                if (ResourceClumpNameTranslationKeys.TryGetValue(resourceClump.parentSheetIndex.Value, out string? translationKey))
                {
                    return translationKey;
                }
                else
                {
                    // Log the missing translation key and some info about the clump
                    Log.Warn($"Missing translation key for resource clump with parentSheetIndex {resourceClump.parentSheetIndex.Value}.", true);
                    return Translator.Instance.Translate("tile-resource_clump-unknown", new { id = resourceClump.parentSheetIndex.Value });
                }
            }
        }

        // No matching resource clump found
        return null;
    }
}
