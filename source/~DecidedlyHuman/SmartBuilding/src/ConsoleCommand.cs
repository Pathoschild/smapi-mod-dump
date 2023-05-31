/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using DecidedlyShared.Logging;
using DecidedlyShared.APIs;
using SmartBuilding.Utilities;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using xTile.ObjectModel;
using Object = StardewValley.Object;

namespace SmartBuilding
{
    public class ConsoleCommand
    {
        private readonly IDynamicGameAssetsApi dgaApi;
        private readonly IdentificationUtils identificationUtils;
        private readonly Logger logger;
        private bool commandRunOnce;
        private ModEntry mod;

        public ConsoleCommand(Logger logger, ModEntry mod, IDynamicGameAssetsApi dgaApi,
            IdentificationUtils identificationUtils)
        {
            this.logger = logger;
            this.mod = mod; // This is a terrible way to access the item identification, but it'll do for now.
            this.dgaApi = dgaApi; // This is also terrible.
            this.identificationUtils = identificationUtils;
        }

        public void IdentifyItemsCommand(string command, string[] args)
        {
            if (Game1.player.Items != null && Game1.player.Items.Count > 0)
            {
                IList<Item> items = Game1.player.Items;

                foreach (var item in items)
                    if (item != null)
                        try
                        {
                            var type = this.identificationUtils.IdentifyItemType(item as Object);
                            this.logger.Log($"ItemType of {item.Name}: {type}.");

                            if (this.dgaApi?.GetDGAItemId(item) != null)
                                // This did not return null, so we know this is a DGA item.
                                this.logger.Log($"{item.Name} is a DGA item.");
                        }
                        catch (Exception e)
                        {
                            this.logger.Log($"{e.Message}");
                        }
            }
        }

        public void CountInMap(string command, string[] args)
        {
            if (args.Length < 1)
            {
                this.logger.Error("Invalid number of arguments.");
                this.logger.Error("Usage: sb_count <objects|hoedirt> [ParentSheetID].");
                this.logger.Error("Example: \"sb_count objects 645\" for counting all iridium sprinklers on the map.");
                this.logger.Error("Example: \"sb_count hoedirt\" for counting all patches of hoed dirt on the map.");

                return;
            }

            GameLocation location = Game1.currentLocation;
            int objectCount = 0;

            // Now we need to determine if we're handling objects, or HoeDirts.
            if (args[0].Equals("objects"))
            {
                // If we're querying objects, we need to know a parent sheet ID.
                if (args.Length < 2)
                {
                    this.logger.Error("Invalid number of arguments.");
                    this.logger.Error("Usage: sb_count <objects|hoedirt> [ParentSheetID].");
                    this.logger.Error(
                        "Example: \"sb_count objects 645\" for counting all iridium sprinklers on the map.");
                    this.logger.Error(
                        "Example: \"sb_count hoedirt\" for counting all patches of hoed dirt on the map.");

                    return;
                }

                if (!int.TryParse(args[1], out int parentSheetIndex))
                {
                    this.logger.Error("Couldn't parse ParentSheetID. Are you sure it's just a number?");

                    return;
                }

                int count = location.Objects.Values.Where(o => o.ParentSheetIndex == parentSheetIndex).Count();

                this.logger.Log(
                    $"Found {count.ToString()} objects with a ParentSheetIndex matching {parentSheetIndex.ToString()} in this location.");
            }
            else if (args[0].Equals("hoedirt"))
            {
                int count = location.terrainFeatures.Values.Where(tf => tf is HoeDirt).Count();

                this.logger.Log($"Found {count.ToString()} HoeDirts in this location.");
            }
        }

        public void IdentifyCursorTarget(string command, string[] args)
        {
            var here = Game1.currentLocation;
            var targetTile = Game1.currentCursorTile;

            if (here.objects.ContainsKey(targetTile))
            {
                var obj = here.objects[targetTile];
                this.logger.Log($"Object under cursor's name: {obj.Name}");
                this.logger.Log($"Object under cursor's display name: {obj.DisplayName}");
                this.logger.Log($"Object under cursor's ParentSheetIndex: {obj.ParentSheetIndex}");
                this.logger.Log($"Object under cursor's Type: {obj.Type}");
                this.logger.Log($"Object under cursor's Category: {obj.Category}");
                this.logger.Log($"Object under cursor's Fragility: {obj.Fragility}");
                this.logger.Log($"Object under cursor's isSpawnedObject: {obj.isSpawnedObject}");
                this.logger.Log($"Object under cursor's canBeGrabbed: {obj.CanBeGrabbed}");

                if (obj is Chest newChest) this.logger.Log($"Object is giftBox: {newChest.giftbox}");

                foreach (SerializableDictionary<string, string>? data in obj.modData)
                foreach (string? key in data.Keys)
                foreach (string? value in data.Values)
                    this.logger.Log($"Key: {key}, {value}");

                if (obj.heldObject != null)
                {
                    this.logger.Log($"Held object name: {obj.heldObject.Value.Name}", LogLevel.Info);
                    this.logger.Log($"Held object display name: {obj.heldObject.Value.DisplayName}", LogLevel.Info);
                    this.logger.Log($"Held object parent sheet ID: {obj.heldObject.Value.ParentSheetIndex}",
                        LogLevel.Info);

                    if (obj.heldObject.Value.heldObject.Value != null)
                    {
                        this.logger.Log($"Held object's held object name: {obj.heldObject.Value.heldObject.Value.Name}",
                            LogLevel.Info);
                        this.logger.Log(
                            $"Held object's held object display name: {obj.heldObject.Value.heldObject.Value.DisplayName}",
                            LogLevel.Info);
                        this.logger.Log(
                            $"Held object's held object parent sheet ID: {obj.heldObject.Value.heldObject.Value.ParentSheetIndex}",
                            LogLevel.Info);
                    }
                }
            }

            if (here.terrainFeatures.ContainsKey(targetTile))
            {
                var feature = here.terrainFeatures[targetTile];
                this.logger.Log($"TerrainFeature under cursor's modData: {feature.modData}");
                this.logger.Log($"TerrainFeature under cursor's isPassable: {feature.isPassable()}");
                this.logger.Log($"TerrainFeature under cursor's isActionable: {feature.isActionable()}");

                foreach (SerializableDictionary<string, string>? data in feature.modData)
                foreach (string? key in data.Keys)
                foreach (string? value in data.Values)
                    this.logger.Log($"Key: {key}, {value}");
            }
        }

        public void TestCommand(string command, string[] args)
        {
            if (!this.commandRunOnce)
            {
                this.logger.Log(
                    "THIS IS YOUR ONE AND ONLY WARNING. This command is purely for testing, may wipe your player inventory, the farm, " +
                    "spawn buildings on the farm, and perform other acts of chaos. After this warning, you will be able to use " +
                    "the command.", LogLevel.Alert);
                this.commandRunOnce = true;

                return;
            }

            string subCommand = "";

            if (args.Length <= 0)
            {
                this.logger.Log("You forgot an argument.");

                return;
            }

            var player = Game1.player;

            // We only care about the first argument, and can ignore the rest.
            subCommand = args[0];

            // Wipe the player's inventory with the vanilla "debug clear" command.
            Game1.game1.parseDebugInput("clear");

            // Max out the player's inventory size. It's over 9000.
            Game1.game1.parseDebugInput("backpack 9001");

            // Our item list
            Item[] items;

            // And figure out which subcommand we want.
            switch (subCommand)
            {
                case "notreallyplaceable":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Stone", 100),
                        Utility.fuzzyItemSearch("Wood", 100),
                        Utility.fuzzyItemSearch("Spaghetti", 100),
                        Utility.fuzzyItemSearch("Legend", 100)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "torches":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Wood Fence", 100),
                        Utility.fuzzyItemSearch("Stone Fence", 100),
                        Utility.fuzzyItemSearch("Iron Fence", 100),
                        Utility.fuzzyItemSearch("Hardwood Fence", 100),
                        Utility.fuzzyItemSearch("Torch", 100)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "rectangle":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Wood Fence", 100),
                        Utility.fuzzyItemSearch("Stone Fence", 100),
                        Utility.fuzzyItemSearch("Iron Fence", 100),
                        Utility.fuzzyItemSearch("Hardwood Fence", 100),
                        Utility.fuzzyItemSearch("Weathered Floor"),
                        Utility.fuzzyItemSearch("Straw Floor", 4),
                        Utility.fuzzyItemSearch("Brick Floor", 9),
                        Utility.fuzzyItemSearch("Stone Floor", 16),
                        Utility.fuzzyItemSearch("Crystal Floor", 25),
                        Utility.fuzzyItemSearch("Stone Walkway Floor", 36),
                        Utility.fuzzyItemSearch("Gravel Path", 49),
                        Utility.fuzzyItemSearch("Wood Path", 64),

                        Utility.fuzzyItemSearch("Torch", 100)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "morefertilizers":
                case "morefertilisers":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Joja Fertilizer", 100),
                        Utility.fuzzyItemSearch("Fish Food Fertilizer", 100),
                        Utility.fuzzyItemSearch("Fruit Tree Fertilizer", 100),
                        Utility.fuzzyItemSearch("Domesticated Fish Food Fertilizer", 100),
                        Utility.fuzzyItemSearch("Apple Sapling", 20),
                        Utility.fuzzyItemSearch("Banana Sapling", 20),
                        Utility.fuzzyItemSearch("Mango Sapling", 20),
                        Utility.fuzzyItemSearch("Peach Sapling", 20),
                        Utility.fuzzyItemSearch("Orange Sapling", 20)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    Game1.game1.parseDebugInput("build Fish9Pond");

                    break;
                case "regression":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Auto-Grabber")
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "identification":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Small FishTank"),
                        Utility.fuzzyItemSearch("Dresser"),
                        Utility.fuzzyItemSearch("Bed"),
                        Utility.fuzzyItemSearch("TV"),
                        Utility.fuzzyItemSearch("Oak Table"),
                        Utility.fuzzyItemSearch("Weathered Floor", 10),
                        Utility.fuzzyItemSearch("Chest", 10),
                        Utility.fuzzyItemSearch("Stone Chest", 10),
                        Utility.fuzzyItemSearch("Junimo Chest", 10),
                        Utility.fuzzyItemSearch("Hardwood Fence", 10),
                        Utility.fuzzyItemSearch("Iron Fence", 10),
                        Utility.fuzzyItemSearch("Grass Starter", 10),
                        Utility.fuzzyItemSearch("Crab Pot", 100),
                        Utility.fuzzyItemSearch("Parsnip Seed", 100),
                        Utility.fuzzyItemSearch("Eerie Seed", 100),
                        Utility.fuzzyItemSearch("Apple Sapling", 100),
                        Utility.fuzzyItemSearch("Acorn", 100),
                        Utility.fuzzyItemSearch("Tree Fertilizer", 100),
                        Utility.fuzzyItemSearch("Stone", 100),
                        Utility.fuzzyItemSearch("Wood", 100)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "flooring":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Weathered Floor", 100),
                        Utility.fuzzyItemSearch("Straw Floor", 100),
                        Utility.fuzzyItemSearch("Brick Floor", 100),
                        Utility.fuzzyItemSearch("Stone Floor", 100)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "furniture":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Dresser"),
                        Utility.fuzzyItemSearch("Oak Table"),
                        Utility.fuzzyItemSearch("Chair")
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "tv":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Floor TV"),
                        Utility.fuzzyItemSearch("Budget TV"),
                        Utility.fuzzyItemSearch("Plasma TV"),
                        Utility.fuzzyItemSearch("Tropical TV")
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "storage":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Birch Dresser"),
                        Utility.fuzzyItemSearch("Oak Dresser"),
                        Utility.fuzzyItemSearch("Walnut Dresser"),
                        Utility.fuzzyItemSearch("Mahogany Dresser")
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "bed":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Single Bed"),
                        Utility.fuzzyItemSearch("Double Bed"),
                        Utility.fuzzyItemSearch("Fisher Double Bed"),
                        Utility.fuzzyItemSearch("Wild Double Bed")
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "chests":
                case "chest":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Chest", 100),
                        Utility.fuzzyItemSearch("Stone Chest", 100),
                        Utility.fuzzyItemSearch("Junimo Chest", 100)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "tree":
                case "trees":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Apple Sapling", 10),
                        Utility.fuzzyItemSearch("Apricot Sapling", 10),
                        Utility.fuzzyItemSearch("Banana Sapling", 10),
                        Utility.fuzzyItemSearch("Cherry Sapling", 10),
                        Utility.fuzzyItemSearch("Maple Seed", 10),
                        Utility.fuzzyItemSearch("Acorn", 10),
                        Utility.fuzzyItemSearch("Pine Cone", 10),
                        Utility.fuzzyItemSearch("Mahogany Seed", 10),
                        Utility.fuzzyItemSearch("Tapper", 10),
                        Utility.fuzzyItemSearch("Heavy Tapper", 10),
                        Utility.fuzzyItemSearch("Tree Fertilizer", 100)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "crop":
                case "crops":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Eerie Seed", 100),
                        Utility.fuzzyItemSearch("Parsnip Seed", 100),
                        Utility.fuzzyItemSearch("Bison Seed", 100),
                        Utility.fuzzyItemSearch("Cactus Flower Seed", 100),
                        Utility.fuzzyItemSearch("Cabbage Seed", 100)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "fence":
                case "fences":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Wood Fence", 100),
                        Utility.fuzzyItemSearch("Stone Fence", 100),
                        Utility.fuzzyItemSearch("Iron Fence", 100),
                        Utility.fuzzyItemSearch("Hardwood Fence", 100)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "modded":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Cream Soda Maker", 100),
                        Utility.fuzzyItemSearch("Artisanal Soda Maker", 100),
                        Utility.fuzzyItemSearch("Alembic", 100)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
                case "insertion":
                    items = new[]
                    {
                        Utility.fuzzyItemSearch("Furnace", 100),
                        Utility.fuzzyItemSearch("Copper Ore", 100),
                        Utility.fuzzyItemSearch("Coal", 999),

                        Utility.fuzzyItemSearch("Keg", 100),
                        Utility.fuzzyItemSearch("Hops", 100),

                        Utility.fuzzyItemSearch("Cream Soda Maker", 100),
                        Utility.fuzzyItemSearch("Artisanal Soda Maker", 100),
                        Utility.fuzzyItemSearch("Carbonator", 100),

                        Utility.fuzzyItemSearch("Alembic", 100),
                        Utility.fuzzyItemSearch("Vanilla", 100),

                        Utility.fuzzyItemSearch("Strawberry", 100),
                        Utility.fuzzyItemSearch("Sugar", 100),
                        Utility.fuzzyItemSearch("Fresh Water", 100),
                        Utility.fuzzyItemSearch("Sparkling Water", 100)
                    };

                    foreach (var item in items)
                        if (item != null)
                            player.addItemToInventory(item);

                    break;
            }
        }
    }
}
