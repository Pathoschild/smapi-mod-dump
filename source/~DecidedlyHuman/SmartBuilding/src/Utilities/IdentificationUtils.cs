/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Diagnostics;
using DecidedlyShared.Logging;
using DecidedlyShared.APIs;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace SmartBuilding.Utilities
{
    public class IdentificationUtils
    {
        private readonly IDynamicGameAssetsApi? dgaApi;
        private readonly IModHelper helper;
        private readonly Logger logger;
        private ModConfig config;
        private IMoreFertilizersAPI? moreFertilizersApi;
        private IGrowableBushesAPI? growableBushesApi;
        private PlacementUtils placementUtils;

        public IdentificationUtils(IModHelper helper, Logger logger, ModConfig config, IDynamicGameAssetsApi? dgaApi,
            IMoreFertilizersAPI? moreFertilizersApi, IGrowableBushesAPI? growableBushesAPI, PlacementUtils placementUtils)
        {
            this.helper = helper;
            this.logger = logger;
            this.config = config;
            this.dgaApi = dgaApi;
            this.moreFertilizersApi = moreFertilizersApi;
            this.growableBushesApi = growableBushesAPI;
            this.placementUtils = placementUtils;
        }

        public bool IsValidPrismaticFireGem(Item gem)
        {
            switch (gem.Name)
            {
                case "Prismatic Shard":
                    return true;
                case "Amethyst":
                    return true;
                case "Ruby":
                    return true;
                case "Emerald":
                    return true;
                case "Diamond":
                    return true;
                case "Topaz":
                    return true;
            }

            return false;
        }

        public ProducerType IdentifyProducer(SObject o)
        {
            var type = ProducerType.NotAProducer;

            // If aedenthorn's Prismatic Fire mod is installed, we want to check for the presence of a torch.
            if (this.helper.ModRegistry.IsLoaded("aedenthorn.PrismaticFire"))
                if (o is Torch || o is Fence)
                {
                    // It's a torch or a fence, so we need to determine, firstly, if it's a torch.
                    if (o is Fence)
                    {
                        // It's a fence, so we grab a reference to it.
                        var fence = (Fence)o;

                        if (fence.heldObject.Value != null)
                            if (fence.heldObject.Value is Torch)
                                // It's a torch, so we return appropriately.
                                return ProducerType.TechnicallyNotAProducerButIsATorch;
                    }
                    else
                        // We know it isn't a fence, but that it is a torch, so we simply need to return appropriately.
                        return ProducerType.TechnicallyNotAProducerButIsATorch;

                    return ProducerType.NotAProducer;
                }

            if (o.Category == -9 && o.Type.Equals("Crafting"))
            {
                // We know this matches the two things all producers (both vanilla and PFM) have in common, so now we can move on to figuring out exactly what type of producer we're looking at.
                string? producerName = o.Name;

                // Now, the most efficient thing to do will be to attempt to find only the vanilla machines which do not deduct automatically, as everything else, vanilla and PFM, deducts automatically.
                switch (producerName)
                {
                    case "Mayonnaise Machine":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Preserves Jar":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Cheese Press":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Loom":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Keg":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Cask":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Oil Maker":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Crystalarium":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Recycling Machine":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Seed Maker":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Slime Incubator":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Ostrich Incubator":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Deconstructor":
                        type = ProducerType.ManualRemoval;
                        break;
                    default:
                        // At this point, we've filtered out all vanilla producers which require manual removal, so we're left with only producers, vanilla and modded, that deduct automatically.
                        type = ProducerType.AutomaticRemoval;
                        break;
                }

                return type;
            }

            return type;
        }

        public bool DoesObjectContainModData(SObject obj, string search)
        {
            if (obj != null && obj.modData != null)
                foreach (SerializableDictionary<string, string>? modData in obj.modData)
                foreach (string? key in modData.Keys)
                foreach (string? value in modData.Values)
                    if (key.Contains(search) || value.Contains(search))
                        return true;

            return false;
        }

        public bool DoesTerrainFeatureContainModData(TerrainFeature tf, string search)
        {
            var timer = new Stopwatch();

            timer.Start();
            if (tf != null && tf.modData != null)
                foreach (SerializableDictionary<string, string>? modData in tf.modData)
                foreach (string? key in modData.Keys)
                foreach (string? value in modData.Values)
                    if (key.Contains(search) || value.Contains(search))
                        return true;

            timer.Stop();

            this.logger.Log($"Took {timer.ElapsedMilliseconds}ms to search modData.", LogLevel.Trace);

            return false;
        }

        public bool IsTypeOfObject(SObject o, ItemType type)
        {
            // We try to identify what kind of object we've been passed.
            var oType = this.IdentifyItemType(o);

            return oType == type;
        }

        public ItemType IdentifyItemType(SObject item)
        {
            if (item == null)
                return ItemType.NotPlaceable;

            // Tools do not have a .Name property.
            if (item is Tool)
                return ItemType.NotPlaceable;

            // The only thing I know of to have a category of zero and a name of Chest is the starting
            // seed packet, and picking that up with Smart Building is bad.
            if (item.Category == 0 && item.Name.Equals("Chest"))
                return ItemType.NotPlaceable;

            string? itemName = item.Name;

            // The whole point of this is to determine whether the object being placed requires special treatment.
            if (item.Name.Equals("Torch") && item.Category.Equals(0) && item.Type.Equals("Crafting"))
                return ItemType.Torch;
            if (!item.isPlaceable())
                return ItemType.NotPlaceable;
            if (item is FishTankFurniture)
                return ItemType.FishTankFurniture;
            if (item is StorageFurniture)
                return ItemType.StorageFurniture;
            if (item is BedFurniture)
                return ItemType.BedFurniture;
            if (item is TV)
                return ItemType.TvFurniture;
            if (item is Furniture)
                return ItemType.GenericFurniture;
            if (itemName.Contains("Floor") || (itemName.Contains("Path") && item.Category == -24))
                return ItemType.Floor;
            if (itemName.Contains("Chest") || item is Chest)
                return ItemType.Chest;
            if (itemName.Contains("Fence"))
                return ItemType.Fence;
            if (itemName.Equals("Gate") || item.ParentSheetIndex.Equals(325))
                return ItemType.Fence;
            if (itemName.Equals("Grass Starter"))
                return ItemType.GrassStarter;
            if (itemName.Equals("Crab Pot"))
                return ItemType.CrabPot;
            if (item.Type == "Seeds" || item.Category == -74)
            {
                if (!item.Name.Contains("Sapling") && !item.Name.Equals("Acorn") && !item.Name.Equals("Maple Seed") &&
                    !item.Name.Equals("Pine Cone") && !item.Name.Equals("Mahogany Seed"))
                    return ItemType.Seed;
            }
            else if (item.Name.Equals("Tree Fertilizer"))
                return ItemType.TreeFertilizer;
            else if (item.Category == -19)
                return ItemType.Fertilizer;
            else if (item.Name.Equals("Tapper") || item.Name.Equals("Heavy Tapper"))
                return ItemType.Tapper;
            else if (item is SObject obj)
            {
                if (this.growableBushesApi!= null && this.growableBushesApi?.GetSizeOfBushIfApplicable(obj) != BushSizes.Invalid)
                    return ItemType.atravitaBush;
            }

            return ItemType.Generic;
        }

        public ItemInfo GetItemInfo(SObject item)
        {
            var itemType = this.IdentifyItemType(item);
            bool isDgaItem = false;

            if (this.dgaApi != null)
                // Check to see if the item is a DGA item.
                if (this.dgaApi.GetDGAItemId(item) != null)
                    isDgaItem = true;

            return new ItemInfo
            {
                Item = item,
                ItemType = itemType,
                IsDgaItem = isDgaItem
            };
        }

        /// <summary>
        ///     Get the flooring ID based on the item name passed in. Required for the
        ///     <see cref="StardewValley.TerrainFeatures.Flooring" /> constructor.
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public int? GetFlooringIdFromName(string itemName)
        {
            switch (itemName)
            {
                case "Wood Floor":
                    return 0; // Correct.
                case "Rustic Plank Floor":
                    return 11; // Correct.
                case "Straw Floor":
                    return 4; // Correct
                case "Weathered Floor":
                    return 2; // Correct.
                case "Crystal Floor":
                    return 3; // Correct.
                case "Stone Floor":
                    return 1; // Correct.
                case "Stone Walkway Floor":
                    return 12; // Correct.
                case "Brick Floor":
                    return 10; // Correct
                case "Wood Path":
                    return 6; // Correct.
                case "Gravel Path":
                    return 5; // Correct.
                case "Cobblestone Path":
                    return 8; // Correct.
                case "Stepping Stone Path":
                    return 9; // Correct.
                case "Crystal Path":
                    return 7; // Correct.
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Get the name of <see cref="StardewValley.TerrainFeatures.Flooring" /> based on the ID passed in.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string? GetFlooringNameFromId(int id)
        {
            switch (id)
            {
                case 0:
                    return "Wood Floor"; // Correct.
                case 11:
                    return "Rustic Plank Floor"; // Correct.
                case 4:
                    return "Straw Floor"; // Correct
                case 2:
                    return "Weathered Floor"; // Correct.
                case 3:
                    return "Crystal Floor"; // Correct.
                case 1:
                    return "Stone Floor"; // Correct.
                case 12:
                    return "Stone Walkway Floor"; // Correct.
                case 10:
                    return "Brick Floor"; // Correct
                case 6:
                    return "Wood Path"; // Correct.
                case 5:
                    return "Gravel Path"; // Correct.
                case 8:
                    return "Cobblestone Path"; // Correct.
                case 9:
                    return "Stepping Stone Path"; // Correct.
                case 7:
                    return "Crystal Path"; // Correct.
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Get the ID for the type of <see cref="StardewValley.Objects.Chest" /> passed in by name.
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public int? GetChestType(string itemName)
        {
            switch (itemName)
            {
                case "Chest":
                    return 130;
                case "Stone Chest":
                    return 232;
                case "Junimo Chest":
                    return 256;
                default:
                    return null;
            }
        }
    }
}
